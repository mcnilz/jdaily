# ADR-004: Agile-Board-Leseweg nutzen und Rangrichtungsabweichung vor der Jira-Integration entscheiden

| Feld | Wert |
|---|---|
| Status | `Proposed` |
| Datum | `2026-08-02` |
| Backlog | `SPK-003` |
| Verantwortlich | Junie |
| Ersetzt | `–` |

## Fragestellung und Kontext

`SPK-003` untersucht den offiziellen Jira-Cloud-Leseweg für die globale
Boardreihenfolge, Pagination, dynamisches Rank-Feld, Sortierrichtung und
Multi-Sprint-Projektion. Die [verbindliche Boardreihenfolge](../../avalonia-fsharp-funcui-stack-handoff.md#verbindliche-jira-boardreihenfolge)
fordert unveränderte API-Reihenfolge über Seiten, dynamische Rank-Erkennung und
eine verifizierte Richtung für Subtasks. `JiraTui` ist dort nur als
Verhaltensreferenz erlaubt.

Eine Live-Antwort mit zugehöriger JiraTui-Ausgabe liegt nicht vor. Der
Produkteigentümer hat am 2. August 2026 deshalb als Ersatznachweis offizielle
Atlassian-Dokumentation, die anonymisierten Offline-Fixtures und öffentliches
JiraTui-Quellverhalten genehmigt. Diese Evidenz belegt keine Gleichheit mit
einer konkreten Jira-Cloud-Instanz.

## Entscheidung

Für die spätere Jira-Integration ist der offiziell dokumentierte Agile-Pfad
`GET /rest/agile/1.0/board/{boardId}/configuration` plus
`GET /rest/agile/1.0/board/{boardId}/issue` maßgeblich. Die Konfiguration
liefert das boardbezogene `rankCustomFieldId`; der Client leitet daraus das
Response-Feld ab und verwendet keine feste `customfield_*`-ID. Die
Issue-Antworten werden seitenweise angehängt, bevor jede Position einen
monotonen `BoardOrdinal` erhält.

Für `AllActiveSprints` bleibt die globale Boardantwort die alleinige Quelle der
sichtbaren Reihenfolge. Sprint-Mitgliedschaften bestimmen nur die stabile
Teilfolge und die Deduplizierung über `IssueId`; parallele Sprintantworten,
Latenz oder Collection-Iteration bestimmen niemals die Anzeige.

Der aktuelle JiraTui-Renderer sortiert Issues innerhalb einer Spalte mit
`OrderByDescending(issue => issue.Rank ?? EmptyRankSortValue)`. Der bestehende
Domänenvertrag `resolveBoardOrder` vergleicht vorhandene Ranks dagegen
aufsteigend. Dieser Spike übernimmt weder JiraTui-Code noch ändert er
Produktcode. Vor `JIR-007` muss der Produkteigentümer die Zielrichtung anhand
einer Live-Vertragsfixture entscheiden; bis dahin bleibt die Abweichung
sichtbar und darf nicht stillschweigend durch eine lokale Sortierung verdeckt
werden.

## Betrachtete Alternativen

| Alternative | Dafür | Dagegen | Ergebnis |
|---|---|---|---|
| Agile-Board-Issues plus Boardkonfiguration | offiziell dokumentiert, boardbezogene Rank-Metadaten und Pagination | Live-Gleichheit ist ohne Capture nicht bewiesen | gewählt |
| JiraTui `search/jql`-/Software-Pfade übernehmen | bestehendes Referenzverhalten verfügbar | kein zugelassener Nachweis für den neuen offiziellen Board-Leseweg | verworfen |
| Rank-Feld oder Reihenfolge lokal fest verdrahten | geringe Implementierungskosten | verletzt dynamisches Feld, `BoardOrdinal` und Reihenfolge-Vertrag | verworfen |

## Konsequenzen

- Positiv: Die spätere Anti-Corruption-Layer hat einen dokumentierten,
  boardbezogenen Read-only-Pfad und die Offline-Fixtures sichern Pagination und
  dynamische Rank-Auflösung ab.
- Negativ: Die freigegebene Evidenz beweist keine Live-Instanz-Gleichheit;
  außerdem bleibt die Rangrichtungsabweichung offen.
- Folgearbeit: Vor `JIR-007` eine herkunftsbelegte Live-Vertragsfixture für die
  Zielrichtung bereitstellen und den Domänenvertrag nur nach ausdrücklicher
  Produktentscheidung anpassen.

## Nachweise

- Atlassian: [Get issues for board](https://developer.atlassian.com/cloud/jira/software/rest/api-group-board/#api-rest-agile-1-0-board-boardid-issue-get) und [Get board configuration](https://developer.atlassian.com/cloud/jira/software/rest/api-group-board/#api-rest-agile-1-0-board-boardid-configuration-get), am 2. August 2026 jeweils mit HTTP 200 abgerufen.
- JiraTui: [`JiraClient.cs`](https://github.com/mcnilz/JiraTui/blob/master/src/JiraTui.Infrastructure/Services/JiraClient.cs), [`BoardRenderModelBuilder.cs`](https://github.com/mcnilz/JiraTui/blob/master/src/JiraTui.Tui/BoardRendering/BoardRenderModelBuilder.cs) und [`BoardRenderSwimlaneBuilder.cs`](https://github.com/mcnilz/JiraTui/blob/master/src/JiraTui.Tui/BoardRendering/BoardRenderSwimlaneBuilder.cs), am 2. August 2026 gelesen.
- `BoardOrderSpikeTests.fs` prüft die durch die Konfiguration bestimmte Rank-Feld-Auflösung, die Seitenfolge und die dokumentierte Richtungs-/Folgeentscheidung.
- `BoardOrderTests.fs` und `SprintProjectionTests.fs` sichern fehlende/gleiche Ranks sowie die stabile Multi-Sprint-Teilfolge ab.

## Offene Punkte

- Produkteigentümer: Vor `JIR-007` die Rangrichtung mit einer herkunftsbelegten
  Live-Vertragsfixture entscheiden.