# UI-006 – Tastatur- und Automation-Verträge

Katalogfixtures erhalten nachweisbare Tastatur- und Accessibility-Verträge für die bestehenden Produktionsviews. Produktintegration, Jira-Zugriff und Replay-Animationen bleiben ausgeschlossen.

## For Future Agents

Als Fortschritt erfolgt, Checkboxen abhaken, Phasenstatus aktualisieren und die jeweilige Zusammenfassung nach ihrer Verifikation eintragen. Vor einer Kontextübergabe beschreibt dieser Plan den verbleibenden Arbeitsstand, während `active-state.md` und der Product Backlog die maßgeblichen Status führen.

## Phase 1: Bestehende Verträge und Testlücken erfassen
Status: Complete

- [x] Produktionsviews, Katalogfixtures und vorhandene Testinfrastruktur für Fokus, Eingaben und Automation identifizieren.
- [x] Verhaltenstests für Navigation, Auslöser, Abbruch, Namen und Tooltips vor der Implementierung ergänzen und ihr Scheitern nachweisen.

### Verification Plan
- `dotnet test -c Release --filter "FullyQualifiedName~UiCatalog"` schlägt vor der Implementierung für die neuen Verträge fehl.

### Phase Summary
Bestehende Produktionsviews und Katalogintegration wurden erfasst; vier neue Verhaltenstests decken die Kataloginteraktion und Kartentexte ab.

## Phase 2: Katalogverträge implementieren
Status: Complete

- [x] Roving Focus und die spezifizierten Tastaturaktionen in den Produktionsviews für Katalogfixtures umsetzen.
- [x] Automation-Namen und Tooltips für normale, blockierte und priorisierte Karten ergänzen.

### Verification Plan
- `dotnet test -c Release --filter "FullyQualifiedName~UiCatalog"` besteht vollständig.

### Phase Summary
Die Kataloginteraktion verarbeitet Tab, Pfeile, Leertaste, Enter und Escape; Karten liefern denselben vollständigen Text als Tooltip und Automation-Name.

## Phase 3: Vollständige Validierung und Review vorbereiten
Status: Complete

- [x] Relevante Tests, Release-Build und Gesamtsuite ausführen.
- [x] Status, Prüfnachweise und konkrete menschliche Abnahmeprüfung auf `In Review` vorbereiten.

### Verification Plan
- `dotnet restore JiraBoard.slnx`, `dotnet build JiraBoard.slnx -c Release` und `dotnet test JiraBoard.slnx -c Release --no-build` enden erfolgreich.

### Phase Summary
Restore, Release-Build und Gesamtsuite sind erfolgreich; 107/107 Tests bestanden ohne Warnungen oder Fehler.

## Final Recap
`UI-006` ist umgesetzt, geprüft und am 28. Juli 2026 menschlich abgenommen.

## Deployment Plan
Keine Bereitstellung erforderlich; die Katalogverträge sind Teil der bestehenden Desktop-Anwendung.