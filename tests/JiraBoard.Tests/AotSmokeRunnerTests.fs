module JiraBoard.Tests.AotSmokeRunnerTests

open Xunit
open JiraBoard.AotSmokeTests

[<Fact>]
let ``smoke runner executes every static check and fails when one check fails`` () =
    let executed = ResizeArray<string>()

    let checks =
        [ { Name = "successful check"
            Run = fun () ->
                executed.Add "successful check"
                Ok() }
          { Name = "failing check"
            Run = fun () ->
                executed.Add "failing check"
                Error "expected failure" } ]

    let result = SmokeRunner.run checks

    Assert.Equal<string list>([ "successful check"; "failing check" ], List.ofSeq executed)
    Assert.Equal<string list>([ "failing check: expected failure" ], result.Failures)
    Assert.Equal(1, SmokeRunner.exitCode result)