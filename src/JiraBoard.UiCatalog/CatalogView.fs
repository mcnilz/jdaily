namespace JiraBoard.UiCatalog

open Avalonia
open Avalonia.Controls
open Avalonia.Controls.Primitives
open Avalonia.FuncUI
open Avalonia.FuncUI.DSL
open Avalonia.Layout
open Avalonia.Media
open JiraBoard.Ui

[<RequireQualifiedAccess>]
module CatalogView =
    let overviewDescription =
        "Produktionskomponenten und ihre Pflichtzustände sind als auswählbare Szenarien verfügbar."

    let private brush color =
        SolidColorBrush(Avalonia.Media.Color.Parse color.Hex) :> IBrush

    let private motionName preset =
        match preset with
        | Motion.Calm -> "Ruhig"
        | Motion.Normal -> "Normal"
        | Motion.Fast -> "Schnell"

    let private controlButton (label: string) message dispatch =
        Button.create
            [ Button.minHeight HitTarget.minimum
              Button.padding (Thickness(Spacing.md, Spacing.sm))
              Button.content label
              Button.onClick (fun _ -> dispatch message) ]

    let private menu =
        Menu.create
            [ Menu.dock Dock.Top
              Menu.height CatalogShell.layout.MenuHeight
              Menu.viewItems
                  [ MenuItem.create [ MenuItem.header "Datei" ]
                    MenuItem.create [ MenuItem.header "Projekt" ]
                    MenuItem.create [ MenuItem.header "Sprint" ]
                    MenuItem.create [ MenuItem.header "Board" ]
                    MenuItem.create [ MenuItem.header "Daily" ]
                    MenuItem.create [ MenuItem.header "Ansicht" ]
                    MenuItem.create [ MenuItem.header "Hilfe" ] ] ]

    let private controlBar state dispatch =
        Border.create
            [ Border.dock Dock.Top
              Border.minHeight CatalogShell.layout.ControlBarMinimumHeight
              Border.background (brush Colors.surfaceSubtle)
              Border.borderBrush (brush Colors.border)
              Border.borderThickness (Thickness(0.0, 0.0, 0.0, Lines.normal))
              Border.padding (Thickness(Spacing.sm))
              Border.child (
                  WrapPanel.create
                      [ WrapPanel.orientation Orientation.Horizontal
                        WrapPanel.children
                            [ controlButton
                                  $"Viewport · {state.Viewport.Name}"
                                  CycleViewport
                                  dispatch
                              controlButton
                                  $"App-Zoom · {state.AppZoomPercent} %%"
                                  CycleAppZoom
                                  dispatch
                              controlButton
                                  $"Schrift · {state.FontZoomPercent} %%"
                                  CycleFontZoom
                                  dispatch
                              controlButton
                                  $"Motion · {motionName state.MotionPreset}"
                                  CycleMotionPreset
                                  dispatch
                              CheckBox.create
                                  [ CheckBox.minHeight HitTarget.minimum
                                    CheckBox.margin (Thickness(Spacing.sm, 0.0))
                                    CheckBox.verticalAlignment VerticalAlignment.Center
                                    CheckBox.content "Reduced Motion"
                                    CheckBox.isChecked state.ReducedMotion
                                    CheckBox.onClick (fun _ -> dispatch ToggleReducedMotion) ]
                              TextBlock.create
                                  [ TextBlock.margin (Thickness(Spacing.md, 0.0, Spacing.xs, 0.0))
                                    TextBlock.verticalAlignment VerticalAlignment.Center
                                    TextBlock.text "Fortschritt" ]
                              for progress in CatalogShell.animationProgressStops do
                                  controlButton
                                      $"{progress * 100.0:F0} %%"
                                      (SetAnimationProgress progress)
                                      dispatch ] ]) ]

    let private scenarioNavigation state dispatch =
        Border.create
            [ Border.dock Dock.Left
              Border.width CatalogShell.layout.ScenarioNavigationWidth
              Border.background (brush Colors.surface)
              Border.borderBrush (brush Colors.border)
              Border.borderThickness (Thickness(0.0, 0.0, Lines.normal, 0.0))
              Border.padding (Thickness(Spacing.lg))
              Border.child (
                  ScrollViewer.create
                      [ ScrollViewer.verticalScrollBarVisibility ScrollBarVisibility.Auto
                        ScrollViewer.horizontalScrollBarVisibility ScrollBarVisibility.Disabled
                        ScrollViewer.content (
                            StackPanel.create
                                [ StackPanel.spacing Spacing.md
                                  StackPanel.children
                                      [ TextBlock.create
                                            [ TextBlock.fontSize Typography.componentTitle.Size
                                              TextBlock.fontWeight (FontWeight.SemiBold)
                                              TextBlock.foreground (brush Colors.textPrimary)
                                              TextBlock.text "Szenarien" ]
                                        for scenario in CatalogScenarios.all do
                                            Button.create
                                                [ Button.horizontalContentAlignment HorizontalAlignment.Stretch
                                                  Button.background (
                                                      if scenario.Id = state.SelectedScenarioId then
                                                          brush Colors.surfaceSelected
                                                      else
                                                          brush Colors.surface
                                                  )
                                                  Button.padding (Thickness(Spacing.md))
                                                  Button.onClick (
                                                      fun _ ->
                                                          dispatch (SelectScenario scenario.Id)
                                                  )
                                                  Button.content (
                                                      StackPanel.create
                                                          [ StackPanel.spacing Spacing.xs
                                                            StackPanel.children
                                                                [ TextBlock.create
                                                                      [ TextBlock.fontWeight (FontWeight.SemiBold)
                                                                        TextBlock.textWrapping (
                                                                            if CatalogShell.layout.WrapScenarioLabels then
                                                                                TextWrapping.Wrap
                                                                            else
                                                                                TextWrapping.NoWrap
                                                                        )
                                                                        TextBlock.text scenario.Name ]
                                                                  TextBlock.create
                                                                      [ TextBlock.fontSize Typography.caption.Size
                                                                        TextBlock.foreground (brush Colors.textSecondary)
                                                                        TextBlock.textWrapping (
                                                                            if CatalogShell.layout.WrapScenarioLabels then
                                                                                TextWrapping.Wrap
                                                                            else
                                                                                TextWrapping.NoWrap
                                                                        )
                                                                        TextBlock.text scenario.Id ] ] ]
                                                  ) ] ] ]
                        ) ]
              ) ]

    let private preview state =
        let effectiveFontSize =
            Typography.body.Size
            * (float state.AppZoomPercent / 100.0)
            * (float state.FontZoomPercent / 100.0)

        Border.create
            [ Border.margin (Thickness(Spacing.xl))
              Border.background (brush Colors.surface)
              Border.borderBrush (brush Colors.borderStrong)
              Border.borderThickness (Thickness(Lines.normal))
              Border.cornerRadius (CornerRadius(CornerRadii.lg))
              Border.padding (Thickness(Spacing.xxl))
              Border.child (
                  StackPanel.create
                      [ StackPanel.spacing Spacing.lg
                        StackPanel.children
                            [ TextBlock.create
                                  [ TextBlock.fontSize Typography.boardTitle.Size
                                    TextBlock.fontWeight (FontWeight.SemiBold)
                                    TextBlock.foreground (brush Colors.textPrimary)
                                    TextBlock.text "JiraBoard UI Catalog" ]
                              TextBlock.create
                                  [ TextBlock.fontSize effectiveFontSize
                                    TextBlock.foreground (brush Colors.textSecondary)
                                    TextBlock.text overviewDescription ]
                              Border.create
                                  [ Border.background (brush Colors.canvas)
                                    Border.borderBrush (brush Colors.border)
                                    Border.borderThickness (Thickness(Lines.normal))
                                    Border.cornerRadius (CornerRadius(CornerRadii.md))
                                    Border.padding (Thickness(Spacing.xl))
                                    Border.child (
                                        StackPanel.create
                                            [ StackPanel.spacing Spacing.sm
                                              StackPanel.children
                                                  [ TextBlock.create
                                                        [ TextBlock.fontWeight (FontWeight.SemiBold)
                                                          TextBlock.text state.Viewport.Name ]
                                                    TextBlock.create
                                                        [ TextBlock.text (
                                                              $"App-Zoom {state.AppZoomPercent} %% · Schrift {state.FontZoomPercent} %%"
                                                          ) ]
                                                    TextBlock.create
                                                        [ TextBlock.text (
                                                              $"Motion {motionName state.MotionPreset} · Reduced Motion {state.ReducedMotion}"
                                                          ) ]
                                                    TextBlock.create
                                                        [ TextBlock.foreground (brush Colors.primary)
                                                          TextBlock.text (
                                                              $"Animationsfortschritt {state.AnimationProgress * 100.0:F0} %%"
                                                          ) ] ] ]
                                    ) ] ] ]) ]

    let view state dispatch =
        DockPanel.create
            [ DockPanel.background (brush Colors.canvas)
              DockPanel.lastChildFill true
              DockPanel.children
                  [ menu
                    controlBar state dispatch
                    scenarioNavigation state dispatch
                    if state.SelectedScenarioId = "Shell.Overview" then
                        preview state
                    else
                        ComponentCatalogView.view
                            state.AppZoomPercent
                            state.FontZoomPercent
                            state.SelectedScenarioId ] ]
