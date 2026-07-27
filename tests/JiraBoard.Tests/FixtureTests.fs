namespace JiraBoard.Tests

open Xunit
open System.Text.Json
open JiraBoard.Domain

module FixtureTests =

    let allFixtureFiles = [
        "projects-boards.json"
        "sprints.json"
        "board-configuration.json"
        "issues-hierarchy.json"
        "issues-pagination-p1.json"
        "issues-pagination-p2.json"
        "issue-changelog.json"
        "errors.json"
    ]

    [<Fact>]
    let ``Fixtures are valid JSON and anonymized`` () =
        for file in allFixtureFiles do
            let content = Fixture.readResource file
            
            // Structural check
            use doc = JsonDocument.Parse(content)
            Assert.NotNull(doc.RootElement)

            // Basic Anonymization checks
            Assert.DoesNotContain("atlassian.net", content.Replace("https://anonymized.atlassian.net", ""))
            Assert.DoesNotContain("token", content, System.StringComparison.OrdinalIgnoreCase)
            Assert.DoesNotContain("cookie", content, System.StringComparison.OrdinalIgnoreCase)
            Assert.DoesNotContain("password", content, System.StringComparison.OrdinalIgnoreCase)

    [<Fact>]
    let ``Hierarchy fixture covers mandatory levels`` () =
        let content = Fixture.readResource "issues-hierarchy.json"
        use doc = JsonDocument.Parse(content)
        let issues = doc.RootElement.GetProperty("issues").EnumerateArray() |> Seq.toList
        
        let epics = issues |> List.filter (fun i -> i.GetProperty("fields").GetProperty("issuetype").GetProperty("hierarchyLevel").GetInt32() = 1)
        let standard = issues |> List.filter (fun i -> i.GetProperty("fields").GetProperty("issuetype").GetProperty("hierarchyLevel").GetInt32() = 0)
        let subtasks = issues |> List.filter (fun i -> i.GetProperty("fields").GetProperty("issuetype").GetProperty("hierarchyLevel").GetInt32() = -1)

        Assert.NotEmpty(epics)
        Assert.NotEmpty(standard)
        Assert.NotEmpty(subtasks)

    [<Fact>]
    let ``Multi-sprint fixture is present`` () =
        let content = Fixture.readResource "sprints.json"
        use doc = JsonDocument.Parse(content)
        let activeSprints = 
            doc.RootElement.GetProperty("values").EnumerateArray() 
            |> Seq.filter (fun s -> s.GetProperty("state").GetString() = "active")
            |> Seq.toList
        
        Assert.True(activeSprints.Length >= 2, "Should have at least two parallel active sprints")

    [<Fact>]
    let ``Rank field is present in configuration`` () =
        let content = Fixture.readResource "board-configuration.json"
        use doc = JsonDocument.Parse(content)
        let rankFieldId = doc.RootElement.GetProperty("ranking").GetProperty("rankCustomFieldId").GetInt32()
        Assert.Equal(10019, rankFieldId)

    [<Fact>]
    let ``JiraRanks can be extracted from hierarchy fixture`` () =
        let content = Fixture.readResource "issues-hierarchy.json"
        use doc = JsonDocument.Parse(content)
        let issues = doc.RootElement.GetProperty("issues").EnumerateArray()
        for issue in issues do
            let rankValue = issue.GetProperty("fields").GetProperty("customfield_10019").GetString()
            let rank = JiraRank rankValue
            let (JiraRank actualValue) = rank
            Assert.Equal(rankValue, actualValue)

    [<Fact>]
    let ``BoardEvents can be extracted from changelog fixture`` () =
        let content = Fixture.readResource "issue-changelog.json"
        use doc = JsonDocument.Parse(content)
        let histories = doc.RootElement.GetProperty("histories").EnumerateArray()
        for history in histories do
            let eventIdStr = history.GetProperty("id").GetString()
            let eventId = BoardEventId eventIdStr
            let (BoardEventId actualValue) = eventId
            Assert.Equal(eventIdStr, actualValue)
