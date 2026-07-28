# Active State

## Zweck

Dieses Dokument ist der kompakte operative Wiedereinstieg für einen neuen Codex-Kontext. Es zeigt ausschließlich den gegenwärtigen Arbeitszustand und verlinkt auf die maßgeblichen Quellen. Es ist weder ein zweites Backlog noch eine zweite Spezifikation. Ausführliche Prüfergebnisse stehen im [Prüfstand](pruefstand.md) und werden nur bei Bedarf gelesen.

Bei Widersprüchen gelten in dieser Reihenfolge:

1. die fünf autoritativen Produkt-, Domain-, Lizenz-, Readiness- und UI-Dokumente aus [AGENTS.md](AGENTS.md);
2. der Item-Status und die Lieferreihenfolge im [Product Backlog](product-backlog.md);
3. diese operative Zustandsprojektion.

## Aktueller Projektsnapshot

| Feld | Aktueller Stand |
|---|---|
| Stand | 28. Juli 2026 |
| Phase | Welle 0 – UiCatalog, Designsystem und Risikospikes |
| Aktiver Arbeitsauftrag | keiner |
| Nächste menschliche Aktion | Nächsten zulässigen Ready-Kandidaten bestätigen, nachdem er mit Ziel, Scope, Risiken, Prüfplan und Abnahmepunkten vorgeschlagen wurde |
| Nächster Paketkandidat danach | noch zu bestimmen |
| Aktuelles Readiness-Gate | G1–G8 offen; in G2 sind DDD-Glossar und Agent-Mensch-Arbeitsflow abgeschlossen |
| Feature-Grenze | keine breite Featureimplementierung vor Abschluss von `VS-007` |
| Repositoryzustand | `UI-002`, `UI-006`, `FND-008`, `FND-009` und `FND-010` sind menschlich abgenommen und `Done`. `FND-009` korrigiert die fehlende zeilenendungsunabhängige Notice-Reproduzierbarkeit aus GitHub-Actions-Lauf 30350973755 mit kanonischem LF-Output, Regressionstest und erfolgreicher lokaler Gesamtvalidierung. Der Headless-Harness bleibt planmäßig `UI-007`. |

## Aktive Arbeitspositionen

Hier stehen ausschließlich `Proposed`, `In Progress`, `In Review` oder `Blocked` geführte Positionen:

- `FND-005` – Architekturgrenzen testen – `Blocked`. Blocker: Vor dem ersten Domainprojekt existiert keine Domainassembly, die eine Grenzprüfung schützen könnte. Benötigte Abhängigkeit: `DOM-001` (menschlich abgenommen), das die Grenzprüfung (keine Domainreferenzen auf UI, Jira-Transport, HTTP, SQLite und Credential-Implementierungen) inhaltlich mit umsetzt; das Item bleibt zur Nachverfolgung offen.
## Offene Blocker und Entscheidungen

- `FND-005` (Architekturgrenzen testen) ist auf ausdrückliche menschliche Entscheidung vom 27. Juli 2026 zurückgestellt und auf `Blocked` mit neuer Abhängigkeit von `DOM-001` gesetzt; die Zurückstellung wurde am 27. Juli 2026 ausdrücklich abgenommen. Die Domain-Grenzprüfung wird zusammen mit dem ersten Domainprojekt in `DOM-001` eingeführt; Backlog, Readiness-G2 und der technische Handoff wurden entsprechend nachgezogen.
- Breite Featureimplementierung bleibt absichtlich durch `VS-007` gesperrt.
- Der frühere DataGrid-Blocker ist durch die ausdrückliche Freigabe von `11.3.13` und die synchronisierte Vertragskorrektur aufgelöst.
- `SkiaSharp.NativeAssets.* 2.88.9` und `HarfBuzzSharp.NativeAssets.* 8.3.1.1` wurden am 27. Juli 2026 für diesen Avalonia-/Native-AOT-Einsatz ausdrücklich freigegeben, verbunden mit der Pflicht, die Lizenz- und Attributionstexte vollständig mitzuliefern und in der Anwendung zu verankern. Die Freigabe erweitert die globale Lizenz-Allowlist nicht.

## Prüfstand

Ausführliche aktuelle und historische Prüfergebnisse werden im [Prüfstand](pruefstand.md) fortgeführt. Diese Datei enthält nur die für den operativen Wiedereinstieg erforderliche Kurzfassung im Projektsnapshot.

## Wiedereinstieg in einem neuen Kontext

1. [AGENTS.md](AGENTS.md) und anschließend diese Datei lesen.
2. Die für die aktive Position verlinkten Backlog-, Gate- und Spezifikationsabschnitte lesen; vor Implementierung weiterhin alle in `AGENTS.md` vorgeschriebenen Dokumente berücksichtigen.
3. Arbeitsverzeichnis und vorhandene Benutzeränderungen prüfen.
4. Existiert eine aktive Position, mit ihrer „Nächsten konkreten Aktion“ fortfahren. Bei `Proposed` ausschließlich Bestätigung oder Änderungsfeedback verarbeiten; bei `In Review` ausschließlich Abnahme oder Feedback verarbeiten. In beiden Fällen nicht eigenmächtig implementieren.
5. Existiert keine aktive Position, den nächsten zulässigen `Ready`-Kandidaten auf `Proposed` setzen, mit Ziel, Scope, Risiken, Prüfplan und Abnahmepunkten vorstellen und die Bestätigung abwarten.
6. Erst nach Bestätigung TDD-, Lizenz-, AOT-, UiCatalog- und Validierungsregeln aus `AGENTS.md` anwenden.

## Verbindliche Pflege

- **Beim Vorschlag:** Den nächsten zulässigen Kandidaten von `Ready` auf `Proposed` setzen, hier eintragen und dem Menschen Ziel, Scope, Risiken, Prüfplan sowie spätere Abnahmepunkte vorlegen.
- **Nach Startbestätigung:** Das Paket von `Proposed` auf `In Progress` setzen und Verantwortlichen, Teilschritt, nächste Aktion und exklusiven Schreibbereich aktualisieren.
- **Während der Arbeit:** Nach jedem kohärenten Zwischenergebnis nur Teilschritt, nächste Aktion und Schreibbereich hier aktualisieren; ausführliche Prüfergebnisse im [Prüfstand](pruefstand.md) fortführen. Keine Akzeptanzkriterien hierher kopieren.
- **Bei Blockade:** Das Item im Backlog auf `Blocked` setzen und hier den konkreten Blocker sowie die benötigte Entscheidung oder Abhängigkeit nennen.
- **Vor Abnahme:** Nach Umsetzung, Tests und Review auf `In Review` setzen; konkrete Abnahmehinweise, Prüfergebnisse, Einschränkungen und Agentenretrospektive vorstellen.
- **Bei Abnahme:** Nur ein eigenständiges menschliches `Abgenommen` für das aktuelle `In Review`-Paket gilt; Zitate, Beispiele oder Diskussionen des Wortes gelten nicht. Zuerst Tests und Nachweise verlinken, Readiness und Backlog synchronisieren, auf `Done` setzen und die aktive Zeile entfernen; anschließend exakt diesen abgenommenen Zustand automatisch stagen und committen. Eine separate Commit-Anweisung ist nicht erforderlich.
- **Bei Feedback:** Auf `In Progress` zurücksetzen und Umsetzung, Tests, Review und Vorstellung wiederholen. Bei wesentlicher Scopeänderung auf `Proposed` zurücksetzen und erneut bestätigen lassen.
- **Vor Kontextwechsel oder Handover:** Diese Datei muss den tatsächlich verbleibenden Zustand beschreiben; vergangene Sitzungsprotokolle und erledigte Detailaufgaben werden entfernt.
- **Bei Parallelisierung:** Jede aktive Zeile besitzt genau einen Verantwortlichen und einen überschneidungsfreien Schreibbereich. Mehrere Agenten bearbeiten niemals dieselben Dateien.
- **Keine Statusdrift:** Backlog und Readiness-Checkliste bleiben maßgeblich. Diese Datei darf keinen anderen Status behaupten und keine offenen Checkboxen eigenständig als erledigt behandeln.
