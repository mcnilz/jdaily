namespace JiraBoard.Domain

/// A project reachable in the currently loaded offline navigation data. The
/// domain project carries no key or project type; those are a UI-side
/// projection (see `ProjectRow` at the call sites).
type Project = { ProjectId: ProjectId; Name: string }

/// A board reachable in the currently loaded offline navigation data, scoped
/// to its owning project via `OfflineNavData.Boards`.
type Board = { BoardId: BoardId; Name: string }

/// An active sprint delivered for a board; future and closed sprints never
/// appear here (see domain-glossary.md).
type ActiveSprint = { SprintId: SprintId; Name: string }

/// The offline navigation slice for exactly one active site. `SiteId` is the
/// active site identity: a stored `BoardContext` for a different site can
/// never be restored against this data (see domain-glossary.md, site isolation).
type OfflineNavData = {
    SiteId: SiteId
    Projects: Project list
    Boards: Map<ProjectId, Board list>
    ActiveSprints: Map<BoardId, ActiveSprint list>
}

/// A hint surfaced alongside a `Ready` state when the stored context needed
/// an automatic adjustment while restoring.
type NavigationHint =
    | InactiveSprintFallback

/// The navigation model's state machine: no context yet, a stored context
/// that could not be restored, or a confirmed, restorable context.
type NavigationState =
    | InitialProjectSelection
    | RestoreFailed of failedContext: BoardContext * availableProjects: Project list
    | Ready of confirmedContext: BoardContext * hint: NavigationHint option

/// The full navigation model: current state, a monotonic generation counter
/// used for change detection, and the offline data it was derived from.
type NavigationModel = {
    State: NavigationState
    Generation: int
    Data: OfflineNavData
}

/// Pure state transitions for the navigation model. No serialization, no
/// live effects; identity is only ever compared via the stable ids above.
module Navigation =
    /// Restores a stored context against the currently loaded offline data.
    /// A different active site, an unknown project or an unknown board for
    /// the project all fail the restore; an unknown active sprint instead
    /// falls back to `AllActiveSprints` with a hint.
    let init (storedContext: BoardContext option) (data: OfflineNavData) : NavigationModel =
        let state =
            match storedContext with
            | None -> InitialProjectSelection
            | Some ctx ->
                let siteMatches = ctx.SiteId = data.SiteId
                let projectExists = data.Projects |> List.exists (fun p -> p.ProjectId = ctx.ProjectId)
                let boardsForProject = data.Boards |> Map.tryFind ctx.ProjectId |> Option.defaultValue []
                let boardExists = boardsForProject |> List.exists (fun b -> b.BoardId = ctx.BoardId)
                if not siteMatches || not projectExists || not boardExists then
                    RestoreFailed(ctx, data.Projects)
                else
                    match ctx.SprintScope with
                    | AllActiveSprints -> Ready(ctx, None)
                    | ActiveSprint sId ->
                        let activeSprints = data.ActiveSprints |> Map.tryFind ctx.BoardId |> Option.defaultValue []
                        let sprintIsActive = activeSprints |> List.exists (fun s -> s.SprintId = sId)
                        if sprintIsActive then
                            Ready(ctx, None)
                        else
                            Ready({ ctx with SprintScope = AllActiveSprints }, Some InactiveSprintFallback)

        { State = state; Generation = 1; Data = data }

    /// Confirms a project and board selection against the model's active
    /// site data (`model.Data.SiteId`, never a caller-supplied site). An
    /// unknown project or a board id not owned by the project leaves the
    /// model unchanged; a valid selection starts `AllActiveSprints`.
    let confirmBoard (projectId: ProjectId) (boardId: BoardId) (model: NavigationModel) : NavigationModel =
        let projectExists = model.Data.Projects |> List.exists (fun p -> p.ProjectId = projectId)
        let boardsForProject = model.Data.Boards |> Map.tryFind projectId |> Option.defaultValue []
        let boardExists = boardsForProject |> List.exists (fun b -> b.BoardId = boardId)

        if not projectExists || not boardExists then
            model
        else
            let newContext = {
                SiteId = model.Data.SiteId
                ProjectId = projectId
                BoardId = boardId
                SprintScope = AllActiveSprints
            }
            { model with
                State = Ready(newContext, None)
                Generation = model.Generation + 1 }

    /// Confirms an active sprint scope for the already confirmed board. An
    /// unknown or inactive sprint leaves the model unchanged.
    let confirmSprint (sprintId: SprintId) (model: NavigationModel) : NavigationModel =
        match model.State with
        | Ready(ctx, _) ->
            let activeSprints = model.Data.ActiveSprints |> Map.tryFind ctx.BoardId |> Option.defaultValue []
            let sprintIsActive = activeSprints |> List.exists (fun s -> s.SprintId = sprintId)
            if sprintIsActive then
                { model with
                    State = Ready({ ctx with SprintScope = ActiveSprint sprintId }, None)
                    Generation = model.Generation + 1 }
            else
                model
        | _ -> model
