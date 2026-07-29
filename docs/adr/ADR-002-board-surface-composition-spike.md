# ADR-002: BoardSurface-Composition bis zur messbaren Visualtest-Baseline zurückstellen

| Feld | Wert |
|---|---|
| Status | `Proposed` |
| Datum | `2026-07-29` |
| Backlog | `SPK-001` |
| Verantwortlich | Produkteigentümer |
| Ersetzt | `–` |

## Fragestellung und Kontext

`SPK-001` untersucht eine isolierte BoardSurface mit drei Statusspalten für
lane-lokales Daily Replay. Die [UI-Design-Spezifikation](../../ui-design-specification.md)
fordert Reduced Motion und zentrale Tokens; der [technische Handover](../../avalonia-fsharp-funcui-stack-handoff.md)
verlangt Native-AOT-fähige, reflexionsfreie Implementierungen. Nach dem
[Product Backlog](../../product-backlog.md) sind CPU-, Speicher-, Frame-Time-
und Visual-Tree-Baselines bei 1920 × 1080 sowie hoher Auflösung Bestandteil
des Spikes.

## Entscheidung

Der direkte Avalonia-Composition-Offset-Versuch ist technisch zulässig: Er
verwendet `ElementComposition.GetElementVisual`, eine
`Vector3KeyFrameAnimation` und `StartAnimation("Offset", ...)`, ohne
Reflection oder dynamische Ausdrücke. Die BoardSurface bleibt jedoch ein
isolierter Katalog-Spike und wird nicht in Produktcode übernommen. `UI-007`
stellt nun die reproduzierbare Visualtest- und Messumgebung bereit; die
erhobenen Stichproben reichen als Risikoindikator, nicht als Runtime-
Frame-Time-Nachweis für eine Produktübernahme.

Die Katalogansicht bildet vier deterministische Fälle aus derselben
Produktionsview ab: aktive Swimlane, einzelner Subtask, Abbruch und Reduced
Motion. Die reine Projektion begrenzt den aktiven Scope und setzt den Versatz
bei Abbruch oder Reduced Motion auf null.

## Betrachtete Alternativen

| Alternative | Dafür | Dagegen | Ergebnis |
|---|---|---|---|
| Direkte Composition-Offset-Animation | renderthreadnah, ohne Elmish-Update-Flut, AOT-kompatibel kompiliert | Performance noch nicht reproduzierbar messbar | als Kandidat zurückgestellt |
| Deterministische Transform-Projektion im Katalog | prüft Scope, Abbruch und Reduced Motion ohne zeitabhängige Tests | kein Nachweis für Runtime-Frame-Timing | für den Spike gewählt |
| Produktintegration vor `UI-007` | würde Folgearbeit beschleunigen | verletzt das UiCatalog-/Visualtest-Gate und verdeckt Messrisiken | verworfen |

## Konsequenzen

- Positiv: Drei Spalten, Swimlane- und Subtask-Scope, Abbruch und Reduced
  Motion sind als Produktionsview mit gemeinsamen Fixtures und Unit-Tests
  demonstrierbar.
- Negativ: Die Headless-Stichproben erfassen keine Renderthread-Frame-Time einer
  laufenden Composition-Animation; eine Produktübernahme benötigt deshalb vor
  `VS-004`/`REP-007` einen zusätzlichen Laufzeitnachweis.
- Folgearbeit: Der Produkteigentümer entscheidet anhand dieses Spikes, ob der
  lokale Composition-Kandidat für die spätere Produktvalidierung weiterverfolgt
  wird.

## Nachweise

- `BoardSurfaceTests.fs` deckt Scope-Isolation, Abbruch und Reduced Motion ab.
- `UiCatalogShellTests.fs` registriert und prüft die dreispaltige gemeinsame
  Fixture sowie die vier BoardSurface-Szenarien.
- [`ui-007-board-surface-measurements.md`](../validation/ui-007-board-surface-measurements.md)
  dokumentiert CPU, verwalteten Speicher, Frame-Time und Visual-Tree für
  1920 × 1080 sowie 3840 × 2160 unter Windows 11 x64, Standard-Skia und 100 %
  Betriebssystem-DPI.
- `dotnet restore JiraBoard.slnx` am 29. Juli 2026: erfolgreich.
- `dotnet build JiraBoard.slnx -c Release --no-restore` am 29. Juli 2026:
  0 Warnungen, 0 Fehler.
- `dotnet test JiraBoard.slnx -c Release --no-build` am 29. Juli 2026:
  121/121 erfolgreich.
- `dotnet run --project tests\JiraBoard.AotSmokeTests\JiraBoard.AotSmokeTests.fsproj -c Release --no-build`
  am 29. Juli 2026: erfolgreich.

## Offene Punkte

- Menschliche Annahme der Folgeentscheidung für die spätere
  Produktvalidierung; bis dahin bleibt der Spike isoliert im UiCatalog.