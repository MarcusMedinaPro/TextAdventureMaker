# AI Slices Index

## Purpose

Implementation plan for expanding `src/MarcusMedina.TextAdventure.AI` with modern C# and the same code standards as `src/MarcusMedina.TextAdventure`.

## Slice Order

| Slice | Goal | File |
| --- | --- | --- |
| AI_01 | Foundation contracts and extension-first inventory | `AI_01_foundation_contracts_and_extension_inventory.md` |
| AI_02 | Provider router, fallback policy, token budget | `AI_02_router_fallback_and_budget_policy.md` |
| AI_03 | Ollama refactor into provider pipeline | `AI_03_ollama_provider_modernization.md` |
| AI_04 | OpenAI provider adapter | `AI_04_openai_provider_adapter.md` |
| AI_05 | Claude and Mistral provider adapters | `AI_05_claude_mistral_adapters.md` |
| AI_06 | OpenRouter and 1minAI adapters | `AI_06_openrouter_1minai_adapters.md` |
| AI_07 | Gemini adapter + cost-first routing profiles | `AI_07_gemini_and_cost_first_profiles.md` |
| AI_08 | Context, safety gates, observability, docs/release | `AI_08_context_safety_observability_release.md` |
| AI_09 | NPC impersonation with relationship/context | `AI_09_npc_impersonation_and_relationship_context.md` |
| AI_10 | NPC movement planner and map-safe navigation | `AI_10_npc_movement_and_map_navigation.md` |
| AI_11 | Story director proposals from world/map state | `AI_11_story_director_and_event_proposals.md` |
| AI_12 | AI-driven combat choices for NPCs | `AI_12_npc_combat_decisioning.md` |
| AI_13 | Session description cache + world state deltas | `AI_13_session_description_cache_and_world_deltas.md` |
| AI_14 | Room/item description generation and consistency | `AI_14_room_item_description_generation.md` |

## Global Standards (applies to all AI slices)

- Keep command execution boundary in `ICommandParser` + `ICommand`.
- Prefer async APIs (`Task`, `CancellationToken`) for provider IO.
- Use extension methods already available in `MarcusMedina.TextAdventure.Extensions` before adding new helpers.
- Prefer small methods, early returns, expression members, switch expressions, and pattern matching.
- Use British English in player-facing text.
- Keep AI optional: deterministic fallback must always work offline.
- Gameplay AI feature backlog: `docs/plans/AI_FEATURE_BACKLOG.md`
