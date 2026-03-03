## Slice 49: Linear Domestic Horror Progression

**Goal:** Deliver a tightly scripted domestic-horror sequence with state-gated progression and object toggles.

**Reference:** `docs/examples/49_The_Bedtime_Check.md`

### Task 49.1: State-gated progression

- Gate scene advancement with explicit world flags/counters.
- Prevent skipping required beats.
- Keep ordering deterministic for reproducible testing.

### Task 49.2: Object-state toggles

- Toggle key objects (for example lamp/stairs gates).
- Use object/world state to unlock or block movement/dialogue.

### Task 49.3: False-path interactions

- Add optional side interactions that enrich story but preserve core flow.
- Ensure false paths rejoin main path safely.

### Task 49.4: Narrative pacing checks

- Validate transitions happen in intended order.
- Surface pacing failures via debug output/events.

---

## Implementation Checklist (Engine)
- [x] Deterministic progression gates implemented
- [x] Object-state toggles wired into movement/dialogue
- [x] Side-interaction branches rejoin critical path
- [x] Ordered-state tests for main storyline

## Example Checklist (docs/examples)
- [x] Scenario demo: `49_The_Bedtime_Check.md`

## Dependencies
- Slice 9 (World State System)
- Slice 20 (Conditional Event Chains)
