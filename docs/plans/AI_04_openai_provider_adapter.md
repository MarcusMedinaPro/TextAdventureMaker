## AI_04: OpenAI Provider Adapter

**Goal:** Add OpenAI as a first-class AI command provider.

### Scope

- Create provider adapter with configurable model and endpoint.
- Normalize OpenAI output into command candidates.
- Ensure safe fallback to standard parser.

### Tasks

### Task AI_04.1: Settings and Adapter

- Add `OpenAiSettings` (`ApiKey`, `Model`, `Endpoint`, `TimeoutMs`).
- Implement `OpenAiCommandProvider`.
- Add simple prompt template for command-only response.

### Task AI_04.2: Output Normalization

- Normalize output into single command line.
- Strip formatting/noise.
- Validate through safety policy and fallback parser.

### Task AI_04.3: Builder Integration

- Extend AI builder chain with:
  - `.UseOpenAi(...)`
  - `.WithOpenAiModel(...)`
  - `.WithOpenAiApiKey(...)`

### Tests

- Valid output becomes executable command.
- Malformed output triggers safe fallback.
- Missing API key disables provider with explicit telemetry reason.

## Implementation Checklist (engine/AI)

- [ ] `OpenAiSettings`
- [ ] `OpenAiCommandProvider`
- [ ] Builder wiring
- [ ] Unit tests with mocked `HttpMessageHandler`

## Definition of Done

- OpenAI can be primary or fallback provider through router.
- No behavioural regression for non-AI parser flow.
