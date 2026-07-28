namespace JiraBoard.UiCatalog

open Avalonia
open Avalonia.Controls
open Avalonia.Controls.Primitives
open Avalonia.FuncUI.DSL
open Avalonia.FuncUI.Types
open Avalonia.Layout
open Avalonia.Media
open JiraBoard.Ui

[<RequireQualifiedAccess>]
module ComponentCatalogView =
    let private brush color =
        SolidColorBrush(Avalonia.Media.Color.Parse color.Hex) :> IBrush

    let private section scale (title: string) (children: IView list) : IView =
        StackPanel.create
            [ StackPanel.spacing (DisplayScale.layout scale Spacing.md)
              StackPanel.children
                  [ TextBlock.create
                        [ TextBlock.fontFamily (FontFamily Typography.componentTitle.Family)
                          TextBlock.fontSize (
                              DisplayScale.font scale Typography.componentTitle.Size
                          )
                          TextBlock.fontWeight (FontWeight.SemiBold)
                          TextBlock.foreground (brush Colors.textPrimary)
                          TextBlock.text title ]
                    yield! children ] ]

    let private labeled scale (label: string) (child: IView) : IView =
        StackPanel.create
            [ StackPanel.spacing (DisplayScale.layout scale Spacing.xs)
              StackPanel.children
                  [ TextBlock.create
                        [ TextBlock.fontFamily (FontFamily Typography.caption.Family)
                          TextBlock.fontSize (
                              DisplayScale.font scale Typography.caption.Size
                          )
                          TextBlock.foreground (brush Colors.textSecondary)
                          TextBlock.textWrapping TextWrapping.Wrap
                          TextBlock.text label ]
                    child ] ]

    let private ticketStateName =
        function
        | TicketCardState.Normal -> "Normal"
        | TicketCardState.PointerHover -> "PointerHover"
        | TicketCardState.KeyboardFocus -> "KeyboardFocus"
        | TicketCardState.Blocked -> "Blocked"
        | TicketCardState.ReplayActive -> "ReplayActive"
        | TicketCardState.Disabled -> "Disabled"

    let private ticketCards scale =
        ComponentCatalogFixtures.ticketCards
        |> List.map (fun model ->
            labeled
                scale
                (ticketStateName model.State)
                (TicketCard.viewAt scale model))
        |> section scale "TicketCard · alle Pflichtzustände"

    let private ticketCardDataVariants scale =
        ComponentCatalogFixtures.ticketCardDataVariants
        |> List.map (fun fixture ->
            labeled scale fixture.Name (TicketCard.viewAt scale fixture.Model))
        |> section scale "TicketCard · kurze, lange, fehlende und fehlerhafte Daten"

    let private collapsedStateName =
        function
        | CollapsedColumnCellState.Assigned -> "Assigned"
        | CollapsedColumnCellState.Unassigned -> "Unassigned"
        | CollapsedColumnCellState.AvatarFailed -> "AvatarFailed"
        | CollapsedColumnCellState.HighPriority -> "HighPriority"
        | CollapsedColumnCellState.Flagged -> "Flagged"
        | CollapsedColumnCellState.Blocked -> "Blocked"
        | CollapsedColumnCellState.PointerHover -> "PointerHover"
        | CollapsedColumnCellState.KeyboardFocus -> "KeyboardFocus"
        | CollapsedColumnCellState.ReplayActive -> "ReplayActive"

    let private collapsedCells scale =
        let cells =
            ComponentCatalogFixtures.collapsedCells
            |> List.map (fun model ->
                labeled
                    scale
                    (collapsedStateName model.State)
                    (CollapsedColumnCell.viewAt scale model))

        section
            scale
            "CollapsedColumnCell · ein Element pro Subtask"
            [ WrapPanel.create
                  [ WrapPanel.orientation Orientation.Horizontal
                    WrapPanel.itemWidth (
                        DisplayScale.layout
                            scale
                            CatalogShell.layout.CollapsedStatePreviewWidth
                    )
                    WrapPanel.children cells ] ]

    let private collapsedCellDataVariants scale =
        let cells =
            ComponentCatalogFixtures.collapsedCellDataVariants
            |> List.map (fun fixture ->
                labeled
                    scale
                    fixture.Name
                    (CollapsedColumnCell.viewAt scale fixture.Model))

        section
            scale
            "CollapsedColumnCell · kurze, lange, fehlende und fehlerhafte Daten"
            [ WrapPanel.create
                  [ WrapPanel.orientation Orientation.Horizontal
                    WrapPanel.itemWidth (
                        DisplayScale.layout
                            scale
                            CatalogShell.layout.CollapsedStatePreviewWidth
                    )
                    WrapPanel.children cells ] ]

    let private swimlaneStateName =
        function
        | SwimlaneHeaderState.Normal -> "Normal"
        | SwimlaneHeaderState.PointerHover -> "PointerHover"
        | SwimlaneHeaderState.KeyboardFocus -> "KeyboardFocus"
        | SwimlaneHeaderState.ReplayActive -> "ReplayActive"

    let private swimlaneHeaders scale =
        ComponentCatalogFixtures.swimlaneHeaders
        |> List.map (fun model ->
            labeled
                scale
                (swimlaneStateName model.State)
                (SwimlaneHeader.viewAt scale model))
        |> section scale "SwimlaneHeader · kontextuelle Replayaktion"

    let private swimlaneHeaderDataVariants scale =
        ComponentCatalogFixtures.swimlaneHeaderDataVariants
        |> List.map (fun fixture ->
            labeled
                scale
                fixture.Name
                (SwimlaneHeader.viewAt scale fixture.Model))
        |> section
            scale
            "SwimlaneHeader · kurze, lange, fehlende und fehlerhafte Daten"

    let private swimlaneHover scale =
        Border.create
            [ Border.background (brush Colors.surfaceSelected)
              Border.borderBrush (brush Colors.primary)
              Border.borderThickness (
                  Thickness(DisplayScale.layout scale Lines.focus)
              )
              Border.cornerRadius (
                  CornerRadius(DisplayScale.layout scale CornerRadii.lg)
              )
              Border.padding (Thickness(DisplayScale.layout scale Spacing.md))
              Border.child (
                  StackPanel.create
                      [ StackPanel.spacing (DisplayScale.layout scale Spacing.sm)
                        StackPanel.children
                            [ SwimlaneHeader.viewAt
                                  scale
                                  ComponentCatalogFixtures.swimlaneHover
                              for card in ComponentCatalogFixtures.swimlaneHoverSubtasks do
                                  TicketCard.viewAt scale card ] ]
              ) ]

    let private reviewReady scale =
        section
            scale
            "ReviewTrack · Ready for CR"
            [ ReviewTrack.viewAt scale ComponentCatalogFixtures.reviewReady ]

    let private reviewCodeReview scale =
        section
            scale
            "ReviewTrack · Code Review"
            [ ReviewTrack.viewAt scale ComponentCatalogFixtures.reviewCodeReview ]

    let private reviewMultiple scale =
        section
            scale
            "ReviewTrack · vertikale Stapelung"
            [ ReviewTrack.viewAt scale ComponentCatalogFixtures.reviewMultiple ]

    let private reviewInvalid scale =
        section
            scale
            "ReviewTrack · sicherer Fallback"
            [ ReviewTrack.viewAt scale ComponentCatalogFixtures.reviewInvalid ]

    let private reviewUnconfirmed scale =
        section
            scale
            "ReviewTrack · unbestätigtes Mapping fällt sicher zurück"
            [ ReviewTrack.viewAt scale ComponentCatalogFixtures.reviewUnconfirmed ]

    let private reviewTrackDataVariants scale =
        ComponentCatalogFixtures.reviewTrackDataVariants
        |> List.map (fun fixture ->
            labeled
                scale
                fixture.Name
                (ReviewTrack.viewAt scale fixture.Model))
        |> section
            scale
            "ReviewTrack · kurze, lange, fehlende und fehlerhafte Daten"

    let view appZoomPercent fontZoomPercent scenarioId =
        let scale = DisplayScale.create appZoomPercent fontZoomPercent

        ScrollViewer.create
            [ ScrollViewer.horizontalScrollBarVisibility ScrollBarVisibility.Auto
              ScrollViewer.verticalScrollBarVisibility ScrollBarVisibility.Auto
              ScrollViewer.content (
                  Border.create
                      [ Border.margin (Thickness(DisplayScale.layout scale Spacing.xl))
                        Border.background (brush Colors.surface)
                        Border.borderBrush (brush Colors.borderStrong)
                        Border.borderThickness (
                            Thickness(DisplayScale.layout scale Lines.normal)
                        )
                        Border.cornerRadius (
                            CornerRadius(DisplayScale.layout scale CornerRadii.lg)
                        )
                        Border.padding (Thickness(DisplayScale.layout scale Spacing.xl))
                        Border.child (
                            match scenarioId with
                            | "TicketCard.AllStates" -> ticketCards scale
                            | "TicketCard.DataVariants" -> ticketCardDataVariants scale
                            | "CollapsedCell.AllStates" -> collapsedCells scale
                            | "CollapsedCell.DataVariants" ->
                                collapsedCellDataVariants scale
                            | "Board.SwimlaneHover" ->
                                section
                                    scale
                                    "Board.SwimlaneHover · genau ein Replaybutton"
                                    [ swimlaneHover scale ]
                            | "SwimlaneHeader.AllStates" -> swimlaneHeaders scale
                            | "SwimlaneHeader.DataVariants" ->
                                swimlaneHeaderDataVariants scale
                            | "Board.ReviewTrack.Ready" -> reviewReady scale
                            | "Board.ReviewTrack.CodeReview" -> reviewCodeReview scale
                            | "Board.ReviewTrack.Multiple" -> reviewMultiple scale
                            | "Board.ReviewTrack.DataVariants" ->
                                reviewTrackDataVariants scale
                            | "Board.ReviewTrack.InvalidMapping" -> reviewInvalid scale
                            | "Board.ReviewTrack.UnconfirmedMapping" ->
                                reviewUnconfirmed scale
                            | _ -> section scale "Unbekanntes Szenario" []
                        ) ]
              ) ]
