namespace JiraBoard.Ui

open System
open Avalonia
open Avalonia.Controls
open Avalonia.FuncUI.DSL
open Avalonia.Layout
open Avalonia.Media
open JiraBoard.Domain

// VS-001: `ProjectSelectionModal` covers the first-start selection (no
// stored context yet) and the restore-failed selection (stored context could
// not be reopened) described in ui-design-specification.md, section
// "ProjectSelectionModal". Identity and callbacks use `ProjectId`, never the
// project name. Rows are selected, not opened directly: a single global
// primary action opens the currently selected `ProjectId`.

/// The domain `Project` has no key or project type. `ProjectRow` is a
/// UI-specific enrichment that keeps `ProjectId`/`Name` from the domain and
/// adds the display-only key and type label required by the modal.
type ProjectRow =
    { ProjectId: ProjectId
      Name: string
      Key: string
      TypeLabel: string
      IsLastUsed: bool }

// The case is deliberately not named `RestoreFailed`: `JiraBoard.Domain`
// auto-opens a `NavigationState.RestoreFailed` case with the same bare name,
// and this type lives in the same open scope as that domain module.
type ProjectSelectionVariant =
    | FirstStart
    | ContextRestoreFailed

/// `SearchQuery` and `SelectedProjectId` are host-owned UI state, not
/// re-derived on every render: a real host stores the value observed via
/// `OnSearchChange`/`OnSelect` and threads it back in through
/// `ProjectSelectionModal.withSearchQuery`/`selectProject` for the next
/// render. The view itself never keeps hidden mutable state of its own.
type ProjectSelectionModel =
    { SiteId: SiteId
      Variant: ProjectSelectionVariant
      Rows: ProjectRow list
      SearchQuery: string
      SelectedProjectId: ProjectId option
      OnSearchChange: string -> unit
      OnSelect: ProjectId -> unit
      OnOpen: ProjectId -> unit
      OnCancel: unit -> unit }

[<RequireQualifiedAccess>]
module ProjectSelectionModal =
    let private brush (color: JiraBoard.Ui.Color) =
        SolidColorBrush(Avalonia.Media.Color.Parse color.Hex) :> IBrush

    let headerText = "Projekt auswählen"

    let teamManagedScrumHint = "Alle Projekte sind Team-managed Scrum-Boards."

    let restoreFailedHint =
        "Der zuletzt verwendete Projekt- und Boardkontext konnte nicht geöffnet werden. Bitte wählen Sie erneut."

    let selectActionLabel = "Auswählen"

    let selectedActionLabel = "Ausgewählt"

    let openProjectActionLabel = "Projekt öffnen"

    let cancelActionLabel = "Abbrechen"

    /// Visible and automation name of the local project search field
    /// (ui-design-specification.md requirement: "Suchfeld ... 36 DIPs hoch").
    let searchFieldName = "Projekte durchsuchen"

    /// The search field uses the existing preferred hit-target token so the
    /// 36 DIP height never becomes a new local magic number.
    let searchFieldMinHeight = HitTarget.preferred

    /// The site hint is the stable site identity itself, never a display
    /// name that could drift from the actually connected site.
    let siteHint (SiteId site) = site

    /// Minimum project-row height (ui-design-specification.md: "Projektzeile
    /// mindestens 52 DIPs hoch").
    let rowMinHeight = 52.0

    let private highlight (lastUsedProjectId: ProjectId option) (row: ProjectRow) =
        { row with
            IsLastUsed = Some row.ProjectId = lastUsedProjectId }

    let private rowsFor (rowFor: Project -> ProjectRow) (lastUsedProjectId: ProjectId option) (projects: Project list) =
        projects |> List.map (rowFor >> highlight lastUsedProjectId)

    /// A first start with exactly one reachable project preselects it, since
    /// there is no real choice to make; two or more projects start with no
    /// selection so the primary action stays deliberately disabled.
    let private initialFirstStartSelection (rows: ProjectRow list) : ProjectId option =
        match rows with
        | [ single ] -> Some single.ProjectId
        | _ -> None

    /// A failed restore preselects the previously confirmed project only
    /// when it is still among the reachable rows; an unavailable last-used
    /// project starts with no selection instead of pointing at a project
    /// that no longer exists.
    let private initialRestoreFailedSelection (lastUsedProjectId: ProjectId) (rows: ProjectRow list) : ProjectId option =
        rows |> List.tryFind (fun row -> row.ProjectId = lastUsedProjectId) |> Option.map (fun row -> row.ProjectId)

    let firstStart
        (siteId: SiteId)
        (rowFor: Project -> ProjectRow)
        (projects: Project list)
        (onSearchChange: string -> unit)
        (onSelect: ProjectId -> unit)
        (onOpen: ProjectId -> unit)
        (onCancel: unit -> unit)
        : ProjectSelectionModel =
        let rows = rowsFor rowFor None projects

        { SiteId = siteId
          Variant = FirstStart
          Rows = rows
          SearchQuery = ""
          SelectedProjectId = initialFirstStartSelection rows
          OnSearchChange = onSearchChange
          OnSelect = onSelect
          OnOpen = onOpen
          OnCancel = onCancel }

    let restoreFailed
        (rowFor: Project -> ProjectRow)
        (failedContext: BoardContext)
        (projects: Project list)
        (onSearchChange: string -> unit)
        (onSelect: ProjectId -> unit)
        (onOpen: ProjectId -> unit)
        (onCancel: unit -> unit)
        : ProjectSelectionModel =
        let rows = rowsFor rowFor (Some failedContext.ProjectId) projects

        { SiteId = failedContext.SiteId
          Variant = ContextRestoreFailed
          Rows = rows
          SearchQuery = ""
          SelectedProjectId = initialRestoreFailedSelection failedContext.ProjectId rows
          OnSearchChange = onSearchChange
          OnSelect = onSelect
          OnOpen = onOpen
          OnCancel = onCancel }

    /// Real production projection from the domain navigation model: `Ready`
    /// has no selection to show, `InitialProjectSelection` needs the first
    /// start and `RestoreFailed` needs the highlighted restore variant.
    let fromModel
        (siteId: SiteId)
        (rowFor: Project -> ProjectRow)
        (onSearchChange: string -> unit)
        (onSelect: ProjectId -> unit)
        (onOpen: ProjectId -> unit)
        (onCancel: unit -> unit)
        (model: NavigationModel)
        : ProjectSelectionModel option =
        match model.State with
        | InitialProjectSelection ->
            Some(firstStart siteId rowFor model.Data.Projects onSearchChange onSelect onOpen onCancel)
        | RestoreFailed(context, projects) ->
            Some(restoreFailed rowFor context projects onSearchChange onSelect onOpen onCancel)
        | Ready _ -> None

    /// Applies a new search-box text to the model. Filtering itself is the
    /// separate pure projection `visibleRows`; this only records the query a
    /// host observed via `OnSearchChange` so the next render can reflect it,
    /// without any hidden mutable state inside the view.
    let withSearchQuery (query: string) (model: ProjectSelectionModel) : ProjectSelectionModel =
        { model with SearchQuery = query }

    let private matchesQuery (query: string) (row: ProjectRow) =
        if String.IsNullOrEmpty query then
            true
        else
            let contains (value: string) =
                value.IndexOf(query, StringComparison.InvariantCultureIgnoreCase) >= 0

            contains row.Name || contains row.Key

    /// Local, culture-invariant, case-insensitive filter over the project
    /// name and key. An empty search query keeps every row.
    let visibleRows (model: ProjectSelectionModel) : ProjectRow list =
        model.Rows |> List.filter (matchesQuery model.SearchQuery)

    /// Selects a row by its stable id. An id absent from `Rows` leaves the
    /// model unchanged, mirroring the confirm-style validation used
    /// throughout the navigation domain.
    let selectProject (projectId: ProjectId) (model: ProjectSelectionModel) : ProjectSelectionModel =
        if model.Rows |> List.exists (fun row -> row.ProjectId = projectId) then
            { model with SelectedProjectId = Some projectId }
        else
            model

    /// The global primary action only ever opens a project once one has
    /// been selected.
    let canOpen (model: ProjectSelectionModel) = model.SelectedProjectId.IsSome

    let selectActionName (isSelected: bool) (row: ProjectRow) =
        let label = if isSelected then selectedActionLabel else selectActionLabel
        $"{label} · {row.Key}"

    let rowAccessibleName (row: ProjectRow) =
        let core = $"{row.Key} · {row.Name} · {row.TypeLabel}"

        if row.IsLastUsed then
            $"{core} · zuletzt verwendet"
        else
            core

    let private rowView scale (isSelected: bool) dispatch (row: ProjectRow) =
        Border.create
            [ Border.minHeight (DisplayScale.layout scale rowMinHeight)
              Border.background (
                  if isSelected then
                      brush Colors.surfaceSelected
                  elif row.IsLastUsed then
                      brush Colors.surfaceHover
                  else
                      brush Colors.surface
              )
              Border.borderBrush (
                  if isSelected then
                      brush Colors.primary
                  elif row.IsLastUsed then
                      brush Colors.borderStrong
                  else
                      brush Colors.border
              )
              Border.borderThickness (Thickness(DisplayScale.layout scale Lines.normal))
              Border.cornerRadius (CornerRadius(DisplayScale.layout scale CornerRadii.md))
              Border.padding (Thickness(DisplayScale.layout scale Spacing.md))
              Border.margin (Thickness(0.0, 0.0, 0.0, DisplayScale.layout scale Spacing.xs))
              Border.tip (rowAccessibleName row)
              Accessibility.name (rowAccessibleName row)
              Border.child (
                  Grid.create
                      [ Grid.columnDefinitions "Auto,*,Auto"
                        Grid.children
                            [ TextBlock.create
                                  [ Grid.column 0
                                    TextBlock.margin (
                                        Thickness(0.0, 0.0, DisplayScale.layout scale Spacing.md, 0.0)
                                    )
                                    TextBlock.fontFamily (FontFamily Typography.issueKey.Family)
                                    TextBlock.fontSize (DisplayScale.font scale Typography.issueKey.Size)
                                    TextBlock.fontWeight (FontWeight.SemiBold)
                                    TextBlock.foreground (brush Colors.textPrimary)
                                    TextBlock.verticalAlignment VerticalAlignment.Center
                                    TextBlock.text row.Key ]
                              StackPanel.create
                                  [ Grid.column 1
                                    StackPanel.verticalAlignment VerticalAlignment.Center
                                    StackPanel.spacing (DisplayScale.layout scale Spacing.xxs)
                                    StackPanel.children
                                        [ TextBlock.create
                                              [ TextBlock.fontFamily (FontFamily Typography.body.Family)
                                                TextBlock.fontSize (DisplayScale.font scale Typography.body.Size)
                                                TextBlock.foreground (brush Colors.textPrimary)
                                                TextBlock.text row.Name ]
                                          TextBlock.create
                                              [ TextBlock.fontFamily (FontFamily Typography.caption.Family)
                                                TextBlock.fontSize (DisplayScale.font scale Typography.caption.Size)
                                                TextBlock.foreground (brush Colors.textSecondary)
                                                TextBlock.text row.TypeLabel ] ] ]
                              Button.create
                                  [ Grid.column 2
                                    Button.minHeight (DisplayScale.layout scale HitTarget.minimum)
                                    Button.padding (
                                        Thickness(DisplayScale.layout scale Spacing.md, 0.0)
                                    )
                                    Button.background (
                                        if isSelected then
                                            brush Colors.primary
                                        else
                                            brush Colors.surfaceSubtle
                                    )
                                    Button.foreground (
                                        if isSelected then
                                            brush Colors.surface
                                        else
                                            brush Colors.textPrimary
                                    )
                                    Button.content (if isSelected then selectedActionLabel else selectActionLabel)
                                    Accessibility.name (selectActionName isSelected row)
                                    Button.onClick (fun _ -> dispatch row.ProjectId) ] ] ]
              ) ]

    let viewAt scale (model: ProjectSelectionModel) =
        Border.create
            [ Border.background (brush Colors.surface)
              Border.borderBrush (brush Colors.borderStrong)
              Border.borderThickness (Thickness(DisplayScale.layout scale Lines.normal))
              Border.cornerRadius (CornerRadius(DisplayScale.layout scale CornerRadii.lg))
              Border.padding (Thickness(DisplayScale.layout scale Spacing.xl))
              Border.child (
                  StackPanel.create
                      [ StackPanel.spacing (DisplayScale.layout scale Spacing.md)
                        StackPanel.children
                            [ TextBlock.create
                                  [ TextBlock.fontFamily (FontFamily Typography.componentTitle.Family)
                                    TextBlock.fontSize (DisplayScale.font scale Typography.componentTitle.Size)
                                    TextBlock.fontWeight (FontWeight.SemiBold)
                                    TextBlock.foreground (brush Colors.textPrimary)
                                    TextBlock.text headerText ]
                              TextBlock.create
                                  [ TextBlock.fontFamily (FontFamily Typography.caption.Family)
                                    TextBlock.fontSize (DisplayScale.font scale Typography.caption.Size)
                                    TextBlock.foreground (brush Colors.textSecondary)
                                    TextBlock.text (siteHint model.SiteId) ]
                              TextBlock.create
                                  [ TextBlock.fontFamily (FontFamily Typography.caption.Family)
                                    TextBlock.fontSize (DisplayScale.font scale Typography.caption.Size)
                                    TextBlock.foreground (brush Colors.textSecondary)
                                    TextBlock.text teamManagedScrumHint ]
                              if model.Variant = ContextRestoreFailed then
                                  TextBlock.create
                                      [ TextBlock.fontFamily (FontFamily Typography.bodyStrong.Family)
                                        TextBlock.fontSize (DisplayScale.font scale Typography.body.Size)
                                        TextBlock.foreground (brush Colors.warning)
                                        TextBlock.textWrapping TextWrapping.Wrap
                                        TextBlock.text restoreFailedHint ]
                              TextBox.create
                                  [ TextBox.minHeight (DisplayScale.layout scale searchFieldMinHeight)
                                    TextBox.fontFamily (FontFamily Typography.body.Family)
                                    TextBox.fontSize (DisplayScale.font scale Typography.body.Size)
                                    TextBox.watermark searchFieldName
                                    TextBox.text model.SearchQuery
                                    TextBox.tip searchFieldName
                                    Accessibility.name searchFieldName
                                    TextBox.onTextChanged model.OnSearchChange ]
                              for row in visibleRows model do
                                  rowView scale (model.SelectedProjectId = Some row.ProjectId) model.OnSelect row
                              StackPanel.create
                                  [ StackPanel.orientation Orientation.Horizontal
                                    StackPanel.horizontalAlignment HorizontalAlignment.Right
                                    StackPanel.spacing (DisplayScale.layout scale Spacing.sm)
                                    StackPanel.children
                                        [ Button.create
                                              [ Button.minHeight (DisplayScale.layout scale HitTarget.minimum)
                                                Button.padding (
                                                    Thickness(DisplayScale.layout scale Spacing.md, 0.0)
                                                )
                                                Button.content cancelActionLabel
                                                Accessibility.name cancelActionLabel
                                                Button.onClick (fun _ -> model.OnCancel()) ]
                                          Button.create
                                              [ Button.minHeight (DisplayScale.layout scale HitTarget.minimum)
                                                Button.padding (
                                                    Thickness(DisplayScale.layout scale Spacing.md, 0.0)
                                                )
                                                Button.background (brush Colors.primary)
                                                Button.foreground (brush Colors.surface)
                                                Button.content openProjectActionLabel
                                                Button.isEnabled (canOpen model)
                                                Accessibility.name openProjectActionLabel
                                                Button.onClick (fun _ ->
                                                    match model.SelectedProjectId with
                                                    | Some projectId -> model.OnOpen projectId
                                                    | None -> ()) ] ] ] ] ]
              ) ]

    let view model = viewAt DisplayScale.normal model
