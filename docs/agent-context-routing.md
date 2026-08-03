# Agent Context Routing

## Purpose and authority

This policy makes the authoritative project documentation usable without loading unrelated full documents for every work package. It changes context selection only: product scope, architecture, licensing, TDD, validation, status transitions and human acceptance remain binding as specified by their authoritative sources and [AGENTS.md](../AGENTS.md).

The compact [Active State](../active-state.md) is always the operational starting point. The technical handover, DDD glossary, license policy, implementation-readiness checklist and UI design specification remain authoritative. The [Product Backlog](../product-backlog.md) remains authoritative for item status, sequence and acceptance criteria.

## Mandatory minimum context

| Phase | Always read fully | Additionally read fully | Stop condition |
|---|---|---|---|
| Einstieg oder Wiederaufnahme | `AGENTS.md`; compact output of `eng/active-state.ps1` | Current active item, if one exists | The Active State and Backlog disagree about an active item or its status. |
| Proposal | Exact backlog item; direct dependencies; relevant gate rows in the readiness checklist | The sections selected by the change routing below | The item has no observable acceptance criteria, its terms are undefined, an open dependency exists, or a product decision is missing. |
| Implementation | Confirmed backlog item; applicable rules in `AGENTS.md`; direct dependencies | The sections selected by the change routing below and existing code/tests in the bounded write area | A required authoritative section, fixture, decision or approval is absent or contradictory. |
| Validation | Accepted scope; changed files; applicable `AGENTS.md` validation rules | The relevant test, visual, AOT, license or security contracts selected below | The required check cannot run, is ambiguous, or reports a failure or unsuppressed warning. |
| Review and handoff | Diff; acceptance criteria; current Active State and Backlog item | Relevant contracts and evidence for the changed behavior | Evidence does not establish every acceptance criterion or a human decision is still required. |

Read a named section in full, including subordinate headings, rather than relying on a search-result snippet. Loading a whole document is required only when the change genuinely spans all of its contracts or no narrower authoritative section can be determined.

## Change routing

| Change class | Authoritative sections to load | Required additional evidence |
|---|---|---|
| Domain model, ordering or replay projection | DDD glossary: `Identität und Navigationskontext`, `Issues und Hierarchie`, `Boardprojektion und Reihenfolge`, `Daily und Zeit`, `Ereignisse und Replay`, `Architektur- und Testsprache`; handover: `Zielarchitektur`, `Zustandsmodell`, `Verbindliche Jira-Boardreihenfolge`, `Einheitliches Ereignismodell`, `Replay-Zustandsautomat`; relevant G2/G4/G5 rows | Relevant deterministic fixtures and pure/architecture tests. |
| UI component, layout, interaction or accessibility | UI design specification: `Design-Tokens`, `Anwendungsshell`, `Boardgeometrie`, affected `Komponentenverträge`, `Zustände und Interaktion`, `Accessibility` and `Visuelle Validierung`; handover: affected feature interaction, `Designsystem als Code`, `Native Component Gallery`, `Design- und Visual-Regression-Testing`; relevant G3/G6/G8 rows | UiCatalog scenario, production-view reuse and affected headless/visual tests. |
| Jira API, mapping, pagination, context or transition | Handover: `Projekt- und Sprintauswahl`, `Verbindliche Jira-Boardreihenfolge`, `Jira-Integration` and the affected transition/polling rules; DDD glossary: `Identität und Navigationskontext`, `Boardprojektion und Reihenfolge`, `Synchronisation und lokale Daten`; relevant G4/G5 rows | Anonymized fixture coverage and ordering/error-path regression tests. |
| Dependency, font, icon, asset, packaging, trimming or AOT | Entire license and Avalonia-Free policy; handover: `Verbindliche Technologieentscheidung`, `Paketkonfiguration`, `Projektkonfiguration für AOT-Kompatibilität`, `AOT-Regeln für Abhängigkeiten`; relevant G1/G7 rows | Allowlist/inventory/notice evidence, marker scans and required self-contained/AOT smoke checks. |
| Persistence, credential, snapshot or security | Handover: `Historischen Zustand rekonstruieren`, `Lokalen Snapshot löschen`, `Authentifizierung` and synchronization/polling rules; DDD glossary: `Synchronisation und lokale Daten`; relevant G5/G7 rows | Security, migration/error-path and required AOT/persistence tests. |
| Workflow, backlog, Active State or agent tooling | `AGENTS.md`; handover: `Verbindlicher Agent-Mensch-Arbeitsflow`, `Arbeitsregeln für den Codex-Agenten`; product backlog: `Prioritäten und Status`, `Definition of Ready für ein Backlog-Item`, `Globale Definition of Done` and the affected item; readiness rows named by the item | A before/after example, deterministic script tests when automation changes, and preservation of explicit human acceptance. |
| Documentation only | Affected document section; backlog item and Active State if their status/evidence changes | Link integrity, Markdown/diff check and confirmation that no executable behavior changed. |

If a change belongs to more than one class, load the union of the named sections. For a new or unclear class, the agent must propose the required context explicitly and obtain a decision before implementation.

## Context budget and evidence

The baseline on 2 August 2026 was 313,668 characters across `AGENTS.md`, Active State, Handoff, Glossary, License Policy, Readiness Checklist, UI Specification and Product Backlog, approximately 78,000–98,000 input tokens before source code or tool output. `WFL-001` establishes an initial bounded package context of roughly 9,000–20,000 tokens for a normal single-class change. `WFL-002` and `WFL-003` may lower that further through a shorter global rule set and generated context manifests; neither is a prerequisite for this routing policy.

The following measurements use the current line-bounded routing sets, the compact Active-State output rather than the complete state file, and the same 3.2–4.0 characters-per-token estimate. They include this routing policy, the selected backlog item and direct dependencies, but exclude source code, test output and images.

| Example item class | Before: complete mandatory document set | After: selected routing set | Reduction | Selected authorities |
|---|---:|---:|---:|---|
| Domain/order (`DOM-004`-like) | 313,668 characters, 78,000–98,000 tokens | about 51,000 characters, 12,800–15,900 tokens | about 84% | Global rules and Active State; item/dependencies; glossary identity, hierarchy and board-order sections; handover target architecture, state model and ordering sections; G2/G4 rows. |
| UI interaction (`UI-` or `SPK-005`-like) | 313,668 characters, 78,000–98,000 tokens | about 60,000 characters, 15,000–18,800 tokens | about 81% | Global rules and Active State; item/dependencies; affected UI component, geometry, interaction, accessibility and visual-validation sections; handover UiCatalog, interaction and visual-test sections; G3/G6/G8 rows. |
| Dependency/AOT (`FND-`-like) | 313,668 characters, 78,000–98,000 tokens | about 55,000 characters, 13,800–17,200 tokens | about 82% | Global rules and Active State; item/dependencies; complete license policy; handover stack, package and AOT sections; G1/G7 rows. |

For every completed work package, the handoff records:

- phase and change class or classes;
- documents and exact headings read;
- approximate input size before and after routing, using character count divided by 3.2–4.0 as a range;
- any deliberate whole-document read and its reason.

This measurement is evidence for workflow improvement, not a reason to omit required tests, reviews, approvals or authoritative contracts.
