module JiraBoard.VisualTests.GoldenMasterEnvironmentTests

open Xunit

[<Fact>]
let ``golden master environment is fixed and does not update baselines automatically`` () =
    let environment = GoldenMasterEnvironment.configuration

    Assert.Equal("Windows 11 x64", environment.OperatingSystem)
    Assert.Equal("Skia", environment.Renderer)
    Assert.Equal(100, environment.OperatingSystemDpiPercent)
    Assert.Equal("de-DE", environment.Culture)
    Assert.Equal("Europe/Berlin", environment.TimeZone)
    Assert.Equal<(int * int) list>(
        [ (1920, 1080); (2560, 1440); (3440, 1440); (3840, 2160) ],
        environment.Viewports
    )
    Assert.False(environment.AutomaticBaselineUpdates)