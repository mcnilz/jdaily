# UI-007 – Headless-Visualtest-Harness

Der Harness erzeugt reproduzierbare Visualtest- und Messnachweise für Windows 11 x64 mit Standard-Skia und 100 % Betriebssystem-DPI. Er verwendet ausschließlich Produktionsviews aus `JiraBoard.Ui`, akzeptiert Golden Masters nie automatisch und liefert die fehlende Messbasis für `SPK-001`.

## For Future Agents

Als Fortschritt erfolgt, Checkboxen abhaken, Phasenstatus aktualisieren und die jeweilige Zusammenfassung nach ihrer Verifikation eintragen. Vor einer Kontextübergabe beschreibt dieser Plan den verbleibenden Arbeitsstand, während `active-state.md` und der Product Backlog die maßgeblichen Status führen.

## Phase 1: Deterministischen Headless-Host absichern
Status: Completed

- [x] Zuerst fehlschlagende Tests für die feste Windows-, Skia-, DPI-, Locale- und Viewport-Konfiguration ergänzen.
- [x] Die Headless-Testframeworkentscheidung treffen: Der Produkteigentümer entschied einen reinen `Avalonia.Headless`-/xUnit-v3-Host ohne `AvaloniaFact`, weil `Avalonia.Headless.XUnit 11.3.18` `xunit.core 2.4.0` fordert und mit `xunit.v3 3.2.2` kollidiert.
- [x] Die exakten Lizenz- und Dependency-Graphen des gewählten Pfads prüfen und zur aktiven Verwendung freigeben lassen; alle zehn neu beobachteten Testidentitäten sind aktiv inventarisiert, der Lockgraph, die Dependency-Policy und die Notices sind grün.
- [x] Den Headless-Testhost mit ausschließlich freigegebenen, zentral gepinnten Abhängigkeiten einrichten; `HeadlessTestHostTests.fs` bestätigt den Skia-/Headless-Builder über stabiles xUnit v3 ohne `AvaloniaFact`.
- [x] Produktionsviews aus UiCatalog und VisualTests gemeinsam verwenden; der zunächst rote `BoardSurface`-Rendervertrag erfasst die unveränderte Produktionsview bei 1920 × 1080 in einer isolierten Skia-Headless-Sitzung und ist nun grün.

### Verification Plan
- Der neue Umgebungsvertrag schlägt vor der Hostimplementierung fehl und besteht danach.
- `dotnet build JiraBoard.slnx -c Release --no-restore` bleibt warnungsfrei.

### Phase Summary
Der Umgebungsvertrag, der reine xUnit-v3-Headless-Host und die erste echte Produktionsframe-Erfassung sind test-first nachgewiesen. Phase 2 ergänzt darauf Diff-/Baseline-Schutz und Messartefakte.

## Phase 2: Diffs, Baseline-Schutz und BoardSurface-Messung
Status: Completed

- [x] Deterministische BoardSurface-Szenarien bei 1920 × 1080 und hoher Auflösung rendern.
- [x] Diffs als Artefakte ausgeben und einen automatischen Baseline-Update-Versuch als Negativkontrolle sperren.
- [x] CPU, Speicher, Frame-Time und Visual-Tree mit dokumentierter Methode erfassen.

### Verification Plan
- Der Difftest erzeugt ein Artefakt bei kontrollierter Abweichung, ohne die Baseline zu ändern.
- Beide Auflösungen liefern dokumentierte BoardSurface-Messwerte.

### Phase Summary
Die Production-`BoardSurface` rendert bei 1920 × 1080 und 3840 × 2160 als PNG in einer exklusiven, dispatcher-konformen Skia-Sitzung. Bei fehlenden oder abweichenden Referenzen entstehen nur Kandidat und Diffbeschreibung, nie eine automatische `.verified.png`; die Messstichprobe steht in [`ui-007-board-surface-measurements.md`](../docs/validation/ui-007-board-surface-measurements.md).

## Phase 3: Review vorbereiten
Status: Completed

- [x] Readiness und Backlog mit dem Messnachweis synchronisieren; `SPK-001` bleibt bis zur menschlichen Abnahme von `UI-007` als abhängiger Spike geführt.
- [x] Restore, Release-Build, Gesamtsuite, Visualtests und anwendbaren AOT-Smoke ausführen.
- [x] `UI-007` auf `In Review` setzen und konkrete menschliche Abnahmepunkte vorlegen.

### Verification Plan
- Alle angegebenen Befehle enden erfolgreich; Golden Masters wurden nicht automatisch verändert.

### Phase Summary
`dotnet restore`, der warnungsfreie Release-Build, `121/121` Tests und der selbstständige AOT-Smoke sind grün. `UI-007` steht im Backlog auf `In Review`; eine `.verified.png` bleibt bis zur ausdrücklichen menschlichen Entscheidung aus.

## Final Recap
Der xUnit-v3-konforme Headless-Harness verwendet ausschließlich Produktionsviews, serialisiert BoardSurface-Frames bei den zwei Referenzauflösungen und schützt Referenzbilder vor automatischer Änderung. Der Messnachweis dokumentiert CPU, Speicher, Frame-Time und Visual Tree; die Review wartet nur auf die menschliche Abnahme.

## Deployment Plan
_(write when all phases complete: step-by-step deployment instructions)_