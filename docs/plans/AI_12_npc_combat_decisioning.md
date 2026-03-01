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

## Implementation Checklist (engine/AI)

- [x] `INpcCombatAiService`
- [x] `CombatAiContext` model with action set and state
- [x] Action legality validation with deterministic fallback action
- [x] Integration path for AI combat decision output

## Validation Notes (2026-03-01)

- Combat AI service is implemented with structured action parsing and safe fallback when output is invalid.
