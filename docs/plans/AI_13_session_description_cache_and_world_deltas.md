## AI_13: Session Description Cache and World Deltas

**Goal:** Keep generated descriptions stable during session and support state-based changes.

### Tasks

1. Add `IAiDescriptionCache` (session scope).
2. Cache keys:
   - room/item id
   - version hash from world-delta state
3. Add world-delta model for changed environments:
   - explosion/fire/flood/repaired
4. Retrieval policy:
   - same key => same description
   - changed key/version => regenerate and cache new variant

### Tests

- Same key returns same description.
- Delta change invalidates old key and creates new description.
- Cache bypass works when AI is disabled.

## Implementation Checklist (engine/AI)

- [x] `IAiDescriptionCache` session abstraction
- [x] Session cache implementation and cache-key strategy
- [x] Delta-aware description request support via `DescriptionDelta`
- [x] Cache invalidation wiring in plugin runtime policy

## Validation Notes (2026-03-01)

- Session-stable cache behaviour and delta-aware regeneration are implemented and covered by cache tests.
