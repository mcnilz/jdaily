module JiraBoard.Tests.BoardSurfaceTests

open Xunit
open JiraBoard.Ui

let private fixture =
    { Columns = [ "To Do"; "In Progress"; "Done" ]
      Cards =
        [ { IssueKey = "APP-1"
            SwimlaneKey = "APP-1"
            Column = "To Do" }
          { IssueKey = "APP-2"
            SwimlaneKey = "APP-1"
            Column = "In Progress" }
          { IssueKey = "APP-3"
            SwimlaneKey = "APP-3"
            Column = "To Do" } ]
      Replay = Some(SwimlaneScope "APP-1")
      Progress = 0.5
      ReducedMotion = false }

[<Fact>]
let ``board surface keeps inactive swimlanes static during a lane replay`` () =
    let projected = BoardSurface.project fixture

    Assert.Equal(3, projected.Columns.Length)

    let inactive = projected.Cards |> List.find (fun card -> card.IssueKey = "APP-3")

    Assert.False(inactive.IsReplayActive)
    Assert.Equal(0.0, inactive.Offset, 3)

[<Fact>]
let ``board surface abort and reduced motion remove spatial replay movement`` () =
    let aborted = BoardSurface.project { fixture with Replay = None }

    Assert.All(aborted.Cards, fun card -> Assert.Equal(0.0, card.Offset, 3))

    let reduced = BoardSurface.project { fixture with ReducedMotion = true }
    let active = reduced.Cards |> List.find (fun card -> card.IssueKey = "APP-1")

    Assert.True(active.IsReplayActive)
    Assert.Equal(0.0, active.Offset, 3)