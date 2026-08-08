namespace JiraBoard.Domain

/// The traceable origin of a normalized board event. It records where the change
/// was observed without leaking raw Jira changelog items into the domain (see
/// domain-glossary.md).
type BoardEventSource =
    /// A Jira history entry, identified by its stable item index inside the
    /// changelog so duplicate deliveries stay distinguishable.
    | JiraHistory of itemIndex: int
    /// A Jira comment.
    | JiraComment
    /// Officially readable Jira Development Information (commits, branches, PRs).
    | DevelopmentInformation

/// A single label change. Adding and removing are distinct so a replay can show
/// the correct indicator.
type LabelChange =
    | LabelAdded of label: string
    | LabelRemoved of label: string

/// The kind of a normalized board event. Exactly one kind describes each event.
/// Impossible combinations are excluded by the union: every kind carries only
/// the data it needs. `StatusChanged` is an observed result (from one status to
/// another), never a command sent to Jira.
type BoardEventKind =
    /// A status transition observed on the board, from one status to another.
    | StatusChanged of fromStatus: StatusId * toStatus: StatusId
    /// The assignee changed; `None` represents an unassigned issue.
    | AssigneeChanged of assignee: string option
    /// A label was added or removed.
    | LabelChanged of change: LabelChange
    /// A new comment was added.
    | CommentAdded
    /// A Git commit was linked to the issue via Development Information. Boards
    /// without any such events are still fully functional.
    | CommitLinked of commitHash: string

/// A normalized business change with a stable identity, its issue, a UTC
/// timestamp, a traceable source and exactly one kind. The domain never sees
/// raw Jira changelog items.
type BoardEvent =
    { EventId: BoardEventId
      IssueId: IssueId
      OccurredAtUtc: System.DateTimeOffset
      Source: BoardEventSource
      Kind: BoardEventKind }

/// Pure, deterministic ordering of normalized board events. Auto-opened with the
/// namespace so the functions are available wherever the domain is opened.
[<AutoOpen>]
module BoardEventOrder =
    /// The fixed rank of an event kind. It is a deliberate, documented order
    /// (Status, Assignee, Label, Comment, Commit) so that events sharing the
    /// same instant, issue and source still sort deterministically and never
    /// depend on culture or input order.
    let private kindRank (kind: BoardEventKind) =
        match kind with
        | StatusChanged _ -> 0
        | AssigneeChanged _ -> 1
        | LabelChanged _ -> 2
        | CommentAdded -> 3
        | CommitLinked _ -> 4

    /// The fixed rank of an event source. History entries keep their changelog
    /// item index so duplicate deliveries stay distinguishable, then comments,
    /// then Development Information.
    let private sourceRank (source: BoardEventSource) =
        match source with
        | JiraHistory itemIndex -> (0, itemIndex)
        | JiraComment -> (1, 0)
        | DevelopmentInformation -> (2, 0)

    /// The deterministic ordering key of a single event. `ordinalOf` resolves an
    /// issue's captured `BoardOrdinal`; issues without one sort after those that
    /// have it, mirroring the board-order cascade so `BoardOrdinal` always wins
    /// before the last `BoardEventId` anchor is ever needed. The timestamp is
    /// compared as a UTC instant, so an equal moment in a different offset is
    /// treated as equal in time. The readable issue key is intentionally not
    /// part of this key, keeping ordering culture-invariant.
    let private orderKey (ordinalOf: IssueId -> BoardOrdinal option) (event: BoardEvent) =
        let instant = event.OccurredAtUtc.UtcTicks

        let ordinalKey =
            match ordinalOf event.IssueId with
            | Some(BoardOrdinal ordinal) -> (0L, ordinal)
            | None -> (1L, 0L)

        let (BoardEventId eventId) = event.EventId
        (instant, ordinalKey, kindRank event.Kind, sourceRank event.Source, eventId)

    /// Orders board events into their deterministic replay sequence. The sort is
    /// stable and depends only on the events and the resolved `BoardOrdinal`, so
    /// the same input yields the same sequence regardless of the incoming order
    /// or the current culture. The cascade is: UTC instant, then issue
    /// `BoardOrdinal`, then a fixed event-kind order, then the source order
    /// (`JiraHistory` item index first), and finally the `BoardEventId`.
    let orderBoardEvents (ordinalOf: IssueId -> BoardOrdinal option) (events: BoardEvent list) : BoardEvent list =
        events |> List.sortBy (orderKey ordinalOf)

/// Configures whether inverse status transitions are suppressed from a replay.
/// `Enabled` values are created through `StatusBounceWindow.create`; the
/// normalization function nevertheless treats malformed values as the default.
type StatusBounceWindow =
    | Disabled
    | Enabled of minutes: int

[<RequireQualifiedAccess>]
module StatusBounceWindow =
    let defaultValue = Enabled 5

    let create minutes =
        if minutes >= 1 && minutes <= 30 then
            Enabled minutes
        else
            defaultValue

    let minutes window =
        match window with
        | Disabled -> None
        | Enabled value when value >= 1 && value <= 30 -> Some value
        | Enabled _ -> Some 5

/// Immutable replay-start snapshot of the noise policy for one run.
type ReplayNoisePolicy = { StatusBounceWindow: StatusBounceWindow }

/// Pure replay-only projection. It never mutates or rewrites canonical events.
/// Input must already have the deterministic replay order from `orderBoardEvents`.
[<AutoOpen>]
module ReplayNormalization =
    let private areInverseStatusChanges first second =
        match first.Kind, second.Kind with
        | StatusChanged(firstFrom, firstTo), StatusChanged(secondFrom, secondTo) ->
            firstFrom = secondTo && firstTo = secondFrom
        | _ -> false

    let private isBounceWithin minutes first second =
        let elapsed = second.OccurredAtUtc - first.OccurredAtUtc
        elapsed >= System.TimeSpan.Zero && elapsed <= System.TimeSpan.FromMinutes(float minutes)

    let private bouncedEventIds minutes events =
        let rec collect statusEvents =
            match statusEvents with
            | first :: second :: remaining when areInverseStatusChanges first second && isBounceWithin minutes first second ->
                first.EventId :: second.EventId :: collect remaining
            | _ :: remaining -> collect remaining
            | [] -> []

        events
        |> List.choose (fun event ->
            match event.Kind with
            | StatusChanged _ -> Some event
            | _ -> None)
        |> List.groupBy (fun event -> event.IssueId)
        |> List.collect (fun (_, statusEvents) -> collect statusEvents)
        |> Set.ofList

    /// Removes only adjacent inverse status transitions for the same issue that
    /// fall within the configured inclusive window. Non-status events remain in
    /// the projected stream, including those between the two transitions.
    let normalizeForReplay (policy: ReplayNoisePolicy) (events: BoardEvent list) : BoardEvent list =
        match StatusBounceWindow.minutes policy.StatusBounceWindow with
        | None -> events
        | Some minutes ->
            let suppressed = bouncedEventIds minutes events
            events |> List.filter (fun event -> not (Set.contains event.EventId suppressed))
