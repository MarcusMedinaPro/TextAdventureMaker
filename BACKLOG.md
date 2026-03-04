# TextAdventureMaker — Quality Backlog

Generated 2026-03-04 from three-agent audit (Code Quality · Command Harmony · Architecture).

Slice order: bugs → quick wins → harmony → interfaces → larger refactors.
One slice at a time. Mark `[>]` in progress, `[x]` done.

---

## 🔴 P1 — Critical Bugs (breaks gameplay)

### [x] S001 — `open/close/lock/unlock`: add target arg + container support
**Why:** `open chest` currently returns "No door here." The four door commands take no argument and blindly grab the first exit with a door. Two doors in one room = silent wrong selection.
**Files:** `OpenCommand`, `CloseCommand`, `LockCommand`, `UnlockCommand`, `KeywordParser`, `KeywordParserConfig`
**Scope:**
- Add `string? target` parameter to all four commands
- Parser passes `ParseItemName(tokens, 1)` as target (same pattern as `DestroyCommand`)
- Commands search exits for a matching door by name, then fall back to `IContainer<T>` room items for open/close
- Add `ItemAction.Open` and `ItemAction.Close` to enum for container reactions
- Tests: open named door, open chest, open with no target, open wrong target

---

### [ ] S002 — Register 12 orphaned commands in parser
**Why:** These command classes exist but are never reachable by the player. They silently return `UnknownCommand`.
**Commands to wire:** `ThrowCommand`, `RepairCommand`, `SolveCommand`, `ShoutCommand`, `ListenCommand`, `BuyCommand`, `SellCommand`, `ShopCommand`, `UndoCommand`, `RedoCommand`, `MapCommand`, `HistoryCommand`
**Files:** `KeywordParserConfig`, `KeywordParser`, `KeywordParserConfigBuilder`
**Scope:** Add a keyword set per command in config, add a parser branch per command. Tests: each verb parses to correct command type.

---

## 🟠 P2 — Quick Wins (small effort, high value)

### [ ] S003 — `CommandContext.State` → `IGameState`
**Why:** All 55 commands depend on the concrete `GameState`, making unit testing without a full engine setup impossible. DIP violation.
**File:** `Commands/CommandContext.cs` + all command unit tests
**Scope:** Change `public GameState State { get; }` to `public IGameState State { get; }`. Verify all commands compile (some may need casts removed). Tests: construct a mock `IGameState` in a command test.

---

### [ ] S004 — Add `Name` to `ILocation`
**Why:** Debug commands and others cast to concrete `Location` to read `.Name`, leaking the abstraction.
**Files:** `Interfaces/ILocation.cs`, `Models/Location.cs`, debug commands
**Scope:** Add `string Name { get; }` to `ILocation`. Remove 2+ concrete casts in debug command files. Tests: `ILocation.Name` accessible without cast.

---

### [ ] S005 — Extract `FuzzyItemResolver` utility
**Why:** The fuzzy-match block is copy-pasted across 14 command files (21 call sites). Any change to fuzzy logic requires touching 14 files.
**Files:** New `Helpers/FuzzyItemResolver.cs`, all 14 command files
**Scope:**
```csharp
// Before (repeated 21×):
if (item is null && context.State.EnableFuzzyMatching && !FuzzyMatcher.IsLikelyCommandToken(name))
{
    IItem? best = FuzzyMatcher.FindBestItem(source, name, context.State.FuzzyMaxDistance);
    if (best is not null) { item = best; suggestion = best.Name; }
}
// After:
(item, suggestion) = FuzzyItemResolver.Resolve(context.State, source, name);
```
Tests: fuzzy resolution works identically before/after in TakeCommand, EatCommand, ReadCommand.

---

### [ ] S006 — Add `WithOptionalSuggestion` + `OkWithReaction` extensions
**Why:** Every item-interaction command ends with the same ternary chain (18 occurrences):
```csharp
var result = reaction is not null ? CommandResult.Ok(msg, reaction) : CommandResult.Ok(msg);
return suggestion is not null ? result.WithSuggestion(suggestion) : result;
```
**File:** `Commands/CommandResultExtensions.cs`
**Scope:** Add two extension methods:
```csharp
static CommandResult OkWithReaction(string message, string? reaction)
static CommandResult WithOptionalSuggestion(this CommandResult result, string? suggestion)
```
Replace all 18 occurrences across commands. Tests: null reaction → no extra param, null suggestion → no suggestion appended.

---

### [ ] S007 — `ConsumableExtensions`/`StackExtensions` → target `IItem`
**Why:** `AsFood(this Item)`, `AsDrink(this Item)`, `AsStack(this Item)` take a concrete type, breaking fluent chains that hold `IItem`. `ConsumableExtensions` also casts return to `(Item)` — unsafe for subclasses.
**Files:** `Extensions/ConsumableExtensions.cs`, `Extensions/StackExtensions.cs`
**Scope:** Change `this Item` to `this IItem` on all three methods. Remove unsafe `(Item)` casts. Tests: call on a subclass type (e.g., `Key`) without exception.

---

### [ ] S008 — Remove static dictionaries from `StoreExtensions`/`PuzzleExtensions`
**Why:** Both use `private static Dictionary<ILocation, T>` as process-global side-channel state. Multiple game instances in one process share stores/puzzles; tests cannot isolate.
**Files:** `Extensions/StoreExtensions.cs`, `Extensions/PuzzleExtensions.cs`, `Interfaces/ILocation.cs` or `Interfaces/IGameState.cs`
**Scope:** Move store and puzzle associations onto `ILocation` as optional properties (or onto `IGameState`). Remove static fields. Tests: two separate `GameState` instances do not share store data.

---

### [ ] S009 — Dead properties on `RepairCommand`
**Why:** `Name`, `Aliases`, `Description` on `RepairCommand` are not part of `ICommand` and are never read anywhere. Pure noise.
**File:** `Commands/RepairCommand.cs`
**Scope:** Delete the three dead properties. Verify build.

---

## 🟡 P3 — Command Harmony

### [ ] S010 — `eat`/`drink`: room item fallback + pickup hint
**Why:** `eat bread` when bread is on the table gives "No such item in inventory" — as if the bread doesn't exist. `ReadCommand` correctly handles this with `Language.MustTakeToRead`.
**Files:** `Commands/EatCommand.cs`, `Commands/DrinkCommand.cs`, `Localization/Language.cs`
**Scope:** Search room items as fallback. If found but not in inventory, return `Language.MustPickUpToEat` / `Language.MustPickUpToDrink`. Tests: food on floor gives hint; food in inventory still works.

---

### [ ] S011 — `look` shows container contents
**Why:** `look chest` shows the chest description but never lists its contents. The player cannot discover what is inside without game-specific DSL hints.
**File:** `Commands/LookCommand.cs` — `ExecuteTarget` method
**Scope:** After resolving an `IItem` target, check `item is IContainer<IItem> container`. If yes, append contents list (respecting `HiddenFromItemList`). Tests: `look chest` with 2 items inside shows items; empty chest says "It is empty."

---

### [ ] S012 — Fix misleading error codes: `talk`/`attack` on wrong type
**Why:** `talk door` returns `Language.NoSuchNpcHere` with `GameError.TargetNotFound`. The door exists — it just can't be talked to. `GameError.TargetNotFound` misleads adventure authors debugging reactions.
**Files:** `Commands/TalkCommand.cs`, `Commands/AttackCommand.cs`, `Enums/GameError.cs`
**Scope:** Add `GameError.WrongObjectType`. When target is found but wrong type (not NPC), return a distinct message + error code. Tests: `talk <item>` → `WrongObjectType`; `talk <nonexistent>` → `TargetNotFound`.

---

### [ ] S013 — 4 door commands: extract shared `DoorCommandBase`
**Why:** `OpenCommand`, `CloseCommand`, `LockCommand`, `UnlockCommand` all repeat "find door by target name" logic after S001. Combine into base class.
**Files:** New `Commands/DoorCommandBase.cs`, all four command files
**Scope:** Extract `protected Exit? ResolveDoor(CommandContext, string? target)` into base class. Four concrete commands inherit and only provide the verb-specific action. Tests: all four still pass existing tests.

---

### [ ] S014 — `EatCommand`/`DrinkCommand`: extract shared consume logic
**Why:** Both commands have an identical "apply heal → check poison → remove from inventory → fire reaction" body.
**File:** New helper or base class `ConsumableItemHandler`
**Scope:** Extract `CommandResult ApplyConsumable(IItem item, CommandContext context, ItemAction action, string successMessage)`. Both commands delegate to it. Tests: eat food heals; drink poison damages; reaction fires.

---

### [ ] S015 — `NpcReactionResolver`: OCP fix via `IReactableCommand`
**Why:** `BuildTriggers` is a type-switch on concrete command types. Every new command that wants NPC reactions requires modifying this static method. Violates Open/Closed.
**Files:** `Engine/NpcReactionResolver.cs`, `Interfaces/ICommand.cs` or new `Interfaces/IReactableCommand.cs`
**Scope:** Add `string[] GetNpcTriggers()` to an opt-in `IReactableCommand` interface. Commands that participate implement it. `BuildTriggers` checks `command is IReactableCommand r ? r.GetNpcTriggers() : []`. Update existing commands. Tests: new command implementing interface gets triggers without touching resolver.

---

## 🟢 P4 — Missing Interfaces

### [ ] S016 — Add `IExaminable` + unify `LookCommand.ExecuteTarget`
**Why:** `LookCommand.ExecuteTarget` is 110 lines of `if item → if npc → if door → if key → if room`. Adding a new entity type requires modifying this method. `IItem`, `INpc`, `IDoor` all independently define `GetDescription()`.
**Files:** New `Interfaces/IExaminable.cs`, `Interfaces/IItem.cs`, `Interfaces/INpc.cs`, `Interfaces/IDoor.cs`, `Commands/LookCommand.cs`
**Scope:**
```csharp
public interface IExaminable
{
    string GetDescription();
    bool Matches(string input);
}
```
`IItem`, `INpc`, `IDoor` extend `IExaminable`. `LookCommand.ExecuteTarget` collapses to a single `FindExaminable()` call. Tests: look at item, NPC, door — all produce description via same code path.

---

### [ ] S017 — Add `IOpenable` + unify `OpenCommand`/`CloseCommand`
**Why:** After S001 adds container support, both `OpenCommand` and `CloseCommand` check for `IDoor` and `IContainer<T>` independently. An `IOpenable` interface unifies them.
**Files:** New `Interfaces/IOpenable.cs`, `Interfaces/IDoor.cs`, `Interfaces/IContainer.cs`, `Commands/OpenCommand.cs`, `Commands/CloseCommand.cs`
**Scope:**
```csharp
public interface IOpenable
{
    bool Open();
    bool Close();
    bool IsOpen { get; }
}
```
`IDoor` and `IContainer<T>` extend `IOpenable`. Commands work against `IOpenable`. Tests: open door → works; open chest → works; same command class handles both.

---

### [ ] S018 — Add `IMatchable` + unify NPC name resolution
**Why:** `IItem.Matches(string)` and `IDoor.Matches(string)` exist on their interfaces. `INpc` has no `Matches()` — lookup uses `.Name.TextCompare()` inline in commands. Inconsistent.
**Files:** New `Interfaces/IMatchable.cs`, `Interfaces/IItem.cs`, `Interfaces/IDoor.cs`, `Interfaces/INpc.cs`, `Models/Npc.cs`
**Scope:** Add `IMatchable { bool Matches(string input); }`. `IItem`, `IDoor`, `INpc` all extend it. Add `Matches()` to `Npc.cs` (checks Id and Name case-insensitive). Update NPC-lookup code in commands to use `Matches()`. Tests: `npc.Matches("keeper")` and `npc.Matches("Keeper")` both true.

---

## 🔵 P5 — Architecture (larger, do last)

### [ ] S019 — `AdventureDslParser.ApplyItemOptions` dedup
**Why:** Two overloads for `Item` and `Key` with identical bodies.
**File:** `Dsl/AdventureDslParser.cs`
**Scope:** Merge into one method taking `IItem`. Tests: key options still applied correctly.

---

### [ ] S020 — `Game.Run()` pipeline dedup
**Why:** `Run()` reimplements the parse→execute→display pipeline independently of `Execute(string)`.
**File:** `Engine/Game.cs`
**Scope:** `Run()` calls `Execute(string)` internally. Tests: Run loop produces same results as explicit Execute calls.

---

### [ ] S021 — `IFluid` → extend `IGameEntity`
**Why:** `IFluid` defines `Id`, `Name`, `GetDescription()` manually — all of which are already on `IGameEntity`. Fluids are outside the entity hierarchy, preventing entity extensions from working on them.
**Files:** `Interfaces/IFluid.cs`, `Models/FluidItem.cs`
**Scope:** `IFluid : IGameEntity`. Remove duplicated members. Tests: `GameEntityExtensions` work on a fluid item.

---

### [ ] S022 — `GrammarExtensions`: extract `EnglishGrammar` to own file
**Why:** `EnglishGrammar : IGrammarProvider` is a model/service embedded in an extensions file. SRP violation.
**Files:** `Extensions/GrammarExtensions.cs` → split into `Extensions/GrammarExtensions.cs` + `Localization/EnglishGrammar.cs`
**Scope:** Move class, update namespace. Tests: unchanged.

---

### [ ] S023 — `ConsoleExtensions` → `ConsoleRenderer`
**Why:** `SetupC64`, `WriteLineC64`, `WritePromptC64` are static utility methods with no `this` parameter. They are not extension methods and don't belong in `Extensions/`.
**Files:** Rename/move to `Engine/ConsoleRenderer.cs` or `Output/ConsoleRenderer.cs`
**Scope:** Move methods, update all callers. Tests: C64 output still works.

---

### [ ] S024 — Unify direction alias table (DSL vs KeywordParser)
**Why:** `AdventureDslParser` has its own `TryParseDirection` with its own alias table; `KeywordParser` delegates to `KeywordParserConfig.DirectionAliases`. The two can diverge (e.g., DSL recognises "nw" but config doesn't).
**Files:** `Dsl/AdventureDslParser.cs`, `Parsing/KeywordParserConfig.cs`
**Scope:** Both share a single static `DirectionAliases` source. Tests: DSL `exit: hall north` and `go nw` both work from the same alias set.

---

### [ ] S025 — `IItem` ISP split
**Why:** `IItem` has 50+ members covering identity, consumables, readability, durability, stackability, events. Any `IItem` implementor must carry all of them. Mocking is painful.
**Files:** `Interfaces/IItem.cs`, new `Interfaces/IConsumable.cs`, `IReadable.cs`, `IDurable.cs`, `IStackable.cs`, `Models/Item.cs`
**Scope:** Extract the four sub-interfaces. `IItem` composes them (or they are optional). Existing `Item` implements all. Commands use narrow interfaces where possible. Tests: an `Item` stub only implementing `IItem` + `IReadable` works in `ReadCommand`.
> ⚠️ Large change — affects all item tests and DSL item creation. Do last.

---

## Notes

- Each slice: implement → tests pass → commit before next slice
- Slices marked `[ ]` = pending, `[>]` = in progress, `[x]` = done
- S001 is the highest-priority bug — player-visible breakage
- S003 (CommandContext.State → IGameState) unlocks clean unit tests for all subsequent slices
- S016–S018 (interfaces) should come after S001–S015 to avoid mid-flight API churn
