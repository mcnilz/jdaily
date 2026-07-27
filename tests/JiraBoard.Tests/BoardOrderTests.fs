module JiraBoard.Tests.BoardOrderTests

open Xunit
open JiraBoard.Domain

// Board ordering is a pure domain contract (see domain-glossary.md, section
// "Boardprojektion und Reihenfolge"). The resolved order compares by the
// verified `JiraRank` first, then by `BoardOrdinal`, and only when both are
// absent or equal does the `IssueKey` act as the last ordinal emergency anchor
// (decision 2026-07-20). A `StableSubsequence` removes elements without ever
// changing the relative order of the remaining issues.

/// A board position carrying its stable issue key, optional Jira rank and the
/// ordinal captured while reading the unchanged API order.
let private position key rank ordinal =
    { IssueKey = IssueKey key
      JiraRank = rank
      BoardOrdinal = BoardOrdinal ordinal }

let private keys positions =
    positions |> List.map (fun p -> p.IssueKey)

[<Fact>]
let ``resolved order sorts by jira rank when present`` () =
    // Ordinals and keys intentionally disagree with the rank so only the rank
    // can produce the expected order.
    let unordered =
        [ position "APP-3" (Some(JiraRank "c")) 0L
          position "APP-1" (Some(JiraRank "a")) 1L
          position "APP-2" (Some(JiraRank "b")) 2L ]

    let ordered = resolveBoardOrder unordered

    Assert.Equal<IssueKey list>([ IssueKey "APP-1"; IssueKey "APP-2"; IssueKey "APP-3" ], keys ordered)

[<Fact>]
let ``board ordinal breaks ties for equal or missing rank`` () =
    // Same rank for two, none for the third: the captured API ordinal decides.
    let unordered =
        [ position "APP-3" None 2L
          position "APP-1" (Some(JiraRank "a")) 1L
          position "APP-2" (Some(JiraRank "a")) 0L ]

    let ordered = resolveBoardOrder unordered

    Assert.Equal<IssueKey list>([ IssueKey "APP-2"; IssueKey "APP-1"; IssueKey "APP-3" ], keys ordered)

[<Fact>]
let ``issue key is only the last anchor when rank and ordinal are equal`` () =
    // Identical rank and identical ordinal for both: only the readable key can
    // still order them deterministically.
    let unordered =
        [ position "APP-2" (Some(JiraRank "a")) 5L
          position "APP-1" (Some(JiraRank "a")) 5L ]

    let ordered = resolveBoardOrder unordered

    Assert.Equal<IssueKey list>([ IssueKey "APP-1"; IssueKey "APP-2" ], keys ordered)

[<Fact>]
let ``resolved order is stable for already equal positions`` () =
    // Fully equal order keys must preserve the input order (no spurious swaps).
    let first = position "APP-1" (Some(JiraRank "a")) 5L
    let second = { first with IssueKey = IssueKey "APP-1" }
    let ordered = resolveBoardOrder [ first; second ]
    Assert.Equal<IssueKey list>([ IssueKey "APP-1"; IssueKey "APP-1" ], keys ordered)

[<Fact>]
let ``stable subsequence keeps the relative order of retained elements`` () =
    let ordered = [ 1; 2; 3; 4; 5 ]
    let retained = stableSubsequence (fun n -> n % 2 = 1) ordered
    Assert.Equal<int list>([ 1; 3; 5 ], retained)

[<Fact>]
let ``stable subsequence keeping everything is the identity`` () =
    let ordered = [ IssueKey "APP-1"; IssueKey "APP-2"; IssueKey "APP-3" ]
    Assert.Equal<IssueKey list>(ordered, stableSubsequence (fun _ -> true) ordered)

[<Fact>]
let ``stable subsequence removing everything yields the empty sequence`` () =
    Assert.Equal<int list>([], stableSubsequence (fun _ -> false) [ 1; 2; 3 ])
