module JiraBoard.Tests.HarnessTests

open Xunit

[<Fact>]
let ``result helper accepts the expected successful value`` () =
    TestResult.assertOk 42 (Ok 42)
