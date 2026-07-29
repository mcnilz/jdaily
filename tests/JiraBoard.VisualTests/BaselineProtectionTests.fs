module JiraBoard.VisualTests.BaselineProtectionTests

open System
open System.IO
open Xunit

[<Fact>]
let ``missing baseline writes an actual artifact but never creates a verified baseline`` () =
    let artifactDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"))
    let baselinePath = Path.Combine(artifactDirectory, "board-surface.verified.png")

    try
        let comparison =
            BaselineProtection.compare artifactDirectory "board-surface" baselinePath [| 1uy; 2uy; 3uy |]

        Assert.Equal(BaselineComparisonOutcome.MissingBaseline, comparison.Outcome)
        Assert.True(File.Exists(comparison.ActualArtifactPath))
        Assert.True(File.Exists(comparison.DiffArtifactPath))
        Assert.False(File.Exists(baselinePath))
    finally
        if Directory.Exists(artifactDirectory) then
            Directory.Delete(artifactDirectory, true)

[<Fact>]
let ``different frame emits artifacts without overwriting the verified baseline`` () =
    let artifactDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"))
    let baselinePath = Path.Combine(artifactDirectory, "board-surface.verified.png")
    let verifiedFrame = [| 1uy; 2uy; 3uy |]

    try
        Directory.CreateDirectory(artifactDirectory) |> ignore
        File.WriteAllBytes(baselinePath, verifiedFrame)

        let comparison =
            BaselineProtection.compare artifactDirectory "board-surface" baselinePath [| 4uy; 5uy; 6uy |]

        Assert.Equal(BaselineComparisonOutcome.Different, comparison.Outcome)
        Assert.True(File.Exists(comparison.ActualArtifactPath))
        Assert.True(File.Exists(comparison.DiffArtifactPath))
        Assert.Equal<byte>(verifiedFrame, File.ReadAllBytes(baselinePath))
    finally
        if Directory.Exists(artifactDirectory) then
            Directory.Delete(artifactDirectory, true)

[<Fact>]
let ``matching frame does not emit a diff artifact`` () =
    let artifactDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"))
    let baselinePath = Path.Combine(artifactDirectory, "board-surface.verified.png")
    let frame = [| 1uy; 2uy; 3uy |]

    try
        Directory.CreateDirectory(artifactDirectory) |> ignore
        File.WriteAllBytes(baselinePath, frame)

        let comparison = BaselineProtection.compare artifactDirectory "board-surface" baselinePath frame

        Assert.Equal(BaselineComparisonOutcome.Matches, comparison.Outcome)
        Assert.False(File.Exists(comparison.ActualArtifactPath))
        Assert.False(File.Exists(comparison.DiffArtifactPath))
    finally
        if Directory.Exists(artifactDirectory) then
            Directory.Delete(artifactDirectory, true)