# AI Gameplay Feature Backlog

## Requested Features (Marcus)

These features should be optional and only active when AI mode is enabled.

1. NPC impersonation with context
   - AI can answer as an NPC using:
     - current location context
     - NPC inventory/context
     - NPC relationship/opinion about player
2. Logical NPC movement
   - AI suggests/chooses NPC movement paths that respect map connectivity.
3. Story steering
   - AI receives current map + state and proposes what happens next.
4. Combat decisioning
   - AI controls NPC attack choices in combat.
5. Dialogue + relationship reactions
   - Positive/negative conversation outcomes affect relationship score.
6. Room description generation
   - AI creates room descriptions.
7. Changed room descriptions after events
   - Example: explosion changes description.
8. Item/object descriptions
   - AI creates object descriptions.
9. Session caching for all generated descriptions
   - Descriptions are stable during session to avoid drift.

## Additional Suggested Features

1. Tone profiles per NPC
   - same facts, different speaking style per character.
2. Memory summarisation per NPC
   - compact memory snapshots to keep token usage low.
3. Explainable AI actions
   - optional debug reason for movement/combat/dialogue choices.
4. Safety rails for lore consistency
   - prevent contradiction of established world facts.
5. Deterministic replay mode
   - seed-driven AI decisions for testing and QA.
6. Latency-aware degradation
   - fallback to rules when provider is slow/unavailable.

## Architecture Constraints

- Keep command execution authoritative in core engine.
- AI decisions become proposals/actions, then validated by game rules.
- Cache generated descriptions within session (and optional persisted cache later).
- Preserve offline deterministic path when AI is disabled.
