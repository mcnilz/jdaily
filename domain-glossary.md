# DDD-Glossar und Ubiquitous Language

## Status und Zweck

Dieses Dokument ist die verbindliche gemeinsame Fachsprache für Produkt, Domaincode, Tests, UI-Szenarien und Agentenkommunikation. Es konkretisiert den [technischen Handover](avalonia-fsharp-funcui-stack-handoff.md), ersetzt ihn aber nicht. Bei einem Widerspruch zwischen beiden Dokumenten wird nicht geraten: Die Umsetzung hält an, und die Dokumente werden nach einer Produktentscheidung gemeinsam korrigiert.

Das Glossar beschreibt fachliche Begriffe und ihre Invarianten. Jira-Transportfelder, Avalonia-Controls, HTTP-Endpunkte, SQLite-Tabellen und konkrete Persistenzformate gehören nicht in das Domainmodell. Begriffe werden ergänzt oder geändert, sobald ein Feature eine neue fachliche Unterscheidung benötigt.

## Sprachregeln

- Codenamen und F#-Typen verwenden die in diesem Dokument angegebenen englischen Begriffe.
- Deutsche UI-Texte dürfen natürlich formuliert sein, dürfen aber keine abweichende Fachbedeutung einführen.
- `Ticket` ist im Gespräch und in UI-Texten ein verständliches Synonym für ein Jira-Issue. Im Domaincode lautet der Oberbegriff `Issue`.
- `Story` ist ausschließlich ein möglicher Issue-Typ. Es ist kein Synonym für `StandardIssue`, `Swimlane` oder deren Header.
- Namen und lokalisierte Texte sind Darstellung. Identität und Domainverzweigungen verwenden stabile IDs und fachliche Typen.
- Ein Begriff erhält nur dann einen technischen Zusatz wie `Jira`, `Visual` oder `Daily`, wenn damit zwei tatsächlich verschiedene Konzepte auseinandergehalten werden.
- Neue Begriffe werden zuerst hier geklärt und danach konsistent in Typen, Testnamen, UiCatalog-Szenarien und Dokumentation verwendet.

## Fachliche Bereiche

| Bereich | Verantwortung | Kennt ausdrücklich nicht |
|---|---|---|
| `Board` | aktueller Boardzustand, Hierarchie, Spaltenprojektion und Jira-Reihenfolge | Avalonia, HTTP und Jira-DTOs |
| `DailyReplay` | Zeitraum, Ereignisnormalisierung, Scope und historische Wiedergabe | konkrete Animationstechnik |
| `IssueDetails` | read-only Details eines ausgewählten Issues einschließlich Parent-Kontext | Boardlayout und Transition-UI |
| `Settings` | board- und anwendungsbezogene Policies und Anzeigepräferenzen | Credential-Geheimnisse |
| `NavigationContext` | aktive Site, Projekt, Board und Sprintscope | sichtbare Namen als Identität |
| `Jira Adapter` | Jira Cloud transportieren und in die gemeinsame Sprache übersetzen | Produkt- und UI-Entscheidungen |

Diese Bereiche sind Feature- und Sprachgrenzen eines modularen Monolithen. Konkrete Aggregate-Grenzen werden nicht vorab aus DDD-Zeremonie behauptet; sie entstehen testgetrieben aus Konsistenzanforderungen.

## Identität und Navigationskontext

| Begriff | Codebegriff | Verbindliche Bedeutung und Invarianten |
|---|---|---|
| Jira-Site | `JiraSite` / `SiteId` | Eine Jira-Cloud-Instanz. Im MVP ist genau eine Site aktiv. Server und Data Center sind ausgeschlossen. Site-URL oder Anzeigename ersetzen keine stabile Site-Identität. |
| Projekt | `Project` / `ProjectId` | Ein zugängliches Jira-Projekt. Im MVP muss es Team-managed und für Scrum geeignet sein. Der Projektname ist keine Identität. |
| Board | `Board` / `BoardId` | Das bestätigte Scrum-Board des ausgewählten Projekts. Alle boardbezogenen Einstellungen und Snapshots sind zusätzlich durch die aktive Site isoliert. |
| Sprint | `Sprint` / `SprintId` | Ein Jira-Sprint des ausgewählten Boards. Das MVP zeigt nur aktive Sprints im Sprint-Menü. Zukünftige und geschlossene Sprints gehören nicht zum wählbaren Scope. |
| Aktiver Sprint | `ActiveSprint` | Ein Sprint, dessen von Jira gelieferter Zustand aktiv ist. Gleichnamige aktive Sprints bleiben durch ihre IDs verschieden. |
| Sprintscope | `SprintScope` | Exakt `AllActiveSprints` oder `ActiveSprint of SprintId`. Er wird von Board, Pending-Zähler und Replay gemeinsam verwendet. |
| Boardkontext | `BoardContext` | Die bestätigte Kombination aus aktiver Site, Projekt, Board und Sprintscope. Im vorhandenen F#-Entwurf kann die Site durch den äußeren aktiven Site-Kontext bereitgestellt werden; fachlich gehört sie dennoch zur Isolation. |
| Kontextgeneration | `ContextGeneration` | Interne Generation eines geladenen Boardkontexts. Antworten einer älteren Generation sind veraltet und dürfen den neuen Kontext nicht verändern. |
| Letzter gültiger Kontext | `LastValidBoardContext` | Zuletzt bestätigter und lokal gespeicherter Boardkontext. Er wird beim Start sofort wiederhergestellt und anschließend gegen Jira validiert. |

## Issues und Hierarchie

| Begriff | Codebegriff | Verbindliche Bedeutung und Invarianten |
|---|---|---|
| Issue | `Issue` / `IssueId` | Domain-Oberbegriff für ein Jira-Issue aller relevanten Hierarchieebenen. Die stabile Jira-Issue-ID ist Identität; der lesbare Key ist es nicht. |
| Issue-Key | `IssueKey` | Lesbarer Schlüssel wie `APP-142`. Er wird angezeigt und kann letzter deterministischer Notfall-Fallback sein, ersetzt aber niemals `IssueId`. |
| Issue-Typ | `IssueType` | Von Jira konfigurierbarer Typ mit ID, Name, `hierarchyLevel` und Subtask-Kennzeichen. Typnamen dürfen keine Domainverzweigung auslösen. |
| Hierarchieebene | `WorkItemLevel` | Normalisierte Einordnung in `ParentLevel`, `StandardLevel` oder `SubtaskLevel`. Sie wird aus Jira-Metadaten gemappt, nicht aus Namen geraten. |
| Parent-Issue | `ParentIssue` | Issue oberhalb von Level 0, im MVP typischerweise ein Epic. Es ist auf der Boardoberfläche und im Replay nicht selbst sichtbar. |
| Parent-Kontext | `ParentContext` | Schlanke, read-only Information über das Parent-Issue eines Standard-Issues, mindestens ID, Key und Titel. Sie erscheint nur im Issue-Modal. |
| Standard-Issue | `StandardIssue` | Issue auf Level 0, beispielsweise Story, Bug, Task oder benutzerdefinierter Standardtyp. Jedes Standard-Issue erzeugt genau eine Swimlane. |
| Story | `Story` als Wert eines `IssueType` | Ein möglicher Jira-Typ eines Standard-Issues. Eine Story besitzt gegenüber Bug, Task oder Custom Standard Issue keine besondere Swimlane-Logik. |
| Subtask | `Subtask` | Issue unterhalb Level 0 beziehungsweise mit Jira-Subtask-Kennzeichen. Es gehört anhand der stabilen Parent-ID genau einer Standard-Issue-Swimlane. |
| Swimlane-Root | `SwimlaneRoot` / `rootIssueId` | Das Standard-Issue, das eine Swimlane begründet. `Root` bedeutet hier nicht Epic oder Projektwurzel. |
| Fehlender Parent | `MissingStandardParent` | Der Standard-Parent eines Subtasks wurde noch nicht geladen, existiert nicht oder ist nicht zugreifbar. Zuerst wird er gezielt nachgeladen. |
| Fallback-Swimlane | `MissingParentSwimlane` | Sichtbare, diagnostizierbare Swimlane `Parent nicht verfügbar` für Subtasks ohne auflösbaren Standard-Parent. Solche Subtasks werden nie still ausgeblendet. |

## Boardprojektion und Reihenfolge

| Begriff | Codebegriff | Verbindliche Bedeutung und Invarianten |
|---|---|---|
| Swimlane | `Swimlane` | Vertikaler Boardbereich eines Standard-Issues. Er besteht aus `SwimlaneHeader` und den zugehörigen Subtask-Zellen über alle sichtbaren, kombinierten und eingeklappten Spalten. |
| Swimlane-Header | `SwimlaneHeader` | Kopf einer Swimlane für jedes Standard-Issue. Der frühere Name `StorySwimlaneHeader` ist verboten, weil er Bugs, Tasks und Custom Standard Issues fälschlich ausschließt. |
| Boardspalte | `BoardColumn` / `ColumnId` | Sichtbare Workflowspalte des ausgewählten Jira-Boards. Eine Boardspalte kann mehrere Jira-Status-IDs enthalten. |
| Jira-Status | `JiraStatus` / `StatusId` | Workflowzustand eines Issues. Statusname ist Darstellung; Zuordnung und Identität verwenden stabile Status-IDs. |
| Spaltenzuordnung | `ColumnMapping` | Abbildung einer oder mehrerer Jira-Status-IDs auf eine Boardspalte. Ein unbekannter Status führt zu einem sichtbaren diagnostischen Fallback. |
| Aktueller Boardzustand | `CurrentBoardState` | Zuletzt erfolgreich angewendeter, sichtbarer Zustand. Erkannte Pending Events verändern ihn erst durch `Refresh`. |
| Boardreihenfolge | `BoardOrder` | Vom ausgewählten Jira-Board vorgegebene relative Reihenfolge von Standard-Issues und Subtasks. Sie ist fachliche Information und kein UI-Zufall. |
| Jira-Rank | `JiraRank` | Dynamisch pro Board erkanntes, von Jira geliefertes vergleichbares Ranking-Merkmal. Es wird weder numerisch interpretiert noch lokal erzeugt; eine feste `customfield_*`-ID ist verboten. |
| Boardordinal | `BoardOrdinal` | Beim Einlesen monoton vergebene Position in der unveränderten API-Reihenfolge über alle Seiten. Sie gilt innerhalb derselben geladenen Boardrevision beziehungsweise ihres Snapshots und ist kein global dauerhafter Rang. |
| Aufgelöste Reihenfolge | `ResolvedBoardOrder` | Globale Jira-Boardreihenfolge, soweit der unterstützte Leseweg sie liefert. Für Subtasks beziehungsweise einen notwendigen Merge gilt der verifizierte Jira-Rank, danach `BoardOrdinal`; nur wenn auch dieser fehlt, folgt der Issue-Key ordinal als letzter Notanker. |
| Stabile Teilfolge | `StableSubsequence` | Ergebnis von Scope, Filter, Collapse oder Replay, bei dem Elemente entfernt oder anders dargestellt werden, ohne die relative Reihenfolge der verbleibenden Issues zu verändern. |
| Alle aktiven Sprints | `AllActiveSprints` | Vereinigung der Issue-IDs aller aktiven Sprints, dedupliziert nach `IssueId` und als stabile Teilfolge der globalen Boardreihenfolge projiziert. Sprintantworten werden niemals als Darstellungsreihenfolge aneinandergehängt. |
| Eingeklappte Spalte | `CollapsedColumn` | Schmale Darstellung einer Boardspalte. Jeder Subtask bleibt einzeln und in seiner Swimlane erhalten; Collapse ist keine Aggregation und verändert keine Reihenfolge. |
| Review-Track | `ReviewTrack` | Lokale kompakte Projektion zweier bestätigter benachbarter Review-Spalten. Er verändert weder Jira-Workflow noch Status oder Reihenfolge und fällt bei ungültigem Mapping auf normale Spalten zurück. |

## Daily und Zeit

| Begriff | Codebegriff | Verbindliche Bedeutung und Invarianten |
|---|---|---|
| Daily-Uhrzeit | `DailyScheduleTime` | Pro Board konfigurierte reguläre Uhrzeit des Daily Meetings. Sie verschiebt den Bezugspunkt niemals automatisch. |
| Arbeitstag | `Workday` | Für das MVP Montag bis Freitag. Wochenenden werden bei der Ermittlung des vorherigen regulären Daily-Tages übersprungen. |
| Daily-Bezugspunkt | `DailyReferencePoint` | Gespeicherter Zeitpunkt des zuletzt fachlich abgeschlossenen Dailies. Er ist der Start des nächsten Replay-Zeitraums. |
| Daily-Zeitraum | `DailyReplayPeriod` | Fachlicher Zeitraum vom `DailyReferencePoint` bis zum für den Replay-Lauf erfassten Jetztzeitpunkt. Look-behind/-ahead dient nur der Normalisierung und gehört nicht zur sichtbaren Wiedergabe. |
| Daily abschließen | `CompleteDaily` | Explizite, idempotente Fachaktion. Sie speichert den Snapshot erfolgreich und setzt erst danach atomar den neuen Bezugspunkt. Die geplante Uhrzeit allein führt diese Aktion nicht aus. |
| Manuelle Tageskorrektur | `PreviousDailyDateOverride` | Bewusste Wahl des vorherigen Daily-Tages für Feiertage, ausgefallene oder abweichende Dailies. |
| Erfasstes Jetzt | `ReplayNow` | Beim Vorbereiten eines Replays unveränderlich erfasster Endzeitpunkt. Ein laufendes Replay wächst nicht durch neu eintreffende Ereignisse. |

## Ereignisse und Replay

| Begriff | Codebegriff | Verbindliche Bedeutung und Invarianten |
|---|---|---|
| Kanonische Historie | `CanonicalHistory` | Vollständige normalisierte, deduplizierte Ereignishistorie aus verfügbaren Quellen. Replay-Filter verändern sie nicht. |
| Boardereignis | `BoardEvent` | Normalisierte fachliche Änderung mit stabiler Event-ID, Issue-ID, Zeitstempel, Quelle und Art. Die View erhält keine rohen Jira-Changelog-Items. |
| Ereignisquelle | `BoardEventSource` | Nachvollziehbare Herkunft, etwa Jira-History mit Item-Index, Jira-Kommentar oder offiziell lesbare Development Information. |
| Statusänderung | `StatusChanged` | Beobachtetes Boardereignis von einem Status zu einem anderen. Es ist das Ergebnis einer Änderung, nicht der Befehl an Jira. |
| Transition | `Transition` / `TransitionId` | Von Jira angebotene Workflowaktion, die einen Statuswechsel auslösen kann und gegebenenfalls Pflichtfelder besitzt. Sie ist getrennt von Ranking und vom beobachteten `StatusChanged`-Ereignis. |
| Daily Replay | `DailyReplay` | Deterministische historische Wiedergabe der normalisierten Ereignisse eines Scopes im Daily-Zeitraum. Ohne laufendes Replay bleibt ausschließlich der aktuelle Zustand sichtbar. |
| Replay-Scope | `ReplayScope` | Exakt `SwimlaneScope` für Root plus Subtasks oder `SubtaskScope` für genau einen Subtask. Es existiert höchstens ein aktiver Scope. |
| Replay-Ereignisstrom | `ReplayEventStream` | Aus der kanonischen Historie für Zeitraum, Noise Policy und Scope abgeleitete unveränderliche Sequenz eines Replay-Laufs. Neue Pending Events werden nicht hineinsortiert. |
| Replay-Generation | `ReplayGeneration` | Interne Identität eines Replay-Laufs. Abbruch, Refresh oder Kontextwechsel invalidiert sie; spätere Callbacks einer alten Generation sind wirkungslos. |
| Status-Bounce | `StatusBounce` | Zwei exakt inverse Statusänderungen desselben Issues innerhalb des konfigurierten Fensters ohne dazwischenliegende Statusänderung. Nur die Replay-Projektion darf sie unterdrücken. |
| Bounce-Fenster | `StatusBounceWindow` | Boardbezogene Policy `Disabled` oder validierte Dauer von 1 bis 30 Minuten; Standard sind 5 Minuten. Die Grenze ist inklusive. |
| Replay-Noise-Policy | `ReplayNoisePolicy` | Beim Replay-Start unveränderlich erfasste Regeln zur Bereinigung des Replay-Stroms. Änderungen gelten erst für den nächsten Lauf. |
| Zeitgleiche Ereignisse | `SimultaneousEvents` | Ereignisse mit gleichem normalisiertem UTC-Zeitpunkt. Sie werden sequenziell und deterministisch geordnet: Quellreihenfolge, Scope-/Boardreihenfolge, Ereignisart, Event-ID. Bei fehlendem Rank gilt `BoardOrdinal` vor Issue-Key. |
| Reduced Motion | `ReducedMotion` | Darstellungsprofil ohne räumliche Flugbahnen, Rotation, Overshoot oder Partikel. Fachliche Ereignisse, Reihenfolge und Bedienbarkeit bleiben vollständig erhalten. |

## Synchronisation und lokale Daten

| Begriff | Codebegriff | Verbindliche Bedeutung und Invarianten |
|---|---|---|
| Polling | `Polling` | Best-Effort-Prüfung auf neue Jira-Ereignisse. Es erkennt Änderungen, wendet sie aber nicht automatisch auf den sichtbaren Zustand an. |
| Synchronisationscursor | `SyncCursor` | Wasserzeichen für die nächste Delta-Abfrage. Ein Zeitstempel allein genügt nicht als Ereignisidentität. |
| Pending Event | `PendingEvent` | Normalisiertes, dedupliziertes Boardereignis, das erkannt, aber noch nicht auf den aktuellen Boardzustand angewendet wurde. |
| Pending-Puffer | `PendingBuffer` | Boardkontextbezogene Menge der Pending Events. Sein Zähler zählt Ereignisse, nicht Issues. |
| Pending-Projektion | `PendingProjection` | Aus Pending Events berechnete Vorschau beziehungsweise Zählerinformation. Sie ist nicht der aktuelle Boardzustand und wird bei einem Kontextwechsel neu gebildet. |
| Aktualisieren | `Refresh` | Benutzeraktion, die zuerst einen Delta-Poll ausführt und danach vorhandene und neue Pending Events gemeinsam atomar anwendet. Sie bricht ein laufendes Replay vorher ab. |
| Daily-Snapshot | `DailySnapshot` | Atomar gespeicherter lokaler Zustand des Boards beim erfolgreichen Daily-Abschluss einschließlich der zur exakten Offline-Reihenfolge benötigten Rank-/Ordinaldaten. Er enthält niemals Credentials. |
| Snapshot löschen | `DeleteDailySnapshot` | Boardbezogene, bestätigte und idempotente Aktion. Sie löscht Snapshot und abgeleitete Historiencaches, aber keine Credentials, Einstellungen oder Daten anderer Boards. |
| Policy-Snapshot | `ReplayPolicySnapshot` | Nur für einen Replay-Lauf eingefrorene Einstellungskopie. Sie ist kein Daily-Snapshot und keine dauerhaft gespeicherte Boardaufnahme. |
| Visuelle Referenz | `GoldenMaster` / `VisualBaseline` | Freigegebenes Testbild einer Produktionskomponente. Es ist kein fachlicher Snapshot und enthält keine echten Jira-Daten. |

## Issue-Details und Development Information

| Begriff | Codebegriff | Verbindliche Bedeutung und Invarianten |
|---|---|---|
| Issue-Modal | `IssueModal` | Read-only Detailansicht über dem Board. Sie zeigt Parent-Kontext nur für das ausgewählte Standard-Issue und erhält Boardposition sowie kontrollierten Replayzustand. |
| Development Information | `DevelopmentInformation` | Von Jira bereitgestellte Informationen über Commits, Branches oder Pull Requests. Sie stammen im MVP niemals aus einem direkten Git-Provider-Client. |
| Development-Capability | `DevelopmentInfoCapability` | Explizit `Unavailable` oder `JiraProvided` mit unterstützten Arten. `Unavailable` ist ein normaler, vollständig funktionsfähiger Zustand. |

## Architektur- und Testsprache

| Begriff | Codebegriff | Verbindliche Bedeutung und Invarianten |
|---|---|---|
| Anti-Corruption Layer | `Jira Mapping` | Explizite DTOs und Mappingfunktionen zwischen veränderlichen Jira-Formaten und Domainbegriffen. Jira-Feldnamen dürfen nicht unkontrolliert in die Domain durchsickern. |
| Port | `Port` | Kleine, vom fachlichen Bedarf bestimmte Schnittstelle für Zeit, Jira, Persistenz, Credentials oder Scheduling. Kein allgemeiner Service-Locator. |
| UiCatalog | `JiraBoard.UiCatalog` | Nativer Avalonia-Komponentenkatalog und Storybook des Projekts. Er verwendet dieselben Produktionsviews wie App und VisualTests. |
| Katalogszenario | `CatalogScenario` | Benannte, deterministische Fixture eines Komponenten- oder Boardzustands ohne Netzwerk und echte Credentials. |
| Vertragsfixture | `ContractFixture` | Anonymisierte, versionierte Jira-Antwort, die Mapping- und Ordnungsverhalten offline belegt. Herkunft und relevante API-Annahmen sind dokumentiert. |

## Besonders wichtige Abgrenzungen

| Nicht verwechseln | Unterschied |
|---|---|
| `Story` und `StandardIssue` | Story ist ein Typ; Standard Issue ist die fachliche Hierarchieklasse aller Swimlane-Roots. |
| `ParentIssue` und `SwimlaneRoot` | Parent Issue liegt oberhalb Level 0 und bleibt vom Board verborgen; Swimlane Root ist das sichtbare Standard Issue auf Level 0. |
| `Transition` und `StatusChanged` | Transition ist eine ausführbare Jira-Aktion; StatusChanged ist das anschließend beobachtete Ereignis. |
| `JiraRank` und `BoardOrdinal` | JiraRank stammt aus Jira-Ranking; BoardOrdinal konserviert die konkrete API-Position einer geladenen Revision. |
| `DailySnapshot` und `GoldenMaster` | DailySnapshot ist lokale fachliche Boardhistorie; GoldenMaster ist ein visuelles Testbild. |
| `PendingEvent` und `BoardEvent` | Jedes Pending Event ist ein Board Event mit dem zusätzlichen Zustand „noch nicht angewendet“. |
| `Polling` und `Refresh` | Polling erkennt und puffert; Refresh prüft erneut und wendet atomar an. |
| `ReviewTrack` und Jira-Spalte | ReviewTrack ist nur eine lokale kompakte Projektion zweier Jira-Spalten. |
| `ReplayScope` und `SprintScope` | ReplayScope bestimmt, welches Issue animiert wird; SprintScope bestimmt, welche Issues zum Boardkontext gehören. |
| `Ticket` und `Issue` | Ticket ist verständliche UI-/Gesprächssprache; Issue ist der verbindliche Domain-Oberbegriff. |

## Entscheidungs- und Klärungsprotokoll

Bereits ausdrücklich entschieden:

- `2026-07-20`: Bei fehlendem oder gleichem Jira-Rank gilt `BoardOrdinal` vor dem Issue-Key. Der Issue-Key ist nur der letzte technische Notanker.
- `2026-07-27` (`DOM-006`): Die deterministische Ereignisreihenfolge (`orderBoardEvents`) vergleicht zuerst den UTC-Zeitpunkt als Instant (offset-unabhängig), danach das `BoardOrdinal` des Issues (Issues ohne Ordinal danach), dann die feste Ereignisart-Ordnung `StatusChanged`, `AssigneeChanged`, `LabelChanged`, `CommentAdded`, `CommitLinked`, dann die Quellreihenfolge (`JiraHistory` nach Item-Index, dann `JiraComment`, dann `DevelopmentInformation`) und zuletzt die `BoardEventId`. Der lesbare Issue-Key ist nicht Teil des Schlüssels, damit die Reihenfolge kultur-invariant bleibt.
- `2026-07-20`: Die neutrale Komponente heißt `SwimlaneHeader`; `StorySwimlaneHeader` ist wegen Bug, Task und Custom Standard Issues nicht mehr zulässig.
- Parent-/Epic-Issues erscheinen nur als `ParentContext` im Issue-Modal, niemals als Swimlane oder Boardkarte.
- `AllActiveSprints` ist eine stabile Teilfolge der globalen Jira-Boardreihenfolge, keine Verkettung einzelner Sprintantworten.

Noch durch technische Spikes zu belegen, ohne die Fachbedeutung zu ändern:

- welcher offiziell unterstützte Jira-Cloud-Leseweg die globale Boardreihenfolge zuverlässig liefert;
- wie das dynamische Rank-Feld pro Board erkannt wird und welche Vergleichsrichtung die reale JiraTui-/Jira-Vertragsfixture bestätigt;
- ob Development Information mit der MVP-API-Token-Anmeldung offiziell lesbar ist.

Wenn beim Lesen ein Begriff anders verstanden wird, wird zuerst dieses Dokument korrigiert und erst danach Code oder Tests erstellt. Eine Glossaränderung, die Verhalten oder Scope verändert, benötigt eine ausdrückliche Produktentscheidung und passende Änderungen an Handover, Checkliste und UI-Spezifikation.
