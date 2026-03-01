# AI Planning Summary

## Purpose

This document gathers the current AI planning for the TextAdventure project in one place, with focus on future additions in and around `src/MarcusMedina.TextAdventure.AI`.

## Source Documents Used (non-slice)

- `README.md`
- `docs/plans/overview.md`
- `docs/plans/index.md`
- `docs/api-command-reference.md`
- `src/MarcusMedina.TextAdventure.AI/*` (current implementation state)

## Current AI Scope (documented)

The documented AI scope is currently command parsing:

- A separate package: `MarcusMedina.TextAdventure.AI`
- Ollama-based natural-language parsing
- Integration point: `ICommandParser`
- Expected behaviour: map natural language to semantic `ICommand` objects, with fallback parsing support

In short: AI is currently positioned as a parser layer, not a full autonomous gameplay system.

## Confirmed Provider Direction

Target providers for this project:

- Ollama (local/self-hosted)
- LMStudio (local OpenAI-compatible)
- Docker AI (local/container OpenAI-compatible endpoint)
- OpenAI
- Anthropic Claude
- Mistral
- OpenRouter
- 1minAI

## Multi-Provider Architecture Plan

## Phase 0: Provider Abstraction (foundation)

- Introduce `IAiCommandProvider` with a single responsibility:
  - convert free-text input + context into a normalized command candidate
- Add provider adapters:
  - `OllamaCommandProvider`
  - `LmStudioCommandProvider`
  - `DockerAiCommandProvider`
  - `OpenAiCommandProvider`
  - `ClaudeCommandProvider`
  - `MistralCommandProvider`
  - `OpenRouterCommandProvider`
  - `OneMinAiCommandProvider`
- Add provider-specific DTO mapping where formats differ:
  - `OneMinAiCommandProvider` requires dedicated request payload + response JSON normalization
- Keep `ICommandParser` as the execution boundary:
  - provider output must still be validated and converted through existing command parsing pipeline

## Provider Routing Policy

- Primary provider can be selected per environment (`dev`, `test`, `prod`).
- Add ordered fallback chain (for example: local -> low-cost cloud -> premium cloud).
- Add per-provider timeout and failure budget.
- Add cost-aware router mode:
  - prefer free/local provider first
  - then cheapest configured provider
  - then premium provider as last fallback

## Current Implementation Snapshot (`src/MarcusMedina.TextAdventure.AI`)

- `OllamaCommandParser`
  - Calls Ollama `/api/generate`
  - Uses a fallback `ICommandParser`
  - Falls back silently on errors
- `OllamaCommandParserBuilder`
  - Fluent setup for endpoint/model/system prompt/fallback
- `OllamaSettings`
  - Endpoint, model, system prompt, temperature, timeout, enabled flag

## Gaps Between Current State and Future AI Additions

1. Current implementation is single-provider (Ollama only).
2. Parser reliability and observability are minimal (silent fallback only).
3. AI-specific tests are not yet visible in the current test suite.
4. Command safety validation is currently implicit (fallback parse), not explicit as a policy layer.
5. AI is not yet connected to broader game intelligence (intent planning, world-context reasoning, NPC decision behaviour).

## Proposed Future AI Roadmap

## Phase 1: Stabilise AI Parser (short term)

- Add explicit error/result metadata from AI parse attempts
- Add AI parser tests (success, malformed response, timeout, fallback path)
- Add configurable strict mode:
  - strict: accept only validated command format
  - permissive: current fallback behaviour
- Add command allowlist validation before command execution

## Phase 2: Context-Aware Parsing (mid term)

- Provide minimal game context to the AI parser:
  - current location
  - visible exits
  - inventory summary
- Support language-aware prompting (aligned with `ICommandParser` language abstraction)
- Add confidence/validation gate before converting to command text

## Phase 3: Intent Layer (future gameplay extension)

- Add optional intent abstraction above direct command parsing
- Example: `"I want to get to safety"` -> intent -> suggested command sequence
- Keep final execution inside existing command pipeline (`ICommand`)

## Phase 4: NPC/World AI Extensions (future)

- Introduce goal-based behaviour hooks for NPCs (optional module)
- Reuse existing engine systems (events, world state, quests) rather than bypassing them
- Keep AI modules composable and opt-in, matching current package split (`Core` + `AI`)

## Provider Cost Notes (as of 2026-03-01)

These notes are for planning and should be re-validated before production rollout.

- Ollama: local runtime and open-source model workflow (typically best baseline for zero per-call cost once hardware is available).
- OpenAI API:
  - usage tiers include a free tier in supported geographies, then paid tiers
  - prepaid billing starts from minimum top-up (documented minimum: USD 5)
- Anthropic API:
  - API pricing is model-based (for example Claude Sonnet/Haiku tiers)
  - no publicly documented permanent "daily free" buffer; accounts can start in limited mode
- Mistral:
  - published token pricing for several models (low-cost options exist)
  - free API experimentation tier is documented as limited
- OpenRouter:
  - provider routing is built in
  - free models are available in catalog (with limits)
- 1minAI:
  - provider uses its own payload/response JSON shape (adapter mapping required)
  - daily free allowance target: 15,000 tokens/day
  - paid tiers are credit-based

## Suggested Additional Low-Cost / Free-Buffer Providers

- Google Gemini API (AI Studio): documented free tier/rate limits for several models, and preferred as low-cost fallback.
- Hugging Face Inference Providers: free monthly credits on free account tier.
- GroqCloud: free developer plan is publicly advertised.

Use these as optional fallback/overflow providers in the same adapter model.

## External References

- https://ollama.com/
- https://platform.openai.com/docs/guides/rate-limits/usage-tiers
- https://help.openai.com/en/articles/8264644-what-is-prepaid-billing
- https://www.anthropic.com/pricing#api
- https://support.anthropic.com/en/articles/8241253-how-do-i-get-access-to-the-anthropic-api
- https://docs.mistral.ai/getting-started/models/models_overview/
- https://help.mistral.ai/en/articles/347393-do-you-offer-a-free-tier-for-your-api
- https://openrouter.ai/docs/features/provider-routing
- https://openrouter.ai/models?max_price=0
- https://docs.1min.ai/docs/getting-started/usage-and-pricing
- https://1min.ai/pricing
- https://ai.google.dev/gemini-api/docs/rate-limits
- https://huggingface.co/docs/inference-providers/pricing
- https://groq.com/groqcloud/

## Engineering Constraints for AI Additions

- Preserve deterministic fallback path (`KeywordParser`/standard parser flow)
- Keep AI optional and non-breaking when offline
- Keep fluent configuration style consistent with existing builder API
- Keep commands as the authoritative execution boundary
- Core engine must remain AI-agnostic:
  - `MarcusMedina.TextAdventure` must not depend on `MarcusMedina.TextAdventure.AI`
  - AI integration is provided as a plugin layer in the AI package

## Suggested Next Concrete Task

Implement **Phase 0 + Phase 1** first: provider abstraction, then AI parser test coverage + explicit parse result metadata and documented fallback behaviour.

## Implementation Snapshot (in repo now)

- [x] Provider abstraction (`IAiCommandProvider`, router, routing result metadata)
- [x] Multi-provider adapters scaffold:
  - [x] Ollama
  - [x] OpenAI
  - [x] Claude
  - [x] Mistral
  - [x] OpenRouter
  - [x] 1minAI
  - [x] Gemini
- [x] Safety policy + allowlist checks
- [x] Daily token budget policy (including 1minAI daily limit default in settings)
- [x] Builder integration for provider chain and routing profiles
- [x] Gameplay AI service contracts:
  - [x] NPC dialogue impersonation
  - [x] NPC movement
  - [x] Story director proposal
  - [x] NPC combat decisions
  - [x] Room/item description services
- [x] Session description cache + cache key builder for world deltas
