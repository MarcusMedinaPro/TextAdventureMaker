## AI_06: OpenRouter and 1minAI Adapters

**Goal:** Add OpenRouter and 1minAI support, including 1minAI-specific payload/response handling.

### Scope

- Implement provider adapters with explicit schema mapping.
- Configure 1minAI daily free budget defaults.
- Support OpenRouter model/provider routing options.

### Tasks

### Task AI_06.1: OpenRouter Adapter

- Add `OpenRouterSettings`.
- Implement `OpenRouterCommandProvider`.
- Support per-call model selection.

### Task AI_06.2: 1minAI Adapter

- Add `OneMinAiSettings`.
- Implement `OneMinAiCommandProvider`.
- Create dedicated payload and response DTOs for 1minAI JSON shape.
- Normalize result into command candidate model.

### Task AI_06.3: Budget Defaults

- Add default budget profile for 1minAI:
  - `DailyTokenLimit = 15000`
- Expose override in settings/builder.

### Tests

- OpenRouter response mapping.
- 1minAI payload serialization and response deserialization.
- 1minAI budget limit enforcement.

## Implementation Checklist (engine/AI)

- [ ] `OpenRouterSettings` + provider
- [ ] `OneMinAiSettings` + provider
- [ ] 1minAI DTO mapping layer
- [ ] 1minAI budget default wiring
- [ ] Adapter tests

## Definition of Done

- OpenRouter and 1minAI are both routable via `IAiProviderRouter`.
- 1minAI adapter handles its unique JSON schema without leaking provider details into parser core.
