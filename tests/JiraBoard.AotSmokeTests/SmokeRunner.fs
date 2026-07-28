namespace JiraBoard.AotSmokeTests

type SmokeCheck =
    { Name: string
      Run: unit -> Result<unit, string> }

type SmokeRunResult =
    { Failures: string list }

[<RequireQualifiedAccess>]
module SmokeRunner =
    let run checks =
        let failures =
            checks
            |> List.choose (fun check ->
                try
                    match check.Run() with
                    | Ok() -> None
                    | Error message -> Some $"{check.Name}: {message}"
                with error ->
                    Some $"{check.Name}: {error.Message}")

        { Failures = failures }

    let exitCode result =
        if List.isEmpty result.Failures then 0 else 1