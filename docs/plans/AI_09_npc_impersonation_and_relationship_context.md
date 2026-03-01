## AI_09: NPC Impersonation and Relationship Context

**Goal:** Let AI answer as NPCs using structured context and relationship state.

### Tasks

1. Add `NpcAiContext` model:
   - npc id/name/persona
   - current location
   - npc inventory summary
   - relationship score/opinion towards player
2. Add `INpcDialogueAiService` contract.
3. Add prompt template for NPC impersonation with strict style rules.
4. Add safety filter to prevent lore-breaking/system leakage.
5. Add deterministic fallback when AI disabled/unavailable.

### Tests

- Context includes relationship and location.
- AI response is rejected when it breaks safety format.
- Fallback returns deterministic non-AI dialogue.

## Implementation Checklist (engine/AI)

- [x] `NpcAiContext` with relationship and location context
- [x] `INpcDialogueAiService`
- [x] NPC dialogue prompt template
- [x] Safety-gated fallback behaviour for unavailable/invalid AI output

## Validation Notes (2026-03-01)

- Dialogue context and deterministic fallback behaviour are implemented through `NpcDialogueAiService` and plugin command integration.
