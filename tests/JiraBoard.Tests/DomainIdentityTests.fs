module JiraBoard.Tests.DomainIdentityTests

open Xunit
open JiraBoard.Domain

// Strong identities: each identity is its own type so that IDs of different
// concepts cannot be substituted for one another. Names are representation and
// never identity (see domain-glossary.md).

[<Fact>]
let ``distinct identities wrap and unwrap their stable values`` () =
    Assert.Equal(SiteId "acme", SiteId "acme")
    Assert.Equal(ProjectId "PHX", ProjectId "PHX")
    Assert.Equal(BoardId 42L, BoardId 42L)
    Assert.Equal(SprintId 7L, SprintId 7L)
    Assert.Equal(IssueId "10001", IssueId "10001")
    Assert.Equal(IssueKey "APP-142", IssueKey "APP-142")

[<Fact>]
let ``identities with different underlying values are not equal`` () =
    Assert.NotEqual(ProjectId "PHX", ProjectId "PHOENIX")
    Assert.NotEqual(BoardId 1L, BoardId 2L)
    Assert.NotEqual(IssueId "10001", IssueId "10002")

[<Fact>]
let ``IssueKey is representation and never replaces IssueId`` () =
    // Same readable key but distinct stable issue IDs must remain distinct issues.
    let first = IssueId "10001"
    let second = IssueId "10002"
    Assert.NotEqual(first, second)
    // The readable key type is intentionally separate from the identity type.
    let key = IssueKey "APP-142"
    Assert.Equal(IssueKey "APP-142", key)

[<Fact>]
let ``sprint scope models all active sprints and a single active sprint`` () =
    let all = AllActiveSprints
    let single = ActiveSprint(SprintId 7L)
    Assert.Equal(AllActiveSprints, all)
    Assert.Equal(ActiveSprint(SprintId 7L), single)
    Assert.NotEqual(all, single)
    Assert.NotEqual(ActiveSprint(SprintId 7L), ActiveSprint(SprintId 8L))

[<Fact>]
let ``board context combines site project board and sprint scope`` () =
    let context =
        { SiteId = SiteId "acme"
          ProjectId = ProjectId "PHX"
          BoardId = BoardId 42L
          SprintScope = AllActiveSprints }

    Assert.Equal(SiteId "acme", context.SiteId)
    Assert.Equal(ProjectId "PHX", context.ProjectId)
    Assert.Equal(BoardId 42L, context.BoardId)
    Assert.Equal(AllActiveSprints, context.SprintScope)

[<Fact>]
let ``board context isolates settings by site even for equal project and board`` () =
    let onAcme =
        { SiteId = SiteId "acme"
          ProjectId = ProjectId "PHX"
          BoardId = BoardId 42L
          SprintScope = ActiveSprint(SprintId 7L) }

    let onOther =
        { onAcme with SiteId = SiteId "other" }

    Assert.NotEqual(onAcme, onOther)
