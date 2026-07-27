namespace JiraBoard.Domain

/// A Jira-configurable issue type with its stable id, display name, normalized
/// `HierarchyLevel` and subtask marker. The type name is representation only and
/// must never branch the domain (see domain-glossary.md).
type IssueType =
    { Id: string
      Name: string
      HierarchyLevel: int
      IsSubtask: bool }

/// Normalized hierarchy classification. It is mapped from Jira metadata, never
/// guessed from names: level 0 is a standard issue (one swimlane each), anything
/// above level 0 is a parent (hidden from board and replay), and the subtask
/// marker places an issue below level 0 inside its parent swimlane.
type WorkItemLevel =
    | ParentLevel
    | StandardLevel
    | SubtaskLevel

/// Pure hierarchy classification. It is auto-opened with the namespace so that
/// `classify` is available wherever the domain is opened.
[<AutoOpen>]
module IssueHierarchy =
    /// Classifies an `IssueType` into its `WorkItemLevel` purely from metadata.
    /// The subtask marker dominates, so a subtask stays inside its swimlane even
    /// if Jira reports a non-zero level. Story, Bug, Task and any custom standard
    /// type with level 0 therefore share the identical standard swimlane rule.
    let classify (issueType: IssueType) : WorkItemLevel =
        if issueType.IsSubtask then SubtaskLevel
        elif issueType.HierarchyLevel > 0 then ParentLevel
        else StandardLevel
