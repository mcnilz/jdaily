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
| Aktiver Arbeitsauftrag | 'UI-001' – Design Tokens als F#-Code (Done, abgenommen) |
| Nächste menschliche Aktion | UI-001 abgenommen und committet; nächstes eligibles Arbeitspaket auswählen/bestätigen |
| Nächster Paketkandidat danach | 'DOM-009' – Fixture-Sicherheitsprüfung automatisieren (P0; 'DOM-007' ist P1) |
| Aktuelles Readiness-Gate | G1–G8 offen; in G2 sind DDD-Glossar und Agent-Mensch-Arbeitsflow abgeschlossen |
| Feature-Grenze | keine breite Featureimplementierung vor Abschluss von `VS-007` |
| Repositoryzustand | `JiraBoard.slnx` mit sieben F#-Projekten; das Domainprojekt `JiraBoard.Domain` (nur `FSharp.Core`) trägt jetzt zusätzlich das normalisierte Boardereignismodell. `DOM-001`, `DOM-002`, `DOM-003`, `DOM-004` und `DOM-005` sind menschlich abgenommen und `Done`; `FND-005` bleibt `Blocked` mit Abhängigkeit von `DOM-001` und ist inhaltlich durch dessen Grenzprüfung erfüllt |

## Aktive Arbeitspositionen

Hier stehen ausschließlich `Proposed`, `In Progress`, `In Review` oder `Blocked` geführte Positionen:

- `FND-005` – Architekturgrenzen testen – `Blocked`. Blocker: Vor dem ersten Domainprojekt existiert keine Domainassembly, die eine Grenzprüfung schützen könnte. Benötigte Abhängigkeit: `DOM-001` (menschlich abgenommen), das die Grenzprüfung (keine Domainreferenzen auf UI, Jira-Transport, HTTP, SQLite und Credential-Implementierungen) inhaltlich mit umsetzt; das Item bleibt zur Nachverfolgung offen.

## Aktuelle menschliche Abnahme

`DOM-008` (Repräsentative Jira-Fixtures aufbauen) wurde am 27. Juli 2026 mit einem eigenständigen menschlichen `Abgenommen` ausdrücklich abgenommen, auf `Done` gesetzt und automatisch committet. Die Fixtures sind unter `tests/JiraBoard.Tests/Fixtures/` verfügbar und über `FixtureTests.fs` validiert. Gate G4 ist abgeschlossen.

## Offene Blocker und Entscheidungen

- `FND-005` (Architekturgrenzen testen) ist auf ausdrückliche menschliche Entscheidung vom 27. Juli 2026 zurückgestellt und auf `Blocked` mit neuer Abhängigkeit von `DOM-001` gesetzt; die Zurückstellung wurde am 27. Juli 2026 ausdrücklich abgenommen. Die Domain-Grenzprüfung wird zusammen mit dem ersten Domainprojekt in `DOM-001` eingeführt; Backlog, Readiness-G2 und der technische Handoff wurden entsprechend nachgezogen.
- Breite Featureimplementierung bleibt absichtlich durch `VS-007` gesperrt.
- Der frühere DataGrid-Blocker ist durch die ausdrückliche Freigabe von `11.3.13` und die synchronisierte Vertragskorrektur aufgelöst.
- `SkiaSharp.NativeAssets.* 2.88.9` und `HarfBuzzSharp.NativeAssets.* 8.3.1.1` wurden am 27. Juli 2026 für diesen Avalonia-/Native-AOT-Einsatz ausdrücklich freigegeben, verbunden mit der Pflicht, die Lizenz- und Attributionstexte vollständig mitzuliefern und in der Anwendung zu verankern. Die Freigabe erweitert die globale Lizenz-Allowlist nicht.

## Letzter Prüfstand

- `DOM-005` (Boardereignismodell erstellen) wurde am 27. Juli 2026 mit einem eigenständigen menschlichen `Abgenommen` ausdrücklich abgenommen, auf `Done` gesetzt und automatisch committet. Neu im Domainprojekt: [`BoardEvents.fs`](src/JiraBoard.Domain/BoardEvents.fs) mit `BoardEventSource` (`JiraHistory of itemIndex`/`JiraComment`/`DevelopmentInformation`), `LabelChange` (`LabelAdded`/`LabelRemoved`), `BoardEventKind` (`StatusChanged of fromStatus * toStatus`, `AssigneeChanged of assignee option`, `LabelChanged`, `CommentAdded`, `CommitLinked of commitHash`) und dem Record `BoardEvent` (`EventId`, `IssueId`, `OccurredAtUtc: DateTimeOffset`, `Source`, `Kind`); das Modell ist normalisiert, ohne rohe Jira-Changelog-Items, mit `StatusChanged` als beobachtetem Ergebnis. [`Identifiers.fs`](src/JiraBoard.Domain/Identifiers.fs) trägt zusätzlich die starken Identitäten `StatusId` und `BoardEventId`. Verhaltenstests [`BoardEventTests.fs`](tests/JiraBoard.Tests/BoardEventTests.fs) belegen Status-Übergang, Konstruier-/Unterscheidbarkeit aller Arten, Assignee inkl. `None`, Label add/remove, Quelle und `BoardEventId`-Identität. TDD-Rot war bewiesen (63 Kompilierfehler wegen fehlender Ereignistypen). `dotnet build -c Release` (gesamte `JiraBoard.slnx`) 0 Fehler/0 Warnungen; `dotnet test -c Release --no-build` 37/37 grün (29 bisher + 8 neu), der statische `DomainBoundaryTests` bleibt grün. Kein neues Paket, daher Lizenzinventar und `THIRD-PARTY-NOTICES.txt` unverändert. Mitcommittet wurde auf ausdrücklichen Wunsch das token-sparende Hilfsskript [`eng/active-state.ps1`](eng/active-state.ps1) samt Verweis in `AGENTS.md`.

- `DOM-004` (Multi-Sprint-Scope projizieren) wurde am 27. Juli 2026 mit einem eigenständigen menschlichen `Abgenommen` ausdrücklich abgenommen, auf `Done` gesetzt und automatisch committet. Neu im Domainprojekt: [`SprintProjection.fs`](src/JiraBoard.Domain/SprintProjection.fs) mit dem Eingabetyp `SprintBoardIssue` (`IssueId`, `Position: BoardPosition`, `Sprints: Set<SprintId>`) und der puren, auto-geöffneten Funktion `projectSprintScope : SprintScope -> SprintBoardIssue list -> SprintBoardIssue list`. Die Projektion bildet zuerst die globale `resolveBoardOrder` über die unveränderten `BoardPosition`-Werte, filtert dann per `stableSubsequence` nach Scope (`AllActiveSprints`: nicht-leere Sprintmenge; `ActiveSprint sprintId`: Menge enthält `sprintId`) und entfernt Duplikate nach `IssueId` reihenfolgeerhaltend; die Ausgabe ist damit immer eine stabile Teilfolge der globalen `ResolvedBoardOrder` und nie eine Verkettung einzelner Sprintantworten (Glossar Z. 78, Entscheidung Z. 168). Verhaltenstests [`SprintProjectionTests.fs`](tests/JiraBoard.Tests/SprintProjectionTests.fs) belegen Dedup eines Multi-Sprint-Issues auf eine globale Position, stabile Teilfolge trotz abweichender Eingabereihenfolge, exakten Einzelsprint-Filter sowie die Randfälle (leere Eingabe, Sprint ohne Issues, Issue ohne Sprintzugehörigkeit). `dotnet build -c Release` (gesamte `JiraBoard.slnx`) 0 Fehler/0 Warnungen; `dotnet test -c Release --no-build` 29/29 grün (21 bisher + 8 neu), der statische `DomainBoundaryTests` bleibt grün. Kein neues Paket, daher Lizenzinventar und `THIRD-PARTY-NOTICES.txt` unverändert.

- `DOM-003` wurde am 27. Juli 2026 mit einem eigenständigen menschlichen `Abgenommen` ausdrücklich abgenommen, auf `Done` gesetzt und automatisch committet. Neu im Domainprojekt: [`BoardOrder.fs`](src/JiraBoard.Domain/BoardOrder.fs) mit `JiraRank` (opaker, nicht numerisch interpretierter Vergleichswert; keine feste `customfield_*`-ID), `BoardOrdinal` (monotone Position der unveränderten API-Reihenfolge einer Revision), `BoardPosition` sowie den puren, auto-geöffneten Funktionen `resolveBoardOrder` und `stableSubsequence`. `resolveBoardOrder` ist die stabile `ResolvedBoardOrder`-Kaskade: zuerst verifizierter `JiraRank`, dann `BoardOrdinal`, und nur bei fehlenden/gleichen beiden der `IssueKey` als letzter Notanker (Entscheidung 2026-07-20). `stableSubsequence` entfernt Elemente ohne Änderung der relativen Reihenfolge der verbleibenden. Verhaltenstests [`BoardOrderTests.fs`](tests/JiraBoard.Tests/BoardOrderTests.fs) belegen Rank-Sortierung, Ordinal-Tiebreak, Key als letzten Anker, Stabilität bei gleichen Positionen sowie die reihenfolgeerhaltende und identitätswahrende Teilfolge. TDD-Rot war bewiesen (Kompilierfehler wegen fehlender Typen/Funktionen vor der Umsetzung). `dotnet build -c Release` (gesamte `JiraBoard.slnx`) 0 Fehler/0 Warnungen; `dotnet test -c Release --no-build` 21/21 grün (14 bisher + 7 neu), der statische `DomainBoundaryTests` bleibt grün. Kein neues Paket, daher Lizenzinventar und `THIRD-PARTY-NOTICES.txt` unverändert.

- `DOM-002` wurde am 27. Juli 2026 mit einem eigenständigen menschlichen `Abgenommen` ausdrücklich abgenommen, auf `Done` gesetzt und einschließlich der ausdrücklich gewünschten `.gitignore`-Anpassung automatisch committet. Neu im Domainprojekt: [`IssueHierarchy.fs`](src/JiraBoard.Domain/IssueHierarchy.fs) mit `IssueType` (`Id`, `Name`, `HierarchyLevel`, `IsSubtask`), `WorkItemLevel` (`ParentLevel`/`StandardLevel`/`SubtaskLevel`) und der puren, auto-geöffneten `classify`-Funktion; das Level entsteht ausschließlich aus Metadaten (Subtask-Kennzeichen dominiert, danach `HierarchyLevel > 0` -> Parent, sonst Standard), nie aus dem Typnamen. Verhaltenstests [`IssueHierarchyTests.fs`](tests/JiraBoard.Tests/IssueHierarchyTests.fs) belegen unter anderem, dass Story, Bug, Task und ein Custom Standard-Typ dieselbe Standard-Swimlane-Regel ergeben. TDD-Rot war bewiesen (31 Kompilierfehler vor der Umsetzung). `dotnet build -c Release` -> 0 Fehler/0 Warnungen; `dotnet test -c Release --no-build` -> 14/14 grün (9 bisher + 5 neu), der statische `DomainBoundaryTests` bleibt grün. Kein neues Paket -> Lizenzinventar und `THIRD-PARTY-NOTICES.txt` unverändert.

- `DOM-001` wurde am 27. Juli 2026 mit einem eigenständigen menschlichen `Abgenommen` ausdrücklich abgenommen, auf `Done` gesetzt und automatisch committet. Neu: Domainprojekt `JiraBoard.Domain` (`Identifiers.fs` mit `SiteId`/`ProjectId`/`BoardId`/`SprintId`/`IssueId`/`IssueKey`, `Board.fs` mit `SprintScope` und `BoardContext` inkl. `SiteId`), nur `FSharp.Core` referenziert; in `JiraBoard.slnx` als innerste Schicht aufgenommen. Zugleich die zurückgestellte `FND-005`-Grenze umgesetzt: statischer, reflexionsfreier [`DomainBoundaryTests`](tests/JiraBoard.Tests/DomainBoundaryTests.fs) liest das Domain-Dependency-Lock (direkt + transitiv) und schlägt bei verbotenen Referenzen fehl. TDD-Rot war bewiesen (Tests kompilierten vor dem Domainprojekt nicht); die Negativkontrolle (temporäre Avalonia-Referenz) machte den Grenztest rot, der bereinigte Stand ist grün. `dotnet restore`/`build -c Release`/`test -c Release --no-build` aus bereinigtem Zustand: 0 Fehler/0 Warnungen, 9/9 Tests grün. Kein neues Paket, daher Lizenzinventar und `THIRD-PARTY-NOTICES.txt` unverändert.

- Die `FND-005`-Zurückstellung wurde am 27. Juli 2026 mit einem eigenständigen menschlichen `Abgenommen` akzeptiert. Es ist eine reine Planungs-/Dokumentationsänderung an `product-backlog.md`, `implementation-readiness-checklist.md`, `avalonia-fsharp-funcui-stack-handoff.md` und `active-state.md` ohne Produktionscode; daher kein Build/Test erforderlich. FND-005 bleibt als Item `Blocked` mit Abhängigkeit von `DOM-001`.

- `FND-004` wurde nach Feedback auf einen 58-zeiligen Scanner und eine 49-zeilige kombinierte Negativkontrolle reduziert, am 27. Juli 2026 ausdrücklich menschlich abgenommen und auf `Done` gesetzt; es existiert bewusst kein lokaler MSBuild-Hook. Direkte, transitive, Lock-/Assets-, Alias-, Workflow- und generische Wrapper-Fixtures liefern Exit-Code 1 und werden wieder entfernt; der echte Repository-Scan ist grün. Restore und Release-Build liefen mit 0 Fehlern/0 Warnungen, xUnit mit 1/1 Test grün, und der wiederholte Standards-/Spec-Review meldet keine verbleibenden Befunde. Die GitHub-Actions-Verdrahtung bleibt bei `FND-006`.
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
