# ADR-007: Development Information capability is unavailable without a documented token read path

## Status

Accepted for `SPK-006` on 3 August 2026.

## Context

The [DDD glossary](../../domain-glossary.md) requires Development Information to
be either `JiraProvided` with supported kinds or the normal, fully functional
state `Unavailable`. `MOD-005` may only display information supplied by Jira,
and `REP-004` may only normalize commits after that capability is confirmed.
Neither feature may introduce a direct Git-provider client.

The [technical handover](../../avalonia-fsharp-funcui-stack-handoff.md) permits
small explicit clients only for documented Jira Cloud endpoints and requires the
personal API token to remain in the native credential store. This spike excludes
internal `dev-status` endpoints, browser automation, HTML scraping, OAuth, and
any token capture or persistence.

## Decision

Publish the small transport-free `DevelopmentInfoCapabilityPort` domain contract
with `Unavailable` and `JiraProvided` (`Commit`, `Branch`, `PullRequest`), and
select `Unavailable` for the current MVP integration boundary. Do not register a
Jira HTTP client, DTO mapping, or Native-AOT smoke path for Development
Information because no documented Jira Cloud API-token read endpoint was found.

On 3 August 2026, the official [Development Information API group](https://developer.atlassian.com/cloud/jira/software/rest/api-group-development-information/)
was retrieved successfully. Its published content provided general Basic API-token
security metadata but no `dev-status` or Development Information read route. The
published [Jira Platform REST OpenAPI definition](https://dac-static.atlassian.com/cloud/jira/platform/swagger-v3.v3.json)
likewise contains no `dev-status`, Development Information, or `/rest/devinfo`
read route. General Basic authentication therefore does not justify calling an
undocumented interface.

## Evidence and alternatives

`DevelopmentInformationTests` was first added before the domain contract and
failed to compile because the capability union and port did not exist. The
minimal contract then made the tests pass. The anonymized
`development-information-capability.json` fixture covers `JiraProvided`,
`Unavailable`, and HTTP `403` normalized to `Unavailable`; `FixtureTests`
validates it with the existing secret and PII safety checks.

| Alternative | Result |
|---|---|
| Call an internal `dev-status` endpoint | Rejected: it is outside the confirmed scope and has no verified official API-token read contract. |
| Scrape the Jira issue page or automate a browser | Rejected: unsupported, non-deterministic, and explicitly excluded. |
| Add OAuth or a direct Git-provider client | Rejected: neither belongs to the MVP or this spike. |
| Treat a missing or HTTP `403` capability as an error | Rejected: contradicts the glossary; both resolve to the normal `Unavailable` state. |
| Add a typed client and AOT smoke without an endpoint | Rejected: it would encode an undocumented product assumption. |

## Consequences

- `MOD-005` must render no Development Information and no error when the port
  supplies `Unavailable`; it may only display the supported kinds after a future
  documented path establishes `JiraProvided`.
- `REP-004` must not emit `BoardEventSource.Development` or `CommitLinked` events
  until capability is confirmed by a later, separately approved integration.
- A future reevaluation needs an official endpoint and token-authentication
  documentation, anonymized response fixtures, explicit DTO mapping, relevant
  AOT smoke coverage, and a product decision before any client is added.
- No token, cookie, tenant URL, personal data, raw Jira Development Information,
  provider SDK, UI, replay projection, or new dependency is introduced.

## Open points

- A future official Jira Cloud read API requires a new proposal before the
  capability can become `JiraProvided` in production.