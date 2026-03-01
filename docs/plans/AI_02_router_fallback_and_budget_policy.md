## AI_02: Router, Fallback and Budget Policy

**Goal:** Add deterministic multi-provider routing with explicit fallback and token-budget controls.

### Scope

- Route through ordered providers with timeout/error handling.
- Add cost-aware routing profile support.
- Add token budget checks per provider/day.

### Tasks

### Task AI_02.1: Router Implementation

- Implement `AiProviderRouter`:
  - tries providers in order
  - honours per-provider timeout
  - returns first validated result
  - returns fallback metadata when all providers fail

### Task AI_02.2: Budgeting

- Add `ITokenBudgetPolicy`.
- Add in-memory daily budget implementation.
- Add provider-specific limits, including:
  - `1minAI`: default 15,000 tokens/day in config profile

### Task AI_02.3: Routing Profiles

- Add simple profiles:
  - `LocalFirst`
  - `LowCostFirst`
  - `QualityFirst`
- Add profile builder methods.

### Tests

- Router chooses first successful provider.
- Router falls back correctly when providers fail/timeout.
- Budget policy blocks provider when daily cap is exceeded.
- 1minAI budget profile defaults are enforced.

## Implementation Checklist (engine/AI)

- [x] `AiProviderRouter`
- [x] `ITokenBudgetPolicy` + in-memory implementation
- [x] Routing profiles (implemented as `DevLocalFirst`, `CloudLowCost`, `CloudBalanced`)
- [x] Unit tests for router/budget behaviour

## Definition of Done

- Fallback path is deterministic.
- Budget controls are configurable and testable.
- Telemetry includes provider sequence and stop reason.

## Validation Notes (2026-03-01)

- Router order, fallback handling, and daily budget gating are implemented and covered by routing tests.
