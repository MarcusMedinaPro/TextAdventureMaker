## AI_01: Foundation Contracts and Extension Inventory

**Goal:** Build the provider-agnostic AI foundation in `MarcusMedina.TextAdventure.AI` with clean modern C# contracts.

### Scope

- Define provider abstraction interfaces and core DTOs.
- Add explicit parse result metadata (success/fallback/error/token usage).
- Capture extension-first inventory from core project and mark reusable methods.
- Keep current behaviour compatible with fallback parser.

### Extension Inventory (must be used first)

From `src/MarcusMedina.TextAdventure/Extensions/`:

- `StringExtensions`:
  - `TextCompare`, `Is`, `EqualsIgnoreCase`
  - `StartsWithIgnoreCase`, `EndsWithIgnoreCase`
  - `CollapseRepeats`, `FuzzyMatch`
- `EnumerableExtensions`:
  - `CommaJoin`, `SpaceJoin`
- `CollectionExtensions`:
  - `PickRandom`, `Shuffle`, `WeightedRandom`
- `RangeExtensions`:
  - `Clamp`, `IsBetween`
- `TimeExtensions`:
  - `Milliseconds`, `Seconds`, `Minutes`

### Tasks

### Task AI_01.1: Core Contracts

- Add `IAiCommandProvider`.
- Add immutable request/response models:
  - `AiParseRequest`
  - `AiProviderResult`
  - `AiParseTelemetry`
- Add `AiProviderName` enum/string constants.

### Task AI_01.2: Parser Pipeline Contract

- Add `IAiProviderRouter` contract.
- Add `IAiCommandSafetyPolicy` contract for command allowlist validation.
- Add `AiParserOptions` (strict/permissive, timeout defaults, fallback behaviour).

### Task AI_01.3: Modern C# Baseline

- Use primary constructors/records where appropriate.
- Use `required`/`init` for config models.
- Use expression-bodied members for simple returns.
- Prefer `switch` expressions and pattern matching.

### Tests

- Contract-level tests for result mapping and fallback metadata.
- Safety policy tests for allowed/disallowed command output.
- Null/empty input guard tests.

## Implementation Checklist (engine/AI)

- [x] `IAiCommandProvider`
- [x] `IAiProviderRouter`
- [x] `IAiCommandSafetyPolicy`
- [x] `AiParseRequest` / `AiProviderResult` / routing telemetry equivalents (`AiRoutingResult`, `AiProviderAttempt`, `AiTokenUsage`)
- [x] `AiParserOptions`
- [x] AI foundation tests

## Definition of Done

- Existing AI usage still compiles with adapter shim.
- Foundation is provider-agnostic and test-covered.
- No duplicated logic that already exists in extension methods.

## Validation Notes (2026-03-01)

- Reviewed against current implementation: contracts, parser options, routing models, and baseline AI tests are present.
