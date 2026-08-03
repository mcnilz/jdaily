module JiraBoard.Tests.DevelopmentInformationTests

open JiraBoard.Domain
open Xunit

[<Fact>]
let ``Jira provided capability publishes exactly the supported development information kinds`` () =
    let capability =
        JiraProvided(Set.ofList [ Commit; Branch; PullRequest ])

    match capability with
    | JiraProvided kinds ->
        Assert.Equal<Set<DevelopmentInfoKind>>(
            Set.ofList [ Commit; Branch; PullRequest ],
            kinds
        )
    | Unavailable -> Assert.Fail("Expected JiraProvided capability.")

[<Fact>]
let ``unavailable development information is a normal capability result`` () =
    let capability: DevelopmentInfoCapability = Unavailable

    Assert.Equal(Unavailable, capability)

[<Fact>]
let ``a capability port can publish unavailable without transport or credential details`` () =
    let port: DevelopmentInfoCapabilityPort = { GetCapability = fun () -> Unavailable }

    Assert.Equal(Unavailable, port.GetCapability())