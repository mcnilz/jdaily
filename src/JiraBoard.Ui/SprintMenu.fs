namespace JiraBoard.Ui

open Avalonia
open Avalonia.Controls
open Avalonia.Controls.Primitives
open Avalonia.FuncUI.DSL
open Avalonia.Layout
open Avalonia.Media
open JiraBoard.Domain

// VS-001: `SprintMenu` covers "Alle aktiven Sprints" and a single confirmed
// active sprint (ui-design-specification.md, section "Sprint-Menü"). Future
// and closed sprints never reach `OfflineNavData.ActiveSprints`, so this menu
// never needs to filter them out itself. The delivered order of
// `ActiveSprints` is already the stable source order; this menu never
// re-sorts it and never introduces a `StartDate`.

type SprintMenuItemModel =
    { Scope: SprintScope
      Label: string
      IsSelected: bool }

type SprintMenuModel =
    { Items: SprintMenuItemModel list
      OnSelect: SprintScope -> unit }

[<RequireQualifiedAccess>]
module SprintMenu =
    let private brush (color: JiraBoard.Ui.Color) =
        SolidColorBrush(Avalonia.Media.Color.Parse color.Hex) :> IBrush

    let allActiveSprintsLabel = "Alle aktiven Sprints"

    /// Minimum menu-row height (ui-design-specification.md: "Menüzeile
    /// mindestens 32 DIPs hoch").
    let itemMinHeight = HitTarget.minimum

    /// `AllActiveSprints` is fixed at the first position, followed by the
    /// active sprints in the exact order they were delivered. If the
    /// confirmed scope no longer matches any delivered sprint, the menu
    /// falls back to `AllActiveSprints` being selected instead of leaving no
    /// row selected.
    let items (activeSprints: ActiveSprint list) (currentScope: SprintScope) : SprintMenuItemModel list =
        let scopeIsKnown =
            match currentScope with
            | AllActiveSprints -> true
            | ActiveSprint sprintId ->
                activeSprints |> List.exists (fun sprint -> sprint.SprintId = sprintId)

        let effectiveScope = if scopeIsKnown then currentScope else AllActiveSprints

        let allItem =
            { Scope = AllActiveSprints
              Label = allActiveSprintsLabel
              IsSelected = effectiveScope = AllActiveSprints }

        let sprintItems =
            activeSprints
            |> List.map (fun sprint ->
                { Scope = ActiveSprint sprint.SprintId
                  Label = sprint.Name
                  IsSelected = effectiveScope = ActiveSprint sprint.SprintId })

        allItem :: sprintItems

    let build (activeSprints: ActiveSprint list) (currentScope: SprintScope) (onSelect: SprintScope -> unit) : SprintMenuModel =
        { Items = items activeSprints currentScope
          OnSelect = onSelect }

    /// Real production projection from the domain navigation model: only the
    /// `Ready` state has a confirmed board with a sprint scope to show.
    let fromModel (onSelect: SprintScope -> unit) (model: NavigationModel) : SprintMenuModel option =
        match model.State with
        | Ready(context, _) ->
            let activeSprints =
                model.Data.ActiveSprints
                |> Map.tryFind context.BoardId
                |> Option.defaultValue []

            Some(build activeSprints context.SprintScope onSelect)
        | InitialProjectSelection
        | RestoreFailed _ -> None

    /// The selected state is exposed in the accessible name in addition to
    /// the checkmark/background so automation never depends on color alone.
    let accessibleName (item: SprintMenuItemModel) =
        if item.IsSelected then
            $"{item.Label} · ausgewählt"
        else
            item.Label

    let private itemView scale dispatch (item: SprintMenuItemModel) =
        ToggleButton.create
            [ ToggleButton.minHeight (DisplayScale.layout scale itemMinHeight)
              ToggleButton.horizontalContentAlignment HorizontalAlignment.Left
              ToggleButton.horizontalAlignment HorizontalAlignment.Stretch
              ToggleButton.padding (
                  Thickness(DisplayScale.layout scale Spacing.md, DisplayScale.layout scale Spacing.sm)
              )
              ToggleButton.background (
                  if item.IsSelected then
                      brush Colors.surfaceSelected
                  else
                      brush Colors.surface
              )
              ToggleButton.foreground (brush Colors.textPrimary)
              ToggleButton.isChecked item.IsSelected
              ToggleButton.tip (accessibleName item)
              Accessibility.name (accessibleName item)
              ToggleButton.content (
                  StackPanel.create
                      [ StackPanel.orientation Orientation.Horizontal
                        StackPanel.spacing (DisplayScale.layout scale Spacing.sm)
                        StackPanel.children
                            [ TextBlock.create
                                  [ TextBlock.width (DisplayScale.layout scale Spacing.lg)
                                    TextBlock.fontFamily (FontFamily Typography.body.Family)
                                    TextBlock.fontSize (DisplayScale.font scale Typography.body.Size)
                                    TextBlock.foreground (brush Colors.primary)
                                    TextBlock.text (if item.IsSelected then "✓" else "") ]
                              TextBlock.create
                                  [ TextBlock.fontFamily (FontFamily Typography.body.Family)
                                    TextBlock.fontSize (DisplayScale.font scale Typography.body.Size)
                                    TextBlock.foreground (brush Colors.textPrimary)
                                    TextBlock.text item.Label ] ] ]
              )
              ToggleButton.onClick (fun _ -> dispatch item.Scope) ]

    let viewAt scale (model: SprintMenuModel) =
        StackPanel.create
            [ StackPanel.spacing (DisplayScale.layout scale Spacing.xxs)
              StackPanel.children [ for item in model.Items -> itemView scale model.OnSelect item ] ]

    let view model = viewAt DisplayScale.normal model
