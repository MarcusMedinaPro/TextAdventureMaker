## AI_08: Context, Safety, Observability and Release

**Goal:** Finalize production readiness for AI parsing with context injection, safety gates, tests, docs, and release notes.

### Scope

- Add optional game-context prompt shaping.
- Enforce strict command safety gates.
- Add robust telemetry/logging and final documentation.

### Tasks

### Task AI_08.1: Context Builder

- Add `AiContextBuilder` for compact state summary:
  - current location id/name
  - visible exits
  - inventory summary
- Keep context token-efficient and provider-neutral.

### Task AI_08.2: Safety Gates

- Add allowlist/denylist command policy.
- Validate command arguments before execution.
- Add strict mode for production safety.

### Task AI_08.3: Observability

- Add provider attempt logs and parse outcome tags.
- Add counters: success/fallback/error/timeout per provider.
- Add token usage collection where provider returns usage data.

### Task AI_08.4: Docs and Sandbox

- Add usage examples for each provider in docs.
- Add one sandbox demo showing provider chain fallback.
- Update package docs/release notes.

### Tests

- Context builder output is stable and deterministic.
- Strict safety mode blocks invalid AI output.
- End-to-end parser integration tests with provider chain.

## Implementation Checklist (engine/AI)

- [x] `AiContextBuilder`
- [x] Safety policy implementation + strict mode
- [x] Telemetry hooks and counters
- [x] Provider-chain sandbox demo
- [x] Documentation update

## Validation Notes (2026-03-01)

- Telemetry hooks now exist in router attempts with pluggable sink (`IAiTelemetrySink`), including budget skips and token collection.
- In-memory counters are provided via `InMemoryAiTelemetrySink`.
- Provider setup and fallback usage docs are captured in `docs/plans/AI_PROVIDER_SETUP.md`.

## Definition of Done

- Multi-provider parsing is production-safe and observable.
- All critical paths are covered by tests.
- Documentation reflects real setup and fallback behaviour.
