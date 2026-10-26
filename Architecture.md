# MazEscaper: architektura projektu

Dokument opisuje strukturę kodu i zasady, których trzymamy się w prototypie.

## 1. Założenia

- **Widok:** zwykły ekran UI, bez sceny izometrycznej. Osada to panele i listy, labirynt to siatka 5x5. Izometria dopiero po prototypie.
- **UI:** uGUI + TextMeshPro.
- **Logika w czystym C#:** reguły gry nie używają `UnityEngine`, dzięki czemu testujemy je w EditMode i symulujemy bez edytora.
- **Dane zamiast kodu:** cechy, statusy, wydarzenia, sektory i stanowiska to definicje w ScriptableObjectach.
- **Jedna konfiguracja:** wszystkie liczby balansu (3 PA, 5% psucia, budżet 10, labirynt 5x5) w jednym pliku.
- **Losowość z seedem:** jeden generator, seed zapisany w logu, żeby błąd z playtestu dało się odtworzyć.
- **Widoki tylko czytają stan i wysyłają komendy**, np. "przydziel 1 PA osobie X do studni".

## 2. Struktura repozytorium

```
MazEscaper/                  (root repo)
├─ .gitignore, .gitattributes
├─ Docs/                     GDD, plan prototypu, ten plik (poza Assets/)
├─ Packages/
├─ ProjectSettings/
└─ Assets/
   ├─ Core/                  asmdef: MazEscaper.Core (No Engine References)
   ├─ Data/                  asmdef: MazEscaper.Data
   ├─ UI/                    asmdef: MazEscaper.UI
   ├─ Tests/                 asmdef: MazEscaper.Tests (tylko Editor)
   └─ TextMesh Pro/          tworzy Unity, nie ruszać
```

Foldery są płaskie. Podfoldery dodajemy dopiero, gdy w folderze zrobi się ciasno (około 15-20 plików).

## 3. Assembly i zależności

| Assembly | Referencje | Ustawienia |
|---|---|---|
| `MazEscaper.Core` | brak | No Engine References = ON, Auto Referenced = OFF |
| `MazEscaper.Data` | Core | Auto Referenced = OFF |
| `MazEscaper.UI` | Core, Data, `Unity.TextMeshPro`, `UnityEngine.UI` | Auto Referenced = OFF |
| `MazEscaper.Tests` | Core, Data, `UnityEngine.TestRunner`, `UnityEditor.TestRunner` | Platforma: Editor; `nunit.framework.dll`; Define Constraints: `UNITY_INCLUDE_TESTS` |

```
UI ──► Data ──► Core
UI ──► Core
Tests ──► Core, Data
```

Core nie zna nikogo. Nikt nie zna UI.

## 4. Zawartość folderów

### Core/ (czysty C#)

| Grupa | Przykłady |
|---|---|
| Stan gry | `GameState`, `Phase`, `Colonist`, `Maze`, `MazeCell` |
| Komendy (akcje gracza) | `ICommand`, `AdvancePhaseCommand`, później `AssignActionPointCommand` |
| Reguły gry | `PhaseSystem`, `BreakdownSystem`, `EventSystem`, `MissionSystem` |
| Definicje rodzajów rzeczy | `TraitDefinition`, `StatusDefinition`, `EventDefinition`, `SectorDefinition`, `PositionDefinition` |
| Konfiguracja | `GameConfig` |
| Usługi | `IRandom`, `SeededRandom`, `ILogSink` |

Zakaz: `MonoBehaviour`, `ScriptableObject`, `Debug`, `Time`, `Input`, `UnityEngine.Random`.

Różnica między stanem a definicją: `TraitDefinition` to szablon ("Leniwy daje -1 PA", nie zmienia się w grze), a `Colonist` to konkretna osoba z tą cechą (zmienia się w grze).

### Data/ (ScriptableObjecty)

- **Skrypty SO:** `GameConfigAsset`, `TraitAsset`, `EventAsset` itd. Każdy ma `ToConfig()` albo `ToDefinition()`, która zwraca czysty obiekt z Core.
- **Pliki `.asset`:** `GameConfig.asset` oraz po jednym pliku na cechę, status, wydarzenie, sektor i stanowisko.

Core nigdy nie widzi ScriptableObjectów, tylko obiekty zwrócone przez `ToConfig()` / `ToDefinition()`.

### UI/ (wszystko, co widać)

- **Skrypty:** `GameController`, widoki (`PhaseLabelView`, później `SettlementPanelView`, `MazeGridView`), `UnityLogSink`.
- **Prefaby:** jeden na ekran (`SettlementScreen`, `MazeScreen`...) oraz małe elementy wielokrotnego użytku (`ColonistRow`).
- **Sceny:** `Prototype.unity`.

### Tests/ (EditMode)

Testy Core, nazwane po testowanej klasie: `AdvancePhaseCommandTests`, `SeededRandomTests`.

### Docs/

Dokumenty projektowe w docx i md. Poza `Assets/`, żeby Unity nie tworzył plików `.meta`.

## 5. Przepływ danych

```
Klik w UI ──► View ──► GameController ──► ICommand.Execute(GameState)
                                                │
                                                ▼
                              Systems zmieniają GameState (używają IRandom, GameConfig)
                                                │
                                                ▼
UI odświeża widoki ◄── Views czytają GameState
```

1. Gracz klika przycisk w widoku.
2. Widok zgłasza to kontrolerowi, kontroler tworzy komendę i ją wykonuje na `GameState`.
3. Komenda wywołuje systemy, które zmieniają stan.
4. Kontroler odświeża widoki, a te tylko odczytują stan.

## 6. Konfiguracja

- `GameConfig` (Core) to zwykła klasa z wartościami domyślnymi, więc testy działają bez Unity.
- `GameConfigAsset` (Data) ma te same pola w Inspectorze i `ToConfig()`.
- `GameController` ma `[SerializeField] GameConfigAsset`, robi `ToConfig()` i przekazuje wynik do `GameState`.
- Zasada: żadnej magicznej liczby w logice. Wszystko przez `config.Coś`.

## 7. Losowość

- `IRandom`: `NextInt(min, max)`, `NextFloat()`, `Chance(p)`.
- `SeededRandom(int seed)`: własny algorytm (xorshift albo SplitMix32), bo `System.Random` nie gwarantuje takich samych wyników między wersjami runtime'u i platformami.
- Cała logika dostaje `IRandom` przez konstruktor lub `GameState`. Nic nie tworzy własnego generatora.
- `ILogSink` (Core) + `UnityLogSink` (UI): na starcie gry log `Seed: 123456`.
- `GameController` ma `[SerializeField] int fixedSeed`: 0 = losuj nowy, inna wartość = odtwarzanie błędu.
- Kolejność losowań wpływa na wynik. Ten sam seed odtworzy błąd tylko przy tych samych akcjach w tej samej kolejności.

## 8. Gdzie wrzucić nową rzecz

| Rzecz | Miejsce |
|---|---|
| Nowa mechanika | Stan, reguły i config w `Core/` |
| Nowa akcja gracza | Komenda w `Core/`, przycisk w `UI/` |
| Nowe wydarzenie lub cecha | `.asset` w `Data/` |
| Nowy typ treści (np. przedmioty) | Definicja w `Core/`, skrypt SO i assety w `Data/` |
| Nowy ekran | Prefab i widok w `UI/` |
| Coś z `Debug`, `Time`, `Input` | `UI/`, za interfejsem z `Core/` |

## 9. Konwencje

- **Namespace'y:** `MazEscaper.Core`, `MazEscaper.Data`, `MazEscaper.UI`, `MazEscaper.Tests`. Jeden na assembly.
- **Sceny:** minimum. Na razie tylko `Prototype.unity`, edytowana przez jedną osobę naraz.
- **Prefaby:** każdy ekran to osobny prefab, żeby uniknąć konfliktów.

## 10. Git

- Force Text serialization i Visible Meta Files (*Edit → Project Settings → Editor*).
- `.gitignore` z szablonu "Unity" (github/gitignore).
- `.gitattributes`: `* text=auto` oraz `eol=lf` dla `.unity`, `.prefab`, `.asset`, `.meta`.
- Smart Merge (UnityYAMLMerge) konfigurowany lokalnie, używany przez `git mergetool` przy konflikcie.
- Commitujemy `Assets/`, `Packages/manifest.json`, `Packages/packages-lock.json`, `ProjectSettings/` razem z plikami `.meta`.
- Git LFS dla grafiki i audio dopiero, gdy będą potrzebne.

## 11. Definicja ukończenia Części 0

- Świeży klon repo otwiera się bez błędów i bez zmian w `git status`.
- Assembly są rozdzielone: `using UnityEngine;` w Core nie kompiluje się.
- Scena `Prototype` odpala się, przycisk "Następna faza" zmienia fazę, seed widać w Console.
- Test EditMode w Core przechodzi (Test Runner na zielono).
