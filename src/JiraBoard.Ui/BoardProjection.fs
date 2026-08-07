namespace JiraBoard.Ui

open System
open Avalonia
open Avalonia.Controls
open Avalonia.FuncUI.DSL
open Avalonia.FuncUI.Types
open Avalonia.Layout
open Avalonia.Media
open JiraBoard.Domain

/// A UI-ready issue supplied by an already resolved domain board projection.
/// Input order is Jira board order and is deliberately retained verbatim.
type BoardProjectionIssue =
    { Id: IssueId
      Key: string
      Title: string
      Level: WorkItemLevel
      ParentIssueId: IssueId option
      Column: string }

type ProjectedBoardCard =
    { IssueKey: string
      Title: string }

type ProjectedSwimlane =
    { Header: SwimlaneHeaderModel
      CardsByColumn: Map<string, ProjectedBoardCard list> }

type StaticBoardProjection =
    { Columns: string list
      Swimlanes: ProjectedSwimlane list }

type SwimlaneContainerContract =
    { Background: JiraBoard.Ui.Color
      Border: JiraBoard.Ui.Color
      BorderThickness: float }

[<RequireQualifiedAccess>]
module BoardProjection =
    let private brush (color: JiraBoard.Ui.Color) =
        SolidColorBrush(Avalonia.Media.Color.Parse color.Hex) :> IBrush

    let swimlaneContainerContract state =
        match state with
        | SwimlaneHeaderState.PointerHover ->
            { Background = Colors.surfaceSelected
              Border = Colors.primary
              BorderThickness = Lines.focus }
        | SwimlaneHeaderState.KeyboardFocus ->
            { Background = Colors.surfaceSelected
              Border = Colors.focus
              BorderThickness = Lines.focus }
        | SwimlaneHeaderState.ReplayActive ->
            { Background = Colors.surfaceSelected
              Border = Colors.primary
              BorderThickness = Lines.focus }
        | SwimlaneHeaderState.Normal ->
            { Background = Colors.surfaceSubtle
              Border = Colors.borderStrong
              BorderThickness = Lines.normal }

    let project columns issues : StaticBoardProjection =
        let cardsFor standardIssue column =
            issues
            |> List.choose (fun issue ->
                if issue.Level = SubtaskLevel
                   && issue.ParentIssueId = Some standardIssue.Id
                   && issue.Column = column then
                    Some
                        { IssueKey = issue.Key
                          Title = issue.Title }
                else
                    None)

        let swimlanes =
            issues
            |> List.choose (fun issue ->
                if issue.Level = StandardLevel then
                    Some
                        { Header =
                            { IssueKey = issue.Key
                              Title = issue.Title
                              Metadata = None
                              State = SwimlaneHeaderState.Normal
                              OnReplayRequested = ignore }
                          CardsByColumn =
                            columns
                            |> List.map (fun column -> column, cardsFor issue column)
                            |> Map.ofList }
                else
                    None)

        { Columns = columns
          Swimlanes = swimlanes }

    let private cardView scale columnWidth (card: ProjectedBoardCard): IView<Border> =
        TicketCard.viewAt
            scale
            { AvailableWidth = columnWidth
              IssueKey = card.IssueKey
              Title = card.Title
              Assignee = None
              Priority = TicketCardPriority.Standard
              State = TicketCardState.Normal }

    let private metrics boardWidth (model: StaticBoardProjection) =
        BoardLayout.calculate
            { BoardWidth = boardWidth
              NormalColumnCount = model.Columns.Length
              CollapsedColumnCount = 0
              IncludesReviewTrack = false }

    let columnDefinitions boardWidth (model: StaticBoardProjection) =
        let layout = metrics boardWidth model

        model.Columns
        |> List.collect (fun column ->
            if column = List.head model.Columns then
                [ layout.NormalColumnWidth.ToString("0") ]
            else
                [ Spacing.md.ToString("0")
                  layout.NormalColumnWidth.ToString("0") ])
        |> String.concat ","

    let headerColumnSpan (model: StaticBoardProjection) =
        max 1 (2 * model.Columns.Length - 1)

    let swimlaneWidth boardWidth (_: StaticBoardProjection) = boardWidth

    let statusHeaderInset = Spacing.md

    let private statusColumn scale columnWidth index cards =
        Border.create
            [ Grid.column index
              Border.minHeight (DisplayScale.layout scale ComponentMetrics.swimlaneHeaderMinimumHeight)
              Border.background (brush Colors.canvas)
              Border.borderBrush (brush Colors.border)
              Border.borderThickness (Thickness(DisplayScale.layout scale Lines.normal))
              Border.cornerRadius (CornerRadius(DisplayScale.layout scale CornerRadii.md))
              Border.padding (Thickness(DisplayScale.layout scale Spacing.sm))
              Border.child (
                  StackPanel.create
                      [ StackPanel.spacing (DisplayScale.layout scale Spacing.sm)
                        StackPanel.children (
                            cards
                            |> List.map (fun card ->
                                Border.create
                                    [ Border.child (
                                          cardView scale (columnWidth - 2.0 * Spacing.sm) card
                                      ) ])
                        ) ]
              ) ]

    let viewAt scale boardWidth (model: StaticBoardProjection): IView =
        let layout = metrics boardWidth model
        let definitions = columnDefinitions boardWidth model
        StackPanel.create
            [ StackPanel.spacing (DisplayScale.layout scale Spacing.md)
              StackPanel.children
                  [ Border.create
                        [ Border.horizontalAlignment HorizontalAlignment.Left
                          Border.width (DisplayScale.layout scale (swimlaneWidth boardWidth model))
                          Border.background (brush Colors.surfaceSubtle)
                          Border.borderBrush (brush Colors.border)
                          Border.borderThickness (Thickness(0.0, 0.0, 0.0, Lines.normal))
                          Border.padding (
                              Thickness(DisplayScale.layout scale statusHeaderInset, 0.0, 0.0, 0.0)
                          )
                          Border.child (
                              Grid.create
                                  [ Grid.horizontalAlignment HorizontalAlignment.Left
                                    Grid.columnDefinitions definitions
                                    Grid.children
                                        [ for index, column in model.Columns |> List.indexed do
                                              TextBlock.create
                                                   [ Grid.column (2 * index)
                                                     TextBlock.foreground (brush Colors.textPrimary)
                                                     TextBlock.text column ] ] ]
                          ) ]
                    for swimlane in model.Swimlanes do
                        let visual = swimlaneContainerContract swimlane.Header.State

                        Border.create
                            [ Border.horizontalAlignment HorizontalAlignment.Left
                              Border.width (DisplayScale.layout scale (swimlaneWidth boardWidth model))
                              Border.background (brush visual.Background)
                              Border.borderBrush (brush visual.Border)
                              Border.borderThickness (
                                  Thickness(DisplayScale.layout scale visual.BorderThickness)
                              )
                              Border.cornerRadius (
                                  CornerRadius(DisplayScale.layout scale CornerRadii.lg)
                              )
                              Border.padding (Thickness(DisplayScale.layout scale Spacing.sm))
                              Border.child (
                                  StackPanel.create
                                      [ StackPanel.spacing (DisplayScale.layout scale Spacing.sm)
                                        StackPanel.children
                                            [ SwimlaneHeader.viewAt scale swimlane.Header
                                              Grid.create
                                                  [ Grid.horizontalAlignment HorizontalAlignment.Left
                                                    Grid.columnDefinitions definitions
                                                    Grid.children
                                                        [ for index, column in model.Columns |> List.indexed do
                                                              (statusColumn
                                                                  scale
                                                                  layout.NormalColumnWidth
                                                                  (2 * index)
                                                                  (swimlane.CardsByColumn |> Map.find column)
                                                               :> IView) ] ] ] ]
                              ) ] ] ]
