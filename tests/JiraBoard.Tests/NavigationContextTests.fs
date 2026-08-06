namespace JiraBoard.Tests

open Xunit
open JiraBoard.Domain
open JiraBoard.Domain.Navigation

module NavigationContextTests =

    let siteId = SiteId "https://example.atlassian.net"
    let otherSiteId = SiteId "https://other.atlassian.net"

    let p1 = { ProjectId = ProjectId "10000"; Name = "Project 1" }
    let b1 = { BoardId = BoardId 1L; Name = "Board 1" }
    let s1 = { SprintId = SprintId 11L; Name = "Active Sprint 1" }
    let s2 = { SprintId = SprintId 12L; Name = "Active Sprint 2" }

    let offlineData = {
        SiteId = siteId
        Projects = [ p1 ]
        Boards = Map.ofList [ (p1.ProjectId, [ b1 ]) ]
        ActiveSprints = Map.ofList [ (b1.BoardId, [ s1; s2 ]) ]
    }

    [<Fact>]
    let ``No stored context results in InitialProjectSelection`` () =
        let model = Navigation.init None offlineData
        Assert.Equal(InitialProjectSelection, model.State)

    [<Fact>]
    let ``Valid stored context results in Ready state without hint`` () =
        let context = { SiteId = siteId; ProjectId = p1.ProjectId; BoardId = b1.BoardId; SprintScope = AllActiveSprints }
        let model = Navigation.init (Some context) offlineData
        match model.State with
        | Ready(c, None) ->
            Assert.Equal(context, c)
        | _ -> Assert.Fail($"Expected Ready state without hint, but got {model.State}")

    [<Fact>]
    let ``Stored context for a different site results in RestoreFailed`` () =
        let context = { SiteId = otherSiteId; ProjectId = p1.ProjectId; BoardId = b1.BoardId; SprintScope = AllActiveSprints }
        let model = Navigation.init (Some context) offlineData
        match model.State with
        | RestoreFailed(failedContext, availableProjects) ->
            Assert.Equal(context, failedContext)
            Assert.Contains(p1, availableProjects)
        | _ -> Assert.Fail($"Expected RestoreFailed state, but got {model.State}")

    [<Fact>]
    let ``Unknown project results in RestoreFailed and keeps last project context`` () =
        let unknownProjectId = ProjectId "99999"
        let context = { SiteId = siteId; ProjectId = unknownProjectId; BoardId = b1.BoardId; SprintScope = AllActiveSprints }
        let model = Navigation.init (Some context) offlineData
        match model.State with
        | RestoreFailed(failedContext, availableProjects) ->
            Assert.Equal(context, failedContext)
            Assert.Contains(p1, availableProjects)
        | _ -> Assert.Fail("Expected RestoreFailed state")

    [<Fact>]
    let ``Unknown board results in RestoreFailed for the project`` () =
        let unknownBoardId = BoardId 99L
        let context = { SiteId = siteId; ProjectId = p1.ProjectId; BoardId = unknownBoardId; SprintScope = AllActiveSprints }
        let model = Navigation.init (Some context) offlineData
        match model.State with
        | RestoreFailed(failedContext, _) ->
            Assert.Equal(context, failedContext)
        | _ -> Assert.Fail("Expected RestoreFailed state")

    [<Fact>]
    let ``Inactive sprint results in Ready with AllActiveSprints and hint`` () =
        let inactiveSprintId = SprintId 10L
        let context = { SiteId = siteId; ProjectId = p1.ProjectId; BoardId = b1.BoardId; SprintScope = ActiveSprint inactiveSprintId }
        let model = Navigation.init (Some context) offlineData
        match model.State with
        | Ready(newContext, Some InactiveSprintFallback) ->
            Assert.Equal(p1.ProjectId, newContext.ProjectId)
            Assert.Equal(b1.BoardId, newContext.BoardId)
            Assert.Equal(AllActiveSprints, newContext.SprintScope)
        | _ -> Assert.Fail("Expected Ready state with InactiveSprintFallback hint")

    [<Fact>]
    let ``Selecting a project and board starts AllActiveSprints and increments generation`` () =
        let model = Navigation.init None offlineData
        let newModel = Navigation.confirmBoard p1.ProjectId b1.BoardId model
        match newModel.State with
        | Ready(context, _) ->
            Assert.Equal(siteId, context.SiteId)
            Assert.Equal(p1.ProjectId, context.ProjectId)
            Assert.Equal(b1.BoardId, context.BoardId)
            Assert.Equal(AllActiveSprints, context.SprintScope)
            Assert.True(newModel.Generation > model.Generation)
        | _ -> Assert.Fail("Expected Ready state after confirmation")

    [<Fact>]
    let ``Confirming an unknown project leaves the model unchanged`` () =
        let model = Navigation.init None offlineData
        let unknownProjectId = ProjectId "99999"
        let newModel = Navigation.confirmBoard unknownProjectId b1.BoardId model
        Assert.Equal(model, newModel)

    [<Fact>]
    let ``Confirming a board that does not belong to the project leaves the model unchanged`` () =
        let model = Navigation.init None offlineData
        let unrelatedBoardId = BoardId 999L
        let newModel = Navigation.confirmBoard p1.ProjectId unrelatedBoardId model
        Assert.Equal(model, newModel)

    [<Fact>]
    let ``Selecting an active sprint updates context and increments generation`` () =
        let initialContext = { SiteId = siteId; ProjectId = p1.ProjectId; BoardId = b1.BoardId; SprintScope = AllActiveSprints }
        let model = { State = Ready(initialContext, None); Generation = 1; Data = offlineData }
        let newModel = Navigation.confirmSprint s1.SprintId model
        match newModel.State with
        | Ready(context, _) ->
            Assert.Equal(ActiveSprint s1.SprintId, context.SprintScope)
            Assert.Equal(2, newModel.Generation)
        | _ -> Assert.Fail("Expected Ready state with ActiveSprint")

    [<Fact>]
    let ``Selecting an invalid sprint does not change confirmed context`` () =
        let initialContext = { SiteId = siteId; ProjectId = p1.ProjectId; BoardId = b1.BoardId; SprintScope = AllActiveSprints }
        let model = { State = Ready(initialContext, None); Generation = 1; Data = offlineData }
        let invalidSprintId = SprintId 99L
        let newModel = Navigation.confirmSprint invalidSprintId model
        Assert.Equal(model, newModel)
