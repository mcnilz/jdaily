module JiraBoard.Tests.IssueHierarchyTests

open Xunit
open JiraBoard.Domain

// Hierarchy classification is derived from Jira metadata (`hierarchyLevel` and
// the subtask marker), never guessed from the type name. Story, Bug, Task and a
// custom standard type share the exact same swimlane rule (see domain-glossary.md).

/// A standard-level issue type (level 0, not a subtask) for the given name.
let private standardType name =
    { Id = "10001"
      Name = name
      HierarchyLevel = 0
      IsSubtask = false }

[<Fact>]
let ``subtask marker maps to subtask level regardless of hierarchy level`` () =
    let subtask =
        { Id = "10100"
          Name = "Sub-task"
          HierarchyLevel = 0
          IsSubtask = true }

    Assert.Equal(SubtaskLevel, classify subtask)

[<Fact>]
let ``hierarchy level zero without subtask marker maps to standard level`` () =
    Assert.Equal(StandardLevel, classify (standardType "Story"))

[<Fact>]
let ``positive hierarchy level maps to parent level`` () =
    let epic =
        { Id = "10200"
          Name = "Epic"
          HierarchyLevel = 1
          IsSubtask = false }

    Assert.Equal(ParentLevel, classify epic)

[<Fact>]
let ``story bug task and custom standard type all yield the same standard level`` () =
    // The type name must not branch the domain: every level-0 non-subtask type
    // produces the identical standard swimlane rule.
    let levels =
        [ "Story"; "Bug"; "Task"; "Verbesserung" ]
        |> List.map (standardType >> classify)

    Assert.All(levels, fun level -> Assert.Equal(StandardLevel, level))

[<Fact>]
let ``subtask marker dominates a positive hierarchy level`` () =
    // If Jira ever reports a subtask marker together with a non-zero level, the
    // subtask marker wins so the issue stays inside its swimlane.
    let oddSubtask =
        { Id = "10300"
          Name = "Sub-task"
          HierarchyLevel = 1
          IsSubtask = true }

    Assert.Equal(SubtaskLevel, classify oddSubtask)
