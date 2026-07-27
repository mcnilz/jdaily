module JiraBoard.Tests.SprintProjectionTests

open Xunit
open JiraBoard.Domain

// Multi-sprint projection is a pure domain contract (see domain-glossary.md,
// line 78 and decision line 168). `AllActiveSprints` unites the issues of all
// active sprints, deduplicated by `IssueId`, and projects them as a stable
// subsequence of the global `ResolvedBoardOrder` (never a concatenation of
// single sprint responses). `ActiveSprint sprintId` filters exactly on that one
// sprint while keeping the global board order.

/// A board issue carrying its stable id, its global board position and the set
/// of active sprints it belongs to. Membership is already normalized (no DTOs).
let private issue id key rank ordinal sprints =
    { IssueId = IssueId id
      Position =
        { IssueKey = IssueKey key
          JiraRank = rank
          BoardOrdinal = BoardOrdinal ordinal }
      Sprints = sprints |> List.map SprintId |> Set.ofList }

let private issueIds projection =
    projection |> List.map (fun (i: SprintBoardIssue) -> i.IssueId)

[<Fact>]
let ``all active sprints deduplicates a multi sprint issue to one global position`` () =
    // APP-2 belongs to both sprints 10 and 20. It must appear exactly once and
    // at its global board position (between APP-1 and APP-3), not twice.
    let issues =
        [ issue "1" "APP-1" (Some(JiraRank "a")) 0L [ 10L ]
          issue "2" "APP-2" (Some(JiraRank "b")) 1L [ 10L; 20L ]
          issue "3" "APP-3" (Some(JiraRank "c")) 2L [ 20L ] ]

    let projected = projectSprintScope AllActiveSprints issues

    Assert.Equal<IssueId list>([ IssueId "1"; IssueId "2"; IssueId "3" ], issueIds projected)

[<Fact>]
let ``all active sprints output is a stable subsequence of the global board order`` () =
    // The input order deliberately disagrees with the global board order (given
    // by JiraRank). The projection must still be in global order, proving it is
    // a stable subsequence and never a concatenation of sprint responses.
    let issues =
        [ issue "3" "APP-3" (Some(JiraRank "c")) 2L [ 20L ]
          issue "1" "APP-1" (Some(JiraRank "a")) 0L [ 10L ]
          issue "2" "APP-2" (Some(JiraRank "b")) 1L [ 10L; 20L ] ]

    let projected = projectSprintScope AllActiveSprints issues

    Assert.Equal<IssueId list>([ IssueId "1"; IssueId "2"; IssueId "3" ], issueIds projected)

[<Fact>]
let ``all active sprints unites disjoint sprints without duplicates`` () =
    let issues =
        [ issue "1" "APP-1" (Some(JiraRank "a")) 0L [ 10L ]
          issue "2" "APP-2" (Some(JiraRank "b")) 1L [ 20L ]
          issue "3" "APP-3" (Some(JiraRank "c")) 2L [ 30L ] ]

    let projected = projectSprintScope AllActiveSprints issues

    Assert.Equal<IssueId list>([ IssueId "1"; IssueId "2"; IssueId "3" ], issueIds projected)

[<Fact>]
let ``all active sprints excludes issues without any sprint membership`` () =
    let issues =
        [ issue "1" "APP-1" (Some(JiraRank "a")) 0L [ 10L ]
          issue "2" "APP-2" (Some(JiraRank "b")) 1L []
          issue "3" "APP-3" (Some(JiraRank "c")) 2L [ 20L ] ]

    let projected = projectSprintScope AllActiveSprints issues

    Assert.Equal<IssueId list>([ IssueId "1"; IssueId "3" ], issueIds projected)

[<Fact>]
let ``active sprint filters exactly and keeps the global board order`` () =
    // Only issues that belong to sprint 10 must appear, in global board order.
    let issues =
        [ issue "3" "APP-3" (Some(JiraRank "c")) 2L [ 20L ]
          issue "1" "APP-1" (Some(JiraRank "a")) 0L [ 10L ]
          issue "2" "APP-2" (Some(JiraRank "b")) 1L [ 10L; 20L ] ]

    let projected = projectSprintScope (ActiveSprint(SprintId 10L)) issues

    Assert.Equal<IssueId list>([ IssueId "1"; IssueId "2" ], issueIds projected)

[<Fact>]
let ``active sprint with no matching issues yields the empty projection`` () =
    let issues =
        [ issue "1" "APP-1" (Some(JiraRank "a")) 0L [ 10L ]
          issue "2" "APP-2" (Some(JiraRank "b")) 1L [ 20L ] ]

    let projected = projectSprintScope (ActiveSprint(SprintId 99L)) issues

    Assert.Equal<IssueId list>([], issueIds projected)

[<Fact>]
let ``empty input yields the empty projection for all active sprints`` () =
    Assert.Equal<IssueId list>([], issueIds (projectSprintScope AllActiveSprints []))

[<Fact>]
let ``empty input yields the empty projection for a single active sprint`` () =
    Assert.Equal<IssueId list>([], issueIds (projectSprintScope (ActiveSprint(SprintId 10L)) []))
