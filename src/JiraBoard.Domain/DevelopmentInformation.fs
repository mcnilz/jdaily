namespace JiraBoard.Domain

/// A kind of Development Information that Jira may officially provide for an
/// issue. The domain does not know the Jira payload that established it.
type DevelopmentInfoKind =
    | Commit
    | Branch
    | PullRequest

/// The explicitly observed Development Information capability. Unavailable is
/// a normal state: board functionality continues without Development
/// Information.
type DevelopmentInfoCapability =
    | Unavailable
    | JiraProvided of supportedKinds: Set<DevelopmentInfoKind>

/// The small domain port for obtaining the currently available Development
/// Information capability. Implementations may use Jira transport and
/// credentials, but neither crosses this boundary.
type DevelopmentInfoCapabilityPort = {
    GetCapability: unit -> DevelopmentInfoCapability
}