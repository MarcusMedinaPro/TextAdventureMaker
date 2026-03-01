## AI_05: Claude and Mistral Adapters

**Goal:** Add Anthropic Claude and Mistral provider adapters with shared normalization strategy.

### Scope

- Implement two providers under the same contract.
- Reuse shared helper(s) for output cleaning and validation.
- Keep provider-specific payload/headers isolated.

### Tasks

### Task AI_05.1: Claude Adapter

- Add `ClaudeSettings`.
- Implement `ClaudeCommandProvider`.
- Support provider-specific headers/auth config.

### Task AI_05.2: Mistral Adapter

- Add `MistralSettings`.
- Implement `MistralCommandProvider`.

### Task AI_05.3: Shared Normalization

- Add `AiOutputNormalizer` helper to reduce duplication.
- Use core string extension methods for compare/trim/normalise logic.

### Tests

- Claude response mapping to command text.
- Mistral response mapping to command text.
- Shared normalizer handles multi-line/extra prose responses.

## Implementation Checklist (engine/AI)

- [ ] `ClaudeSettings` + provider
- [ ] `MistralSettings` + provider
- [ ] Shared normalization helper
- [ ] Adapter tests

## Definition of Done

- Claude and Mistral providers are selectable in routing profiles.
- Duplicate normalization code is minimized.
