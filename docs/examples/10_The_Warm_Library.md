# The Warm Library

_Slice tag: Slice 10 — Save/Load (Memento). Demo focuses on saving and restoring a cozy scene._

## Story beats (max ~10 steps)
1) You stand outside a locked library.
2) Find the key in the snow.
3) Enter the warm library.
4) Save your progress.
5) Load it later.

## Example (save/load)
```csharp
using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Helpers;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;

// Locations
var outside = new Location("outside", "Snow falls quietly outside a locked library.");
var library = new Location("library", "Warm light and quiet pages surround you.");

// Items
var key = new Key("library_key", "library key", "Cold metal in your hand.");
outside.AddItem(key);

// Door
var door = new Door("library_door", "library door", "A heavy wooden door.")
    .RequiresKey(key)
    .SetReaction(DoorAction.Unlock, "The library door unlocks.");

outside.AddExit(Direction.In, library, door);

// Game state
var state = new GameState(outside, worldLocations: new[] { outside, library });
state.RegisterLocations(new[] { outside, library });

// Parser config (save/load enabled)
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
    all: CommandHelper.NewCommands("all"),
    ignoreItemTokens: CommandHelper.NewCommands(),
    combineSeparators: CommandHelper.NewCommands(),
    pourPrepositions: CommandHelper.NewCommands(),
    directionAliases: new Dictionary<string, Direction>(StringComparer.OrdinalIgnoreCase)
    {
        ["in"] = Direction.In
    },
    allowDirectionEnumNames: true,
    enableFuzzyMatching: true,
    fuzzyMaxDistance: 1);

var parser = new KeywordParser(parserConfig);

// Input loop (simplified)
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

    if (result.ShouldQuit) break;
}
```
