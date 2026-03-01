## AI_14: Room/Item Description Generation

**Goal:** Generate rich room and item descriptions with consistency controls.

### Tasks

1. Add `IRoomDescriptionAiService` and `IItemDescriptionAiService`.
2. Provide concise prompt schemas:
   - room summary (static facts + mood)
   - item summary (material/state/importance)
3. Integrate with session cache (`AI_13`).
4. Add consistency checks:
   - no contradiction with known room/item properties
   - no mention of forbidden hidden/takeable spoilers unless configured

### Tests

- Generated descriptions are cached and stable.
- Changed room state yields updated description.
- Output violating consistency rules is rejected/fallbacked.

## Implementation Checklist (engine/AI)

- [x] `IRoomDescriptionAiService`
- [x] `IItemDescriptionAiService`
- [x] Prompt schemas with baseline + delta input
- [x] Session cache integration for stable descriptions
- [x] Fallback behaviour for invalid or unavailable AI output

## Validation Notes (2026-03-01)

- Room/item description services are implemented with cache integration and deterministic fallback paths.
