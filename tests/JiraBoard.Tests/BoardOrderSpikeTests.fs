module JiraBoard.Tests.BoardOrderSpikeTests

open System
open System.Text.Json
open Xunit

let private readIssuePage (rankField: string) name =
    use document = JsonDocument.Parse(Fixture.readResource name)

    document.RootElement.GetProperty("issues").EnumerateArray()
    |> Seq.map (fun issue ->
        issue.GetProperty("key").GetString(),
        issue.GetProperty("fields").GetProperty(rankField).GetString())
    |> Seq.toList

[<Fact>]
let ``fixture pages retain API order and use the board configured rank field`` () =
    use configuration = JsonDocument.Parse(Fixture.readResource "board-configuration.json")
    let rankFieldId = configuration.RootElement.GetProperty("ranking").GetProperty("rankCustomFieldId").GetInt32()
    let rankField = $"customfield_{rankFieldId}"

    let pagedIssues =
        readIssuePage rankField "issues-pagination-p1.json"
        @ readIssuePage rankField "issues-pagination-p2.json"

    Assert.Equal(10019, rankFieldId)
    Assert.Equal<string list>([ "TMS-10"; "TMS-11"; "TMS-12"; "TMS-13" ], pagedIssues |> List.map fst)
    Assert.Equal<string list>([ "0|i00010:"; "0|i00011:"; "0|i00012:"; "0|i00013:" ], pagedIssues |> List.map snd)

[<Fact>]
let ``spike evidence records the JiraTui rank direction and its domain follow-up`` () =
    let manifest = Fixture.readResource "manifest.md"

    Assert.Contains("## SPK-003 Evidence", manifest)
    Assert.Contains("OrderByDescending(issue => issue.Rank", manifest)
    Assert.Contains("ADR-004", manifest)