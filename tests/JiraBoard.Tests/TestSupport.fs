namespace JiraBoard.Tests

open Xunit

[<RequireQualifiedAccess>]
module TestResult =
    let assertOk<'value, 'error> (expected: 'value) (result: Result<'value, 'error>) =
        match result with
        | Ok actual -> Assert.Equal<'value>(expected, actual)
        | Error error -> Assert.Fail $"Expected Ok, but received Error: %A{error}"
