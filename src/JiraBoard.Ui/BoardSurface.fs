namespace JiraBoard.Ui

open System
open System.Numerics
open Avalonia
open Avalonia.Controls
open Avalonia.FuncUI.DSL
open Avalonia.FuncUI.Types
open Avalonia.Layout
open Avalonia.Media
open Avalonia.Rendering.Composition

type BoardSurfaceCard =
    { IssueKey: string
      SwimlaneKey: string
      Column: string }

type BoardSurfaceReplayScope =
    | SwimlaneScope of string
    | SubtaskScope of string

/// A deterministic, externally controlled replay interval. The UiCatalog
/// selects its progress directly, so visual tests never depend on timers.
type BoardSurfaceKeyframe =
    { IssueKey: string
      StartProgress: float
      EndProgress: float
      Offset: float }

type BoardSurfaceModel =
    { Columns: string list
      Cards: BoardSurfaceCard list
      Replay: BoardSurfaceReplayScope option
      Progress: float
      Keyframes: BoardSurfaceKeyframe list
      ReducedMotion: bool }

type BoardSurfaceProjectedCard =
    { IssueKey: string
      IsReplayActive: bool
      Offset: float }

type BoardSurfaceProjection =
    { Columns: string list
      Cards: BoardSurfaceProjectedCard list
      ActiveKeyframe: BoardSurfaceKeyframe option }

[<RequireQualifiedAccess>]
module BoardSurface =
    let private isInScope (scope: BoardSurfaceReplayScope) (card: BoardSurfaceCard) =
        match scope with
        | SwimlaneScope swimlaneKey -> card.SwimlaneKey = swimlaneKey
        | SubtaskScope issueKey -> card.IssueKey = issueKey

    let private activeKeyframe progress (keyframes: BoardSurfaceKeyframe list) =
        keyframes
        |> List.tryFind (fun keyframe ->
            progress >= keyframe.StartProgress && progress <= keyframe.EndProgress)

    let private keyframeOffset startOffset progress (keyframe: BoardSurfaceKeyframe) =
        let duration = keyframe.EndProgress - keyframe.StartProgress

        if duration <= 0.0 then
            keyframe.Offset
        else
            let completed = (progress - keyframe.StartProgress) / duration
            startOffset + (keyframe.Offset - startOffset) * max 0.0 (min 1.0 completed)

    let private completedOffset issueKey progress (keyframes: BoardSurfaceKeyframe list) =
        keyframes
        |> List.filter (fun keyframe -> keyframe.IssueKey = issueKey && keyframe.EndProgress <= progress)
        |> List.sortBy _.EndProgress
        |> List.tryLast
        |> Option.map _.Offset
        |> Option.defaultValue 0.0

    let private keyframeStartOffset (keyframe: BoardSurfaceKeyframe) keyframes =
        completedOffset keyframe.IssueKey keyframe.StartProgress keyframes

    let project (model: BoardSurfaceModel): BoardSurfaceProjection =
        let activeKeyframe = activeKeyframe model.Progress model.Keyframes

        { Columns = model.Columns
          ActiveKeyframe = activeKeyframe
          Cards =
            model.Cards
            |> List.map (fun card ->
                let isReplayActive = model.Replay |> Option.exists (fun scope -> isInScope scope card)

                let offset =
                    match activeKeyframe with
                    | Some keyframe when keyframe.IssueKey = card.IssueKey && not model.ReducedMotion ->
                        keyframeOffset
                            (keyframeStartOffset keyframe model.Keyframes)
                            model.Progress
                            keyframe
                    | _ -> completedOffset card.IssueKey model.Progress model.Keyframes

                { IssueKey = card.IssueKey
                  IsReplayActive = isReplayActive
                  Offset = offset }) }

    let startOffsetAnimation (visual: Visual) offset duration =
        let compositionVisual = ElementComposition.GetElementVisual visual
        let animation = compositionVisual.Compositor.CreateVector3KeyFrameAnimation()

        animation.InsertKeyFrame(1.0f, Vector3(float32 offset, 0.0f, 0.0f))
        animation.Duration <- duration
        compositionVisual.StartAnimation("Offset", animation)

    let private translated offset =
        let transform = TranslateTransform()
        transform.X <- offset
        transform :> ITransform

    let private cardView scale columnWidth (projected: BoardSurfaceProjectedCard) (card: BoardSurfaceCard): IView =
        let state =
            if projected.IsReplayActive then
                TicketCardState.ReplayActive
            else
                TicketCardState.Normal

        let model =
            { AvailableWidth = columnWidth
              IssueKey = card.IssueKey
              Title = $"Replay-Karte in {card.Column}"
              Assignee = None
              Priority = TicketCardPriority.Standard
              State = state }

        Border.create
            [ Border.renderTransform (translated projected.Offset)
              Border.child (TicketCard.viewAt scale model) ]

    let viewAt scale boardWidth (model: BoardSurfaceModel): IView =
        let metrics =
            BoardLayout.calculate
                { BoardWidth = boardWidth
                  NormalColumnCount = model.Columns.Length
                  CollapsedColumnCount = 0
                  IncludesReviewTrack = false }

        let projection = project model

        Grid.create
            [ Grid.columnDefinitions (String.replicate model.Columns.Length "*," |> fun value -> value.TrimEnd(','))
              Grid.children
                  [ for index, column in model.Columns |> List.indexed do
                        let cards: IView list =
                            model.Cards
                            |> List.filter (fun card -> card.Column = column)
                            |> List.map (fun card ->
                                let projected =
                                    projection.Cards
                                    |> List.find (fun candidate -> candidate.IssueKey = card.IssueKey)

                                cardView
                                    scale
                                    metrics.NormalColumnWidth
                                    { projected with
                                        Offset = projected.Offset * metrics.NormalColumnWidth }
                                    card)

                        StackPanel.create
                            [ Grid.column index
                              StackPanel.spacing (DisplayScale.layout scale Spacing.sm)
                              StackPanel.children
                                  [ TextBlock.create
                                        [ TextBlock.fontFamily (FontFamily Typography.componentTitle.Family)
                                          TextBlock.fontSize (
                                              DisplayScale.font scale Typography.componentTitle.Size
                                          )
                                          TextBlock.fontWeight FontWeight.SemiBold
                                          TextBlock.text column ]
                                    yield! cards ] ] ] ]
