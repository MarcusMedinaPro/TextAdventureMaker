# The Key Under the Stone

_Slice tag: Slice 6 — Event System (Observer). Demo focuses on events revealing items and reacting to actions._

## Story beats (max ~10 steps)
1) You stand in a small garden.
2) A stone rests near a gate.
3) Move the stone.
4) Find the hidden key.
5) Unlock the gate.

## Map (rough layout)
```
          N
    W           E
          S

┌────────────────────┐
│                    │
│       Garden       │
│                    │
│        S           │
│                    │
└─────────┬──────────┘
          │
       [Gate]

S = Stone (key hidden underneath)
```

## Example (events + doors)
```csharp
using System.Linq;
using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Helpers;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;

// Location
var garden = new Location("garden", "A quiet garden with a weathered gate and a patient silence.");

// Items
var stone = new Item("stone", "stone", "A heavy flat stone.");
var key = new Key("gate_key", "iron key", "An iron key hidden under the stone.");

garden.AddItem(stone);

// Door
var gate = new Door("gate", "garden gate", "A locked iron gate.")
    .RequiresKey(key)
    .SetReaction(DoorAction.Unlock, "The gate creaks open.");

garden.AddExit(Direction.North, garden, gate);

// Game state
var state = new GameState(garden, worldLocations: new[] { garden });
state.EnableFuzzyMatching = true;
state.FuzzyMaxDistance = 1;

// Event: when stone is taken, reveal the key
state.Events.Subscribe(GameEventType.PickupItem, e =>
{
    if (e.Item != null && e.Item.Id == "stone")
    {
        garden.AddItem(key);
        Console.WriteLine("You lift the stone and find a key beneath it.");
    }
});

// Parser config (minimal)
var parserConfig = new KeywordParserConfig(
    quit: CommandHelper.NewCommands("quit"),
    look: CommandHelper.NewCommands("look"),
    examine: CommandHelper.NewCommands("examine"),
    inventory: CommandHelper.NewCommands("inventory"),
    stats: CommandHelper.NewCommands("stats"),
    open: CommandHelper.NewCommands("open"),
    unlock: CommandHelper.NewCommands("unlock"),
    take: CommandHelper.NewCommands("take"),
    drop: CommandHelper.NewCommands("drop"),
    use: CommandHelper.NewCommands("use"),
    combine: CommandHelper.NewCommands("combine"),
    pour: CommandHelper.NewCommands("pour"),
    move: CommandHelper.NewCommands("move", "push", "shift", "lift", "slide"),
    go: CommandHelper.NewCommands("go"),
    read: CommandHelper.NewCommands("read"),
    talk: CommandHelper.NewCommands("talk"),
    attack: CommandHelper.NewCommands("attack"),
    flee: CommandHelper.NewCommands("flee"),
    save: CommandHelper.NewCommands("save"),
    load: CommandHelper.NewCommands("load"),
    quest: CommandHelper.NewCommands("quest"),
    all: CommandHelper.NewCommands("all"),
    ignoreItemTokens: CommandHelper.NewCommands(),
    combineSeparators: CommandHelper.NewCommands(),
    pourPrepositions: CommandHelper.NewCommands(),
    directionAliases: new Dictionary<string, Direction>(StringComparer.OrdinalIgnoreCase)
    {
        ["n"] = Direction.North
    },
    allowDirectionEnumNames: true,
    enableFuzzyMatching: true,
    fuzzyMaxDistance: 1);

var parser = new KeywordParser(parserConfig);

Console.WriteLine("=== THE KEY UNDER THE STONE (Slice 6) ===");
Console.WriteLine("Goal: reveal the key, unlock the gate, and listen for the reaction.");
ShowRoom();

// Input loop
while (true)
{
    Console.Write("\n> ");
    var input = Console.ReadLine()?.Trim();
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

    if (command is GoCommand && !result.ShouldQuit)
    {
        ShowRoom();
    }

    if (result.ShouldQuit) break;
}

void ShowRoom()
{
    Console.WriteLine();
    Console.WriteLine($"Room: {state.CurrentLocation.Id.ToProperCase()}");
    Console.WriteLine(state.CurrentLocation.GetDescription());

    var items = state.CurrentLocation.Items.CommaJoinNames(properCase: true);
    Console.WriteLine(string.IsNullOrWhiteSpace(items) ? "Items: None" : $"Items: {items}");

    var exits = state.CurrentLocation.Exits.Keys
        .Select(direction => direction.ToString().ToLowerInvariant().ToProperCase())
        .ToList();
    Console.WriteLine(exits.Count > 0 ? $"Exits: {exits.CommaJoin()}" : "Exits: None");
}
```
