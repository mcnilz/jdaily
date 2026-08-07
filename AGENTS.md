# AGENTS.md

## Context and authority

At every start or resume, read this file and the compact [Active State](active-state.md) with `pwsh eng/active-state.ps1`. Then apply [Agent Context Routing](docs/agent-context-routing.md): derive the smallest change class from the confirmed scope and current substep, read its named authoritative sections in full, and expand the context only when a concrete dependency, changed file or encountered contract requires it. A generated manifest is a routing aid, not a requirement to load every possible vertical-slice contract up front.

The technical handover, DDD glossary, license policy, readiness checklist and UI specification are authoritative for their subjects. The product backlog is authoritative for sequence, status and acceptance criteria. Active State is only the operational projection. Stop and ask when a decision, routing reference or required source is missing or conflicts.

## Work-package lifecycle

- Select the next eligible `Ready` item, set it to `Proposed`, and present goal, scope/non-goals, risks, validation, affected areas and human acceptance points. A direct instruction to implement a concrete scope is its confirmation.
- After confirmation, set `In Progress` and record owner, current substep, next action and exclusive write area in Active State. The Backlog is the single lifecycle-status authority; update its status and the Active-State projection together only on a real transition, block or handoff, and update readiness or historical evidence only when that evidence actually changes.
- Move completed, tested and reviewed work to `In Review` with behavior, evidence, warnings, a concrete acceptance checklist and a brief retrospective. Feedback returns to `In Progress`; material scope changes return to `Proposed`.
- Only a standalone human `Abgenommen` for the current `In Review` package permits `Done`. Then synchronize evidence, stage and commit exactly that accepted package. Never pre-commit, infer acceptance or start the next package without confirmation.

## Non-negotiable project rules

- Use .NET 10, F#, Avalonia `11.3.18` (`Avalonia.Controls.DataGrid` `11.3.13`) and Avalonia.FuncUI/Elmish `1.6.0`; no preview packages, stack/version, scope or central-contract changes without approval.
- Avalonia Free only: use approved permissive OSS, no Community/Plus/Pro/Enterprise/Accelerate, premium tools, trials, subscriptions or licence keys. Review every dependency, font, icon and asset; keep allowlist, inventory, notices and marker scans hard CI failures.
- `FluentAssertions` is forbidden everywhere, including transitively, aliases and wrappers. Use xUnit `Assert`, F# pattern matching or small local helpers only.
- No XAML/AXAML, WebView, Electron, Blazor Hybrid, reflection-based discovery or dynamic assembly loading. Keep trimming and Native AOT viable.
- Jira Cloud only: one active site, Team-managed Scrum and either all active sprints or one active sprint. Preserve API board order as `BoardOrdinal`; dynamically resolve rank; never locally re-sort. Multi-sprint views deduplicate stable IDs and remain a stable subsequence of global board order.
- Store Jira API tokens only in the native credential store; never in files, SQLite, logs, telemetry, fixtures or snapshots.

## Engineering and UI invariants

- Work Red-Green-Refactor; add a reproducing test before every bug fix. Keep the domain independent of UI, Jira DTOs, HTTP, SQLite and credential implementations; use explicit DTO mappings and the DDD glossary.
- Build UI production views in `JiraBoard.UiCatalog` before `JiraBoard.App`; reuse those views in catalog, tests and app. Use central design and motion tokens with pure layout functions.
- Preserve unrelated work; one writer owns each bounded area. Add dependencies only after license, maintenance, trimming and AOT review. Follow backlog order and do not begin broad feature work before `VS-007` closes Definition of Ready.
- Restore valid project/board/sprint context by stable Jira IDs; context changes invalidate replay. Follow the authoritative contracts for replay, UI geometry, accessibility, polling, persistence and Jira behavior selected by context routing.

## Validation and handoff

During implementation run the smallest targeted check that can disprove the current change. Before handoff, select the applicable profile from [the validation matrix](docs/validation-matrix.md) from the actual diff, combine profiles only for genuinely mixed changes, and run that closure once. Executable changes normally include:

```sh
dotnet restore JiraBoard.slnx
dotnet build JiraBoard.slnx -c Release
dotnet test JiraBoard.slnx -c Release --no-build
```

For UI changes run relevant headless visual tests; for dependency, serialization, persistence, wiring or publishing changes run relevant self-contained and Native-AOT smoke checks. Do not repeat unchanged publish or full-suite checks after every local correction; review the coherent final diff once and recheck only findings after review fixes. Documentation-only changes use the `Docs` profile and need no executable build. Never update Golden Masters automatically. If sandboxed NuGet access to the user configuration or global lock is denied, rerun the identical command with the required escalation; do not delete locks, rewrite user configuration or create a repository workaround.

Keep Red-Green evidence compact: name the reproducing test and observed failure once. Report progress only at a cause, completed scope chunk, validation result, changed direction or block. Before every handoff, Active State reflects the real remainder; report changed behavior, commands and results, warnings, deferred decisions, exact acceptance checks and a short retrospective.
