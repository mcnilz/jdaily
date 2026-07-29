module JiraBoard.Tests.UiCatalogShellTests

open Xunit
open JiraBoard.Ui
open JiraBoard.UiCatalog

[<Fact>]
let ``catalog starts in the deterministic 1080p inspection state`` () =
    let state = CatalogShell.init ()

    Assert.Equal("1920 × 1080", state.Viewport.Name)
    Assert.Equal(1920, state.Viewport.Width)
    Assert.Equal(1080, state.Viewport.Height)
    Assert.Equal(100, state.AppZoomPercent)
    Assert.Equal(100, state.FontZoomPercent)
    Assert.Equal(Motion.Normal, state.MotionPreset)
    Assert.False(state.ReducedMotion)
    Assert.Equal(0.0, state.AnimationProgress, 3)

[<Fact>]
let ``catalog exposes the specified deterministic inspection choices`` () =
    let viewportNames = CatalogShell.viewportPresets |> List.map _.Name

    Assert.Equal<string list>(
        [ "1920 × 1080"; "2560 × 1440"; "3440 × 1440"; "3840 × 2160" ],
        viewportNames
    )

    Assert.Equal<int list>([ 75; 90; 100; 110; 125; 150; 175; 200 ], CatalogShell.appZoomLevels)
    Assert.Equal<int list>([ 80; 90; 100; 110; 125; 150; 175; 200 ], CatalogShell.fontZoomLevels)
    Assert.Equal<Motion.SpeedPreset list>([ Motion.Calm; Motion.Normal; Motion.Fast ], CatalogShell.motionPresets)

[<Fact>]
let ``catalog controls update the inspected presentation state`` () =
    let ultrawide = CatalogShell.viewportPresets |> List.item 2

    let state =
        CatalogShell.init ()
        |> CatalogShell.update (SelectViewport ultrawide)
        |> CatalogShell.update (SelectAppZoom 125)
        |> CatalogShell.update (SelectFontZoom 150)
        |> CatalogShell.update (SelectMotionPreset Motion.Fast)
        |> CatalogShell.update (SetReducedMotion true)
        |> CatalogShell.update (SetAnimationProgress 0.75)

    Assert.Equal("3440 × 1440", state.Viewport.Name)
    Assert.Equal(125, state.AppZoomPercent)
    Assert.Equal(150, state.FontZoomPercent)
    Assert.Equal(Motion.Fast, state.MotionPreset)
    Assert.True(state.ReducedMotion)
    Assert.Equal(0.75, state.AnimationProgress, 3)

[<Fact>]
let ``catalog registers and selects its deterministic shell scenario`` () =
    let scenarioIds = CatalogScenarios.all |> List.map _.Id
    let state = CatalogShell.init ()

    Assert.Contains("Shell.Overview", scenarioIds)
    Assert.Equal("Shell.Overview", state.SelectedScenarioId)

[<Fact>]
let ``catalog overview describes the available production components`` () =
    Assert.Equal(
        "Produktionskomponenten und ihre Pflichtzustände sind als auswählbare Szenarien verfügbar.",
        CatalogView.overviewDescription
    )

[<Fact>]
let ``catalog registers every UI-005 production component scenario`` () =
    let scenarioIds = CatalogScenarios.all |> List.map _.Id

    let required =
        [ "TicketCard.AllStates"
          "TicketCard.DataVariants"
          "CollapsedCell.AllStates"
          "CollapsedCell.DataVariants"
          "Board.SwimlaneHover"
          "SwimlaneHeader.AllStates"
          "SwimlaneHeader.DataVariants"
          "Board.ReviewTrack.Ready"
          "Board.ReviewTrack.CodeReview"
          "Board.ReviewTrack.Multiple"
          "Board.ReviewTrack.DataVariants"
          "Board.ReviewTrack.InvalidMapping"
          "Board.ReviewTrack.UnconfirmedMapping"
          "Board.Surface.SwimlaneReplay"
          "Board.Surface.SubtaskReplay"
          "Board.Surface.Aborted"
          "Board.Surface.ReducedMotion" ]

    for scenarioId in required do
        Assert.Contains(scenarioId, scenarioIds)

[<Fact>]
let ``catalog shares deterministic component fixtures with tests`` () =
    Assert.Equal(6, ComponentCatalogFixtures.ticketCards.Length)
    Assert.Equal(9, ComponentCatalogFixtures.collapsedCells.Length)
    Assert.Equal(4, ComponentCatalogFixtures.swimlaneHeaders.Length)
    Assert.Equal(SwimlaneHeaderState.PointerHover, ComponentCatalogFixtures.swimlaneHover.State)
    Assert.Equal(4, ComponentCatalogFixtures.ticketCardDataVariants.Length)
    Assert.Equal(4, ComponentCatalogFixtures.collapsedCellDataVariants.Length)
    Assert.Equal(4, ComponentCatalogFixtures.swimlaneHeaderDataVariants.Length)
    Assert.Equal(4, ComponentCatalogFixtures.reviewTrackDataVariants.Length)
    Assert.Equal(2, ComponentCatalogFixtures.swimlaneHoverSubtasks.Length)
    Assert.Equal<string list>([ "To Do"; "In Progress"; "Done" ], ComponentCatalogFixtures.boardSurface.Columns)
    Assert.Equal(3, ComponentCatalogFixtures.boardSurface.Cards.Length)
    Assert.Equal(Some(SwimlaneScope "APP-400"), ComponentCatalogFixtures.boardSurface.Replay)

    let unassigned =
        ComponentCatalogFixtures.collapsedCells
        |> List.find (fun model -> model.State = CollapsedColumnCellState.Unassigned)

    Assert.Equal<string option>(None, unassigned.Assignee)
    Assert.Contains("Nicht zugewiesen", CollapsedColumnCell.accessibleName unassigned)

    let assigned =
        ComponentCatalogFixtures.collapsedCells
        |> List.find (fun model -> model.State = CollapsedColumnCellState.Assigned)

    Assert.Equal("MS", CollapsedColumnCell.initials assigned.Assignee)

[<Fact>]
let ``catalog scenario navigation selects the production component preview`` () =
    let state =
        CatalogShell.init ()
        |> CatalogShell.update (SelectScenario "TicketCard.AllStates")

    Assert.Equal("TicketCard.AllStates", state.SelectedScenarioId)

[<Fact>]
let ``repeated viewport actions advance from the current selection`` () =
    let state =
        CatalogShell.init ()
        |> CatalogShell.update CycleViewport
        |> CatalogShell.update CycleViewport

    Assert.Equal("3440 × 1440", state.Viewport.Name)

[<Fact>]
let ``catalog shell dimensions and animation stops match the specification`` () =
    Assert.Equal(32.0, CatalogShell.layout.MenuHeight, 3)
    Assert.Equal(48.0, CatalogShell.layout.ControlBarMinimumHeight, 3)
    Assert.Equal(1024.0, CatalogShell.layout.MinimumWindowWidth, 3)
    Assert.Equal(640.0, CatalogShell.layout.MinimumWindowHeight, 3)

    Assert.Equal<float list>(
        [ 0.0; 0.25; 0.50; 0.75; 1.0 ],
        CatalogShell.animationProgressStops
    )

[<Fact>]
let ``catalog allocates readable labels for long component state names`` () =
    Assert.True(CatalogShell.layout.WrapScenarioLabels)
    Assert.True(CatalogShell.layout.CollapsedStatePreviewWidth >= 128.0)

[<Fact>]
let ``catalog scenario navigation remains vertically scrollable`` () =
    Assert.True(CatalogShell.layout.ScenarioNavigationScrollable)

[<Fact>]
let ``repeated reduced motion actions toggle from the current state`` () =
    let state =
        CatalogShell.init ()
        |> CatalogShell.update ToggleReducedMotion
        |> CatalogShell.update ToggleReducedMotion

    Assert.False(state.ReducedMotion)

[<Fact>]
let ``catalog keyboard enters the board once and moves focus by logical board geometry`` () =
    let state = CatalogKeyboard.init ComponentCatalogFixtures.keyboardBoard

    let entered = CatalogKeyboard.handle Tab state
    let down = CatalogKeyboard.handle ArrowDown entered
    let right = CatalogKeyboard.handle ArrowRight down

    Assert.Equal(Some "APP-98", entered.FocusedIssueKey)
    Assert.Equal(Some "APP-99", down.FocusedIssueKey)
    Assert.Equal(Some "APP-100", right.FocusedIssueKey)

[<Fact>]
let ``catalog keyboard leaves the board as one tab stop and keeps focus at navigation edges`` () =
    let state =
        CatalogKeyboard.init ComponentCatalogFixtures.keyboardBoard
        |> CatalogKeyboard.handle Tab
        |> CatalogKeyboard.handle ShiftTab

    let atTopEdge = CatalogKeyboard.handle ArrowUp state

    Assert.Equal(None, state.FocusedIssueKey)
    Assert.Equal(None, atTopEdge.FocusedIssueKey)

[<Fact>]
let ``catalog keyboard toggles replay opens the focused issue and escapes in precedence order`` () =
    let focused =
        CatalogKeyboard.init ComponentCatalogFixtures.keyboardBoard
        |> CatalogKeyboard.handle Tab
        |> CatalogKeyboard.handle ArrowDown

    let replaying = CatalogKeyboard.handle Space focused
    let modalOpen = CatalogKeyboard.handle Enter replaying
    let modalClosed = CatalogKeyboard.handle Escape modalOpen
    let replayStopped = CatalogKeyboard.handle Escape modalClosed

    Assert.Equal(Some "APP-99", replaying.ReplayIssueKey)
    Assert.Equal(Some "APP-99", modalOpen.ModalIssueKey)
    Assert.Equal(None, modalClosed.ModalIssueKey)
    Assert.Equal(Some "APP-99", modalClosed.ReplayIssueKey)
    Assert.Equal(None, replayStopped.ReplayIssueKey)

[<Fact>]
let ``catalog shell delegates keyboard messages to the visible board interaction state`` () =
    let state =
        CatalogShell.init ()
        |> CatalogShell.update (HandleKeyboard Tab)
        |> CatalogShell.update (HandleKeyboard ArrowDown)
        |> CatalogShell.update (HandleKeyboard Space)

    Assert.Equal(Some "APP-99", state.Keyboard.FocusedIssueKey)
    Assert.Equal(Some "APP-99", state.Keyboard.ReplayIssueKey)

[<Fact>]
let ``ticket cards describe key title assignee and warning for tooltip and automation`` () =
    let normal =
        { AvailableWidth = 320.0
          IssueKey = "APP-142"
          Title = "Daily Replay im Board nachvollziehbar machen"
          Assignee = Some "Mara"
          Priority = TicketCardPriority.Standard
          State = TicketCardState.Normal }

    let blocked = { normal with State = TicketCardState.Blocked }
    let highPriority = { normal with Priority = TicketCardPriority.High }

    Assert.Equal(
        "APP-142 · Daily Replay im Board nachvollziehbar machen · Mara · Kein Warnzustand",
        TicketCard.accessibleName normal
    )
    Assert.Equal(
        "APP-142 · Daily Replay im Board nachvollziehbar machen · Mara · Blockiert",
        TicketCard.accessibleName blocked
    )
    Assert.Equal(
        "APP-142 · Daily Replay im Board nachvollziehbar machen · Mara · Hohe Priorität",
        TicketCard.accessibleName highPriority
    )
