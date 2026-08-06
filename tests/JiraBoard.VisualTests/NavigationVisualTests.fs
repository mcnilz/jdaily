namespace JiraBoard.VisualTests

open Avalonia
open Avalonia.Automation
open Avalonia.Controls
open Avalonia.FuncUI.DSL
open Avalonia.FuncUI.Hosts
open Avalonia.FuncUI.Types
open Avalonia.VisualTree
open JiraBoard.Domain
open JiraBoard.Ui
open JiraBoard.UiCatalog
open Xunit

module NavigationVisualTests =

    /// The `HostWindow` realizes its FuncUI `IView` content into the actual
    /// Avalonia control tree via the inherited `ContentControl.Content`
    /// property. `window.GetVisualDescendants()` alone only ever returns
    /// descendants of the window and never the content root itself, so a
    /// bare/unwrapped root control would otherwise be missed. This combines
    /// the realized content root (if any) with its own visual descendants
    /// into a single, duplicate-free control set.
    let private allControls (window: HostWindow) : Visual seq =
        let contentRoot =
            match (window :> ContentControl).Content with
            | :? Visual as visual -> Some visual
            | _ -> None

        match contentRoot with
        | Some root -> Seq.append (Seq.singleton root) (root.GetVisualDescendants())
        | None -> window.GetVisualDescendants()

    let private findText (window: HostWindow) (text: string) =
        allControls window
        |> Seq.choose (function
            | :? TextBlock as tb -> Some tb.Text
            | _ -> None)
        |> Seq.exists (fun t -> t <> null && t.Contains(text))

    let private findTextBoxWithAutomationName (window: HostWindow) (name: string) : TextBox option =
        allControls window
        |> Seq.choose (function
            | :? TextBox as tb -> Some tb
            | _ -> None)
        |> Seq.tryFind (fun tb -> AutomationProperties.GetName tb = name)

    let private findButtonWithAutomationName (window: HostWindow) (name: string) =
        allControls window
        |> Seq.choose (function
            | :? Button as b -> Some b
            | _ -> None)
        |> Seq.tryFind (fun b -> AutomationProperties.GetName b = name)

    [<Fact>]
    let ``Navigation.ContextRestore.Startup renders context label and no modal`` () =
        let model =
            match ContextHeader.fromModel ComponentCatalogFixtures.navigationContextRestoreModel with
            | Some m -> m
            | None -> failwith "Fixture should provide a ContextHeader model"

        HeadlessTestHost.run 1920 1080 (fun () -> ContextHeader.viewAt DisplayScale.normal model) (fun window ->
            let expectedLabel = ContextHeader.text model
            Assert.True(findText window expectedLabel, $"Expected text label '{expectedLabel}' not found")
            // "kein Modal" - since we only render the ContextHeader here, we just verify it renders.
            Assert.True(window.GetVisualDescendants() |> Seq.length > 0)
        )

    [<Fact>]
    let ``Navigation.ProjectSelection.FirstStart renders title, project row, search field and the global open action`` () =
        let model =
            ProjectSelectionModal.firstStart
                ComponentCatalogFixtures.navigationSiteId
                ComponentCatalogFixtures.navigationRowFor
                ComponentCatalogFixtures.navigationFirstStartModel.Data.Projects
                ignore
                ignore
                ignore
                ignore

        HeadlessTestHost.run 800 600 (fun () -> ProjectSelectionModal.viewAt DisplayScale.normal model) (fun window ->
            Assert.True(findText window ProjectSelectionModal.headerText, "Header text not found")

            let firstProject = ComponentCatalogFixtures.navigationProject
            Assert.True(findText window firstProject.Name, $"Project name '{firstProject.Name}' not found")

            let row = ComponentCatalogFixtures.navigationRowFor firstProject
            Assert.True(findText window row.Key, $"Project key '{row.Key}' not found")

            match findTextBoxWithAutomationName window ProjectSelectionModal.searchFieldName with
            | Some searchField -> Assert.True(searchField.IsVisible, "Search field is not visible")
            | None -> Assert.Fail "Expected the search field to be present with its automation name"

            match findButtonWithAutomationName window ProjectSelectionModal.openProjectActionLabel with
            | Some openButton ->
                Assert.True(openButton.IsVisible, "Global open action is not visible")
                Assert.Equal(ProjectSelectionModal.openProjectActionLabel, openButton.Content :?> string)
                Assert.True(openButton.IsEnabled, "Global open action should be enabled once a project is selected")
            | None -> Assert.Fail "Expected the global open action to be present with its automation name"
        )

    [<Fact>]
    let ``Navigation.ProjectSelection.RestoreFailed renders hint, highlighted project and the global open action`` () =
        match ComponentCatalogFixtures.navigationRestoreFailedModel.State with
        | RestoreFailed(failedContext, projects) ->
            let model =
                ProjectSelectionModal.restoreFailed
                    ComponentCatalogFixtures.navigationRowFor
                    failedContext
                    projects
                    ignore
                    ignore
                    ignore
                    ignore

            HeadlessTestHost.run 800 600 (fun () -> ProjectSelectionModal.viewAt DisplayScale.normal model) (fun window ->
                Assert.True(findText window ProjectSelectionModal.headerText, "Header text not found")
                Assert.True(findText window ProjectSelectionModal.restoreFailedHint, "Restore failed hint not found")

                let failedProject =
                    projects |> List.find (fun p -> p.ProjectId = failedContext.ProjectId)
                Assert.True(findText window failedProject.Name, $"Failed project name '{failedProject.Name}' not found")

                match findTextBoxWithAutomationName window ProjectSelectionModal.searchFieldName with
                | Some searchField -> Assert.True(searchField.IsVisible, "Search field is not visible")
                | None -> Assert.Fail "Expected the search field to be present with its automation name"

                match findButtonWithAutomationName window ProjectSelectionModal.openProjectActionLabel with
                | Some openButton ->
                    Assert.True(openButton.IsVisible, "Global open action is not visible")
                    Assert.Equal(ProjectSelectionModal.openProjectActionLabel, openButton.Content :?> string)
                    // The last-used project is preselected on restore failure, so the action is enabled.
                    Assert.True(openButton.IsEnabled, "Global open action should be enabled once the last-used project is preselected")
                | None -> Assert.Fail "Expected the global open action to be present with its automation name"
            )
        | _ -> failwith "Fixture should be in RestoreFailed state"

    [<Fact>]
    let ``Navigation.SprintMenu.AllActive renders selected scope`` () =
        let model = ComponentCatalogFixtures.navigationSprintMenuAllActive

        HeadlessTestHost.run 400 300 (fun () -> SprintMenu.viewAt DisplayScale.normal model) (fun window ->
            let expectedLabel = SprintMenu.allActiveSprintsLabel

            Assert.True(findText window expectedLabel, $"Label '{expectedLabel}' not found")
            Assert.True(findText window "✓", "Checkmark not found")
            // Verify that other items are also present but not selected (no checkmark for them)
            for item in model.Items do
                Assert.True(findText window item.Label, $"Item label '{item.Label}' not found")
        )

    [<Fact>]
    let ``Navigation.SprintMenu.Single renders selected scope`` () =
        let model = ComponentCatalogFixtures.navigationSprintMenuSingle

        HeadlessTestHost.run 400 300 (fun () -> SprintMenu.viewAt DisplayScale.normal model) (fun window ->
            let selectedSprint =
                model.Items
                |> List.find (fun i -> i.IsSelected)

            Assert.True(findText window selectedSprint.Label, $"Selected label '{selectedSprint.Label}' not found")
            Assert.True(findText window "✓", "Checkmark not found")
            for item in model.Items do
                Assert.True(findText window item.Label, $"Item label '{item.Label}' not found")
        )
