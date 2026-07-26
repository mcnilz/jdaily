# UI-Design-Spezifikation

## Status und Geltung

Diese Spezifikation ist die verbindliche visuelle Baseline für `JiraBoard.Ui`, `JiraBoard.UiCatalog` und die Produktoberfläche. Sie konkretisiert das [technische Handover](avalonia-fsharp-funcui-stack-handoff.md), verwendet die gemeinsame Sprache des [DDD-Glossars](domain-glossary.md) und ersetzt keine Produktregel. Die [Implementation-Readiness-Checkliste](implementation-readiness-checklist.md) verwendet sie als Abnahmekriterium für das UiCatalog-first-Gate.

Die am 19. Juli 2026 erstellten Konzeptbilder bestimmen die visuelle Richtung: freundlich, hellblau, kompakt, metallisch und mit der Direktheit einer hochwertigen 2D-Spieloberfläche. Sie sind keine Pixelvorlagen. Bei Abweichungen zwischen generiertem Bild und diesem Dokument gilt dieses Dokument. Maße werden nicht aus den Bildern abgelesen, sondern ausschließlich aus den folgenden Tokens und Regeln erzeugt.

## Visuelle Leitidee

Die Oberfläche verbindet drei Eigenschaften:

1. **Meeting-Lesbarkeit:** Issue-Key, Standard-Issue-Titel, Status, Assignee und Replayzustand sind bei 1920 × 1080 sofort erfassbar.
2. **Native Präzision:** klassische Menüleiste, kompakte Controls, klare Fokusrahmen und kein Web-Dashboard-Look.
3. **Spielerische Direktheit:** kurze räumliche Animationen, deutliche Hoverreaktion und kleine Ereigniseffekte, ohne Neon, Partikelregen oder dekorative Unruhe.

Verbindlich ausgeschlossen bleiben eine permanente linke Sidebar, dunkle oder violett-neonartige Grundästhetik, riesige Karten, globale Transportcontrols und ein Epic als sichtbare Boardkarte.

Alle Komponenten werden ausschließlich mit dem MIT-/OSS-basierten Avalonia-Free-Stack und eigenen Produktionscontrols gemäß [Lizenz- und Avalonia-Free-Policy](license-policy.md) umgesetzt. Avalonia Community-/Plus-/Pro-/Enterprise-/Accelerate-Tools, Premium-Controls, Lizenzschlüssel oder accountgebundene Designer sind keine zulässige Design- oder Implementierungsabhängigkeit. Fehlt ein fertiges Free-Control, wird der Komponentenvertrag mit freien Avalonia-Primitiven umgesetzt oder nach ausdrücklicher Entscheidung neu geschnitten; ein Agent darf nicht auf eine Premium-Komponente ausweichen.

## Maßeinheit und Skalierung

Alle Werte dieses Dokuments sind Avalonia-DIPs bei `100 %` App-Zoom. Betriebssystem-DPI, App-Zoom und Schriftzoom bleiben getrennte Faktoren.

- Layoutwert: `Tokenwert × AppZoom`
- Schriftwert: `TypografieToken × AppZoom × SchriftZoom`
- Linien unter 1 physischem Pixel werden auf einen scharf renderbaren Wert ausgerichtet.
- App-Zoom wird durch erneutes Layouten umgesetzt, niemals durch einen Root-`RenderTransform`.
- Die Referenz ist 1920 × 1080 bei 100/100 Prozent; 2560 × 1440, 3440 × 1440 und 3840 × 2160 sind verbindliche weitere Zielgrößen.
- Unterhalb der verfügbaren Mindestbreite scrollt das Board horizontal. Karten und Schrift werden nicht unter ihre Mindestmaße gequetscht.

## Design-Tokens

### Farben

| Token | Wert | Verwendung |
|---|---|---|
| `Color.Canvas` | `#F5F8FC` | Fenster- und Boardhintergrund |
| `Color.Surface` | `#FFFFFF` | Karten, Menüs und Modalflächen |
| `Color.SurfaceSubtle` | `#EDF4FC` | Toolbar, Header und ruhige Gruppenflächen |
| `Color.SurfaceHover` | `#E6F1FF` | Hover ohne Auswahlsemantik |
| `Color.SurfaceSelected` | `#DEEBFF` | ausgewählter oder aktiver Scope |
| `Color.Border` | `#D0DAE8` | normale Konturen und Rasterlinien |
| `Color.BorderStrong` | `#A8BDD6` | hervorgehobene Gruppengrenzen |
| `Color.TextPrimary` | `#172B4D` | primärer Text |
| `Color.TextSecondary` | `#5E6C84` | Metadaten und Hilfstext |
| `Color.TextDisabled` | `#8993A4` | deaktivierter Text |
| `Color.Primary` | `#2684FF` | Aktion, Auswahl und Replay |
| `Color.PrimaryHover` | `#0C66E4` | Hover einer primären Aktion |
| `Color.PrimaryPressed` | `#0055CC` | gedrückte primäre Aktion |
| `Color.Focus` | `#0C66E4` | Tastaturfokus |
| `Color.Success` | `#2CA24C` | abgeschlossen oder positiv |
| `Color.Warning` | `#F2B01E` | erhöhte Aufmerksamkeit |
| `Color.Danger` | `#DE350B` | blockiert oder Fehler |
| `Color.Info` | `#579DFF` | neutrale Information |
| `Color.MetalHighlight` | `#FFFFFF` bei 70 % | obere Innenkante metallischer Flächen |

Farbe ist niemals der einzige Informationsträger. Priority, Blocker, Fokus, Reviewseite und Replaystatus benötigen zusätzlich Form, Icon, Kontur oder Text.

### Typografie

Die Typografie verwendet zwei optisch verwandte Iosevka-Builds:

- `Iosevka Aile` als schmale, quasi-proportionale UI-Schrift für Menüs, Titel, Fließtext, Kommentare und Controls;
- `Iosevka Fixed` als echte Monospace-Schrift für Jira-Issue-Keys wie `APP-142`, technische IDs und diagnostische Codewerte.

Beide Fonts werden mit der Anwendung und dem kanonischen Screenshot-Runner gebündelt. Exakte Fontdateien, Version und Lizenztext werden vor Aufnahme in der Readiness-Checkliste festgesetzt. Native Fallbacks dürfen die Anwendung bei einem beschädigten Fontartefakt noch bedienbar halten, erzeugen aber niemals eine Golden-Master-Baseline.

```text
Font.Ui   = "Iosevka Aile"
Font.Mono = "Iosevka Fixed"
```

`Font.Mono` wird gezielt und nicht flächendeckend verwendet. Ticketitel, Beschreibungen, Kommentare, Menüs und normale Labels bleiben in `Font.Ui`. Issue-Keys erhalten durch Monospace eine schnell erkennbare, stabile Form, dürfen aber nicht wie Codeblöcke wirken.

| Token | Größe | Zeilenhöhe | Gewicht | Verwendung |
|---|---:|---:|---:|---|
| `Type.Caption` | 11 | 15 | 500 | kleine Metadaten |
| `Type.Compact` | 12 | 16 | 500 | kompakte Boardinformationen |
| `Type.Body` | 14 | 20 | 400 | normale Inhalte |
| `Type.BodyStrong` | 14 | 20 | 600 | hervorgehobene UI-Texte und Aktionen |
| `Type.ComponentTitle` | 16 | 22 | 600 | Kataloggruppe und Modalabschnitt |
| `Type.SwimlaneTitle` | 17 | 24 | 600 | Standard-Issue-Titel |
| `Type.BoardTitle` | 24 | 32 | 650 | Boardname |
| `Type.IssueKey` | 13 | 18 | 600 | Issue-Key in `Font.Mono`, Tracking `0,2` |

Lange Titel verwenden höchstens zwei Zeilen in Storyköpfen und eine Zeile mit Ellipsis in kompakten Karten. Vollständiger Text bleibt über Tooltip und Accessible Name verfügbar. Issue-Key und Titel sind getrennte Textelemente, damit Schriftfamilie, Semantik und Automation unabhängig bleiben. Die Testzeichenfolge `APP-142 · I1lO0 · PROJ-99999` muss in allen Zoomstufen eindeutig lesbar bleiben.

### Abstände, Radien und Linien

| Kategorie | Tokens |
|---|---|
| Abstände | `2`, `4`, `8`, `12`, `16`, `24`, `32`, `48` |
| Radien | `4`, `6`, `8`, `12`, `16` |
| normale Kontur | `1` |
| Fokus-/Scope-Kontur | `2` |
| Mindest-Hit-Target | `32 × 32` |
| bevorzugtes Hit-Target | `36 × 36` |

Neue Zwischenwerte sind nicht lokal zulässig. Sie benötigen zuerst einen benannten Token und einen Layouttest.

### Schatten und metallische Tiefe

| Token | Spezifikation |
|---|---|
| `Shadow.Card` | `0 1 2`, Schwarz 12 % |
| `Shadow.Hover` | `0 4 12`, Dunkelblau 14 % |
| `Shadow.Floating` | `0 8 24`, Dunkelblau 18 % |
| `Shadow.Modal` | `0 16 48`, Dunkelblau 24 % |

Metallische Wirkung entsteht durch helle Innenkante, kühle Kontur und sehr flachen vertikalen Verlauf von maximal 4 Prozent Helligkeitsdifferenz. Keine Glossy-Verläufe, Spiegelungen oder starken Bevels.

## Anwendungsshell

| Element | Maß bei 100 % | Regel |
|---|---:|---|
| native Titelleiste | Betriebssystem | nicht nachbauen, sofern Packaging nichts anderes erfordert |
| klassische Menüleiste | 32 hoch | `Datei`, `Projekt`, `Sprint`, `Board`, `Daily`, `Ansicht`, `Hilfe` |
| kompakte Toolbar | 56 hoch | Projekt/Sprintscope, Daily-Zeitraum, Aktualisieren, Filter und Einstellungen |
| Boardspalten-Header | 44 hoch | sticky innerhalb der Boardfläche |
| äußerer Boardabstand | 12 | an allen Seiten |
| Mindestfenster | 1024 × 640 | horizontales Scrollen statt unlesbarer Kompression |

Die Toolbar besitzt keine globale Replaysteuerung. Sie zeigt den bestätigten Kontext als `Projektname · Sprintscope`, beispielsweise `Phoenix · Alle aktiven Sprints`. `Aktualisieren` ist 36 DIPs hoch, verwendet einen Zählerbadge bis `99+` und bricht ein laufendes Replay vor der Datenübernahme ab.

Beim normalen Start erscheint kein Auswahlmodal: Die Shell öffnet unmittelbar den zuletzt bestätigten Projekt-, Board- und Sprintscope, zeigt falls vorhanden dessen lokalen Snapshot und kennzeichnet eine laufende Hintergrundvalidierung kompakt. Offline bleibt dieser Kontext mit Offline-Hinweis sichtbar.

### `ProjectSelectionModal`

Die Projektauswahl erscheint beim ersten Start ohne gespeicherten Kontext, wenn der gespeicherte Projekt-/Boardkontext nicht mehr geöffnet werden kann, und über `Projekt > Projekt auswählen…`. Bei einem gültigen gespeicherten Kontext startet die Anwendung direkt mit dem zuletzt bestätigten Projekt, Board und Sprintscope.

```text
Breite = min(920, Viewportbreite - 64)
Höhe  = min(720, Viewporthöhe - 96)
```

- Kopf: `Projekt auswählen`, aktive Site und verständlicher Hinweis auf Team-managed Scrum.
- Suchfeld mit 36 DIPs Höhe; filtert lokal nach Projektname und Key.
- Projektzeile mindestens 52 DIPs hoch; zeigt Key in `Font.Mono`, Namen in `Font.Ui` und Projekttyp.
- Nach fehlgeschlagener Wiederherstellung darf das zuletzt verwendete Projekt hervorgehoben sein, benötigt aber eine neue Bestätigung.
- Genau ein Scrum-Board wird automatisch verwendet; mehrere Boards erscheinen als zweiter klarer Auswahlschritt im selben Modal.
- Primäraktion `Projekt öffnen`, Sekundäraktion `Abbrechen`.
- Abbruch der automatisch erforderlichen Auswahl führt zu einem neutralen `Kein Projekt ausgewählt`-Zustand mit erneuter Auswahlaktion.
- Loading, Empty, Offline, Error und fehlende Berechtigung sind eigene Zustände ohne Layoutsprung.

### Sprint-Menü

```text
Sprint
|-- ✓ Alle aktiven Sprints
|-- Sprint Team Alpha
|-- Sprint Team Beta
`-- Sprint Team Gamma
```

- Menüzeile mindestens 32 DIPs hoch; aktueller Scope besitzt Häkchen und Accessible State.
- `Alle aktiven Sprints` steht fest an erster Position, danach Trennlinie und aktive Sprints.
- Sprints werden stabil und kulturunabhängig nach Startdatum und anschließend ordinalem Namen/ID-Fallback sortiert.
- Gleichnamige Sprints zeigen einen sekundären Boardkontext; der sichtbare Name ist niemals Identität.
- Ein Scopewechsel schließt das Menü, stoppt Replay und zeigt einen kompakten Ladezustand ohne alten Boardinhalt als neuen Kontext auszugeben.
- Bei keinem aktiven Sprint bleibt `Alle aktiven Sprints` ausgewählt, es erscheinen keine einzelnen Sprintzeilen und das Board zeigt den erklärenden Empty State.

## Boardgeometrie

### Horizontale Berechnung

Die linke Standard-Issue-/Subtask-Identitätsfläche ist Teil des Boardrasters und ausdrücklich keine Sidebar:

```text
IdentityRail = clamp(280, 20 % der Boardbreite, 360)
CollapsedColumn = 64
NormalColumn = clamp(180, verfügbare gewichtete Breite, 320)
ReviewTrack = 1,33 × NormalColumn
ReviewCard = 0,80 × ReviewTrack
ReadyForCR-Offset = 0
CodeReview-Offset = 0,20 × ReviewTrack
```

Bei der gewichteten Berechnung zählt eine normale sichtbare Spalte als `1,0`, der kombinierte Review-Track als `1,33` und eine eingeklappte Spalte als feste 64 DIPs. Maximalbreiten verhindern, dass 4K lediglich riesige Controls erzeugt; zusätzliche Breite dient mehr sichtbaren Spalten, Text und Inhalt.

### Swimlanes und Zeilen

| Element | Maß |
|---|---:|
| Abstand zwischen Swimlanes | 12 |
| Innenabstand einer Swimlane | 12 |
| Standard-Issue-Kopf | mindestens 52 hoch |
| Subtask-Zeile | 48 hoch, bei Textzoom dynamisch wachsend |
| Kartenabstand zur Spaltenkante | 8 |
| Scope-Kontur | 2, `Color.Primary` |

Jedes Standard-Level-Issue erzeugt genau eine Swimlane. Parent-/Epic-Level-Issues erzeugen weder Swimlane noch Boardkarte. Subtasks bleiben in jeder normalen, kombinierten und eingeklappten Spalte einzeln ihrer Swimlane zugeordnet.

Die vertikale Reihenfolge ist kein lokales UI-Sortierkriterium, sondern Teil des Jira-Boardvertrags. Standard-Issue-Swimlanes erscheinen in derselben Reihenfolge wie im ausgewählten Jira-Board. Innerhalb einer Swimlane und Statuszelle erscheinen Subtasks in der verifizierten Jira-Rank-Reihenfolge. Bei fehlendem oder gleichem Rank bleibt die vom Jira-Adapter gelieferte `BoardOrdinal`-Reihenfolge erhalten.

Die UI darf weder nach Issue-Key, Titel, Status, Sprint, Erstellungsdatum oder Assignee noch nach der Iterationsreihenfolge einer Map oder eines Sets neu sortieren. Sprintfilter, Suche, zusammengeklappte Spalten, Review-Track und Replay zeigen stabile Teilfolgen: Ausgeblendete Elemente werden entfernt, die relative Reihenfolge der verbleibenden Elemente bleibt unverändert. `Alle aktiven Sprints` darf insbesondere keine gruppierte Darstellung in der Reihenfolge der einzelnen Sprintantworten erzeugen.

## Komponentenverträge

### `SwimlaneHeader`

- Mindesthöhe 52 DIPs, horizontaler Innenabstand 12.
- Zeigt Issue-Key, Titel und optional kompakte Metadaten des Standard-Issues.
- Zeigt keinen Epic-Namen oder Epic-Badge.
- Hover oder Tastaturfokus hebt die gesamte Swimlane mit `Color.SurfaceSelected` und 2-DIP-Scope-Kontur hervor.
- Genau ein kontextueller `ReplayLoopButton` erscheint rechts neben dem Titel.
- Titel maximal zwei Zeilen; bei großer Schrift darf der Kopf wachsen.

### `TicketCard`

| Eigenschaft | Wert |
|---|---|
| normale Breite | Spaltenbreite minus 16 |
| Mindesthöhe | 44 |
| Innenabstand | 8 horizontal, 6 vertikal |
| Radius | 6 |
| normale Kontur | 1, `Color.Border` |
| Fokus | 2, `Color.Focus`, zusätzlicher äußerer Abstand 2 |

Pflichtvarianten sind `Normal`, `Hover`, `KeyboardFocus`, `Blocked`, `ReplayActive` und `Disabled`. Eine Karte zeigt den Issue-Key mit `Type.IssueKey`/`Font.Mono`, den kurzen Titel mit `Font.Ui`, Assignee und nur relevante Priority-/Blocker-Signale. Epic-Kontext ist verboten. Hover verändert Oberfläche und Schatten, aber nicht Größe oder Rasterposition.

### `CollapsedColumnCell`

- Verfügbare Breite 48 DIPs innerhalb der 64-DIP-Spalte.
- Mindesthöhe 36, bevorzugte Avatargröße 24.
- Zeigt pro Subtask genau ein eigenes Element; niemals Aggregation oder Stapelzähler als Ersatz.
- Varianten: `Assigned`, `Unassigned`, `AvatarFailed`, `HighPriority`, `Flagged`, `Blocked`, `Hover`, `KeyboardFocus`, `ReplayActive`.
- Priority/Blocker erscheint als 12- bis 16-DIP-Form oder Icon zusätzlich zum Avatar.
- Tooltip und Accessible Name enthalten Key, Titel, Assignee und Warnzustand.
- Hover/Fokus darf einen Overlay-Tooltip und Replaybutton zeigen, aber die Spalte nicht verbreitern.

### `ReviewTrack`

- Gesamtbreite exakt `1,33 × NormalColumn`.
- Zwei gleichwertige semantische Zielbereiche mit sichtbaren Labels `Ready for CR` und `Code Review`.
- Jede Karte exakt `0,80 × TrackWidth`.
- Ready-for-CR-Karten links bei Offset 0; Code-Review-Karten bei Offset `0,20 × TrackWidth`.
- Mehrere Karten werden vertikal gestapelt und nicht verkleinert.
- Ungültiges oder unbestätigtes Mapping fällt auf zwei normale Jira-Spalten zurück.
- Bewegung innerhalb des Tracks verwendet das kürzere Review-Motion-Token.

### `ReplayLoopButton`

- 32 × 32 DIPs, Radius 6, ein Loop-Symbol und kein Text im kompakten Zustand.
- Im Ruhezustand nur bei Hover oder Fokus des zugehörigen Scopes sichtbar.
- Tooltip `Änderungen abspielen`; aktiv `Replay stoppen`.
- Aktivzustand besitzt `Color.Primary`, sichtbare gedrückte Semantik und Accessible Name.
- Ein Button steuert genau eine Swimlane oder genau einen Subtask.

### `RefreshButton`

- Höhe 36, horizontaler Innenabstand 12, Abstand zwischen Icon/Text/Badge 8.
- Text `Aktualisieren`; Badge `1` bis `99+`.
- Ohne Pending Events kein leeres Badge anzeigen.
- Während Datenübernahme Busy-Zustand ohne Layoutsprung.
- Klick während Replay: Replaygeneration invalidieren, Effekte entfernen, dann Daten anwenden.

### `IssueModal`

```text
Breite = min(1040, Viewportbreite - 64)
Höhe  = min(760, Viewporthöhe - 96)
```

- Zentriertes modales Overlay mit `Shadow.Modal` und Radius 12.
- Kopf zeigt Issue-Key, Titel, Status, Assignee und Schließen-Aktion.
- Nur hier darf der Epic-/Parent-Kontext als schlanke Informationszeile erscheinen.
- Hauptbereich zeigt Beschreibung und Kommentare; MVP bleibt read-only.
- Boardkontext bleibt sichtbar abgedunkelt, aber nicht bedienbar.
- `Escape` schließt; Fokus bleibt eingeschlossen und kehrt fachlich zum auslösenden Issue zurück.

### Menüs und Anzeigeeinstellungen

- Menüs verwenden 32 DIPs Mindestzeilenhöhe, Radius 8 und `Shadow.Floating`.
- Aktive Option besitzt Häkchen und Text, nicht nur Hintergrundfarbe.
- App-Zoom: `75/90/100/110/125/150/175/200 %`.
- Schriftzoom: `80/90/100/110/125/150/175/200 %`.
- Replay-Geschwindigkeit: `Ruhig`, `Normal`, `Schnell`.
- Bounce-Filter unter erweiterten Daily-Einstellungen: `Aus` oder 1 bis 30 Minuten, Standard 5.

## Zustände und Interaktion

Jede interaktive Komponente besitzt mindestens:

- `Normal`
- `PointerHover`
- `KeyboardFocus`
- `Pressed`
- `Disabled`, falls fachlich möglich
- `Busy`, falls asynchron
- `Error`, falls lokal darstellbar

Fokus verwendet immer eine 2-DIP-Kontur mit mindestens 3:1 Kontrast zum direkten Umfeld. Hover allein darf keine fachliche Zustandsänderung auslösen. Pointer- und Tastaturinteraktion müssen dieselben Aktionen erreichen.

Das Board verwendet einen Tab-Einstieg und roving focus. Pfeiltasten navigieren fachlich durch Swimlanes, Spalten und Subtasks. `Leertaste` startet/stoppt Replay, `Enter` öffnet das Modal, `Escape` bricht Replay ab oder schließt das Modal.

## Replay- und Motion-Design

Die im Handover festgelegten Motion-Zeiten bleiben verbindlich. Zusätzlich gelten folgende visuelle Regeln:

- höchstens ein aktiver Replay-Scope;
- primäre Karte scharf, maximal zwei sehr schwache Bewegungsechos;
- Trail in `Color.Info` mit maximal 24 Prozent Deckkraft;
- Bewegung vollständig innerhalb der aktiven Swimlane;
- andere Swimlanes behalten unverändert ihren aktuellen Zustand;
- Eventsymbol 20 bis 24 DIPs, Aufstieg 28 bis 40 DIPs, maximal vier gleichzeitig sichtbare Symbole;
- Assignee grün, Label violett nur als kleines semantisches Icon, Kommentar blau, Commit neutral dunkelblau; die Gesamtoberfläche bleibt hellblau und nicht violett geprägt;
- keine zufällige Partikelwolke, kein Screen Shake, keine Rotation der Ticketkarte;
- Stop, Hover-Verlust, Fehler und Refresh entfernen alle temporären Effekte sofort und zeigen den aktuellen Zustand.

Bei Reduced Motion werden Ticketpositionen unmittelbar aktualisiert. Ein kurzer Crossfade von 120 bis 160 ms, Fokusrahmen und statische Ereignissymbole dürfen die fachliche Änderung verdeutlichen. Keine Flugbahn, kein Overshoot und keine Bewegungsechos.

## UiCatalog-Struktur

Der UiCatalog ist eine native Avalonia-Anwendung mit klassischer Menüleiste und einer 48 DIPs hohen Kontrollleiste. Diese enthält Viewport, App-Zoom, Schriftzoom, Motion-Preset, Reduced-Motion-Schalter und einen direkt einstellbaren Animationsfortschritt.

### Komponentenbereiche

1. `TicketCard`
2. `CollapsedColumnCell`
3. `SwimlaneHeader`
4. `ReviewTrack`
5. `Actions`
6. `EventEffects`
7. `IssueModal`
8. `DesignTokens`
9. `Typography`
10. `ProjectSelectionModal`
11. `SprintMenu`
12. `DisplaySettings`
13. `SystemStates`

### Verbindliche benannte Szenarien

| Szenario-ID | Inhalt |
|---|---|
| `Board.Idle.1080p` | aktueller Zustand ohne Replayhinweise |
| `Navigation.ContextRestore.Startup` | letztes gültiges Projekt, Board und Sprintscope werden ohne Modal geöffnet |
| `Navigation.ProjectSelection.FirstStart` | Projektauswahl ohne gespeicherten Kontext |
| `Navigation.ProjectSelection.RestoreFailed` | Projektauswahl mit Hinweis und hervorgehobenem letzten Projekt |
| `Navigation.ProjectSelection.MultipleBoards` | expliziter zweiter Boardauswahlschritt |
| `Navigation.ProjectSelection.Cancelled` | neutraler Zustand ohne geladenes Projekt |
| `Navigation.SprintMenu.AllActive` | alle aktiven Sprints ausgewählt |
| `Navigation.SprintMenu.Single` | genau ein aktiver Sprint ausgewählt |
| `Navigation.SprintMenu.DuplicateNames` | gleichnamige Sprints mit Boardkontext |
| `Navigation.SprintMenu.StoredSprintClosed` | sichtbarer Fallback auf alle aktiven Sprints |
| `Navigation.NoActiveSprints` | erklärender Empty State ohne Backlog |
| `Board.SwimlaneHover` | Standard-Issue-Scope mit genau einem Loop-Button |
| `Board.SubtaskHover` | Subtask-Scope mit genau einem Loop-Button |
| `Board.CollapsedColumns` | jeder Subtask einzeln in Done/Red Carpet |
| `Board.Ordering.JiraBoardOrder` | Swimmlanes und Subtasks folgen der Jira-Boardreihenfolge |
| `Board.Ordering.MultiSprintStableMerge` | mehrere aktive Sprints bilden eine stabile Teilfolge der globalen Boardreihenfolge |
| `Board.Ordering.MissingAndEqualRank` | API-Ordinal bleibt bei fehlendem oder gleichem Rank stabil |
| `Board.Ordering.PaginationBoundary` | Reihenfolge bleibt über Seitengrenzen erhalten |
| `Board.ReviewTrack.Ready` | 80-Prozent-Karte links |
| `Board.ReviewTrack.CodeReview` | 80-Prozent-Karte um 20 Prozent versetzt |
| `Board.ReviewTrack.Multiple` | vertikale Stapelung |
| `Board.ReviewTrack.InvalidMapping` | sichere normale Spalten |
| `Replay.Swimlane.Progress25` | lane-lokaler früher Keyframe |
| `Replay.Swimlane.Progress50` | Bewegung und Eventsymbole |
| `Replay.Swimlane.Progress75` | Zielannäherung |
| `Replay.Subtask.Progress50` | ausschließlich ein Subtask |
| `Replay.ReducedMotion` | Crossfade ohne räumliche Effekte |
| `TicketCard.AllStates` | Normal/Hover/Fokus/Blockiert/Replay/Disabled |
| `Typography.UiAndIssueKeys` | Iosevka Aile plus Iosevka Fixed mit `APP-142 · I1lO0 · PROJ-99999` |
| `CollapsedCell.AllStates` | Assigned/Unassigned/Fehler/Priority/Flag/Blocker |
| `Actions.RefreshCounter` | 0, 7 und 99+ Pending Events |
| `Actions.ReplayLoop` | verborgen, Hover, Fokus und aktiv |
| `Modal.StandardIssue.WithEpicContext` | Epic nur im Modal |
| `Modal.StandardIssue.WithoutEpic` | Modal ohne leere Epiczeile |
| `Settings.Display.AllLevels` | Zoom- und Motionoptionen |
| `Settings.Daily.BounceWindow` | Aus, 5 und 30 Minuten |
| `System.Loading` | Skeleton ohne Layoutsprung |
| `System.Empty` | leeres Scrum-Board |
| `System.Offline` | Snapshot plus Offlinehinweis |
| `System.Error` | verständlicher Fehler mit Retry |

Alle Szenarien verwenden dieselben deterministischen Fixtures wie Unit- und VisualTests. Netzwerk, echte Uhr, zufällige IDs und Systemavatare sind verboten.

## Visuelle Testmatrix

Nicht jede Kombination erhält einen Golden Master. Alle Werte werden funktional geprüft; folgende repräsentative Kombinationen sind visuell blockierend:

| Viewport | App-Zoom | Schriftzoom | Pflichtzustände |
|---|---:|---:|---|
| 1920 × 1080 | 100 | 100 | alle zentralen Board- und Komponentenszenarien |
| 2560 × 1440 | 100 | 100 | Idle, ReviewTrack, Modal |
| 3440 × 1440 | 100 | 100 | Idle und breite Workflows |
| 3840 × 2160 | 100 | 100 | Idle, Modal und Textschärfe |
| 1920 × 1080 | 75 | 100 | minimale App-Dichte |
| 1920 × 1080 | 200 | 100 | maximale App-Skalierung mit Scrollen |
| 1920 × 1080 | 100 | 80 | minimale Schrift |
| 1920 × 1080 | 100 | 200 | Reflow, Wachstum und Ellipsis |
| 1920 × 1080 | 75 | 200 | gezielter kombinierter Stressfall |
| 1920 × 1080 | 200 | 80 | gezielter kombinierter Stressfall |

Animationen werden zusätzlich bei Fortschritt `0.00`, `0.25`, `0.50`, `0.75` und `1.00` gerendert. Golden Masters werden nur nach ausdrücklicher Designentscheidung aktualisiert.

## Implementierungsreihenfolge

1. Tokens als F#-Module und pure Layoutfunktionen anlegen.
2. Token- und Geometrietests zuerst fehlschlagen lassen.
3. UiCatalog-Shell und deterministische Szenarioregistrierung erstellen.
4. `TicketCard`, `CollapsedColumnCell`, `SwimlaneHeader` und `ReviewTrack` im Katalog implementieren.
5. Zustände, Fokus, Tooltip und Automation absichern.
6. Replay-Keyframes und Reduced Motion implementieren.
7. Modal, Menüs und Systemzustände ergänzen.
8. Headless-Baselines abnehmen.
9. Erst danach dieselben Produktionskomponenten in `JiraBoard.App` verdrahten.

## Definition of Done für eine UI-Komponente

Eine Komponente ist erst fertig, wenn:

- [ ] ausschließlich zentrale Tokens und pure Layoutmetriken verwendet werden;
- [ ] alle verbindlichen Zustände im UiCatalog sichtbar sind;
- [ ] kurze, lange, fehlende und fehlerhafte Daten dargestellt werden;
- [ ] Pointer, Tastatur und Automation denselben fachlichen Vertrag erreichen;
- [ ] 100 Prozent und relevante Zoomextreme ohne Überlappung funktionieren;
- [ ] Reduced Motion geprüft ist, falls Animation vorkommt;
- [ ] pure Geometrie-/Zustandstests und mindestens ein Headless-Test vorhanden sind;
- [ ] Golden Master nur bei visuell relevanter Komponente freigegeben wurde;
- [ ] keine zweite Katalogimplementierung neben der Produktionskomponente existiert;
- [ ] die Komponente erhält die vorgegebene `BoardOrdinal`-Reihenfolge und führt keine verdeckte lokale Sortierung ein;
- [ ] Issue-Keys `Font.Mono` und alle normalen UI-Texte `Font.Ui` verwenden;
- [ ] gebündelte Fontversion und Lizenztext reproduzierbar festgesetzt sind;
- [ ] Projekt- und Sprintnavigation mit gleichen Namen, leerem Zustand, Fokus, Automation und Kontextwechsel getestet ist;
- [ ] Epic-Kontext niemals versehentlich auf Boardkarte oder Swimlane erscheint.

## Änderungsregel

Änderungen an Farben, Typografie, Grundmaßen, Reviewgeometrie, Motion oder Komponentenverträgen benötigen:

1. einen benannten Designgrund;
2. Aktualisierung dieses Dokuments und der Tokens im selben Change;
3. aktualisierte pure Tests;
4. manuelle Prüfung im UiCatalog;
5. ausdrückliche Freigabe neuer Golden Masters.

Ein Coding-Agent darf visuelle Rohwerte nicht lokal „passend machen“. Wenn ein Zustand mit den vorhandenen Tokens nicht lösbar ist, meldet er den Konflikt und schlägt eine zentrale Token- oder Vertragsänderung vor.
