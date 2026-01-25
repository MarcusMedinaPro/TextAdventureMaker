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
