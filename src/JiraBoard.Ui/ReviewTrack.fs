namespace JiraBoard.Ui

open Avalonia
open Avalonia.Controls
open Avalonia.FuncUI
open Avalonia.FuncUI.DSL
open Avalonia.Layout
open Avalonia.Media

type ReviewTrackMapping =
    | ConfirmedReviewMapping
    | UnconfirmedReviewMapping
    | InvalidReviewMapping

type ReviewTrackContract =
    { Metrics: ReviewMetrics
      ReadyForCrOffset: float
      CodeReviewOffset: float
      Labels: string list
      ContentInset: float }

type ReviewTrackProjection =
    | CombinedReviewTrack of ReviewTrackContract
    | NormalColumnFallback of columnCount: int

type ReviewTrackModel =
    { NormalColumnWidth: float
      Mapping: ReviewTrackMapping
      ReadyForCrCards: TicketCardModel list
      CodeReviewCards: TicketCardModel list }

type ReviewTrackCardPlacement =
    { Side: ReviewSide
      Row: int
      Card: TicketCardModel }

[<RequireQualifiedAccess>]
module ReviewTrack =
    let private brush (color: JiraBoard.Ui.Color) =
        SolidColorBrush(Avalonia.Media.Color.Parse color.Hex) :> IBrush

    let contractAt scale normalColumnWidth mapping =
        match mapping with
        | InvalidReviewMapping
        | UnconfirmedReviewMapping -> NormalColumnFallback 2
        | ConfirmedReviewMapping ->
            let metrics =
                normalColumnWidth
                |> DisplayScale.layout scale
                |> BoardLayout.reviewMetrics

            CombinedReviewTrack
                { Metrics = metrics
                  ReadyForCrOffset = BoardLayout.reviewX metrics ReadyForCr
                  CodeReviewOffset = BoardLayout.reviewX metrics CodeReview
                  Labels = [ "Ready for CR"; "Code Review" ]
                  ContentInset = DisplayScale.layout scale Spacing.sm }

    let contract normalColumnWidth mapping =
        contractAt DisplayScale.normal normalColumnWidth mapping

    let placements model =
        let ready =
            model.ReadyForCrCards
            |> List.mapi (fun row card ->
                { Side = ReadyForCr
                  Row = row
                  Card = card })

        let codeReview =
            model.CodeReviewCards
            |> List.mapi (fun index card ->
                { Side = CodeReview
                  Row = model.ReadyForCrCards.Length + index
                  Card = card })

        ready @ codeReview

    let private heading scale label =
        TextBlock.create
            [ TextBlock.fontFamily (FontFamily Typography.compact.Family)
              TextBlock.fontSize (DisplayScale.font scale Typography.compact.Size)
              TextBlock.fontWeight (FontWeight.SemiBold)
              TextBlock.foreground (brush Colors.textSecondary)
              TextBlock.verticalAlignment VerticalAlignment.Center
              TextBlock.text label ]

    let private cardAt scale metrics offset row card =
        let baseCardWidth = metrics.CardWidth / scale.AppFactor

        let fittedCard =
            { card with
                AvailableWidth =
                    baseCardWidth + ComponentMetrics.ticketCardColumnInset }

        Border.create
            [ Canvas.left offset
              Canvas.top (
                  float row
                  * DisplayScale.layout scale ComponentMetrics.reviewCardStackStep
              )
              Border.width metrics.CardWidth
              Border.child (TicketCard.viewAt scale fittedCard) ]

    let private combinedView scale review model =
        let cardPlacements = placements model

        Border.create
            [ Border.width (
                  review.Metrics.TrackWidth + 2.0 * review.ContentInset
              )
              Border.background (brush Colors.surfaceSubtle)
              Border.borderBrush (brush Colors.borderStrong)
              Border.borderThickness (
                  Thickness(DisplayScale.layout scale Lines.normal)
              )
              Border.cornerRadius (
                  CornerRadius(DisplayScale.layout scale CornerRadii.md)
              )
              Border.padding (
                  Thickness(review.ContentInset, 0.0, review.ContentInset, 0.0)
              )
              Border.child (
                  StackPanel.create
                      [ StackPanel.spacing (DisplayScale.layout scale Spacing.sm)
                        StackPanel.children
                            [ Grid.create
                                  [ Grid.height (
                                        DisplayScale.layout
                                            scale
                                            ComponentMetrics.boardColumnHeaderHeight
                                    )
                                    Grid.columnDefinitions "*,*"
                                    Grid.children
                                        [ heading scale review.Labels[0]
                                          TextBlock.create
                                              [ Grid.column 1
                                                TextBlock.horizontalAlignment HorizontalAlignment.Right
                                                TextBlock.fontFamily (
                                                    FontFamily Typography.compact.Family
                                                )
                                                TextBlock.fontSize (
                                                    DisplayScale.font
                                                        scale
                                                        Typography.compact.Size
                                                )
                                                TextBlock.fontWeight (FontWeight.SemiBold)
                                                TextBlock.foreground (
                                                    brush Colors.textSecondary
                                                )
                                                TextBlock.verticalAlignment VerticalAlignment.Center
                                                TextBlock.text review.Labels[1] ] ] ]
                              Canvas.create
                                  [ Canvas.height (
                                        float cardPlacements.Length
                                        * DisplayScale.layout
                                            scale
                                            ComponentMetrics.reviewCardStackStep
                                    )
                                    Canvas.children
                                        [ for placement in cardPlacements do
                                              let offset =
                                                  match placement.Side with
                                                  | ReadyForCr -> review.ReadyForCrOffset
                                                  | CodeReview -> review.CodeReviewOffset

                                              cardAt
                                                  scale
                                                  review.Metrics
                                                  offset
                                                  placement.Row
                                                  placement.Card ] ] ] ]
              ) ]

    let private normalColumn scale label width cards =
        let scaledWidth = DisplayScale.layout scale width

        Border.create
            [ Border.width scaledWidth
              Border.background (brush Colors.surfaceSubtle)
              Border.borderBrush (brush Colors.border)
              Border.borderThickness (
                  Thickness(DisplayScale.layout scale Lines.normal)
              )
              Border.cornerRadius (
                  CornerRadius(DisplayScale.layout scale CornerRadii.md)
              )
              Border.padding (Thickness(DisplayScale.layout scale Spacing.sm))
              Border.child (
                  StackPanel.create
                      [ StackPanel.spacing (DisplayScale.layout scale Spacing.sm)
                        StackPanel.children
                            [ heading scale label
                              for card in cards do
                                  TicketCard.viewAt
                                      scale
                                      { card with
                                          AvailableWidth = width } ] ]
              ) ]

    let viewAt scale model =
        match contractAt scale model.NormalColumnWidth model.Mapping with
        | CombinedReviewTrack review -> combinedView scale review model
        | NormalColumnFallback _ ->
            Border.create
                [ Border.child (
                      StackPanel.create
                          [ StackPanel.orientation Orientation.Horizontal
                            StackPanel.spacing (DisplayScale.layout scale Spacing.md)
                            StackPanel.children
                                [ normalColumn
                                      scale
                                      "Ready for CR"
                                      model.NormalColumnWidth
                                      model.ReadyForCrCards
                                  normalColumn
                                      scale
                                      "Code Review"
                                      model.NormalColumnWidth
                                      model.CodeReviewCards ] ]
                  ) ]

    let view model = viewAt DisplayScale.normal model
