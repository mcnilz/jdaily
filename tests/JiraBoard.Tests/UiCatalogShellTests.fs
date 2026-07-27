module JiraBoard.Tests.UiCatalogShellTests

open Xunit
open JiraBoard.Ui
open JiraBoard.UiCatalog

[<Fact>]
let ``catalog starts in the deterministic 1080p inspection state`` () =
    let state = CatalogShell.init ()

    Assert.Equal("1920 × 1080", state.Viewport.Name)
    Assert.Equal(1920, state.Viewport.Width)
    Assert.Equal(1080, state.Viewport.Height)
    Assert.Equal(100, state.AppZoomPercent)
    Assert.Equal(100, state.FontZoomPercent)
    Assert.Equal(Motion.Normal, state.MotionPreset)
    Assert.False(state.ReducedMotion)
    Assert.Equal(0.0, state.AnimationProgress, 3)

[<Fact>]
let ``catalog exposes the specified deterministic inspection choices`` () =
    let viewportNames = CatalogShell.viewportPresets |> List.map _.Name

    Assert.Equal<string list>(
        [ "1920 × 1080"; "2560 × 1440"; "3440 × 1440"; "3840 × 2160" ],
        viewportNames
    )

    Assert.Equal<int list>([ 75; 90; 100; 110; 125; 150; 175; 200 ], CatalogShell.appZoomLevels)
    Assert.Equal<int list>([ 80; 90; 100; 110; 125; 150; 175; 200 ], CatalogShell.fontZoomLevels)
    Assert.Equal<Motion.SpeedPreset list>([ Motion.Calm; Motion.Normal; Motion.Fast ], CatalogShell.motionPresets)

[<Fact>]
let ``catalog controls update the inspected presentation state`` () =
    let ultrawide = CatalogShell.viewportPresets |> List.item 2

    let state =
        CatalogShell.init ()
        |> CatalogShell.update (SelectViewport ultrawide)
        |> CatalogShell.update (SelectAppZoom 125)
        |> CatalogShell.update (SelectFontZoom 150)
        |> CatalogShell.update (SelectMotionPreset Motion.Fast)
        |> CatalogShell.update (SetReducedMotion true)
        |> CatalogShell.update (SetAnimationProgress 0.75)

    Assert.Equal("3440 × 1440", state.Viewport.Name)
    Assert.Equal(125, state.AppZoomPercent)
    Assert.Equal(150, state.FontZoomPercent)
    Assert.Equal(Motion.Fast, state.MotionPreset)
    Assert.True(state.ReducedMotion)
    Assert.Equal(0.75, state.AnimationProgress, 3)

[<Fact>]
let ``catalog registers and selects its deterministic shell scenario`` () =
    let scenarioIds = CatalogScenarios.all |> List.map _.Id
    let state = CatalogShell.init ()

    Assert.Equal<string list>([ "Shell.Overview" ], scenarioIds)
    Assert.Equal("Shell.Overview", state.SelectedScenarioId)

[<Fact>]
let ``repeated viewport actions advance from the current selection`` () =
    let state =
        CatalogShell.init ()
        |> CatalogShell.update CycleViewport
        |> CatalogShell.update CycleViewport

    Assert.Equal("3440 × 1440", state.Viewport.Name)

[<Fact>]
let ``catalog shell dimensions and animation stops match the specification`` () =
    Assert.Equal(32.0, CatalogShell.layout.MenuHeight, 3)
    Assert.Equal(48.0, CatalogShell.layout.ControlBarMinimumHeight, 3)
    Assert.Equal(1024.0, CatalogShell.layout.MinimumWindowWidth, 3)
    Assert.Equal(640.0, CatalogShell.layout.MinimumWindowHeight, 3)

    Assert.Equal<float list>(
        [ 0.0; 0.25; 0.50; 0.75; 1.0 ],
        CatalogShell.animationProgressStops
    )

[<Fact>]
let ``repeated reduced motion actions toggle from the current state`` () =
    let state =
        CatalogShell.init ()
        |> CatalogShell.update ToggleReducedMotion
        |> CatalogShell.update ToggleReducedMotion

    Assert.False(state.ReducedMotion)
