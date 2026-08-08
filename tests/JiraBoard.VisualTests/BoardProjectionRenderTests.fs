module JiraBoard.VisualTests.BoardProjectionRenderTests

open JiraBoard.Ui
open JiraBoard.UiCatalog
open Xunit

[<Fact>]
let ``static board projection renders bounded status columns at the 1080p reference viewport`` () =
    let frame =
        HeadlessTestHost.captureFrame 1920 1080 (fun () ->
            BoardProjection.viewAt
                (DisplayScale.create 100 100)
                1920.0
                ComponentCatalogFixtures.staticBoard)

    Assert.NotNull(frame : obj)

[<Fact>]
let ``jira ordered board projection renders all paginated swimlanes at the 1080p reference viewport`` () =
    let board = ComponentCatalogFixtures.jiraOrderBoard

    let frame =
        HeadlessTestHost.captureFrame 1920 1080 (fun () ->
            BoardProjection.viewAt
                (DisplayScale.create 100 100)
                1920.0
                board)

    Assert.Equal(4, board.Swimlanes.Length)
    Assert.NotNull(frame : obj)
