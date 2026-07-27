namespace JiraBoard.Domain

/// A board issue with its global board position and the set of active sprints it
/// belongs to. Sprint membership is already normalized (no Jira DTOs); the order
/// comes solely from the global `ResolvedBoardOrder`, never from the sprints.
type SprintBoardIssue =
    { IssueId: IssueId
      Position: BoardPosition
      Sprints: Set<SprintId> }

/// Pure sprint-scope projection. Auto-opened with the namespace so the function
/// is available wherever the domain is opened.
[<AutoOpen>]
module SprintProjection =
    /// Removes repeated issue ids while keeping the relative order of the first
    /// occurrence of each id (an order-preserving `StableSubsequence`).
    let private distinctById (issues: SprintBoardIssue list) : SprintBoardIssue list =
        let mutable seen = Set.empty

        issues
        |> stableSubsequence (fun issue ->
            if Set.contains issue.IssueId seen then
                false
            else
                seen <- Set.add issue.IssueId seen
                true)

    /// Orders the issues by the global `ResolvedBoardOrder` without altering any
    /// issue's real `BoardPosition`. It resolves the order over the untouched
    /// positions and maps each resolved position back to its owning issue,
    /// consuming matches so that issues with an identical position stay stable.
    let private byGlobalBoardOrder (issues: SprintBoardIssue list) : SprintBoardIssue list =
        let remaining = System.Collections.Generic.List<SprintBoardIssue>(issues)

        issues
        |> List.map (fun issue -> issue.Position)
        |> resolveBoardOrder
        |> List.map (fun position ->
            let index = remaining.FindIndex(fun issue -> issue.Position = position)
            let issue = remaining.[index]
            remaining.RemoveAt(index)
            issue)

    /// Projects the given sprint scope onto the global board order. The result
    /// is always a `StableSubsequence` of the global `ResolvedBoardOrder`,
    /// deduplicated by `IssueId`. It never concatenates single sprint responses:
    /// the global order is resolved first, then the scope filter and dedup only
    /// remove elements without ever reordering the retained ones.
    let projectSprintScope (scope: SprintScope) (issues: SprintBoardIssue list) : SprintBoardIssue list =
        let inScope (issue: SprintBoardIssue) =
            match scope with
            | AllActiveSprints -> not (Set.isEmpty issue.Sprints)
            | ActiveSprint sprintId -> Set.contains sprintId issue.Sprints

        issues
        |> byGlobalBoardOrder
        |> stableSubsequence inScope
        |> distinctById
