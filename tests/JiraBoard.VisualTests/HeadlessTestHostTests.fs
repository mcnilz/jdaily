module JiraBoard.VisualTests.HeadlessTestHostTests

open Xunit

[<Fact>]
let ``headless test host creates an Avalonia application builder`` () =
    let applicationBuilder = HeadlessTestHost.createApplicationBuilder ()

    Assert.NotNull(applicationBuilder)