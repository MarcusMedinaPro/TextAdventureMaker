# Morning Ritual

_Slice tag: Slice 1 — Location + Navigation. Demo focuses on moving between rooms in a simple routine._

A tiny, quiet demo about waking up, making coffee, finding the newspaper, and reading in the sun.

## Story beats (max ~10 steps)
1) Wake up in the bedroom.
2) Walk to the kitchen.
3) Go to the living room.
4) Sit down by the window.

## Example (core engine + navigation only)
```csharp
using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Helpers;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;

// Locations
var bedroom = new Location("bedroom", "Soft morning light spills across the room.");
var kitchen = new Location("kitchen", "A kettle hums quietly on the counter.");
var livingRoom = new Location("living_room", "A couch by the window invites you to sit.");

bedroom.AddExit(Direction.East, kitchen);
kitchen.AddExit(Direction.South, livingRoom);
livingRoom.AddExit(Direction.North, kitchen);

// Game state
var state = new GameState(bedroom, worldLocations: new[] { bedroom, kitchen, livingRoom });

// Parser config (minimal)
var parserConfig = new KeywordParserConfig(
    quit: CommandHelper.NewCommands("quit"),
    look: CommandHelper.NewCommands("look"),
    inventory: CommandHelper.NewCommands("inventory"),
    stats: CommandHelper.NewCommands("stats"),
    open: CommandHelper.NewCommands("open"),
    unlock: CommandHelper.NewCommands("unlock"),
    take: CommandHelper.NewCommands("take"),
    drop: CommandHelper.NewCommands("drop"),
    use: CommandHelper.NewCommands("use"),
    combine: CommandHelper.NewCommands("combine"),
    pour: CommandHelper.NewCommands("pour"),
    go: CommandHelper.NewCommands("go"),
    read: CommandHelper.NewCommands("read"),
    talk: CommandHelper.NewCommands("talk"),
    attack: CommandHelper.NewCommands("attack"),
    flee: CommandHelper.NewCommands("flee"),
    save: CommandHelper.NewCommands("save"),
    load: CommandHelper.NewCommands("load"),
    all: CommandHelper.NewCommands("all"),
    ignoreItemTokens: CommandHelper.NewCommands(),
    combineSeparators: CommandHelper.NewCommands(),
    pourPrepositions: CommandHelper.NewCommands(),
    directionAliases: new Dictionary<string, Direction>(StringComparer.OrdinalIgnoreCase)
    {
        ["e"] = Direction.East,
        ["s"] = Direction.South,
        ["n"] = Direction.North
    },
    allowDirectionEnumNames: true);

var parser = new KeywordParser(parserConfig);

// Run loop
while (true)
{
    Console.Write("\n> ");
    var input = Console.ReadLine()?.Trim();
    if (string.IsNullOrWhiteSpace(input)) continue;

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

    if (result.ShouldQuit) break;
}
```
