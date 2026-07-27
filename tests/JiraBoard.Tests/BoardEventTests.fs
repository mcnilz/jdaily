module JiraBoard.Tests.BoardEventTests

open Xunit
open JiraBoard.Domain

// The board event model is a pure domain contract (see domain-glossary.md,
// lines 99-102, and the handoff, lines 176-192). A `BoardEvent` is a normalized
// business change carrying a stable `BoardEventId`, an `IssueId`, a UTC
// timestamp, a `BoardEventSource` and exactly one kind. The domain never sees
// raw Jira changelog items. `StatusChanged` is an observed result (from one
// status to another), not a command sent to Jira.

let private utc year month day =
    System.DateTimeOffset(year, month, day, 9, 0, 0, System.TimeSpan.Zero)

/// A board event with the given kind, using stable defaults for the parts that
/// a particular test does not care about.
let private event id issueId source kind =
    { EventId = BoardEventId id
      IssueId = IssueId issueId
      OccurredAtUtc = utc 2026 7 27
      Source = source
      Kind = kind }

[<Fact>]
let ``status changed carries the from and to status ids`` () =
    let e =
        event "e1" "10001" (JiraHistory 0) (StatusChanged(StatusId "3", StatusId "4"))

    match e.Kind with
    | StatusChanged(fromStatus, toStatus) ->
        Assert.Equal(StatusId "3", fromStatus)
        Assert.Equal(StatusId "4", toStatus)
    | other -> Assert.Fail(sprintf "expected StatusChanged, got %A" other)

[<Fact>]
let ``a board event carries its identity issue timestamp and source`` () =
    let e =
        event "e1" "10001" (JiraHistory 2) (StatusChanged(StatusId "3", StatusId "4"))

    Assert.Equal(BoardEventId "e1", e.EventId)
    Assert.Equal(IssueId "10001", e.IssueId)
    Assert.Equal(utc 2026 7 27, e.OccurredAtUtc)
    Assert.Equal(JiraHistory 2, e.Source)

[<Fact>]
let ``assignee change is a distinct kind carrying the new assignee`` () =
    let e = event "e2" "10001" (JiraHistory 1) (AssigneeChanged(Some "alice"))

    match e.Kind with
    | AssigneeChanged assignee -> Assert.Equal(Some "alice", assignee)
    | other -> Assert.Fail(sprintf "expected AssigneeChanged, got %A" other)

[<Fact>]
let ``assignee change represents unassigned as none`` () =
    let e = event "e2" "10001" (JiraHistory 1) (AssigneeChanged None)

    match e.Kind with
    | AssigneeChanged assignee -> Assert.Equal(None, assignee)
    | other -> Assert.Fail(sprintf "expected AssigneeChanged, got %A" other)

[<Fact>]
let ``label change distinguishes adding from removing`` () =
    let added = event "e3" "10001" (JiraHistory 0) (LabelChanged(LabelAdded "blocked"))
    let removed = event "e4" "10001" (JiraHistory 1) (LabelChanged(LabelRemoved "blocked"))

    match added.Kind, removed.Kind with
    | LabelChanged(LabelAdded a), LabelChanged(LabelRemoved r) ->
        Assert.Equal("blocked", a)
        Assert.Equal("blocked", r)
    | _ -> Assert.Fail("expected one added and one removed label change")

[<Fact>]
let ``comment added is sourced from a jira comment`` () =
    let e = event "e5" "10001" JiraComment CommentAdded

    Assert.Equal(JiraComment, e.Source)

    match e.Kind with
    | CommentAdded -> ()
    | other -> Assert.Fail(sprintf "expected CommentAdded, got %A" other)

[<Fact>]
let ``commit linked is sourced from development information`` () =
    let e = event "e6" "10001" DevelopmentInformation (CommitLinked "abc123")

    Assert.Equal(DevelopmentInformation, e.Source)

    match e.Kind with
    | CommitLinked hash -> Assert.Equal("abc123", hash)
    | other -> Assert.Fail(sprintf "expected CommitLinked, got %A" other)

[<Fact>]
let ``events with different ids are different and equal ids are equal`` () =
    let baseKind = StatusChanged(StatusId "3", StatusId "4")
    let first = event "e1" "10001" (JiraHistory 0) baseKind
    let second = event "e2" "10001" (JiraHistory 0) baseKind
    let firstAgain = event "e1" "10001" (JiraHistory 0) baseKind

    Assert.NotEqual(first.EventId, second.EventId)
    Assert.Equal(first.EventId, firstAgain.EventId)
