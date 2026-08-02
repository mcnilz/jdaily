namespace JiraBoard.Ui

open Avalonia
open Avalonia.Controls
open Avalonia.FuncUI.DSL
open Avalonia.FuncUI.Types
open Avalonia.Layout
open Avalonia.Media

type DragDropTarget =
    { SwimlaneKey: string
      Column: string }

type DragDropState =
    { ConfirmedCard: BoardSurfaceCard
      Target: DragDropTarget option
      IsDragging: bool
      FocusIssueKey: string option
      ReducedMotion: bool }

type DragDropProjection =
    { ConfirmedColumn: string
      GhostIssueKey: string option
      HighlightedTarget: DragDropTarget option
      FocusIssueKey: string option }

[<RequireQualifiedAccess>]
module DragDropSpike =
    let start card =
        { ConfirmedCard = card
          Target = None
          IsDragging = true
          FocusIssueKey = None
          ReducedMotion = false }

    let private isValidTarget state target =
        state.ConfirmedCard.SwimlaneKey = target.SwimlaneKey
        && state.ConfirmedCard.Column <> target.Column

    let over target state =
        { state with
            Target =
                if state.IsDragging && isValidTarget state target then
                    Some target
                else
                    None }

    let private finish state column =
        { state with
            ConfirmedCard = { state.ConfirmedCard with Column = column }
            Target = None
            IsDragging = false
            FocusIssueKey = Some state.ConfirmedCard.IssueKey }

    let drop state =
        match state.Target with
        | Some target when state.IsDragging -> finish state target.Column
        | _ -> finish state state.ConfirmedCard.Column

    let cancel state = finish state state.ConfirmedCard.Column
    let focusLost state = cancel state

    let withReducedMotion (state: DragDropState) =
        { state with ReducedMotion = true }

    let project state =
        { ConfirmedColumn = state.ConfirmedCard.Column
          GhostIssueKey =
            if state.IsDragging && not state.ReducedMotion then
                Some state.ConfirmedCard.IssueKey
            else
                None
          HighlightedTarget = state.Target
          FocusIssueKey = state.FocusIssueKey }

    let private brush color =
        SolidColorBrush(Avalonia.Media.Color.Parse color.Hex) :> IBrush

    let private cardView scale state =
        let model =
            { AvailableWidth = 280.0
              IssueKey = state.ConfirmedCard.IssueKey
              Title = "Ziehen und Ablegen prüfen"
              Assignee = None
              Priority = TicketCardPriority.Standard
              State = TicketCardState.Normal }

        TicketCard.viewAt scale model

    let viewAt scale columns state: IView =
        let projection = project state

        let cells: IView list =
            columns
            |> List.indexed
            |> List.map (fun (index, column) ->
                let isTarget =
                    projection.HighlightedTarget
                    |> Option.exists (fun target -> target.Column = column)

                let isConfirmed = projection.ConfirmedColumn = column

                Border.create
                    [ Grid.column index
                      Border.minHeight (DisplayScale.layout scale ComponentMetrics.ticketCardMinimumHeight)
                      Border.margin (Thickness(DisplayScale.layout scale Spacing.sm))
                      Border.padding (Thickness(DisplayScale.layout scale Spacing.sm))
                      Border.background (brush (if isTarget then Colors.surfaceSelected else Colors.surfaceSubtle))
                      Border.borderBrush (brush (if isTarget then Colors.primary else Colors.border))
                      Border.borderThickness (Thickness(DisplayScale.layout scale (if isTarget then Lines.focus else Lines.normal)))
                      Border.child (
                          StackPanel.create
                              [ StackPanel.spacing (DisplayScale.layout scale Spacing.sm)
                                StackPanel.children
                                    [ TextBlock.create
                                          [ TextBlock.fontFamily (FontFamily Typography.componentTitle.Family)
                                            TextBlock.fontSize (
                                                DisplayScale.font scale Typography.componentTitle.Size
                                            )
                                            TextBlock.text column ]
                                      if isConfirmed then
                                          cardView scale state
                                      if projection.GhostIssueKey.IsSome && isTarget then
                                          Border.create
                                              [ Border.opacity Opacity.dragGhost
                                                Border.child (cardView scale state) ] ] ]) ])

        Grid.create
            [ Grid.columnDefinitions (String.replicate columns.Length "*," |> fun value -> value.TrimEnd(','))
              Grid.children cells ]