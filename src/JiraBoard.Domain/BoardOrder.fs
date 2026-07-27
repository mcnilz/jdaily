namespace JiraBoard.Domain

/// The dynamically discovered, Jira-provided comparable ranking feature of a
/// board. It is never interpreted numerically nor produced locally, and a fixed
/// `customfield_*` id is forbidden (see domain-glossary.md). It is modeled as an
/// opaque comparable value: only relative ordering matters, not its content.
type JiraRank = JiraRank of string

/// The monotonically assigned position in the unchanged API order across all
/// pages. It holds within the same loaded board revision (or its snapshot) and
/// is not a globally durable rank.
type BoardOrdinal = BoardOrdinal of int64

/// A single board position with the inputs needed to resolve its board order:
/// its readable `IssueKey`, the optional verified `JiraRank` and the captured
/// `BoardOrdinal`. The key is representation and only the last emergency anchor.
type BoardPosition =
    { IssueKey: IssueKey
      JiraRank: JiraRank option
      BoardOrdinal: BoardOrdinal }

/// Pure board-order contracts. Auto-opened with the namespace so the functions
/// are available wherever the domain is opened.
[<AutoOpen>]
module BoardOrder =
    /// The ordering key of a board position. Positions without a `JiraRank` sort
    /// after ranked ones, so a verified rank always wins; ties then fall back to
    /// `BoardOrdinal` and, only when both are equal, to the readable key as the
    /// last deterministic anchor (decision 2026-07-20).
    let private orderKey (position: BoardPosition) =
        let rankKey =
            match position.JiraRank with
            | Some(JiraRank rank) -> (0, rank)
            | None -> (1, "")

        let (BoardOrdinal ordinal) = position.BoardOrdinal
        let (IssueKey key) = position.IssueKey
        (rankKey, ordinal, key)

    /// Resolves the board order of the given positions. The sort is stable, so
    /// fully equal positions keep their input order. This is the `ResolvedBoardOrder`
    /// cascade: verified `JiraRank`, then `BoardOrdinal`, then `IssueKey`.
    let resolveBoardOrder (positions: BoardPosition list) : BoardPosition list =
        positions |> List.sortBy orderKey

    /// Produces a `StableSubsequence`: it removes the elements that fail the
    /// predicate without ever changing the relative order of those retained.
    let stableSubsequence (keep: 'a -> bool) (ordered: 'a list) : 'a list =
        ordered |> List.filter keep
