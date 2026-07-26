# Active State

## Zweck

Dieses Dokument ist der kompakte operative Wiedereinstieg für einen neuen Codex-Kontext. Es zeigt ausschließlich den gegenwärtigen Arbeitszustand und verlinkt auf die maßgeblichen Quellen. Es ist weder ein zweites Backlog noch eine zweite Spezifikation.

Bei Widersprüchen gelten in dieser Reihenfolge:

1. die fünf autoritativen Produkt-, Domain-, Lizenz-, Readiness- und UI-Dokumente aus [AGENTS.md](AGENTS.md);
2. der Item-Status und die Lieferreihenfolge im [Product Backlog](product-backlog.md);
3. diese operative Zustandsprojektion.

## Aktueller Projektsnapshot

| Feld | Aktueller Stand |
|---|---|
| Stand | 26. Juli 2026 |
| Phase | Welle 0 – Produkt- und Ausführungsgrundlage |
| Aktiver Arbeitsauftrag | `FND-001` – Solution-Skelett erzeugen; Status `Proposed` |
| Nächste menschliche Aktion | Ziel, Scope, Risiken, Prüfplan und spätere Abnahmepunkte bestätigen oder Änderungsfeedback geben |
| Nächster Paketkandidat danach | `FND-002` wird erst nach Umsetzung und menschlicher Abnahme von `FND-001` vorgeschlagen |
| Aktuelles Readiness-Gate | G1–G8 offen; in G2 sind DDD-Glossar und Agent-Mensch-Arbeitsflow abgeschlossen |
| Feature-Grenze | keine breite Featureimplementierung vor Abschluss von `VS-007` |
| Repositoryzustand | bisher Dokumentation; noch keine Solution und kein ausführbarer Produktionscode |

## Aktive Arbeitspositionen

Hier stehen ausschließlich `Proposed`, `In Progress`, `In Review` oder `Blocked` geführte Positionen:

| Backlog-ID | Status | Verantwortlich | Aktueller Teilschritt | Nächste konkrete Aktion | Schreibbereich | Letzter Prüfstand |
|---|---|---|---|---|---|---|
| `FND-001` | `Proposed` | Codex | Arbeitsumfang zur Startbestätigung vorgelegt | Menschliche Bestätigung oder Änderungsfeedback abwarten | neues Solution- und Projektgerüst; Statusdokumente | Abhängigkeit `GOV-005` erfüllt; noch keine Umsetzung |

## Aktuelle menschliche Abnahme

Keine; `FND-001` wartet auf Startbestätigung, nicht auf Abnahme.

## Offene Blocker und Entscheidungen

- Es besteht kein technischer Blocker.
- Breite Featureimplementierung bleibt absichtlich durch `VS-007` gesperrt.
- `FND-001` ist vorgeschlagen und darf erst nach ausdrücklicher Startbestätigung umgesetzt werden.

## Letzter Prüfstand

- `GOV-007` wurde am 26. Juli 2026 ausdrücklich menschlich abgenommen, auf `Done` gesetzt und im G2-Gate nachgeführt.
- `GOV-008` wurde am 26. Juli 2026 ausdrücklich menschlich abgenommen und auf `Done` gesetzt.
- Neun Markdown-Dateien und acht PNGs wurden für das Übergabepaket inventarisiert; alle PNGs waren lesbar und das ZIP bestand vor der Abnahme den vollständigen Integritätstest. Das temporäre ZIP wurde anschließend auf ausdrücklichen Wunsch gelöscht und wird nicht erneut angelegt.
- `GOV-005` wurde am 26. Juli 2026 ausdrücklich menschlich abgenommen und auf `Done` gesetzt; die beiden Vorlagen unter `docs/templates/` sind in den G2-Nachweisen verlinkt.
- Es wurden keine Build- oder Testbefehle ausgeführt, weil noch keine Solution existiert und `GOV-005` ausschließlich Markdown betrifft.
- Die Lizenzgrenze, die ausnahmslose FluentAssertions-Sperre, die Readiness-Gates und der Human-in-the-loop-Flow sind in den maßgeblichen Dokumenten verankert.

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
- **Während der Arbeit:** Nach jedem kohärenten Zwischenergebnis nur Teilschritt, nächste Aktion, Schreibbereich und Prüfstand aktualisieren. Keine Akzeptanzkriterien hierher kopieren.
- **Bei Blockade:** Das Item im Backlog auf `Blocked` setzen und hier den konkreten Blocker sowie die benötigte Entscheidung oder Abhängigkeit nennen.
- **Vor Abnahme:** Nach Umsetzung, Tests und Review auf `In Review` setzen; konkrete Abnahmehinweise, Prüfergebnisse, Einschränkungen und Agentenretrospektive vorstellen.
- **Bei Abnahme:** Nur nach ausdrücklicher menschlicher Zustimmung Tests und Nachweise verlinken, Readiness aktualisieren, auf `Done` setzen, die aktive Zeile entfernen und den nächsten Kandidaten bestimmen.
- **Bei Feedback:** Auf `In Progress` zurücksetzen und Umsetzung, Tests, Review und Vorstellung wiederholen. Bei wesentlicher Scopeänderung auf `Proposed` zurücksetzen und erneut bestätigen lassen.
- **Vor Kontextwechsel oder Handover:** Diese Datei muss den tatsächlich verbleibenden Zustand beschreiben; vergangene Sitzungsprotokolle und erledigte Detailaufgaben werden entfernt.
- **Bei Parallelisierung:** Jede aktive Zeile besitzt genau einen Verantwortlichen und einen überschneidungsfreien Schreibbereich. Mehrere Agenten bearbeiten niemals dieselben Dateien.
- **Keine Statusdrift:** Backlog und Readiness-Checkliste bleiben maßgeblich. Diese Datei darf keinen anderen Status behaupten und keine offenen Checkboxen eigenständig als erledigt behandeln.
