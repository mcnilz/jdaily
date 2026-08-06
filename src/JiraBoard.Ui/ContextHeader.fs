namespace JiraBoard.Ui

open Avalonia
open Avalonia.Controls
open Avalonia.FuncUI.DSL
open Avalonia.Layout
open Avalonia.Media
open JiraBoard.Domain

// VS-001: the confirmed project/board/sprint context is shown as a compact
// header without a modal (ui-design-specification.md, section
// "Anwendungsshell": "Beim normalen Start erscheint kein Auswahlmodal"). The
// header never uses a name-based identity; it is a pure projection of the
// active sprint scope onto its display label.

/// UI-specific projection of a confirmed `BoardContext`. The offline slice
/// only ever shows Team-managed Scrum, so that label is a fixed UI text and
/// not modeled in the domain.
type ContextHeaderModel =
    { ActiveSprints: ActiveSprint list
      Scope: SprintScope }

[<RequireQualifiedAccess>]
module ContextHeader =
    let private brush (color: JiraBoard.Ui.Color) =
        SolidColorBrush(Avalonia.Media.Color.Parse color.Hex) :> IBrush

    let teamManagedScrumLabel = "Team Managed Scrum"

    let allActiveSprintsLabel = "Alle aktiven Sprints"

    /// The sprint-scope label never falls back to a stale name: an unknown
    /// sprint id (e.g. a scope no longer present in `ActiveSprints`) reads as
    /// "Alle aktiven Sprints" instead of guessing a display name.
    let sprintScopeLabel (activeSprints: ActiveSprint list) (scope: SprintScope) =
        match scope with
        | AllActiveSprints -> allActiveSprintsLabel
        | ActiveSprint sprintId ->
            activeSprints
            |> List.tryFind (fun sprint -> sprint.SprintId = sprintId)
            |> Option.map (fun sprint -> $"{sprint.Name} (Active)")
            |> Option.defaultValue allActiveSprintsLabel

    let text (model: ContextHeaderModel) =
        $"{teamManagedScrumLabel} · {sprintScopeLabel model.ActiveSprints model.Scope}"

    /// Real production projection from the domain navigation model: only the
    /// `Ready` state has a confirmed context to show without a modal.
    let fromModel (model: NavigationModel) : ContextHeaderModel option =
        match model.State with
        | Ready(context, _) ->
            let activeSprints =
                model.Data.ActiveSprints
                |> Map.tryFind context.BoardId
                |> Option.defaultValue []

            Some { ActiveSprints = activeSprints; Scope = context.SprintScope }
        | InitialProjectSelection
        | RestoreFailed _ -> None

    let viewAt scale (model: ContextHeaderModel) =
        let label = text model

        Border.create
            [ Border.background (brush Colors.surfaceSubtle)
              Border.borderBrush (brush Colors.border)
              Border.borderThickness (Thickness(0.0, 0.0, 0.0, DisplayScale.layout scale Lines.normal))
              Border.padding (
                  Thickness(DisplayScale.layout scale Spacing.lg, DisplayScale.layout scale Spacing.sm)
              )
              Border.tip label
              Accessibility.name label
              Border.child (
                  TextBlock.create
                      [ TextBlock.fontFamily (FontFamily Typography.bodyStrong.Family)
                        TextBlock.fontSize (DisplayScale.font scale Typography.bodyStrong.Size)
                        TextBlock.fontWeight (FontWeight.SemiBold)
                        TextBlock.foreground (brush Colors.textPrimary)
                        TextBlock.verticalAlignment VerticalAlignment.Center
                        TextBlock.text label ]
              ) ]

    let view model = viewAt DisplayScale.normal model
