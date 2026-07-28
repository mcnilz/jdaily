namespace JiraBoard.Ui

open System
open Avalonia
open Avalonia.Automation
open Avalonia.Controls
open Avalonia.FuncUI.DSL
open Avalonia.Layout
open Avalonia.Media

type CollapsedColumnCellState =
    | Assigned
    | Unassigned
    | AvatarFailed
    | HighPriority
    | Flagged
    | Blocked
    | PointerHover
    | KeyboardFocus
    | ReplayActive

type CollapsedColumnCellModel =
    { IssueKey: string
      Title: string
      Assignee: string option
      State: CollapsedColumnCellState }

type CollapsedColumnCellContract =
    { Width: float
      MinimumHeight: float
      AvatarSize: float
      Background: JiraBoard.Ui.Color
      Border: JiraBoard.Ui.Color
      BorderThickness: float
      Signal: JiraBoard.Ui.Color option
      FlagColor: JiraBoard.Ui.Color option
      IsInteractive: bool }

[<RequireQualifiedAccess>]
module CollapsedColumnCell =
    let private brush (color: JiraBoard.Ui.Color) =
        SolidColorBrush(Avalonia.Media.Color.Parse color.Hex) :> IBrush

    let accessibleName model =
        let assignee = model.Assignee |> Option.defaultValue "Nicht zugewiesen"

        let warning =
            match model.State with
            | HighPriority -> "Hohe Priorität"
            | Flagged -> "Markiert"
            | Blocked -> "Blockiert"
            | AvatarFailed -> "Avatar nicht verfügbar"
            | ReplayActive -> "Replay aktiv"
            | _ -> "Kein Warnzustand"

        $"{model.IssueKey} · {model.Title} · {assignee} · {warning}"

    let contractAt scale state =
        let background, border, borderThickness, signal, flagColor =
            match state with
            | Assigned
            | Unassigned
            | AvatarFailed -> Colors.surface, Colors.border, Lines.normal, None, None
            | HighPriority ->
                Colors.surface, Colors.border, Lines.normal, Some Colors.warning, None
            | Flagged ->
                Colors.surface, Colors.border, Lines.normal, None, Some Colors.danger
            | Blocked ->
                Colors.surface, Colors.border, Lines.normal, Some Colors.danger, None
            | PointerHover ->
                Colors.surfaceHover, Colors.borderStrong, Lines.normal, None, None
            | KeyboardFocus -> Colors.surface, Colors.focus, Lines.focus, None, None
            | ReplayActive ->
                Colors.surfaceSelected,
                Colors.primary,
                Lines.focus,
                Some Colors.primary,
                None

        { Width = DisplayScale.layout scale ComponentMetrics.collapsedCellWidth
          MinimumHeight = DisplayScale.layout scale ComponentMetrics.collapsedCellMinimumHeight
          AvatarSize = DisplayScale.layout scale ComponentMetrics.collapsedCellAvatarSize
          Background = background
          Border = border
          BorderThickness = DisplayScale.layout scale borderThickness
          Signal = signal
          FlagColor = flagColor
          IsInteractive = true }

    let contract state = contractAt DisplayScale.normal state

    let initials (assignee: string option) =
        match assignee with
        | None -> "–"
        | Some value ->
            let names =
                value.Split(' ', StringSplitOptions.RemoveEmptyEntries)

            match names with
            | [||] -> "–"
            | [| name |] -> name.Substring(0, 1).ToUpperInvariant()
            | _ ->
                let first = names[0].Substring(0, 1)
                let last = names[names.Length - 1].Substring(0, 1)
                (first + last).ToUpperInvariant()

    let private avatarText model =
        match model.State with
        | AvatarFailed -> "!"
        | Unassigned -> "–"
        | _ -> initials model.Assignee

    let viewAt scale model =
        let visual = contractAt scale model.State
        let accessibleDescription = accessibleName model

        Button.create
            [ Button.width visual.Width
              Button.minHeight visual.MinimumHeight
              Button.padding (Thickness 0.0)
              Button.background (brush visual.Background)
              Button.borderBrush (brush visual.Border)
              Button.borderThickness (Thickness visual.BorderThickness)
              Button.cornerRadius (
                  CornerRadius(DisplayScale.layout scale CornerRadii.card)
              )
              Button.tip accessibleDescription
              Accessibility.name accessibleDescription
              Button.content (
                  Grid.create
                      [ Grid.children
                            [ Border.create
                                  [ Border.width visual.AvatarSize
                                    Border.height visual.AvatarSize
                                    Border.horizontalAlignment HorizontalAlignment.Center
                                    Border.verticalAlignment VerticalAlignment.Center
                                    Border.background (brush Colors.surfaceSubtle)
                                    Border.borderBrush (brush Colors.borderStrong)
                                    Border.borderThickness (
                                        Thickness(DisplayScale.layout scale Lines.normal)
                                    )
                                    Border.cornerRadius (CornerRadius(visual.AvatarSize / 2.0))
                                    Border.child (
                                        TextBlock.create
                                            [ TextBlock.horizontalAlignment HorizontalAlignment.Center
                                              TextBlock.verticalAlignment VerticalAlignment.Center
                                              TextBlock.fontFamily (
                                                  FontFamily Typography.bodyStrong.Family
                                              )
                                              TextBlock.fontSize (
                                                  DisplayScale.font scale Typography.bodyStrong.Size
                                              )
                                              TextBlock.fontWeight (FontWeight.SemiBold)
                                              TextBlock.foreground (brush Colors.textPrimary)
                                              TextBlock.text (avatarText model) ]
                                    ) ]
                              match visual.Signal with
                              | Some signal ->
                                  Border.create
                                      [ Border.width (DisplayScale.layout scale Spacing.md)
                                        Border.height (DisplayScale.layout scale Spacing.md)
                                        Border.horizontalAlignment HorizontalAlignment.Right
                                        Border.verticalAlignment VerticalAlignment.Top
                                        Border.margin (
                                            Thickness(DisplayScale.layout scale Spacing.xxs)
                                        )
                                        Border.background (brush signal)
                                        Border.cornerRadius (
                                            CornerRadius(
                                                DisplayScale.layout scale CornerRadii.sm
                                            )
                                        ) ]
                              | None -> ()
                              match visual.FlagColor with
                              | Some flagColor ->
                                  TextBlock.create
                                      [ TextBlock.horizontalAlignment HorizontalAlignment.Right
                                        TextBlock.verticalAlignment VerticalAlignment.Top
                                        TextBlock.margin (
                                            Thickness(DisplayScale.layout scale Spacing.xxs)
                                        )
                                        TextBlock.fontFamily (
                                            FontFamily Typography.compact.Family
                                        )
                                        TextBlock.fontSize (
                                            DisplayScale.font scale Typography.compact.Size
                                        )
                                        TextBlock.fontWeight (FontWeight.SemiBold)
                                        TextBlock.foreground (brush flagColor)
                                        TextBlock.text "⚑" ]
                              | None -> () ] ]
              ) ]

    let view model = viewAt DisplayScale.normal model
