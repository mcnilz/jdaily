namespace JiraBoard.UiCatalog

open JiraBoard.Ui

type ViewportPreset =
    { Name: string
      Width: int
      Height: int }

type CatalogScenario =
    { Id: string
      Name: string
      Area: string }

type CatalogShellLayout =
    { MenuHeight: float
      ControlBarMinimumHeight: float
      ScenarioNavigationWidth: float
      InitialWindowWidth: float
      InitialWindowHeight: float
      MinimumWindowWidth: float
      MinimumWindowHeight: float
      ScenarioNavigationScrollable: bool
      WrapScenarioLabels: bool
      CollapsedStatePreviewWidth: float }

[<RequireQualifiedAccess>]
module CatalogScenarios =
    let all =
        [ { Id = "Shell.Overview"
            Name = "Katalogübersicht"
            Area = "Shell" }
          { Id = "TicketCard.AllStates"
            Name = "TicketCard · alle Zustände"
            Area = "TicketCard" }
          { Id = "TicketCard.DataVariants"
            Name = "TicketCard · Datenvarianten"
            Area = "TicketCard" }
          { Id = "CollapsedCell.AllStates"
            Name = "CollapsedColumnCell · alle Zustände"
            Area = "CollapsedColumnCell" }
          { Id = "CollapsedCell.DataVariants"
            Name = "CollapsedColumnCell · Datenvarianten"
            Area = "CollapsedColumnCell" }
          { Id = "Board.SwimlaneHover"
            Name = "SwimlaneHeader · Zustände"
            Area = "SwimlaneHeader" }
          { Id = "SwimlaneHeader.AllStates"
            Name = "SwimlaneHeader · alle Zustände"
            Area = "SwimlaneHeader" }
          { Id = "SwimlaneHeader.DataVariants"
            Name = "SwimlaneHeader · Datenvarianten"
            Area = "SwimlaneHeader" }
          { Id = "Board.ReviewTrack.Ready"
            Name = "ReviewTrack · Ready for CR"
            Area = "ReviewTrack" }
          { Id = "Board.ReviewTrack.CodeReview"
            Name = "ReviewTrack · Code Review"
            Area = "ReviewTrack" }
          { Id = "Board.ReviewTrack.Multiple"
            Name = "ReviewTrack · mehrere Karten"
            Area = "ReviewTrack" }
          { Id = "Board.ReviewTrack.DataVariants"
            Name = "ReviewTrack · Datenvarianten"
            Area = "ReviewTrack" }
          { Id = "Board.ReviewTrack.InvalidMapping"
            Name = "ReviewTrack · ungültiges Mapping"
            Area = "ReviewTrack" }
          { Id = "Board.ReviewTrack.UnconfirmedMapping"
            Name = "ReviewTrack · unbestätigtes Mapping"
            Area = "ReviewTrack" }
          { Id = "Board.Surface.SwimlaneReplay"
            Name = "BoardSurface · Swimlane-Replay"
            Area = "BoardSurface" }
          { Id = "Board.Surface.SubtaskReplay"
            Name = "BoardSurface · Subtask-Replay"
            Area = "BoardSurface" }
          { Id = "Board.Surface.Aborted"
            Name = "BoardSurface · Abbruch"
            Area = "BoardSurface" }
          { Id = "Board.Surface.ReducedMotion"
            Name = "BoardSurface · Reduced Motion"
            Area = "BoardSurface" }
          { Id = "DragDrop.Active"
            Name = "Drag-and-drop · Ghost und Ziel"
            Area = "Drag-and-drop" }
          { Id = "DragDrop.ReducedMotion"
            Name = "Drag-and-drop · Reduced Motion"
            Area = "Drag-and-drop" }
          { Id = "DragDrop.Rollback"
            Name = "Drag-and-drop · Abbruch"
            Area = "Drag-and-drop" } ]

type CatalogShellState =
    { Viewport: ViewportPreset
      AppZoomPercent: int
      FontZoomPercent: int
      MotionPreset: Motion.SpeedPreset
      ReducedMotion: bool
      AnimationProgress: float
      SelectedScenarioId: string
      Keyboard: CatalogKeyboardState }

type CatalogShellMessage =
    | SelectScenario of string
    | SelectViewport of ViewportPreset
    | SelectAppZoom of int
    | SelectFontZoom of int
    | SelectMotionPreset of Motion.SpeedPreset
    | CycleViewport
    | CycleAppZoom
    | CycleFontZoom
    | CycleMotionPreset
    | SetReducedMotion of bool
    | ToggleReducedMotion
    | SetAnimationProgress of float
    | HandleKeyboard of CatalogKeyboardKey

[<RequireQualifiedAccess>]
module CatalogShell =
    let viewportPresets =
        [ { Name = "1920 × 1080"
            Width = 1920
            Height = 1080 }
          { Name = "2560 × 1440"
            Width = 2560
            Height = 1440 }
          { Name = "3440 × 1440"
            Width = 3440
            Height = 1440 }
          { Name = "3840 × 2160"
            Width = 3840
            Height = 2160 } ]

    let appZoomLevels = [ 75; 90; 100; 110; 125; 150; 175; 200 ]
    let fontZoomLevels = [ 80; 90; 100; 110; 125; 150; 175; 200 ]
    let motionPresets = [ Motion.Calm; Motion.Normal; Motion.Fast ]
    let animationProgressStops = [ 0.0; 0.25; 0.50; 0.75; 1.0 ]

    let layout =
        { MenuHeight = 32.0
          ControlBarMinimumHeight = 48.0
          ScenarioNavigationWidth = 248.0
          InitialWindowWidth = 1280.0
          InitialWindowHeight = 800.0
          MinimumWindowWidth = 1024.0
          MinimumWindowHeight = 640.0
          ScenarioNavigationScrollable = true
          WrapScenarioLabels = true
          CollapsedStatePreviewWidth = 128.0 }

    let private nextItem current items =
        let index = items |> List.findIndex ((=) current)
        items |> List.item ((index + 1) % List.length items)

    let init () =
        { Viewport = List.head viewportPresets
          AppZoomPercent = 100
          FontZoomPercent = 100
          MotionPreset = Motion.Normal
          ReducedMotion = false
          AnimationProgress = 0.0
          SelectedScenarioId = CatalogScenarios.all.Head.Id
          Keyboard = CatalogKeyboard.init CatalogKeyboard.boardTargets }

    let update message state =
        match message with
        | SelectScenario scenarioId ->
            { state with
                SelectedScenarioId = scenarioId }
        | SelectViewport viewport -> { state with Viewport = viewport }
        | SelectAppZoom percent -> { state with AppZoomPercent = percent }
        | SelectFontZoom percent -> { state with FontZoomPercent = percent }
        | SelectMotionPreset preset -> { state with MotionPreset = preset }
        | CycleViewport ->
            { state with
                Viewport = nextItem state.Viewport viewportPresets }
        | CycleAppZoom ->
            { state with
                AppZoomPercent = nextItem state.AppZoomPercent appZoomLevels }
        | CycleFontZoom ->
            { state with
                FontZoomPercent = nextItem state.FontZoomPercent fontZoomLevels }
        | CycleMotionPreset ->
            { state with
                MotionPreset = nextItem state.MotionPreset motionPresets }
        | SetReducedMotion enabled -> { state with ReducedMotion = enabled }
        | ToggleReducedMotion ->
            { state with
                ReducedMotion = not state.ReducedMotion }
        | SetAnimationProgress progress -> { state with AnimationProgress = progress }
        | HandleKeyboard key ->
            { state with
                Keyboard = CatalogKeyboard.handle key state.Keyboard }
