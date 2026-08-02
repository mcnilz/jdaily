module JiraBoard.Tests.DragDropSpikeTests

open Xunit
open JiraBoard.Ui

let private card =
    { IssueKey = "APP-401"
      SwimlaneKey = "APP-400"
      Column = "To Do" }

let private target =
    { DragDropTarget.SwimlaneKey = "APP-400"
      Column = "In Progress" }

[<Fact>]
let ``drag start projects a ghost and valid target overlay without moving the confirmed card`` () =
    let started = DragDropSpike.start card
    let projected = DragDropSpike.project (DragDropSpike.over target started)

    Assert.Equal("To Do", projected.ConfirmedColumn)
    Assert.Equal(Some "APP-401", projected.GhostIssueKey)
    Assert.Equal(Some target, projected.HighlightedTarget)

[<Fact>]
let ``valid drop changes only the confirmed column and restores initiating focus`` () =
    let dropped = DragDropSpike.start card |> DragDropSpike.over target |> DragDropSpike.drop
    let projected = DragDropSpike.project dropped

    Assert.Equal("In Progress", projected.ConfirmedColumn)
    Assert.Equal(Some "APP-401", projected.FocusIssueKey)

[<Fact>]
let ``invalid drop and Escape preserve the confirmed column and restore initiating focus`` () =
    let invalidTarget =
        { DragDropTarget.SwimlaneKey = "APP-999"
          Column = "Done" }

    let invalid =
        DragDropSpike.start card
        |> DragDropSpike.over invalidTarget
        |> DragDropSpike.drop

    Assert.Equal("To Do", (DragDropSpike.project invalid).ConfirmedColumn)
    Assert.Equal(Some "APP-401", (DragDropSpike.project invalid).FocusIssueKey)

    let cancelled = DragDropSpike.start card |> DragDropSpike.cancel

    Assert.Equal("To Do", (DragDropSpike.project cancelled).ConfirmedColumn)
    Assert.Equal(Some "APP-401", (DragDropSpike.project cancelled).FocusIssueKey)

[<Fact>]
let ``focus loss and reduced motion cancel the drag without a spatial ghost`` () =
    let focusLost = DragDropSpike.start card |> DragDropSpike.focusLost
    let focusLostProjection = DragDropSpike.project focusLost

    Assert.Equal("To Do", focusLostProjection.ConfirmedColumn)
    Assert.Equal(Some "APP-401", focusLostProjection.FocusIssueKey)

    let reducedMotion = DragDropSpike.start card |> DragDropSpike.withReducedMotion
    let reducedMotionProjection = DragDropSpike.project reducedMotion

    Assert.Equal(None, reducedMotionProjection.GhostIssueKey)