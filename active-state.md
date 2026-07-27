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
| Nächste menschliche Aktion | keine; der nächste Kandidat muss vor Umsetzung als `Proposed` vorgestellt und bestätigt werden |
| Nächster Paketkandidat danach | `FND-004` – FluentAssertions-Sperre automatisieren |
| Aktuelles Readiness-Gate | G1–G8 offen; in G2 sind DDD-Glossar und Agent-Mensch-Arbeitsflow abgeschlossen |
| Feature-Grenze | keine breite Featureimplementierung vor Abschluss von `VS-007` |
| Repositoryzustand | `JiraBoard.slnx` mit sechs F#-Projekten; FND-003 und der test-first entstandene xUnit-3.2.2-Harness mit 47 vollständig inventarisierten Lockpaaren sind menschlich abgenommen; fachliche Tests und Produktoberfläche fehlen weiterhin |

## Aktive Arbeitspositionen

Hier stehen ausschließlich `Proposed`, `In Progress`, `In Review` oder `Blocked` geführte Positionen:

Keine.

## Aktuelle menschliche Abnahme

`FND-003` wurde am 27. Juli 2026 für das fertiggestellte `In Review`-Paket ausdrücklich mit dem eigenständigen Wort `Abgenommen` akzeptiert und auf `Done` gesetzt.

## Offene Blocker und Entscheidungen

- Breite Featureimplementierung bleibt absichtlich durch `VS-007` gesperrt.
- Der frühere DataGrid-Blocker ist durch die ausdrückliche Freigabe von `11.3.13` und die synchronisierte Vertragskorrektur aufgelöst.
- `SkiaSharp.NativeAssets.* 2.88.9` und `HarfBuzzSharp.NativeAssets.* 8.3.1.1` wurden am 27. Juli 2026 für diesen Avalonia-/Native-AOT-Einsatz ausdrücklich freigegeben, verbunden mit der Pflicht, die Lizenz- und Attributionstexte vollständig mitzuliefern und in der Anwendung zu verankern. Die Freigabe erweitert die globale Lizenz-Allowlist nicht.

## Letzter Prüfstand

- `FND-003` wurde nach der bestätigten Vertragskorrektur und Feedbackumsetzung am 27. Juli 2026 ausdrücklich menschlich abgenommen und auf `Done` gesetzt: xUnit `3.2.2`, Microsoft Testing Platform, eingebautes `Assert` und `TestResult.assertOk` sind grün; der Harness-Test war vor der Helper-Implementierung rot. Restore und Release-Build liefen mit 0 Fehlern/0 Warnungen, die Suite mit 1/1 Test grün, Lockgraph und Allowlist stimmen mit 47/47 Paaren überein, der Notice-Hash ist reproduzierbar, und der erneute Standards-/Spec-Review meldet keinen blockierenden Befund. `.gitignore` ignoriert `.idea/` nachweislich.
- `GOV-007` wurde am 26. Juli 2026 ausdrücklich menschlich abgenommen, auf `Done` gesetzt und im G2-Gate nachgeführt.
- `GOV-008` wurde am 26. Juli 2026 ausdrücklich menschlich abgenommen und auf `Done` gesetzt.
- Neun Markdown-Dateien und acht PNGs wurden für das Übergabepaket inventarisiert; alle PNGs waren lesbar und das ZIP bestand vor der Abnahme den vollständigen Integritätstest. Das temporäre ZIP wurde anschließend auf ausdrücklichen Wunsch gelöscht und wird nicht erneut angelegt.
- `GOV-005` wurde am 26. Juli 2026 ausdrücklich menschlich abgenommen und auf `Done` gesetzt; die beiden Vorlagen unter `docs/templates/` sind in den G2-Nachweisen verlinkt.
- Für `FND-001` sind die sechs Projekte nach offizieller SDK-Migration in `JiraBoard.slnx` enthalten; Restore, Release-Build und der derzeit noch leere Testlauf sind grün.
- `AGENTS.md` erklärt den NuGet-Eskalationsweg ohne reale lokale Pfade; Skill-Commits vor der Abnahme sind verboten, während ein klares eigenständiges `Abgenommen` den Commit des danach synchronisierten Paketstands automatisch autorisiert.
- Der zweiachsige Review des ungestagten Flow-Diffs gegen Commit `74eca91` meldet nach Angleichung des operativen Pflegeabschnitts keine verbleibenden Befunde; HEAD und Index sind unverändert.
- `FND-001` wurde am 27. Juli 2026 mit einem eigenständigen menschlichen `Abgenommen` ausdrücklich abgenommen, auf `Done` gesetzt und als Commit `9730c9e` automatisch gespeichert.
- `FND-002` wurde am 27. Juli 2026 nach grüner vollständiger Suite und befundfreiem Standards-/Spec-Review ausdrücklich menschlich abgenommen und auf `Done` gesetzt; Lockgraph und Allowlist decken exakt 31 Paare ab, die reproduzierbaren Notices sind in App und Publishes hashgleich.
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
