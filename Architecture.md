# MazEscaper: ściągawka z architektury i konwencji

Dokument dla całej gry, nie tylko prototypu. Odpowiada na pytania: gdzie umieścić skrypt, jak go nazwać, kiedy zrobić interfejs, kto kogo widzi i za co odpowiada każda warstwa. Nie opisuje konkretnych plików, bo będzie ich dużo i zmieniają się szybciej niż zasady.

---

## 1. Zasady nadrzędne

1. **Logika w czystym C#.** Reguły gry nie znają Unity. Dzięki temu testujemy je bez edytora i możemy symulować tysiące partii.
2. **Widoki tylko wyświetlają.** Czytają stan i zgłaszają kliknięcia. Nie zmieniają stanu i nie liczą reguł.
3. **Stan to dane, reguły to systemy.** Stan niczego nie decyduje. Systemy liczą i zmieniają stan, dostając go jako argument.
4. **Każda zmiana stanu to komenda.** Gracz (i bot) coś robi tylko przez komendę.
5. **Liczby balansu w jednym miejscu.** Config, nie kod. Żadnych magicznych liczb.
6. **Jedno źródło reguły.** Jeśli dwa miejsca liczą to samo, wołają tę samą funkcję, nie dwa osobne wzory.
7. **Treść w danych.** Cechy, statusy, wydarzenia, sektory, przedmioty to definicje, nie kod.
8. **Losowość z seedem.** Jeden generator, seed w logu, powtarzalny błąd.

---

## 2. Foldery

```
MazEscaper/
├─ Docs/              dokumenty projektowe (poza Assets/, bez plików .meta)
├─ Packages/
├─ ProjectSettings/
└─ Assets/
   ├─ Core/           logika gry (czysty C#)
   │  ├─ Enums/       enumy
   │  ├─ Commands/    akcje gracza
   │  └─ Systems/     reguły gry
   ├─ Data/           ScriptableObjecty i ich pliki .asset
   ├─ UI/             wszystko, co widać i co dotyka Unity
   ├─ Tests/          testy EditMode
   └─ TextMesh Pro/   tworzy Unity, nie ruszać
```

### Co wchodzi do którego folderu

| Folder | Co tu leży | Czego tu nie wolno |
|---|---|---|
| `Core/` (luzem) | Stan gry i model, raporty i wyniki obliczeń, konfiguracja, interfejsy usług i ich czyste implementacje | Wszystko, co dotyka Unity |
| `Core/Enums/` | Enumy opisujące pojęcia z reguł gry | Logika |
| `Core/Commands/` | Interfejs komendy i komendy. Jedna klasa = jedna akcja gracza | Liczenie (idzie do systemów) |
| `Core/Systems/` | Statyczne klasy z regułami: systemy i klasy zasad | Zmienny stan własny |
| `Data/` | Skrypty ScriptableObject i pliki `.asset` z treścią i configiem | Logika gry |
| `UI/` | Kontrolery, widoki, implementacje usług opartych o Unity, prefaby, sceny | Reguły gry, zmiana stanu |
| `Tests/` | Testy logiki z Core | Testy zależne od sceny |
| `Docs/` | GDD, plany, ten plik | Pliki, które mają trafić do gry |

### Rozrost: kiedy dzielić folder

Płasko, dopóki nie zrobi się ciasno (około 15-20 plików w jednym folderze). Wtedy dodaj podfolder o nazwie mówiącej, co w nim jest. Typowe kierunki:

- `Core/` → `Model/` (stan i dane), `Definitions/` (szablony rzeczy), `Services/` (interfejsy usług), `Config/`.
- `UI/` → `Controllers/`, `Views/`, `Prefabs/`, `Scenes/`, `Services/`.
- `Data/` → `Content/` z podfolderem na każdy typ treści.
- `Tests/` → lustro struktury Core.

Reguła dla namespace'ów: podfolder nie zmienia namespace'a. Folder jest do porządku.

---

## 3. Assembly i kto kogo widzi

| Assembly | Widzi | Nie widzi |
|---|---|---|
| `MazEscaper.Core` | nikogo (No Engine References = ON) | Unity, Data, UI |
| `MazEscaper.Data` | Core | UI |
| `MazEscaper.UI` | Core, Data, TextMeshPro, UnityEngine.UI | Tests |
| `MazEscaper.Tests` | Core, Data, TestRunner, NUnit (tylko Editor) | UI |

```
UI ──► Data ──► Core
UI ──► Core
Tests ──► Core, Data
```

Zasady:

- **Strzałki idą w jedną stronę.** Core nie zna nikogo, nikt nie zna UI.
- Ustawienia wspólne: Auto Referenced = OFF dla wszystkich.
- Test rozdziału: `using UnityEngine;` w Core ma się nie kompilować. Jeśli się kompiluje, asmdef Core jest źle ustawiony.
- **Nowe assembly** zakładaj tylko wtedy, gdy kod ma osobny cel i osobne zależności, np. symulacje i bot (widzą tylko Core) albo narzędzia edytora (tylko Editor). Nie dziel Core na wiele assembly bez konkretnego powodu.
- Nazwa assembly (`MazEscaper.Core`) to nie namespace (`Core`).

---

## 4. Odpowiedzialność warstw

### Stan i model (Core)

- Same dane gry: to, co byłoby zapisane w pliku zapisu.
- Bez decyzji i reguł. Dopuszczalne tylko niezmienniki danych (np. pojemność, której nie wolno przekroczyć).
- Zmienia go wyłącznie system lub komenda, nigdy widok.
- Wyniki obliczeń i raporty to osobne klasy tylko do odczytu, ustawiane w konstruktorze.
- Dane pochodne (prognozy, sumy do wyświetlenia) nie trafiają do stanu. Liczysz je na bieżąco.
- Dane opisujące zdarzenie, które już się wydarzyło (np. raport dnia), mogą być zapisane w stanie.

### Systemy (Core/Systems)

- Klasy statyczne, bez zmiennego stanu własnego. Co trzeba zapamiętać, idzie do stanu.
- Dostają stan jako argument. Config bierzesz ze stanu, nie osobnym parametrem.
- **Obliczenie jest oddzielone od zastosowania.** Metoda obliczająca nic nie zmienia (`Calculate...`, `Preview...`), metoda zmieniająca stan to `Apply...` albo czasownik. Dzięki temu UI, prognozy i testy mogą bezpiecznie liczyć "co by było".
- Zasady (klasy `...Rules`) to czyste funkcje na liczbach i configu. Wołają je zarówno systemy, jak i prognozy.
- System fazy dnia lub zdarzenia jest wywoływany z jednego miejsca (przełącznik fazy), nie rozproszony po innych klasach.
- Nie dodawaj wspólnego interfejsu dla systemów, bo nie mają wspólnego zachowania.

### Komendy (Core/Commands)

- Jedna klasa to jedna akcja gracza. Dane komendy (kto, gdzie, ile) trafiają do konstruktora, stan dopiero do `Execute`.
- Komenda nie liczy. Woła system. Zwykle to jedna linijka.
- To zwykłe klasy (nie statyczne), bo implementują interfejs i niosą dane.
- Cofanie: osobny interfejs tylko dla komend, które da się cofnąć. Komenda, której skutków nie da się odwrócić (losowanie, rozliczenie dnia), go nie dostaje.

### Config i dane (Core + Data)

- Liczby balansu: jedna klasa konfiguracji w Core z wartościami domyślnymi (testy działają bez Unity). Przy każdym polu komentarz ze źródłem (numer rozdziału GDD albo `ROBOCZE`).
- Treść gry (cechy, statusy, wydarzenia, sektory, przedmioty): czysta definicja w Core + skrypt ScriptableObject w Data, który zwraca ją metodą `ToDefinition()`.
- Core nigdy nie widzi ScriptableObjectów, tylko zwrócone przez nie czyste obiekty.
- Asset konfiguracji zwraca kopię, żeby gra nie nadpisywała pliku.
- Wartość z assetu wygrywa nad domyślną z kodu. Po zmianie wartości w kodzie zresetuj asset.
- Nie twórz własnej konfiguracji w kodzie gry (wyjątek: testy).

### Kontroler (UI)

- Łączy świat Unity z Core: tworzy stan, wykonuje komendy, po każdej komendzie odświeża widoki.
- Jedyne miejsce, które trzyma referencję do assetu konfiguracji i losuje seed.
- Nie zna szczegółów żadnego widoku, tylko woła ich wspólną metodę wyświetlania.
- Mało kontrolerów, za to "grubych". Jeden na prototyp, podział per ekran dopiero, gdy ekranów jest wiele.

### Widoki (UI)

- **Odpowiadają wyłącznie za wyświetlanie.** Dostają stan, rysują swój fragment ekranu, zgłaszają kliknięcia.
- Wszystkie dziedziczą po jednej klasie bazowej z jedną metodą wyświetlania stanu.
- Prosta etykieta ma własną bazę (zwraca tekst, baza wpisuje go do pola). Widok z kilkoma polami lub kolorem dziedziczy bezpośrednio po klasie bazowej widoków.
- Kontenery trzymają widoki podrzędne i tylko przekazują im stan.
- Widok może wywołać **czystą** funkcję obliczającą (np. prognozę), ale nigdy funkcji, która zmienia stan.
- Tłumaczenie enumów i identyfikatorów na tekst dla gracza robi widok. Kolory trzymaj w polach Inspectora.
- Widok musi radzić sobie z danymi, które jeszcze nie istnieją (np. brak pierwszego raportu).

---

## 5. Przepływ danych

```
klik w UI ─► widok ─► kontroler ─► komenda.Execute(stan)
                                        │
                                        ▼
                    system zmienia stan (używa configu i generatora losowości)
                                        │
                                        ▼
kontroler odświeża widoki ◄── widoki czytają stan
```

- Zmiana stanu zawsze przez komendę.
- Core nie wie, że widoki istnieją. Kontroler odświeża je po komendzie, a nie Core.
- Gdy stan zmienia się bez komendy gracza (np. czasomierz), dodaj w Core zdarzenie "stan się zmienił", do którego kontroler się zapisze. Nie wołaj widoków z Core.

---

## 6. Kiedy tworzyć interfejs

**Tak, gdy:**

1. Jest kod, który traktuje wiele implementacji jednakowo (wszystkie komendy wykonywane tym samym kodem, bot i gracz).
2. Core potrzebuje czegoś z świata Unity: logowania, czasu, wejścia, zapisu na dysk. Interfejs leży w Core, implementacja w UI.
3. Chcesz podmienić implementację w testach lub symulacji.

**Nie, gdy:**

- jest jedna implementacja i nic jej nie podmienia,
- klasy nie mają wspólnego zachowania (np. ogólny `ISystem` jest bezużyteczny),
- chodzi o dane (stan, raporty, definicje).

Zasady:

- Nazwa interfejsu mówi, co robi (`ICommand`, `IRandom`), prefiks `I`.
- Interfejs zawiera tylko to, co wspólne dla wszystkich implementacji. Dodatkowe możliwości (np. cofanie) to osobny, węższy interfejs.
- Nie dodawaj metod na zapas.

---

## 7. Gdzie położyć nowy skrypt: pytania po kolei

| Pytanie | Jeśli tak |
|---|---|
| Używa Unity (`MonoBehaviour`, `Debug`, `Time`, `Input`, UI)? | `UI/` (lub `Data/` dla ScriptableObjectów) |
| Tylko wyświetla stan i zgłasza kliknięcia? | Widok w `UI/` |
| Łączy Unity z logiką (tworzy stan, wykonuje komendy)? | Kontroler w `UI/` |
| Jest akcją gracza? | Komenda w `Core/Commands/` + system, który ją realizuje |
| Liczy albo zmienia stan wg reguł gry? | System lub klasa zasad w `Core/Systems/` |
| Trzyma dane gry, które byłyby zapisane? | Stan lub model w `Core/` |
| Jest wynikiem obliczenia do pokazania? | Klasa wyniku w `Core/` (tylko do odczytu) |
| Jest liczbą balansu? | Pole w konfiguracji |
| Jest treścią (cecha, status, wydarzenie)? | Definicja w `Core/` + SO i `.asset` w `Data/` |
| Jest pojęciem z reguł (faza, typ zasobu)? | Enum w `Core/Enums/` |
| Jest czymś, czego Core potrzebuje od Unity? | Interfejs w `Core/`, implementacja w `UI/` |
| Sprawdza logikę? | Test w `Tests/` |

---

## 8. Nazewnictwo

| Co | Konwencja |
|---|---|
| Plik | nazwa pliku = nazwa klasy (wymóg Unity dla `MonoBehaviour`, zasada wszędzie) |
| Klasa | PascalCase, sufiks mówi o roli: `...System`, `...Rules`, `...Command`, `...View`, `...Controller`, `...Report`, `...Asset`, `...Definition` |
| Interfejs | `I...` |
| Enum | liczba pojedyncza, wartości PascalCase |
| Pole prywatne | camelCase, `readonly` gdy się da |
| Pole w Inspectorze | `[SerializeField] private` |
| Właściwość | PascalCase, tylko `get` gdy się da |
| Pole configu | PascalCase, nazwa opisuje sens z jednostką (`...PerDay`, `...Days`) |
| Metoda obliczająca (nic nie zmienia) | `Calculate...`, `Preview...`, `Get...` |
| Metoda zmieniająca stan | `Apply...`, `Execute`, czasownik (`Add`, `Remove`) |
| Test | `Metoda_Warunek_Wynik` |
| Obiekty w scenie | nazwa mówi, co to jest (`ResourceBar`, `EventLogScroll`) |

---

## 9. Konwencje w kodzie

### Namespace'y

- Assembly Core: `namespace Core`, enumy `namespace Core.Enums`.
- UI, Data, Tests: bez namespace'a. Jeśli kiedyś je wprowadzisz, wprowadź we wszystkich trzech naraz.
- Nie dodawaj podnamespace'ów odpowiadających folderom.
- Zapis blokowy (`namespace Core { ... }`). Zapis plikowy wymaga C# 10, a Unity obsługuje C# 9.

### Zakazy w Core

- `using UnityEngine;`, `Mathf` (użyj `System.Math`), `Debug`, `Time`, `Input`, `UnityEngine.Random`.
- `record` z `init` i rekordy pozycyjne (brak `IsExternalInit` w Unity). Używaj zwykłych klas z konstruktorem.
- Zmienne pola statyczne (ukryty stan globalny, który przecieka między testami i przetrwa wyjście z Play Mode).
- Magiczne liczby. Wszystko z configu.
- Tworzenie `new` konfiguracji lub generatora losowości w logice.

### Wartości opcjonalne

Brak wartości oznaczaj typem dopuszczającym `null` (np. `int?`), nie liczbą zastępczą (`-1`, `999`). Kompilator pilnuje wtedy, że przypadek brakującej wartości został obsłużony.

### Losowość

- Cała logika losuje przez interfejs generatora ze stanu. Nigdy przez `UnityEngine.Random` ani `System.Random`.
- Jedyne miejsce, w którym wolno użyć losowości Unity, to wylosowanie seeda w kontrolerze.
- Kolejność losowań wpływa na wynik. Dodanie losowania w środku istniejącej logiki zmienia przebieg dla starych seedów.

### Teksty dla gracza

- Docelowo Core przechowuje rodzaj zdarzenia i liczby, a widok zamienia je na tekst. Dzięki temu zmiana sformułowań i tłumaczenia nie dotykają logiki.
- Tymczasowe odstępstwa (tekst składany w Core) oznacz w komentarzu i trzymaj w jednym miejscu.
- Dopóki czcionka TMP nie ma polskich znaków, teksty w widokach są bez ogonków.

### Komentarze nad klasą

Krótko, w tym układzie:

```csharp
/// <summary>
/// Rola: co ta klasa robi.
/// Wejście: z czego korzysta.
/// Zmienia: co zmienia w stanie (albo "nic").
/// Używane przez: kto ją woła.
/// </summary>
```

Nad metodą, gdy nie jest oczywista: `// Czysta: niczego nie zmienia.` albo `// Zmienia stan: ...`. Nad polem configu: skąd liczba (`// GDD 4.5`, `// ROBOCZE`).

---

## 10. Testy

- Testujemy logikę z Core w EditMode, bez sceny.
- Nazwa: `Metoda_Warunek_Wynik`. Plik testów nazywa się po testowanej klasie.
- Każdy test zaczyna od świeżego stanu zbudowanego pomocnikiem, który **jawnie ustawia** wszystkie liczby, od których zależy wynik (stawki, pojemności, progi). Test nie zależy od wartości domyślnych configu.
- Przypadki z różnymi liczbami: `[TestCase(...)]`.
- **Co testować:** arytmetykę i zaokrąglenia, reguły z gałęziami, wartości graniczne, zgodność dwóch ścieżek liczenia (np. prognoza kontra rzeczywisty wynik), że metody obliczające niczego nie zmieniają.
- **Czego nie testować:** getterów, tego, że stała ma wartość z configu, kodu widoków.
- Test dopisujesz tam, gdzie logika jest wrażliwa, albo po błędzie, który cię ugryzł.
- Czytanie błędu: komunikat `Expected / But was` z liczbą wyraźnie mniejszą od oczekiwanej często oznacza ucięcie przez pojemność magazynu w stanie startowym testu.
- Folder testów odzwierciedla strukturę Core, gdy testów robi się dużo.

---

## 11. Unity: scena, prefaby, Inspector

- **Minimum scen.** Jedna scena główna, edytowana przez jedną osobę naraz.
- **Prefab na ekran** i prefab na powtarzalny element (wiersz listy, karta postaci).
- Odwołania do obiektów przez `[SerializeField] private` i przeciągnięcie w Inspectorze.
- Do kontrolera trafia w tablicy widoków tylko widok najwyższego poziomu, nie każdy element podrzędny.
- Przycisk: publiczna metoda kontrolera podpięta w **On Click ()**. Gdy przycisków jest wiele, widoki wystawiają zdarzenia, a kontroler zamienia je na komendy.
- Układ UI przez Layout Group i Content Size Fitter, bez ręcznego ustawiania pozycji.
- Canvas: Screen Space - Overlay, Scale With Screen Size, 1920x1080, Match 0.5.
- Błędy kompilacji o `TMPro` w UI: brakuje referencji `Unity.TextMeshPro` w asmdef.

---

## 12. Git

- Ustawienia: Force Text, Visible Meta Files.
- `.gitignore` z szablonu "Unity". `.gitattributes`: `* text=auto` i `eol=lf` dla `.unity`, `.prefab`, `.asset`, `.meta`.
- Commitujemy `Assets/`, `Packages/manifest.json`, `Packages/packages-lock.json`, `ProjectSettings/` zawsze razem z plikami `.meta`.
- Pliki przenoś w oknie Project w Unity, nie w Eksploratorze (inaczej gubisz `.meta`).
- Commit po każdym skończonym kroku, z zielonymi testami. Wiadomość mówi, co dodano (np. `Część 2, krok 3: przydział punktów`).
- Smart Merge (UnityYAMLMerge) konfigurowany lokalnie i używany przez `git mergetool` przy konflikcie w scenie lub prefabie.
- Git LFS dla grafiki i audio dopiero, gdy będą potrzebne.

---

## 13. Pułapki

| Objaw | Zwykle przyczyna |
|---|---|
| Błąd kompilacji w Core o `UnityEngine` / `Mathf` | Zakazany `using` lub `Mathf`; zamień na `System.Math` |
| Zmiana wartości w kodzie nie działa w grze | Asset konfiguracji ma starą wartość; zresetuj go w Inspectorze |
| `NullReferenceException` zaraz po starcie | Puste pole z assetem lub widokiem w Inspectorze kontrolera |
| Przycisk nie reaguje | Brak EventSystem w scenie albo metoda nie jest `public` |
| Widok się nie odświeża | Nie jest w tablicy kontrolera albo odświeżenie wołane przed komendą |
| Test padający z mniejszą liczbą niż oczekiwano | Pojemność magazynu w stanie testowym jest za mała |
| Tekst pokazuje kwadraty zamiast liter | Czcionka TMP bez polskich znaków |
| Błąd o niedostępnej funkcji C# | Zapis plikowy namespace'a, `record` z `init` lub inna funkcja nowsza niż C# 9 |
| Kolejny uruchomiony Play startuje ze starymi danymi | Zmienne pole statyczne w klasie |

---

## 14. Zasady utrzymania dokumentu

- Dokument opisuje zasady, nie pliki. Gdy zasada się zmienia, zmień tu, nie w komentarzach.
- Konkretne decyzje liczbowe i reguły gry są w GDD, nie tutaj.
- Przy dodaniu nowej warstwy lub assembly dopisz ją do sekcji 2, 3 i 4.
