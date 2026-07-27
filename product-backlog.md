# Product Backlog

## Status und Verwendung

Dieses Dokument übersetzt den [technischen Handover](avalonia-fsharp-funcui-stack-handoff.md), das [DDD-Glossar](domain-glossary.md), die [Lizenz- und Avalonia-Free-Policy](license-policy.md), die [Implementation-Readiness-Checkliste](implementation-readiness-checklist.md) und die [UI-Design-Spezifikation](ui-design-specification.md) in eine priorisierte Lieferreihenfolge. Der kompakte [Active State](active-state.md) zeigt nur vorgeschlagene, laufende, zur Abnahme stehende oder blockierte Positionen und den nächsten Kandidaten; er ersetzt keinen Status und keine Anforderung dieses Backlogs.

Die fünf genannten Dokumente bleiben für Produkt, Fachsprache, Lizenzierung, Architektur und Design maßgeblich. Dieses Backlog entscheidet keine abweichenden Anforderungen. Bei einem Konflikt wird das Backlog korrigiert; die Umsetzung rät nicht. Neue Produktentscheidungen werden zuerst in den maßgeblichen Dokumenten festgehalten.

Das Backlog ist bewusst outcome-orientiert. Ein Item darf bei der Umsetzung in kleinere Jira-Issues oder Subtasks zerlegt werden, solange Akzeptanzkriterien, Abhängigkeiten und fachliche Grenzen erhalten bleiben.

## Ziel und MVP-Schnitt

Ziel ist eine native Jira-Cloud-Desktop-App für Windows, Linux und macOS, deren zentrale Boardansicht Daily Meetings unterstützt und die Reihenfolge des echten Jira-Boards exakt erhält.

Zum MVP gehören:

- eine aktive Jira-Cloud-Site;
- Team-managed Scrum;
- ein ausgewähltes Projekt und Board;
- alle aktiven Sprints oder genau ein aktiver Sprint;
- exakte Jira-Reihenfolge von Swimlanes und Subtasks;
- read-only Board und Issue-Modal;
- lokale Snapshots, Pending Events und explizites Aktualisieren;
- Jira-Transitionen mit kontrolliertem Drag-and-drop;
- Daily Replay für genau eine Swimlane oder einen Subtask;
- Reduced Motion, Tastaturbedienung und visuelle Regressionstests;
- self-contained Builds und kontinuierlich geprüfte Native-AOT-Fähigkeit.

Nicht zum MVP gehören Jira Server/Data Center, Kanban, Company-managed Projects, mehrere gleichzeitig aktive Sites, direkte Git-Provider-Clients und OAuth.

## Prioritäten und Status

| Wert | Bedeutung |
|---|---|
| `P0` | Voraussetzung oder Release-Blocker; zuerst erledigen |
| `P1` | notwendiger MVP-Umfang |
| `P2` | wichtige Härtung oder Capability, die das Kern-MVP nicht blockiert |
| `P3` | ausdrücklich spätere Ausbaustufe |

| Status | Bedeutung |
|---|---|
| `Planned` | fachlich beschrieben, aber noch von Vorgängern abhängig |
| `Ready` | ausreichend geklärt und ohne offene Abhängigkeit umsetzbar |
| `Proposed` | vom Agenten als nächstes Paket vorgeschlagen; wartet vor jeder Umsetzung auf menschliche Bestätigung |
| `In Progress` | gegenwärtig in Bearbeitung; Verantwortlicher und Schreibbereich stehen im Active State |
| `In Review` | Umsetzung, Tests und Agentenreview sind abgeschlossen; wartet auf ausdrückliche menschliche Abnahme |
| `Blocked` | begonnen oder startbereit, aber durch eine konkret benannte Entscheidung oder Abhängigkeit angehalten |
| `Done` | mit überprüfbarem Nachweis und ausdrücklicher menschlicher Abnahme abgeschlossen |
| `Spike` | zeitlich begrenzte Untersuchung; endet mit ADR, nicht mit übernommenem Spike-Code |
| `Future` | außerhalb des MVP-Schnitts |

Schätzungen werden erst ergänzt, wenn das umsetzende Team zwei bis drei vergleichbare Items abgeschlossen hat. Vorher erzeugen Story Points nur Scheingenauigkeit.

## Verbindliche Lieferreihenfolge

| Welle | Ergebnis | Zugehörige Epics | Gate |
|---:|---|---|---|
| 0 | Repository, TDD/DDD, CI und technische Risiken beherrscht | E00–E03 | Readiness G1–G7 |
| 1 | offline abgenommener Board-Vertical-Slice im UiCatalog | E04 | Readiness G8 und Definition of Ready |
| 2 | echte Jira-Site und read-only Board | E05 | Live-Jira-Vertragsfixtures und Ordnungsnachweis |
| 3 | robuste lokale Daten, Aktualisierung und Issue-Details | E06–E07 | Offline-/Fehler-/Security-Nachweis |
| 4 | kontrollierte Jira-Schreiboperationen | E08 | Transition-/Rollback-Nachweis |
| 5 | vollständiges Daily Replay | E09 | deterministische Replay- und Motion-Tests |
| 6 | Qualität, Cross-Plattform und Release | E10–E11 | MVP-Release-Gate |

Breite Featureimplementierung außerhalb des offline Vertical Slice beginnt erst, wenn die Definition of Ready der Readiness-Checkliste erfüllt oder eine Ausnahme ausdrücklich dokumentiert ist.

## Definition of Ready für ein Backlog-Item

Ein Item ist `Ready`, wenn:

- fachlicher Nutzen oder technisches Risiko verständlich benannt ist;
- verwendete Begriffe im DDD-Glossar definiert sind;
- Akzeptanzkriterien beobachtbares Verhalten beschreiben;
- Abhängigkeiten und Nicht-Ziele feststehen;
- benötigte Jira-Fixtures anonymisiert vorliegen oder als Vorgänger eingeplant sind;
- bei UI-Arbeit ein benanntes UiCatalog-Szenario vorgesehen ist;
- Security-, Accessibility-, AOT- und Persistenzauswirkungen geprüft wurden;
- keine offene Produktentscheidung durch eine technische Annahme ersetzt wird.

## Globale Definition of Done

Zusätzlich zu den item-spezifischen Kriterien gilt:

- Verhalten wurde im Red-Green-Refactor-Zyklus entwickelt; ein Bugfix besitzt zuerst einen Reproduktionstest.
- Domaincode bleibt frei von Avalonia, Jira-DTOs, HTTP, SQLite und Credential-Store-Implementierungen.
- `FluentAssertions` ist weder direkt noch transitiv noch im Quelltext vorhanden; der CI-Hard-Fail bleibt grün.
- Ausschließlich Avalonia Free/MIT und freigegebene, kommerziell nutzbare OSS-Abhängigkeiten sind vorhanden; Community, Plus, Pro, Enterprise, Accelerate, Premium-Pakete, unbekannte Lizenzen und Lizenzschlüsselmarker fehlen.
- Lizenzinventar und `THIRD-PARTY-NOTICES.txt` decken direkte/transitive Pakete, Tools, Fonts, Icons und Assets reproduzierbar ab.
- Relevante Unit-, Architektur-, Elmish-, Layout- und Integrationstests sind grün.
- UI-Komponenten wurden zuerst als Produktionsview im UiCatalog umgesetzt und abgenommen.
- Tastatur, Automation und Reduced Motion sind bei UI-Änderungen berücksichtigt.
- Jira-Reihenfolge bleibt bei Pagination, Filterung, Sprint-Merge und Snapshot-Restore erhalten.
- `dotnet restore`, `dotnet build -c Release` und `dotnet test -c Release --no-build` sind für den Change grün.
- Relevante Self-contained- und Native-AOT-Smoke-Checks sind bei Dependency-, Mapping-, Persistenz- oder Wiring-Änderungen grün.
- Keine Golden Masters wurden automatisch aktualisiert.
- Dokumentation, Glossar und ADRs wurden angepasst, wenn sich ein öffentlicher Vertrag geändert hat.
- Der Agent hat Ergebnis, Prüfungen, Einschränkungen, konkrete Abnahmepunkte und Retrospektive vorgestellt; `Done` wurde erst nach ausdrücklicher menschlicher Abnahme gesetzt.

## E00 – Produkt- und Ausführungsgrundlage

**Outcome:** Agenten und Menschen arbeiten mit derselben Fachsprache, demselben Scope und einer sichtbaren Priorisierung.

| ID | Prio | Status | Item | Akzeptanzkriterien | Abhängigkeit |
|---|---|---|---|---|---|
| GOV-001 | P0 | Done | Technisches Handover konsolidieren | Stack, Scope, Architektur, Testing und MVP-Regeln sind verbindlich dokumentiert. | – |
| GOV-002 | P0 | Done | DDD-Glossar erstellen | Begriffe, Invarianten und Abgrenzungen einschließlich `BoardOrdinal`, `JiraRank` und `SwimlaneHeader` sind festgehalten. | GOV-001 |
| GOV-003 | P0 | Done | UI-Design-Spezifikation und Readiness-Gate erstellen | UiCatalog-first, visuelle Verträge und Start-Gates sind überprüfbar. | GOV-001 |
| GOV-004 | P0 | Done | Priorisiertes Product Backlog erstellen | MVP-Schnitt, Reihenfolge, Abhängigkeiten und Akzeptanzkriterien sind dokumentiert. | GOV-001–003 |
| GOV-005 | P0 | Done | ADR- und Feature-Notiz-Format anlegen | [ADR-Vorlage](docs/templates/adr-template.md) und [Feature-Notiz-Vorlage](docs/templates/feature-note-template.md) halten Fragestellung, Entscheidung, Alternativen, Konsequenzen und Nachweise kompakt fest. Menschlich abgenommen am 26. Juli 2026. | GOV-004 |
| GOV-006 | P0 | Done | Operativen Active State etablieren | Vorgeschlagene, laufende, zur Abnahme stehende und blockierte Items, nächster Kandidat, Schreibbereiche und Prüfstand sind kompakt sichtbar; Backlog und Readiness bleiben maßgeblich. | GOV-004 |
| GOV-007 | P0 | Done | Agent-Mensch-Arbeitsflow etablieren | Nächstes Paket benötigt Startbestätigung; der Agent organisiert Umsetzung, Tests und Review; die Vorstellung enthält konkrete Abnahmehinweise und Retrospektive; nur der Mensch erlaubt `Done`, sonst läuft das Paket erneut durch den Zyklus. Menschlich abgenommen am 26. Juli 2026. | GOV-006 |
| GOV-008 | P0 | Done | Entwicklungsübergabepaket mit Designreferenzen erstellen | Alle verbindlichen Markdown-Dateien und genau acht ausgewählte, fachlich benannte Konzept-PNGs lagen in einem geprüften Windows-ZIP; eine README grenzt die Bilder von Spezifikation, Golden Masters und Produkt-Assets ab. Menschlich abgenommen am 26. Juli 2026; das temporäre ZIP wurde anschließend bewusst gelöscht. | GOV-006–007 |

## E01 – Repository, Toolchain und Qualitätsgrenzen

**Outcome:** Ein sauberer Checkout baut reproduzierbar und verhindert verbotene oder AOT-schädliche Grundlagen frühzeitig.

| ID | Prio | Status | Item | Akzeptanzkriterien | Abhängigkeit |
|---|---|---|---|---|---|
| FND-001 | P0 | Done | Solution-Skelett erzeugen | [JiraBoard.slnx](JiraBoard.slnx) enthält App, UI, UiCatalog, Tests, AOT-SmokeTests und VisualTests mit expliziter F#-Dateireihenfolge. Menschlich abgenommen am 27. Juli 2026. | GOV-005 |
| FND-002 | P0 | Done | .NET-, Paket- und Avalonia-Free-Versionen festsetzen | `global.json`, Central Package Management, .NET 10, Avalonia `11.3.18`, die am 27. Juli 2026 ausdrücklich freigegebene DataGrid-Ausnahme `11.3.13` und FuncUI/Elmish `1.6.0` sind exakt gepinnt; nur geprüfte Avalonia-Free-/OSS-Pakete, keine Preview-Pakete. Die nativen SkiaSharp-/HarfBuzzSharp-Unterlizenzen sind am 27. Juli 2026 verwendungsspezifisch in [ADR-001](docs/adr/ADR-001-native-skiasharp-license-exception.md) freigegeben; vollständige Lizenztexte und Attributionen werden mit der Anwendung ausgeliefert. Menschlich abgenommen am 27. Juli 2026. | FND-001 |
| FND-003 | P0 | Done | TDD-Harness einrichten | Stabiles xUnit, eingebaute `Assert`-APIs und kleine F#-Testhelper funktionieren; ein Harness-Test schlägt vor der zugehörigen Testhelper-Implementierung fehl und läuft danach grün. Der erste fachliche Domain-/Update-Test folgt nach `FND-005` und weiterhin vor seinem Produktionscode. Menschlich abgenommen am 27. Juli 2026. | FND-001–002 |
| FND-004 | P0 | Done | FluentAssertions-Sperre automatisieren | Direkte, transitive, Paketdatei-, Namespace-, Alias- und Wrapper-Treffer beenden den [Repository-Scanner](eng/check-fluent-assertions.ps1) mit Hard Fail; die [kombinierte Negativkontrolle](eng/tests/check-fluent-assertions.Tests.ps1) belegt Paketgraph, Source und generische Wrapper. Die GitHub-Actions-Verdrahtung folgt in `FND-006`. Menschlich abgenommen am 27. Juli 2026. | FND-002–003 |
| FND-005 | P0 | Blocked | Architekturgrenzen testen | Tests verhindern Domainreferenzen auf UI, Jira-Transport, HTTP, SQLite und Credential-Implementierungen. Zurückgestellt: Die Grenzprüfung wird zusammen mit dem ersten Domainprojekt in `DOM-001` eingeführt, weil vor dem Domainprojekt keine Domainassembly zum Schützen existiert. | DOM-001 |
| FND-006 | P0 | Planned | GitHub-Actions-CI erstellen | Restore, Release-Build, Tests, Format-/Warnungscheck, Paket-/Lizenzprüfung und Secret-Scan laufen auf sauberem Checkout. | FND-002–004 |
| FND-007 | P2 | Planned | GitHub Actions lokal mit `act` prüfen | Unter Windows 11 mit Docker Desktop lässt sich der Linux-kompatible Kernworkflow vor Push ausführen; Unterschiede zu GitHub-hosted Runnern sind dokumentiert, GitHub Actions bleibt maßgeblich. | FND-006 |
| FND-008 | P0 | Planned | AOT-Smoke-Runner anlegen | Gewöhnliches F#-Executable registriert Checks statisch, lässt sich publishen und liefert bei Fehlern Exit-Code ungleich null. | FND-001–003 |
| FND-009 | P0 | Planned | Lizenz-, Avalonia-Free- und Dependency-Gate etablieren | Vollständiger Graph plus Fonts/Assets besitzt Lizenzinventar und Allowlist; Community/Plus/Pro/Enterprise/Accelerate, Premium-Pakete, Schlüsselmarker, unbekannte Lizenzen und nicht allowlistete Transitiva blockieren CI; `THIRD-PARTY-NOTICES.txt` und negative Kontrolltests sind reproduzierbar. | FND-002, FND-006 |

## E02 – Domainfundament und Vertragsfixtures

**Outcome:** Die Kernregeln sind unabhängig von Jira und Avalonia testbar und durch realistische Offline-Daten belegt.

| ID | Prio | Status | Item | Akzeptanzkriterien | Abhängigkeit |
|---|---|---|---|---|---|
| DOM-001 | P0 | Done | Starke Identitäten und `BoardContext` modellieren | Site, Projekt, Board, Sprint und Issue verwenden unterschiedliche Typen; Namen sind keine Identität. Zusammen mit dem ersten Domainprojekt wird die Architekturgrenze aus `FND-005` umgesetzt: ein Test/eine Regel verhindert Domainreferenzen auf UI, Jira-Transport, HTTP, SQLite und Credential-Implementierungen. Menschlich abgenommen am 27. Juli 2026. | FND-003 |
| DOM-002 | P0 | Done | Issue-Hierarchie klassifizieren | Parent-, Standard- und Subtask-Level entstehen aus Metadaten; Story, Bug, Task und Custom Standard ergeben dieselbe Swimlane-Regel. Menschlich abgenommen am 27. Juli 2026. | DOM-001 |
| DOM-003 | P0 | Done | Boardreihenfolge modellieren | `JiraRank`, `BoardOrdinal`, `ResolvedBoardOrder` und stabile Teilfolgen sind pure, getestete Verträge. Menschlich abgenommen am 27. Juli 2026. | DOM-001 |
| DOM-004 | P0 | Done | Multi-Sprint-Scope projizieren | Alle aktiven Sprints deduplizieren nach `IssueId` und erhalten die globale Boardreihenfolge; einzelner Sprint filtert exakt. Menschlich abgenommen am 27. Juli 2026. | DOM-001, DOM-003 |
| DOM-005 | P0 | Done | Boardereignismodell erstellen | Status, Assignee, Labels, Kommentare und optionale Commits werden als normalisierte `BoardEvent`-Werte modelliert. Menschlich abgenommen am 27. Juli 2026. | DOM-001 |
| DOM-006 | P0 | Done | Ereignisreihenfolge deterministisch machen | UTC, Quellreihenfolge, Scope-/Boardreihenfolge, Ereignisart und Event-ID liefern kultur- und eingabeunabhängig dieselbe Sequenz; `BoardOrdinal` steht vor dem Issue-Key-Fallback. Menschlich abgenommen am 27. Juli 2026. | DOM-003, DOM-005 |
| DOM-007 | P1 | Planned | Status-Bounce-Policy modellieren | `Aus` oder 1–30 Minuten, Standard 5, inklusive Grenze und Erhalt anderer Ereignisse sind pure getestet. | DOM-005–006 |
| DOM-008 | P0 | Done | Repräsentative Jira-Fixtures aufbauen | Anonymisierte Fixtures decken Hierarchie, Multi-Sprint, Pagination, dynamischen/missing/equal Rank, Statusmapping und Fehler ab. Menschlich abgenommen am 27. Juli 2026. | FND-003 |
| DOM-009 | P0 | Planned | Fixture-Sicherheitsprüfung automatisieren | Manifest dokumentiert Herkunft und API-Annahmen; Tokens, Cookies, interne URLs und personenbezogene Inhalte werden blockiert. | DOM-008, FND-006 |

## E03 – UiCatalog, Designsystem und Risikospikes

**Outcome:** Das visuelle System und die größten technischen Unsicherheiten sind vor breiter App-Integration praktisch bewiesen.

| ID | Prio | Status | Item | Akzeptanzkriterien | Abhängigkeit |
|---|---|---|---|---|---|
| UI-001 | P0 | Planned | Design Tokens als F#-Code anlegen | Farben, Typografie, Abstände, Radien, Z-Order und Motion liegen zentral; keine lokalen Magic Numbers. | FND-001–003 |
| UI-002 | P0 | Planned | Fonts reproduzierbar bündeln | Freigegebene Iosevka-Aile-/Fixed-Builds und Lizenztext sind gepinnt; UI-Text und Issue-Keys verwenden die richtigen Fonts. | UI-001, FND-009 |
| UI-003 | P0 | Planned | UiCatalog-Shell erstellen | Native Avalonia-/FuncUI-App ohne XAML startet zuerst und steuert Viewport, Zoom, Motion, Reduced Motion und Animationsfortschritt. | FND-002, UI-001 |
| UI-004 | P0 | Planned | Boardlayout als pure Funktionen implementieren | IdentityRail, normale/eingeklappte Spalten und 1,33-/80-/20-Reviewgeometrie sind deterministisch getestet. | UI-001, DOM-002–004 |
| UI-005 | P0 | Planned | Kernkomponenten im UiCatalog bauen | `SwimlaneHeader`, `TicketCard`, `CollapsedColumnCell` und `ReviewTrack` verwenden Produktionsviews und zeigen alle Pflichtzustände. | UI-003–004 |
| UI-006 | P0 | Planned | Tastatur- und Automation-Verträge demonstrieren | Roving Focus, Pfeile, Leertaste, Enter, Escape, Tooltips und Accessible Names funktionieren in Katalogfixtures. | UI-005 |
| UI-007 | P0 | Planned | Headless-Visualtest-Harness aufbauen | Kanonisches OS, Fonts, Locale, DPI und Renderer sind fixiert; Diffs werden als Artefakte ausgegeben, Baselines nie automatisch aktualisiert. | UI-002–005, FND-006 |
| SPK-001 | P0 | Spike | Avalonia-Composition- und BoardSurface-Spike | Drei Spalten, lane-lokale Bewegung, Abbruch, Reduced Motion und 1080p-/High-Resolution-Messwerte sind dokumentiert; endet mit ADR. | UI-003–005 |
| SPK-002 | P0 | Spike | Native-AOT-/Cross-Plattform-Spike | Minimalhost, JSON-Mapping, SQLite-Migration und AOT-Smoke starten; Windows praktisch, Linux/macOS über passende CI-Hosts; endet mit ADR. | FND-008, DOM-008 |
| SPK-003 | P0 | Spike | Jira-Cloud-Boardorder- und Rank-Spike | Offizieller Leseweg, Pagination, dynamisches Rank-Feld, Sortierrichtung und Multi-Sprint-Projektion sind gegen reale anonymisierte Jira/JiraTui-Daten belegt; endet mit ADR. | DOM-003–004, DOM-008 |
| SPK-004 | P0 | Spike | Native Credential Stores prüfen | Port, Fehlerzustände und mindestens Windows-Adapter sind real getestet; Linux/macOS-Voraussetzungen sind belegt; Token-Leak-Test ist grün; endet mit ADR. | FND-009 |
| SPK-005 | P1 | Spike | Drag-and-drop-Verhalten prüfen | Start, Ghost/Overlay, Drop, Abbruch und Rollback laufen im UiCatalog ohne Jira; Pointer-Capture und Fokus sind bewertet; endet mit ADR. | UI-005–006, SPK-001 |
| SPK-006 | P2 | Spike | Development Information Capability prüfen | Nur offizielle Jira-Cloud-Endpunkte mit MVP-Token; Ergebnis ist `JiraProvided` oder normaler Zustand `Unavailable`; endet mit ADR. | DOM-008 |

## E04 – Erster offline Vertical Slice

**Outcome:** Der kleinste fachliche Boardfluss ist test-first, visuell abgenommen und vollständig ohne Netzwerk ausführbar.

| ID | Prio | Status | Item | Akzeptanzkriterien | Abhängigkeit |
|---|---|---|---|---|---|
| VS-001 | P0 | Planned | Offline-Projekt- und Sprintkontext darstellen | Fixture bietet Projektauswahl, zwei aktive Sprints, `AllActiveSprints`, Einzelsprint und Wiederherstellung letzter Auswahl. | DOM-001, DOM-004, UI-003 |
| VS-002 | P0 | Planned | Statisches Board aus Domainprojektion rendern | Ein Parent bleibt modal, ein Standard-Issue bildet eine Swimlane, zwei Subtasks liegen in ihren Statusspalten. | DOM-002–004, UI-004–005 |
| VS-003 | P0 | Planned | Jira-Reihenfolge im Slice beweisen | Multi-Sprint, Pagination, gleicher/fehlender Rank und Filter verändern die erwartete relative Reihenfolge nicht. | DOM-003–004, DOM-008, VS-002 |
| VS-004 | P0 | Planned | Deterministisches Offline-Replay integrieren | Eine Swimlane und ein Subtask besitzen je einen Loop-Button; drei Statusereignisse und ein gefilterter Bounce laufen über steuerbare Keyframes. | DOM-005–007, SPK-001, VS-002 |
| VS-005 | P0 | Planned | Reduced Motion und Abbruch integrieren | Stop, Hover-/Fokusverlust und Reduced Motion kehren zum aktuellen Zustand zurück; verspätete Callbacks bleiben wirkungslos. | VS-004, UI-006 |
| VS-006 | P0 | Planned | Slice visuell und fachlich abnehmen | Pure Domain-, Elmish-, Layout-, Headless- und Screenshot-Nachweise sind grün; dieselben Produktionsviews laufen im UiCatalog. | VS-001–005, UI-007 |
| VS-007 | P0 | Planned | Definition-of-Ready-Gate abschließen | G1–G8 und alle Spike-ADRs sind nachgewiesen oder ausdrücklich ausgenommen; erst danach beginnt Welle 2. | FND-001–009, DOM-001–009, UI-001–007, SPK-001–005, VS-006 |

## E05 – Jira Cloud Read-only und Navigation

**Outcome:** Die App verbindet sich sicher mit einer Jira-Cloud-Site und zeigt das ausgewählte reale Scrum-Board korrekt an.

| ID | Prio | Status | Item | Akzeptanzkriterien | Abhängigkeit |
|---|---|---|---|---|---|
| JIR-001 | P1 | Planned | Site-Setup-Modal implementieren | Site-URL, E-Mail und maskierter API-Token; offizieller Token-Link öffnet extern; UiCatalog-, Validierungs- und Fehlerzustände vorhanden. | VS-007, SPK-004 |
| JIR-002 | P1 | Planned | Token ausschließlich nativ speichern | Token verlässt nach Übergabe das UI-Modell, erscheint nie in Dateien, SQLite, Logs, Exceptions, Fixtures oder Snapshots. | JIR-001, SPK-004 |
| JIR-003 | P1 | Planned | Projekte und Scrum-Boards ermitteln | Nur zugängliche Team-managed-Scrum-Projekte; ein Board automatisch, mehrere explizit, kein Board als erklärender Zustand. | JIR-002, SPK-003 |
| JIR-004 | P1 | Planned | Aktive Sprints und Scope-Menü laden | `AllActiveSprints` plus einzelne aktive Sprints, stabile IDs, gleiche Namen unterscheidbar, future/closed ausgeschlossen. | JIR-003 |
| JIR-005 | P1 | Planned | Letzten Boardkontext wiederherstellen | Lokaler Start ohne Netzblockade, Hintergrundvalidierung, Offlinehinweis und sichtbarer Fallback bei ungültigem Kontext. | JIR-003–004 |
| JIR-006 | P1 | Planned | Boardkonfiguration und Statusmapping lesen | Spalten und alle zugehörigen Status-IDs werden explizit gemappt; unbekannte Status verschwinden nicht. | JIR-003, SPK-003 |
| JIR-007 | P1 | Planned | Issues paginiert und geordnet laden | API-Reihenfolge bleibt über Seiten als `BoardOrdinal`; Rank wird dynamisch erkannt; feste Custom-Field-ID ist ausgeschlossen. | JIR-004–006, SPK-003 |
| JIR-008 | P1 | Planned | Multi-Sprint-Board projizieren | Vereinigung dedupliziert nach `IssueId` und bleibt stabile Teilfolge der globalen Boardreihenfolge; Antwortlatenz beeinflusst nichts. | JIR-004, JIR-007 |
| JIR-009 | P1 | Planned | Hierarchie und Parent-Kontext mappen | Level 0 erzeugt Swimlanes, Subtasks werden über Parent-ID zugeordnet, fehlende Parents gezielt geladen, Parent-Level bleibt vom Board verborgen. | JIR-007–008, DOM-002 |
| JIR-010 | P1 | Planned | Jira-Fehler robust darstellen | 401 stoppt Retries und bietet Tokenwechsel; 403/404/429, Timeout, Offline und partielle Antworten besitzen verständliche Zustände. | JIR-001–009 |

## E06 – Persistenz, Polling und Aktualisierung

**Outcome:** Der sichtbare Zustand bleibt stabil, neue Ereignisse werden kontrolliert angewendet und Offlinebetrieb ist nachvollziehbar.

| ID | Prio | Status | Item | Akzeptanzkriterien | Abhängigkeit |
|---|---|---|---|---|---|
| SYN-001 | P1 | Planned | Lokales Schema und Migrationen implementieren | Boardkontext, nicht geheime Einstellungen, Daily-Snapshot, Rank/Ordinal und Cursor sind getrennt; Token-Spalten existieren nicht. | SPK-002, JIR-005–009 |
| SYN-002 | P1 | Planned | Snapshot sofort wiederherstellen | Letzter gültiger Zustand erscheint vor Netzantwort, erhält Jira-Reihenfolge und wird anschließend atomar validiert/ersetzt. | SYN-001 |
| SYN-003 | P1 | Planned | Delta-Polling implementieren | Vordergrund etwa 30 Sekunden, Hintergrund etwa 10 Minuten Best Effort, Reaktivierung und Backoff laufen über Fake-Uhr/Scheduler. | SYN-001, DOM-005 |
| SYN-004 | P1 | Planned | Pending Events deduplizieren und zählen | Puffer zählt Events, nicht Issues; identische History-/Comment-/Development-IDs erhöhen den Zähler nicht. | SYN-003, DOM-005–006 |
| SYN-005 | P1 | Planned | Explizites Aktualisieren implementieren | Sofortiger Delta-Poll plus atomare Anwendung; bei Fehler bleiben Zustand, Cursor und Pending Events erhalten. | SYN-004 |
| SYN-006 | P1 | Planned | Refresh gegen Replay und alte Kontexte absichern | Refresh stoppt Replay vor Anwendung; alte Generationen und alte Boardkontextantworten können nichts überschreiben. | SYN-005, VS-004–005 |
| SYN-007 | P1 | Planned | Lokalen Snapshot sicher löschen | Bestätigung nennt Site/Board/Folgen; nur aktueller Board-Snapshot und abgeleitete Caches werden transaktional/idempotent gelöscht; Reload folgt. | SYN-001–006 |
| SYN-008 | P0 | Planned | Logging und Redaction absichern | Token, Jira-Inhalte und personenbezogene Daten erscheinen nicht unkontrolliert in Logs, Exceptions, Crashdaten oder CI-Artefakten. | JIR-002, SYN-001–007 |

## E07 – Issue-Details

**Outcome:** Ein Issue lässt sich read-only im Modal verstehen, ohne Boardkontext oder Fokus zu verlieren.

| ID | Prio | Status | Item | Akzeptanzkriterien | Abhängigkeit |
|---|---|---|---|---|---|
| MOD-001 | P1 | Planned | IssueModal im UiCatalog fertigstellen | Mit/ohne Parent, lange Beschreibung, Kommentare, Lade-, Fehler- und leere Zustände sind visuell abgenommen. | UI-003, UI-005–007 |
| MOD-002 | P1 | Planned | Issue-Details aus Jira laden | Key, Titel, Typ, Ebene, Beschreibung, Status, Assignee, Labels und Kommentare werden explizit gemappt. | JIR-007, MOD-001 |
| MOD-003 | P1 | Planned | Parent-Kontext ausschließlich modal zeigen | Epic/Parent erscheint mit Key und Titel im Standard-Issue-Modal, niemals als Boardkarte, Swimlane oder Replay-Scope. | JIR-009, MOD-002 |
| MOD-004 | P1 | Planned | Fokus und Replayzustand kontrollieren | Modal ist per Pointer/Tastatur erreichbar, trappt Fokus korrekt und gibt ihn an dasselbe Issue oder definierten Fallback zurück. | MOD-001–003, UI-006 |
| MOD-005 | P2 | Planned | Development Information capability-gesteuert anzeigen | Bei `JiraProvided` unterstützte Daten, bei `Unavailable` kein Fehler und keine direkte Providerintegration. | MOD-002, SPK-006 |

## E08 – Jira-Transitionen und Drag-and-drop

**Outcome:** Benutzer können erlaubte Workflowtransitionen sicher auslösen; Fehler stellen den bestätigten Zustand wieder her.

| ID | Prio | Status | Item | Akzeptanzkriterien | Abhängigkeit |
|---|---|---|---|---|---|
| TRN-001 | P1 | Planned | Erlaubte Transitionen als Port laden | Transitionen sind getrennt von Status und Ranking; Cancellation und Fehler sind explizit. | JIR-006–010, VS-007 |
| TRN-002 | P1 | Planned | DragState test-first implementieren | Idle, Dragging, Committing und Reverting sind pure Elmish-Zustände; Abbruch verliert keinen bestätigten Boardzustand. | SPK-005, TRN-001 |
| TRN-003 | P1 | Planned | Drag-and-drop-UI aus Katalog übernehmen | Pointer, Tastatur/Fallback, Ghost/Overlay, gültige Ziele und Ablehnung verändern keine lokale Reihenfolge unbeabsichtigt. | SPK-005, TRN-002 |
| TRN-004 | P1 | Planned | Mehrdeutige und feldpflichtige Transitionen behandeln | Eine einfache direkte Transition läuft sofort; mehrere zeigen Auswahl; Pflichtfelder zeigen Modal; keine automatische Zwischenstatuskette. | TRN-001–003 |
| TRN-005 | P1 | Planned | Optimistisches Commit und Rollback implementieren | Erst nach eindeutiger Transition; Erfolg bestätigt, Serverfehler rollt sichtbar und deterministisch zurück; POST wird nicht blind wiederholt. | TRN-004 |
| TRN-006 | P1 | Planned | Ranking als getrennte Operation implementieren | Reorder wird niemals mit Statuswechsel gleichgesetzt; bestätigte Jira-Reihenfolge ersetzt atomar lokale Erwartung. | TRN-005, SPK-003 |
| TRN-007 | P1 | Planned | Transition-Race- und Regressionstests ergänzen | Refresh, Kontextwechsel, Offline, Rate Limit und verspätete Antworten können keinen neueren Zustand überschreiben. | TRN-005–006, SYN-006 |

## E09 – Daily Replay

**Outcome:** Im Daily lässt sich die Entwicklung seit dem vorherigen Daily für genau eine Swimlane oder einen Subtask nachvollziehen.

| ID | Prio | Status | Item | Akzeptanzkriterien | Abhängigkeit |
|---|---|---|---|---|---|
| REP-001 | P1 | Planned | Daily-Uhrzeit und Bezugspunkt modellieren | Boardbezogene Uhrzeit, Montag–Freitag, manuelle Feiertagskorrektur und unveränderter Bezugspunkt ohne Abschluss sind getestet. | DOM-001, SYN-001 |
| REP-002 | P1 | Planned | `Daily abschließen` implementieren | Explizite idempotente Aktion speichert Snapshot und setzt Bezugspunkt atomar; Fehler verändert bisherigen Bezugspunkt nicht. | REP-001, SYN-001–002 |
| REP-003 | P1 | Planned | Historischen Startzustand rekonstruieren | Snapshot plus Changelog ist Normalfall; fehlender/beschädigter Snapshot nutzt bestmöglichen Fallback und kennzeichnet Unsicherheit. | REP-002, JIR-007–009 |
| REP-004 | P1 | Planned | Changelogs und Quellen normalisieren | Status, Assignee, Labels und Kommentare werden dedupliziert; Commits nur bei bestätigter Capability. | DOM-005–006, REP-003, SPK-006 |
| REP-005 | P1 | Planned | Status-Bounces an Zeitraumgrenzen filtern | Look-behind/-ahead, inklusive Grenze, `Aus`, 1/30 Minuten und Erhalt anderer Events sind getestet; Historie bleibt unverändert. | DOM-007, REP-004 |
| REP-006 | P1 | Planned | Swimlane- und Subtask-Scope implementieren | Hover/Fokus zeigt genau einen Loop-Button; höchstens ein Scope spielt; andere Lanes bleiben statisch. | VS-004–005, REP-003–005 |
| REP-007 | P1 | Planned | Statusbewegungen animieren | Mehrere Statuswechsel laufen sequenziell in derselben Swimlane über Composition/FLIP; Stop/Ende kehrt zum aktuellen Zustand zurück. | SPK-001, REP-006 |
| REP-008 | P1 | Planned | Andere Ereignisse visualisieren | Assignee-, Label-, Kommentar- und capability-gesteuerte Commit-Symbole erscheinen kurz nur im aktiven Scope. | REP-004, REP-006–007 |
| REP-009 | P1 | Planned | Motion-Presets und Reduced Motion integrieren | Ruhig/Normal/Schnell sind persistent; Reduced Motion hat Vorrang und erhält Reihenfolge, Semantik und Bedienbarkeit. | UI-001, UI-006, REP-007–008 |
| REP-010 | P1 | Planned | Replay deterministisch und race-sicher testen | Keyframes ohne Schlafzeiten; gleiche Zeitstempel, Refresh, Hoververlust, Kontextwechsel und alte Callbacks liefern reproduzierbare Ergebnisse. | REP-005–009, SYN-006 |

## E10 – Accessibility, Visual Quality, Performance und Security

**Outcome:** Das MVP ist auf den Zielauflösungen verständlich, bedienbar, reproduzierbar und schützt lokale Jira-Daten.

| ID | Prio | Status | Item | Akzeptanzkriterien | Abhängigkeit |
|---|---|---|---|---|---|
| QLT-001 | P1 | Planned | Visuelle Referenzmatrix abnehmen | 1080p, 1440p, Ultrawide, 4K und Zoomextreme besitzen ausgewählte Golden Masters für Pflichtszenarien. | UI-007, MOD-001, REP-009 |
| QLT-002 | P1 | Planned | Vollständige Tastatur- und Automation-Prüfung | Boardeinstieg, Roving Focus, Modal, Menüs, Refresh, Replay und zusammengeklappte Zellen sind ohne Maus bedienbar. | UI-006, MOD-004, SYN-005, REP-006 |
| QLT-003 | P1 | Planned | Screenreader- und Reduced-Motion-Semantik prüfen | Namen, Rollen, Live-Regionen und nicht rein farbliche Signale sind verständlich; Reduced Motion verliert keine Information. | QLT-002, REP-009 |
| QLT-004 | P1 | Planned | Performancebudgets messen | Repräsentatives 1080p-Board und hohe Auflösung dokumentieren Startzeit, Speicher, Frame-Time und Visual-Tree-Größe; Regressionen besitzen Schwellenwerte. | SPK-001, JIR-008–009, REP-007–009 |
| QLT-005 | P0 | Planned | Threat Model und Datenklassifikation abschließen | Token, Jira-Inhalte, Snapshots, Avatare, Kommentare, Logs und externe Links besitzen Risiken, Schutzmaßnahmen und Aufbewahrungsregeln. | JIR-002, SYN-008 |
| QLT-006 | P1 | Planned | Offline-, Fehler- und Recovery-Flows abnehmen | Loading, Empty, Offline, 401/403/404/429, Timeout, beschädigter Snapshot und partielle Historie sind sichtbar und bedienbar. | JIR-010, SYN-002–007, REP-003 |

## E11 – Cross-Plattform-Publishing und MVP-Release

**Outcome:** Reproduzierbare Artefakte starten auf Windows, Linux und macOS; AOT- und Packaging-Risiken sind sichtbar entschieden.

| ID | Prio | Status | Item | Akzeptanzkriterien | Abhängigkeit |
|---|---|---|---|---|---|
| REL-001 | P1 | Planned | Self-contained Builds pro Ziel-OS erzeugen | Windows, Linux und macOS werden auf passenden Hosts gebaut und gestartet; native Skia-Abhängigkeiten sind enthalten. | FND-006, QLT-001–006 |
| REL-002 | P1 | Planned | Native-AOT-Publishes pro Ziel-OS prüfen | App und AOT-SmokeTests publishen/starten ohne pauschal unterdrückte Warnungen; Warnungen sind behoben oder entschieden. | SPK-002, REL-001 |
| REL-003 | P1 | Planned | GitHub-Release-Workflow erstellen | Versionierte, nachvollziehbare Artefakte entstehen aus einem sauberen Commit; Checks und Artefaktnamen sind reproduzierbar. | FND-006, REL-001–002 |
| REL-004 | P2 | Planned | Packaging, Signierung und Notarisierung entscheiden | Windows-Paket, Linux-Formate und macOS-App/Signierung sind als ADR mit Voraussetzungen und Releaseweg festgelegt. | REL-001–003 |
| REL-005 | P1 | Planned | MVP-Release-Checkliste ausführen | Scope, Tests, Visuals, Accessibility, Security, Offline, Self-contained und AOT sind nachgewiesen; bekannte Einschränkungen dokumentiert. | E05–E10, REL-001–003 |

## Nach dem MVP

Diese Items bleiben sichtbar, werden aber nicht in den MVP hineingezogen:

| ID | Prio | Status | Item | Eintrittsbedingung |
|---|---|---|---|---|
| FUT-001 | P3 | Future | Kanban-Boards | eigene Produkt-, Workflow- und UI-Entscheidung |
| FUT-002 | P3 | Future | Company-managed Projects | eigene Jira-Fixtures und Hierarchie-/Boardanalyse |
| FUT-003 | P3 | Future | mehrere gespeicherte Jira-Site-Profile | Credential- und Navigationserweiterung; weiterhin nur eine aktive Site |
| FUT-004 | P3 | Future | OAuth/Auth-Broker | Verteilungs-, Security- und Hostingentscheidung |
| FUT-005 | P3 | Future | direkte Git-Provider-Integration | eigene Architektur-, Datenschutz-, Lizenz- und Scope-Entscheidung |
| FUT-006 | P3 | Future | Bearbeitung und Kommentare im Issue-Modal | Schreib-, Berechtigungs- und Konfliktkonzept |

## Empfohlener erster Arbeitsauftrag

Der erste Umsetzungsauftrag umfasst ausschließlich:

1. `GOV-005`;
2. `FND-001` bis `FND-004`;
3. danach `DOM-001`, das zugleich die zurückgestellte Architekturgrenze aus `FND-005` umsetzt und mit einem ersten fehlschlagenden Domain-Test aus `DOM-001` oder `DOM-002` beginnt.

Er beginnt noch nicht mit Live-Jira, Credential-Eingabe oder der Produktoberfläche. Der erste ausführbare UI-Host bleibt `JiraBoard.UiCatalog`.

## Pflege des Backlogs

- Der Agent wählt das nächste abhängigkeitstechnisch zulässige `Ready`-Item, setzt es auf `Proposed`, trägt es in den [Active State](active-state.md) ein und bittet mit Ziel, Scope, Risiken, Prüfplan und Abnahmepunkten um menschliche Bestätigung.
- Ohne Bestätigung findet keine Umsetzung statt. Eine ausdrückliche menschliche Anweisung, ein konkret benanntes und abgegrenztes Paket umzusetzen, gilt als Bestätigung für genau diesen Scope.
- Nach Bestätigung wechselt das Item von `Proposed` auf `In Progress`; Verantwortlicher, Teilschritt, nächste Aktion und exklusiver Schreibbereich werden aktualisiert.
- Nur ein Item trägt pro bearbeitetem Bereich und Verantwortlichem den Status `In Progress`; jede aktive Position besitzt einen überschneidungsfreien Schreibbereich.
- Ein angehaltenes Item wechselt auf `Blocked`; der Active State benennt die genaue Blockade und die benötigte Entscheidung oder Abhängigkeit.
- Nach Umsetzung, Tests und Review wechselt das Item auf `In Review`. Der Agent stellt Verhalten, Nachweise, verbleibende Risiken, konkrete menschliche Abnahmeprüfungen und seine kurze Retrospektive vor.
- Nur eine ausdrückliche menschliche Abnahme erlaubt `Done`. Bei Feedback wechselt das Item zurück auf `In Progress`; bei wesentlicher Scopeänderung auf `Proposed`. Ohne Antwort bleibt es `In Review`.
- Nach der Abnahme werden Nachweise verlinkt, betroffene Readiness-Checkboxen aktualisiert, das Item auf `Done` gesetzt und aus den aktiven Positionen entfernt. Erst danach wird das nächste Paket vorgeschlagen.
- Ein Agent übernimmt einen klar begrenzten Schreibbereich; parallele Agenten bearbeiten nicht dieselben Dateien.
- Abgeschlossene Items erhalten Links auf Tests, ADR, UiCatalog-Szenario und CI-Lauf.
- Neue Bugs werden als eigenes reproduzierendes Test-Item vor dem Fix aufgenommen.
- Ein Spike wird nicht als Produktcode weitergeführt; seine Entscheidung fließt über ADR und neue test-first Items ein.
- Scope- oder Designänderungen werden zuerst in Handover, Glossar oder UI-Spezifikation entschieden und erst danach hier nachgezogen.
