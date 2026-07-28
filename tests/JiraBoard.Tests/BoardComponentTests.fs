module JiraBoard.Tests.BoardComponentTests

open Xunit
open JiraBoard.Ui

[<Fact>]
let ``display scale keeps app and font zoom as separate layout factors`` () =
    let scale = DisplayScale.create 150 200

    Assert.Equal(12.0, DisplayScale.layout scale 8.0, 3)
    Assert.Equal(42.0, DisplayScale.font scale 14.0, 3)
    Assert.Equal(1.5, scale.AppFactor, 3)
    Assert.Equal(2.0, scale.FontFactor, 3)

[<Fact>]
let ``production component geometry remeasures at app zoom`` () =
    let scale = DisplayScale.create 200 80
    let ticket = TicketCard.contractAt scale TicketCardState.Normal
    let cell = CollapsedColumnCell.contractAt scale CollapsedColumnCellState.Assigned
    let header = SwimlaneHeader.contractAt scale SwimlaneHeaderState.Normal

    Assert.Equal(88.0, ticket.MinimumHeight, 3)
    Assert.Equal(2.0, ticket.BorderThickness, 3)
    Assert.Equal(96.0, cell.Width, 3)
    Assert.Equal(2.0, cell.BorderThickness, 3)
    Assert.Equal(104.0, header.MinimumHeight, 3)
    Assert.Equal(2.0, header.BorderThickness, 3)

    match ReviewTrack.contractAt scale 200.0 ConfirmedReviewMapping with
    | CombinedReviewTrack review -> Assert.Equal(532.0, review.Metrics.TrackWidth, 3)
    | NormalColumnFallback _ -> Assert.Fail "Confirmed mapping must remain combined"

[<Fact>]
let ``ticket focus spacing stays inside the cards assigned outer width`` () =
    let model =
        { AvailableWidth = 228.8
          IssueKey = "APP-302"
          Title = "Review fokussieren"
          Assignee = Some "Alex"
          Priority = TicketCardPriority.Standard
          State = TicketCardState.KeyboardFocus }

    let scale = DisplayScale.create 200 100
    let visual = TicketCard.contractAt scale model.State
    let assignedWidth = DisplayScale.layout scale model.AvailableWidth - visual.ColumnInset
    let borderWidth = TicketCard.borderWidth scale model

    Assert.Equal(assignedWidth, borderWidth + 2.0 * visual.OuterFocusSpacing, 3)

[<Fact>]
let ``ticket card states preserve geometry and expose their semantic emphasis`` () =
    let states =
        [ TicketCardState.Normal
          TicketCardState.PointerHover
          TicketCardState.KeyboardFocus
          TicketCardState.Blocked
          TicketCardState.ReplayActive
          TicketCardState.Disabled ]

    let contracts = states |> List.map TicketCard.contract

    for contract in contracts do
        Assert.Equal(44.0, contract.MinimumHeight, 3)
        Assert.Equal(8.0, contract.HorizontalPadding, 3)
        Assert.Equal(6.0, contract.VerticalPadding, 3)
        Assert.Equal(6.0, contract.CornerRadius, 3)

    Assert.Equal(Colors.surfaceHover, contracts[1].Background)
    Assert.Equal(Colors.focus, contracts[2].Border)
    Assert.Equal(2.0, contracts[2].BorderThickness, 3)
    Assert.Equal(Some Colors.danger, contracts[3].Signal)
    Assert.Equal(Some Colors.primary, contracts[4].Signal)
    Assert.Equal(Colors.textDisabled, contracts[5].Foreground)

    Assert.Equal(16.0, contracts[0].ColumnInset, 3)
    Assert.Equal(Some Shadows.card, contracts[0].Shadow)
    Assert.Equal(Some Shadows.hover, contracts[1].Shadow)
    Assert.Equal(2.0, contracts[2].OuterFocusSpacing, 3)

[<Fact>]
let ``ticket card keeps assignee visible beside blocker and replay signals`` () =
    let model state =
        { AvailableWidth = 320.0
          IssueKey = "APP-142"
          Title = "Replay nachvollziehen"
          Assignee = Some "Mara"
          Priority = TicketCardPriority.Standard
          State = state }

    Assert.Equal("Mara · BLOCKIERT", TicketCard.trailingText (model TicketCardState.Blocked))
    Assert.Equal("Mara · REPLAY", TicketCard.trailingText (model TicketCardState.ReplayActive))

[<Fact>]
let ``ticket card exposes high priority independently from interaction state`` () =
    let model =
        { AvailableWidth = 320.0
          IssueKey = "APP-143"
          Title = "Kritischen Fehler prüfen"
          Assignee = Some "Mara"
          Priority = TicketCardPriority.High
          State = TicketCardState.Normal }

    Assert.Equal(Some Colors.warning, TicketCard.signalColor model)
    Assert.Equal("Mara · HOCH", TicketCard.trailingText model)

[<Fact>]
let ``collapsed column cell keeps one subtask visible across all required states`` () =
    let states =
        [ CollapsedColumnCellState.Assigned
          CollapsedColumnCellState.Unassigned
          CollapsedColumnCellState.AvatarFailed
          CollapsedColumnCellState.HighPriority
          CollapsedColumnCellState.Flagged
          CollapsedColumnCellState.Blocked
          CollapsedColumnCellState.PointerHover
          CollapsedColumnCellState.KeyboardFocus
          CollapsedColumnCellState.ReplayActive ]

    let contracts = states |> List.map CollapsedColumnCell.contract

    Assert.Equal(9, contracts.Length)

    for contract in contracts do
        Assert.Equal(48.0, contract.Width, 3)
        Assert.Equal(36.0, contract.MinimumHeight, 3)
        Assert.Equal(24.0, contract.AvatarSize, 3)
        Assert.True(contract.IsInteractive)

    Assert.Equal(Some Colors.warning, contracts[3].Signal)
    Assert.Equal(None, contracts[4].Signal)
    Assert.Equal(Some Colors.danger, contracts[4].FlagColor)
    Assert.Equal(Some Colors.danger, contracts[5].Signal)
    Assert.Equal(Colors.focus, contracts[7].Border)
    Assert.Equal(Colors.primary, contracts[8].Border)

[<Fact>]
let ``collapsed cell describes issue assignee and warning for tooltip and automation`` () =
    let model: CollapsedColumnCellModel =
        { IssueKey = "APP-217"
          Title = "Boardreihenfolge erhalten"
          Assignee = Some "Noah"
          State = CollapsedColumnCellState.Blocked }

    Assert.Equal(
        "APP-217 · Boardreihenfolge erhalten · Noah · Blockiert",
        CollapsedColumnCell.accessibleName model
    )

[<Fact>]
let ``collapsed cell renders first and last name initials`` () =
    Assert.Equal("MS", CollapsedColumnCell.initials (Some "Martin Schnabel"))
    Assert.Equal("M", CollapsedColumnCell.initials (Some "Martin"))
    Assert.Equal("–", CollapsedColumnCell.initials None)

[<Fact>]
let ``flagged collapsed cell uses a dedicated red flag instead of a warning dot`` () =
    let contract = CollapsedColumnCell.contract CollapsedColumnCellState.Flagged

    Assert.Equal(Some Colors.danger, contract.FlagColor)
    Assert.Equal(None, contract.Signal)

[<Fact>]
let ``swimlane header reveals one contextual replay action without parent context`` () =
    let normal = SwimlaneHeader.contract SwimlaneHeaderState.Normal
    let hover = SwimlaneHeader.contract SwimlaneHeaderState.PointerHover
    let focus = SwimlaneHeader.contract SwimlaneHeaderState.KeyboardFocus
    let replay = SwimlaneHeader.contract SwimlaneHeaderState.ReplayActive

    for contract in [ normal; hover; focus; replay ] do
        Assert.Equal(52.0, contract.MinimumHeight, 3)
        Assert.Equal(12.0, contract.HorizontalPadding, 3)

    Assert.False(normal.ReplayButtonVisible)
    Assert.True(hover.ReplayButtonVisible)
    Assert.True(focus.ReplayButtonVisible)
    Assert.True(replay.ReplayButtonVisible)
    Assert.Equal(Colors.surfaceSelected, hover.Background)
    Assert.Equal(Colors.focus, focus.Border)
    Assert.Equal(Colors.primary, replay.Border)
    Assert.Equal(Some "Änderungen abspielen", hover.ReplayActionName)
    Assert.Equal(Some "Replay stoppen", replay.ReplayActionName)

[<Fact>]
let ``review track uses confirmed geometry and falls back for invalid mapping`` () =
    let confirmed = ReviewTrack.contract 200.0 ConfirmedReviewMapping

    match confirmed with
    | CombinedReviewTrack contract ->
        Assert.Equal(266.0, contract.Metrics.TrackWidth, 3)
        Assert.Equal(212.8, contract.Metrics.CardWidth, 3)
        Assert.Equal(0.0, contract.ReadyForCrOffset, 3)
        Assert.Equal(53.2, contract.CodeReviewOffset, 3)
        Assert.Equal<string list>([ "Ready for CR"; "Code Review" ], contract.Labels)
        Assert.Equal(8.0, contract.ContentInset, 3)
    | NormalColumnFallback _ ->
        Assert.Fail "Confirmed mapping must produce the combined review track"

    match ReviewTrack.contract 200.0 InvalidReviewMapping with
    | NormalColumnFallback columnCount -> Assert.Equal(2, columnCount)
    | CombinedReviewTrack _ ->
        Assert.Fail "Invalid mapping must fall back to two normal Jira columns"

    match ReviewTrack.contract 200.0 UnconfirmedReviewMapping with
    | NormalColumnFallback columnCount -> Assert.Equal(2, columnCount)
    | CombinedReviewTrack _ ->
        Assert.Fail "Unconfirmed mapping must fall back to two normal Jira columns"

[<Fact>]
let ``review track assigns every card a distinct vertical row`` () =
    let card issueKey =
        { AvailableWidth = 240.0
          IssueKey = issueKey
          Title = issueKey
          Assignee = None
          Priority = TicketCardPriority.Standard
          State = TicketCardState.Normal }

    let model =
        { NormalColumnWidth = 240.0
          Mapping = ConfirmedReviewMapping
          ReadyForCrCards = [ card "APP-303"; card "APP-304" ]
          CodeReviewCards = [ card "APP-305" ] }

    let placements = ReviewTrack.placements model

    Assert.Equal<int list>([ 0; 1; 2 ], placements |> List.map _.Row)
    Assert.Equal<string list>(
        [ "APP-303"; "APP-304"; "APP-305" ],
        placements |> List.map _.Card.IssueKey
    )
