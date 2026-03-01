## AI_10: NPC Movement and Map Navigation

**Goal:** Let AI propose logical NPC movement while respecting map graph constraints.

### Tasks

1. Add `INpcMovementAiService`.
2. Build `NpcMovementContext`:
   - current room
   - reachable exits
   - short-term npc goals
   - player position visibility (if known)
3. Add rule validator:
   - suggested move must exist as valid exit
   - blocked doors/rules are enforced
4. Add fallback movement strategy when AI output invalid.

### Tests

- AI proposal outside map is rejected.
- Valid AI move executes.
- Fallback strategy runs on timeout/error.

## Implementation Checklist (engine/AI)

- [x] `INpcMovementAiService`
- [x] `NpcMovementContext` with reachable exits and player location
- [x] Map-safe move validation against passable exits
- [x] Deterministic fallback movement strategy for invalid/timeout AI output

## Validation Notes (2026-03-01)

- NPC movement service and strategy enforce map legality and fallback behaviour, including runtime controls in plugin options.
