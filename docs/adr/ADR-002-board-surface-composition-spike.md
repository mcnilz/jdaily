# ADR-002: BoardSurface-Composition bis zur messbaren Visualtest-Baseline zurückstellen

| Feld | Wert |
|---|---|
| Status | `Proposed` |
| Datum | `2026-07-28` |
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
isolierter Katalog-Spike und wird nicht in Produktcode übernommen, bis
`UI-007` eine reproduzierbare Visualtest- und Messumgebung bereitstellt.

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
- Negativ: Für 1920 × 1080 und hohe Auflösung existieren bewusst keine
  erfundenen CPU-, Speicher-, Frame-Time- oder Visual-Tree-Werte.
- Folgearbeit: Der Produkteigentümer legt für `UI-007` das kanonische
  Golden-Master-Betriebssystem fest. Anschließend erfasst der Headless-Harness
  diese vier Werte je Auflösung und entscheidet über eine Übernahme in
  `VS-004`/`REP-007`.

## Nachweise

- `BoardSurfaceTests.fs` deckt Scope-Isolation, Abbruch und Reduced Motion ab.
- `UiCatalogShellTests.fs` registriert und prüft die dreispaltige gemeinsame
  Fixture sowie die vier BoardSurface-Szenarien.
- `dotnet build JiraBoard.slnx -c Release` am 28. Juli 2026: 0 Warnungen,
  0 Fehler.
- `dotnet test JiraBoard.slnx -c Release --no-build` am 28. Juli 2026:
  113/113 erfolgreich.
- `dotnet run --project src\JiraBoard.UiCatalog\JiraBoard.UiCatalog.fsproj -c Release --no-build`
  startete den Katalog am 28. Juli 2026 ohne Startfehler.

## Offene Punkte

- `UI-007`: kanonisches Golden-Master-Betriebssystem, Renderer, DPI und
  Messmethode festlegen; ohne diese Entscheidung bleiben Performance-Baselines
  und eine Annahme dieser ADR offen.