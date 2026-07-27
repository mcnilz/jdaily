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
| Stand | 27. Juli 2026 |
| Phase | Welle 0 – Produkt- und Ausführungsgrundlage |
| Aktiver Arbeitsauftrag | keiner |
| Nächste menschliche Aktion | keine offene Abnahme; nächster Arbeitsauftrag wird vor Umsetzung vorgeschlagen |
| Nächster Paketkandidat danach | `FND-002` steht auf `Ready` und ist der nächste zulässige Vorschlag |
| Aktuelles Readiness-Gate | G1–G8 offen; in G2 sind DDD-Glossar und Agent-Mensch-Arbeitsflow abgeschlossen |
| Feature-Grenze | keine breite Featureimplementierung vor Abschluss von `VS-007` |
| Repositoryzustand | `JiraBoard.slnx` mit sechs minimalen F#-Projekten; noch keine Drittanbieterpakete, Tests oder Produktoberfläche |

## Aktive Arbeitspositionen

Hier stehen ausschließlich `Proposed`, `In Progress`, `In Review` oder `Blocked` geführte Positionen:

| Backlog-ID | Status | Verantwortlich | Aktueller Teilschritt | Nächste konkrete Aktion | Schreibbereich | Letzter Prüfstand |
|---|---|---|---|---|---|---|
| – | – | – | – | – | – | – |

## Aktuelle menschliche Abnahme

Keine.

## Offene Blocker und Entscheidungen

- Es besteht kein technischer Blocker.
- Breite Featureimplementierung bleibt absichtlich durch `VS-007` gesperrt.
- `FND-002` steht nach erfüllter Abhängigkeit auf `Ready` und muss vor der Umsetzung als nächster Arbeitsauftrag vorgeschlagen und bestätigt werden.

## Letzter Prüfstand

- `GOV-007` wurde am 26. Juli 2026 ausdrücklich menschlich abgenommen, auf `Done` gesetzt und im G2-Gate nachgeführt.
- `GOV-008` wurde am 26. Juli 2026 ausdrücklich menschlich abgenommen und auf `Done` gesetzt.
- Neun Markdown-Dateien und acht PNGs wurden für das Übergabepaket inventarisiert; alle PNGs waren lesbar und das ZIP bestand vor der Abnahme den vollständigen Integritätstest. Das temporäre ZIP wurde anschließend auf ausdrücklichen Wunsch gelöscht und wird nicht erneut angelegt.
- `GOV-005` wurde am 26. Juli 2026 ausdrücklich menschlich abgenommen und auf `Done` gesetzt; die beiden Vorlagen unter `docs/templates/` sind in den G2-Nachweisen verlinkt.
- Für `FND-001` sind die sechs Projekte nach offizieller SDK-Migration in `JiraBoard.slnx` enthalten; Restore, Release-Build und der derzeit noch leere Testlauf sind grün.
- `AGENTS.md` erklärt den NuGet-Eskalationsweg ohne reale lokale Pfade; Skill-Commits vor der Abnahme sind verboten, während ein klares eigenständiges `Abgenommen` den Commit des danach synchronisierten Paketstands automatisch autorisiert.
- Der zweiachsige Review des ungestagten Flow-Diffs gegen Commit `74eca91` meldet nach Angleichung des operativen Pflegeabschnitts keine verbleibenden Befunde; HEAD und Index sind unverändert.
- `FND-001` wurde am 27. Juli 2026 mit einem eigenständigen menschlichen `Abgenommen` ausdrücklich abgenommen und auf `Done` gesetzt.
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
- **Bei Abnahme:** Nur ein eigenständiges menschliches `Abgenommen` für das aktuelle `In Review`-Paket gilt; Zitate, Beispiele oder Diskussionen des Wortes gelten nicht. Zuerst Tests und Nachweise verlinken, Readiness und Backlog synchronisieren, auf `Done` setzen und die aktive Zeile entfernen; anschließend exakt diesen abgenommenen Zustand automatisch stagen und committen. Eine separate Commit-Anweisung ist nicht erforderlich.
- **Bei Feedback:** Auf `In Progress` zurücksetzen und Umsetzung, Tests, Review und Vorstellung wiederholen. Bei wesentlicher Scopeänderung auf `Proposed` zurücksetzen und erneut bestätigen lassen.
- **Vor Kontextwechsel oder Handover:** Diese Datei muss den tatsächlich verbleibenden Zustand beschreiben; vergangene Sitzungsprotokolle und erledigte Detailaufgaben werden entfernt.
- **Bei Parallelisierung:** Jede aktive Zeile besitzt genau einen Verantwortlichen und einen überschneidungsfreien Schreibbereich. Mehrere Agenten bearbeiten niemals dieselben Dateien.
- **Keine Statusdrift:** Backlog und Readiness-Checkliste bleiben maßgeblich. Diese Datei darf keinen anderen Status behaupten und keine offenen Checkboxen eigenständig als erledigt behandeln.
