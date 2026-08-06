module JiraBoard.Tests.NavigationViewTests

open Xunit
open JiraBoard.Domain
open JiraBoard.Domain.Navigation
open JiraBoard.Ui

module private Fixtures =
    let siteId = SiteId "https://example.atlassian.net"

    let p1 = { ProjectId = ProjectId "10000"; Name = "Project 1" }
    let b1 = { BoardId = BoardId 1L; Name = "Board 1" }

    // Deliberately not alphabetically ordered so a test can prove the sprint
    // menu keeps the delivered source order instead of sorting by name.
    let s1 = { SprintId = SprintId 11L; Name = "Zulu Sprint" }
    let s2 = { SprintId = SprintId 12L; Name = "Alpha Sprint" }
    let activeSprints = [ s1; s2 ]

    let confirmedContext =
        { SiteId = siteId
          ProjectId = p1.ProjectId
          BoardId = b1.BoardId
          SprintScope = AllActiveSprints }

    let offlineData =
        { SiteId = siteId
          Projects = [ p1 ]
          Boards = Map.ofList [ (p1.ProjectId, [ b1 ]) ]
          ActiveSprints = Map.ofList [ (b1.BoardId, activeSprints) ] }

    /// UI-specific enrichment: the domain `Project` has no key or type, so the
    /// row projection adds them while keeping `ProjectId`/`Name` from the domain.
    let rowFor (project: Project) : ProjectRow =
        { ProjectId = project.ProjectId
          Name = project.Name
          Key = "PRJ"
          TypeLabel = "Team-managed Scrum"
          IsLastUsed = false }

open Fixtures

// ContextHeader -----------------------------------------------------------

[<Fact>]
let ``context header shows team managed scrum and all active sprints`` () =
    let model: ContextHeaderModel =
        { ActiveSprints = activeSprints
          Scope = AllActiveSprints }

    Assert.Equal("Team Managed Scrum · Alle aktiven Sprints", ContextHeader.text model)

[<Fact>]
let ``context header shows the active sprint name with an active suffix`` () =
    let model: ContextHeaderModel =
        { ActiveSprints = activeSprints
          Scope = ActiveSprint s2.SprintId }

    Assert.Equal("Team Managed Scrum · Alpha Sprint (Active)", ContextHeader.text model)

[<Fact>]
let ``context header projects directly from a ready navigation state without a modal`` () =
    let model =
        { State = Ready(confirmedContext, None)
          Generation = 1
          Data = offlineData }

    match ContextHeader.fromModel model with
    | Some header ->
        Assert.Equal<ActiveSprint list>(activeSprints, header.ActiveSprints)
        Assert.Equal(AllActiveSprints, header.Scope)
    | None -> Assert.Fail "Expected a context header projection for the Ready state"

[<Fact>]
let ``context header projects nothing while a project still needs to be selected`` () =
    let model =
        { State = InitialProjectSelection
          Generation = 1
          Data = offlineData }

    Assert.Equal<ContextHeaderModel option>(None, ContextHeader.fromModel model)

// ProjectSelectionModal -----------------------------------------------------

[<Fact>]
let ``project selection shows the stable site identity, not a display name`` () =
    Assert.Equal("https://example.atlassian.net", ProjectSelectionModal.siteHint siteId)

[<Fact>]
let ``project selection first start shows the fixed header and preselects the single available project`` () =
    let model = ProjectSelectionModal.firstStart siteId rowFor [ p1 ] ignore ignore ignore ignore

    Assert.Equal("Projekt auswählen", ProjectSelectionModal.headerText)
    Assert.Equal(siteId, model.SiteId)
    Assert.Equal(FirstStart, model.Variant)
    Assert.False((List.head model.Rows).IsLastUsed)
    Assert.Equal(Some p1.ProjectId, model.SelectedProjectId)

[<Fact>]
let ``project selection first start starts without a selection when multiple projects are available`` () =
    let p2 = { ProjectId = ProjectId "20000"; Name = "Project 2" }
    let model = ProjectSelectionModal.firstStart siteId rowFor [ p1; p2 ] ignore ignore ignore ignore

    Assert.Equal(None, model.SelectedProjectId)

[<Fact>]
let ``project selection highlights and preselects only the previously confirmed project after a failed restore`` () =
    let failedContext = { confirmedContext with BoardId = BoardId 999L }
    let model = ProjectSelectionModal.restoreFailed rowFor failedContext [ p1 ] ignore ignore ignore ignore

    Assert.Equal(ContextRestoreFailed, model.Variant)
    Assert.Equal(failedContext.SiteId, model.SiteId)
    Assert.True((List.head model.Rows).IsLastUsed)
    Assert.Equal(Some p1.ProjectId, model.SelectedProjectId)

[<Fact>]
let ``project selection restore failed starts without a selection when the last used project is unavailable`` () =
    let unknownContext = { confirmedContext with ProjectId = ProjectId "unknown" }
    let model = ProjectSelectionModal.restoreFailed rowFor unknownContext [ p1 ] ignore ignore ignore ignore

    Assert.Equal(None, model.SelectedProjectId)

[<Fact>]
let ``project selection rows are selected via a stable id callback, never opened directly`` () =
    let mutable selectedProjectId = None

    let model =
        ProjectSelectionModal.firstStart siteId rowFor [ p1 ] ignore (fun id -> selectedProjectId <- Some id) ignore ignore

    model.OnSelect (List.head model.Rows).ProjectId

    Assert.Equal(Some p1.ProjectId, selectedProjectId)

[<Fact>]
let ``project selection select updater ignores an unknown project id and accepts a known one`` () =
    let model = ProjectSelectionModal.firstStart siteId rowFor [ p1 ] ignore ignore ignore ignore

    let unchanged = ProjectSelectionModal.selectProject (ProjectId "unknown") model
    Assert.Equal(model.SelectedProjectId, unchanged.SelectedProjectId)

    let selected = ProjectSelectionModal.selectProject p1.ProjectId model
    Assert.Equal(Some p1.ProjectId, selected.SelectedProjectId)

[<Fact>]
let ``project selection global open action opens only the selected project id and is disabled without a selection`` () =
    let mutable openedProjectId = None
    let p2 = { ProjectId = ProjectId "20000"; Name = "Project 2" }

    let model =
        ProjectSelectionModal.firstStart
            siteId
            rowFor
            [ p1; p2 ]
            ignore
            ignore
            (fun id -> openedProjectId <- Some id)
            ignore

    Assert.Equal(None, model.SelectedProjectId)
    Assert.False(ProjectSelectionModal.canOpen model)

    let selected = ProjectSelectionModal.selectProject p1.ProjectId model
    Assert.True(ProjectSelectionModal.canOpen selected)

    selected.OnOpen(selected.SelectedProjectId |> Option.get)

    Assert.Equal(Some p1.ProjectId, openedProjectId)
    Assert.Equal("Projekt öffnen", ProjectSelectionModal.openProjectActionLabel)

[<Fact>]
let ``project selection cancel callback is reachable independently from open`` () =
    let mutable cancelled = false
    let model = ProjectSelectionModal.firstStart siteId rowFor [ p1 ] ignore ignore ignore (fun () -> cancelled <- true)

    model.OnCancel()

    Assert.True(cancelled)

[<Fact>]
let ``project selection row action names reflect selection state distinctly per project key for automation`` () =
    let p2 = { ProjectId = ProjectId "20000"; Name = "Project 2" }

    let rowForTwo (project: Project) =
        if project.ProjectId = p1.ProjectId then
            rowFor project
        else
            { rowFor project with Key = "SEC" }

    let model = ProjectSelectionModal.firstStart siteId rowForTwo [ p1; p2 ] ignore ignore ignore ignore
    let row1 = model.Rows |> List.find (fun r -> r.ProjectId = p1.ProjectId)
    let row2 = model.Rows |> List.find (fun r -> r.ProjectId = p2.ProjectId)

    Assert.Equal("Auswählen · PRJ", ProjectSelectionModal.selectActionName false row1)
    Assert.Equal("Ausgewählt · PRJ", ProjectSelectionModal.selectActionName true row1)
    Assert.Equal("Auswählen · SEC", ProjectSelectionModal.selectActionName false row2)

[<Fact>]
let ``project row accessible name mentions key name and type and highlights the previous selection`` () =
    let normalRow = rowFor p1
    let highlightedRow = { normalRow with IsLastUsed = true }

    Assert.Equal("PRJ · Project 1 · Team-managed Scrum", ProjectSelectionModal.rowAccessibleName normalRow)

    Assert.Equal(
        "PRJ · Project 1 · Team-managed Scrum · zuletzt verwendet",
        ProjectSelectionModal.rowAccessibleName highlightedRow
    )

[<Fact>]
let ``project selection search field has a fixed automation name and the specified hit-target height`` () =
    Assert.Equal("Projekte durchsuchen", ProjectSelectionModal.searchFieldName)
    Assert.Equal(HitTarget.preferred, ProjectSelectionModal.searchFieldMinHeight)

[<Fact>]
let ``project selection search filters rows by name or key case-insensitively and an empty search shows all`` () =
    let p2 = { ProjectId = ProjectId "20000"; Name = "Security Initiative" }

    let rowForTwo (project: Project) =
        if project.ProjectId = p1.ProjectId then
            rowFor project
        else
            { rowFor project with Key = "SEC" }

    let baseModel = ProjectSelectionModal.firstStart siteId rowForTwo [ p1; p2 ] ignore ignore ignore ignore

    let byName = ProjectSelectionModal.withSearchQuery "security" baseModel

    Assert.Equal<string list>(
        [ "Security Initiative" ],
        ProjectSelectionModal.visibleRows byName |> List.map (fun r -> r.Name)
    )

    let byKey = ProjectSelectionModal.withSearchQuery "sec" baseModel

    Assert.Equal<string list>(
        [ "Security Initiative" ],
        ProjectSelectionModal.visibleRows byKey |> List.map (fun r -> r.Name)
    )

    let byUpperCaseName = ProjectSelectionModal.withSearchQuery "PROJECT 1" baseModel

    Assert.Equal<string list>(
        [ "Project 1" ],
        ProjectSelectionModal.visibleRows byUpperCaseName |> List.map (fun r -> r.Name)
    )

    let noMatch = ProjectSelectionModal.withSearchQuery "zzz" baseModel
    Assert.Equal<string list>([], ProjectSelectionModal.visibleRows noMatch |> List.map (fun r -> r.Name))

    let empty = ProjectSelectionModal.withSearchQuery "" baseModel
    Assert.Equal(2, (ProjectSelectionModal.visibleRows empty).Length)

[<Fact>]
let ``project selection search change callback and query updater keep search state out of any hidden view-local field`` () =
    let mutable observedQuery = None

    let model =
        ProjectSelectionModal.firstStart siteId rowFor [ p1 ] (fun query -> observedQuery <- Some query) ignore ignore ignore

    model.OnSearchChange "PR"

    Assert.Equal(Some "PR", observedQuery)
    Assert.Equal("", model.SearchQuery)

    let updated = ProjectSelectionModal.withSearchQuery "PR" model
    Assert.Equal("PR", updated.SearchQuery)

[<Fact>]
let ``project selection projects directly from the domain navigation state`` () =
    let initial =
        { State = InitialProjectSelection
          Generation = 1
          Data = offlineData }

    match ProjectSelectionModal.fromModel siteId rowFor ignore ignore ignore ignore initial with
    | Some model -> Assert.Equal(FirstStart, model.Variant)
    | None -> Assert.Fail "Expected a FirstStart projection"

    let failedModel = Navigation.init (Some { confirmedContext with BoardId = BoardId 999L }) offlineData

    match ProjectSelectionModal.fromModel siteId rowFor ignore ignore ignore ignore failedModel with
    | Some model ->
        Assert.Equal(ContextRestoreFailed, model.Variant)
        Assert.True((List.head model.Rows).IsLastUsed)
    | None -> Assert.Fail "Expected a RestoreFailed projection"

    let readyModel =
        { State = Ready(confirmedContext, None)
          Generation = 1
          Data = offlineData }

    Assert.Equal<ProjectSelectionModel option>(
        None,
        ProjectSelectionModal.fromModel siteId rowFor ignore ignore ignore ignore readyModel
    )

// SprintMenu ----------------------------------------------------------------

[<Fact>]
let ``sprint menu always lists all active sprints first`` () =
    let items = SprintMenu.items activeSprints AllActiveSprints

    Assert.Equal("Alle aktiven Sprints", (List.head items).Label)
    Assert.Equal(AllActiveSprints, (List.head items).Scope)

[<Fact>]
let ``sprint menu keeps the delivered source order without sorting by name`` () =
    let items = SprintMenu.items activeSprints AllActiveSprints
    let sprintLabels = items |> List.skip 1 |> List.map (fun item -> item.Label)

    Assert.Equal<string list>([ "Zulu Sprint"; "Alpha Sprint" ], sprintLabels)

[<Fact>]
let ``sprint menu marks the confirmed scope as selected`` () =
    let items = SprintMenu.items activeSprints (ActiveSprint s2.SprintId)
    let selected = items |> List.filter (fun item -> item.IsSelected)

    Assert.Equal(1, selected.Length)
    Assert.Equal(ActiveSprint s2.SprintId, selected.Head.Scope)

[<Fact>]
let ``sprint menu falls back to all active sprints selected when no scope matches`` () =
    let items = SprintMenu.items activeSprints (ActiveSprint(SprintId 999L))
    let selected = items |> List.filter (fun item -> item.IsSelected)

    Assert.Equal(1, selected.Length)
    Assert.Equal(AllActiveSprints, selected.Head.Scope)

[<Fact>]
let ``sprint menu selection callback receives the sprint scope not the sprint name`` () =
    let mutable selectedScope = None
    let model = SprintMenu.build activeSprints AllActiveSprints (fun scope -> selectedScope <- Some scope)
    let secondItem = model.Items |> List.item 1

    model.OnSelect secondItem.Scope

    Assert.Equal(Some(ActiveSprint s1.SprintId), selectedScope)

[<Fact>]
let ``sprint menu accessible name marks the selected scope with checked state text`` () =
    let items = SprintMenu.items activeSprints AllActiveSprints
    let selected = List.head items
    let unselected = List.item 1 items

    Assert.Contains("ausgewählt", SprintMenu.accessibleName selected)
    Assert.DoesNotContain("ausgewählt", SprintMenu.accessibleName unselected)

[<Fact>]
let ``sprint menu rows meet the minimum hit target height`` () =
    Assert.True(SprintMenu.itemMinHeight >= HitTarget.minimum)
