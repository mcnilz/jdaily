# Implementation-Readiness-Checkliste

## Zweck

Diese Checkliste ist das operative Start-Gate für die Umsetzung der Jira-Desktop-App. Die dauerhaften Produkt-, Architektur- und Technologieentscheidungen stehen im [technischen Handover](avalonia-fsharp-funcui-stack-handoff.md); die verbindliche gemeinsame Fachsprache steht im [DDD-Glossar](domain-glossary.md), die zulässigen Abhängigkeiten in der [Lizenz- und Avalonia-Free-Policy](license-policy.md). Das [Product Backlog](product-backlog.md) ordnet die hier verlangten Arbeiten, ersetzt aber kein Gate. Dieses Dokument enthält ausschließlich vorbereitende Arbeiten, überprüfbare Akzeptanzkriterien und Nachweise.

Die Checkliste darf während der Umsetzung abgehakt und um Links auf ADRs, Testausgaben oder Screenshots ergänzt werden. Fachliche Entscheidungen werden nicht hier neu definiert, sondern im Handover geändert. Der kompakte [Active State](active-state.md) verweist auf die gerade bearbeiteten Gate-Punkte, ist aber kein Ersatz für deren Checkboxen und Nachweise.

## Verwendung

- `[ ]` bedeutet offen, `[x]` bedeutet mit Nachweis abgeschlossen.
- Ein abgehakter Punkt erhält im zugehörigen Nachweisfeld oder Backlog-Item einen überprüfbaren Test-, ADR-, Screenshot- oder CI-Verweis.
- Die Gate-Übersicht wird im selben Arbeitsschritt auf `[x]` gesetzt, sobald sämtliche Blocker des Gates nachweislich abgeschlossen oder ausdrücklich als `N/A` akzeptiert sind.
- Ein Gate ist erst erfüllt, wenn alle als Blocker markierten Punkte erledigt sind.
- Explorative Spikes sind zeitlich begrenzt. Ihr Code wird anschließend verworfen oder test-first neu aufgebaut.
- Der Agent beginnt keine breite Featureimplementierung, solange das abschließende Definition-of-Ready-Gate nicht erfüllt ist.
- Wenn ein Punkt bewusst entfällt, wird er nicht einfach abgehakt, sondern mit Begründung als `N/A` dokumentiert.

## Absolute Projektregel: kein FluentAssertions

> **`FluentAssertions` darf im Repository in keiner Form vorkommen. Diese Regel besitzt keine Ausnahme.**

Verboten sind direkte und transitive Pakete, Central-Package-Einträge, ältere festgepinnte Versionen, `open FluentAssertions`, vollqualifizierte Aufrufe, Aliasse und kopierte Kompatibilitätswrapper. Bringt eine andere Bibliothek FluentAssertions transitiv ein, wird diese Bibliothek ersetzt oder entfernt. CI muss jeden Paket-, Abhängigkeits- oder Quelltexttreffer als Hard Fail behandeln. Zulässig sind xUnit-`Assert`, F#-Pattern-Matching und kleine projektspezifische F#-Helper ohne FluentAssertions-kompatible Fassade.

## Gate-Übersicht

| Gate | Ergebnis | Status |
|---|---|---|
| G1 – Repository und Toolchain | reproduzierbares Solution-Skelett und Build | `[ ]` |
| G2 – TDD- und DDD-Fundament | getestete Domain-Grenzen und Arbeitszyklus | `[ ]` |
| G3 – UiCatalog-first | nativer Storybook-Host vor der Produktoberfläche | `[ ]` |
| G4 – Jira-Fixtures | repräsentative, anonymisierte Offline-Testdaten | `[ ]` |
| G5 – Risikospikes | zentrale technische Unsicherheiten praktisch geklärt | `[ ]` |
| G6 – Tests und visuelle Referenz | deterministische Test- und Screenshotumgebung | `[ ]` |
| G7 – CI, Security und Datenschutz | automatisierte Qualitäts- und Sicherheitsgrenzen | `[ ]` |
| G8 – erster Vertical Slice | kleinster End-to-End-Umfang eindeutig definiert | `[ ]` |

## G1 – Repository und Toolchain

### Blocker

- [x] Repository und Solution mit den im Handover festgelegten Projekten anlegen; Nachweis: [JiraBoard.slnx](JiraBoard.slnx), `FND-001`, menschlich abgenommen am 27. Juli 2026.
- [x] `global.json` auf einen freigegebenen .NET-10-SDK-Feature-Band festlegen; Nachweis: [`global.json`](global.json) pinnt SDK `10.0.302`, deaktiviert Roll-forward und Preview-SDKs.
- [x] Central Package Management einrichten und Avalonia `11.3.18`, die ausdrücklich freigegebene DataGrid-Ausnahme `11.3.13` sowie FuncUI/Elmish `1.6.0` exakt festsetzen; Nachweis: [`Directory.Packages.props`](Directory.Packages.props) und versionsgesperrte `packages.lock.json` je Projekt.
- [x] Preview-Pakete ausschließen; Nachweis: SDK-Preview deaktiviert, zentrale Versionen ohne Preview-Suffix und lokaler Vergleich aller 47 restaurierten Paket-/Versionspaare ohne Preview-/Alpha-/Beta-/RC-Treffer.
- [x] versionsgenaue Allowlist ausschließlich für Avalonia Free/MIT und freigegebene, kommerziell nutzbare OSS-Pakete mit Quelle, SPDX-Ausdruck und Lizenzdatei anlegen; Nachweis: [`eng/dependency-allowlist.json`](eng/dependency-allowlist.json) deckt alle 47 restaurierten Paare ab und markiert die am 27. Juli 2026 exakt verwendungsspezifisch freigegebenen Native-Assets-Ausnahmen.
- [x] Avalonia Community, Plus, Pro, Enterprise und Accelerate sowie lizenz-, account-, trial- oder subscriptiongebundene Tools und Controls ausdrücklich ausschließen; Nachweis: [Lizenzpolicy](license-policy.md), Allowlist und Marker-Scan der Projekt-, Paket-, Lock-, JSON-, Workflow- und Quelltextdateien ohne Treffer.
- [ ] CI-Hard-Fail für unbekannte direkte oder transitive `Avalonia*`-/AvaloniaUI-Pakete, Premium-Paketmarker und Avalonia-Lizenzschlüsselmarker anlegen.
- [x] nachweisen, dass weder `AvaloniaUILicenseKey`, `AVALONIA_TOOLS_LICENSE_KEY` noch `ACCELERATE_LICENSE_KEY` in Projektdateien, Props/Targets, Workflows oder Quelltext vorkommen; Nachweis: lokaler Marker-Scan am 27. Juli 2026 ohne Treffer. Prüfung von späteren CI-Secrets und benötigtem Umgebungssetup bleibt Teil von `FND-009`.
- [x] Lizenzinventar für den aktuellen direkten/transitiven Code-, Build- und Packaging-Graphen anlegen; Nachweis: [Paket-/Lizenzinventar](docs/dependencies/package-license-inventory.md) mit allen Pflichtfeldern für 47 restaurierte Paare, Native-AOT-Tools und exakten Ausnahmeentscheidungen. Fonts, Icons und Assets sind noch nicht eingeführt.
- [x] CI-Hard-Fail für direkte und transitive FluentAssertions-Abhängigkeiten im vollständigen Restore-Graphen anlegen; Nachweis: [`eng/check-fluent-assertions.ps1`](eng/check-fluent-assertions.ps1) prüft Paketdeklarationen, Lockfiles und restaurierte Assets und beendet Treffer mit Exit-Code 1. Die GitHub-Actions-Verdrahtung folgt in `FND-006`.
- [x] CI-Hard-Fail für FluentAssertions-Einträge in `Directory.Packages.props`, Projektdateien, Lock-/Assets-Dateien sowie für Namespace-, Alias- oder Wrapper-Verwendungen im Quelltext anlegen; Nachweis: derselbe Scanner deckt Projekt-, Quelltext-, Skript- und Workflowdateien sowie generische F#-/C#-`Should`-Extensions ab.
- [x] Nachweisen, dass der FluentAssertions-Check nicht per Allowlist, Suppression oder Testprojekt-Ausnahme umgangen werden kann; Nachweis: Der Scanner besitzt keine Ausnahme- oder Suppressionskonfiguration und die [kombinierte Negativkontrolle](eng/tests/check-fluent-assertions.Tests.ps1) liegt selbst unter `tests/`.
- [x] F#-Dateireihenfolge und Projektabhängigkeiten explizit festlegen; Nachweis: explizite `Compile`- und `ProjectReference`-Einträge in den sechs Projekten aus [JiraBoard.slnx](JiraBoard.slnx), `FND-001`, menschlich abgenommen am 27. Juli 2026.
- [x] `AGENTS.md` beziehungsweise die lokale Agentenanweisung auf Handover und diese Checkliste verweisen und die ausnahmslose FluentAssertions-Sperre wörtlich wiederholen lassen; Nachweis: [`AGENTS.md`](AGENTS.md) verlinkt beide Dokumente unter „Read first“ und wiederholt die ausnahmslose Sperre in den Non-negotiable rules.
- [x] `dotnet restore` und `dotnet build -c Release` reproduzierbar ausführen; Nachweis: normaler und Locked-Mode-Restore sowie vollständiger Solution-Release-Build am 27. Juli 2026 mit 0 Fehlern und 0 Warnungen. Der saubere Checkout wird nach menschlicher Abnahme und Commit erneut in CI geprüft.
- [x] minimalen Self-contained-Publish und Native-AOT-Publish ohne pauschal unterdrückte Trim-/AOT-Warnungen erzeugen; Nachweis: `JiraBoard.App` für `win-x64` in beiden Modi veröffentlicht und beide erzeugten Executables am 27. Juli 2026 mit Exit-Code 0 gestartet.
- [x] Lizenz- und Paketübersicht für alle initialen direkten und transitiven Abhängigkeiten vollständig dokumentieren; Nachweis: [Paket-/Lizenzinventar](docs/dependencies/package-license-inventory.md).
- [x] Copyright-, Lizenz- und Attributionstexte reproduzierbar als [`THIRD-PARTY-NOTICES.txt`](THIRD-PARTY-NOTICES.txt) für den aktuellen Auslieferungs- und Buildgraphen erfassen; [`eng/generate-third-party-notices.ps1`](eng/generate-third-party-notices.ps1) prüft die exakten Paketquellhashes und erzeugte zweimal denselben Artefakthash. Der vollständige Text wird neben die EXE kopiert, in `JiraBoard.App` eingebettet und durch den AOT-Smoke gegen Pflichtmarker, Inhaltsgleichheit und den normalisierten Native-Vendor-Hash geprüft.
- [ ] mit negativen Kontrolltests belegen, dass Premium-Paket, Lizenzschlüsselmarker, unbekannte Lizenz und nicht allowlistete transitive Abhängigkeit CI zuverlässig fehlschlagen lassen; Kontrolländerungen danach entfernen.

### Nachweis

- Pfad oder CI-Lauf: [JiraBoard.slnx](JiraBoard.slnx); `dotnet restore JiraBoard.slnx`, `dotnet build JiraBoard.slnx -c Release --no-restore` und `dotnet test JiraBoard.slnx -c Release --no-build` am 27. Juli 2026 lokal grün.
- SDK- und Paketversionen: SDK `10.0.302`; Avalonia `11.3.18`; ausdrücklich freigegebene DataGrid-Ausnahme `11.3.13`; FuncUI/Elmish `1.6.0`; xUnit `3.2.2`; 47 Paket-/Versionspaare in den Lockfiles.
- offene Warnungen mit Entscheidung: keine Build-, Restore-, Publish-, AOT- oder Trimwarnung im aktuellen Stand. Die nativen Unterlizenzen sind für die exakten Paketversionen und diesen Einsatz freigegeben; die vollständigen Vendor-Texte sind Auslieferungs- und App-Ressource. Die Online-Vulnerability-/Deprecation-Abfrage wurde wegen möglicher Weitergabe des Paketgraphen an benutzerkonfigurierte NuGet-Endpunkte nicht genehmigt und nicht umgangen; die GitHub-Actions-Verdrahtung und die übrigen automatisierten Lizenz-/Dependency-Gates bleiben `FND-006` und `FND-009`.

## G2 – TDD- und DDD-Fundament

### Blocker

- [x] Fachliche Begriffe und Invarianten im verbindlichen [DDD-Glossar](domain-glossary.md) festhalten und mit Handover sowie UI-Spezifikation abgleichen.
- [x] verbindlichen Agent-Mensch-Arbeitsflow mit Startbestätigung, organisierter Umsetzung/Test/Review-Phase, konkreter Abnahmevorstellung und Feedback-Loop dokumentieren und ausdrücklich menschlich abnehmen; Nachweis: `GOV-007`, abgenommen am 26. Juli 2026.
- [x] Domain-, Feature-, Jira-Adapter-, UI- und Infrastrukturgrenzen in einem kleinen Dependency-Test oder einer gleichwertigen automatischen Regel schützen. Umgesetzt mit dem ersten Domainprojekt in `DOM-001`: der statische, reflexionsfreie Grenztest [`DomainBoundaryTests`](tests/JiraBoard.Tests/DomainBoundaryTests.fs) liest das restaurierte Dependency-Lock von `JiraBoard.Domain` (direkte und transitive Pakete) und schlägt bei jeder verbotenen Referenz fehl.
- [x] Sicherstellen, dass Domaincode keine Referenz auf Avalonia, Jira-DTOs, HTTP, SQLite oder Credential-Store-Implementierungen besitzt (Nachweis über die Grenzprüfung in `DOM-001`).
- [ ] Jira-DTOs und explizite Mapping-Funktionen als Anti-Corruption-Layer vorsehen.
- [ ] `JiraRank` und `BoardOrdinal` als explizite Ordnungsinformationen modellieren; Reihenfolge darf nicht als zufällige Collection-Eigenschaft behandelt werden.
- [ ] injizierbare Ports für Uhr, Scheduler, ID-Erzeugung, Jira-Zugriff, Persistenz und Credential-Zugriff definieren, sobald sie fachlich benötigt werden.
- [ ] ersten fehlschlagenden Domain- oder Elmish-Update-Test vor dem zugehörigen Produktionscode einchecken.
- [ ] Red-Green-Refactor als verbindlichen PR-/Agentenablauf dokumentieren.
- [ ] Regel festhalten, dass bei tatsächlichen Fehlern zuerst ein reproduzierender Test entsteht.
- [ ] Explorationsspikes sichtbar kennzeichnen und ihre Übernahme in Produktcode durch ein explizites Gate verhindern.

### Erster empfohlener Domain-Test

Aus einer Fixture mit einem Parent-Level-Issue, einem Standard-Issue und zwei Subtasks entsteht genau eine Swimlane. Das Parent-Level-Issue erscheint nur als Modal-Kontext und niemals als eigene Boardkarte oder Replay-Scope.

### Nachweis

- Testname und Commit/PR: `JiraBoard.Tests.DomainIdentityTests` und `JiraBoard.Tests.DomainBoundaryTests` (`DOM-001`, menschlich abgenommen am 27. Juli 2026).
- Architekturtest: umgesetzt in `DOM-001` als statischer, reflexionsfreier [`DomainBoundaryTests`](tests/JiraBoard.Tests/DomainBoundaryTests.fs); die Negativkontrolle (temporäre Avalonia-Referenz) lässt den Test rot werden, der bereinigte Stand ist grün. Die Grenzprüfung ist damit abgenommen; das eigenständige `FND-005` bleibt zur Nachverfolgung `Blocked` und ist inhaltlich erfüllt.
- Glossar: [DDD-Glossar](domain-glossary.md)
- Arbeitsformate: [Feature-Notiz-Vorlage](docs/templates/feature-note-template.md) und [ADR-Vorlage](docs/templates/adr-template.md)

## G3 – UiCatalog-first

### Blocker

- [ ] die [UI-Design-Spezifikation](ui-design-specification.md) lesen und ihre Token-, Geometrie-, Komponenten- und Szenarioverträge als verbindliche Baseline übernehmen.
- [ ] `JiraBoard.UiCatalog` als ersten ausführbaren UI-Host ohne XAML erstellen.
- [ ] `JiraBoard.App` bis zur Katalogabnahme nur als leeren Composition-/Packaging-Host verwenden.
- [ ] `JiraBoard.UiCatalog` und `JiraBoard.App` dieselben Produktionsviews aus `JiraBoard.Ui` referenzieren lassen.
- [ ] getrennte oder vereinfachte Storybook-Doubles für Produktkomponenten technisch und organisatorisch ausschließen.
- [ ] zentrale Design-Tokens für Farbe, Typografie, Abstände, Radien, Größen, Z-Order und Motion anlegen.
- [ ] deterministische Fixtures und eine benannte Szenarioregistrierung bereitstellen.
- [ ] Katalogregler für Viewport, App-Zoom, Schriftzoom, Theme, Reduced Motion und Animationsfortschritt vorsehen.
- [ ] mindestens `TicketCard`, eine Standard-Issue-Swimlane, Subtasks und zusammengeklappte Spalten zuerst im UiCatalog darstellen.
- [ ] `ProjectSelectionModal` und `SprintMenu` mit stabilen IDs, mehreren aktiven Sprints, gleichen Namen, mehreren Boards und leerem Zustand zuerst im UiCatalog darstellen.
- [ ] festlegen, dass eine Komponente erst nach Katalogszenario, Unit-/Headless-Tests und Designabnahme in `JiraBoard.App` integriert werden darf.
- [ ] automatisierte Tests für IdentityRail, normale/eingeklappte Spalten, 1,33-/80-/20-Reviewgeometrie und Mindest-Hit-Targets anlegen.
- [ ] nachweisen, dass generierte Konzeptbilder nicht als Pixelquelle oder Golden Master verwendet werden; Baselines entstehen ausschließlich aus den implementierten Produktionskomponenten.

### Katalog-Startzustände

- [ ] normaler aktueller Boardzustand ohne Replay-Hinweise;
- [ ] Standard-Issue-Swimlane-Hover mit kontextuellem Loop-Button;
- [ ] Subtask-Hover mit lokalem Replay-Button;
- [ ] kompakter Subtask in zusammengeklappter Spalte;
- [ ] Reduced Motion;
- [ ] Loading, Empty, Offline und Error.
- [ ] automatische Wiederherstellung des letzten gültigen Projekt-/Board-/Sprintkontexts, Projektauswahl bei Erststart oder ungültigem Kontext, erneute Auswahl über Menü, mehrere Boards und neutraler Abbruchzustand;
- [ ] Sprint-Menü mit allen aktiven Sprints, Einzelauswahl, geschlossenem gespeicherten Sprint und keinem aktiven Sprint.

### Nachweis

- ausführbarer UiCatalog: _offen_
- erste Szenarionamen: _offen_
- Designabnahme: _offen_

## G4 – Repräsentative Jira-Fixtures

### Blocker

- [ ] JSON-Fixtures aus einer Jira-Cloud-Site mit Team-managed Scrum gewinnen oder realistisch aus dokumentierten Antworten erzeugen.
- [ ] alle Site-, Benutzer-, Kommentar-, Projekt- und Ticketdaten vollständig anonymisieren.
- [ ] Secrets, API-Tokens, Cookies, interne URLs und personenbezogene Freitexte automatisiert ausschließen.
- [ ] Herkunft, Jira-API-Pfad, relevante Queryparameter und Schemaannahmen jeder Fixture dokumentieren.
- [ ] Fixtures unveränderlich versionieren und ausschließlich offline in Tests und UiCatalog verwenden.

### Pflichtszenarien

- [ ] mehrere zugängliche Projekte, darunter unterstützte Team-managed-Scrum- und auszuschließende Projekttypen;
- [ ] Projekt mit genau einem, mehreren und keinem passenden Scrum-Board;
- [ ] Board mit mehreren parallelen aktiven Sprints, einem zukünftigen und einem geschlossenen Sprint;
- [ ] gleichnamige aktive Sprints mit unterschiedlichen stabilen IDs;
- [ ] überlappende Sprint-Issue-Antworten zur Deduplizierung über Issue-ID;
- [ ] absichtlich unterschiedliche Reihenfolgen in globaler Boardantwort und einzelnen Sprintantworten;
- [ ] mindestens zwei paginierte Seiten, deren Reihenfolge über die Seitengrenze erhalten bleiben muss;
- [ ] Rank-Feld unter unterschiedlichen `customfield_*`-IDs sowie fehlender und gleicher Rank;
- [ ] Parent/Epic → Story, Bug, Task oder Custom Standard Issue → Subtasks;
- [ ] Standard Issue ohne Epic und ohne Subtasks;
- [ ] fehlender oder nicht zugreifbarer Standard-Parent;
- [ ] mehrere Jira-Status-IDs innerhalb einer Boardspalte;
- [ ] `Ready for CR` und `Code Review` sowie umbenannte oder ungültige Review-Mappings;
- [ ] Status-, Assignee-, Label- und Kommentaränderungen;
- [ ] exakt inverser Status-Bounce innerhalb, genau an und außerhalb des konfigurierten Fensters;
- [ ] gleiche Zeitstempel, unterschiedliche Zeitzonen-Offsets und stabile Jira-History-Ranks;
- [ ] Pagination und Deduplizierung über mehrere Antworten;
- [ ] `401`, `403`, `404`, `429`, Timeout, Offline und partiell unvollständige Historie;
- [ ] Development Information verfügbar, nicht verfügbar und nicht berechtigt.

### Nachweis

- Fixture-Verzeichnis und Manifest: _offen_
- Anonymisierungsprüfung: _offen_

## G5 – Zeitlich begrenzte Risikospikes

Jeder Spike endet mit einer kurzen ADR: Fragestellung, Versuchsaufbau, Ergebnis, verworfene Alternativen und verbindliche Konsequenz.

### G5.1 Animation und BoardSurface

- [ ] Ticketbewegung über mindestens drei Statusspalten mit Avalonia Composition demonstrieren.
- [ ] Replay auf genau eine Swimlane und genau einen Subtask begrenzen.
- [ ] Abbruch bei Hover-Verlust und `Aktualisieren` ohne verspätete Callback-Mutation nachweisen.
- [ ] Reduced Motion ohne räumliche Bewegung bei gleicher Ereignisreihenfolge demonstrieren.
- [ ] 1920 × 1080 sowie eine hohe Auflösung mit repräsentativer Kartenanzahl messen.
- [ ] CPU-, Speicher-, Frame-Time- und Visual-Tree-Messwerte als Ausgangsbaseline dokumentieren.

### G5.2 Native AOT und Cross-Plattform

- [ ] FuncUI-/Avalonia-Minimalhost mit Native AOT starten.
- [ ] Jira-JSON-Mapping ohne unkontrollierte Reflection ausführen.
- [ ] SQLite öffnen und eine minimale Migration anwenden.
- [ ] statischen AOT-Smoke-Runner ohne Test-Discovery ausführen.
- [ ] Windows-, Linux- und macOS-Buildhost beziehungsweise CI-Matrix praktisch bestätigen.
- [ ] native Skia-/Packaging-Besonderheiten und verbleibende Warnungen dokumentieren.

### G5.3 Jira-Cloud-API

- [ ] API-Token-Anmeldung für die benötigten Scrum-Endpunkte prüfen.
- [ ] paginierte Suche zugänglicher Projekte und Filterung auf den MVP-Scope prüfen.
- [ ] projektbezogene Boardermittlung für kein, ein und mehrere Scrum-Boards prüfen.
- [ ] aktive Sprints eines Boards einschließlich paralleler Sprints laden und zukünftige/geschlossene Zustände ausschließen.
- [ ] Sprint-Issues einzeln sowie für `Alle aktiven Sprints` laden, stabil über Issue-ID deduplizieren und als Teilfolge der globalen Jira-Boardreihenfolge projizieren.
- [ ] offiziell unterstützten Jira-Cloud-Leseweg für die globale Boardreihenfolge verifizieren; JiraTui dient als Verhaltensreferenz, nicht als Freigabe undokumentierter Endpunkte.
- [ ] Rank-Custom-Field pro Board dynamisch aus Konfiguration oder Jira-Feldmetadaten erkennen und die Sortierrichtung gegen eine anonymisierte reale Jira-/JiraTui-Fixture bestätigen.
- [ ] nachweisen, dass Pagination, Antwortlatenz und Collection-Iteration die Reihenfolge nicht verändern und jede Antwortposition als stabiler `BoardOrdinal` erhalten bleibt.
- [ ] letzten Projekt-/Board-/Sprintkontext lokal ohne Netzblockade wiederherstellen, offline beibehalten und nach erfolgreicher Jira-Antwort validieren.
- [ ] Boardkonfiguration, Issues, Changelog, Kommentare, Transitionen und Status-Mappings gegen echte Antworten verifizieren.
- [ ] Pagination, Rate Limits, Berechtigungsfehler und eingeschränkte Token-Scopes prüfen.
- [ ] mehrdeutige beziehungsweise feldpflichtige Transitionen praktisch erfassen.
- [ ] offiziell dokumentierten Leseweg für Development Information prüfen und andernfalls `Unavailable` bestätigen.
- [ ] keine internen `dev-status`-Endpunkte, Browserautomation oder HTML-Scraping verwenden.

### G5.4 Credential Stores

- [ ] Port und Fehlerzustände für Windows, Linux und macOS definieren.
- [ ] mindestens einen realen Credential-Store-Adapter implementieren und testen.
- [ ] Verfügbarkeit oder klare Installationsvoraussetzung für die übrigen Zielsysteme prüfen.
- [ ] nachweisen, dass Token weder in SQLite noch Konfiguration, Logs oder Crashdaten geschrieben wird.

### Nachweis

- Animation-ADR: _offen_
- AOT-ADR: _offen_
- Jira-API-ADR: _offen_
- Credential-Store-ADR: _offen_

## G6 – Test- und visuelle Referenzumgebung

### Blocker

- [x] stabile xUnit-Version und eingebaute Assertions beziehungsweise kleine F#-Helper festsetzen; Nachweis: `xunit.v3 3.2.2` ist zentral gepinnt, [`JiraBoard.Tests`](tests/JiraBoard.Tests/JiraBoard.Tests.fsproj) läuft über Microsoft Testing Platform und [`TestResult.assertOk`](tests/JiraBoard.Tests/TestSupport.fs) verwendet ausschließlich xUnit-`Assert`.
- [x] mit einem negativen Kontrolltest nachweisen, dass ein absichtlich eingebrachter direkter oder transitiver FluentAssertions-Treffer die CI zuverlässig fehlschlagen lässt; Nachweis: [`eng/tests/check-fluent-assertions.Tests.ps1`](eng/tests/check-fluent-assertions.Tests.ps1) erzeugt ein isoliertes Laufzeit-Fixture für Central Package Management, Lockfile und Restore-Assets, erwartet den Hard Fail und entfernt das Fixture anschließend.
- [x] mit einem Quelltext-Kontrolltest nachweisen, dass `open FluentAssertions`, ein Alias oder ein Kompatibilitätswrapper zuverlässig erkannt wird; Nachweis: dieselbe kombinierte Negativkontrolle prüft einen Alias und eine generische kompatible `Should<T>`-Extension; der wortbasierte Marker-Scan deckt Imports und vollqualifizierte Aufrufe identisch ab.
- [ ] kanonisches Betriebssystem für blockierende Golden-Master-Vergleiche bestimmen.
- [ ] die in der UI-Design-Spezifikation festgelegten Builds `Iosevka Aile` und `Iosevka Fixed` samt exakter Version und Lizenztext bündeln und auf dem Runner reproduzierbar laden.
- [ ] per UiCatalog- und Headless-Szenario nachweisen, dass Issue-Keys `Iosevka Fixed` und normale UI-Texte `Iosevka Aile` verwenden.
- [ ] Culture, Zeitzone, Viewport, Betriebssystem-DPI, App-Zoom, Schriftzoom und Render-Skalierung explizit festsetzen.
- [ ] UiCatalog und VisualTests ohne Netzwerk, echte Uhr oder zufällige IDs ausführbar machen.
- [ ] Animation-Keyframes über Fortschritt `0.00`, `0.25`, `0.50`, `0.75` und `1.00` direkt ansteuerbar machen.
- [ ] Prozess für Received-, Verified- und Diff-Bilder dokumentieren; Golden Masters nie automatisch akzeptieren.
- [ ] repräsentative Screenshot-Matrix statt vollständigem kartesischem Produkt festlegen.
- [ ] echten Plattformtest für Fokus, Tastatur und Start der veröffentlichten AOT-App vorsehen.

### Empfohlene blockierende Referenzen

- 1920 × 1080 bei 100 % App- und Schriftzoom;
- 2560 × 1440 sowie 3840 × 2160 bei 100 %;
- minimale und maximale einzelne Zoomstufen;
- kombinierte Zoomextreme als gezielte Stressfälle;
- Normalzustand, beide Hover-Scope-Arten, Review-Track, zusammengeklappte Spalten, Modal und Reduced Motion.

### Nachweis

- kanonischer Runner und Fontliste: _offen_
- erste akzeptierte Baseline: _offen_
- dokumentierter Diff-Prozess: _offen_

## G7 – CI, Security und Datenschutz

### CI-Blocker

- [ ] pro relevantem Commit Restore, Release-Build, Unit-Tests, Architekturtests und Headless-VisualTests ausführen.
- [ ] Formatierung und Compilerwarnungen reproduzierbar prüfen.
- [ ] Paketversionen, Lizenzregeln und verbotene Abhängigkeiten automatisiert prüfen; FluentAssertions-, Avalonia-nicht-Free-, unbekannte Lizenz- und nicht allowlistete Treffer sind nicht freigebbar und beenden den Lauf als Hard Fail.
- [ ] vollständigen Restore-Graphen gegen Lizenz- und Avalonia-Free-Allowlist prüfen und unbekannte Pakete bis zur manuellen Freigabe blockieren.
- [ ] Repository und CI-Konfiguration auf Avalonia-Pro-/Accelerate-Paketnamen, Portalabhängigkeiten und Lizenzschlüsselmarker prüfen.
- [ ] erzeugte `THIRD-PARTY-NOTICES.txt` auf Vollständigkeit und Reproduzierbarkeit prüfen.
- [ ] Secret-Scanning für Repository und erzeugte Testartefakte aktivieren.
- [ ] regelmäßig sowie vor Releases `JiraBoard.App` und `JiraBoard.AotSmokeTests` pro Ziel-OS mit Native AOT veröffentlichen und starten.
- [ ] CI-Artefakte für fehlgeschlagene Screenshots und relevante Logs ohne Secrets bereitstellen.

### Security-/Datenschutz-Blocker

- [ ] kurzes Threat Model für API-Token, Jira-Inhalte, lokale Snapshots, Avatare, Kommentare, Logs und externe Links erstellen.
- [ ] Datenklassifikation und lokale Aufbewahrungsregeln festlegen.
- [ ] Logging- und Exception-Redaction automatisiert testen.
- [ ] Credential Store als einzige Tokenpersistenz festlegen.
- [ ] Snapshot-Löschen boardbezogen und transaktional definieren; Credentials, letzter Projekt-/Board-/Sprintkontext und Einstellungen bleiben erhalten.
- [ ] externe Atlassian-Hilfslinks über feste HTTPS-Allowlist schützen.
- [ ] Telemetrie und Crashreporting bis zu einer ausdrücklichen Datenschutzentscheidung deaktiviert lassen.

### Nachweis

- CI-Lauf: _offen_
- Threat Model: _offen_
- Redaction-/Secret-Scan: _offen_

## G8 – Erster Vertical Slice

Der erste Slice arbeitet zunächst vollständig offline mit einer anonymisierten Fixture:

```text
Jira-Fixture
-> Anti-Corruption-Mapping
-> getestetes Domainmodell
-> Projektauswahl und zwei aktive Sprintscopes
-> eine Standard-Issue-Swimlane mit zwei Subtasks
-> UiCatalog
-> deterministisches Status-Replay
-> Unit-, Headless- und Screenshottests
```

### Enthalten

- [ ] automatischer Start mit dem zuletzt bestätigten gültigen Projekt, Board und Sprintscope;
- [ ] Projektauswahl bei Erststart sowie Fallback mit Hinweis bei nicht mehr zugänglichem Projekt oder Board;
- [ ] ein Projekt mit einem Scrum-Board und zwei gleichzeitig aktiven Sprints;
- [ ] Sprint-Menü mit `Alle aktiven Sprints` und genau einem Sprint;
- [ ] deterministische Umschaltung, Issue-Deduplizierung und Abbruch alter Kontextgenerationen;
- [ ] exakte Jira-Boardreihenfolge der Swimmlanes und Subtasks einschließlich zweier aktiver Sprints, Pagination und fehlendem Rank;
- [ ] ein Parent/Epic als ausschließlich modaler Kontext;
- [ ] ein Standard-Issue als Swimlane;
- [ ] zwei Subtasks in unterschiedlichen aktuellen Statusspalten;
- [ ] mindestens drei chronologische Statusereignisse;
- [ ] ein gefilterter inverser Status-Bounce;
- [ ] statischer aktueller Zustand ohne aktives Replay;
- [ ] Swimlane-Hover und Subtask-Hover mit jeweils genau einem kontextuellen Loop-Button;
- [ ] Stop, Ende und Reduced Motion;
- [ ] mindestens ein Pure-Domain-Test, ein Elmish-Update-Test, ein Layouttest und ein Headless-Screenshot.

### Noch nicht enthalten

- Live-Netzwerk;
- Jira-Schreiboperationen;
- echte Credentials;
- vollständige Persistenz;
- weitere Navigation oder zusätzliche Produktbereiche außerhalb der Projekt-/Sprintauswahl.

Nach Abnahme wird derselbe Slice an den read-only Jira-Adapter angeschlossen. Erst danach beginnt die breitere Featureumsetzung.

### Nachweis

- Slice-Feature-Notiz: _offen_
- Tests: _offen_
- UiCatalog-Szenario und Designabnahme: _offen_

## Definition of Ready für die breite Umsetzung

Die breite Produktimplementierung darf beginnen, wenn:

- [ ] G1 bis G8 abgeschlossen oder mit dokumentierter, akzeptierter Ausnahme versehen sind;
- [ ] keine unbekannte AOT-, UI-Performance-, Jira-API- oder Credential-Store-Blockade verbleibt;
- [ ] der UiCatalog vor der Produktanwendung arbeitet und der erste Vertical Slice dort abgenommen ist;
- [ ] TDD- und Architekturgrenzen durch laufende Tests geschützt sind;
- [ ] CI auf einem sauberen Checkout grün ist;
- [ ] Fixtures nachweislich anonymisiert und geheimnisfrei sind;
- [ ] Wiederherstellung, Projektwahl und `Alle`-/Einzel-Sprintscope mit mehreren aktiven Sprints offline vollständig getestet sind;
- [ ] Vertrags- und Regressionstests beweisen, dass Swimmlanes und Subtasks bei Pagination, Multi-Sprint-Merge, Filter, Collapse, Review-Track, Refresh und Snapshot-Restore dieselbe relative Jira-Reihenfolge behalten;
- [ ] technische Spike-Ergebnisse als ADRs in die Implementierungsregeln eingeflossen sind;
- [ ] ein Agent ohne zusätzliches Produktwissen Handover, Checkliste und ersten Slice eindeutig ausführen kann.

## Bewusst später entscheidbar

Diese Punkte blockieren den Entwicklungsstart nicht:

- endgültiger Produktname, Icon und Branding;
- Installerformat und Distributionskanal;
- Code Signing, macOS-Notarisierung und Store-Veröffentlichung vor dem ersten externen Release;
- Auto-Update-Mechanismus;
- Telemetrie und Crashreporting;
- Kanban und Company-managed Projects;
- direkte Git-Provider-Integrationen;
- endgültige Performancebudgets nach den Messwerten des ersten Spikes.

## Entscheidungs- und Nachweisprotokoll

| Datum | Gate/ADR | Entscheidung oder Nachweis | Verantwortlich |
|---|---|---|---|
| _offen_ | _offen_ | _offen_ | _offen_ |
