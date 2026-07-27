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
