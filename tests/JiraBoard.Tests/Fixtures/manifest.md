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
