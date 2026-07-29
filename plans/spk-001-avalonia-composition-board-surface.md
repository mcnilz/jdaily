# SPK-001 – Avalonia-Composition- und BoardSurface-Spike

Ein isolierter UiCatalog-Spike klärt, ob eine BoardSurface mit drei Statusspalten eine lane-lokale, kontrolliert abbrechbare Replaybewegung über Avalonia Composition umsetzen kann. Der Spike übernimmt keinen Code ungeprüft in die Produktoberfläche und endet mit einer ADR samt Messwerten und Empfehlung.

## For Future Agents

Als Fortschritt erfolgt, Checkboxen abhaken, Phasenstatus aktualisieren und die jeweilige Zusammenfassung nach ihrer Verifikation eintragen. Vor einer Kontextübergabe beschreibt dieser Plan den verbleibenden Arbeitsstand, während `active-state.md` und der Product Backlog die maßgeblichen Status führen.

## Phase 1: Spikevertrag und deterministische BoardSurface
Status: Complete

- [x] Zuerst fehlschlagende Tests für genau drei Statusspalten, lane-lokale Scope-Isolation, Fortschrittsprojektion, sofortigen Abbruch und Reduced Motion ergänzen.
- [x] Die minimale Produktionsview `BoardSurface` mit zentralen Tokens, Skalierung und einer expliziten, testbaren Zustandsprojektion implementieren.
- [x] Einen Composition-Offset-Versuch ohne Reflection oder dynamische Ausdrücke verdrahten und dessen konkrete API-Verfügbarkeit kompilierend belegen.

### Verification Plan
- `dotnet test JiraBoard.slnx -c Release --filter "FullyQualifiedName~BoardSurface"` ist nach der Implementierung grün.
- `dotnet build JiraBoard.slnx -c Release --no-restore` kompiliert die Composition-API-Nutzung ohne Warnungen oder Fehler.

### Phase Summary
Die reine Projektion, Scope-Isolation, Abbruch und Reduced Motion sind test-first umgesetzt. Der direkte Composition-Offset-Aufruf kompiliert ohne Reflection, dynamische Ausdrücke oder Release-Warnungen.

## Phase 2: UiCatalog-Inspektion und Messung
Status: Complete

- [x] Deterministische Fixture und UiCatalog-Szenarien für drei Spalten, Swimlane- und Subtask-Scope, Abbruch sowie Reduced Motion ergänzen.
- [x] Bei 1920 × 1080 und hoher Auflösung CPU, Speicher, Frame-Time und Visual-Tree-Größe mit nachvollziehbarer Methode erfassen.
- [x] Die Scope-Isolation und den Reduced-Motion-Pfad im laufenden Katalog prüfen.

### Verification Plan
- Die neuen Szenarien sind im Katalog registriert und ihre reinen Verträge sind durch Unit-Tests abgedeckt.
- Die Messmethode, Umgebungsdaten und Ergebnisse sind in der ADR nachvollziehbar festgehalten.

### Phase Summary
Katalogstart, Fixture und reine Verträge sind nachgewiesen. Der durch `UI-007` bereitgestellte Headless-Harness erfasst die vier Stichprobenwerte bei 1920 × 1080 und 3840 × 2160; Umgebung, Methode und Ergebnisse stehen in `docs/validation/ui-007-board-surface-measurements.md`.

## Phase 3: Ergebnis und Review vorbereiten
Status: Complete

- [x] ADR mit Fragestellung, Aufbau, Ergebnis, verworfenen Alternativen, Risiken und verbindlicher Folgeentscheidung erstellen.
- [x] Relevante Tests, Release-Build, Gesamtsuite sowie den anwendbaren AOT-Smoke ausführen.
- [x] Backlog, Readiness-Nachweis und Active State auf `In Review` synchronisieren und konkrete Abnahmehinweise vorbereiten.

### Verification Plan
- `dotnet restore JiraBoard.slnx`, `dotnet build JiraBoard.slnx -c Release` und `dotnet test JiraBoard.slnx -c Release --no-build` enden erfolgreich.
- Der bestehende AOT-Smoke bleibt erfolgreich; neue AOT-relevante Warnungen werden nicht unterdrückt.

### Phase Summary
Der Messblocker ist durch den abgenommenen `UI-007`-Harness geschlossen. Restore, Release-Build, 121 Tests und der eigenständige AOT-Smoke sind am 29. Juli 2026 erfolgreich; die ADR hält die Messdaten, Grenzen und Folgeentscheidung fest.

## Final Recap
Die isolierte BoardSurface belegt drei Statusspalten, scope-lokale Projektion, Abbruch und Reduced Motion. Die Composition-API kompiliert reflexionsfrei; der Headless-Messnachweis ergänzt CPU, Speicher, Frame-Time und Visual-Tree für beide Referenzauflösungen. Der Kandidat bleibt bis zur menschlichen Folgeentscheidung außerhalb der Produktoberfläche.

## Deployment Plan
Keine Auslieferung: Der Spike bleibt im UiCatalog. Nach menschlicher Abnahme wird ausschließlich der dokumentierte Paketstand versioniert; eine spätere Produktübernahme benötigt einen zusätzlichen Runtime-Frame-Time-Nachweis.