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
