namespace JiraBoard.VisualTests

type GoldenMasterConfiguration =
    { OperatingSystem: string
      Renderer: string
      OperatingSystemDpiPercent: int
      Culture: string
      TimeZone: string
      Viewports: (int * int) list
      AutomaticBaselineUpdates: bool }

[<RequireQualifiedAccess>]
module GoldenMasterEnvironment =
    let configuration =
        { OperatingSystem = "Windows 11 x64"
          Renderer = "Skia"
          OperatingSystemDpiPercent = 100
          Culture = "de-DE"
          TimeZone = "Europe/Berlin"
          Viewports = [ (1920, 1080); (2560, 1440); (3440, 1440); (3840, 2160) ]
          AutomaticBaselineUpdates = false }