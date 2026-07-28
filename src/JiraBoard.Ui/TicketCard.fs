namespace JiraBoard.Ui

open System
open Avalonia
open Avalonia.Controls
open Avalonia.FuncUI.DSL
open Avalonia.Layout
open Avalonia.Media

type TicketCardState =
    | Normal
    | PointerHover
    | KeyboardFocus
    | Blocked
    | ReplayActive
    | Disabled

type TicketCardPriority =
    | Standard
    | High

type TicketCardModel =
    { AvailableWidth: float
      IssueKey: string
      Title: string
      Assignee: string option
      Priority: TicketCardPriority
      State: TicketCardState }

type TicketCardContract =
    { MinimumHeight: float
      HorizontalPadding: float
      VerticalPadding: float
      CornerRadius: float
      Background: JiraBoard.Ui.Color
      Foreground: JiraBoard.Ui.Color
      Border: JiraBoard.Ui.Color
      BorderThickness: float
      Signal: JiraBoard.Ui.Color option
      ColumnInset: float
      OuterFocusSpacing: float
      Shadow: Shadows.Shadow option }

[<RequireQualifiedAccess>]
module TicketCard =
    let private brush (color: JiraBoard.Ui.Color) =
        SolidColorBrush(Avalonia.Media.Color.Parse color.Hex) :> IBrush

    let private boxShadow (shadow: Shadows.Shadow) =
        let mutable value = BoxShadow()
        value.OffsetX <- shadow.OffsetX
        value.OffsetY <- shadow.OffsetY
        value.Blur <- shadow.Blur
        let baseColor = Avalonia.Media.Color.Parse shadow.Color.Hex

        let alpha =
            shadow.Opacity * 255.0
            |> Math.Round
            |> byte

        value.Color <-
            Avalonia.Media.Color.FromArgb(alpha, baseColor.R, baseColor.G, baseColor.B)

        value

    let contractAt scale state =
        let background, foreground, border, borderThickness, signal =
            match state with
            | Normal -> Colors.surface, Colors.textPrimary, Colors.border, Lines.normal, None
            | PointerHover ->
                Colors.surfaceHover, Colors.textPrimary, Colors.borderStrong, Lines.normal, None
            | KeyboardFocus -> Colors.surface, Colors.textPrimary, Colors.focus, Lines.focus, None
            | Blocked ->
                Colors.surface, Colors.textPrimary, Colors.border, Lines.normal, Some Colors.danger
            | ReplayActive ->
                Colors.surfaceSelected, Colors.textPrimary, Colors.primary, Lines.focus, Some Colors.primary
            | Disabled ->
                Colors.surfaceSubtle, Colors.textDisabled, Colors.border, Lines.normal, None

        { MinimumHeight = DisplayScale.layout scale ComponentMetrics.ticketCardMinimumHeight
          HorizontalPadding = DisplayScale.layout scale ComponentMetrics.ticketCardHorizontalPadding
          VerticalPadding = DisplayScale.layout scale ComponentMetrics.ticketCardVerticalPadding
          CornerRadius = DisplayScale.layout scale CornerRadii.card
          Background = background
          Foreground = foreground
          Border = border
          BorderThickness = DisplayScale.layout scale borderThickness
          Signal = signal
          ColumnInset = DisplayScale.layout scale ComponentMetrics.ticketCardColumnInset
          OuterFocusSpacing =
            if state = KeyboardFocus then
                DisplayScale.layout scale ComponentMetrics.ticketCardFocusOuterSpacing
            else
                0.0
          Shadow =
            if state = PointerHover then
                Some Shadows.hover
            elif state = Disabled then
                None
            else
                Some Shadows.card }

    let contract state = contractAt DisplayScale.normal state

    let trailingText model =
        let assignee = model.Assignee |> Option.defaultValue "Nicht zugewiesen"

        let stateSignal =
            match model.State with
            | Blocked -> [ "BLOCKIERT" ]
            | ReplayActive -> [ "REPLAY" ]
            | _ -> []

        let signals =
            if model.Priority = High then
                stateSignal @ [ "HOCH" ]
            else
                stateSignal

        match signals with
        | [] -> assignee
        | values ->
            let signalText = String.concat " · " values
            $"{assignee} · {signalText}"

    let signalColor model =
        match model.State, model.Priority with
        | Blocked, _ -> Some Colors.danger
        | ReplayActive, _ -> Some Colors.primary
        | _, High -> Some Colors.warning
        | _ -> None

    let displayTitle model =
        if String.IsNullOrWhiteSpace model.Title then
            "Titel nicht verfügbar"
        else
            model.Title

    let borderWidth scale model =
        let visual = contractAt scale model.State

        DisplayScale.layout scale model.AvailableWidth
        - visual.ColumnInset
        - (2.0 * visual.OuterFocusSpacing)

    let viewAt scale model =
        let visual = contractAt scale model.State

        Border.create
            [ Border.width (borderWidth scale model)
              Border.minHeight visual.MinimumHeight
              Border.margin (Thickness visual.OuterFocusSpacing)
              Border.background (brush visual.Background)
              Border.borderBrush (brush visual.Border)
              Border.borderThickness (Thickness visual.BorderThickness)
              Border.cornerRadius (CornerRadius visual.CornerRadius)
              Border.padding (
                  Thickness(visual.HorizontalPadding, visual.VerticalPadding)
              )
              Border.opacity (if model.State = Disabled then 0.72 else 1.0)
              match visual.Shadow with
              | Some shadow ->
                  Border.boxShadow (
                      boxShadow
                          { shadow with
                              OffsetX = DisplayScale.layout scale shadow.OffsetX
                              OffsetY = DisplayScale.layout scale shadow.OffsetY
                              Blur = DisplayScale.layout scale shadow.Blur }
                  )
              | None -> ()
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
                                            DisplayScale.layout scale Spacing.sm,
                                            0.0
                                        )
                                    )
                                    TextBlock.fontFamily (FontFamily Typography.issueKey.Family)
                                    TextBlock.fontSize (
                                        DisplayScale.font scale Typography.issueKey.Size
                                    )
                                    TextBlock.fontWeight (FontWeight.SemiBold)
                                    TextBlock.foreground (brush visual.Foreground)
                                    TextBlock.verticalAlignment VerticalAlignment.Center
                                    TextBlock.text model.IssueKey ]
                              TextBlock.create
                                  [ Grid.column 1
                                    TextBlock.fontFamily (FontFamily Typography.body.Family)
                                    TextBlock.fontSize (
                                        DisplayScale.font scale Typography.body.Size
                                    )
                                    TextBlock.foreground (brush visual.Foreground)
                                    TextBlock.textTrimming TextTrimming.CharacterEllipsis
                                    TextBlock.verticalAlignment VerticalAlignment.Center
                                    TextBlock.text (displayTitle model) ]
                              TextBlock.create
                                  [ Grid.column 2
                                    TextBlock.margin (
                                        Thickness(
                                            DisplayScale.layout scale Spacing.sm,
                                            0.0,
                                            0.0,
                                            0.0
                                        )
                                    )
                                    TextBlock.fontFamily (FontFamily Typography.caption.Family)
                                    TextBlock.fontSize (
                                        DisplayScale.font scale Typography.caption.Size
                                    )
                                    TextBlock.foreground (
                                        signalColor model
                                        |> Option.defaultValue visual.Foreground
                                        |> brush
                                    )
                                    TextBlock.verticalAlignment VerticalAlignment.Center
                                    TextBlock.text (trailingText model) ] ] ]
              ) ]

    let view model = viewAt DisplayScale.normal model
