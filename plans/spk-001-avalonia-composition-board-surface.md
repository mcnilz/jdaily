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
Status: In progress

- [x] Deterministische Fixture und UiCatalog-Szenarien für drei Spalten, Swimlane- und Subtask-Scope, Abbruch sowie Reduced Motion ergänzen.
- [ ] Bei 1920 × 1080 und hoher Auflösung CPU, Speicher, Frame-Time und Visual-Tree-Größe mit nachvollziehbarer Methode erfassen. Blockiert durch `UI-007`: kanonisches Golden-Master-Betriebssystem, Renderer und DPI sind nicht entschieden.
- [x] Die Scope-Isolation und den Reduced-Motion-Pfad im laufenden Katalog prüfen.

### Verification Plan
- Die neuen Szenarien sind im Katalog registriert und ihre reinen Verträge sind durch Unit-Tests abgedeckt.
- Die Messmethode, Umgebungsdaten und Ergebnisse sind in der ADR nachvollziehbar festgehalten.

### Phase Summary
Katalogstart, Fixture und reine Verträge sind nachgewiesen. Die Messbaseline wird nicht geschätzt und wartet auf den `UI-007`-Harness.

## Phase 3: Ergebnis und Review vorbereiten
Status: In progress

- [x] ADR mit Fragestellung, Aufbau, Ergebnis, verworfenen Alternativen, Risiken und verbindlicher Folgeentscheidung erstellen.
- [ ] Relevante Tests, Release-Build, Gesamtsuite sowie den anwendbaren AOT-Smoke ausführen.
- [ ] Backlog, Readiness-Nachweis und Active State auf `In Review` synchronisieren und konkrete Abnahmehinweise vorbereiten. Blockiert bis die Messbaseline vorliegt.

### Verification Plan
- `dotnet restore JiraBoard.slnx`, `dotnet build JiraBoard.slnx -c Release` und `dotnet test JiraBoard.slnx -c Release --no-build` enden erfolgreich.
- Der bestehende AOT-Smoke bleibt erfolgreich; neue AOT-relevante Warnungen werden nicht unterdrückt.

### Phase Summary
_(write when phase completes)_

## Final Recap
_(write when all phases complete: summary of the entire piece of work)_

## Deployment Plan
_(write when all phases complete: step-by-step deployment instructions)_