# Feature-Notiz: Kurzer fachlicher Titel

| Feld | Wert |
|---|---|
| Status | `Draft` |
| Datum | `YYYY-MM-DD` |
| Backlog | `ABC-NNN` |
| Featuremodul | `Board`, `DailyReplay`, `IssueDetails` oder `Settings` |

## Ziel und Fragestellung

_Welches beobachtbare Fachverhalten soll entstehen und welche Frage beantwortet diese Arbeit?_

## Scope

- Enthalten: _kleinster bestätigter Verhaltensumfang_
- Nicht enthalten: _bewusst unveränderte oder spätere Bereiche_

## Fachsprache und Invarianten

- Begriff: _exakter Begriff aus dem [DDD-Glossar](../../domain-glossary.md)_
- Invariante: _Regel, die immer gelten muss_
- Unmöglicher Zustand: _Zustand, den Typen oder Konstruktoren verhindern sollen_

## Ein- und Ausgaben

| Art | Beschreibung |
|---|---|
| Eingaben | _fachliche Werte und relevante Vorbedingungen_ |
| Ausgaben | _Ergebnis oder Zustandsübergang_ |
| Fehlerfälle | _explizite fachliche Fehler, Abbruch und ungültige Eingaben_ |

## Entscheidung und Alternativen

_Welche kleine fachliche oder schnittstellenbezogene Entscheidung gilt für dieses Feature? Welche ernsthafte Alternative wurde betrachtet und warum verworfen? Für eine dauerhafte oder weitreichende Architekturentscheidung stattdessen ein ADR anlegen und hier verlinken._

## Konsequenzen

- _Auswirkung auf Domain, Ports, UI-/UiCatalog-Szenarien, Persistenz oder Jira-Anti-Corruption-Layer_
- _Risiko oder bewusst akzeptierte Einschränkung_

## Verhalten und Testfälle

- [ ] _Given/When/Then oder präziser Name des zuerst fehlschlagenden Verhaltenstests_
- [ ] _relevanter Fehler-, Abbruch- oder Grenzfall_
- [ ] _falls UI betroffen: deterministisches UiCatalog-Szenario und sichtbarer Komponentenvertrag_

## Nachweise

- _Nach der Umsetzung: Tests, UiCatalog-Szenario, Screenshot, CI-Lauf oder reproduzierbarer Befehl_

