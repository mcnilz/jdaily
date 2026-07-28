namespace JiraBoard.Ui

open Avalonia
open Avalonia.Controls
open Avalonia.Controls.Primitives
open Avalonia.FuncUI.DSL
open Avalonia.Layout
open Avalonia.Media

type SwimlaneHeaderState =
    | Normal
    | PointerHover
    | KeyboardFocus
    | ReplayActive

type SwimlaneHeaderModel =
    { IssueKey: string
      Title: string
      Metadata: string option
      State: SwimlaneHeaderState
      OnReplayRequested: unit -> unit }

type SwimlaneHeaderContract =
    { MinimumHeight: float
      HorizontalPadding: float
      Background: JiraBoard.Ui.Color
      Border: JiraBoard.Ui.Color
      BorderThickness: float
      ReplayButtonVisible: bool
      ReplayActionName: string option }

[<RequireQualifiedAccess>]
module SwimlaneHeader =
    let private brush (color: JiraBoard.Ui.Color) =
        SolidColorBrush(Avalonia.Media.Color.Parse color.Hex) :> IBrush

    let contractAt scale state =
        let background, border, borderThickness, replayButtonVisible =
            match state with
            | Normal -> Colors.surface, Colors.border, Lines.normal, false
            | PointerHover ->
                Colors.surfaceSelected, Colors.primary, Lines.focus, true
            | KeyboardFocus ->
                Colors.surfaceSelected, Colors.focus, Lines.focus, true
            | ReplayActive ->
                Colors.surfaceSelected, Colors.primary, Lines.focus, true

        { MinimumHeight = DisplayScale.layout scale ComponentMetrics.swimlaneHeaderMinimumHeight
          HorizontalPadding =
            DisplayScale.layout scale ComponentMetrics.swimlaneHeaderHorizontalPadding
          Background = background
          Border = border
          BorderThickness = DisplayScale.layout scale borderThickness
          ReplayButtonVisible = replayButtonVisible
          ReplayActionName =
            if not replayButtonVisible then
                None
            elif state = ReplayActive then
                Some "Replay stoppen"
            else
                Some "Änderungen abspielen" }

    let contract state = contractAt DisplayScale.normal state

    let viewAt scale model =
        let visual = contractAt scale model.State

        Border.create
            [ Border.minHeight visual.MinimumHeight
              Border.background (brush visual.Background)
              Border.borderBrush (brush visual.Border)
              Border.borderThickness (Thickness visual.BorderThickness)
              Border.cornerRadius (
                  CornerRadius(DisplayScale.layout scale CornerRadii.md)
              )
              Border.padding (
                  Thickness(
                      visual.HorizontalPadding,
                      DisplayScale.layout scale Spacing.sm
                  )
              )
              Border.child (
                  Grid.create
                      [ Grid.columnDefinitions "Auto,*,Auto"
                        Grid.children
                            [ TextBlock.create
                                  [ Grid.column 0
                                    TextBlock.margin (
                                        Thickness(
                                            0.0,
                                            0.0,
                                            DisplayScale.layout scale Spacing.md,
                                            0.0
                                        )
                                    )
                                    TextBlock.fontFamily (FontFamily Typography.issueKey.Family)
                                    TextBlock.fontSize (
                                        DisplayScale.font scale Typography.issueKey.Size
                                    )
                                    TextBlock.fontWeight (FontWeight.SemiBold)
                                    TextBlock.foreground (brush Colors.textPrimary)
                                    TextBlock.verticalAlignment VerticalAlignment.Center
                                    TextBlock.text model.IssueKey ]
                              StackPanel.create
                                  [ Grid.column 1
                                    StackPanel.verticalAlignment VerticalAlignment.Center
                                    StackPanel.children
                                        [ TextBlock.create
                                              [ TextBlock.fontFamily (
                                                    FontFamily Typography.swimlaneTitle.Family
                                                )
                                                TextBlock.fontSize (
                                                    DisplayScale.font
                                                        scale
                                                        Typography.swimlaneTitle.Size
                                                )
                                                TextBlock.fontWeight (FontWeight.SemiBold)
                                                TextBlock.foreground (brush Colors.textPrimary)
                                                TextBlock.textWrapping TextWrapping.Wrap
                                                TextBlock.maxLines 2
                                                TextBlock.text model.Title ]
                                          match model.Metadata with
                                          | Some metadata ->
                                              TextBlock.create
                                                  [ TextBlock.fontFamily (
                                                        FontFamily Typography.caption.Family
                                                    )
                                                    TextBlock.fontSize (
                                                        DisplayScale.font
                                                            scale
                                                            Typography.caption.Size
                                                    )
                                                    TextBlock.foreground (
                                                        brush Colors.textSecondary
                                                    )
                                                    TextBlock.text metadata ]
                                          | None -> () ] ]
                              if visual.ReplayButtonVisible then
                                  ToggleButton.create
                                      [ Grid.column 2
                                        ToggleButton.width (
                                            DisplayScale.layout
                                                scale
                                                ComponentMetrics.replayLoopButtonSize
                                        )
                                        ToggleButton.height (
                                            DisplayScale.layout
                                                scale
                                                ComponentMetrics.replayLoopButtonSize
                                        )
                                        ToggleButton.margin (
                                            Thickness(
                                                DisplayScale.layout scale Spacing.md,
                                                0.0,
                                                0.0,
                                                0.0
                                            )
                                        )
                                        ToggleButton.padding (Thickness 0.0)
                                        ToggleButton.background (
                                            if model.State = ReplayActive then
                                                brush Colors.primary
                                            else
                                                brush Colors.surface
                                        )
                                        ToggleButton.foreground (
                                            if model.State = ReplayActive then
                                                brush Colors.surface
                                            else
                                                brush Colors.primary
                                        )
                                        ToggleButton.isChecked (model.State = ReplayActive)
                                        ToggleButton.fontFamily (
                                            FontFamily Typography.body.Family
                                        )
                                        ToggleButton.fontSize (
                                            DisplayScale.font scale Typography.body.Size
                                        )
                                        ToggleButton.tip visual.ReplayActionName.Value
                                        Accessibility.name visual.ReplayActionName.Value
                                        ToggleButton.content "↻"
                                        ToggleButton.onClick (
                                            fun _ -> model.OnReplayRequested()
                                        ) ] ] ]
              ) ]

    let view model = viewAt DisplayScale.normal model
