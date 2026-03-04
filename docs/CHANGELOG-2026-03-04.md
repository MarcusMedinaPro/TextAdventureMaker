# Changelog — 2026-03-04

Full quality audit and systematic refactoring session. 3-agent code review produced a prioritised backlog (25 slices). All P1–P4 slices and most P5 slices were implemented in one session.

**Tests: 427 → 496 (+69)**

---

## New Features

### NPC Idle Behaviours
- `npc_idle: keeper | 3 | picking his nose | humming a song` — ambient flavour text fires every N commands
- Each NPC has its own step counter
- Idle is suppressed when the NPC would fire a reaction the same turn
- Dead NPCs do not idle
- DSL keyword: `npc_idle: <npcId> | <interval> | <msg1> | <msg2> | ...`

### `open`/`close` now target containers
- `open chest` — lists contents (or "It's empty.")
- `close chest` — confirms close
- `ItemAction.Open` and `ItemAction.Close` reactions fire on containers

### Named door targeting
- `open iron gate`, `close iron gate`, `lock gate`, `unlock gate` all select a specific door by name
- Two doors in one room no longer silently picks the wrong one

### 12 Previously Unreachable Commands Now Wired
All of these are now parseable by players:

| Command | Example |
|---------|---------|
| `ThrowCommand` | `throw sword north` |
| `RepairCommand` | `repair sword` / `repair sword with hammer` |
| `SolveCommand` | `solve puzzle1 answer` |
| `ShoutCommand` | `shout` / `shout north` / `shout hello world` |
| `ListenCommand` | `listen` / `listen north` |
| `BuyCommand` | `buy bread` |
| `SellCommand` | `sell sword` |
| `ShopCommand` | `shop` |
| `UndoCommand` | `undo` |
| `RedoCommand` | `redo` |
| `MapCommand` | `map` |
| `HistoryCommand` | `history` |

Default aliases registered; all configurable via `KeywordParserConfigBuilder.With*()`.

### `eat`/`drink` room fallback
- `eat bread` when bread is on the floor now says "You need to pick it up first before eating it." instead of "No such item in inventory."

### `look <container>` shows contents
- `look chest` now appends contents list (or "It is empty.") after the item description

### Better error codes for wrong-type targets
- `talk door` → `GameError.WrongObjectType` (not `TargetNotFound`)
- `attack rock` → `GameError.WrongObjectType`

---

## Refactoring / Architecture

### DRY — Fuzzy Item Matching
- New `Helpers/FuzzyItemResolver.cs` — single static `Resolve()` method
- Removed 21 copy-pasted fuzzy blocks across 10 command files

### DRY — CommandResult helpers
- `CommandResult.OkWithReaction(msg, reaction?)` — replaces ternary
- `result.WithOptionalSuggestion(suggestion?)` — replaces ternary
- 13+ sites updated

### DRY — Consumable commands
- New `Commands/ConsumableItemHandler.cs` — shared heal/poison/remove/react logic
- `EatCommand` and `DrinkCommand` delegate to it

### DRY — Door commands
- New `Commands/DoorCommandBase` abstract class with `protected ResolveDoor()`
- `OpenCommand`, `CloseCommand`, `LockCommand`, `UnlockCommand` all inherit

### SOLID — Open/Closed for NPC reactions
- New `Interfaces/IReactableCommand` — commands opt in via `GetNpcTriggers()`
- `NpcReactionResolver.BuildTriggers` no longer type-switches on concrete types

### Interfaces added
- `IExaminable` — `GetDescription()` + `Matches()` on `IItem`, `INpc`, `IDoor`
- `IOpenable` — `Open()`, `Close()`, `IsOpen` on `IDoor` and `IContainer<T>`
- `IMatchable` — `Matches(string)` unified across items, NPCs, doors
- `LookCommand.ExecuteTarget` simplified via `FindExaminable()` dispatch

### DIP — CommandContext.State
- `CommandContext.State` changed from `GameState` (concrete) to `IGameState` (interface)
- All 55+ commands now depend on the abstraction, not the implementation

### Store/Puzzle isolation fix
- `StoreExtensions` and `PuzzleExtensions` previously used `static Dictionary<ILocation, T>` — shared across all game instances in a process
- Now use `ILocation.Store` and `ILocation.PuzzleSystem` instance properties
- Multiple `GameState` instances are now fully isolated

### SRP — File organisation
- `EnglishGrammar` moved from `GrammarExtensions.cs` → `Localization/EnglishGrammar.cs`
- `SetupC64`/`WriteLineC64`/`WritePromptC64` moved from `ConsoleExtensions.cs` → `Engine/ConsoleRenderer.cs`

### DSL Dedup
- `AdventureDslParser.ApplyItemOptions` — two identical overloads merged to one `IItem` method
- Direction alias table unified: `AdventureDslParser` now references `KeywordParserConfig.DefaultDirectionAliases` instead of maintaining its own copy

### Interface hierarchy
- `IFluid` now extends `IGameEntity` — removes duplicated `Id`/`Name`/`GetDescription()`
- `IItem`, `INpc`, `IDoor` now implement `IExaminable → IMatchable`

### Quick fixes
- `ILocation.Name { get; }` added to interface (was concrete-only)
- `ConsumableExtensions`/`StackExtensions` — `this Item` → `this IItem`, removed unsafe `(Item)` casts
- `RepairCommand` — removed dead `Name`/`Aliases`/`Description` properties

---

## Deferred

- **S025** — `IItem` ISP split (`IConsumable`, `IReadable`, `IDurable`, `IStackable`) — large, affects all item tests and DSL creation. Marked for a dedicated session.
- **S003 partial** — `CommandContext.State` is now `IGameState`, but command unit tests still use full `GameState` setup. Mock-friendly tests can be added incrementally.
