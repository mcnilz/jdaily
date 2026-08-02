# Jira-Fixture Manifest

This manifest documents the source, structure, and anonymization of the JSON fixtures used for offline development and testing of the JiraBoard app.

## Security & Anonymization Rules

1. **No Real PII:** All names, emails, and usernames are replaced with "Anonymized User", "User 123", or similar.
2. **No Secrets:** API tokens, cookies, and session IDs are strictly forbidden.
3. **No Internal URLs:** Internal Atlassian site names and instance URLs are replaced with `https://anonymized.atlassian.net`.
4. **Scrubbed Text:** Descriptions and comments are summarized or replaced with placeholder text to ensure no sensitive internal project information is leaked.

## Fixture Inventory

| File | Jira API Path / Resource | Purpose |
|---|---|---|
| `projects-boards.json` | `/rest/api/3/project` / `/rest/agile/1.0/board` | Project and board discovery. |
| `sprints.json` | `/rest/agile/1.0/board/{id}/sprint` | Sprint management (active, future, closed). |
| `board-configuration.json` | `/rest/agile/1.0/board/{id}/configuration` | Column mappings and rank field. |
| `issues-hierarchy.json` | `/rest/agile/1.0/board/{id}/issue` | Issue hierarchy (Epic -> Standard -> Subtask). |
| `issues-pagination-p1.json` | `/rest/agile/1.0/board/{id}/issue?startAt=0` | First page of paginated issues. |
| `issues-pagination-p2.json` | `/rest/agile/1.0/board/{id}/issue?startAt=50` | Second page of paginated issues. |
| `issue-changelog.json` | `/rest/api/3/issue/{key}/changelog` | Historical events for replay. |
| `errors.json` | N/A | Simulated API error responses (401, 403, 404, 429). |

## API Assumptions

- **Team-managed Scrum:** Fixtures assume the project is a "next-gen" (team-managed) Scrum project.
- **Rank Field:** Uses the standard Jira Agile `LexoRank` system, mapped via a dynamic custom field ID.
- **Hierarchy:** Standard hierarchy level 0 (Standard Issues) and level -1 (Subtasks). Level 1 (Epics) are used for context.

## SPK-003 Evidence

The product owner approved this evidence set as the replacement for a live Jira
capture on 2 August 2026. It proves the supported API surface and the reference
behavior, not equality with a particular live Jira tenant.

- Atlassian documents the supported [board issue endpoint](https://developer.atlassian.com/cloud/jira/software/rest/api-group-board/#api-rest-agile-1-0-board-boardid-issue-get) and [board configuration endpoint](https://developer.atlassian.com/cloud/jira/software/rest/api-group-board/#api-rest-agile-1-0-board-boardid-configuration-get). The fixtures model their paginated issue responses, `expand=schema,names`, and the configuration-supplied `rankCustomFieldId`.
- JiraTui is a behavioral reference only. Its [`BoardRenderSwimlaneBuilder`](https://github.com/mcnilz/JiraTui/blob/master/src/JiraTui.Tui/BoardRendering/BoardRenderSwimlaneBuilder.cs) contains `OrderByDescending(issue => issue.Rank ?? EmptyRankSortValue)` for issues in a column. Its [`JiraClient`](https://github.com/mcnilz/JiraTui/blob/master/src/JiraTui.Infrastructure/Services/JiraClient.cs) currently uses different `search/jql` and Software API paths, which are not adopted as the board-order proof for this client.
- [`ADR-004`](../../../docs/adr/ADR-004-jira-cloud-boardorder-and-rank-spike.md) records the selected Agile API path, preserves API order as `BoardOrdinal`, and marks the observed descending JiraTui rank direction versus the current ascending domain contract as a product follow-up before `JIR-007`.
