# Morning Ritual

_Slice tag: Slice 10 — Save/Load (Memento). Demo focuses on simple worldstate progression and a calm “routine” loop._

A tiny, quiet demo about waking up, making coffee, finding the newspaper, and reading in the sun.

## Story beats (max ~10 steps)
1) Wake up in the bedroom.
2) Walk to the kitchen.
3) Grab a mug and coffee beans.
4) Find the newspaper.
5) Go to the living room.
6) Sit down and read.

## Example (core engine + worldstate flags)
```csharp
using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;

// Locations
var bedroom = new Location("bedroom", "Soft morning light spills across the room.");
var kitchen = new Location("kitchen", "A kettle hums quietly on the counter.");
var livingRoom = new Location("living_room", "A couch by the window invites you to sit.");

bedroom.AddExit(Direction.East, kitchen);
kitchen.AddExit(Direction.South, livingRoom);
livingRoom.AddExit(Direction.North, kitchen);

// Items
var mug = new Item("mug", "mug", "A clean ceramic mug.");
var beans = new Item("beans", "coffee beans", "Freshly roasted coffee beans.");
var newspaper = new Item("newspaper", "morning paper", "The morning headlines.")
    .SetReadable()
    .SetReadText("You read the headlines. The world feels far away.");

kitchen.AddItem(mug);
kitchen.AddItem(beans);
bedroom.AddItem(newspaper);

// Game state
var state = new GameState(bedroom, worldLocations: new[] { bedroom, kitchen, livingRoom });

// WorldState hooks
state.Events.Subscribe(GameEventType.PickupItem, e =>
{
    if (e.Item == null) return;
    if (e.Item.Id == "mug") state.WorldState.SetFlag("has_mug", true);
    if (e.Item.Id == "beans") state.WorldState.SetFlag("has_beans", true);
    if (e.Item.Id == "newspaper") state.WorldState.SetFlag("has_paper", true);
});

state.Events.Subscribe(GameEventType.EnterLocation, e =>
{
    if (e.Location == null || e.Location.Id != "living_room") return;

    var ready = state.WorldState.GetFlag("has_mug")
        && state.WorldState.GetFlag("has_beans")
        && state.WorldState.GetFlag("has_paper");

    if (ready)
    {
        Console.WriteLine("Coffee in hand, you settle into the couch and start to read.");
    }
    else
    {
        Console.WriteLine("You feel like you're missing something.");
    }
});

// Parser config (minimal)
var parserConfig = new KeywordParserConfig(
    quit: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "quit" },
    look: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "look" },
    inventory: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "inventory" },
    stats: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "stats" },
    open: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "open" },
    unlock: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "unlock" },
    take: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "take" },
    drop: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "drop" },
    use: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "use" },
    combine: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "combine" },
    pour: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "pour" },
    go: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "go" },
    read: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "read" },
    talk: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "talk" },
    attack: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "attack" },
    flee: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "flee" },
    save: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "save" },
    load: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "load" },
    all: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "all" },
    ignoreItemTokens: new HashSet<string>(StringComparer.OrdinalIgnoreCase),
    combineSeparators: new HashSet<string>(StringComparer.OrdinalIgnoreCase),
    pourPrepositions: new HashSet<string>(StringComparer.OrdinalIgnoreCase),
    directionAliases: new Dictionary<string, Direction>(StringComparer.OrdinalIgnoreCase)
    {
        ["e"] = Direction.East,
        ["s"] = Direction.South,
        ["n"] = Direction.North
    },
    allowDirectionEnumNames: true);

var parser = new KeywordParser(parserConfig);
```
