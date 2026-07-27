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
      MinimumWindowHeight: float }

[<RequireQualifiedAccess>]
module CatalogScenarios =
    let all =
        [ { Id = "Shell.Overview"
            Name = "Katalogübersicht"
            Area = "Shell" } ]

type CatalogShellState =
    { Viewport: ViewportPreset
      AppZoomPercent: int
      FontZoomPercent: int
      MotionPreset: Motion.SpeedPreset
      ReducedMotion: bool
      AnimationProgress: float
      SelectedScenarioId: string }

type CatalogShellMessage =
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
          MinimumWindowHeight = 640.0 }

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
          SelectedScenarioId = CatalogScenarios.all.Head.Id }

    let update message state =
        match message with
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
