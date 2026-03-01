## AI_12: NPC Combat Decisioning

**Goal:** AI chooses NPC combat actions from valid options based on tactical context.

### Tasks

1. Add `INpcCombatAiService`.
2. Build `CombatAiContext`:
   - npc hp/state
   - player hp/state
   - available actions (attack/flee/defend/use)
   - room/environment factors
3. AI outputs one action id + target.
4. Validator ensures action exists and is legal this turn.

### Tests

- Illegal actions are rejected and replaced by fallback strategy.
- Valid AI action is executed through existing combat system.
- Relationship/fear inputs affect action tendencies.
