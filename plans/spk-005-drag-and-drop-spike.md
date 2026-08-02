# SPK-005 Drag-and-Drop Spike

Prove an isolated, Jira-free drag lifecycle in the UiCatalog. The spike covers visual ghost and target overlay states, valid and invalid drops, cancellation and focus restoration; keyboard card movement is explicitly out of scope, while existing roving focus remains the fallback.

## For Future Agents
As work proceeds: mark checkboxes `- [x]` as items complete; when a phase is done, set its status to `Complete` and write its **Phase Summary** (what was done, key decisions, anything needed to continue with zero context); run the phase's **Verification Plan** and record the result before moving on. When all phases are done, fill in **Final Recap** and **Deployment Plan**.

## Phase 1: Define and prove the pure lifecycle
Status: Complete

- [x] Add failing tests for drag start, valid/invalid target evaluation, cancellation, focus loss, and reduced motion.
- [x] Implement the dependency-free pure drag state and projection used by the production view.
- [x] Add a catalog fixture and scenarios for ghost/overlay, valid drop, and rollback.

### Verification Plan
- `dotnet test JiraBoard.slnx -c Release --no-restore` passes.

### Phase Summary

The test-first pure lifecycle now preserves the confirmed card until a valid drop; invalid drop, Escape, and focus loss restore the initiating focus. Active, reduced-motion, and rollback states reuse the production probe in the UiCatalog.

## Phase 2: Visual proof and decision record
Status: Complete

- [x] Add headless visual tests that render the real drag probe in normal, reduced-motion, and rollback states.
- [x] Document pointer capture at the board root and focus-only keyboard fallback in `ADR-006`.
- [x] Update readiness, backlog, active state, and test evidence for human review.

### Verification Plan
- `dotnet restore JiraBoard.slnx` completes.
- `dotnet build JiraBoard.slnx -c Release --no-restore` completes without warnings or errors.
- `dotnet test JiraBoard.slnx -c Release --no-build` passes, including visual tests.

### Phase Summary

Headless tests render normal, reduced-motion, and rollback probe states. `ADR-006` records board-root capture for later event wiring and the confirmed focus-only fallback; restore, warning-free Release build, all tests, and policy scans pass.

## Final Recap

The isolated, dependency-free UiCatalog spike proves a stable confirmed card during drag, valid target overlay, valid drop, cancellation and focus recovery, plus reduced-motion ghost suppression. It was human-accepted as `Done` on 2 August 2026.

## Deployment Plan

No deployment. The accepted package is synchronized to `Done` and ready for its required package-only commit.