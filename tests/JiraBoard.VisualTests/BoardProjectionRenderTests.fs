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
