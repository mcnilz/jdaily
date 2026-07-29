module JiraBoard.VisualTests.BoardSurfaceRenderTests

open Avalonia.FuncUI.Types
open JiraBoard.Ui
open Xunit

let private boardSurface =
    { Columns = [ "To Do"; "In Progress"; "Done" ]
      Cards = []
      Replay = None
      Progress = 0.0
      ReducedMotion = false }

[<Fact>]
let ``board surface renders a frame at the 1080p reference viewport`` () =
    let frame =
        HeadlessTestHost.captureFrame 1920 1080 (fun () ->
            BoardSurface.viewAt (DisplayScale.create 100 100) 1920.0 boardSurface)

    Assert.NotNull(frame : obj)