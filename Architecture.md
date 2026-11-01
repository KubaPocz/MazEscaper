# MazEscaper: architektura projektu

Dokument opisuje, jak zbudowany jest kod, gdzie co leży, jak się łączy i jakich zasad się trzymamy. Stan: koniec Części 1 prototypu. Przy każdej kolejnej części zaktualizuj sekcje 4, 5 i 6.

Powiązane dokumenty: GDD (liczby i reguły gry), plan prototypu (kolejność prac). Ten plik odpowiada na pytania "gdzie to dodać" i "jak to napisać".

---

## 1. Założenia

1. **Logika w czystym C#.** Reguły gry nie używają `UnityEngine`. Dzięki temu testujemy je w EditMode i (w Części 7) symulujemy tysiące partii bez edytora.
2. **Widoki tylko czytają stan i wysyłają komendy.** Żaden widok nie zmienia `GameState` i nie liczy reguł gry.
3. **Stan to dane, reguły to systemy.** `GameState` trzyma wartości. Systemy (klasy statyczne) liczą i zmieniają stan, dostając go jako argument.
4. **Jedno źródło liczb balansu.** Każda liczba z GDD siedzi w `GameConfig` i jest czytana przez `state.Config`. Żadnych magicznych liczb w logice.
5. **Jedno źródło reguł.** Gdy dwa miejsca muszą liczyć to samo (wieczór i prognoza), korzystają z tej samej funkcji. Nigdy z dwóch osobnych wzorów.
6. **Losowość z seedem.** Jeden generator (`IRandom`) w `GameState`, seed zapisany w logu, żeby błąd z playtestu dało się odtworzyć.

---

## 2. Struktura repozytorium

```
MazEscaper/                       (root repo)
├─ .gitignore, .gitattributes
├─ Docs/                          GDD, plan prototypu, ten plik (poza Assets/)
├─ Packages/
├─ ProjectSettings/
└─ Assets/
   ├─ Core/                       asmdef: MazEscaper.Core (No Engine References)
   │  ├─ Enums/                   enumy (Phase, ResourceType, ForecastLevel)
   │  ├─ Commands/                ICommand i wszystkie komendy
   │  └─ Systems/                 reguły gry: systemy i klasy "Rules"
   ├─ Data/                       asmdef: MazEscaper.Data
   ├─ UI/                         asmdef: MazEscaper.UI
   ├─ Tests/                      asmdef: MazEscaper.Tests (tylko Editor)
   └─ TextMesh Pro/               tworzy Unity, nie ruszać
```

Reszta plików Core (stan, model, config, interfejsy usług) leży luzem w `Core/`. Kolejne podfoldery dodajemy dopiero wtedy, gdy w folderze zrobi się ciasno (powyżej 15-20 plików).

### Co leży w którym folderze

| Folder | Co tu wchodzi | Czego tu nie wolno |
|---|---|---|
| `Core/` | Stan gry i model (`GameState`, `ResourceStore`, `Storehouse`), raporty i wyniki (`EveningReport`, `ResourceForecast`), log (`EventLog`), konfiguracja (`GameConfig`), interfejsy usług (`IRandom`, `ILogSink`) i ich czyste implementacje (`SeededRandom`) | `MonoBehaviour`, `ScriptableObject`, `Debug`, `Time`, `Input`, `Mathf`, `UnityEngine.Random` |
| `Core/Enums/` | Enumy opisujące pojęcia z reguł gry | Logika |
| `Core/Commands/` | `ICommand` i komendy: jedna klasa = jedna akcja gracza | Logika liczenia (ta idzie do systemów) |
| `Core/Systems/` | Statyczne klasy z regułami: `PhaseSystem`, `EveningPhaseSystem`, `ConsumptionRules`, `ResourceForecastSystem` | Własny stan (pola zmienne, liczniki) |
| `Data/` | Skrypty `ScriptableObject` (`GameConfigAsset`) i pliki `.asset` | Logika gry |
| `UI/` | Kontroler, widoki, implementacje usług z Unity (`UnityLogSink`), prefaby, scena | Reguły gry, zmiany stanu |
| `Tests/` | Testy EditMode dla Core | Testy zależne od sceny |
| `Docs/` | Dokumenty projektowe (docx, md) | Pliki, które mają być w grze |

---

## 3. Assembly i zależności

| Assembly | Referencje | Ustawienia |
|---|---|---|
| `MazEscaper.Core` | brak | No Engine References = ON, Auto Referenced = OFF |
| `MazEscaper.Data` | Core | Auto Referenced = OFF |
| `MazEscaper.UI` | Core, Data, `Unity.TextMeshPro`, `UnityEngine.UI` | Auto Referenced = OFF |
| `MazEscaper.Tests` | Core, Data, `UnityEngine.TestRunner`, `UnityEditor.TestRunner` | Platforma: tylko Editor; Override References → `nunit.framework.dll`; Define Constraints: `UNITY_INCLUDE_TESTS` |

```
UI ──► Data ──► Core
UI ──► Core
Tests ──► Core, Data
```

Core nie zna nikogo. Nikt nie zna UI. Jeśli Core nie kompiluje się po dodaniu `using UnityEngine;`, to znaczy, że rozdział działa (i tak ma być).

---

## 4. Rejestr skryptów

Tabela mówi, co każdy skrypt robi i kto go używa. Przy dodaniu nowego skryptu dopisz do niej wiersz.

### Core/ (luzem)

| Skrypt | Rola | Używany przez |
|---|---|---|
| `GameState` | Cały stan gry: `Config`, `Random`, `Phase`, `CurrentDay`, `Storehouse`, `ResidentCount`, `EveningReport`, `EventLog` | wszystko |
| `GameConfig` | Płaska klasa z liczbami balansu i ich źródłem w GDD (komentarz przy polu) | `GameState`, systemy (przez `state.Config`) |
| `ResourceStore` | Jeden zasób: ilość i pojemność. `AddResource`, `RemoveResource`, `PreviewRemove` (bez zmiany stanu) | `Storehouse` |
| `Storehouse` | Magazyn: sześć `ResourceStore` (Food, Water, Wood, Stone, Parts, Medicine) i metoda `Get(ResourceType)` | `GameState`, systemy, widoki |
| `EveningReport` | Wynik rozliczenia wieczoru (potrzeba, zużyto, zepsuło się). Tylko do odczytu | `EveningPhaseSystem`, widoki |
| `ResourceForecast` | Wynik prognozy jednego zasobu: jutro, bilans, dni do wyczerpania (`int?`), poziom. Tylko do odczytu | `ResourceForecastSystem`, widoki |
| `EventLog`, `EventLogEntry` | Log zdarzeń dla gracza: dzień, faza, tekst | `EveningPhaseSystem`, `EventLogView` |
| `IRandom`, `SeededRandom` | Losowość z seedem | `GameState`, systemy |
| `ILogSink` | Kanał diagnostyczny (seed, błędy). Nie jest logiem dla gracza | `GameController` |

### Core/Enums/

| Skrypt | Wartości |
|---|---|
| `Phase` | Morning, Day, Evening (kolejność ma znaczenie: na niej opiera się zawijanie faz) |
| `ResourceType` | Food, Water, Wood, Stone, Parts, Medicine (kolejność = kolejność wierszy paska zasobów) |
| `ForecastLevel` | Green, Yellow, Red |

### Core/Commands/

| Skrypt | Rola |
|---|---|
| `ICommand` | Kontrakt: `Execute(GameState)` |
| `AdvancePhaseCommand` | Przejście do następnej fazy. Cienka nakładka: woła `PhaseSystem.NextPhase` |

### Core/Systems/

| Skrypt | Rola | Wywołuje |
|---|---|---|
| `PhaseSystem` | Zmienia fazę, podbija dzień po zawinięciu, w `OnPhaseEntered` robi `switch` po `state.Phase` | `EveningPhaseSystem` |
| `EveningPhaseSystem` | `CalculateReport` (czysta) i `ApplyPhase` (zdejmuje zasoby, zapisuje raport, dopisuje wpisy do logu) | `ConsumptionRules` |
| `ConsumptionRules` | Reguły zużycia i psucia: `NeededFood`, `NeededWater`, `SpoiledFood`, `FoodTomorrow`, `WaterTomorrow` | nic |
| `ResourceForecastSystem` | Prognoza żywności i wody (jutro, bilans, dni do wyczerpania, poziom) | `ConsumptionRules` |

### Data/

| Skrypt | Rola |
|---|---|
| `GameConfigAsset` | `ScriptableObject` z polem `GameConfig`. `ToConfig()` zwraca kopię (`Clone`), żeby gra nie nadpisywała assetu |
| `GameConfig.asset` | Plik z liczbami, edytowany w Inspectorze |

### UI/

| Skrypt | Rola |
|---|---|
| `GameController` | Tworzy `GameState` z assetu i seeda, wykonuje komendy, po każdej woła `Render` na wszystkich widokach z tablicy |
| `GameView` | Klasa abstrakcyjna: `Render(GameState)`. Baza wszystkich widoków |
| `LabelView` | Baza prostych etykiet: jedna metoda zwraca tekst, klasa wpisuje go do `TMP_Text` |
| `PhaseLabelView`, `DayLabelView` | Etykiety fazy i dnia |
| `ResourceRowView` | Jeden wiersz zasobu: stan z pojemnością, prognoza (żywność, woda), kolor. Typ zasobu ustawiany w Inspectorze |
| `ResourceBarView` | Kontener na sześć wierszy. Przekazuje stan do każdego |
| `EventLogView` | Składa wpisy logu w jeden tekst w `Scroll View`, przewija na dół |
| `UnityLogSink` | Implementacja `ILogSink` przez `Debug.Log` |

### Tests/

| Skrypt | Co sprawdza |
|---|---|
| `TestHelpers` | `MakeState(food, water)`: stan z jawnie ustawionym configiem (nie zależy od wartości domyślnych) |
| `ResourceStoreTests` | Przycinanie do pojemności, odejmowanie ponad stan, błędy |
| `GameStateTests` | Stan startowy, wartości z configu, niezależność stanów |
| `PhaseSystemTests`, `AdvancePhaseCommandTests` | Kolejność faz, wzrost dnia |
| `EveningPhaseSystemTests` | Zużycie, psucie od zapasu po zużyciu, niedobór, sekwencja dni |
| `ResourceForecastSystemTests` | Wartości prognozy, granica horyzontu, **zgodność prognozy z rzeczywistością przez 20 dni** |
| `EventLogTests` | Wpisy logu po wieczorze (tylko gdy coś się zepsuło lub zabrakło) |
| `SeededRandomTests` | Ten sam seed daje tę samą sekwencję |

---

## 5. Przepływ danych

### Kliknięcie "Następna faza"

```
przycisk (On Click)
   └─► GameController.OnNextPhaseClicked()
          └─► new AdvancePhaseCommand().Execute(state)
                 └─► PhaseSystem.NextPhase(state)
                        ├─ zmienia state.Phase, przy zawinięciu CurrentDay++
                        └─ OnPhaseEntered(state): switch (state.Phase)
                               └─ Evening: EveningPhaseSystem.ApplyPhase(state)
                                      ├─ CalculateReport(state)   (czysta, korzysta z ConsumptionRules)
                                      ├─ zdejmuje zasoby z Storehouse
                                      ├─ state.EveningReport = raport
                                      └─ state.EventLog.Add(...)
          └─► GameController odświeża widoki: view.Render(state) dla każdego z tablicy
```

### Wyświetlenie paska zasobów

```
ResourceBarView.Render(state)
   └─► ResourceRowView.Render(state)  (po jednym na zasób)
          ├─ stan i pojemność:  state.Storehouse.Get(typ)
          └─ prognoza (Food, Water): ResourceForecastSystem.ForecastFood/Water(state)
                 └─ ConsumptionRules (te same funkcje, których używa wieczór)
```

### Zasady przepływu

- Zmiana stanu zawsze przez komendę. Widok nigdy nie woła systemu, który zmienia stan.
- Widok może wywołać **czystą** funkcję liczącą (np. prognozę), bo nic nie zmienia.
- Kontroler odświeża widoki po komendzie. Core nie wie, że widoki istnieją.
- Prognoza jest liczona przy każdym `Render`. Nie zapisuj jej w `GameState`: to dane pochodne i szybko by się zdezaktualizowały.
- Raport wieczoru jest zapisany w `GameState`, bo opisuje zdarzenie, które już się wydarzyło. Przed pierwszym wieczorem jest `null`, więc każdy widok musi to obsłużyć. `EventLog` nigdy nie jest `null`.

---

## 6. Zasady dla poszczególnych warstw

### GameState

- Same dane. Żadnych metod, które podejmują decyzje.
- `Phase` i `CurrentDay` zmienia tylko `PhaseSystem`. Mimo że settery są publiczne (wygodne w testach), nie ustawiaj ich w innych systemach.
- `Config` i `Random` są w stanie jako zależności, wygodne, bo systemy i tak dostają stan.
- Liczba mieszkańców to `ResidentCount` (nie `Starting...`). Wartość startowa pochodzi z configu, ale systemy czytają tylko stan. W Części 2 zmieni się w właściwość zwracającą długość listy mieszkańców, a kod systemów zostanie bez zmian.

### Systemy (Core/Systems)

- Klasy `static`, bez pól zmiennych. Jeśli czegoś trzeba zapamiętać, idzie to do `GameState`.
- Każdy system dostaje `GameState` jako argument. Config bierz z `state.Config`, nie przez osobny parametr.
- **Rozdziel obliczenie od zastosowania.** `Calculate...` jest czysta (nie zmienia nic, może ją wołać prognoza lub test). `Apply...` zmienia stan. Nazwa metody ma mówić, która jest która.
- Funkcje reguł (klasy `...Rules`) przyjmują liczby i config, a nie cały stan, kiedy mogą. Dzięki temu prognoza może policzyć "co będzie jutro" na liczbie, której jeszcze nie ma w magazynie.
- Tekst dla gracza w Core tylko w `EventLog` (tymczasowo, patrz sekcja 9). Reszta tekstów w widokach.
- `switch` po fazie w `PhaseSystem.OnPhaseEntered`: w każdym `case` jedno wywołanie systemu. Pusty `case` z komentarzem zamiast pominięcia, plus `default` rzucający wyjątek, żeby nowa faza bez obsługi wywaliła się w testach.

### Komendy (Core/Commands)

- Jedna klasa = jedna akcja gracza (`AdvancePhaseCommand`, później `AssignActionPointCommand`, `StartExpeditionCommand`).
- Implementują `ICommand` z jedną metodą `Execute(GameState)`. Dane komendy (kto, gdzie, ile) trafiają do konstruktora, stan dopiero do `Execute`.
- Komenda nie liczy. Woła system. Cała treść zwykle to jedna linijka.
- Komendy są zwykłymi klasami (nie statycznymi), bo muszą implementować interfejs i przenosić dane.
- Cofanie dopiero od Części 2: osobny interfejs (np. `IUndoableCommand`) tylko dla komend, które da się cofnąć. Przejście fazy nigdy go nie dostanie.

### Config

- Płaska klasa `GameConfig`, wartości domyślne ustawione w kodzie (testy działają bez Unity).
- Przy każdym polu komentarz ze źródłem: `// GDD 4.5` albo `// ROBOCZE` (gdy GDD nie podaje wartości).
- `GameConfigAsset` trzyma `GameConfig` i zwraca kopię przez `ToConfig()`.
- Po zmianie wartości domyślnych w kodzie asset w Inspectorze trzeba zresetować (trzy kropki → Reset), bo asset wygrywa nad kodem.
- Nie twórz `new GameConfig()` w kodzie gry. Wyjątek: testy.
- Skala jednostek: zużycie 4 żywność + 4 woda na osobę dziennie; 1 PA = 8 jednostek żywności lub wody albo 4 jednostki drewna lub kamienia. Wszystkie liczby w jednostkach liczysz w tej skali.

### Widoki (UI)

- Każdy widok dziedziczy po `GameView` i implementuje `Render(GameState)`.
- Prosta etykieta dziedziczy po `LabelView` i implementuje jedną metodę zwracającą tekst. Widok z kilkoma polami albo kolorem dziedziczy bezpośrednio po `GameView`.
- Widok nie zmienia stanu i nie wywołuje systemów zmieniających stan.
- Kontenery (`ResourceBarView`) trzymają widoki podrzędne i tylko przekazują im stan.
- Do `GameController` w tablicy widoków trafia tylko widok najwyższego poziomu (pasek), nie każdy wiersz osobno.
- Tłumaczenie enumów na tekst (Morning → "Poranek") robi widok, nie Core.
- Kolory trzymaj w polach `[SerializeField]`, nie w kodzie.

### Kontroler

- Jeden `GameController` na prototyp. Tworzy stan, wykonuje komendy, odświeża widoki.
- Jedyne pole z `GameConfigAsset` w całym projekcie jest w kontrolerze.
- Seed: `[SerializeField] int fixedSeed` (0 = losuj). To jedyne miejsce, w którym wolno użyć `UnityEngine.Random`, i tylko do wylosowania seeda. Cała logika losuje przez `IRandom`.
- Przycisk to publiczna metoda w kontrolerze podpięta w **On Click ()**. Gdy przycisków będzie wiele (Część 2), widoki wystawią zdarzenia, a kontroler zamieni je na komendy.

---

## 7. Konwencje

### Nazwy

| Co | Konwencja | Przykład |
|---|---|---|
| Plik | nazwa pliku = nazwa klasy | `EveningPhaseSystem.cs` |
| Klasa | PascalCase, sufiks mówi o roli | `...System`, `...Rules`, `...Command`, `...View`, `...Report`, `...Asset` |
| Interfejs | prefiks `I` | `ICommand`, `IRandom` |
| Enum | liczba pojedyncza, wartości PascalCase | `Phase.Morning` |
| Pole prywatne | camelCase, `readonly` gdy się da | `private readonly List<...> entries` |
| Właściwość | PascalCase, tylko `get` gdy się da | `public int Day { get; }` |
| Pole configu | PascalCase, nazwa opisuje sens z jednostką | `FoodPerResidentPerDay` |
| Pole w Inspectorze | `[SerializeField] private` | `[SerializeField] private TMP_Text nameText;` |
| Metoda obliczająca | `Calculate...`, `Preview...`, nic nie zmienia | `CalculateReport` |
| Metoda zmieniająca | `Apply...`, `Execute`, czasownik | `ApplyPhase` |

### Namespace'y

- Wszystkie skrypty w assembly Core mają `namespace Core`, a enumy `namespace Core.Enums`.
- Skrypty w UI, Data i Tests są bez namespace'a (tak jest dziś). Nie dodawaj podnamespace'ów typu `Core.Commands`: folder służy do porządku, namespace nie musi go odzwierciedlać.
- Nazwa assembly (`MazEscaper.Core`) to nie to samo co namespace (`Core`).
- Zapis blokowy `namespace Core { ... }`. Zapis plikowy (`namespace Core;`) jest z C# 10, a Unity obsługuje C# 9.

### Zakaz w kodzie Core

- `using UnityEngine;`, `Mathf` (używaj `System.Math`), `Debug`, `Time`, `Input`.
- `record` z `init` i pozycyjne rekordy (Unity nie ma `IsExternalInit`). Używaj zwykłych klas z konstruktorem.
- Statycznych pól, które się zmieniają.
- Magicznych liczb. Wszystko z `state.Config`.

### Jak opisywać skrypty

Nad każdą klasą krótki komentarz w tym układzie (kilka linii, nie esej):

```csharp
/// <summary>
/// Rola: rozliczenie wieczoru (zużycie, psucie, wpisy do logu).
/// Wejście: GameState.
/// Zmienia: Storehouse, EveningReport, EventLog (tylko ApplyPhase).
/// Używane przez: PhaseSystem.
/// </summary>
public static class EveningPhaseSystem
```

Nad metodą, gdy nie jest oczywista, jedna linia z oznaczeniem rodzaju:

- `// Czysta: niczego nie zmienia.`
- `// Zmienia stan: ...`

Nad polem configu: skąd liczba (`// GDD 4.7`) albo `// ROBOCZE`.

Po dodaniu skryptu dopisz wiersz w rejestrze (sekcja 4).

### Testy

- Nazwa: `Metoda_Warunek_Wynik`, np. `ApplyPhase_Shortage_UsesOnlyWhatIsAvailable`.
- Każdy test zaczyna od świeżego stanu z `TestHelpers.MakeState`, a nie z domyślnego configu. Helper ustawia jawnie wszystkie liczby, od których zależy wynik (stawki, pojemność, progi).
- Dla przypadków z różnymi liczbami używaj `[TestCase(...)]`.
- Zasada, co testować: arytmetykę i zaokrąglenia, reguły z gałęziami, zgodność dwóch ścieżek liczenia (prognoza kontra rzeczywistość). Nie testuj getterów i tego, że stała ma wartość z configu.
- Test dopisujesz wtedy, gdy logika jest wrażliwa albo po błędzie, który cię ugryzł.
- Odczyt komunikatu `Expected / But was` z Test Runnera: liczba wyraźnie mniejsza od oczekiwanej to zwykle ucięcie przez pojemność magazynu.

### Unity i scena

- Jedna scena `Prototype.unity`, edytowana przez jedną osobę naraz.
- Każdy ekran to osobny prefab. Wiersze powtarzalne (`ResourceRow`) to osobne prefaby.
- Obiekty UI: nazwa mówi, co to jest (`ResourceBar`, `EventLogScroll`, `NextPhaseButton`).
- Po zmianie wartości domyślnych `GameConfig` w kodzie zresetuj asset.
- Polskie znaki: dopóki czcionka TMP ich nie ma, teksty w widokach są bez ogonków. Po dodaniu fontu (*Font Asset Creator*, Atlas Population Mode: Dynamic) przywróć ogonki w `GetName` i podobnych metodach.

### Git

- Commit po każdym skończonym kroku, z zielonymi testami. Wiadomość np. `Część 1, krok 5: prognoza zasobów`.
- Pliki `.meta` zawsze razem z plikami. Przenoś pliki w oknie Project w Unity, nie w Eksploratorze.
- Ustawienia projektu: Force Text i Visible Meta Files.
- Smart Merge (UnityYAMLMerge) konfigurowany lokalnie i używany przez `git mergetool`, gdy dwie osoby zmienią ten sam prefab.

---

## 8. Jak dodać nową rzecz (przepisy)

### Nowa liczba balansu
1. Dodaj pole do `GameConfig` z wartością domyślną i komentarzem (`// GDD x.y` lub `// ROBOCZE`).
2. Czytaj je przez `state.Config.NazwaPola`.
3. Zresetuj `GameConfig.asset` w Inspectorze.

### Nowy stan gry
1. Pole albo klasa w `Core/` (tylko dane).
2. Wartość startowa w konstruktorze `GameState`, z configu jeśli to liczba balansu.
3. Test stanu startowego w `GameStateTests`.

### Nowa reguła (obliczenie)
1. Funkcja w `Core/Systems/` (zwykle w klasie `...Rules`), przyjmująca liczby i config.
2. Jeśli ktoś inny ma liczyć to samo (prognoza, UI), woła tę samą funkcję.
3. Test z liczbami ręcznie policzonymi na papierze.

### Nowy system (zmiana stanu)
1. Statyczna klasa w `Core/Systems/` z parą `Calculate...` (czysta) i `Apply...` (zmienia stan).
2. Podpięcie: dla fazy to nowy `case` w `PhaseSystem.OnPhaseEntered`, dla akcji gracza komenda.
3. Wpis w logu dla zdarzeń, które gracz ma zobaczyć.
4. Testy: wynik zmiany, brak zmian w `Calculate...`, przypadek graniczny.

### Nowa komenda
1. Klasa w `Core/Commands/` implementująca `ICommand`. Dane w konstruktorze.
2. `Execute` woła system, nie liczy sam.
3. W `GameController` publiczna metoda tworząca komendę, wykonująca ją i odświeżająca widoki. Podpięcie w **On Click ()** przycisku.
4. Test komendy: stan po `Execute`.

### Nowy widok
1. Klasa w `UI/` dziedzicząca po `GameView` (albo `LabelView` dla prostej etykiety).
2. Pola `[SerializeField] private` na elementy TMP i kolory.
3. W scenie: obiekt z komponentem, pola przeciągnięte w Inspectorze.
4. Dodanie do tablicy widoków w `GameController` (lub do kontenera, jeśli to wiersz).
5. Widok sprawdza `null` dla danych, które mogą jeszcze nie istnieć (np. `EveningReport`).

### Nowy enum
1. Plik w `Core/Enums/`, `namespace Core.Enums`, jedna wartość na linię.
2. Nowe wartości dopisuj na końcu (kolejność bywa używana, np. w zawijaniu faz).
3. Tłumaczenie na tekst w widoku.

### Nowa faza dnia
1. Wartość w `Phase` na właściwym miejscu (uwaga na zawijanie).
2. Nowy `case` w `PhaseSystem.OnPhaseEntered`.
3. Liczba faz w `NextPhase` bierze się z enuma, nie z cyfry w kodzie.

### Nowa treść z ScriptableObjectów (Część 2 i dalej)
1. Czysta definicja w `Core/` (np. `TraitDefinition`).
2. Skrypt SO w `Data/` z metodą `ToDefinition()`.
3. Pliki `.asset` w `Data/`. Core widzi tylko definicje, nigdy SO.

---

## 9. Znane odstępstwa i plan

- **Tekst logu w Core.** `EveningPhaseSystem` składa teksty wpisów w Core, bo to prostsze w prototypie. Gdy dojdzie tłumaczenie albo zmiana sformułowań, wpis powinien przechowywać rodzaj zdarzenia i liczby, a widok zamieniałby je na tekst.
- **Prognoza tylko dla żywności i wody.** Drewno, kamień, części i leki nie mają zużycia, więc pokazują tylko stan. Produkcja (Część 2) wejdzie do funkcji "zapas po jednym dniu" w `ConsumptionRules`, bez zmian w reszcie prognozy.
- **Niedobór.** Zasób schodzi do zera i raport zapamiętuje brakującą ilość. Decyzja, kto je (Głód, Pragnienie), pojawia się w Części 3.
- **Liczba mieszkańców jako liczba.** W Części 2 zamieni się na listę mieszkańców, a `ResidentCount` zostanie właściwością tylko do odczytu.
- **Komendy bez cofania.** Cofanie przydziału punktów (Część 2) przez osobny interfejs.
- **Jeden kontroler.** Rozważ podział per ekran dopiero w Części 4 (ekran wyprawy).

---

## 10. Definicja ukończenia Części 1

- Test Runner: wszystkie testy na zielono, w tym test zgodności prognozy z rzeczywistością (20 dni).
- Scena: przycisk zmienia fazy i dni, wieczorem zasoby spadają zgodnie z prognozą, niedobór świeci na czerwono, log pokazuje wpisy.
- Brak błędów w Console.
- Core kompiluje się bez `UnityEngine`.