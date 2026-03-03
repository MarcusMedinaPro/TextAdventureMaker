## Slice 48: Social Horror Party System

**Goal:** Deliver a social-horror scenario with faction pressure, suspicion tracking, and branching outcomes.

**Reference:** `docs/examples/48_The_Garden_Party.md`

### Task 48.1: Social pressure loop

- Track player suspicion and social trust in `WorldState`.
- Raise/reduce suspicion based on dialogue and actions.
- Gate progression by social state thresholds.

### Task 48.2: Faction-driven reactions

- Use `FactionSystem` reputation to influence NPC tone and access.
- Apply reputation deltas from player choices.

### Task 48.3: Branching endings

- Support at least two endings:
  - escape/survive path
  - exposure/capture path
- Ensure ending conditions are deterministic and testable.

### Task 48.4: Story instrumentation

- Log key social transitions with `StoryLogger`.
- Expose suspicion/reputation values for debug validation.

---

## Implementation Checklist (Engine)
- [x] Suspicion/trust state tracked in world counters/flags
- [x] Faction reputation integrated into NPC reactions
- [x] Branching ending conditions implemented
- [x] Story/debug logging for social state transitions
- [x] Tests covering at least two major branches

## Example Checklist (docs/examples)
- [x] Scenario demo: `48_The_Garden_Party.md`

## Dependencies
- Slice 22 (Faction & Reputation System)
- Slice 34 (Player Agency Tracking)
