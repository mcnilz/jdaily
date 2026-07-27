# AGENTS.md

## Read first

Before implementation, read:

1. [Active state](active-state.md)
2. [Technical handover](avalonia-fsharp-funcui-stack-handoff.md)
3. [DDD glossary](domain-glossary.md)
4. [License and Avalonia Free policy](license-policy.md)
5. [Implementation readiness](implementation-readiness-checklist.md)
6. [UI design specification](ui-design-specification.md)
7. [Product backlog](product-backlog.md)

The active-state file is the operational re-entry point, but it is not a specification or second backlog. The following five files are authoritative for product, domain language, licensing, architecture and design. The product backlog defines execution order and must be corrected if it conflicts with them. If a product decision is missing, stop and ask instead of guessing.

## Project state

**`active-state.md` must always describe the actual current project state.** Read it first whenever starting or resuming work.

- Select the next eligible `Ready` backlog item, move it to `Proposed`, present goal, scope, risks, validation plan and later human acceptance points, and wait for explicit human confirmation. A direct human instruction to implement a concrete scoped item counts as its confirmation.
- Only after confirmation, move the item to `In Progress` and record owner, current substep, next concrete action and exclusive write scope in `active-state.md`.
- After every coherent result, synchronize the active entry, affected backlog status and readiness checkboxes.
- If work is blocked, mark the backlog item `Blocked` and record the exact blocker and required decision in `active-state.md`.
- After implementation, tests and review, move the item to `In Review` and present a concrete human acceptance checklist plus agent feedback on successes, problems and suggested improvements.
- Never move an item to `Done` without explicit human acceptance. Feedback returns it to `In Progress`; a material scope change returns it to `Proposed` for renewed confirmation.
- Never stage or commit an `In Review` package before the human clearly accepts that current package with the standalone acceptance word `Abgenommen`. A quoted mention, example or discussion of that word is not acceptance. A skill instruction to commit does not override this repository rule. After a valid `Abgenommen`, first synchronize the package to `Done`, then automatically stage and commit exactly the accepted current package state.
- Before any context switch, pause or handoff, update `active-state.md` to the real remaining state.
- Keep it compact: remove completed work, link to evidence and specifications, and never duplicate requirements or maintain a session diary there.

## Human approval workflow

1. **Propose:** The agent decides the next eligible work package, explains why it is next and asks the human to confirm it.
2. **Execute:** After confirmation, the agent organizes implementation, TDD, applicable tests, self-review and any useful independent review within the approved scope.
3. **Present:** The agent sets the package to `In Review`, demonstrates the result, lists exact acceptance checks, reports commands and results, and provides a short retrospective with improvements.
4. **Accept or loop:** Only a clear human `Abgenommen` for the current `In Review` package permits `Done` and authorizes its automatic commit. The agent first synchronizes Active State, backlog and readiness evidence, then stages and commits exactly that accepted state. Any other wording, quoted occurrence or feedback does not authorize a commit; feedback returns the same package to implementation. The next package is not started before acceptance.

## Non-negotiable rules

- Use .NET 10, F#, Avalonia `11.3.18`, Avalonia.FuncUI/Elmish `1.6.0`.
- **Avalonia Free only:** use only the MIT-licensed Avalonia framework and explicitly approved, commercially usable OSS packages. Avalonia Community, Plus, Pro, Enterprise, Accelerate, premium controls/tools, trials, subscriptions, portal requirements and license keys are forbidden without exception.
- Never add or require `AvaloniaUILicenseKey`, `AVALONIA_TOOLS_LICENSE_KEY` or `ACCELERATE_LICENSE_KEY`. Unknown direct or transitive `Avalonia*`/AvaloniaUI packages fail until their exact version and license are approved and allowlisted.
- Check every direct/transitive production, test, build, analysis and packaging dependency plus fonts, icons and assets. Pre-approved licenses are MIT, Apache-2.0, BSD-2-Clause, BSD-3-Clause, ISC, 0BSD, OFL-1.1 for fonts and CC0-1.0 for assets; every other or unclear license requires documented review and explicit owner approval.
- No XAML/AXAML, WebView, Electron, Blazor Hybrid or preview packages.
- **FluentAssertions is forbidden without exception:** no direct/transitive package, namespace, alias, wrapper or pinned older version. Use xUnit `Assert`, F# pattern matching or small local F# helpers.
- Keep Native AOT and trimming viable; avoid reflection-based discovery and dynamic assembly loading.
- Jira Cloud only; MVP is one active site, one confirmed Team-managed Scrum project/board, and a sprint scope of all active sprints or one active sprint. No Server/Data Center.
- Preserve the exact Jira board order of swimlanes and subtasks. Keep API order across pagination as `BoardOrdinal`, discover the board rank field dynamically, and never sort locally by key, title, status, sprint, creation time or assignee.
- For `All active sprints`, deduplicate by stable issue ID but project the union as a stable subsequence of the global board order; never concatenate sprint responses as the display order.
- Store API tokens only in the native credential store; never in files, SQLite, logs, telemetry, fixtures or snapshots.
- Do not change pinned stack versions, product scope or central design contracts without explicit approval.

## Development workflow

- Work TDD: failing behavior test, minimal implementation, refactor. Add a reproducing test before every bug fix.
- Keep the domain independent of Avalonia, Jira DTOs, HTTP, SQLite and credential-store implementations.
- Map Jira through explicit DTOs and an anti-corruption layer.
- Use the exact ubiquitous language from `domain-glossary.md`. `Story` is only an issue type; every level-0 `StandardIssue` uses the neutral `SwimlaneHeader`.
- Build every UI component in `JiraBoard.UiCatalog` before wiring it into `JiraBoard.App`.
- Reuse the same production views in UiCatalog, VisualTests and the app; never create catalog-only copies.
- Use central design tokens and pure layout functions. Do not introduce local visual magic numbers.
- Preserve unrelated user changes. Keep parallel agents off the same files; assign one writer per bounded area.
- Add dependencies only after license, maintenance, trimming and AOT review.
- Keep the license inventory, `THIRD-PARTY-NOTICES.txt`, Avalonia-Free allowlist and marker scans as CI hard failures. Never use a Community entitlement, trial or personal account to bypass them locally or in CI.
- Keep `active-state.md`, backlog status and readiness checkboxes synchronized across `Proposed`, `In Progress`, `In Review`, `Blocked` and human-accepted `Done`. Before a context switch or handoff, update the actual remaining state. Never duplicate requirements in the active-state file.
- Follow the delivery order and dependencies in `product-backlog.md`. Do not begin broad feature implementation before backlog item `VS-007` closes the Definition-of-Ready gate.
- Restore the last valid project, board and sprint scope automatically on app start. Show project selection on first use, when restoration is impossible, and through `Projekt > Projekt auswählen…`.
- Use stable Jira IDs for project, board and sprint selection. Changing this context stops replay and makes stale asynchronous results inapplicable.
- Treat JiraTui as the behavioral reference for ordering, not as permission to use undocumented endpoints. Cover pagination, dynamic/missing/equal rank, multi-sprint merge, filtering and snapshot restore with regression tests.

## UI essentials

- Primary reference: 1920 × 1080; also validate 1440p, ultrawide and 4K plus defined zoom extremes.
- Use `Iosevka Aile` for UI text and `Iosevka Fixed` for issue keys.
- No permanent left sidebar and no global replay transport controls.
- Replay affects exactly one swimlane or one subtask; inactive lanes stay static.
- Respect keyboard navigation, automation semantics and Reduced Motion.
- Epic/parent context appears only in the standard issue modal, never as a board card or swimlane.
- The `Sprint` menu selects either all active sprints or exactly one active sprint of the selected project/board; future and closed sprints are excluded.

## Validation

After each coherent change, run the applicable checks, normally:

```sh
dotnet restore
dotnet build -c Release
dotnet test -c Release --no-build
```

### Codex sandbox and NuGet locks

If restore or build inside the Codex sandbox reports denied access to the user-level `NuGet.Config` or cannot access a NuGet lock in a global scratch area outside the workspace, treat that first as a sandbox permission problem, not as a stale project lock. Re-run the identical `rtk dotnet restore JiraBoard.slnx` or build command with the required sandbox escalation (`require_escalated`) so NuGet can read the existing user configuration and global scratch area.

Do not delete the global lock file, overwrite the user-level `NuGet.Config`, commit a repository-local workaround configuration or redirect package sources merely to bypass this sandbox error. Only investigate a genuinely stale lock after the same command also fails with normal user access and no other NuGet process is active.

Also run headless visual tests for UI changes and self-contained/Native-AOT smoke tests for dependency, serialization, persistence, wiring or publishing changes. Never update Golden Masters automatically.

When handing off, update `active-state.md` first, move completed implementation to `In Review`, then report changed behavior, tests run, remaining warnings, deferred decisions, exact human acceptance checks and the agent retrospective. Only explicit human acceptance permits `Done`.
