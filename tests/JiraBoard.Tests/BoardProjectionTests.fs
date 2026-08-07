module JiraBoard.Tests.BoardProjectionTests

open Xunit
open JiraBoard.Domain
open JiraBoard.Ui

let private standardIssue =
    { Id = IssueId "issue-standard"
      Key = "DEMO-101"
      Title = "Boardprojektion anzeigen"
      Level = StandardLevel
      ParentIssueId = None
      Column = "To Do" }

let private parentIssue =
    { Id = IssueId "issue-parent"
      Key = "DEMO-100"
      Title = "Übergeordnetes Issue"
      Level = ParentLevel
      ParentIssueId = None
      Column = "To Do" }

let private subtask key column =
    { Id = IssueId key
      Key = key
      Title = $"Subtask {key}"
      Level = SubtaskLevel
      ParentIssueId = Some standardIssue.Id
      Column = column }

[<Fact>]
let ``static board projection hides parents and puts ordered subtasks into their status columns`` () =
    let board =
        BoardProjection.project
            [ "To Do"; "In Progress" ]
            [ parentIssue
              standardIssue
              subtask "DEMO-102" "To Do"
              subtask "DEMO-103" "In Progress" ]

    Assert.Equal(1, board.Swimlanes.Length)

    let swimlane = List.exactlyOne board.Swimlanes

    Assert.Equal("DEMO-101", swimlane.Header.IssueKey)
    Assert.Equal<string list>([ "DEMO-102" ], swimlane.CardsByColumn |> Map.find "To Do" |> List.map _.IssueKey)
    Assert.Equal<string list>([ "DEMO-103" ], swimlane.CardsByColumn |> Map.find "In Progress" |> List.map _.IssueKey)
    Assert.DoesNotContain("DEMO-100", swimlane.CardsByColumn |> Map.values |> Seq.collect id |> Seq.map _.IssueKey)

[<Fact>]
let ``static board uses bounded visible column widths instead of stretching empty space`` () =
    let model =
        BoardProjection.project [ "To Do"; "In Progress" ] [ standardIssue ]

    Assert.Equal("320,12,320", BoardProjection.columnDefinitions 1920.0 model)

[<Fact>]
let ``swimlane container spans the board row and highlights the full lane on hover or focus`` () =
    let normal = BoardProjection.swimlaneContainerContract SwimlaneHeaderState.Normal
    let hover = BoardProjection.swimlaneContainerContract SwimlaneHeaderState.PointerHover
    let focus = BoardProjection.swimlaneContainerContract SwimlaneHeaderState.KeyboardFocus

    Assert.Equal(Colors.surfaceSubtle, normal.Background)
    Assert.Equal(Colors.borderStrong, normal.Border)
    Assert.Equal(Colors.surfaceSelected, hover.Background)
    Assert.Equal(Colors.primary, hover.Border)
    Assert.Equal(2.0, hover.BorderThickness, 3)
    Assert.Equal(Colors.focus, focus.Border)

[<Fact>]
let ``standard issue header spans every status column above the subtask row`` () =
    let model =
        BoardProjection.project [ "To Do"; "In Progress" ] [ standardIssue ]

    Assert.Equal(3, BoardProjection.headerColumnSpan model)

[<Fact>]
let ``swimlane header uses the full available board width independently of its issue status`` () =
    let model =
        BoardProjection.project [ "To Do"; "In Progress" ] [ standardIssue ]

    Assert.Equal(1920.0, BoardProjection.swimlaneWidth 1920.0 model, 3)

[<Fact>]
let ``status headers use the same lane inset as their status cells`` () =
    Assert.Equal(12.0, BoardProjection.statusHeaderInset, 3)
