module JiraBoard.VisualTests.BoardSurfaceGoldenMasterTests

open System
open System.IO
open JiraBoard.Ui
open JiraBoard.UiCatalog
open Xunit

[<Theory>]
[<InlineData(1920, 1080)>]
[<InlineData(3840, 2160)>]
let ``catalog board surface renders a PNG at each golden master reference viewport`` width height =
    let artifactDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"))
    let baselinePath = Path.Combine(artifactDirectory, "board-surface.verified.png")

    try
        let png, measurements =
            HeadlessTestHost.measurePngFrame width height (fun () ->
                BoardSurface.viewAt
                    (DisplayScale.create 100 100)
                    (float width)
                    ComponentCatalogFixtures.boardSurface)

        let comparison = BaselineProtection.compare artifactDirectory "board-surface" baselinePath png

        Assert.True(png.Length > 8)
        Assert.Equal<byte>([| 137uy; 80uy; 78uy; 71uy; 13uy; 10uy; 26uy; 10uy |], png[..7])
        Assert.Equal(BaselineComparisonOutcome.MissingBaseline, comparison.Outcome)
        Assert.True(File.Exists(comparison.ActualArtifactPath))
        Assert.True(File.Exists(comparison.DiffArtifactPath))
        Assert.False(File.Exists(baselinePath))
        Assert.True(measurements.CpuMilliseconds >= 0.0)
        Assert.True(measurements.ManagedMemoryBytes >= 0L)
        Assert.True(measurements.FrameMilliseconds > 0.0)
        Assert.True(measurements.VisualTreeNodeCount > 0)
    finally
        if Directory.Exists(artifactDirectory) then
            Directory.Delete(artifactDirectory, true)