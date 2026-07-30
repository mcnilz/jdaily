# Native-AOT- und Cross-Plattform-Spike

`SPK-002` prüft eine AOT-taugliche JSON- und SQLite-Mindestintegration mit `Microsoft.Data.Sqlite` und versionierten manuellen SQL-Skripten. Der Spike bleibt von Produktpersistenz und Jira-Transport getrennt und endet mit einer ADR.

## For Future Agents

As work proceeds: mark checkboxes `- [x]` as items complete; when a phase is done, set its status to `Complete` and write its **Phase Summary** (what was done, key decisions, anything needed to continue with zero context); run the phase's **Verification Plan** and record the result before moving on. When all phases are done, fill in **Final Recap** and **Deployment Plan**.

## Phase 1: Dependency decision and baseline
Status: Complete

- [x] Confirm `Microsoft.Data.Sqlite` as the provider to evaluate and versioned manual SQL scripts as the migration approach.
- [x] Review an exact stable provider version, its complete transitive graph, license files, maintenance status and trim/AOT behavior before adding any package reference; `Microsoft.Data.Sqlite 10.0.10` needs a pin to `SQLitePCLRaw.bundle_e_sqlite3 3.0.5` to avoid GHSA-2m69-gcr7-jv3q, but then transitively includes `SQLite 3.53.4` under Public Domain.
- [x] Extend the versioned allowlist, license inventory and notices only after the review approves every new dependency.
- [x] Capture the selected approach, rejected alternatives and remaining platform limits in [`ADR-003`](../docs/adr/ADR-003-native-aot-sqlite-spike.md).

### Verification Plan
- The dependency-policy scanner approves the regenerated locked graph and the notices generator verifies without modifying the repository.
- The documented review identifies exact package versions, licenses and AOT evidence for every added package.

### Phase Summary
The owner approved the exact `SQLite 3.53.4` dependency on 30 July 2026. The pinned graph, allowlist, full inventory, reproducible notices and ADR are complete; the policy and notice checks are green.

## Phase 2: Minimal AOT paths
Status: Complete

- [x] Add a failing test for explicit JSON DTO deserialization through known metadata and no Domain-Union serialization.
- [x] Add the minimal JSON DTO and explicit mapping required by the AOT smoke; `SnapshotJson` maps only stable context IDs and does not serialize Domain unions.
- [x] Add a failing test for opening a temporary SQLite file and applying versioned manual SQL migrations.
- [x] Implement the reflection-free migration runner and add both paths to the static AOT smoke register.

### Verification Plan
- Relevant JIT tests fail before and pass after each behavior change.
- The Windows Native-AOT smoke publishes and starts successfully without new trim/AOT warnings.

### Phase Summary
The tests prove explicit JSON DTO mapping, missing required input handling and idempotent temporary SQLite schema initialization. The Windows `win-x64` Native-AOT artifact starts successfully and executes both paths; only the known external `FSharp.Core` warnings `IL3053` and `IL2104` remain.

## Phase 3: Platform evidence and review
Status: In progress

- [x] Add matching Linux and macOS `osx-x64` AOT workflow steps; macOS runs on the Intel `macos-13` host.
- [ ] Collect successful Linux and macOS CI runs from the versioned workflow on matching hosts.
- [x] Run the complete Release validation and the Windows self-contained Native-AOT smoke.
- [x] Self-review the final dependency graph, architecture boundaries, static registration and secret-free temporary database behavior.
- [ ] Synchronize backlog, readiness evidence and active state, then present the ADR and platform evidence for human acceptance.

### Verification Plan
- `dotnet restore JiraBoard.slnx`
- `dotnet build JiraBoard.slnx -c Release --no-restore`
- `dotnet test JiraBoard.slnx -c Release --no-build`
- Native-AOT publish and start smokes on each configured matching platform host.

### Phase Summary
The local validation, review and workflow configuration are complete. The only remaining evidence is the actual Linux/macOS execution in GitHub Actions, which cannot run against the uncommitted package.

## Final Recap
_(write when all phases complete: summary of the entire piece of work)_

## Deployment Plan
_(write when all phases complete: step-by-step deployment instructions)_