# Text Adventure Engine - Implementation Plan

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Build a fluent, SOLID text adventure engine as two NuGet packages (core + AI) with extensible DSL.

**Architecture:** Core engine uses design patterns (State, Command, Observer, Memento, Strategy, Factory, Decorator, Composite, Chain of Responsibility, Visitor, Facade, Flyweight, Proxy, Prototype, Template). All systems behind interfaces, registered via fluent GameBuilder. AI package adds Ollama-based command parsing via ICommandParser.

**Tech Stack:** C# (latest), .NET 10, xUnit, System.Text.Json

---

## Core Design Decisions

### TDD Approach: Green/Blue
Skip red phase. Write test → write code → test passes → refactor.

### Slice Workflow Rule (mandatory)
test → kod → test → refactor → test → commit → update sandbox → låt Marcus testa sandbox → fixa vid behov, korrigera tester, commit igen.

### Fluent Consistency Rule
Om ett objekt får en property/metod/extension, ge motsvarande funktionalitet till närliggande objekt (t.ex. Item/Key/Door) så API:t förblir konsekvent och lättläst.

### Language Generic

Varje kommando ska kunna ha språk-specifika nyckelord (t.ex. "go" på engelska, "gå" på svenska). Detta möjliggörs genom att ICommandParser hanterar språk och mappar inmatning till ICommand-objekt.

Finns det bättre sätt att göra detta?
Kom med förslag.

### Language System
- Single language loaded at runtime via file (e.g., `gamelang.en.txt`, `gamelang.sv.txt`)
- Default: English
- All game text comes from loaded language file

### Bi-directional Exits
Creating `hall.AddExit(Direction.North, bedroom)` automatically creates `bedroom.AddExit(Direction.South, hall)`.
Can be disabled per-exit for one-way passages.

### Doors
- Exits can have doors (IDoor)
- Doors can be: Open, Closed, Locked, Destroyed
- Locked doors require IKey
- Doors trigger events: OnOpen, OnClose, OnLock, OnUnlock, OnDestroy

### Items/Objects
- `Takeable: bool` — can player pick it up?
- `Weight: float` — optional, for inventory limits
- Containers: items can contain other items (Composite)
- Type constraints: `Glass : IContainer<IFluid>` — glass only holds fluids
- Combinations: `ice + fire → destroy both, create water`
- Events: OnTake, OnDrop, OnUse, OnOpen, OnClose, OnDestroy

### Inventory Limits
Configurable: by weight, by count, or unlimited.

### NPCs
- State: Friendly, Hostile, Dead, etc.
- Movement: None, Random, Patrol (pattern), Follow
- NPCs move between locations on game ticks

---

## Project Structure

```
C:\git\MarcusMedina\TextAdventure\
├── src\
│   ├── MarcusMedina.TextAdventure\           (core NuGet - engine + DSL)
│   └── MarcusMedina.TextAdventure.AI\        (AI NuGet - Ollama facade)
├── tests\
│   └── MarcusMedina.TextAdventure.Tests\     (xUnit tests)
├── sandbox\
│   └── TextAdventure.Sandbox\               (console app - testbädd)
├── docs\
│   └── plans\
└── TextAdventure.sln
```

---

## Slice 1: Projekt-setup + Location + Navigation

**Mål:** Spelaren kan röra sig mellan rum. Sandbox visar det.

### Task 1.1: Skapa solution och projekt

**Step 1: Skapa projektstruktur**

```bash
cd /mnt/c/git/MarcusMedina/TextAdventure
dotnet new sln -n TextAdventure
mkdir -p src/MarcusMedina.TextAdventure
mkdir -p src/MarcusMedina.TextAdventure.AI
mkdir -p tests/MarcusMedina.TextAdventure.Tests
mkdir -p sandbox/TextAdventure.Sandbox
dotnet new classlib -n MarcusMedina.TextAdventure -o src/MarcusMedina.TextAdventure
dotnet new classlib -n MarcusMedina.TextAdventure.AI -o src/MarcusMedina.TextAdventure.AI
dotnet new xunit -n MarcusMedina.TextAdventure.Tests -o tests/MarcusMedina.TextAdventure.Tests
dotnet new console -n TextAdventure.Sandbox -o sandbox/TextAdventure.Sandbox
dotnet sln add src/MarcusMedina.TextAdventure
dotnet sln add src/MarcusMedina.TextAdventure.AI
dotnet sln add tests/MarcusMedina.TextAdventure.Tests
dotnet sln add sandbox/TextAdventure.Sandbox
dotnet add tests/MarcusMedina.TextAdventure.Tests reference src/MarcusMedina.TextAdventure
dotnet add sandbox/TextAdventure.Sandbox reference src/MarcusMedina.TextAdventure
dotnet add src/MarcusMedina.TextAdventure.AI reference src/MarcusMedina.TextAdventure
```

**Step 2: Commit**

```bash
git init && git add . && git commit -m "chore: initial project structure"
```

---

### Task 1.2: ILocation + Location (bi-directional exits)

**Files:**
- Create: `src/MarcusMedina.TextAdventure/Interfaces/ILocation.cs`
- Create: `src/MarcusMedina.TextAdventure/Models/Location.cs`
- Create: `src/MarcusMedina.TextAdventure/Enums/Direction.cs`
- Create: `src/MarcusMedina.TextAdventure/Helpers/DirectionHelper.cs`
- Test: `tests/MarcusMedina.TextAdventure.Tests/LocationTests.cs`

**Step 1: Write tests**

```csharp
// LocationTests.cs
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Enums;

namespace MarcusMedina.TextAdventure.Tests;

public class LocationTests
{
    [Fact]
    public void Location_ShouldHaveId()
    {
        var loc = new Location("cave");
        Assert.Equal("cave", loc.Id);
    }

    [Fact]
    public void Location_ShouldHaveDescription()
    {
        var loc = new Location("cave")
            .Description("A dark cave with glowing mushrooms");

        Assert.Equal("A dark cave with glowing mushrooms", loc.GetDescription());
    }

    [Fact]
    public void AddExit_ShouldCreateBidirectionalPassage()
    {
        var hall = new Location("hall");
        var bedroom = new Location("bedroom");

        hall.AddExit(Direction.North, bedroom);

        Assert.Equal(bedroom, hall.GetExit(Direction.North));
        Assert.Equal(hall, bedroom.GetExit(Direction.South)); // Auto-created!
    }

    [Fact]
    public void AddExit_OneWay_ShouldNotCreateReturnPath()
    {
        var hall = new Location("hall");
        var pit = new Location("pit");

        hall.AddExit(Direction.Down, pit, oneWay: true);

        Assert.Equal(pit, hall.GetExit(Direction.Down));
        Assert.Null(pit.GetExit(Direction.Up)); // No return!
    }
}
```

**Step 2: Write implementation**

```csharp
// Enums/Direction.cs
namespace MarcusMedina.TextAdventure.Enums;

public enum Direction
{
    North, South, East, West, Up, Down,
    NorthEast, NorthWest, SouthEast, SouthWest
}
```

```csharp
// Helpers/DirectionHelper.cs
namespace MarcusMedina.TextAdventure.Helpers;

public static class DirectionHelper
{
    public static Direction GetOpposite(Direction direction) => direction switch
    {
        Direction.North => Direction.South,
        Direction.South => Direction.North,
        Direction.East => Direction.West,
        Direction.West => Direction.East,
        Direction.Up => Direction.Down,
        Direction.Down => Direction.Up,
        Direction.NorthEast => Direction.SouthWest,
        Direction.NorthWest => Direction.SouthEast,
        Direction.SouthEast => Direction.NorthWest,
        Direction.SouthWest => Direction.NorthEast,
        _ => throw new ArgumentOutOfRangeException(nameof(direction))
    };
}
```

```csharp
// Interfaces/ILocation.cs
using MarcusMedina.TextAdventure.Enums;

namespace MarcusMedina.TextAdventure.Interfaces;

public interface ILocation
{
    string Id { get; }
    string GetDescription();
    ILocation? GetExit(Direction direction);
    IReadOnlyDictionary<Direction, ILocation> Exits { get; }
}
```

```csharp
// Models/Location.cs
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Helpers;
using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public class Location : ILocation
{
    public string Id { get; }
    private string _description = "";
    private readonly Dictionary<Direction, ILocation> _exits = new();

    public IReadOnlyDictionary<Direction, ILocation> Exits => _exits;

    public Location(string id)
    {
        Id = id;
    }

    public Location Description(string text)
    {
        _description = text;
        return this;
    }

    public string GetDescription() => _description;

    public Location AddExit(Direction direction, ILocation target, bool oneWay = false)
    {
        _exits[direction] = target;

        if (!oneWay && target is Location targetLoc)
        {
            var opposite = DirectionHelper.GetOpposite(direction);
            targetLoc._exits[opposite] = this;
        }

        return this;
    }

    public ILocation? GetExit(Direction direction)
    {
        return _exits.TryGetValue(direction, out var loc) ? loc : null;
    }
}
```

**Step 3: Run tests, verify pass**

Run: `dotnet test tests/MarcusMedina.TextAdventure.Tests`
Expected: PASS

**Step 4: Commit**

```bash
git add . && git commit -m "feat: add Location with bi-directional exits"
```

---

### Task 1.3: IGameState + Navigation

**Files:**
- Create: `src/MarcusMedina.TextAdventure/Interfaces/IGameState.cs`
- Create: `src/MarcusMedina.TextAdventure/Engine/GameState.cs`
- Test: `tests/MarcusMedina.TextAdventure.Tests/NavigationTests.cs`

**Step 1: Write failing test**

```csharp
// NavigationTests.cs
namespace MarcusMedina.TextAdventure.Tests;

public class NavigationTests
{
    [Fact]
    public void Player_CanMoveNorth()
    {
        var entrance = new Location("entrance");
        var forest = new Location("forest");
        entrance.AddExit(Direction.North, forest);
        forest.AddExit(Direction.South, entrance);

        var state = new GameState(entrance);
        var moved = state.Move(Direction.North);

        Assert.True(moved);
        Assert.Equal(forest, state.CurrentLocation);
    }

    [Fact]
    public void Player_CannotMoveWhereNoExit()
    {
        var entrance = new Location("entrance");
        var state = new GameState(entrance);
        var moved = state.Move(Direction.North);

        Assert.False(moved);
        Assert.Equal(entrance, state.CurrentLocation);
    }
}
```

**Step 2: Implement GameState**

```csharp
// Interfaces/IGameState.cs
namespace MarcusMedina.TextAdventure.Interfaces;

public interface IGameState
{
    ILocation CurrentLocation { get; }
    bool Move(Direction direction);
}
```

```csharp
// Engine/GameState.cs
namespace MarcusMedina.TextAdventure.Engine;

public class GameState : IGameState
{
    public ILocation CurrentLocation { get; private set; }

    public GameState(ILocation startLocation)
    {
        CurrentLocation = startLocation;
    }

    public bool Move(Direction direction)
    {
        var target = CurrentLocation.GetExit(direction);
        if (target == null) return false;
        CurrentLocation = target;
        return true;
    }
}
```

**Step 3: Run tests, commit**

---

### Task 1.4: Sandbox - Enkel navigation

**Files:**
- Modify: `sandbox/TextAdventure.Sandbox/Program.cs`

```csharp
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;

var entrance = new Location("entrance")
    .Description("You stand at the forest gate. It's dark.");

var forest = new Location("forest")
    .Description("A thick forest surrounds you. Shadows stretch long.");

var cave = new Location("cave")
    .Description("A dark cave with glowing mushrooms.");

// Bi-directional: entrance <-> forest <-> cave
entrance.AddExit(Direction.North, forest);  // Auto-creates forest -> South -> entrance
forest.AddExit(Direction.East, cave);       // Auto-creates cave -> West -> forest

var state = new GameState(entrance);

while (true)
{
    Console.WriteLine($"\n{state.CurrentLocation.GetDescription()}");
    Console.WriteLine("Exits: " + string.Join(", ", state.CurrentLocation.Exits.Keys));
    Console.Write("> ");

    var input = Console.ReadLine()?.Trim().ToLower();
    if (input == "quit") break;

    if (Enum.TryParse<Direction>(input, true, out var dir))
    {
        if (!state.Move(dir))
            Console.WriteLine("You can't go that way.");
    }
    else
    {
        Console.WriteLine("Unknown command.");
    }
}
```

**Commit:** `feat: sandbox with basic navigation`

---

## Slice 2: Doors + Keys

**Mål:** Dörrar som blockerar utgångar, kräver nycklar.

### Task 2.1: IDoor + Door (State: Open, Closed, Locked, Destroyed) ✅
### Task 2.2: IKey + Key ✅
### Task 2.3: Door events: OnOpen, OnClose, OnLock, OnUnlock, OnDestroy
### Task 2.4: Location.AddExit with door ✅
### Task 2.5: Sandbox — låst dörr till skattkammaren, hitta nyckel ✅

---

## Slice 3: Command Pattern + Parser

**Mål:** Kommandon som objekt. Keyword-parser. "go north", "look", "quit".

### Task 3.1: ICommand + CommandResult ✅
### Task 3.2: ICommandParser + KeywordParser ✅
### Task 3.3: Inbyggda kommandon (GoCommand, LookCommand, QuitCommand, OpenCommand, UnlockCommand) ✅
### Task 3.4: Sandbox uppdatering — parser istället för raw input ✅

---

## Slice 4: Items + Inventory

**Mål:** Items i rum, plocka upp, släpp, visa inventory. Containers och kombinationer.

**Notis:** Lägg till stöd för synonymer/alias på IItem (t.ex. `Aliases: string[]`) och använd i parser/kommandon.
**Notis:** När nya item-subklasser (t.ex. Weapon/Potion) införs, ge dem fluent overrides för SetWeight/SetTakeable/AddAliases så chaining behåller typen.

### Task 4.1: IItem + Item (Factory + Prototype) ✅
- `Takeable: bool`
- `Weight: float` (optional)
- Events: OnTake, OnDrop, OnUse, OnDestroy

### Task 4.2: IInventory + Inventory ✅
- Configurable limits: ByWeight, ByCount, Unlimited
- `TakeAll()` method

### Task 4.3: Item Decorators (RustyModifier, EnchantedModifier) ✅

### Task 4.4: IContainer<T> — items that hold other items ✅
- `Glass : IContainer<IFluid>`
- `Chest : IContainer<IItem>`

### Task 4.5: Item Combinations ✅
- `ice + fire → destroy both, create water`
- Recipe system for crafting

### Task 4.6: TakeCommand, TakeAllCommand, DropCommand, InventoryCommand, UseCommand ✅

### Task 4.7: Readable Items med villkor ✅

```csharp
// Skylt - läs utan att ta
var sign = new Item("sign", "Wooden Sign")
    .SetTakeable(false)
    .SetReadable(true)
    .SetReadText("Welcome to the Dark Forest!");

// Tidning - måste ta för att läsa
var newspaper = new Item("newspaper", "Daily News")
    .SetTakeable(true)
    .SetReadable(true)
    .RequireTakeToRead()
    .SetReadText("HEADLINE: Dragon spotted near village!");

// Bok - kan läsa men tar tid (förbrukar drag)
var tome = new Item("tome", "Ancient Tome")
    .SetReadable(true)
    .RequireTakeToRead()
    .SetReadingCost(3)  // tar 3 drag att läsa
    .SetReadText("The secret to defeating the dragon is...");

// Hemligt brev - kräver ljus
var letter = new Item("letter", "Sealed Letter")
    .SetReadable(true)
    .RequireTakeToRead()
    .RequiresToRead(ctx => ctx.HasLight())
    .SetReadText("Meet me at midnight...");
```

**ReadCommand:**
```csharp
// "read sign" → visar text direkt
// "read newspaper" → "You need to pick it up first."
// "read newspaper" (i inventory) → visar text
// "read tome" → "You spend 3 turns reading... [text]"
// "read letter" (mörkt) → "It's too dark to read."
```

### Task 4.8: Sandbox — plocka upp svärd, häll vatten i glas, kombinera items, läs skylt/tidning ✅

---

## Slice 5: NPCs + Dialog + Movement

**Mål:** NPCs i rum, prata med dem, dialog-träd. NPCs rör sig.

### Task 5.1: INpc + Npc (State pattern — Friendly/Hostile/Dead)
### Task 5.2: Dialog system (Composite — konversationsträd)
### Task 5.3: NPC Movement (Strategy — None, Random, Patrol, Follow)
### Task 5.4: TalkCommand
### Task 5.5: Sandbox — prata med räven, drake patrullerar mellan grottor

---

## Slice 6: Event System (Observer)

**Mål:** Triggers när saker händer.

### Task 6.1: IEventSystem + EventSystem (Observer)
### Task 6.2: Inbyggda events (OnEnter, OnExit, OnPickup, OnDrop, OnTalk, OnCombatStart)
### Task 6.3: Item/Door events kopplas till EventSystem
### Task 6.4: Sandbox — drake vaknar när man går in i grottan

---

## Slice 7: Combat (Strategy)

**Mål:** Utbytbart stridssystem.

### Task 7.1: ICombatSystem + TurnBasedCombat (Strategy)
### Task 7.2: AttackCommand, FleeCommand
### Task 7.3: Health/Damage system
### Task 7.4: Sandbox — slåss mot draken

---

## Slice 8: Quest System

**Mål:** Objectives och progress.

### Task 8.1: IQuest + Quest (State pattern)
### Task 8.2: Quest conditions (Visitor)
### Task 8.3: Sandbox — "Hitta svärdet och döda draken"

---

## Slice 9: World State System

**Mål:** Centralt state för att spåra global världsstatus. Foundation för quests, events, stories.

### Task 9.1: IWorldState interface
- Flags: `bool` (isDragonDead, isKingdomAtWar)
- Counters: `int` (villagersSaved, daysElapsed)
- Relationships: NPC-attityd (-100 till +100)
- Timeline: kronologiska händelser

### Task 9.2: WorldState implementation
```csharp
worldState.SetFlag("dragon_defeated", true);
worldState.Increment("reputation", 50);
worldState.SetRelationship("blacksmith", 75);
```

### Task 9.3: Quest/Event conditions mot WorldState
### Task 9.4: Sandbox — villagers räknar, reputation påverkar NPC-dialog

---

## Slice 10: Save/Load (Memento)

**Mål:** Spara och ladda spelstatus.

### Task 10.1: IMemento + GameMemento
### Task 10.2: ISaveSystem + JsonSaveSystem
### Task 10.3: SaveCommand, LoadCommand
### Task 10.4: Sandbox — save, quit, load, continue

---

## Slice 11: Language System

**Mål:** Ladda språkfil för all speltext. Default engelska.

### Task 11.1: ILanguageProvider + FileLanguageProvider
### Task 11.2: Language file format (gamelang.en.txt, gamelang.sv.txt)
### Task 11.3: System messages (You pick up the {item}, You can't go that way, etc.)
### Task 11.4: Sandbox — ladda svenskt språk

---

## Slice 12: DSL Parser

**Mål:** Ladda spel från .adventure-filer.

### Task 12.1: DSL-syntax definition
### Task 12.2: IDslParser + AdventureDslParser (Template method)
### Task 12.3: Registrera egna keywords
### Task 12.4: Sandbox — ladda spel från .adventure-fil

---

## Slice 13: GameBuilder (Fluent API)

**Mål:** Bygga spel helt i C# med fluent syntax.

### Task 13.1: GameBuilder med alla .Use/.Add-metoder
### Task 13.2: IGame + Game (huvudloop, game ticks för NPC movement)
### Task 13.3: Sandbox — hela spelet via GameBuilder

---

## Slice 14: Loggers

**Mål:** Story logger + dev logger.

### Task 14.1: IStoryLogger — loggar äventyret som en saga
### Task 14.2: IDevLogger — debug/position/state
### Task 14.3: Sandbox — saga.txt genereras medan man spelar

---

## Slice 15: Pathfinder

**Mål:** Guida spelaren genom kartan.

### Task 15.1: IPathfinder + AStarPathfinder (Strategy)
### Task 15.2: HintCommand — "How do I get to the cave?"
### Task 15.3: Sandbox — pathfinder visar vägen

---

## Slice 16: AI-paket (MarcusMedina.TextAdventure.AI)

**Mål:** Ollama-integration som ICommandParser.

### Task 16.1: OllamaCommandParser (Facade)
### Task 16.2: AI-konfiguration via fluent API
### Task 16.3: Sandbox — "go somewhere dark" → Cave

---

## Slice 17: NuGet-paketering

**Mål:** Publicera båda paketen.

### Task 17.1: .nuspec / csproj-metadata
### Task 17.2: Pack + publish till NuGet
### Task 17.3: README + dokumentation

---

## Slice 18: Story Branches & Consequences

**Mål:** Hantera storylines baserat på spelarval.

### Task 18.1: IStoryBranch + IConsequence interfaces
### Task 18.2: StoryState — aktiva/avslutade grenar
### Task 18.3: Branching conditions
```csharp
game.AddStoryBranch("dragon_path")
    .Condition(q => q.IsQuestComplete("slay_dragon"))
    .Consequence(w => w.UnlockLocation("dragon_lair_treasure"))
    .Consequence(w => w.SetNpcState("king", NpcState.Grateful));
```
### Task 18.4: Sandbox — två endings baserat på val

---

## Slice 19: Multi-Stage Quests

**Mål:** Quests med stages, optional objectives, failure paths.

### Task 19.1: IQuestStage — delmål
### Task 19.2: Optional vs Required objectives
### Task 19.3: Alternative completion paths
### Task 19.4: Failure consequences, hidden objectives
```csharp
quest.AddStage("find_sword")
     .RequireObjective("search_armory")
     .OptionalObjective("ask_blacksmith")
     .AlternativePath("steal_from_castle")
     .OnFailure(w => w.SpawnHostileGuards())
     .OnComplete(w => w.UnlockStage("confront_dragon"));
```
### Task 19.5: Sandbox — quest med 3 stages, optional hints

---

## Slice 20: Conditional Event Chains

**Mål:** Sekvenser av events som påverkar varandra.

### Task 20.1: IEventChain + ICondition interfaces
### Task 20.2: Time/location/state triggers
```csharp
game.AddEventChain("village_rescue")
    .Step1(e => e.OnEnterLocation("village").ShowDialog("burning_houses"))
    .Step2(e => e.WhenItemFound("water_bucket").EnableAction("extinguish"))
    .Step3(e => e.WhenAllFiresOut().SpawnNpc("grateful_mayor"))
    .Step4(e => e.AfterTicks(20).If(q => !q.IsComplete("save_village"))
                 .Then(w => w.DestroyLocation("village")));
```
### Task 20.3: Sandbox — village rescue med tidslimit

---

## Slice 21: Time System

**Mål:** Dag/natt cycles, tidbaserade events.

### Task 21.1: ITimeSystem — ticks, dagar, faser
### Task 21.2: TimeOfDay: Dawn, Day, Dusk, Night
### Task 21.3: Dag/natt påverkar:
- NPC-platser (shopkeeper hem på natten)
- Events (varulvar spawnar i fullmåne)
- Lighting (fackla behövs i mörka grottor)
```csharp
game.UseTimeSystem()
    .SetStartTime(TimeOfDay.Dawn)
    .TicksPerDay(100)
    .OnPhase(TimePhase.Night, ctx => ctx.SetVisibility(0.3f));
```
### Task 21.4: Move/Turn Limits

```csharp
// Global drag-begränsning (hela spelet)
game.UseTimeSystem()
    .MaxMoves(400)
    .OnMovesRemaining(50, ctx => ctx.ShowWarning("Time is running out!"))
    .OnMovesRemaining(10, ctx => ctx.SetMood(Mood.Desperate))
    .OnMovesExhausted(ctx => ctx.GameOver("You ran out of time."));

// Lokal drag-begränsning (puzzle/sektion)
var bombPuzzle = game.CreateTimedChallenge("defuse_bomb")
    .MaxMoves(30)
    .OnStart(ctx => ctx.ShowMessage("The bomb will explode in 30 moves!"))
    .OnMovesRemaining(10, ctx => ctx.ShowMessage("10 moves left!"))
    .OnSuccess(ctx => ctx.Reward("bomb_defused"))
    .OnFailure(ctx => ctx.Explode());

// Aktivera när spelaren hittar bomben
room.OnEnter(ctx => bombPuzzle.Start());

// Kolla status
if (game.MovesRemaining < 100) { ... }
if (bombPuzzle.IsActive && bombPuzzle.MovesRemaining < 5) { ... }
```

**Features:**
- Globalt: `game.MaxMoves(400)` - hela spelet
- Lokalt: `CreateTimedChallenge()` - specifik puzzle
- Warnings vid trösklar
- Olika consequences vid timeout

### Task 21.5: Sandbox — butik stängd på natten, monster spawnar, bomb puzzle med 30 drag

---

## Slice 22: Faction & Reputation System

**Mål:** NPC-grupper med gemensam reputation.

### Task 22.1: IFaction — grupper av NPCs
### Task 22.2: Reputation thresholds → unlock/consequences
```csharp
game.AddFaction("thieves_guild")
    .WithNpcs("shadow", "pickpocket", "fence")
    .OnReputationThreshold(50, unlock: "guild_headquarters")
    .OnReputationThreshold(-50, consequence: w => w.SpawnHitSquad());

player.ModifyReputation("thieves_guild", +20);
```
### Task 22.3: Faction påverkar priser, locations, encounters
### Task 22.4: Sandbox — två factions, val påverkar ending

---

## Slice 23: Random Event Pool

**Mål:** Dynamiska slumpmässiga events.

### Task 23.1: IRandomEventPool
### Task 23.2: Viktning, cooldowns, context-awareness
```csharp
game.AddRandomEventPool("forest_encounters")
    .AddEvent("wolf_attack", weight: 3, cooldown: 10)
    .AddEvent("friendly_trader", weight: 5)
    .AddEvent("hidden_treasure", weight: 1)
    .RequireTimePhase(TimePhase.Night);
```
### Task 23.3: Sandbox — random encounters i skogen

---

## Slice 24: Location Discovery System

**Mål:** Hidden locations som upptäcks genom exploration.

### Task 24.1: Hidden exits med discover conditions
```csharp
location.AddHiddenExit(Direction.East, secretCave)
    .DiscoverCondition(c => c.HasItem("ancient_map"))
    .Or(c => c.TalkedToNpc("old_hermit"))
    .OnDiscovery(e => e.ShowMessage("You notice a hidden passage!"));
```
### Task 24.2: Perception checks för discovery
### Task 24.3: Fog of war för stora kartor
### Task 24.4: Sandbox — hemlig grotta kräver karta eller NPC-hint

---

## Slice 25: Story Mapper Tool (Visual Editor)

**Mål:** Grafiskt verktyg för content creation.

### Task 25.1: Web-based eller desktop app
### Task 25.2: Features:
- Dra boxes för scenes/locations
- Koppla ihop med pilar för transitions
- Sätt conditions på arrows
- Visa quest flows och NPC-relationer
### Task 25.3: Export till .adventure DSL
### Task 25.4: Import befintlig DSL för visualisering

---

## Slice 26: Mood & Atmosphere System

**Mål:** Stämning som påverkar spelarupplevelsen.

### Task 26.1: IMoodSystem — atmospheric state
### Task 26.2: Mood enum: Peaceful, Tense, Foreboding, Terrifying, Hopeful
### Task 26.3: Environmental cues: sound, smell, temperature, wind
```csharp
location.SetMood(Mood.Foreboding)
    .WithLighting(LightLevel.Dim)
    .WithAmbientSound("distant_dripping")
    .WithSmell("damp earth and decay");
```
### Task 26.4: Mood-modifiers på beskrivningar
```csharp
// Normal: "A cave entrance"
// Foreboding: "A yawning cave entrance, shadows writhing within"
```
### Task 26.5: Mood propagation (angränsande rum påverkar varandra)
### Task 26.6: Sandbox — grotta med ökande skräck ju djupare man går

---

## Slice 27: Dynamic Description System

**Mål:** Beskrivningar som ändras baserat på context.

### Task 27.1: Context-aware descriptions
```csharp
location.Description()
    .Default("A quiet forest glade")
    .When(TimePhase.Night, "Moonlight filters through the canopy")
    .When(w => w.GetFlag("dragon_dead"), "The forest feels lighter now")
    .When(p => p.HasTrait(Trait.Observant), "You notice fresh tracks leading north")
    .FirstVisit("You've never seen trees this old");
```
### Task 27.2: Variable substitution: `{player_name}`, `{npc_emotion}`, `{item_found}`
### Task 27.3: Dialog templates med parametrar
### Task 27.4: Sandbox — rum beskrivs olika beroende på tid, quest, traits

---

## Slice 28: Character Arc Tracking

**Mål:** NPCs utvecklas över tid.

### Task 28.1: ICharacterArc — definierar utveckling
### Task 28.2: Milestones som unlocks traits
```csharp
npc.DefineArc("CowardToHero")
    .StartState(Trait.Fearful)
    .Milestone(1, "witness_courage", unlocks: Trait.Hopeful)
    .Milestone(2, "own_brave_act", unlocks: Trait.Brave)
    .EndState(Trait.Heroic)
    .OnComplete(ctx => ctx.UnlockQuest("lead_rebellion"));
```
### Task 28.3: Dialog ändras automatiskt baserat på arc-progress
### Task 28.4: Sandbox — NPC växer från feg till hjälte

---

## Slice 29: Pacing & Tension System

**Mål:** Balans mellan action och lugn.

### Task 29.1: ITensionMeter — current tension (0.0 - 1.0)
### Task 29.2: Tension modifiers från events
```csharp
story.DefineTension()
    .BuildUp("dragon_approach", rate: 2.0f)
    .Peak("dragon_fight")
    .Release("dragon_dead", cooldown: 50);
```
### Task 29.3: Tension påverkar:
- Random encounter frequency
- Music/sound intensity
- Available actions
### Task 29.4: Rest periods (safe zones)
### Task 29.5: Pacing rules: "no major events within X ticks"
### Task 29.6: Sandbox — tension bygger mot dragon fight

---

## Slice 30: Foreshadowing & Callbacks

**Mål:** Chekov's Gun — plantera och betala av.

### Task 30.1: IForeshadowingSystem — spårar planted seeds
### Task 30.2: Tags/connections mellan seemingly unrelated things
```csharp
// Act 1: Plant
location.AddDetail("ancient_runes")
    .Description("Strange symbols, unreadable...")
    .Tag(Foreshadowing.DragonLanguage);

// Act 2: Hint
npc.Dialog("Those runes? Dragon script. Bad omen.")
    .LinksTo(Foreshadowing.DragonLanguage);

// Act 3: Payoff
boss.OnDefeat(ctx => {
    if (ctx.PlayerHasSeenDetail("ancient_runes"))
        ctx.ShowEpilogue("The runes now glow—a warning you understood too late.");
});
```
### Task 30.3: Payoff detection (varning om planted gun aldrig fires)
### Task 30.4: Optional callbacks (om spelaren missade hint)
### Task 30.5: Sandbox — mystiska runer som får mening senare

---

## Slice 31: Scene Transitions & Beats

**Mål:** Stories flödar mellan scenes, inte bara locations.

### Task 31.1: IScene — orkestrerar events
### Task 31.2: Scene beats (dialog/action i ordning)
```csharp
story.DefineScene("Betrayal")
    .Location("throne_room")
    .Participants("king", "advisor", "player")
    .Beat(1, "king_announces_quest")
    .Beat(2, "advisor_whispers_doubt")
    .Beat(3, "reveal_advisor_is_traitor")
    .Transition()
        .To("Escape").Trigger("player_flees")
        .Or()
        .To("Arrest").Trigger("player_trusts_advisor");

scene.Play();  // All dialog/events körs automatiskt
```
### Task 31.3: Scene transitions baserat på player actions
### Task 31.4: Sandbox — betrayal scene med två utgångar

---

## Slice 32: Emotional Stakes System

**Mål:** Spelaren bryr sig om saker de investerat i.

### Task 32.1: IBond — emotionell koppling till NPCs
### Task 32.2: Investment moments som bygger bond
```csharp
npc.CreateBond("childhood_friend")
    .InvestmentMoments(
        "shared_danger",
        "revealed_secret",
        "saved_life"
    )
    .Payoff(ctx => {
        if (npc.Dies())
            ctx.ImpactWeight = BondStrength * 10;
    });
```
### Task 32.3: Varning om NPC dör utan established bond
### Task 32.4: Sandbox — vän som dör efter vi byggt relation

---

## Slice 33: Narrative Voice System

**Mål:** Flexibel berättarröst.

### Task 33.1: Voice enum: FirstPerson, SecondPerson, ThirdPerson
### Task 33.2: Tense enum: Past, Present
```csharp
game.SetNarrativeVoice(Voice.SecondPerson)
    .Tense(Tense.Present);

// Auto-adjusts all descriptions:
// 1st: "I enter the cave"
// 2nd: "You enter the cave"
// 3rd: "The hero enters the cave"
```
### Task 33.3: Flashbacks i past tense
### Task 33.4: Sandbox — byt perspektiv under spelet

---

## Slice 34: Player Agency Tracking

**Mål:** Anpassa story till spelarstil.

### Task 34.1: IAgencyTracker — spårar meningsfulla val
```csharp
choice.Register("SaveVillage", weight: 10);
choice.Register("HelpOldMan", weight: 3);
choice.Register("StealBread", weight: -5);
```
### Task 34.2: AgencyScore påverkar story paths
```csharp
if (player.AgencyScore > 50)
    story.Unlock("player_shapes_kingdom");
else
    story.Unlock("player_follows_fate");
```
### Task 34.3: Sandbox — aktiv vs passiv protagonist

---

## Slice 35: Dramatic Irony Tracker

**Mål:** Spänning när spelaren vet mer än karaktären.

### Task 35.1: IDramaticIronySystem — spårar kunskapsskillnader
```csharp
// Player learns:
player.LearnSecret("advisor_is_traitor");

// NPC doesn't know:
king.Dialog("My advisor is my most trusted friend");

// System detects irony:
if (dramaticIrony.Exists())
    player.ReceiveAction("warn_king");

// Consequences:
if (!player.WarnedKing())
    story.Consequence(Tragedy.KingBetrayed);
```
### Task 35.2: Ge spelaren chans att agera på kunskap
### Task 35.3: Sandbox — förrädarscenario

---

## Slice 36: Hero's Journey & Narrative Templates

**Mål:** Inbyggda dramaturgiska strukturer som guide för story design.

### Task 36.1: IHeroJourney + JourneyStage enum
Campbell's 12-17 stages med 3 faser (Departure, Initiation, Return)

### Task 36.2: HeroJourneyBuilder — fluent API
```csharp
game.UseHeroJourneyTemplate()
    .OrdinaryWorld("village")
        .EstablishNormalcy("You've lived here all your life...")
    .CallToAdventure()
        .Trigger(call => call.OnEvent("village_attacked"))
    .MeetingMentor("wise_hermit")
        .Provides(gift => gift.Item("magic_amulet"))
    .CrossingThreshold()
        .PointOfNoReturn()
    .Ordeal()
        .FaceGreatestFear("dragon")
    .ReturnWithElixir()
        .TransformationComplete(hero => hero.SetTitle("Dragonslayer"))
    .Build();
```

### Task 36.3: JourneyValidator
- Varnar om saknade stages
- Kontrollerar ordning (Reward före Ordeal = fel)
- "Mentor saknas före Threshold"

### Task 36.4: Character Archetypes
```csharp
public enum CharacterArchetype
{
    Hero, Mentor, Threshold_Guardian, Herald,
    Shapeshifter, Shadow, Ally, Trickster
}
npc.SetArchetype(CharacterArchetype.Mentor)
    .DiesAt(JourneyStage.Ordeal);
```

### Task 36.5: Alternative Narrative Templates

**The Tragic Arc (Aristoteles)**
```csharp
game.UseTragicArc()
    .Hybris("hero_overconfident")
    .Hamartia("fatal_flaw")  // Felsteget
    .Peripeteia("reversal")  // Oundviklig konsekvens
    .Anagnorisis("recognition")  // Insikt för sent
    .Katharsis("audience_emotion");
```

**The Transformation Arc (inre resa)**
```csharp
game.UseTransformationArc()
    .FragmentedIdentity()
    .ConfrontShadow()
    .Integration()
    .NewSelfImage();
```

**The Ensemble Journey (kollektiv)**
```csharp
game.UseEnsembleJourney()
    .Protagonists("luke", "leia", "han")
    .ShiftingPerspectives()
    .InternalConflicts()
    .NoSingleSavior();
```

**The Descent / Katabasis**
```csharp
game.UseDescentArc()
    .DescentIntoChaos()
    .LossOfControl()
    .ConfrontDeath()
    .ReturnChanged();  // eller .NoReturn()
```

**The Spiral Narrative**
```csharp
game.UseSpiralNarrative()
    .RepeatEvents()
    .WithVariations()
    .DeeperUnderstandingEachLoop()
    .TimeLoops();
```

**The Moral Labyrinth**
```csharp
game.UseMoralLabyrinth()
    .NoCorrectEnding()
    .AllChoicesCost()
    .SituationalTruth();
```

**The Caretaker Arc**
```csharp
game.UseCaretakerArc()
    .RepairNotConquer()
    .HealStabilizeProtect()
    .FightEntropy();
```

**The Witness Arc**
```csharp
game.UseWitnessArc()
    .ObserveMoreThanAct()
    .CollectStories()
    .AssembleTruth()
    .ChangeByUnderstanding();
```

**The World-Shift Arc**
```csharp
game.UseWorldShiftArc()
    .GradualWorldChange()
    .PlayerAsCatalyst()  // Inte hjälte
    .SystemsCollide("ecology", "politics", "magic")
    .NewEquilibrium();
```

### Task 36.6: DSL för journey templates
```
journey "DragonSlayer" type: hero {
    phase departure {
        ordinary_world "village" { routine: farming }
        call trigger: event "village_attacked"
        mentor "hermit" gives: "amulet"
        threshold "dark_forest" no_return: true
    }
    phase initiation {
        tests count: 3
        ordeal "dragon" death_rebirth: true
        reward: "dragon_hoard"
    }
    phase return {
        road_back pursuit: "dragon_mate"
        elixir title: "Dragonslayer"
    }
}
```

### Task 36.7: Journey Progress Tracker
```
=== YOUR JOURNEY ===
Phase: Initiation
Stage: 6/12

[✓][✓][✓][✓][✓][●][ ][ ][ ][ ][ ][ ]
 1  2  3  4  5  6  7  8  9 10 11 12

Next: Approach Inmost Cave
```

### Task 36.8: Sandbox — Star Wars journey, Tragic alternative

---

## Slice 37: Generic Chapter System

**Mål:** Flexibel kapitelstruktur utan låst template. Bygg din egen arc.

### Task 37.1: IChapter + ChapterState
```csharp
public interface IChapter
{
    string Id { get; }
    string Title { get; }
    ChapterState State { get; }  // NotStarted, Active, Completed, Skipped
    IEnumerable<IChapterObjective> Objectives { get; }
    IChapter? NextChapter { get; }
}
```

### Task 37.2: ChapterBuilder — define custom arcs
```csharp
game.DefineChapters()
    .Chapter("prologue", "The Beginning")
        .Objectives(o => o
            .Required("wake_up")
            .Required("leave_house")
            .Optional("talk_to_neighbor"))
        .OnComplete(ctx => ctx.Message("Chapter 1 begins..."))

    .Chapter("chapter1", "Into the Unknown")
        .UnlockedWhen(c => c.IsComplete("prologue"))
        .Objectives(o => o
            .Required("reach_forest")
            .Required("find_map")
            .Branch("help_stranger", leadsTo: "chapter2a")
            .Branch("ignore_stranger", leadsTo: "chapter2b"))

    .Chapter("chapter2a", "The Ally Path")
        // ...

    .Chapter("chapter2b", "The Lone Path")
        // ...

    .Chapter("finale", "The End")
        .ConvergesFrom("chapter2a", "chapter2b")
        .MultipleEndings(endings => endings
            .Ending("good", when: w => w.GetCounter("karma") > 50)
            .Ending("bad", when: w => w.GetCounter("karma") < -50)
            .Ending("neutral", isDefault: true))

    .Build();
```

### Task 37.3: Chapter transitions och branching
```csharp
// Auto-advance
game.OnObjectiveComplete("find_map", ctx =>
    ctx.AdvanceChapter());

// Manual control
game.CurrentChapter.Complete();
game.ActivateChapter("chapter2a");
```

### Task 37.4: Chapter progress UI
```
╔══════════════════════════════════════╗
║  CHAPTER 2: Into the Unknown         ║
╠══════════════════════════════════════╣
║  [✓] Reach the forest                ║
║  [●] Find the ancient map            ║
║  [ ] Choose your path                ║
╚══════════════════════════════════════╝
```

### Task 37.5: DSL för chapters
```
chapters {
    chapter "prologue" title: "The Beginning" {
        objectives {
            required "wake_up"
            required "leave_house"
            optional "talk_neighbor"
        }
        on_complete: message "Chapter 1 begins..."
    }

    chapter "chapter1" title: "Into the Unknown" {
        unlocked_when: complete "prologue"
        branches {
            "help_stranger" -> "chapter2a"
            "ignore_stranger" -> "chapter2b"
        }
    }

    chapter "finale" {
        converges_from: ["chapter2a", "chapter2b"]
        endings {
            "good" when: karma > 50
            "bad" when: karma < -50
            "neutral" default: true
        }
    }
}
```

### Task 37.6: Sandbox — 3-chapter mini-adventure med branching

---

## Slice 38: Time/Action Triggered Objects & Doors

**Mål:** Objekt och dörrar som spawnar/öppnas baserat på tid eller actions.

### Task 38.1: ITimedSpawn — objekt som dyker upp
```csharp
location.AddTimedSpawn("treasure_chest")
    .AppearsAt(tick: 100)
    .Or()
    .AppearsWhen(w => w.GetFlag("dragon_dead"))
    .DisappearsAfter(ticks: 50)  // Begränsad tid
    .Message("A treasure chest materializes from thin air!");

location.AddTimedSpawn("ghost")
    .AppearsAt(TimePhase.Night)
    .DisappearsAt(TimePhase.Dawn);
```

### Task 38.2: ITimedDoor — dörrar som öppnas/stängs
```csharp
location.AddExit(Direction.North, secretRoom)
    .WithTimedDoor("magic_portal")
        .OpensAt(tick: 50)
        .ClosesAt(tick: 100)
        .Or()
        .OpensWhen(w => w.TimePhase == TimePhase.Midnight)
        .Message("The portal shimmers into existence...");

location.AddExit(Direction.East, vault)
    .WithTimedDoor("bank_vault")
        .OpensAt(GameTime.Hours(9))   // 9 AM
        .ClosesAt(GameTime.Hours(17)) // 5 PM
        .ClosedMessage("The bank is closed.");
```

### Task 38.3: Action-triggered spawns
```csharp
// Object appears after player action
game.OnAction("pull_lever", ctx => {
    ctx.SpawnItem("hidden_key", in: "throne_room");
    ctx.Message("You hear a click. Something fell somewhere...");
});

// Object appears after NPC dies
game.OnNpcDeath("dragon", ctx => {
    ctx.SpawnItem("dragon_heart", in: ctx.Location);
    ctx.OpenDoor("treasure_vault");
});

// Chain reaction
game.OnItemPickup("crystal", ctx => {
    ctx.After(ticks: 10, () => {
        ctx.CollapseDoor("entrance");
        ctx.Message("The cave begins to collapse!");
    });
});
```

### Task 38.4: Conditional permanent changes
```csharp
// Door permanently opens after condition
door.PermanentlyOpensWhen(w =>
    w.HasItem("master_key") || w.GetFlag("lockpick_master"));

// Location transforms
location.TransformsInto("ruined_village")
    .When(w => w.GetCounter("village_fires") >= 10)
    .Irreversible();
```

### Task 38.5: Scheduled events queue
```csharp
game.Schedule()
    .At(tick: 0, ctx => ctx.Message("Your journey begins..."))
    .At(tick: 50, ctx => ctx.SpawnNpc("merchant", "crossroads"))
    .At(tick: 100, ctx => ctx.TriggerEvent("storm_begins"))
    .At(tick: 200, ctx => ctx.OpenDoor("flooded_passage"))
    .Every(ticks: 25, ctx => ctx.SpawnRandomEncounter("forest"));
```

### Task 38.6: DSL för timed objects
```
location "ancient_temple" {
    timed_spawn "ghost_guardian" {
        appears_at: night
        disappears_at: dawn
    }

    timed_door "sun_door" direction: east {
        opens_when: time_phase == "day"
        closes_when: time_phase == "night"
        message: "Sunlight reveals a hidden passage"
    }

    action_spawn on: "place_gem_in_altar" {
        spawn: "portal" type: exit direction: up
        message: "A portal opens above the altar!"
    }
}

schedule {
    at tick:100 -> spawn "wandering_merchant" in "crossroads"
    every tick:50 -> random_event "forest_encounters"
    when flag:"volcano_active" -> transform "village" into "destroyed_village"
}
```

### Task 38.7: Sandbox — tempel där dörrar öppnas vid rätt tid, skatt spawnar efter boss

---

---

## Future Roadmap (v2+)

### Världssimulation

**Time System**
- Dag/natt-cykler, `TicksPerDay`
- Delayed events: `After(ticks: 50, ctx => ctx.TriggerEvent("cave_collapse"))`
- Saker förändras över tid (blod torkar, eld slocknar, NPC blir trött)
- TimePhase: Dawn, Day, Dusk, Night

**Perception System**
- LightLevel per location (mörker kräver fackla)
- Synfält, dimma, blindhet
- Hörsel: ljud från angränsande rum
- Minnesystem: vad spelaren har sett, hört, lärt sig
- Rykten och osäker information

**Semantic Objects**
- Traits: `Flammable, Brittle, Sharp, Heavy, Holy, Cursed, Frozen, Hot`
- Affordances: vad kan man rimligen göra med objektet
- Emergent interactions: `ice + fire → water` utan hårdkodade recept
- Fysikaliska relationer: vätskor, gaser, temperatur

**Social System**
- Relationer: Trust, Fear, Loyalty, Guilt
- Rykte som sprids mellan NPCs
- Minnesbaserade reaktioner ("du svek mig förut")
- Gruppdynamik (NPC påverkar varandra)

**Intention System**
- Målbaserade handlingar: `player.Intent("escape the cave")`
- Delplaner: om låst → hitta nyckel → gå tillbaka
- Avbrutna handlingar (börjar klättra, blir attackerad)
- Kräver AI-integration

**Narrative Logic**
- Teman och motiv (rädsla, skuld, hopp)
- Foreshadowing-hooks
- Dramaturgiska tillstånd (uppbyggnad, kris, klimax)
- "Författarmedvetenhet" i ramverket

**Meta Perspective**
- Undo / spola tillbaka (tidslinjer)
- What-if scenarier
- Debug-visning av orsak–verkan

---

### Developer Experience

**Prose-like Fluent API**
```csharp
World.Room("Cave")
    .Description("A dark, wet cave")
    .NorthTo("Forest")
    .WithDoor("IronGate").LockedBy("RustyKey")
    .Contains(item => item.Sword("Old Blade"))
    .Contains(npc => npc.Hermit().Hostile());
```

**DSL för författare**
```
room "Cave" {
    description "A dark cave"
    exit north -> "Forest" locked by "RustyKey"
    contains {
        sword "Old Blade"
        npc "Hermit" hostile
    }
}
```

**Extension Hooks överallt**
```csharp
game.OnEnter("Cave", state => {
    state.Spawn("Bat");
    state.Message("You feel watched.");
});

game.OnUse("Torch", "Curtain", ctx => ctx.StartFire());
game.OnTurn(state => state.AdvanceTime());
game.OnEmotionChange("Hermit", Emotion.Angry, ctx => ctx.Attack());
```

**Archetypes / Prefabs**
```csharp
World.Add(Archetypes.DragonBoss()
    .WithName("Vermithrax")
    .Guards("GoldenHall")
    .WeakTo(Element.Fire));

World.Add(Archetypes.LockedTreasureChest()
    .In("Cave")
    .RequiresKey("GoldenKey")
    .Contains("DiamondRing"));

World.Add(Archetypes.WanderingMerchant()
    .Patrol("Town", "Forest", "Cave")
    .Sells("Potion", "Map", "Torch"));
```

**Visual World Inspector**
- Graf av rum och kopplingar
- NPC-rörelser i realtid
- Quest states
- "Varför misslyckades kommandot?" → orsakskedja

**Story-First Tooling**
- Story beats editor
- Quest-flödesgraf
- Relationsgraf
- Tidslinje
- Genererar builder-kod eller DSL

---

### Arkitekturvision

```
┌─────────────────────────────────────────────────────┐
│                   Story Tools                        │
│         (Visual Editor, Quest Graph, Timeline)       │
└─────────────────────┬───────────────────────────────┘
                      │ generates
┌─────────────────────▼───────────────────────────────┐
│                    DSL Layer                         │
│              (.adventure files)                      │
└─────────────────────┬───────────────────────────────┘
                      │ parses to
┌─────────────────────▼───────────────────────────────┐
│               Fluent Builder API                     │
│          (C# prose-like builders)                    │
└─────────────────────┬───────────────────────────────┘
                      │ builds
┌─────────────────────▼───────────────────────────────┐
│                 Core Engine                          │
│    (Interfaces, Events, State, Commands)             │
├─────────────────────────────────────────────────────┤
│  Modules:                                            │
│  ├── Combat (Strategy)                               │
│  ├── Social (Relationships)                          │
│  ├── Time (Day/Night, Delays)                        │
│  ├── Perception (Light, Sound, Memory)               │
│  ├── Semantics (Traits, Affordances)                 │
│  ├── Narrative (Themes, Foreshadowing)               │
│  └── AI (Ollama, Intent parsing)                     │
└─────────────────────────────────────────────────────┘
```

---

## Sammanfattning av slices

### Core Engine (v1)
| # | Slice | Patterns | Status |
|---|-------|----------|--------|
| 1 | Location + Navigation | - | ⬜ |
| 2 | Doors + Keys | State | ⬜ |
| 3 | Command + Parser | Command, Chain of Responsibility | ⬜ |
| 4 | Items + Inventory | Factory, Prototype, Decorator, Composite | ⬜ |
| 5 | NPCs + Dialog + Movement | State, Composite, Strategy | ⬜ |
| 6 | Event System | Observer | ⬜ |
| 7 | Combat | Strategy, Command | ⬜ |
| 8 | Quest System | State, Visitor | ⬜ |
| 9 | World State System | - | ⬜ |
| 10 | Save/Load | Memento | ⬜ |
| 11 | Language System | Flyweight | ⬜ |
| 12 | DSL Parser | Template Method | ⬜ |
| 13 | GameBuilder | Builder, Facade | ⬜ |
| 14 | Loggers | Observer, Proxy | ⬜ |
| 15 | Pathfinder | Strategy | ⬜ |
| 16 | AI-paket | Facade, Strategy | ⬜ |
| 17 | NuGet-paketering | - | ⬜ |

### Advanced Systems (v1.5)
| # | Slice | Patterns | Status |
|---|-------|----------|--------|
| 18 | Story Branches & Consequences | State | ⬜ |
| 19 | Multi-Stage Quests | State, Strategy | ⬜ |
| 20 | Conditional Event Chains | Chain of Responsibility | ⬜ |
| 21 | Time System | Observer, Strategy | ⬜ |
| 22 | Faction & Reputation | Observer | ⬜ |
| 23 | Random Event Pool | Strategy | ⬜ |
| 24 | Location Discovery | - | ⬜ |
| 25 | Story Mapper Tool | - | ⬜ |

### Storytelling Systems (v2)
| # | Slice | Patterns | Status |
|---|-------|----------|--------|
| 26 | Mood & Atmosphere | State, Observer | ⬜ |
| 27 | Dynamic Descriptions | Strategy, Template | ⬜ |
| 28 | Character Arc Tracking | State | ⬜ |
| 29 | Pacing & Tension | Observer, State | ⬜ |
| 30 | Foreshadowing & Callbacks | Observer | ⬜ |
| 31 | Scene Transitions & Beats | State, Command | ⬜ |
| 32 | Emotional Stakes | Observer | ⬜ |
| 33 | Narrative Voice | Strategy | ⬜ |
| 34 | Player Agency Tracking | Observer | ⬜ |
| 35 | Dramatic Irony Tracker | Observer | ⬜ |
| 36 | Hero's Journey & Narrative Templates | Template, Strategy, Builder | ⬜ |
| 37 | Generic Chapter System | State, Builder | ⬜ |
| 38 | Time/Action Triggered Objects | Observer, Scheduler | ⬜ |

### Polish & Documentation (v2+)
| # | Slice | Patterns | Status |
|---|-------|----------|--------|
| 39 | Fluent API & Språksnygghet | Builder, Factory | ⬜ |
| 40 | GitHub Wiki (TextAdventure.wiki) | - | ⬜ |

---

## Slice 39: Fluent API & Språksnygghet

**Mål:** All syntaktisk socker för snygg, läsbar kod.

### Item Description
- `Models/Item.cs` - lägg till `Description` property
- `Interfaces/IItem.cs` - lägg till `string? Description { get; }`
- Fluent method: `SetDescription(string description)`

### Bulk creation - Items.CreateMany
```csharp
public static class Items
{
    // Tuple-baserad (JSON-tänk)
    public static IEnumerable<Item> CreateMany(
        params (string id, string name, float weight)[] items);

    // Med description
    public static IEnumerable<Item> CreateMany(
        params (string id, string name, float weight, string desc)[] items);
}
```

### Inline DSL - Location.AddDSLItems
```csharp
// Syntax: "Name(weight, takeable|fixed)? | description?"
location.AddDSLItems(
    "Sword(2.5kg, takeable)",
    "Shield(5kg)",
    "Torch",
    "Statue(fixed)",
    "Note | A crumpled letter",
    "Gem(0.1kg) | A sparkling ruby"
);
```

Parser regex: `^(?<name>[\w\s]+)(\((?<props>[^)]+)\))?(\s*\|\s*(?<desc>.+))?$`

### Snabb-add för enkla items
```csharp
// Implicit conversion gör detta möjligt
location.AddItems("Sword", "Shield", "Torch");
```

### Random Extensions (int)

- [ ] `Extensions/RandomExtensions.cs`

```csharp
public static class RandomExtensions
{
    private static readonly Random _rng = new();

    /// <summary>10.Random() → 0-10</summary>
    public static int Random(this int max) => _rng.Next(max + 1);

    /// <summary>10.Random(5) → 5-10</summary>
    public static int Random(this int max, int min) => _rng.Next(min, max + 1);

    /// <summary>6.Dice() → 1-6 (aldrig 0)</summary>
    public static int Dice(this int sides) => _rng.Next(1, sides + 1);

    /// <summary>6.Dice(2) → 2d6 (2-12)</summary>
    public static int Dice(this int sides, int count)
    {
        var total = 0;
        for (var i = 0; i < count; i++)
            total += sides.Dice();
        return total;
    }
}
```

**Användning:**
```csharp
var damage = 6.Dice();           // 1d6 → 1-6
var attack = 20.Dice();          // 1d20 → 1-20
var fireball = 6.Dice(3);        // 3d6 → 3-18
var loot = 100.Random();         // 0-100
var enemyCount = 5.Random(2);    // 2-5
```

### Probability Extensions

- [ ] `Extensions/ProbabilityExtensions.cs`

```csharp
public static class ProbabilityExtensions
{
    private static readonly Random _rng = new();

    /// <summary>50.PercentChance() → true/false</summary>
    public static bool PercentChance(this int percent) =>
        _rng.Next(100) < percent;

    /// <summary>0.3.Chance() → 30% chans</summary>
    public static bool Chance(this double probability) =>
        _rng.NextDouble() < probability;
}
```

### Collection Extensions

- [ ] `Extensions/CollectionExtensions.cs`

```csharp
public static class CollectionExtensions
{
    private static readonly Random _rng = new();

    public static T PickRandom<T>(this IList<T> list) =>
        list[_rng.Next(list.Count)];

    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source) =>
        source.OrderBy(_ => _rng.Next());

    public static T WeightedRandom<T>(this IEnumerable<T> source, Func<T, int> weightSelector)
    {
        var items = source.ToList();
        var totalWeight = items.Sum(weightSelector);
        var roll = _rng.Next(totalWeight);
        var cumulative = 0;
        foreach (var item in items)
        {
            cumulative += weightSelector(item);
            if (roll < cumulative) return item;
        }
        return items.Last();
    }
}
```

### Time Extensions

- [ ] `Extensions/TimeExtensions.cs`

```csharp
public static class TimeExtensions
{
    public static TimeSpan Milliseconds(this int ms) => TimeSpan.FromMilliseconds(ms);
    public static TimeSpan Seconds(this int s) => TimeSpan.FromSeconds(s);
    public static TimeSpan Minutes(this int m) => TimeSpan.FromMinutes(m);
}
```

### Console Extensions (OBS: Endast för Console.Write!)

- [ ] `Extensions/ConsoleExtensions.cs`

```csharp
/// <summary>
/// VIKTIGT: Dessa extensions fungerar ENDAST med Console.Write.
/// Om du använder egen output-hantering, implementera IGameOutput istället.
/// </summary>
public static class ConsoleExtensions
{
    public static void TypewriterPrint(this string text, int delayMs = 50)
    {
        foreach (var c in text)
        {
            Console.Write(c);
            Thread.Sleep(delayMs);
        }
        Console.WriteLine();
    }
}
```

### Range/Clamp Extensions

- [ ] `Extensions/RangeExtensions.cs`

```csharp
public static class RangeExtensions
{
    public static int Clamp(this int value, int min, int max) =>
        Math.Max(min, Math.Min(max, value));

    public static bool IsBetween(this int value, int min, int max) =>
        value >= min && value <= max;
}
```

### Conditional Fluent Extensions

- [ ] `Extensions/ConditionalExtensions.cs`

```csharp
public static class ConditionalExtensions
{
    public static ConditionalResult<string> Then(this bool condition, string trueValue) =>
        new(condition, trueValue);

    public static ConditionalResult<T> Then<T>(this bool condition, Func<T> trueAction) =>
        new(condition, condition ? trueAction() : default);

    public static void Then(this bool condition, Action action)
    {
        if (condition) action();
    }
}

public class ConditionalResult<T>
{
    private readonly bool _condition;
    private readonly T? _value;

    public ConditionalResult(bool condition, T? value)
    {
        _condition = condition;
        _value = value;
    }

    public T Else(T falseValue) => _condition ? _value! : falseValue;
    public T Else(Func<T> falseAction) => _condition ? _value! : falseAction();
}
```

**Användning:**
```csharp
// Villkorlig text
var desc = isDark.Then("Pitch black.").Else("Sunlight streams in.");

// Villkorlig action
hasKey.Then(() => door.Unlock())
      .Else(() => Console.WriteLine("The door is locked."));

// Enkel trigger
isFirstVisit.Then(() => ShowIntro());
```

### Grammar Extensions (språkberoende - kräver ILanguage override!)

- [ ] `Extensions/GrammarExtensions.cs`
- [ ] `Interfaces/IGrammarProvider.cs`

```csharp
public interface IGrammarProvider
{
    string WithArticle(string noun);           // "a sword", "an apple", "ett svärd"
    string Plural(string noun, int count);     // "3 swords", "3 svärd"
    string NaturalList(IEnumerable<string> items);  // "sword, shield, and torch"
}

// Default implementation (English)
public class EnglishGrammar : IGrammarProvider { ... }

// Extensions använder registrerad provider
public static class GrammarExtensions
{
    public static IGrammarProvider Provider { get; set; } = new EnglishGrammar();

    public static string WithArticle(this string noun) => Provider.WithArticle(noun);
    public static string Plural(this string noun, int count) => Provider.Plural(noun, count);
    public static IEnumerable<string> ToNaturalList(this IEnumerable<string> items) => ...;
}
```

**OBS:** Byt `GrammarExtensions.Provider = new SwedishGrammar()` för svenska.

### Tester
- `FluentApiTests.cs` - test för CreateMany, AddDSLItems
- `RandomExtensionsTests.cs` - test för Random, Dice
- `ProbabilityExtensionsTests.cs` - test för PercentChance, Chance
- `CollectionExtensionsTests.cs` - test för PickRandom, Shuffle, WeightedRandom
- `ConditionalExtensionsTests.cs` - test för Then/Else
- `GrammarExtensionsTests.cs` - test för alla språk

---

## Slice 40: GitHub Wiki (TextAdventure.wiki)

**Mål:** Komplett dokumentation för användare.

### Wiki-sidor
- **Home** - Projektöversikt och vision
- **Getting Started** - Installation och första spelet
- **API Reference** - Fluent API dokumentation
- **Commands** - Alla inbyggda kommandon (go, take, look, etc.)
- **DSL Guide** - .adventure filformat
- **Examples** - Exempelspel (se nedan)
- **Localization** - Hur man lägger till nya språk
- **Extending** - Skapa egna commands, parsers, etc.
- **Storytelling Guide** - Översikt narrativa verktyg
- **Narrative Arcs** - Alla mallar (se nedan)

### Wiki: Narrative Arcs

Alla inbyggda narrativa mallar med beskrivning och användningsområden.

| Arc | Struktur | Bra för |
|-----|----------|---------|
| **Hero's Journey** | Call → Trials → Transformation → Return | Klassiska äventyr, fantasy |
| **Tragic Arc** | Hybris → Felsteg → Konsekvens → Sen insikt | Mörka berättelser, moraliska val |
| **Transformation Arc** | Fragmenterad → Skuggkonfrontation → Integration → Ny självbild | Psykologisk mognad, trauma, sorg |
| **Ensemble Journey** | Flera hjältar → Växlande perspektiv → Gruppkonflikter → Kollektiv seger | Politik, motstånd, "Jedi Council" |
| **The Descent** | Nedstigning → Kontrollförlust → Möte med tomhet → Förändrad återkomst | Psykologisk skräck, Silent Hill |
| **Spiral Narrative** | Upprepning → Små variationer → Djupare förståelse | Tidsloopar, minnesglitchar |
| **Moral Labyrinth** | Inget rätt slut → Alla val kostar → Situationsbunden sanning | Etik, ledarskap, ansvar |
| **World-Shift Arc** | Gradvis världsförändring → Spelaren som katalysator → Ny jämvikt | Dune-stil, ekologi, politik |
| **Caretaker Arc** | Reparera → Hela → Skydda → Kamp mot entropi | Mogna, lågmälda spel |
| **Witness Arc** | Observera → Samla berättelser → Sanning genom förståelse | Textmysterier, detektiv |

### Exempelspel - Creation Styles (ett per stil)

| Stil | Spel | Beskrivning |
|------|------|-------------|
| **Fluent Builder** | The Haunted Manor | Klassisk spökhistoria, visar method chaining |
| **Implicit Conversion** | Quick Escape | Minimalistiskt rum-escape, snabbaste sättet |
| **Factory/Tuple** | Dungeon Loot | Massa items, visar JSON-tänk bulk creation |
| **DSL** | Forest Adventure | Config-fil driven, visar `.adventure` format |
| **Mixed** | The Complete Quest | Kombinerar alla stilar, best practices |

### Exempelspel - Storytelling Features

Dessa ska vara så fluent som möjligt och visa varje storytelling-funktion.

#### 🎭 Narrativ Struktur & Arcs
| Feature | Spel | Visar |
|---------|------|-------|
| **Hero's Journey** | The Chosen One | Full 12-stegs resa |
| **Tragic Arc** | The King's Folly | Hybris → fall → sen insikt |
| **Transformation Arc** | Shattered Mirror | Inre resa, trauma, integration |
| **Ensemble Journey** | The Resistance | Flera hjältar, växlande perspektiv |
| **The Descent** | Into the Abyss | Katabasis, psykologisk skräck |
| **Spiral Narrative** | Groundhog Dungeon | Tidsloop med variationer |
| **Moral Labyrinth** | The Tribunal | Inga rätta svar, etiska val |
| **World-Shift Arc** | Seeds of Change | Spelaren som katalysator |
| **Caretaker Arc** | The Lighthouse Keeper | Reparera, hela, skydda |
| **Witness Arc** | The Collector | Observera, samla sanningen |
| **Chapters** | The Saga | Akter, kapitelövergångar |
| **Scene Beats** | The Interview | Dialog-driven, dramatiska pauser |

#### 👥 Karaktärer & Relationer
| Feature | Spel | Visar |
|---------|------|-------|
| **Character Arcs** | The Reluctant Hero | NPC utvecklas genom spelarens val |
| **Emotional Stakes** | The Last Goodbye | Relationer, förlust, val med konsekvenser |

#### 🌙 Atmosfär & Beskrivningar
| Feature | Spel | Visar |
|---------|------|-------|
| **Mood & Atmosphere** | The Lighthouse | Väder, ljus, ljud påverkar beskrivningar |
| **Dynamic Descriptions** | The Living Castle | Rum ändras baserat på tid/händelser |
| **Narrative Voice** | Noir Detective | Berättarröst, stiliserad text |

#### ⏱️ Spänning & Tempo
| Feature | Spel | Visar |
|---------|------|-------|
| **Pacing & Tension** | Countdown | Ökande press, timer-baserad spänning |
| **Foreshadowing** | Murder Mystery | Ledtrådar som kopplas ihop senare |
| **Time Triggers** | The Bomb | Objekt aktiveras efter tid/händelser |

#### 🎮 Spelarupplevelse
| Feature | Spel | Visar |
|---------|------|-------|
| **Player Agency** | Branching Paths | Val som faktiskt spelar roll |
| **Dramatic Irony** | The Traitor | Spelaren vet mer än karaktären |

### Sandbox kommentarer

Sandbox ska visa alla stilar med kommentarer:

```csharp
// ============================================
// ITEM CREATION STYLES - Choose your favorite!
// ============================================

// 1. FLUENT BUILDER - Most readable, full control
var sword = new Item("sword", "Rusty Sword")
    .SetWeight(2.5f)
    .SetTakeable(true)
    .SetDescription("A worn blade");

// 2. IMPLICIT CONVERSION - Fastest for simple items
Item torch = "Torch";

// 3. FACTORY/TUPLE - JSON-like bulk creation
var items = Items.CreateMany(
    ("gem", "Ruby Gem", 0.1f),
    ("coin", "Gold Coin", 0.05f)
);

// 4. DSL - Most compact, config-file friendly
cave.AddDSLItems(
    "Gem(0.1kg) | A sparkling ruby",
    "Torch",
    "Statue(fixed)"
);

// 5. PARAMS ARRAY - Quick add simple items
hall.AddItems("Sword", "Shield", "Torch");
```

----

En tanke på hur det ska fungera

*storytelling som kodstruktur*, inte som löpande text.

Om vi tänker rent arkitektoniskt kan du se berättelsen som tre lager som samverkar:

1. **Tillstånd (State)** – hur världen *är*
2. **Händelser (Events)** – vad som *händer*
3. **Mening (Narrative Logic)** – varför det *betyder något*

I kod kan det bli ungefär så här.

---

### 1. Story som tillståndsmaskin

Berättelsen är inte en linje, utan ett nät av tillstånd:

```csharp
StoryState {
    WorldMood: Fear | Hope | Decay | Trust
    PlayerInnerState: Confused | Determined | Guilty | Healed
    Factions: { Order: Weakening, Chaos: Rising }
}
```

Alltså: inte “kapitel 1, kapitel 2”, utan *existentiella lägen*.

---

### 2. Story Beats som kodobjekt

Varje betydelsefull händelse är ett objekt med:

```csharp
StoryBeat {
    Id: "MeetTheHermit"
    Preconditions: Player.Has("Lantern") && WorldMood == Fear
    Effect:
        WorldMood = Hope
        PlayerInnerState = LessAlone
    Symbol: "Light in Darkness"
    MemoryTag: "FirstTrust"
}
```

Detta är inte bara quest-logik.
Det är dramaturgisk semantik.

---

### 3. Tematiska bågar som system, inte manus

```csharp
ThemeArc "Trust" {
    StartsAt: Isolation
    DevelopsThrough: Betrayal, Repair
    ResolvesAt: MutualDependence
}
```

Och varje StoryBeat deklarerar vilket tema den påverkar:

```csharp
AffectsTheme("Trust", +0.2)
```

---

### 4. Karaktärer som narrativa processer

NPC är inte bara state machines, utan *pågående berättelser*:

```csharp
CharacterArc "Hermit" {
    Wound: "LostFamily"
    Mask: "Cynical"
    Need: "Connection"
    TransformationCondition: PlayerShowsConsistency
}
```

---

### 5. Orsak → Konsekvens → Betydelse

Varje handling får tre lager:

```csharp
Action: OpenDoor
Consequence: ReleaseMonster
Meaning: "Curiosity has a cost"
```

Motorn kan då visa olika berättarröster:

– Mekanisk: “The door opens. A creature emerges.”
– Narrativ: “Some things were sealed for a reason.”
– Psykologisk: “You feel responsible.”

---

### 6. Struktur som kan bli DSL

Exempel på framtida “Story DSL”:

```dsl
beat "LightInTheDark" {
    when player.enters Cave and player.has Lantern
    shift world.mood from Fear to Hope
    reveal memory "You are not alone"
    mark theme Trust += 1
}
```

---

Om jag ska sammanfatta kärnan:

Berättelse i kod =
inte text → struktur
inte sekvens → relation
inte plot → tillståndsförändring med mening

Du bygger inte:

“En historia.”

Du bygger:

“Ett system som skapar meningsfull förändring.”

## Exempel

OK vi säger exempelvis ett spel där spelaren  ligger i sin säng och ska gå till köket göra kaffe, ta kaffe, gå till vardagsrum, hämta en bok, sätta sig i soffan dricka kaffe och läsa boken hur skulle det beskrivas i detta fluent state

Bra, det här är ett perfekt mikroscenario att visa hur “story som tillstånd” fungerar.

Vi beskriver det inte som steg, utan som **meningsfulla övergångar mellan tillstånd**.

Först definierar vi världens och spelarens narrativa tillstånd:

```csharp
StoryState {
    Location: Bedroom
    PlayerPosture: LyingInBed
    WorldMood: MorningStillness
    PlayerNeed: WakeUp
    InnerState: Drowsy
}
```

Nu definierar vi *beats* som inte bara är handlingar, utan förändrar tillstånd och mening.

### Beat 1 – Stiga upp

```csharp
StoryBeat "LeaveBed" {
    Preconditions:
        Location == Bedroom
        PlayerPosture == LyingInBed

    Effects:
        PlayerPosture = Standing
        InnerState = Groggy
        Theme("Transition") += 0.2

    Meaning:
        "The day begins. Inertia is broken."
}
```

### Beat 2 – Gå till köket och göra kaffe

```csharp
StoryBeat "BrewCoffee" {
    Preconditions:
        Location == Kitchen
        PlayerHas("Mug")
        PlayerNeed == WakeUp

    Effects:
        PlayerHas("Coffee")
        InnerState = WarmingUp
        WorldMood = Awakening
        Theme("Comfort") += 0.3

    Meaning:
        "Warmth enters the body. Consciousness returns."
}
```

### Beat 3 – Ta boken i vardagsrummet

```csharp
StoryBeat "FindBook" {
    Preconditions:
        Location == LivingRoom
        BookIsPresent
        InnerState >= Awake

    Effects:
        PlayerHas("Book")
        Theme("Contemplation") += 0.2

    Meaning:
        "The mind reaches for stories."
}
```

### Beat 4 – Sätta sig, dricka kaffe, läsa

Det här är kul: det är egentligen *ett nytt existentiellt tillstånd*.

```csharp
StoryBeat "SettleWithBook" {
    Preconditions:
        Location == LivingRoom
        PlayerHas("Coffee")
        PlayerHas("Book")
        PlayerPosture == Standing

    Effects:
        PlayerPosture = Sitting
        InnerState = CalmFocus
        WorldMood = QuietPresence
        PlayerNeed = None
        Theme("Rest") += 0.5
        Theme("Meaning") += 0.2

    Meaning:
        "The world slows. Attention becomes presence."
}
```

Nu kan motorn uttrycka samma händelse på tre nivåer:

### Mekanisk nivå

“You sit down. You drink coffee. You read the book.”

### Narrativ nivå

“With the warmth of the cup in your hands, the room grows still. The book opens like a small, private world.”

### Psykologisk nivå

“A sense of arrival replaces the morning’s restlessness.”

Allt detta kommer från **samma state transition**.

Det viktiga är att historien inte ligger i texten – den ligger i:

– Förändring av behov
– Förändring av inre tillstånd
– Förändring av stämning
– Tematisk progression

Så din “kaffe och bok”-sekvens är i själva verket:

Inertia → Awakening → Comfort → Presence

Det är redan en komplett dramaturgisk båge.

## Exempel på spelkörning

Om vi låter samma scenario spelas igenom i din motor, med både mekanik och narrativt lager, skulle en spelrunda kunna se ut ungefär så här:

---

**Starttillstånd**

> You are lying in bed.
> Morning light filters through the curtains.
> You feel heavy, half-awake.

Tillstånd:
Location = Bedroom
Posture = Lying
InnerState = Drowsy
Need = WakeUp

---

**Spelaren skriver:**
`get up`

Motorn matchar beat: *LeaveBed*

> You swing your legs over the edge of the bed and stand up.
> The day has begun, whether you like it or not.

Tillstånd nu:
Posture = Standing
InnerState = Groggy
Theme(Transition) +

---

**Spelaren:**
`go kitchen`

> The kitchen is quiet. The coffee maker waits on the counter.

Location = Kitchen

---

**Spelaren:**
`brew coffee`

Beat: *BrewCoffee*

> You fill the mug and start the machine.
> The smell of coffee slowly pushes the fog out of your mind.

Tillstånd:
HasCoffee = true
InnerState = WarmingUp
WorldMood = Awakening
Theme(Comfort) +

---

**Spelaren:**
`take coffee`
`go living room`

> The living room is still. A book lies on the table by the sofa.

---

**Spelaren:**
`take book`

Beat: *FindBook*

> You pick up the book. Its weight feels familiar.

HasBook = true
Theme(Contemplation) +

---

**Spelaren:**
`sit down`

Motorn ser: Location = LivingRoom, HasCoffee, HasBook, Posture = Standing
Beat: *SettleWithBook*

> You sit down on the sofa, the warm cup in your hands.
> You open the book.
> The world grows quiet in a way that feels intentional.

Tillstånd:
Posture = Sitting
InnerState = CalmFocus
WorldMood = QuietPresence
Need = None
Theme(Rest) +
Theme(Meaning) +

---

**Nu är berättelsen “klar” utan att vara slut.**

Spelaren har gått från:

Drowsy → Awake → Comforted → Present

Och systemet vet varför det är ett avslut:

– Behovet “WakeUp” är uppfyllt
– Inre tillstånd är stabilt
– Temat “Rest” har nått sitt mål
– Inga drivande konflikter kvar

Så motorn skulle kunna lägga till, helt automatiskt:

> For a moment, there is nothing you need to do.
> And that, too, is part of the story.

Det fina är att exakt samma struktur kan skala till:

– Trauma-berättelser
– Krigsdraman
– Kärlek
– Skräck
– Existentiell tomhet

Det är bara tillstånd, behov, teman och övergångar.
