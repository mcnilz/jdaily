module JiraBoard.Tests.SnapshotJsonTests

open JiraBoard.App
open Xunit

[<Fact>]
let ``snapshot JSON maps the stable context identifiers`` () =
    let json = """{"siteId":"site-7","projectId":"10001","boardId":"42","sprintId":"17"}"""

    let result = SnapshotJson.deserialize json

    match result with
    | Ok snapshot ->
        Assert.Equal("site-7", snapshot.SiteId)
        Assert.Equal("10001", snapshot.ProjectId)
        Assert.Equal("42", snapshot.BoardId)
        Assert.Equal(Some "17", snapshot.SprintId)
    | Error error -> Assert.Fail error

[<Fact>]
let ``snapshot JSON rejects a missing required board identifier`` () =
    let json = """{"siteId":"site-7","projectId":"10001","sprintId":"17"}"""

    let result = SnapshotJson.deserialize json

    match result with
    | Ok _ -> Assert.Fail "A snapshot without a board identifier must be rejected."
    | Error error -> Assert.Equal("Snapshot field 'boardId' is required.", error)

[<Fact>]
let ``snapshot JSON accepts an absent optional sprint identifier`` () =
    let json = """{"siteId":"site-7","projectId":"10001","boardId":"42"}"""

    let result = SnapshotJson.deserialize json

    match result with
    | Ok snapshot -> Assert.Equal<string option>(None, snapshot.SprintId)
    | Error error -> Assert.Fail error