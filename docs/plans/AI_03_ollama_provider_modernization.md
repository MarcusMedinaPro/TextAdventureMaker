## AI_03: Ollama Provider Modernization

**Goal:** Refactor current Ollama parser into the new provider pipeline with modern C# and robust IO handling.

### Scope

- Replace sync-over-async calls with proper async flow.
- Keep current Ollama endpoint contract (`/api/generate`) compatible.
- Keep fallback parser behaviour.

### Tasks

### Task AI_03.1: Provider Adapter

- Implement `OllamaCommandProvider : IAiCommandProvider`.
- Typed request/response models for Ollama JSON.
- Cancellation-token support end-to-end.

### Task AI_03.2: Parser Integration

- Update `OllamaCommandParser` to use router + safety policy.
- Keep fluent builder compatibility.
- Keep silent runtime failure optional via parser options.

### Task AI_03.3: Modernization Pass

- Use early returns and small methods.
- Use extension helpers for string comparisons and normalization.
- Avoid duplicated parsing helpers.

### Tests

- Successful command normalization from Ollama response.
- Non-success HTTP status handling.
- Timeout path handling.
- Router + fallback integration tests.

## Implementation Checklist (engine/AI)

- [x] `OllamaCommandProvider`
- [ ] Async refactor in parser/provider pipeline
- [ ] Typed payload/response models
- [x] Compatibility tests

## Definition of Done

- No sync-over-async in provider calls.
- Existing sandbox flow still works with Ollama.
- Telemetry clearly shows Ollama attempt/result.
