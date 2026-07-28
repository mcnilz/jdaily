namespace JiraBoard.Tests

open Xunit
open System
open System.Text.RegularExpressions
open System.Text.Json
open JiraBoard.Domain

[<RequireQualifiedAccess>]
module FixtureSafety =
    let private emailPattern = Regex(@"(?i)(?<![\w@])[^\s@]+@[^\s@]+\.[^\s@]+")

    let private sensitivePatterns = [
        "a secret", Regex(@"(?i)\b(token|cookie|password|session(?:id)?)\b")
        "a private IPv4 address", Regex(@"(?<![\d.])10(?:\.\d{1,3}){3}(?![\d.])|(?<![\d.])192\.168(?:\.\d{1,3}){2}(?![\d.])|(?<![\d.])172\.(?:1[6-9]|2\d|3[01])(?:\.\d{1,3}){2}(?![\d.])")
        "an internal hostname", Regex(@"(?i)\b[\w-]+\.(internal|local)\b")
    ]

    let private isAnonymousUser displayName =
        displayName = "Anonymized User" || Regex.IsMatch(displayName, @"^User \d+$")

    let rec private hasPersonalDisplayName (element: JsonElement) =
        match element.ValueKind with
        | JsonValueKind.Object ->
            let properties = element.EnumerateObject() |> Seq.toList
            let hasAccountId = properties |> List.exists (fun property -> property.NameEquals "accountId")
            let hasUnsafeDisplayName =
                hasAccountId
                && (properties
                    |> List.tryFind (fun property -> property.NameEquals "displayName")
                    |> Option.exists (fun property -> not (isAnonymousUser (property.Value.GetString()))))

            hasUnsafeDisplayName || (properties |> List.exists (fun property -> hasPersonalDisplayName property.Value))
        | JsonValueKind.Array -> element.EnumerateArray() |> Seq.exists hasPersonalDisplayName
        | _ -> false

    let validate (content: string) =
        if content.Replace("https://anonymized.atlassian.net", "").Contains("atlassian.net", StringComparison.OrdinalIgnoreCase) then
            Error "an internal Atlassian URL"
        elif emailPattern.IsMatch content then
            Error "an email address"
        else
            match sensitivePatterns |> List.tryFind (fun (_, pattern) -> pattern.IsMatch content) with
            | Some (description, _) -> Error description
            | None ->
                use document = JsonDocument.Parse content
                if hasPersonalDisplayName document.RootElement then
                    Error "a personal display name"
                else
                    Ok ()

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
    let ``Fixture safety validation accepts anonymized content and rejects sensitive content`` () =
        let safeContent = "{ \"site\": \"https://anonymized.atlassian.net\", \"user\": \"Anonymized User\" }"
        let sensitiveContents = [
            "{ \"token\": \"secret\" }"
            "{ \"email\": \"person@example.com\" }"
            "{ \"host\": \"192.168.1.10\" }"
            "{ \"host\": \"jira.internal\" }"
            "{ \"author\": { \"displayName\": \"Jane Doe\", \"accountId\": \"acc-123\" } }"
        ]

        match FixtureSafety.validate safeContent with
        | Ok () -> ()
        | Error message -> failwith $"Expected safe fixture content, but received: {message}"

        for content in sensitiveContents do
            match FixtureSafety.validate content with
            | Ok () -> failwith $"Expected sensitive fixture content to be rejected: {content}"
            | Error _ -> ()

    [<Fact>]
    let ``Fixture manifest documents the inventory and API assumptions`` () =
        let manifest = Fixture.readResource "manifest.md"

        Assert.Contains("Jira API Path / Resource", manifest)
        Assert.Contains("## API Assumptions", manifest)

        for file in allFixtureFiles do
            Assert.Contains($"`{file}`", manifest)

    [<Fact>]
    let ``Fixtures are valid JSON and anonymized`` () =
        for file in allFixtureFiles do
            let content = Fixture.readResource file
            
            // Structural check
            use doc = JsonDocument.Parse(content)
            Assert.NotNull(doc.RootElement)

            match FixtureSafety.validate content with
            | Ok () -> ()
            | Error message -> failwith $"Fixture {file} contains {message}."

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
