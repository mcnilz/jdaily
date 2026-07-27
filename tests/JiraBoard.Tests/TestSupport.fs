namespace JiraBoard.Tests

open Xunit

[<RequireQualifiedAccess>]
module TestResult =
    let assertOk<'value, 'error> (expected: 'value) (result: Result<'value, 'error>) =
        match result with
        | Ok actual -> Assert.Equal<'value>(expected, actual)
        | Error error -> Assert.Fail $"Expected Ok, but received Error: %A{error}"

[<RequireQualifiedAccess>]
module Fixture =
    open System.IO
    open System.Reflection

    let readResource (name: string) =
        let assembly = Assembly.GetExecutingAssembly()
        // Namespace is typically JiraBoard.Tests
        let resourceName = $"JiraBoard.Tests.Fixtures.%s{name}"
        use stream = assembly.GetManifestResourceStream(resourceName)
        if stream = null then
            let existing = assembly.GetManifestResourceNames() |> String.concat ", "
            failwith $"Resource '%s{resourceName}' not found. Available: %s{existing}"
        use reader = new StreamReader(stream)
        reader.ReadToEnd()
