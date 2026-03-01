## AI_07: Gemini and Cost-First Profiles

**Goal:** Add Gemini adapter and production-ready cost-aware profile presets.

### Scope

- Add Gemini provider integration.
- Add profile templates tuned for cost/free buffers.
- Keep local-first path for offline development.

### Tasks

### Task AI_07.1: Gemini Adapter

- Add `GeminiSettings`.
- Implement `GeminiCommandProvider`.
- Normalize output through shared normalizer + safety policy.

### Task AI_07.2: Cost Profiles

- Add ready-made profile builders:
  - `DevLocalFirst`: Ollama -> 1minAI -> Gemini
  - `CloudLowCost`: 1minAI -> Gemini -> OpenRouter
  - `CloudBalanced`: OpenAI -> Claude -> Mistral -> OpenRouter
- Add explicit per-profile timeout and budget defaults.

### Tests

- Profile ordering is deterministic.
- Profile-specific budget/timeout settings are applied correctly.
- Gemini provider failure still results in fallback command parse.

## Implementation Checklist (engine/AI)

- [x] `GeminiSettings` + provider
- [x] Cost profile builders
- [x] Profile integration tests

## Definition of Done

- Gemini is a first-class provider in the router.
- Low-cost deployment presets can be enabled with minimal code.

## Validation Notes (2026-03-01)

- Gemini provider and deterministic cost-profile builders are implemented and covered by routing-profile tests.
