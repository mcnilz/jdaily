module JiraBoard.Tests.BoardEventOrderTests

open System
open System.Globalization
open System.Threading
open Xunit
open JiraBoard.Domain

// Deterministic event ordering is a pure domain contract (see product-backlog.md
// item DOM-006 and domain-glossary.md). Given the same events it must produce
// the same sequence independent of the input order and the current culture.
// The ordering cascade is: the UTC timestamp first, then the issue's captured
// `BoardOrdinal` (issues without an ordinal sort after ranked ones, mirroring
// the board-order contract, so `BoardOrdinal` wins before the readable key can
// ever matter), then a fixed event-kind order, then the source order
// (`JiraHistory.itemIndex` keeps duplicate deliveries distinguishable), and only
// as the last deterministic anchor the `BoardEventId`.

let private utc year month day hour minute =
    DateTimeOffset(year, month, day, hour, minute, 0, TimeSpan.Zero)

/// A board event with the given identity, issue, timestamp, source and kind.
let private event id issueId occurredAt source kind =
    { EventId = BoardEventId id
      IssueId = IssueId issueId
      OccurredAtUtc = occurredAt
      Source = source
      Kind = kind }

/// The default status change used where a test does not care about the kind.
let private statusChange = StatusChanged(StatusId "3", StatusId "4")

/// Maps a small demo board so that "10001" comes before "10002" on the board and
/// an unknown issue has no captured ordinal.
let private ordinalOf (IssueId issueId) =
    match issueId with
    | "10001" -> Some(BoardOrdinal 0L)
    | "10002" -> Some(BoardOrdinal 1L)
    | _ -> None

let private ids events =
    events |> List.map (fun e -> e.EventId)

[<Fact>]
let ``events are ordered by their utc timestamp first`` () =
    let earlier = event "b" "10001" (utc 2026 7 27 9 0) (JiraHistory 0) statusChange
    let later = event "a" "10001" (utc 2026 7 27 10 0) (JiraHistory 0) statusChange

    let ordered = orderBoardEvents ordinalOf [ later; earlier ]

    Assert.Equal<BoardEventId list>([ BoardEventId "b"; BoardEventId "a" ], ids ordered)

[<Fact>]
let ``timestamps compare as instants regardless of their offset`` () =
    // Same instant expressed in different offsets must be treated as equal in
    // time, so the later cascade steps decide, not the wall-clock components.
    let plusTwo =
        event "z" "10001" (DateTimeOffset(2026, 7, 27, 11, 0, 0, TimeSpan.FromHours 2.0)) (JiraHistory 0) statusChange

    let utcSameInstant =
        event "a" "10001" (DateTimeOffset(2026, 7, 27, 9, 0, 0, TimeSpan.Zero)) (JiraHistory 1) statusChange

    let ordered = orderBoardEvents ordinalOf [ plusTwo; utcSameInstant ]

    // Equal instant -> the source order (itemIndex 0 before 1) decides.
    Assert.Equal<BoardEventId list>([ BoardEventId "z"; BoardEventId "a" ], ids ordered)

[<Fact>]
let ``equal timestamps fall back to the issue board ordinal`` () =
    let sameTime = utc 2026 7 27 9 0
    // Board ordinal of 10001 (0) is before 10002 (1); ids intentionally disagree.
    let onLaterIssue = event "a" "10002" sameTime (JiraHistory 0) statusChange
    let onEarlierIssue = event "b" "10001" sameTime (JiraHistory 0) statusChange

    let ordered = orderBoardEvents ordinalOf [ onLaterIssue; onEarlierIssue ]

    Assert.Equal<BoardEventId list>([ BoardEventId "b"; BoardEventId "a" ], ids ordered)

[<Fact>]
let ``an issue without a board ordinal sorts after issues that have one`` () =
    let sameTime = utc 2026 7 27 9 0
    let unknownIssue = event "a" "99999" sameTime (JiraHistory 0) statusChange
    let rankedIssue = event "b" "10002" sameTime (JiraHistory 0) statusChange

    let ordered = orderBoardEvents ordinalOf [ unknownIssue; rankedIssue ]

    Assert.Equal<BoardEventId list>([ BoardEventId "b"; BoardEventId "a" ], ids ordered)

[<Fact>]
let ``equal time and ordinal fall back to a fixed event kind order`` () =
    let sameTime = utc 2026 7 27 9 0
    // All on the same issue and time; only the kind differs. The expected order
    // is Status, Assignee, Label, Comment, Commit.
    let commit = event "commit" "10001" sameTime (JiraHistory 0) (CommitLinked "abc")
    let comment = event "comment" "10001" sameTime (JiraHistory 0) CommentAdded
    let label = event "label" "10001" sameTime (JiraHistory 0) (LabelChanged(LabelAdded "x"))
    let assignee = event "assignee" "10001" sameTime (JiraHistory 0) (AssigneeChanged(Some "alice"))
    let status = event "status" "10001" sameTime (JiraHistory 0) statusChange

    let ordered = orderBoardEvents ordinalOf [ commit; comment; label; assignee; status ]

    Assert.Equal<BoardEventId list>(
        [ BoardEventId "status"
          BoardEventId "assignee"
          BoardEventId "label"
          BoardEventId "comment"
          BoardEventId "commit" ],
        ids ordered
    )

[<Fact>]
let ``equal time ordinal and kind fall back to the source order`` () =
    let sameTime = utc 2026 7 27 9 0
    // Same issue, time and kind; the source order decides: history items by
    // their item index, then comment, then development information.
    let dev = event "dev" "10001" sameTime DevelopmentInformation statusChange
    let comment = event "comment" "10001" sameTime JiraComment statusChange
    let historyOne = event "h1" "10001" sameTime (JiraHistory 1) statusChange
    let historyZero = event "h0" "10001" sameTime (JiraHistory 0) statusChange

    let ordered = orderBoardEvents ordinalOf [ dev; comment; historyOne; historyZero ]

    Assert.Equal<BoardEventId list>(
        [ BoardEventId "h0"; BoardEventId "h1"; BoardEventId "comment"; BoardEventId "dev" ],
        ids ordered
    )

[<Fact>]
let ``the event id is the last deterministic anchor`` () =
    let sameTime = utc 2026 7 27 9 0
    // Everything is equal except the event id.
    let second = event "e2" "10001" sameTime (JiraHistory 0) statusChange
    let first = event "e1" "10001" sameTime (JiraHistory 0) statusChange

    let ordered = orderBoardEvents ordinalOf [ second; first ]

    Assert.Equal<BoardEventId list>([ BoardEventId "e1"; BoardEventId "e2" ], ids ordered)

[<Fact>]
let ``ordering is independent of the input permutation`` () =
    let e1 = event "e1" "10001" (utc 2026 7 27 9 0) (JiraHistory 0) statusChange
    let e2 = event "e2" "10002" (utc 2026 7 27 9 0) (JiraHistory 0) statusChange
    let e3 = event "e3" "10001" (utc 2026 7 27 10 0) (JiraHistory 0) statusChange

    let expected = orderBoardEvents ordinalOf [ e1; e2; e3 ] |> ids

    let permutations =
        [ [ e3; e2; e1 ]; [ e2; e1; e3 ]; [ e1; e3; e2 ]; [ e3; e1; e2 ]; [ e2; e3; e1 ] ]

    for permutation in permutations do
        Assert.Equal<BoardEventId list>(expected, orderBoardEvents ordinalOf permutation |> ids)

[<Fact>]
let ``ordering is independent of the current culture`` () =
    let sameTime = utc 2026 7 27 9 0
    // Keys and labels that sort differently under a Turkish culture ("i"/"I")
    // must not change the ordinal, culture-invariant result.
    let events =
        [ event "IB" "10001" sameTime (JiraHistory 1) statusChange
          event "ia" "10001" sameTime (JiraHistory 0) statusChange ]

    let orderUnder (cultureName: string) =
        let original = Thread.CurrentThread.CurrentCulture

        try
            Thread.CurrentThread.CurrentCulture <- CultureInfo(cultureName)
            orderBoardEvents ordinalOf events |> ids
        finally
            Thread.CurrentThread.CurrentCulture <- original

    let invariant = orderUnder ""
    let turkish = orderUnder "tr-TR"

    Assert.Equal<BoardEventId list>(invariant, turkish)
    // The source order still decides: itemIndex 0 ("ia") before 1 ("IB").
    Assert.Equal<BoardEventId list>([ BoardEventId "ia"; BoardEventId "IB" ], invariant)

[<Fact>]
let ``ordering an empty sequence yields the empty sequence`` () =
    Assert.Equal<BoardEventId list>([], orderBoardEvents ordinalOf [] |> ids)
