namespace JiraBoard.UiCatalog

open Avalonia
open Avalonia.Controls
open Avalonia.Controls.Primitives
open Avalonia.FuncUI.DSL
open Avalonia.FuncUI.Types
open Avalonia.Input
open Avalonia.Layout
open Avalonia.Media
open JiraBoard.Domain
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

    let private keyboardKey (eventArgs: KeyEventArgs) =
        match eventArgs.Key, eventArgs.KeyModifiers with
        | Key.Tab, KeyModifiers.Shift -> Some ShiftTab
        | Key.Tab, _ -> Some Tab
        | Key.Up, _ -> Some ArrowUp
        | Key.Down, _ -> Some ArrowDown
        | Key.Left, _ -> Some ArrowLeft
        | Key.Right, _ -> Some ArrowRight
        | Key.Space, _ -> Some Space
        | Key.Enter, _ -> Some Enter
        | Key.Escape, _ -> Some Escape
        | _ -> None

    let private interactiveSwimlaneHover scale keyboard dispatch =
        let focusedIssue = keyboard.FocusedIssueKey |> Option.defaultValue "Kein Fokus"
        let replayIssue = keyboard.ReplayIssueKey |> Option.defaultValue "Kein Replay"
        let modalIssue = keyboard.ModalIssueKey |> Option.defaultValue "Kein Modal"

        StackPanel.create
            [ StackPanel.spacing (DisplayScale.layout scale Spacing.sm)
              StackPanel.children
                  [ TextBlock.create
                        [ TextBlock.foreground (brush Colors.textSecondary)
                          TextBlock.text "Tab: Board betreten · Pfeile: Navigation · Leertaste: Replay · Enter: Modal · Escape: Abbruch" ]
                    Border.create
                        [ Border.focusable true
                          Border.isTabStop true
                          Border.onKeyDown (fun eventArgs ->
                              keyboardKey eventArgs
                              |> Option.iter (fun key -> dispatch (HandleKeyboard key)))
                          Border.child (swimlaneHover scale) ]
                    TextBlock.create
                        [ TextBlock.foreground (brush Colors.textSecondary)
                          TextBlock.text $"Fokus: {focusedIssue} · Replay: {replayIssue} · Modal: {modalIssue}" ] ] ]

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

    let private boardSurface scale boardWidth title model =
        section scale title [ BoardSurface.viewAt scale boardWidth model ]

    let private staticBoard scale boardWidth =
        section
            scale
            "Board · Parent modal, Standard-Issue-Swimlane und Subtasks nach Status"
            [ BoardProjection.viewAt scale boardWidth ComponentCatalogFixtures.staticBoard ]

    let private jiraOrderBoard scale boardWidth =
        section
            scale
            "Board · Jira-Reihenfolge über Pagination, gleiche/fehlende Ranks und Multi-Sprint"
            [ BoardProjection.viewAt scale boardWidth ComponentCatalogFixtures.jiraOrderBoard ]

    let private dragDropProbe scale title state =
        section scale title [ DragDropSpike.viewAt scale ComponentCatalogFixtures.boardSurface.Columns state ]

    let private navigationContextRestore scale =
        match ContextHeader.fromModel ComponentCatalogFixtures.navigationContextRestoreModel with
        | Some header ->
            section
                scale
                "Navigation · letzter Kontext öffnet ohne Modal"
                [ ContextHeader.viewAt scale header ]
        | None -> section scale "Navigation · letzter Kontext öffnet ohne Modal" []

    let private navigationProjectSelectionFirstStart scale =
        let model =
            ProjectSelectionModal.firstStart
                ComponentCatalogFixtures.navigationSiteId
                ComponentCatalogFixtures.navigationRowFor
                ComponentCatalogFixtures.navigationFirstStartModel.Data.Projects
                ignore
                ignore
                ignore
                ignore

        section scale "Navigation · Projektauswahl ohne gespeicherten Kontext" [ ProjectSelectionModal.viewAt scale model ]

    let private navigationProjectSelectionRestoreFailed scale =
        match ComponentCatalogFixtures.navigationRestoreFailedModel.State with
        | RestoreFailed(failedContext, projects) ->
            let model =
                ProjectSelectionModal.restoreFailed
                    ComponentCatalogFixtures.navigationRowFor
                    failedContext
                    projects
                    ignore
                    ignore
                    ignore
                    ignore

            section
                scale
                "Navigation · Projektauswahl nach fehlgeschlagener Wiederherstellung"
                [ ProjectSelectionModal.viewAt scale model ]
        | _ ->
            section scale "Navigation · Projektauswahl nach fehlgeschlagener Wiederherstellung" []

    let private navigationSprintMenu scale title model =
        section scale title [ SprintMenu.viewAt scale model ]

    let view appZoomPercent fontZoomPercent boardWidth animationProgress reducedMotion scenarioId keyboard dispatch =
        let scale = DisplayScale.create appZoomPercent fontZoomPercent
        let surface = ComponentCatalogFixtures.boardSurface

        let replay =
            { surface with
                Progress = animationProgress
                ReducedMotion = reducedMotion }

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
                                    [ interactiveSwimlaneHover scale keyboard dispatch ]
                            | "Board.StaticDomainProjection" -> staticBoard scale boardWidth
                            | "Board.JiraOrderProjection" -> jiraOrderBoard scale boardWidth
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
                            | "Board.Surface.SwimlaneReplay" ->
                                boardSurface scale boardWidth "BoardSurface · aktive Swimlane" replay
                            | "Board.Surface.SubtaskReplay" ->
                                boardSurface
                                    scale
                                    boardWidth
                                    "BoardSurface · einzelner Subtask"
                                    { replay with Replay = Some(SubtaskScope "APP-402") }
                            | "Board.Surface.Aborted" ->
                                boardSurface
                                    scale
                                    boardWidth
                                    "BoardSurface · Replay abgebrochen"
                                    { replay with Replay = None }
                            | "Board.Surface.ReducedMotion" ->
                                boardSurface
                                    scale
                                    boardWidth
                                    "BoardSurface · Reduced Motion"
                                    { replay with ReducedMotion = true }
                            | "DragDrop.Active" ->
                                ComponentCatalogFixtures.dragDropActive
                                |> dragDropProbe scale "Drag-and-drop · Ghost und gültiges Ziel"
                            | "DragDrop.ReducedMotion" ->
                                ComponentCatalogFixtures.dragDropReducedMotion
                                |> dragDropProbe scale "Drag-and-drop · Reduced Motion"
                            | "DragDrop.Rollback" ->
                                ComponentCatalogFixtures.dragDropRollback
                                |> dragDropProbe scale "Drag-and-drop · Abbruch und Fokus-Rückgabe"
                            | "Navigation.ContextRestore.Startup" -> navigationContextRestore scale
                            | "Navigation.ProjectSelection.FirstStart" ->
                                navigationProjectSelectionFirstStart scale
                            | "Navigation.ProjectSelection.RestoreFailed" ->
                                navigationProjectSelectionRestoreFailed scale
                            | "Navigation.SprintMenu.AllActive" ->
                                ComponentCatalogFixtures.navigationSprintMenuAllActive
                                |> navigationSprintMenu scale "Navigation · Sprint-Menü · alle aktiven Sprints"
                            | "Navigation.SprintMenu.Single" ->
                                ComponentCatalogFixtures.navigationSprintMenuSingle
                                |> navigationSprintMenu scale "Navigation · Sprint-Menü · ein aktiver Sprint"
                            | _ -> section scale "Unbekanntes Szenario" []
                        ) ]
              ) ]
