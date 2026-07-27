namespace JiraBoard.Domain

/// The selectable sprint scope of a board. The MVP only offers active sprints;
/// future and closed sprints are out of scope.
type SprintScope =
    | AllActiveSprints
    | ActiveSprint of SprintId

/// The confirmed combination of active site, project, board and sprint scope.
/// Board settings and snapshots are additionally isolated by the active site,
/// so `SiteId` is part of the context identity (see domain-glossary.md).
type BoardContext =
    { SiteId: SiteId
      ProjectId: ProjectId
      BoardId: BoardId
      SprintScope: SprintScope }
