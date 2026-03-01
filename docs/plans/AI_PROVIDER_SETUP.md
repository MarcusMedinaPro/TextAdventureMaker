## AI Provider Setup (Sandbox + Plugin)

This document summarizes practical setup for the AI plugin provider chain and fallback behaviour.

### Primary Runtime Environment Variables

- `TA_AI_PROVIDER`: `ollama|lmstudio|dockerai|openai|claude|mistral|openrouter|1minai|gemini`
- `TA_AI_API_KEY`
- `TA_AI_MODEL`
- `TA_AI_ENDPOINT`
- `TA_AI_TIMEOUT_MS`
- `TA_AI_ENABLED`

### Optional Fallback Provider

- `TA_AI_FALLBACK_PROVIDER`
- `TA_AI_FALLBACK_API_KEY`
- `TA_AI_FALLBACK_MODEL`
- `TA_AI_FALLBACK_ENDPOINT`

The router tries providers in order. If one provider fails, returns invalid output, or exceeds budget, the next provider is tried.

### Behaviour Flags

- `TA_AI_PREFER_LOCAL_PARSE_FIRST=true|false`
- `TA_AI_STRICT=true|false`
- `TA_AI_ESTIMATED_TOKENS=<int>`
- `TA_AI_SPINNER=true|false`
- `TA_AI_DEBUG=true|false`

### Feature Flags (Sandbox)

- `TA_AI_ENABLE_DIALOGUE`
- `TA_AI_ENABLE_DESCRIPTIONS`
- `TA_AI_ENABLE_NPC_MOVEMENT`
- `TA_AI_ENABLE_STORY`
- `TA_AI_ENABLE_DESCRIPTION_CACHE_INVALIDATION`

### Cost/Budget Notes

- 1minAI daily budget can be enforced via provider settings and router budget policy.
- Telemetry counters can be collected via `IAiTelemetrySink` (`InMemoryAiTelemetrySink` available).

### Debugging

Use `/debug on` inside sandbox to see:

- `[AI call: ...]` prompt summary
- `[AI: ...]` response summary
- `[Cache: ...]` description-cache hits

### OpenAI Example

```bash
TA_AI_PROVIDER=openai \
TA_AI_API_KEY=sk-... \
TA_AI_MODEL=gpt-4o-mini \
TA_AI_PREFER_LOCAL_PARSE_FIRST=true \
dotnet run --project sandbox/TextAdventure.Sandbox/TextAdventure.Sandbox.csproj
```

### Ollama Example

```bash
TA_AI_PROVIDER=ollama \
TA_AI_MODEL=llama3.1 \
TA_AI_ENDPOINT=http://localhost:11434 \
dotnet run --project sandbox/TextAdventure.Sandbox/TextAdventure.Sandbox.csproj
```
