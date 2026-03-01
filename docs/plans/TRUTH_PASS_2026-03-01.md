# Truth Pass — 2026-03-01

## Scope

- Validate plan checklist state against current code in:
  - `src/MarcusMedina.TextAdventure`
  - `src/MarcusMedina.TextAdventure.AI`
  - `tests/MarcusMedina.TextAdventure.Tests`
  - `sandbox/TextAdventure.Sandbox`

## Core Slice Reality (pre-AI)

- `docs/plans/slice001.md` to `docs/plans/slice047.md`:
  - Only `slice046.md` and `slice047.md` had unchecked implementation items.
  - Those checklists were stale and have now been updated for implemented items.

### Slice 46 (Consumables)

Implemented in code:

- `IItem.IsDrinkable`, `SetDrinkable`, poison fields:
  - `src/MarcusMedina.TextAdventure/Interfaces/IItem.cs`
  - `src/MarcusMedina.TextAdventure/Models/Item.cs`
- `PoisonEffect`:
  - `src/MarcusMedina.TextAdventure/Models/PoisonEffect.cs`
- `EatCommand`, `DrinkCommand`:
  - `src/MarcusMedina.TextAdventure/Commands/EatCommand.cs`
  - `src/MarcusMedina.TextAdventure/Commands/DrinkCommand.cs`
- Poison tracking/tick:
  - `src/MarcusMedina.TextAdventure/Engine/GameState.cs`
- Parser registration:
  - `src/MarcusMedina.TextAdventure/Parsing/KeywordParser.cs`
  - `src/MarcusMedina.TextAdventure/Parsing/KeywordParserConfig.cs`
- Language strings:
  - `src/MarcusMedina.TextAdventure/Localization/Language.cs`
- Extensions:
  - `src/MarcusMedina.TextAdventure/Extensions/ConsumableExtensions.cs`
- Clone support:
  - `src/MarcusMedina.TextAdventure/Models/Item.cs`

Remaining:

- Focused test coverage target (minimum 10) is still unchecked in plan.
- Named demo file `docs/examples/46_Consumables.md` does not exist.

### Slice 47 (Stackable Items)

Implemented in code:

- `IInventory.FindById` + stack merge + stack-aware `TotalWeight`:
  - `src/MarcusMedina.TextAdventure/Interfaces/IInventory.cs`
  - `src/MarcusMedina.TextAdventure/Models/Inventory.cs`
- Partial take/drop:
  - `src/MarcusMedina.TextAdventure/Commands/TakeCommand.cs`
  - `src/MarcusMedina.TextAdventure/Commands/DropCommand.cs`
- Grouped inventory display:
  - `src/MarcusMedina.TextAdventure/Commands/InventoryCommand.cs`
- Stack support in all/take/drop flows:
  - `src/MarcusMedina.TextAdventure/Commands/TakeAllCommand.cs`
  - `src/MarcusMedina.TextAdventure/Commands/DropAllCommand.cs`
- Stack extensions:
  - `src/MarcusMedina.TextAdventure/Extensions/StackExtensions.cs`
- Parser numeric prefix (`take 3 arrows`, `drop 2 arrows`):
  - `src/MarcusMedina.TextAdventure/Parsing/KeywordParser.cs`

Remaining:

- Focused test coverage target (minimum 12) is still unchecked in plan.
- Named demo file `docs/examples/47_Stackable_Items.md` does not exist.

## AI Slice Reality (AI_01 to AI_08)

Checklists have been updated to match current implementation status.

Implemented:

- Foundation contracts/models/router/providers/settings/builder wiring are present.
- Provider chain bootstrap and sandbox wiring are present.
- Safety policy + strict mode are present.
- Budget policy with default OneMinAI limit wiring is present.

Still open in plan (real gaps):

- Provider adapter test depth (mocked `HttpMessageHandler` style tests).
- Profile integration tests.
- Full observability counters/hooks.
- Some docs still describe older scope (for example README still frames AI as Ollama-only).
- `AI_03` modernization items not fully complete (typed provider DTOs and fully async parser boundary are still open).

## AI Features Beyond AI_08 (AI_09 to AI_14)

Service contracts and implementations already exist for:

- NPC dialogue impersonation
- NPC movement decisions
- Story director proposals
- NPC combat decisions
- Session description cache + delta keys
- Room/item description generation

Primary gap here is verification depth: targeted tests are currently limited.

## Test Run Note

Runtime test execution could not be completed in this environment because installed SDK is `.NET 8.0.124` while projects target `.NET 10.0` (`NETSDK1045`).
