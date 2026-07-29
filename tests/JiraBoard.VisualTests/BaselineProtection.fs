namespace JiraBoard.VisualTests

open System.IO

type BaselineComparisonOutcome =
    | Matches
    | MissingBaseline
    | Different

type BaselineComparison =
    { Outcome: BaselineComparisonOutcome
      ActualArtifactPath: string
      DiffArtifactPath: string }

[<RequireQualifiedAccess>]
module BaselineProtection =
    let private artifactPaths artifactDirectory scenarioName =
        Path.Combine(artifactDirectory, $"{scenarioName}.actual.png"),
        Path.Combine(artifactDirectory, $"{scenarioName}.diff.txt")

    let private writeArtifacts
        (actualArtifactPath: string)
        (diffArtifactPath: string)
        (description: string)
        (actualFrame: byte array)
        =
        File.WriteAllBytes(actualArtifactPath, actualFrame)
        File.WriteAllText(diffArtifactPath, description)

    let compare (artifactDirectory: string) (scenarioName: string) (baselinePath: string) (actualFrame: byte array) =
        Directory.CreateDirectory(artifactDirectory) |> ignore

        let actualArtifactPath, diffArtifactPath = artifactPaths artifactDirectory scenarioName

        if not (File.Exists(baselinePath)) then
            writeArtifacts actualArtifactPath diffArtifactPath "Verified baseline is missing." actualFrame

            { Outcome = MissingBaseline
              ActualArtifactPath = actualArtifactPath
              DiffArtifactPath = diffArtifactPath }
        elif File.ReadAllBytes(baselinePath) = actualFrame then
            { Outcome = Matches
              ActualArtifactPath = actualArtifactPath
              DiffArtifactPath = diffArtifactPath }
        else
            writeArtifacts actualArtifactPath diffArtifactPath "Actual frame differs from the verified baseline." actualFrame

            { Outcome = Different
              ActualArtifactPath = actualArtifactPath
              DiffArtifactPath = diffArtifactPath }