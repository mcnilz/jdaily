# ADR-006: Isolated drag lifecycle with root capture and focus-only fallback

## Status

Accepted on 2 August 2026.

## Context

`SPK-005` must prove start, ghost/overlay, valid and invalid drop, cancellation,
and rollback in the UiCatalog without Jira. The [UI design specification](../../ui-design-specification.md)
requires shared production views, central design tokens, reduced motion, keyboard
focus, and no local visual magic numbers. `TRN-002` and `TRN-003` depend on this
spike but are responsible for the real Elmish transition workflow and event wiring.

The product owner confirmed that this spike has a focus-only keyboard fallback:
existing roving focus and Escape remain usable, but keyboard card movement is out
of scope. A cancellation caused by Escape or focus loss restores focus to the
initiating issue without changing its confirmed column.

## Decision

The spike uses a pure `DragDropState` and projection in `JiraBoard.Ui`. Its
confirmed card remains unchanged while dragging; a target is valid only for the
same swimlane and a different column. A valid drop changes the projected confirmed
column. An invalid drop, Escape, and focus loss preserve it and restore the
initiating issue as focused.

The UiCatalog renders the real production probe for active, reduced-motion, and
rollback states. The normal state shows a translucent ticket-card ghost above the
highlighted valid target; reduced motion retains the target highlight but omits the
spatial ghost. The implementation reuses `TicketCard` and central color, spacing,
line, and typography tokens.

Pointer capture is evaluated as a board-root responsibility for `TRN-003`, not a
per-card responsibility: capture starts after the card has initiated a drag,
continues to receive move/release outside the card, and must release on drop,
Escape, focus loss, or context change. This spike has no application event wiring,
so it represents those outcomes as pure state transitions and avoids a catalog-only
input implementation.

## Evidence and alternatives

`DragDropSpikeTests` first failed because no drag contract existed. The completed
tests cover start, valid target overlay, valid drop, invalid drop, Escape, focus
loss, and reduced motion. `DragDropSpikeRenderTests` render the same production
probe headlessly at 1920 × 1080 in both normal and reduced-motion states; the
pure projection tests verify ghost removal and retained target presentation.

| Alternative | Result |
|---|---|
| Capture pointer on each ticket card | Rejected: release and cancellation outside the original card are less robust and spread lifecycle ownership across cards. |
| Add keyboard card movement in the spike | Rejected by the confirmed scope; `TRN-003` may design it later if required. |
| Mutate the board order while pointer moves | Rejected: the confirmed state must remain stable until a later transition workflow explicitly commits it. |

## Consequences

- `TRN-002` can adopt the proven state boundaries (`Idle`, dragging behavior, commit, and revert) as pure Elmish states.
- `TRN-003` must wire board-root pointer capture/release and retain the focus-only fallback unless product scope changes.
- `TRN-005` owns any server-driven commit failure and visible rollback after an actual Jira transition.
- No Jira transport, mutation, new dependency, Golden Master, or application wiring is introduced by this spike.