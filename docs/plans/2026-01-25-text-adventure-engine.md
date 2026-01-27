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

### Task 5.1: INpc + Npc (State pattern — Friendly/Hostile/Dead) ✅

### Task 5.2: Dialog system (Composite — konversationsträd) ✅

### Task 5.3: NPC Movement (Strategy — None, Random, Patrol, Follow) ✅

### Task 5.4: TalkCommand ✅

### Task 5.5: Sandbox — prata med räven, drake patrullerar mellan grottor ✅

### Task 5.6: Rule-Based Dialog System (from Procedural Storytelling)

_Ersätter/kompletterar dialog-träd med flexibla regler._

**Koncept:** Dialog väljs baserat på världstillstånd, inte fördefinierad träd.

```csharp
// Definiera dialog-regler
npc.AddDialogRule("cat_comment")
    .When(ctx => ctx.Player.IsLookingAt("cat"))
    .When(ctx => !ctx.NpcMemory.HasSaid("cat_comment"))
    .Say("A cat! The Internet must be nearby.")
    .Then(ctx => ctx.NpcMemory.MarkSaid("cat_comment"));

// Specificitet vinner - regel med fler kriterier väljs
npc.AddDialogRule("greeting_rain")
    .When(ctx => ctx.FirstMeeting)
    .When(ctx => ctx.World.Weather == Weather.Rain)
    .Say("Terrible weather for traveling, stranger.");

npc.AddDialogRule("greeting_default")
    .When(ctx => ctx.FirstMeeting)
    .Say("Hello, stranger.")
    .Priority(0);  // Fallback

// Memory system
npc.AddDialogRule("remember_name")
    .When(ctx => ctx.NpcMemory.Knows("player_name"))
    .Say($"Ah, {ctx.NpcMemory.Get("player_name")}! Good to see you again.");

// Running gag / progress
npc.AddDialogRule("bird_joke_progress")
    .When(ctx => ctx.NpcMemory.GetCounter("bird_jokes") < 3)
    .Say("Why did the chicken cross the road? To get to the other side!")
    .Then(ctx => ctx.NpcMemory.Increment("bird_jokes"));

npc.AddDialogRule("bird_joke_exhausted")
    .When(ctx => ctx.NpcMemory.GetCounter("bird_jokes") >= 3)
    .Say("I'm all out of bird jokes, I'm afraid.");
```

**Automatiska triggers:**

```csharp
npc.OnSee("player")
    .After(seconds: 2)
    .SayOnce("Over here, traveler!");

npc.OnHear("combat")
    .Flee();
```

### Task 5.7: Synonym System (from Write Your Own Adventure Programs)

_Tillåt flera ord för samma handling._

```csharp
// I parser-konfiguration
parser.AddSynonyms("take", "get", "grab", "pick", "pickup");
parser.AddSynonyms("look", "examine", "inspect", "view", "check");
parser.AddSynonyms("go", "walk", "move", "head", "travel");
parser.AddSynonyms("attack", "hit", "strike", "fight", "kill");

// Item-specifika alias
item.AddAliases("sword", "blade", "weapon");

// Automatisk normalisering i parser
// "get sword" → "take sword"
// "examine blade" → "look sword"
```

**"Did you mean?" suggestions:**

```csharp
parser.EnableSuggestions(true);
// Input: "tke sword"
// Output: "Unknown command. Did you mean 'take sword'?"
```

---

## Slice 6: Event System (Observer)

**Mål:** Triggers när saker händer.

### Task 6.1: IEventSystem + EventSystem (Observer) ✅

### Task 6.2: Inbyggda events (OnEnter, OnExit, OnPickup, OnDrop, OnTalk, OnCombatStart) ✅

### Task 6.3: Item/Door events kopplas till EventSystem ✅

### Task 6.4: Sandbox — drake vaknar när man går in i grottan ✅

---

## Slice 7: Combat (Strategy)

**Mål:** Utbytbart stridssystem.

### Task 7.1: ICombatSystem + TurnBasedCombat (Strategy) ✅

### Task 7.2: AttackCommand, FleeCommand ✅

### Task 7.3: Health/Damage system ✅

### Task 7.4: Sandbox — slåss mot draken ✅

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
**Notis:** Kodbasen använder för närvarande Slice 20 för Hints & Properties (se `docs/examples/20_Hints_and_Properties.md`).

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

### Task 36.7: Additional Story Structures (from book research)

**Prescriptive/Fill-in-the-blank (UX Storytelling)**

```csharp
game.UsePrescriptiveStructure()
    .Given("context")
    .When("event")
    .Then("outcome");
```

**Familiar to Foreign (Alice in Wonderland)**

```csharp
game.UseFamiliarToForeign()
    .FamiliarWorld("home")
    .TransitionEvent("rabbit_hole")
    .ForeignWorld("wonderland")
    .ReturnWithInsight();
```

**Framed Stories**

```csharp
// Now-Then-Now (flashback)
game.UseFramedNarrative()
    .Present("current_situation")
    .Flashback("memory")
    .ReturnWithNewUnderstanding();

// Me-Them-Me (comparison)
game.UseFramedNarrative()
    .MyPerspective()
    .TheirPerspective("other_character")
    .MyNewUnderstanding();

// Here-There-Here (Odysseus)
game.UseFramedNarrative()
    .StartLocation("home")
    .JourneyTo("foreign_land")
    .ReturnHome();
```

**Layered Stories**

```csharp
game.UseLayeredNarrative()
    .AddLayer(1, "surface_impression")
    .AddLayer(2, "deeper_context")
    .AddLayer(3, "hidden_truth")
    .RevealLayersProgressively();
```

### Task 36.8: Propp's Folktale Functions (Procedural Narrative)

_Från Procedural Storytelling in Game Design_

```csharp
public enum ProppFunction
{
    Absentation,        // En familjemedlem lämnar hemmet
    Interdiction,       // Hjälten får en varning
    Violation,          // Varningen ignoreras
    Reconnaissance,     // Skurken spanar
    Delivery,           // Skurken får info om hjälten
    Trickery,           // Skurken försöker lura
    Complicity,         // Hjälten blir lurad
    Villainy,           // Skurken skadar någon/något
    Mediation,          // Hjälten blir medveten om problemet
    Counteraction,      // Hjälten bestämmer sig för att agera
    Departure,          // Hjälten ger sig av
    DonorTest,          // Hjälten testas av givaren
    HeroReaction,       // Hjälten reagerar på testet
    AcquisitionOfAgent, // Hjälten får magiskt föremål
    Guidance,           // Hjälten leds till målet
    Struggle,           // Hjälten och skurken strider
    Branding,           // Hjälten märks
    Victory,            // Skurken besegras
    LiquidationOfLack,  // Det ursprungliga problemet löses
    Return,             // Hjälten återvänder
    Pursuit,            // Hjälten jagas
    Rescue,             // Hjälten räddas
    UnrecognizedArrival,// Hjälten kommer hem okänd
    UnfoundedClaims,    // Falsk hjälte gör anspråk
    DifficultTask,      // Hjälten får svår uppgift
    Solution,           // Uppgiften löses
    Recognition,        // Hjälten erkänns
    Exposure,           // Falsk hjälte avslöjas
    Transfiguration,    // Hjälten förändras
    Punishment,         // Skurken straffas
    Wedding             // Hjälten gift sig/kröns
}

// Användning
game.UseProppianStructure()
    .RequiredFunctions(
        ProppFunction.Absentation,
        ProppFunction.Villainy,
        ProppFunction.Departure,
        ProppFunction.AcquisitionOfAgent,
        ProppFunction.Victory,
        ProppFunction.Return
    )
    .OptionalFunctions(ProppFunction.Pursuit, ProppFunction.Rescue)
    .ValidateOrder(true);  // Funktioner måste komma i rätt ordning
```

### Task 36.9: Flashback System

_Från UX Storytelling: Now-Then-Now structure_

```csharp
// Definiera minnen
game.AddMemory("childhood_trauma")
    .SetTrigger(trigger => trigger.OnEnterLocation("old_house"))
    .SetContent("The smell brings it all back...")
    .SetDuration(3);  // Antal turns i flashback

// Eller med fluent trigger
location.TriggerFlashback("memory_id")
    .When(player.Has("locket"))
    .WithTransition("Your vision blurs...")
    .ReturnsTo(currentLocation);
```

### Task 36.10: Layer-based Descriptions

_Från UX Storytelling: Layered stories_

```csharp
location.SetLayeredDescription()
    .FirstVisit("A dusty old library.")
    .SecondVisit("You notice scratch marks on the floor.")
    .ThirdVisit("The scratches lead to a hidden panel!")
    .OnItem("magnifying_glass", "With the glass, you see fingerprints.");
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

**Status legend:** ✅ implemented in code · 🟨 demo/docs only · ⬜ planned
| # | Slice | Patterns | Status |
|---|-------|----------|--------|
| 1 | Location + Navigation | - | ✅ |
| 2 | Doors + Keys | State | ✅ |
| 3 | Command + Parser | Command, Chain of Responsibility | ✅ |
| 4 | Items + Inventory | Factory, Prototype, Decorator, Composite | ✅ |
| 5 | NPCs + Dialog + Movement | State, Composite, Strategy | ✅ |
| 6 | Event System | Observer | ✅ |
| 7 | Combat | Strategy, Command | ✅ |
| 8 | Quest System | State, Visitor | ✅ |
| 9 | World State System | - | ✅ |
| 10 | Save/Load | Memento | ✅ |
| 11 | Language System | Flyweight | ✅ |
| 12 | DSL Parser | Template Method | ✅ |
| 13 | GameBuilder | Builder, Facade | ✅ |
| 14 | Loggers | Observer, Proxy | 🟨 |
| 15 | Pathfinder | Strategy | 🟨 |
| 16 | AI-paket | Facade, Strategy | 🟨 |
| 17 | NuGet-paketering | - | 🟨 |

### Advanced Systems (v1.5)

| #   | Slice                             | Patterns           | Status |
| --- | --------------------------------- | ------------------ | ------ |
| 18  | Story Branches & Consequences     | State              | 🟨     |
| 19  | Multi-Stage Quests                | State, Strategy    | 🟨     |
| 20  | Hints & Properties (current impl) | -                  | ✅     |
| 21  | Time System                       | Observer, Strategy | ✅     |
| 22  | Faction & Reputation              | Observer           | ✅     |
| 23  | Random Event Pool                 | Strategy           | ✅     |
| 24  | Location Discovery                | -                  | ✅     |
| 25  | Story Mapper Tool                 | -                  | 🟨     |

### Storytelling Systems (v2)

| #   | Slice                                | Patterns                    | Status |
| --- | ------------------------------------ | --------------------------- | ------ |
| 26  | Mood & Atmosphere                    | State, Observer             | 🟨     |
| 27  | Dynamic Descriptions                 | Strategy, Template          | 🟨     |
| 28  | Character Arc Tracking               | State                       | 🟨     |
| 29  | Pacing & Tension                     | Observer, State             | 🟨     |
| 30  | Foreshadowing & Callbacks            | Observer                    | 🟨     |
| 31  | Scene Transitions & Beats            | State, Command              | 🟨     |
| 32  | Emotional Stakes                     | Observer                    | 🟨     |
| 33  | Narrative Voice                      | Strategy                    | 🟨     |
| 34  | Player Agency Tracking               | Observer                    | 🟨     |
| 35  | Dramatic Irony Tracker               | Observer                    | 🟨     |
| 36  | Hero's Journey & Narrative Templates | Template, Strategy, Builder | 🟨     |
| 37  | Generic Chapter System               | State, Builder              | 🟨     |
| 38  | Time/Action Triggered Objects        | Observer, Scheduler         | 🟨     |

### Polish & Documentation (v2+)

| #   | Slice                                 | Patterns             | Status |
| --- | ------------------------------------- | -------------------- | ------ |
| 39  | Fluent API & Språksnygghet            | Builder, Factory     | 🟨     |
| 40  | GitHub Wiki (TextAdventure.wiki)      | -                    | 🟨     |
| 41  | Testing & Validation Tools            | Visitor, Strategy    | ⬜     |
| 42  | Story-LINQ (Narrative Query Language) | LINQ, Builder, State | ⬜     |
| 43  | Map Generator                         | -                    | ⬜     |
| 44  | String Case Utilities                 | -                    | ✅     |
| 45  | Generic Fixes                         | -                    | ⬜     |

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

## Slice 40: GitHub Wiki (../TextAdventure.wiki)

**Mål:** Komplett dokumentation för användare på engelska

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
- **Story Structures** - (från böcker: Hero's Journey, Familiar-to-Foreign, Framed, Layered)
- **Propp's Functions** - Procedural narrative byggstenar
- **Rule-Based Dialog** - Left 4 Dead-stil dynamisk dialog
- **Testing Your Adventure** - (från Usborne: test allt, låt andra spela)
- **Story-LINQ Guide** - Narrativ state management med LINQ-syntax

### Wiki: Reference & Theory (Nörd-sektionen 🤓)

#### 📜 History of Interactive Fiction

- **1976**: Colossal Cave Adventure (Will Crowther) — den första
- **1977-1979**: Zork (MIT) — definierade genren
- **1979-1989**: Infocom-eran — kommersiell höjdpunkt
- **1990-tal**: Decline & IF-community bildas
- **2000-tal**: Renaissance via IFComp, Twine, Ink
- **Idag**: Narrative games, walking simulators, vårt framework

#### 🔬 Parser Theory

**Tokenization:**

```
"take the rusty sword" → ["take", "the", "rusty", "sword"]
```

**Disambiguation:**

```
> take key
Which key do you mean?
1. The brass key
2. The skeleton key
```

**Parse tree:**

```
Command
├── Verb: "take"
├── Article: "the" (ignored)
├── Adjective: "rusty"
└── Noun: "sword"
```

**Vårt framework:** KeywordParser hanterar detta automatiskt.

#### 🎓 Design Patterns (varför vi använder dem)

| Pattern       | Varför                                 | Exempel i vårt framework                          |
| ------------- | -------------------------------------- | ------------------------------------------------- |
| **State**     | Objekt byter beteende utan if-else     | Door (Open/Closed/Locked), NPC (Friendly/Hostile) |
| **Command**   | Undo, logging, queuing                 | GoCommand, TakeCommand                            |
| **Observer**  | Löskopplad event-hantering             | EventSystem, OnEnter triggers                     |
| **Memento**   | Save/Load utan att exponera internals  | GameMemento                                       |
| **Strategy**  | Utbytbara algoritmer                   | CombatSystem, MovementPattern                     |
| **Composite** | Träd-strukturer                        | Containers, Dialog trees                          |
| **Factory**   | Skapa objekt utan att veta konkret typ | Items.Create("sword")                             |
| **Builder**   | Fluent konstruktion                    | GameBuilder, LocationBuilder                      |
| **Decorator** | Lägg till beteende runtime             | RustyModifier, EnchantedModifier                  |
| **Facade**    | Förenkla komplext subsystem            | OllamaFacade, StoryLinq                           |

#### ⚠️ Common Pitfalls (Anti-patterns)

| Gör INTE                     | Gör istället                            |
| ---------------------------- | --------------------------------------- |
| Hardcoded strings överallt   | Language system / constants             |
| Giant switch-statements      | Command pattern + registry              |
| "Guess the verb" puzzles     | Synonym system, helpful errors          |
| Maze without purpose         | Every room should have meaning          |
| Pixel-hunting (text-version) | Clear, fair descriptions                |
| Softlock utan varning        | Validator + point-of-no-return warnings |
| Dålig inventory management   | Weight/count limits, "take all"         |
| One-way doors utan hint      | Tydliga "no return" meddelanden         |

#### ♿ Accessibility Guide

- **Screen readers**: Strukturerad output, ARIA-liknande hints
- **Dyslexia-friendly**: Konfigurerbar font, radavstånd, färger
- **Cognitive load**: "recap" command, quest log, hints system
- **Motor impairment**: Kortkommandon, command history
- **Color blindness**: Symboler + färg, aldrig bara färg

#### 🧩 Puzzle Design Theory

**Puzzle-typer:**

| Typ                  | Exempel                   | Framework-stöd         |
| -------------------- | ------------------------- | ---------------------- |
| **Lock & Key**       | Dörr + nyckel             | Door.RequiresKey()     |
| **Combination**      | Blanda items              | ItemCombinations       |
| **Sequence**         | Gör saker i rätt ordning  | EventChain             |
| **Information**      | Lär dig något, använd det | WorldState flags       |
| **Environment**      | Manipulera rummet         | Dynamic descriptions   |
| **Dialog**           | Säg rätt sak till NPC     | Rule-based dialog      |
| **Time-based**       | Gör något innan timer     | TimedEvents            |
| **Lateral thinking** | Oväntad användning        | UseCommand flexibility |

**Difficulty curve:**

```
Easy: Lock & Key (1 item, 1 door)
  ↓
Medium: Multi-step (key A → door A → key B → door B)
  ↓
Hard: Interconnected (item X needed for puzzle Y which gives item Z for puzzle W)
  ↓
Expert: Timed multi-step with consequences
```

#### ✍️ Writing Good Descriptions

**Show, don't tell:**

```
❌ "This is a scary room."
✅ "Shadows pool in the corners. Something drips in the darkness."
```

**Be specific:**

```
❌ "There is a sword here."
✅ "A notched longsword leans against the wall, rust creeping up the blade."
```

**Vary sentence structure:**

```
❌ "You are in a cave. There is a torch. There is a chest."
✅ "Firelight dances across the cave walls. A battered chest sits in the corner,
    its lock long since broken."
```

**First visit vs return:**

```
First: "You push through the undergrowth into a sun-dappled clearing."
Return: "The familiar clearing opens before you."
```

#### 📚 Recommended Reading

**Böcker:**

- _Twisty Little Passages_ (Nick Montfort) — IF-historia
- _Creating Interactive Fiction with Inform 7_ (Aaron Reed)
- _Procedural Storytelling in Game Design_ (Short & Adams)
- _The Art of Game Design_ (Jesse Schell)

**Online:**

- [IFDB](https://ifdb.org) — Interactive Fiction Database
- [IFComp](https://ifcomp.org) — Årlig tävling
- [Emily Short's Blog](https://emshort.blog) — Narrative design
- [Brass Lantern](http://brasslantern.org) — IF-tutorials

**Spel att studera:**

- _Zork I_ — Klassiker, parser-design
- _Photopia_ — Narrativ innovation
- _Spider and Web_ — Unreliable narrator
- _Anchorhead_ — Atmosfär, Lovecraft
- _80 Days_ — Modern IF, choices matter
- _Disco Elysium_ — Dialog-system, skill checks
- _Leather Goddesses of Phobos_ — Puzzles, world-building, humor

#### 🔄 Migration Guide

**Från Inform 7:**

```inform7
The Kitchen is a room. "A warm, cluttered space."
The wooden spoon is in the Kitchen.
```

```csharp
var kitchen = new Location("kitchen", "Kitchen")
    .Description("A warm, cluttered space.");
var spoon = new Item("spoon", "Wooden Spoon");
kitchen.AddItem(spoon);
```

**Från TADS:**

```tads
kitchen: Room 'Kitchen'
    "A warm, cluttered space."
;
```

```csharp
// Samma som ovan — vårt API är medvetet Inform/TADS-inspirerat
```

**Från Twine/Ink:**

```ink
=== kitchen ===
A warm, cluttered space.
+ [Take the spoon] -> has_spoon
+ [Leave] -> hallway
```

```csharp
// Story-LINQ för samma effekt:
story.When(s => s.Location == kitchen)
    .Choice("Take the spoon", () => player.Take(spoon))
    .Choice("Leave", () => player.GoTo(hallway));
```

#### 📊 Comparison with Other Systems

| Feature         | Inform 7        | TADS        | Twine     | Ink          | **Vårt**                     |
| --------------- | --------------- | ----------- | --------- | ------------ | ---------------------------- |
| Language        | Natural English | C-like      | HTML/CSS  | Markdown-ish | **C#**                       |
| Parser          | Built-in        | Built-in    | None      | None         | **KeywordParser**            |
| State mgmt      | World model     | World model | Variables | Variables    | **WorldState + Story-LINQ**  |
| Extensible      | Limited         | Yes         | JS        | C#           | **Full .NET**                |
| IDE support     | Inform IDE      | TADS WB     | Web       | Unity/Inky   | **VS/Rider**                 |
| Testing         | Skein           | -           | -         | -            | **Validator**                |
| Narrative tools | Basic           | Basic       | Basic     | Weave/Knots  | **Story-LINQ, Arcs, Themes** |
| NuGet/Package   | No              | No          | No        | Yes          | **Yes**                      |

#### 📖 Glossary

| Term                  | Definition                                |
| --------------------- | ----------------------------------------- |
| **IF**                | Interactive Fiction                       |
| **Parser**            | System som tolkar spelarens input         |
| **CYOA**              | Choose Your Own Adventure (inget parsing) |
| **Softlock**          | Spelet kan inte vinnas men fortsätter     |
| **Hardlock**          | Spelet kraschar/fastnar                   |
| **Feelie**            | Fysiskt objekt som medföljer spel         |
| **Implementor**       | Person som skapar IF                      |
| **Transcript**        | Logg av spelarsession                     |
| **Walkthrough**       | Steg-för-steg lösning                     |
| **Hint system**       | Gradvis avslöjande av hjälp               |
| **Compass rose**      | N/S/E/W navigation                        |
| **Inventory puzzle**  | Lösning kräver rätt items                 |
| **Conversation tree** | Förgrenad dialog                          |
| **World model**       | Internrepresentation av spelvärlden       |

---

### Wiki: Objects & Items (Best Practices)

#### 🗝️ Item Design Principles

**Single Responsibility:**

```csharp
// ❌ God-item
class MagicSword : Item, IWeapon, IKey, ILightSource, IContainer { }

// ✅ Composable
var sword = new Item("sword")
    .AsWeapon(damage: 10)
    .WithEnchantment("glowing")  // Light source via decorator
    .CanUnlock("magic_door");    // Key-like behavior
```

**Item Categories:**

| Type             | Properties              | Use Case              |
| ---------------- | ----------------------- | --------------------- |
| **Basic**        | id, name, description   | Background items      |
| **Takeable**     | + weight, takeable=true | Inventory items       |
| **Key**          | + unlocks(door_id)      | Access control        |
| **Container**    | + contents[], capacity  | Storage, puzzles      |
| **Readable**     | + text, requiresLight   | Documents, signs      |
| **Consumable**   | + uses, onUse effect    | Potions, food         |
| **Weapon**       | + damage, durability    | Combat                |
| **Light Source** | + brightness, fuel      | Dark rooms            |
| **Tool**         | + canUseOn(targets)     | Specific interactions |

**Naming Conventions:**

```csharp
// ID: snake_case, unik
// Name: Human readable, kan ha prefix

new Item("brass_key", "Brass Key")           // ✅
new Item("key1", "Key")                       // ❌ Generic
new Item("the_brass_key", "The Brass Key")   // ❌ Article i ID
```

#### 🚪 Door Design

```csharp
// Simple locked door
door.Locked().RequiresKey("brass_key");

// Combination lock
door.Locked().RequiresCode("1234");

// Conditional
door.Locked().OpensWhen(w => w.GetFlag("alarm_disabled"));

// Destructible
door.Destructible().Health(50).WeakTo("fire");

// One-way
exit.OneWay().Message("The door slams shut behind you!");
```

#### 📦 Container Patterns

```csharp
// Basic chest
var chest = new Container("chest")
    .Capacity(10)
    .InitiallyLocked()
    .RequiresKey("chest_key");

// Nested containers
var bag = new Container("bag").Contains(
    new Container("pouch").Contains(
        new Item("gem")
    )
);

// Type-restricted
var quiver = new Container<Arrow>("quiver").MaxItems(20);
var flask = new Container<Liquid>("flask").MaxVolume(500);
```

---

### Wiki: NPC Design (Best Practices)

#### 👥 NPC Archetypes

| Archetype       | Behavior                                         | Example               |
| --------------- | ------------------------------------------------ | --------------------- |
| **Shopkeeper**  | Stationary, trade dialog, inventory              | Blacksmith, Innkeeper |
| **Quest Giver** | Stationary, quest dialog, tracks progress        | Village Elder, King   |
| **Guide**       | Follows, gives hints, comments on locations      | Companion, Fairy      |
| **Guard**       | Patrols, blocks access, can be bribed/distracted | Castle Guard          |
| **Enemy**       | Hostile, combat AI, drops loot                   | Goblin, Dragon        |
| **Informant**   | Stationary, reveals info based on conditions     | Bartender, Spy        |
| **Victim**      | Needs rescue, follows after saved                | Prisoner, Child       |
| **Merchant**    | Wanders, random inventory, haggling              | Traveling Trader      |

```csharp
// Quick archetype setup
var blacksmith = Npc.Shopkeeper("blacksmith", "Gruff Blacksmith")
    .Sells("sword", "shield", "armor")
    .Buys(ItemType.Metal)
    .Greeting("What'll it be?");

var guard = Npc.Guard("guard", "Castle Guard")
    .Patrols("gate", "courtyard", "tower")
    .Blocks("throne_room")
    .CanBeBribed(gold: 50)
    .CanBeDistracted("noise_elsewhere");
```

#### 💬 Dialog Design Patterns

**Branching (traditional tree):**

```csharp
npc.Dialog()
    .Say("Greetings, traveler.")
    .Choice("Ask about the weather", d => d.Say("Storm's coming."))
    .Choice("Ask about rumors", d => d
        .Say("Heard the dragon's been spotted.")
        .Choice("Where?", d2 => d2.Say("North mountains."))
        .Choice("Thanks", d2 => d2.End())
    )
    .Choice("Goodbye", d => d.End());
```

**Conditional (rule-based):**

```csharp
npc.AddDialogRule("knows_player")
    .When(ctx => ctx.Relationship(npc) > 50)
    .Say("Ah, my friend! Good to see you again.");

npc.AddDialogRule("default_greeting")
    .Priority(0)  // Fallback
    .Say("Hmm? What do you want?");
```

**Mood-affected:**

```csharp
npc.AddDialogRule("angry_greeting")
    .When(ctx => npc.Mood == Mood.Angry)
    .Say("What?! Can't you see I'm busy!");

npc.AddDialogRule("happy_greeting")
    .When(ctx => npc.Mood == Mood.Happy)
    .Say("Hello there! Lovely day, isn't it?");
```

**Random variety:**

```csharp
npc.AddDialogRule("idle_chatter")
    .When(ctx => ctx.IsIdleConversation)
    .SayRandom(
        "Nice weather we're having.",
        "Did you hear about the festival?",
        "My back's been killing me lately."
    );
```

#### 🚶 Movement Patterns

```csharp
// Stationary
npc.Movement(MovementPattern.None);

// Random wandering
npc.Movement(MovementPattern.Wander)
    .InArea("village")
    .Speed(every: 5);  // Move every 5 ticks

// Patrol route
npc.Movement(MovementPattern.Patrol)
    .Route("gate", "courtyard", "tower", "courtyard")
    .Speed(every: 3);

// Follow player
npc.Movement(MovementPattern.Follow)
    .Target(player)
    .MaxDistance(1);

// Schedule-based
npc.Movement(MovementPattern.Scheduled)
    .At(TimePhase.Morning, "bedroom")
    .At(TimePhase.Day, "shop")
    .At(TimePhase.Evening, "tavern")
    .At(TimePhase.Night, "bedroom");
```

#### 😊 Emotion System

```csharp
public enum NpcMood
{
    Neutral, Happy, Sad, Angry, Fearful, Suspicious, Grateful
}

// Mood changes based on events
npc.OnEvent("player_helps")
    .ShiftMood(Mood.Grateful, +0.5f)
    .Say("Thank you so much!");

npc.OnEvent("player_steals")
    .ShiftMood(Mood.Angry, +0.8f)
    .ShiftRelationship(-30);

// Mood decays over time
npc.MoodDecay(rate: 0.1f, toward: Mood.Neutral);
```

#### 📋 Quest-Giving Patterns

```csharp
// Simple fetch quest
npc.OffersQuest("fetch_herbs")
    .Description("Bring me 5 healing herbs.")
    .Requires(player.Has("herb", count: 5))
    .Reward(gold: 50, xp: 100)
    .OnComplete(ctx => ctx.Say("Excellent! Here's your reward."));

// Multi-stage
npc.OffersQuest("dragon_slayer")
    .Stage(1, "Find the dragon's lair")
        .CompleteWhen(w => w.GetFlag("lair_discovered"))
    .Stage(2, "Retrieve the dragon's weakness from the library")
        .CompleteWhen(player.Has("dragon_lore"))
    .Stage(3, "Slay the dragon")
        .CompleteWhen(w => w.GetFlag("dragon_dead"))
    .FinalReward(gold: 1000, title: "Dragonslayer");

// Escort quest
npc.OffersQuest("escort_merchant")
    .Type(QuestType.Escort)
    .Escort(merchant)
    .To("city_gates")
    .FailIf(merchant.IsDead);
```

#### ⚔️ Combat Behavior

```csharp
npc.CombatBehavior()
    .Aggressive()           // Attacks on sight
    .TargetPriority(player) // Who to attack first
    .FleeWhen(health < 20)  // Run away at low health
    .CallForHelp(radius: 3) // Alert nearby allies
    .DropOnDeath("gold", "rusty_sword");

// More nuanced
npc.CombatBehavior()
    .Defensive()                        // Only attacks if attacked
    .Warns(count: 2)                    // Warns before attacking
    .SurrenderWhen(health < 10)         // Gives up
    .OnSurrender(ctx => ctx.CanSpare()) // Player choice
    .LootTable(LootTable.Bandit);
```

#### 🌍 World State Integration

```csharp
// React to player reputation
npc.AddDialogRule("hero_greeting")
    .When(ctx => ctx.World.GetReputation("village") > 80)
    .Say("The hero of our village! What an honor!");

npc.AddDialogRule("villain_greeting")
    .When(ctx => ctx.World.GetReputation("village") < -50)
    .Say("You! Guards! Seize this criminal!")
    .Then(ctx => ctx.TriggerCombat());

// React to time
npc.AddDialogRule("night_warning")
    .When(ctx => ctx.World.TimePhase == TimePhase.Night)
    .Say("You shouldn't be out at night. Dangerous creatures about.");

// React to world events
npc.AddDialogRule("after_dragon_death")
    .When(ctx => ctx.World.GetFlag("dragon_dead"))
    .Say("I heard you killed the dragon! Amazing!")
    .Then(ctx => ctx.ShiftRelationship(+20));
```

---

### Wiki: Language & Localization

#### 🗣️ Object Grammar

**Articles (a/an/the):**

```csharp
// English
item.Grammar(en => en
    .Article(Article.Indefinite, "a")   // "a sword"
    .Article(Article.Definite, "the")); // "the sword"

// Swedish
item.Grammar(sv => sv
    .Article(Article.Indefinite, "ett") // "ett svärd"
    .Article(Article.Definite, "et"));  // "svärdet" (suffix)
```

**Plurals:**

```csharp
item.Grammar(en => en
    .Singular("key")
    .Plural("keys"));

item.Grammar(en => en
    .Singular("knife")
    .Plural("knives"));  // Irregular

// Auto-detect common patterns
item.Grammar(en => en.AutoPlural()); // sword → swords
```

**Natural lists:**

```csharp
// "a sword, a shield, and a torch"
var text = items.ToNaturalList(culture: "en-US");

// "ett svärd, en sköld och en fackla"
var text = items.ToNaturalList(culture: "sv-SE");
```

#### 🔤 Command Synonyms

```csharp
// Global synonyms
parser.AddSynonyms("en", "take", "get", "grab", "pick up", "acquire");
parser.AddSynonyms("sv", "ta", "plocka", "hämta", "grip");

// Item-specific
sword.AddAliases("en", "blade", "weapon", "steel");
sword.AddAliases("sv", "klinga", "vapen", "stål");

// Verb normalization
parser.Normalize("pick up") → "take"
parser.Normalize("plocka upp") → "ta"
```

#### 🌐 Multi-language Support

```csharp
// Load language at startup
game.LoadLanguage("sv-SE", "languages/swedish.json");

// Runtime switching
game.SetLanguage("en-US");

// Fallback chain
game.LanguageFallback("sv-SE", "en-US", "en");

// Per-string localization
location.Description(
    ("en", "A dark cave."),
    ("sv", "En mörk grotta."),
    ("de", "Eine dunkle Höhle.")
);
```

---

### Wiki: Advanced Topics

#### ⏰ Time System Deep Dive

```csharp
// Time phases
public enum TimePhase { Dawn, Morning, Midday, Afternoon, Dusk, Evening, Night, Midnight }

// Automatic progression
game.Time.TicksPerPhase = 20;
game.Time.PhasesPerDay = 8;

// Manual control
game.Time.Advance(ticks: 5);
game.Time.SetPhase(TimePhase.Night);

// Time-dependent descriptions
location.Description()
    .At(TimePhase.Day, "Sunlight streams through the windows.")
    .At(TimePhase.Night, "Moonlight casts long shadows.");

// Scheduled events
game.Time.At(TimePhase.Midnight)
    .Every(days: 1)
    .Do(ctx => ctx.SpawnNpc("ghost", "graveyard"));
```

#### 👁️ Perception System

**Light levels:**

```csharp
public enum LightLevel { Pitch, Dark, Dim, Normal, Bright, Blinding }

location.BaseLightLevel(LightLevel.Dark);

// Light sources affect perception
torch.ProvidesLight(LightLevel.Normal, radius: 2);

// Can't see in darkness
if (location.EffectiveLightLevel < LightLevel.Dim)
    return "It's too dark to see.";

// Items revealed at different light levels
item.VisibleAt(LightLevel.Dim, "You make out a shape in the darkness.");
item.VisibleAt(LightLevel.Normal, "A rusty sword lies on the ground.");
```

**Sound:**

```csharp
// Sound propagates through locations
combat.EmitsSound("combat", loudness: 3, radius: 2);

// NPCs react to sound
npc.OnHear("combat")
    .InRange(2)
    .React(ctx => ctx.Investigate());

// Player can hear through walls
location.OnSound("footsteps")
    .From(Direction.North)
    .Message("You hear footsteps from the north.");
```

**Memory:**

```csharp
// Player remembers visited locations
player.Memory.HasVisited("cave");

// NPCs remember interactions
npc.Memory.Remember("player_helped", timestamp: game.Time.Now);
npc.Memory.Recall("player_helped", maxAge: 100); // Within 100 ticks

// Memories fade
npc.Memory.DecayRate = 0.01f; // Per tick
```

#### 🤝 Social System

**Relationships:**

```csharp
// Player-NPC relationship (-100 to +100)
player.SetRelationship("blacksmith", 50);  // Friendly

// Relationship thresholds
npc.DialogWhen(rel => rel < -50, "Get out of my shop, scum!");
npc.DialogWhen(rel > 50, "Ah, my best customer! Special discount today.");

// Actions affect relationships
player.OnAction("give_gift", npc)
    .ShiftRelationship(+10);

player.OnAction("steal_from", npc)
    .ShiftRelationship(-30);
```

**Reputation:**

```csharp
// Faction-wide reputation
player.Reputation.With("village").Set(75);
player.Reputation.With("thieves_guild").Set(-20);

// Reputation spreads
player.OnAction("heroic_deed")
    .SpreadReputation("region", +10, decay: 0.5f);

// NPCs check faction reputation
npc.Faction = "village";
npc.DialogBasedOnFaction(); // Uses player.Reputation.With("village")
```

#### 🎯 Intention System (Goal-based AI)

```csharp
// NPC has goals
npc.Goals.Add(new Goal("survive", priority: 100));
npc.Goals.Add(new Goal("earn_money", priority: 50));
npc.Goals.Add(new Goal("find_love", priority: 30));

// Goals influence behavior
npc.OnLowHealth()
    .If(goal => goal.Is("survive"))
    .Then(flee);

npc.OnSeeItem("gold")
    .If(goal => goal.Is("earn_money"))
    .Then(negotiate);

// Goals can conflict
npc.OnConflict("survive", "protect_child")
    .Resolve(higherPriority); // or custom logic
```

#### 🎭 Narrative Logic

**Theme tracking:**

```csharp
// Themes are tracked across the game
game.Themes.Track(Theme.Redemption);
game.Themes.Track(Theme.Loss);

// Beats affect themes
beat.Mark(Theme.Redemption, +0.2f);

// Query theme state
var dominantTheme = game.Themes.Dominant();
var redemptionProgress = game.Themes.Progress(Theme.Redemption);
```

**Foreshadowing system:**

```csharp
// Plant a seed
game.Foreshadow("dragon_attack")
    .Hint(1, "You notice claw marks on the trees.")
    .Hint(2, "A farmer mentions missing livestock.")
    .Hint(3, "Smoke rises from the mountains.");

// Callback on reveal
game.OnForeshadowComplete("dragon_attack")
    .Do(ctx => ctx.TriggerEvent("dragon_arrives"));

// Players can recall foreshadowing
player.Memory.Foreshadowing.Contains("dragon_attack");
```

#### 🔄 Meta Perspective

**Undo system:**

```csharp
// Every action creates a checkpoint
game.EnableUndo(maxHistory: 50);

// Player can undo
> undo
"Undoing: open chest"

// Certain actions block undo
action.CannotUndo("This action is permanent.");
```

**What-if scenarios:**

```csharp
// Branching exploration (debugging/design tool)
var scenario = game.WhatIf(ctx => {
    ctx.Player.Take("poison");
    ctx.Player.Use("poison").On("king");
});

// Inspect outcome without affecting real state
scenario.Outcome.GetFlag("king_dead"); // true
scenario.Consequences; // List of changes

// Discard or apply
scenario.Discard(); // Nothing happened
// or
scenario.Apply(); // Commit changes to real game
```

---

### Wiki: Game Design Guides (Planning Section)

Dessa sidor hjälper spelskapare att planera innan de kodar.

#### 📝 Planning Your Adventure

**Innehåll:**

1. **Börja med slutet** — Vad är målet? Vad är "vinst"?
2. **Grid-metoden** (Usborne) — Rita kartan på rutat papper först
3. **The Rule of Three** — 3 puzzles, 3 areas, 3 keys
4. **Scope control** — Börja med 5 rum, expandera senare
5. **Playtest loop** — Skriv → Testa → Fixa → Upprepa

**Checklista innan kodning:**

```
□ Karta ritad (rum + kopplingar)
□ Alla rum numrerade/namngivna
□ Mål definierat (vad är "vinst"?)
□ Kritiska items listade (nycklar, verktyg)
□ Minst en väg från start till mål verifierad
□ "Target story" skriven (vad ska spelaren berätta efteråt?)
```

#### 🗺️ Story → Adventure Mapping

**Från berättelse till spelmekanik:**

| Story Element       | Adventure Mechanic             |
| ------------------- | ------------------------------ |
| Karaktärens mål     | Win condition                  |
| Hinder              | Locked doors, puzzles          |
| Viktiga föremål     | Key items                      |
| Bikaraktärer        | NPCs med dialog                |
| Plats               | Location + description         |
| Tid/press           | Turn limits, timed events      |
| Hemligheter         | Hidden rooms, conditional text |
| Karaktärsutveckling | Theme tracking, InnerState     |
| Spänning            | Mood shifts, pacing            |

**Exempel: Rödluvan → Adventure**

| Story                 | Mechanic                                                 |
| --------------------- | -------------------------------------------------------- |
| "Gå till mormors hus" | Goal: reach `grandmas_house`                             |
| "Genom skogen"        | Locations: `home → forest → crossroads → grandmas_house` |
| "Vargen lurar"        | NPC `wolf` med dialog, blocks path                       |
| "Ta inte genvägen"    | Two paths: safe (long) vs dangerous (short)              |
| "Korg med mat"        | Item `basket` required for win                           |
| "Mormor är konstig"   | Conditional description based on `wolf_arrived_first`    |

#### 🔤 Common Verbs & Nouns

**Standard verbs (alla spel bör stödja):**

| Verb        | Aliases                          | Användning               |
| ----------- | -------------------------------- | ------------------------ |
| `go`        | walk, move, head, travel         | Navigation               |
| `look`      | examine, inspect, check, view, l | Describe room/item       |
| `take`      | get, grab, pick, pickup          | Add to inventory         |
| `drop`      | put, leave, discard              | Remove from inventory    |
| `open`      | -                                | Doors, containers        |
| `close`     | shut                             | Doors, containers        |
| `use`       | -                                | Generic item interaction |
| `talk`      | speak, ask, say                  | NPC dialog               |
| `read`      | -                                | Signs, books, letters    |
| `inventory` | i, inv, items                    | Show carried items       |
| `help`      | ?                                | Show commands            |
| `save`      | -                                | Save game                |
| `load`      | restore                          | Load game                |
| `quit`      | exit, q                          | End game                 |

**Genre-specifika verbs:**

| Genre   | Extra Verbs                       |
| ------- | --------------------------------- |
| Combat  | attack, fight, kill, flee, defend |
| Mystery | search, investigate, accuse       |
| Magic   | cast, enchant, dispel             |
| Social  | bribe, threaten, persuade, charm  |
| Stealth | hide, sneak, steal, lockpick      |

**Common nouns (items):**

| Category   | Examples                                 |
| ---------- | ---------------------------------------- |
| Keys       | key, keycard, password, code             |
| Light      | torch, lantern, lamp, candle, flashlight |
| Containers | bag, chest, box, drawer, safe            |
| Tools      | rope, crowbar, hammer, lockpick          |
| Weapons    | sword, knife, gun, staff                 |
| Documents  | letter, book, map, note, diary           |
| Valuables  | gold, gem, coin, treasure, artifact      |

#### 🎯 Framework Simplification

**Hur vårt framework förenklar planeringen:**

| Traditionellt              | Med vårt framework                     |
| -------------------------- | -------------------------------------- |
| Skriv parser från scratch  | `KeywordParser` + synonymer inbyggt    |
| Manuell state-hantering    | `WorldState` + flags/counters          |
| If-else-helvete för dialog | Rule-based dialog system               |
| Svårt att testa            | `Validator.FindUnreachableLocations()` |
| Hårdkodade beskrivningar   | Dynamic descriptions + layers          |
| Ingen struktur för story   | Story-LINQ + Narrative Arcs            |

**Från plan till kod — 1:1 mapping:**

```
PLAN                          CODE
────                          ────
"Rum: Kök"                →   new Location("kitchen", "Kitchen")
"Utgång norr till hall"   →   .AddExit(Direction.North, hall)
"Låst dörr"               →   .WithDoor().Locked()
"Nyckel i lådan"          →   drawer.Contains(key)
"Prata med kocken"        →   chef.AddDialogRule(...)
"Mål: hitta receptet"     →   quest.Goal(player.Has("recipe"))
```

**Story-LINQ mapping:**

```
PLAN                              STORY-LINQ
────                              ──────────
"Spelaren vaknar trött"       →   .DefineNeed(Need.WakeUp)
"Kaffe gör en pigg"           →   .When(has("coffee")).Satisfy(Need.WakeUp)
"Stämningen ljusnar"          →   .Shift(s => s.Mood, Mood.Hope)
"Temat är ensamhet → gemenskap" → .DefineThemeArc(Theme.Connection)
```

#### 📐 Planning Templates

**Minimal Adventure (5 rum):**

```
START ──→ HUB ──→ GOAL
           │
           ├──→ PUZZLE_ROOM (has KEY)
           │
           └──→ LOCKED_ROOM (needs KEY, has TREASURE)
```

**Three-Act Adventure:**

```
ACT 1: Setup (3 rum)
  START → VILLAGE → CROSSROADS

ACT 2: Confrontation (5 rum)
  CROSSROADS → FOREST → CAVE → MOUNTAIN → CASTLE_GATE

ACT 3: Resolution (2 rum)
  CASTLE_GATE → THRONE_ROOM → ENDING
```

**Mystery Template:**

```
CRIME_SCENE (clue 1)
     │
     ├──→ SUSPECT_A_LOCATION (clue 2, alibi)
     │
     ├──→ SUSPECT_B_LOCATION (clue 3, motive)
     │
     └──→ HIDDEN_LOCATION (requires clues 1+2+3, reveals TRUTH)
            │
            └──→ CONFRONTATION (accuse with TRUTH)
```

### Wiki: Narrative Arcs

Alla inbyggda narrativa mallar med beskrivning och användningsområden.

| Arc                    | Struktur                                                                | Bra för                           |
| ---------------------- | ----------------------------------------------------------------------- | --------------------------------- |
| **Hero's Journey**     | Call → Trials → Transformation → Return                                 | Klassiska äventyr, fantasy        |
| **Tragic Arc**         | Hybris → Felsteg → Konsekvens → Sen insikt                              | Mörka berättelser, moraliska val  |
| **Transformation Arc** | Fragmenterad → Skuggkonfrontation → Integration → Ny självbild          | Psykologisk mognad, trauma, sorg  |
| **Ensemble Journey**   | Flera hjältar → Växlande perspektiv → Gruppkonflikter → Kollektiv seger | Politik, motstånd, "Jedi Council" |
| **The Descent**        | Nedstigning → Kontrollförlust → Möte med tomhet → Förändrad återkomst   | Psykologisk skräck, Silent Hill   |
| **Spiral Narrative**   | Upprepning → Små variationer → Djupare förståelse                       | Tidsloopar, minnesglitchar        |
| **Moral Labyrinth**    | Inget rätt slut → Alla val kostar → Situationsbunden sanning            | Etik, ledarskap, ansvar           |
| **World-Shift Arc**    | Gradvis världsförändring → Spelaren som katalysator → Ny jämvikt        | Dune-stil, ekologi, politik       |
| **Caretaker Arc**      | Reparera → Hela → Skydda → Kamp mot entropi                             | Mogna, lågmälda spel              |
| **Witness Arc**        | Observera → Samla berättelser → Sanning genom förståelse                | Textmysterier, detektiv           |

### Exempelspel - Creation Styles (ett per stil)

| Stil                    | Spel               | Beskrivning                                  |
| ----------------------- | ------------------ | -------------------------------------------- |
| **Fluent Builder**      | The Haunted Manor  | Klassisk spökhistoria, visar method chaining |
| **Implicit Conversion** | Quick Escape       | Minimalistiskt rum-escape, snabbaste sättet  |
| **Factory/Tuple**       | Dungeon Loot       | Massa items, visar JSON-tänk bulk creation   |
| **DSL**                 | Forest Adventure   | Config-fil driven, visar `.adventure` format |
| **Mixed**               | The Complete Quest | Kombinerar alla stilar, best practices       |

### Exempelspel - Storytelling Features

Dessa ska vara så fluent som möjligt och visa varje storytelling-funktion.

#### 🎭 Narrativ Struktur & Arcs

| Feature                | Spel                  | Visar                              |
| ---------------------- | --------------------- | ---------------------------------- |
| **Hero's Journey**     | The Chosen One        | Full 12-stegs resa                 |
| **Tragic Arc**         | The King's Folly      | Hybris → fall → sen insikt         |
| **Transformation Arc** | Shattered Mirror      | Inre resa, trauma, integration     |
| **Ensemble Journey**   | The Resistance        | Flera hjältar, växlande perspektiv |
| **The Descent**        | Into the Abyss        | Katabasis, psykologisk skräck      |
| **Spiral Narrative**   | Groundhog Dungeon     | Tidsloop med variationer           |
| **Moral Labyrinth**    | The Tribunal          | Inga rätta svar, etiska val        |
| **World-Shift Arc**    | Seeds of Change       | Spelaren som katalysator           |
| **Caretaker Arc**      | The Lighthouse Keeper | Reparera, hela, skydda             |
| **Witness Arc**        | The Collector         | Observera, samla sanningen         |
| **Chapters**           | The Saga              | Akter, kapitelövergångar           |
| **Scene Beats**        | The Interview         | Dialog-driven, dramatiska pauser   |

#### 👥 Karaktärer & Relationer

| Feature              | Spel               | Visar                                     |
| -------------------- | ------------------ | ----------------------------------------- |
| **Character Arcs**   | The Reluctant Hero | NPC utvecklas genom spelarens val         |
| **Emotional Stakes** | The Last Goodbye   | Relationer, förlust, val med konsekvenser |

#### 🌙 Atmosfär & Beskrivningar

| Feature                  | Spel              | Visar                                    |
| ------------------------ | ----------------- | ---------------------------------------- |
| **Mood & Atmosphere**    | The Lighthouse    | Väder, ljus, ljud påverkar beskrivningar |
| **Dynamic Descriptions** | The Living Castle | Rum ändras baserat på tid/händelser      |
| **Narrative Voice**      | Noir Detective    | Berättarröst, stiliserad text            |

#### ⏱️ Spänning & Tempo

| Feature              | Spel           | Visar                                |
| -------------------- | -------------- | ------------------------------------ |
| **Pacing & Tension** | Countdown      | Ökande press, timer-baserad spänning |
| **Foreshadowing**    | Murder Mystery | Ledtrådar som kopplas ihop senare    |
| **Time Triggers**    | The Bomb       | Objekt aktiveras efter tid/händelser |

#### 🎮 Spelarupplevelse

| Feature            | Spel            | Visar                          |
| ------------------ | --------------- | ------------------------------ |
| **Player Agency**  | Branching Paths | Val som faktiskt spelar roll   |
| **Dramatic Irony** | The Traitor     | Spelaren vet mer än karaktären |

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

---

## Slice 41: Testing & Validation Tools

**Mål:** Verktyg för att testa och validera textäventyr.

_Från Write Your Own Adventure Programs (Usborne):_

- "Try every possible command in every situation"
- "Ask other people to play - they try things you didn't think of"
- "Testing is for improving puzzles and making sure the game is enjoyable"

### Task 41.1: Reachability Validator

```csharp
var validator = game.CreateValidator();

// Kontrollera att alla rum går att nå
var unreachableLocations = validator.FindUnreachableLocations();
// Output: ["secret_room", "hidden_cave"]

// Kontrollera att alla items går att få tag på
var unreachableItems = validator.FindUnreachableItems();
// Output: [("magic_gem", "Requires key that doesn't exist")]

// Kontrollera att alla quests går att klara
var impossibleQuests = validator.FindImpossibleQuests();
```

### Task 41.2: Command Coverage Report

```csharp
// Lista alla möjliga kommandon per rum
foreach (var location in game.Locations)
{
    var commands = validator.GetPossibleCommands(location);
    Console.WriteLine($"{location.Name}: {string.Join(", ", commands)}");
}

// Output:
// Cave: go north, go south, take torch, look, inventory
// Forest: go east, go west, talk to hermit, attack wolf

// Hitta kommandon som aldrig används
var unusedCommands = validator.FindUnusedCommands();
```

### Task 41.3: Automated Playthrough

```csharp
// "Try everything" automation
var explorer = game.CreateExplorer();

// Random exploration
var log = explorer.RandomWalk(maxSteps: 1000);

// Exhaustive exploration (depth-first)
var allPaths = explorer.ExploreAllPaths();

// Find shortest path to win
var winPath = explorer.FindWinningPath();
if (winPath == null)
    Console.WriteLine("Warning: No winning path exists!");

// Replay a specific path
explorer.Replay(winPath);
```

### Task 41.4: Dead-end Detection

```csharp
// Hitta situationer där spelaren fastnar
var deadEnds = validator.FindDeadEnds();
// Output: [
//   "Using key on wrong door destroys key, can't proceed",
//   "Taking gem triggers collapse, blocks exit permanently"
// ]

// Hitta point-of-no-return utan varning
var noReturnPoints = validator.FindUnmarkedPointsOfNoReturn();
```

### Task 41.5: Testing Mode (Debug)

```csharp
game.EnableTestingMode();

// I testing mode:
// - "teleport <location>" - hoppa till rum
// - "spawn <item>" - skapa item
// - "godmode" - immortal
// - "showmap" - visa alla rum
// - "showitems" - lista alla items och var de är
// - "shownpcs" - lista alla NPCs och deras state
// - "skiptime <ticks>" - hoppa framåt i tid
// - "setflag <name> <value>" - sätt worldstate
// - "listcommands" - visa alla kommandon i nuvarande rum
```

### Task 41.6: Puzzle Dependency Graph

```csharp
// Generera beroendegraf för puzzles
var graph = validator.GeneratePuzzleGraph();

// Export till Mermaid
var mermaid = graph.ToMermaid();
// Output:
// graph LR
//     key --> locked_door
//     locked_door --> treasure_room
//     torch --> dark_cave
//     dark_cave --> ancient_map
//     ancient_map --> secret_entrance

// Hitta cirkulära beroenden (omöjliga puzzles)
var circles = graph.FindCircularDependencies();
```

### Task 41.7: Story Snippet Validation

_Från Procedural Storytelling:_

> "Skriv exempel på berättelser spelaren skulle berätta, fråga: vad är minsta jag kan göra för att generera fler?"

```csharp
// Definiera "target stories" - berättelser som ska kunna hända
validator.AddTargetStory("saved_village")
    .Requires("dragon_defeated")
    .Requires("villagers_alive", min: 5)
    .Requires("reputation", min: 50);

validator.AddTargetStory("tragic_hero")
    .Requires("saved_kingdom")
    .Requires("lost_loved_one");

// Verifiera att engine kan producera dem
var achievable = validator.ValidateTargetStories();
// Output: [
//   ("saved_village", true, "Achievable via: kill dragon, save 7 villagers"),
//   ("tragic_hero", false, "Missing: no way to trigger 'lost_loved_one'")
// ]
```

---

## Slice 42: Story-LINQ (Narrative Query Language)

**Mål:** LINQ-inspirerad fluent syntax för narrativ state management. Svaret på livet, universum och allting.

_"Berättelse är inte text. Berättelse är tillståndsförändring med mening."_

### Kärnkoncept

```
State → Transition → Meaning
(Var vi är) → (Vad som händer) → (Varför det spelar roll)
```

### Task 42.1: IStoryState — Narrativt tillstånd

```csharp
public interface IStoryState
{
    // Yttre tillstånd
    ILocation Location { get; }
    Mood WorldMood { get; }
    TimePhase TimePhase { get; }

    // Inre tillstånd
    InnerState PlayerInnerState { get; }
    IReadOnlyCollection<Need> ActiveNeeds { get; }

    // Tematisk tracking
    IReadOnlyDictionary<Theme, float> ThemeProgress { get; }

    // Minne
    IReadOnlyCollection<string> Memories { get; }
    IReadOnlyCollection<string> RevealedTruths { get; }
}

public enum Mood
{
    Stillness, Fear, Hope, Decay, Trust,
    Awakening, Tension, Release, Wonder, Dread
}

public enum InnerState
{
    Drowsy, Groggy, Awake, Alert, Focused,
    Confused, Determined, Guilty, Healed, Present
}

public enum Need
{
    WakeUp, Safety, Connection, Understanding,
    Closure, Revenge, Redemption, Rest
}

public enum Theme
{
    Trust, Loss, Growth, Corruption, Hope,
    Isolation, Connection, Power, Sacrifice, Truth
}
```

### Task 42.2: Story-LINQ Operators

```csharp
public static class StoryLinq
{
    // === QUERY OPERATORS ===

    // When - precondition (som Where i LINQ)
    public static IStoryQuery When(this IStory story,
        Func<IStoryState, bool> condition);

    // And - kombinera villkor
    public static IStoryQuery And(this IStoryQuery query,
        Func<IStoryState, bool> condition);

    // Or - alternativa villkor
    public static IStoryQuery Or(this IStoryQuery query,
        Func<IStoryState, bool> condition);

    // === TRANSITION OPERATORS ===

    // Shift - ändra tillstånd
    public static IStoryQuery Shift<T>(this IStoryQuery query,
        Expression<Func<IStoryState, T>> property, T newValue);

    // ShiftGradually - ändra över tid
    public static IStoryQuery ShiftGradually<T>(this IStoryQuery query,
        Expression<Func<IStoryState, T>> property, T target, int ticks);

    // === MEANING OPERATORS ===

    // Mark - påverka tema
    public static IStoryQuery Mark(this IStoryQuery query,
        Theme theme, float delta);

    // Satisfy - uppfyll behov
    public static IStoryQuery Satisfy(this IStoryQuery query, Need need);

    // Awaken - skapa nytt behov
    public static IStoryQuery Awaken(this IStoryQuery query, Need need);

    // === NARRATIVE OPERATORS ===

    // Reveal - visa text/sanning
    public static IStoryQuery Reveal(this IStoryQuery query, string text);

    // Remember - lägg till minne
    public static IStoryQuery Remember(this IStoryQuery query, string memory);

    // Echo - referera till tidigare minne
    public static IStoryQuery Echo(this IStoryQuery query, string memoryPattern);

    // === EXECUTION ===

    // AsBeats - materialisera till story beats
    public static IEnumerable<IStoryBeat> AsBeats(this IStoryQuery query);

    // Execute - kör direkt
    public static StoryResult Execute(this IStoryQuery query);
}
```

### Task 42.3: Fluent Beat Builder

```csharp
// Definiera ett beat
story.DefineBeat("morning_coffee")
    .When(s => s.Player.Has("coffee"))
    .And(s => s.Location.Id == "livingroom")
    .And(s => s.PlayerInnerState <= InnerState.Groggy)
    .Shift(s => s.WorldMood, Mood.Awakening)
    .Shift(s => s.PlayerInnerState, InnerState.Awake)
    .Satisfy(Need.WakeUp)
    .Mark(Theme.Connection, +0.1)  // Morgonritualen binder till hemmet
    .Reveal("Warmth spreads through you. The day feels possible now.");

// Beats kan ärva från varandra
story.DefineBeat("morning_coffee_with_book")
    .Extends("morning_coffee")
    .And(s => s.Player.Has("book"))
    .Shift(s => s.PlayerInnerState, InnerState.Present)
    .Mark(Theme.Growth, +0.2)
    .Reveal("The words on the page find a quiet mind.");
```

### Task 42.4: Composable Story Fragments

```csharp
// Återanvändbara fragment (som LINQ-metoder)
public static class StoryFragments
{
    public static IStoryQuery HasMorningEssentials(this IStoryQuery q) =>
        q.And(s => s.Player.Has("coffee"))
         .And(s => s.TimePhase == TimePhase.Morning);

    public static IStoryQuery IsCalm(this IStoryQuery q) =>
        q.And(s => s.WorldMood.In(Mood.Stillness, Mood.Hope))
         .And(s => s.PlayerInnerState >= InnerState.Awake);

    public static IStoryQuery SeekingRest(this IStoryQuery q) =>
        q.And(s => s.ActiveNeeds.Contains(Need.Rest));
}

// Användning - läsbar som en mening
story.DefineBeat("peaceful_reading")
    .When(s => s.Location.HasTag("comfortable"))
    .HasMorningEssentials()
    .IsCalm()
    .And(s => s.Player.Has("book"))
    .Shift(s => s.PlayerInnerState, InnerState.Present)
    .Satisfy(Need.Rest)
    .Mark(Theme.Connection, +0.3);
```

### Task 42.5: Narrative Query — Fråga berättelsen

```csharp
// Vilka beats kan aktiveras nu?
var possibleBeats = story.Beats
    .Where(b => b.CanActivate(currentState))
    .OrderByDescending(b => b.Specificity)  // Mest specifika först
    .ToList();

// Vilka behov är ouppfyllda?
var unmetNeeds = story.State.ActiveNeeds
    .Where(n => !n.IsSatisfied)
    .OrderBy(n => n.Urgency);

// Vilka teman utvecklas?
var risingThemes = story.State.ThemeProgress
    .Where(t => t.Value > 0.3f && t.Value < 0.8f)
    .Select(t => t.Key);

// Hitta alla vägar till ett mål-tillstånd
var pathsToHope = story.FindPaths(
    from: currentState,
    to: s => s.WorldMood == Mood.Hope,
    maxDepth: 10
);

// Vilka minnen är relevanta här?
var echoes = story.State.Memories
    .Where(m => m.RelatesTo(currentState.Location))
    .Select(m => m.Echo());
```

### Task 42.6: Three-Layer Output

```csharp
// Samma beat, tre uttrycksnivåer
story.DefineBeat("open_mysterious_door")
    .When(s => s.Player.Uses("key").On("ancient_door"))
    .Shift(s => s.WorldMood, Mood.Dread)
    .Awaken(Need.Understanding)
    .Mark(Theme.Truth, +0.2)
    .Mark(Theme.Loss, +0.1)

    // Tre nivåer av berättande
    .Mechanical("The door opens. Cold air escapes.")
    .Narrative("Some doors are closed for a reason. This one remembers.")
    .Psychological("A weight settles in your chest. You wanted to know.");

// Motorn väljer nivå baserat på:
// - Spelarinställning (prefer_mechanical, prefer_narrative, prefer_psychological)
// - Tematisk intensitet (hög Theme.Truth → mer psychological)
// - Pacing (snabba sekvenser → mechanical, lugna → narrative)
```

### Task 42.7: Character Arcs som Queries

```csharp
// NPC har sin egen narrativa kurva
var hermit = story.DefineCharacter("hermit")
    .WithWound("lost_family")
    .WithMask("cynicism")
    .WithNeed("connection")
    .TransformsWhen(s =>
        s.Relationship("hermit") > 50 &&
        s.ThemeProgress[Theme.Trust] > 0.6f
    );

// Karaktärens dialog som query
hermit.Says()
    .When(s => s.FirstMeeting)
    .And(s => s.WorldMood == Mood.Fear)
    .Line("Another lost soul. The forest collects them.")
    .Mark(Theme.Isolation, +0.1);

hermit.Says()
    .When(s => s.Relationship("hermit") > 30)
    .And(s => s.Player.Has("family_locket"))
    .Line("That locket... where did you find it?")
    .Shift(s => hermit.Mask, "vulnerability")
    .Awaken(Need.Closure)
    .Mark(Theme.Connection, +0.3);
```

### Task 42.8: Theme Arc Tracking

```csharp
// Definiera tematisk båge
story.DefineThemeArc(Theme.Trust)
    .StartsAt(0.0f, "Isolation", "You trust no one. Wise.")
    .DevelopsAt(0.3f, "First Doubt", "Perhaps not everyone is an enemy.")
    .DevelopsAt(0.5f, "Tentative Bond", "You find yourself caring.")
    .DevelopsAt(0.7f, "Tested", "Trust is easy until it costs something.")
    .ResolvesAt(1.0f, "Mutual Dependence", "Together, or not at all.");

// Query: Var är vi i bågen?
var trustPhase = story.GetThemePhase(Theme.Trust);
// => "Tentative Bond" (om ThemeProgress[Trust] == 0.55)

// Automatiska callbacks vid trösklar
story.OnThemeReaches(Theme.Trust, 0.5f, ctx => {
    ctx.Reveal("Something has shifted. You catch yourself hoping.");
    ctx.TriggerBeat("trust_milestone");
});
```

### Task 42.9: Story-LINQ DSL

```dsl
# Deklarativ syntax för icke-programmerare

beat "morning_peace" {
    when player.has("coffee")
     and player.has("book")
     and location.tag("comfortable")
     and time.phase == morning

    shift mood to stillness
    shift inner to present
    satisfy need:rest

    mark theme:connection +0.3
    mark theme:growth +0.1

    reveal "The world holds its breath. So do you."
}

character "hermit" {
    wound: "lost_family"
    mask: "cynicism"
    need: "connection"

    transforms when relationship > 50 and theme:trust > 0.6

    says when first_meeting and mood == fear {
        "Another lost soul. The forest collects them."
    }
}

theme_arc "trust" {
    0.0: "Isolation" -> "You trust no one."
    0.3: "First Doubt" -> "Perhaps not everyone is an enemy."
    0.5: "Tentative Bond" -> "You find yourself caring."
    0.7: "Tested" -> "Trust costs something."
    1.0: "Mutual Dependence" -> "Together, or not at all."
}
```

### Task 42.10: Integration med existerande slices

```csharp
// Story-LINQ integrerar med Event System (Slice 6)
story.OnEvent("player_enters_location")
    .When(s => s.Location.HasTag("childhood_home"))
    .And(s => s.State.Memories.Contains("happy_childhood"))
    .Echo("happy_childhood")
    .Shift(s => s.WorldMood, Mood.Stillness)
    .Reveal("The smell of old wood. You remember.");

// Integration med NPC Dialog (Slice 5)
npc.DialogRule()
    .When(s => s.ThemeProgress[Theme.Trust] > 0.5f)
    .And(s => s.PlayerInnerState == InnerState.Present)
    .Say("You're different now. Calmer. What changed?")
    .Mark(Theme.Growth, +0.1);

// Integration med Quest System (Slice 8)
quest.OnComplete()
    .Shift(s => s.WorldMood, Mood.Hope)
    .Satisfy(Need.Closure)
    .Mark(Theme.Sacrifice, +0.4)
    .Reveal("It cost everything. It was worth everything.");
```

### Task 42.11: Sandbox — Morning Ritual

```csharp
// Komplett implementation av kaffe-och-bok-scenariot

var morningGame = new GameBuilder()
    .WithStoryLinq()  // Aktivera Story-LINQ
    .Build();

// Definiera tillstånd
morningGame.DefineNeeds(Need.WakeUp, Need.Rest);
morningGame.DefineThemes(Theme.Connection, Theme.Growth);

// Definiera beats
morningGame.DefineBeat("leave_bed")
    .When(s => s.Location.Id == "bedroom")
    .And(s => s.PlayerPosture == Posture.Lying)
    .Shift(s => s.PlayerPosture, Posture.Standing)
    .Shift(s => s.PlayerInnerState, InnerState.Groggy)
    .Mechanical("You get out of bed.")
    .Narrative("The day begins, whether you want it to or not.")
    .Psychological("Inertia broken. Now what?");

morningGame.DefineBeat("brew_coffee")
    .When(s => s.Location.Id == "kitchen")
    .And(s => s.Player.Has("mug"))
    .And(s => s.ActiveNeeds.Contains(Need.WakeUp))
    .Yields("coffee")
    .Shift(s => s.WorldMood, Mood.Awakening)
    .Shift(s => s.PlayerInnerState, InnerState.Awake)
    .Mark(Theme.Connection, +0.1)
    .Mechanical("Coffee made.")
    .Narrative("Warmth enters the body. Consciousness returns.")
    .Psychological("A small act of self-care.");

morningGame.DefineBeat("peaceful_reading")
    .When(s => s.Location.Id == "livingroom")
    .And(s => s.Player.Has("coffee"))
    .And(s => s.Player.Has("book"))
    .Shift(s => s.PlayerPosture, Posture.Sitting)
    .Shift(s => s.PlayerInnerState, InnerState.Present)
    .Shift(s => s.WorldMood, Mood.Stillness)
    .Satisfy(Need.WakeUp)
    .Satisfy(Need.Rest)
    .Mark(Theme.Connection, +0.3)
    .Mark(Theme.Growth, +0.2)
    .Mechanical("You sit down, drink coffee, read.")
    .Narrative("The world quiets. The book opens like a small, private universe.")
    .Psychological("Arrival. Presence. Enough.");

// Dramaturgisk båge i 4 beats:
// Inertia → Awakening → Comfort → Presence
```

---

En tanke på hur det ska fungera

_storytelling som kodstruktur_, inte som löpande text.

Om vi tänker rent arkitektoniskt kan du se berättelsen som tre lager som samverkar:

1. **Tillstånd (State)** – hur världen _är_
2. **Händelser (Events)** – vad som _händer_
3. **Mening (Narrative Logic)** – varför det _betyder något_

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

Alltså: inte “kapitel 1, kapitel 2”, utan _existentiella lägen_.

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

NPC är inte bara state machines, utan _pågående berättelser_:

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

OK vi säger exempelvis ett spel där spelaren ligger i sin säng och ska gå till köket göra kaffe, ta kaffe, gå till vardagsrum, hämta en bok, sätta sig i soffan dricka kaffe och läsa boken hur skulle det beskrivas i detta fluent state

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

Nu definierar vi _beats_ som inte bara är handlingar, utan förändrar tillstånd och mening.

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

Det här är kul: det är egentligen _ett nytt existentiellt tillstånd_.

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

Motorn matchar beat: _LeaveBed_

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

Beat: _BrewCoffee_

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

Beat: _FindBook_

> You pick up the book. Its weight feels familiar.

HasBook = true
Theme(Contemplation) +

---

**Spelaren:**
`sit down`

Motorn ser: Location = LivingRoom, HasCoffee, HasBook, Posture = Standing
Beat: _SettleWithBook_

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

---

## Kodjämförelse

**CSharp**

```cs
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Parsing;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Helpers;
using MarcusMedina.TextAdventure.Localization;

// Create items (all styles are valid)
Key cabinKey = (id: "cabin_key", name: "brass key", description: "A small brass key with worn teeth.");
cabinKey.SetWeight(0.1f).AddAliases("key", "brass key");

Item sword = (id: "sword", name: "rusty sword", description: "A rusted blade with a dull edge.");
sword.SetWeight(3.5f).AddAliases("blade", "sword", "pointy thing");

Item apple = (id: "apple", name: "red apple", description: "A crisp red apple.");
apple.SetWeight(0.4f).AddAliases("apple");

Glass glass = (id: "glass", name: "glass", description: "A clear drinking glass.");
glass.SetWeight(0.6f)
    .SetReaction(ItemAction.Take, "The glas surface is smooth")
    .SetReaction(ItemAction.Drop, "The glass bounces on the floor") // can we destroy it from this reaction?
    .SetReaction(ItemAction.Destroy, "The glass shatters into 1000 pieces");
    ;

Item ice = (id: "ice", name: "ice", description: "A cold chunk of ice.");
ice.SetWeight(0.5f)
    .SetReaction(ItemAction.Take, "The cold chills your hand.")
    .SetReaction(ItemAction.Drop, "It lands with a soft thump.")
    .SetReaction(ItemAction.Use, "You take a bite. Your teeth ache.");

Item fire = (id: "fire", name: "fire", description: "A flickering flame.");
fire.SetWeight(0.5f);

Item lantern = (id: "lantern", name: "lantern", description: "A lantern that casts a warm glow.");
lantern.SetWeight(1.2f);

Item sign = (id: "sign", name: "wooden sign", description: "A weathered wooden sign.");
sign.SetTakeable(false)
    .SetReadable()
    .SetReadText("Welcome to the Dark Forest!");

Item newspaper = (id: "newspaper", name: "daily news", description: "A crinkled newspaper.");
newspaper.SetReadable()
    .RequireTakeToRead()
    .SetReadText("HEADLINE: Dragon spotted near village!");

Item tome = (id: "tome", name: "ancient tome", description: "A heavy book with faded runes.");
tome.SetReadable()
    .RequireTakeToRead()
    .SetReadingCost(3)
    .SetReadText("The secret to defeating the dragon is...");

Item letter = (id: "letter", name: "sealed letter", description: "A sealed letter with red wax.");
letter.SetReadable()
    .RequireTakeToRead()
    .RequiresToRead(s => s.Inventory.Items.Any(i => i.Id == "lantern"))
    .SetReadText("Meet me at midnight...");

// Create locations
Location entrance = (
    id: "entrance",
    description: "You stand at the forest gate. It's dark and foreboding."
    );
Location forest = (
    id: "forest",
    description: "A thick forest surrounds you. Shadows stretch long between ancient trees."
    );
Location cave = (
    id: "cave",
    description: "A dark cave with glowing mushrooms. A brass key glints on the ground!"
    );
Location clearing = (
    id: "clearing", description: "A sunny clearing with wildflowers. A small cabin stands here."
    );
Location cabin = (
    id: "cabin",
    description: "Inside a cozy wooden cabin. A treasure chest sits in the corner!"
    );

// Place items
cave.AddItem(cabinKey);
entrance.AddItem(ice);
entrance.AddItem(sign);
forest.AddItem(apple);
forest.AddItem(fire);
forest.AddItem(lantern);
clearing.AddItem(sword);
clearing.AddItem(glass);
clearing.AddItem(tome);
cave.AddItem(letter);
cabin.AddItem(newspaper);

// Create locked door
Door cabinDoor = (id: "cabin_door", name: "cabin door", description: "A sturdy wooden door with iron hinges.");
cabinDoor.RequiresKey(cabinKey);
cabinDoor
    .SetReaction(DoorAction.Unlock, "The lock clicks open.")
    .SetReaction(DoorAction.Open, "The door creaks as it swings wide.");

// Connect locations
entrance.AddExit(Direction.North, forest);
forest.AddExit(Direction.East, cave);
forest.AddExit(Direction.West, clearing);
clearing.AddExit(Direction.In, cabin, cabinDoor);  // Locked!

// One-way trap
cave.AddExit(Direction.Down, entrance, oneWay: true);

// Game state
var recipeBook = new RecipeBook()
    .Add(new ItemCombinationRecipe("ice", "fire", () => new FluidItem("water", "water", "Clear and cold.")));
var state = new GameState(entrance, recipeBook: recipeBook);

Console.WriteLine("=== FOREST ADVENTURE ===");
Console.WriteLine("Find the key and unlock the cabin!");
var commands = new[]
{
    "go", "look", "read", "open", "unlock", "take", "drop", "use", "inventory", "stats", "combine", "pour", "quit"
};
Console.WriteLine($"Commands: {commands.CommaJoin()} (or just type a direction)\n");

var parserConfig = new KeywordParserConfig(
    quit: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "quit", "exit", "q" },
    look: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "look", "l", "ls" },
    inventory: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "inventory", "inv", "i" },
    stats: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "stats", "stat", "hp", "health" },
    open: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "open" },
    unlock: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "unlock" },
    take: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "take", "get", "pickup", "pick" },
    drop: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "drop" },
    use: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "use", "eat", "bite" },
    combine: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "combine", "mix" },
    pour: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "pour" },
    go: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "go", "move", "cd" },
    read: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "read" },
    all: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "all" },
    ignoreItemTokens: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "up", "to" },
    combineSeparators: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "and", "+" },
    pourPrepositions: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "into", "in" },
    directionAliases: new Dictionary<string, Direction>(StringComparer.OrdinalIgnoreCase)
    {
        ["n"] = Direction.North,
        ["s"] = Direction.South,
        ["e"] = Direction.East,
        ["w"] = Direction.West,
        ["ne"] = Direction.NorthEast,
        ["nw"] = Direction.NorthWest,
        ["se"] = Direction.SouthEast,
        ["sw"] = Direction.SouthWest,
        ["u"] = Direction.Up,
        ["d"] = Direction.Down,
        ["in"] = Direction.In,
        ["out"] = Direction.Out
    },
    allowDirectionEnumNames: true);

var parser = new KeywordParser(parserConfig);

while (true)
{
    var lookResult = state.Look();
    Console.WriteLine($"\n{lookResult.Message}");
    var inventoryResult = state.InventoryView();
    Console.WriteLine(inventoryResult.Message);

    Console.Write("\n> ");
    var input = Console.ReadLine()?.Trim().Lower();

    if (string.IsNullOrEmpty(input)) continue;

    var command = parser.Parse(input);
    var result = state.Execute(command);

    if (!string.IsNullOrWhiteSpace(result.Message))
    {
        Console.WriteLine(result.Message);
    }

    foreach (var reaction in result.ReactionsList)
    {
        if (!string.IsNullOrWhiteSpace(reaction))
        {
            Console.WriteLine($"> {reaction}");
        }
    }

    if (result.ShouldQuit)
    {
        break;
    }

    if (command is GoCommand go && result.Success)
    {
        // Win condition
        if (state.IsCurrentRoomId("cabin"))
        {
            Console.WriteLine("\n*** CONGRATULATIONS! You found the treasure! ***");
            break;
        }

        // Fall trap
        if (go.Direction == Direction.Down && state.CurrentLocation.Id == "entrance")
        {
            Console.WriteLine("Oops! You fell through a hole and ended up back at the entrance!");
        }
    }
}

Console.WriteLine("\nThanks for playing!");
```

\*_DSL (.adventure)_

```cs
Story
    .World("Dark Forest")
    .Mood(Mood.Ominous)
    .Goal("Find the key and unlock the cabin")

    .Location("Entrance")
        .Description("You stand at the forest gate. It's dark and foreboding.")
        .Contains("Ice").As(Cold, Fragile)
        .Contains("Sign").Readable("Welcome to the Dark Forest!")
        .Exit(North).To("Forest")

    .Location("Forest")
        .Description("A thick forest surrounds you. Shadows stretch long between ancient trees.")
        .Contains("Apple").As(Food, Comfort)
        .Contains("Fire").As(Heat, Danger)
        .Contains("Lantern").As(Light, Safety)
        .Exit(East).To("Cave")
        .Exit(West).To("Clearing")

    .Location("Cave")
        .Description("A dark cave with glowing mushrooms. A brass key glints on the ground.")
        .Contains("CabinKey").As(Key, Progress)
        .Contains("Letter")
            .ReadableWhen(Player.Has("Lantern"))
            .Text("Meet me at midnight...")
        .Trap(Down).LeadsTo("Entrance").As(FallBack)

    .Location("Clearing")
        .Description("A sunny clearing with wildflowers. A small cabin stands here.")
        .Contains("Sword").As(Weapon, Old)
        .Contains("Glass").As(Container, Fragile)
        .Contains("Tome")
            .Readable()
            .Costs(Time: 3)
            .Reveals("The secret to defeating the dragon is...")
        .Exit(In)
            .To("Cabin")
            .BlockedBy("CabinDoor")

    .Location("Cabin")
        .Description("Inside a cozy wooden cabin. A treasure chest sits in the corner!")
        .Contains("Newspaper")
            .Readable()
            .Text("HEADLINE: Dragon spotted near village!")

    .Door("CabinDoor")
        .Requires("CabinKey")
        .OnUnlock("The lock clicks open.")
        .OnOpen("The door creaks as it swings wide.")

    .Item("Ice")
        .OnTake("The cold chills your hand.")
        .OnUse("Your teeth ache from the cold.")

    .Item("Fire")
        .OnCombineWith("Ice")
        .Creates("Water")
        .Meaning("Opposites dissolve into balance.")

    .Item("Lantern")
        .Affects(World.Darkness, -1)
        .Meaning("Light makes truth readable.")

    .NarrativeArc("Awakening")
        .StartsAt("Entrance", State: Uncertain)
        .DeepensIn("Forest", Theme: Fear)
        .RevealsIn("Cave", Theme: Knowledge)
        .ResolvesIn("Cabin", Theme: Safety)

    .WinCondition(
        when: Player.IsIn("Cabin"),
        meaning: "You have found shelter, warmth, and understanding."
    );
```

---

## Slice 43: Map Generator

**Mål:** Skapa en enkel map generator som kan rendera en ASCII-karta baserat på location-grafen och exits.

### Förslag på funktioner

- `MapGenerator.Render(GameState state)` → `string`
- Valfritt: `MapGenerator.Render(ILocation start, int maxDepth)` → `string`

### Krav

- Fungerar med Slice 1 (Location + Exits) utan extra beroenden.
- Möjlig att använda i sandbox för att visa en karta vid `look`.

---

## Slice 44: String Case Utilities

**Mål:** Enkla stränghelpers för casing i UI/texter.

### Funktioner

- `string.ToProperCase()` — Title Case.
- `string.ToSentenceCase()` — Första bokstaven versal, resten gemener.
- `string.ToCrazyCaps()` — Slumpad versal/gemen per bokstav.

**Notis:** Använder `Random.Shared`.

---

## Slice 45: Generic Fixes

**Mål:** Samla upp generella förbättringar som dyker upp under verifiering.

### Förslag på funktioner

- `IItem.Amount` (nullable int) + `Item.SetAmount(int amount)`
- `IItem.DecreaseAmount(int amount = 1)` → bool (om det finns kvar)
- Optional: `Item.OnAmountEmpty` reaction/hook
- `Use()` minskar amount om den finns (och tar bort item när 0)
- `IItem.PresenceDescription` + `Item.SetPresenceDescription(string text)`
- Look visar `PresenceDescription` mellan rumsbeskrivning och items/exits

### Krav

- Backwards compatible: items utan amount fungerar som tidigare.
- Inventory/Look visar amount när den finns (t.ex. “Tea Thermos (4)”).
