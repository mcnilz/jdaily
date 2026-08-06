# VS-001 – Offline-Projekt- und Sprintkontext

Test-first Umsetzung des abgegrenzten Offline-Slice für Projektauswahl, Wiederherstellung und Sprint-Scope. Live-Jira, Credential-Eingabe, Persistenz und `JiraBoard.App`-Verdrahtung bleiben außerhalb des Pakets.

## For Future Agents

Das Paket wurde am 6. August 2026 eigenständig menschlich abgenommen und auf `Done` gesetzt. `VS-002` darf weiterhin erst nach einem gesonderten Vorschlag und dessen Bestätigung beginnen.

## Phase 1: Domain Red-Green

Status: Complete

- [x] Erststart, gültige und ungültige Wiederherstellung test-first abdecken
- [x] Site-, Projekt-, Board- und Sprintidentität ausschließlich über stabile IDs prüfen
- [x] inaktiven gespeicherten Sprint auf `AllActiveSprints` mit sichtbarem Hinweis zurücksetzen
- [x] ungültige Projekt-/Board-/Sprintbestätigungen ohne Mutation nachweisen
- [x] Kontextgeneration bei bestätigten Wechseln erhöhen

### Verification Plan

- gezielte `NavigationContextTests`
- Domain-Grenze über vollständigen Solutiontest

### Phase Summary

Der pure Domainzustand validiert die aktive Site sowie Projekt-/Board-Zuordnung, stellt gültige Kontexte wieder her und behandelt veraltete Sprintscopes deterministisch.

## Phase 2: Produktionsviews und UiCatalog

Status: Complete

- [x] `ContextHeader`, `ProjectSelectionModal` und `SprintMenu` als Produktionsviews bauen
- [x] Projektsuche nach Name und Key sowie Auswahl-vor-Öffnen umsetzen
- [x] `AllActiveSprints` und zwei aktive Sprints in stabiler Quellreihenfolge darstellen
- [x] fünf benannte Navigationsszenarien mit gemeinsamer typisierter Fixture registrieren
- [x] sichtbare Texte, stabile ID-Callbacks und Automation-Namen testen

### Verification Plan

- gezielte `NavigationViewTests` und `UiCatalogShellTests`
- reale Avalonia-Visual-Tree-Nachweise für alle fünf Szenarien

### Phase Summary

Der UiCatalog rendert dieselben Produktionsviews wie spätere Hosts. Erststart, Restore-Fehler, wiederhergestellter Kontext und beide Sprintscopes sind offline und deterministisch sichtbar.

## Phase 3: Review und Abschlussvalidierung

Status: Complete

- [x] unabhängiges Domain- und UI-Review durchführen
- [x] Site-Isolation, ungültige Boardbestätigung und gemeinsamen Headless-Host-Lock korrigieren
- [x] FuncUI-Views im Headless-Harness über `IViewHost.Update` realisieren
- [x] Restore, Locked Restore, Build, Tests und Dependency-Policy ausführen
- [x] Self-contained-App und Windows-Native-AOT-Smoke veröffentlichen und starten

### Verification Plan

- `dotnet restore JiraBoard.slnx`
- `pwsh -NoProfile -File eng/validate.ps1 -Profile Dependency`
- Self-contained-Publish und Start von `JiraBoard.App` für `win-x64`
- Native-AOT-Publish und Start von `JiraBoard.AotSmokeTests` für `win-x64`
- `git diff --check`

### Phase Summary

Release-Build und alle 188 Tests sind grün; Dependency-Policy, Self-contained-Start und Native-AOT-Smoke bestehen. Die bekannten externen `FSharp.Core`-Warnungen `IL3053` und `IL2104` bleiben sichtbar und unverändert.

## Final Recap

VS-001 liefert den getesteten Offline-Projekt-/Sprintkontext zuerst im UiCatalog. Mehrere Boards, gleiche Sprintnamen, leere Sprintzustände, Live-Validierung, Persistenz, Boardinhalt und Replay bleiben den vorgesehenen Folgepaketen vorbehalten.

## Deployment Plan

Keine Bereitstellung. Das abgenommene Paket wird als abgeschlossener, eigenständiger Commit gesichert.
