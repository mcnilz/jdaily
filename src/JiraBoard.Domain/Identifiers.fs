namespace JiraBoard.Domain

// Strong identities. Each concept has its own type so that an identity of one
// concept cannot be substituted for another. Names and localized texts are
// representation only; identity and domain branching use these stable IDs
// (see domain-glossary.md).

/// Stable identity of a Jira-Cloud site. A site URL or display name is not identity.
type SiteId = SiteId of string

/// Stable identity of a Jira project. The project name is not identity.
type ProjectId = ProjectId of string

/// Stable identity of a Scrum board.
type BoardId = BoardId of int64

/// Stable identity of a Jira sprint.
type SprintId = SprintId of int64

/// Stable identity of a Jira issue across all relevant hierarchy levels.
type IssueId = IssueId of string

/// Readable key such as `APP-142`. It is displayed and can serve as a last
/// deterministic emergency fallback, but never replaces `IssueId`.
type IssueKey = IssueKey of string

/// Stable identity of a Jira workflow status. The status name is representation
/// only; mapping and identity use this stable id (see domain-glossary.md).
type StatusId = StatusId of string

/// Stable identity of a normalized board event. A timestamp alone is not an
/// event identity (see domain-glossary.md).
type BoardEventId = BoardEventId of string
