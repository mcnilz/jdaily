module JiraBoard.Tests.BoardSurfaceTests

open Xunit
open JiraBoard.Ui
open JiraBoard.UiCatalog

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
      Keyframes = []
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

[<Fact>]
let ``board surface projects exactly one deterministic status keyframe at each catalog stop`` () =
    let model =
        { fixture with
            Keyframes =
                [ { IssueKey = "APP-1"
                    StartProgress = 0.0
                    EndProgress = 0.25
                    Offset = 1.0 }
                  { IssueKey = "APP-2"
                    StartProgress = 0.25
                    EndProgress = 0.50
                    Offset = 1.0 }
                  { IssueKey = "APP-1"
                    StartProgress = 0.50
                    EndProgress = 0.75
                    Offset = 1.0 } ] }

    let activeIssue progress =
        BoardSurface.project { model with Progress = progress }
        |> _.ActiveKeyframe
        |> Option.map _.IssueKey

    Assert.Equal(Some "APP-1", activeIssue 0.25)
    Assert.Equal(Some "APP-2", activeIssue 0.50)
    Assert.Equal(Some "APP-1", activeIssue 0.75)

[<Fact>]
let ``board surface keeps cards outside the active keyframe static`` () =
    let model =
        { fixture with
            Progress = 0.50
            Keyframes =
                [ { IssueKey = "APP-2"
                    StartProgress = 0.25
                    EndProgress = 0.50
                    Offset = 1.0 } ] }

    let projected = BoardSurface.project model
    let inactive = projected.Cards |> List.find (fun card -> card.IssueKey = "APP-1")

    Assert.Equal(0.0, inactive.Offset, 3)

[<Fact>]
let ``completed status movement remains at its destination while the next keyframe starts`` () =
    let model =
        { fixture with
            Progress = 0.26
            Keyframes =
                [ { IssueKey = "APP-1"
                    StartProgress = 0.0
                    EndProgress = 0.25
                    Offset = 1.0 }
                  { IssueKey = "APP-2"
                    StartProgress = 0.25
                    EndProgress = 0.50
                    Offset = 1.0 } ] }

    let projected = BoardSurface.project model
    let first = projected.Cards |> List.find (fun card -> card.IssueKey = "APP-1")
    let second = projected.Cards |> List.find (fun card -> card.IssueKey = "APP-2")

    Assert.Equal(1.0, first.Offset, 3)
    Assert.Equal(0.04, second.Offset, 3)

[<Fact>]
let ``catalog replay fixture exposes three status keyframes after bounce normalization`` () =
    let keyframes = ComponentCatalogFixtures.boardSurface.Keyframes

    Assert.Equal(3, keyframes.Length)
    Assert.Equal<string list>([ "APP-401"; "APP-402"; "APP-401" ], keyframes |> List.map _.IssueKey)
    Assert.Equal<float list>([ 0.0; 0.25; 0.50 ], keyframes |> List.map _.StartProgress)

[<Fact>]
let ``catalog replay keeps the final status transition active beyond seventy-five percent`` () =
    let projected =
        BoardSurface.project
            { ComponentCatalogFixtures.boardSurface with
                Progress = 0.76 }

    Assert.Equal(Some "APP-401", projected.ActiveKeyframe |> Option.map _.IssueKey)

[<Fact>]
let ``catalog replay starts moving subtasks outside their target column`` () =
    let app402 =
        ComponentCatalogFixtures.boardSurface.Cards
        |> List.find (fun card -> card.IssueKey = "APP-402")

    Assert.Equal("To Do", app402.Column)
