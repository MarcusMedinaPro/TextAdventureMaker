# Rain on the Roof

_Slice tag: Slice 7 — Combat (Strategy). Demo focuses on a lightweight “struggle” using combat system hooks._

## Story beats (max ~10 steps)
1) Rain leaks through the roof.
2) A bucket is nearby.
3) The leak worsens ("battle" with the storm).
4) Place the bucket and endure.

## Example (combat system used as a simple struggle)
```csharp
using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Helpers;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;

// Location
var attic = new Location("attic", "The roof leaks in steady drops.");

// Items
var bucket = new Item("bucket", "bucket", "A metal bucket.");
attic.AddItem(bucket);

// NPC as the "storm"
var storm = new Npc("storm", "storm", NpcState.Hostile, stats: new Stats(15))
    .Description("A relentless leak you must endure.");
attic.AddNpc(storm);

// Game state
var state = new GameState(attic, worldLocations: new[] { attic });

// Parser config (attack/flee)
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
    directionAliases: new Dictionary<string, Direction>(StringComparer.OrdinalIgnoreCase),
    allowDirectionEnumNames: true);

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

    if (!storm.IsAlive)
    {
        Console.WriteLine("The bucket catches the leak. The storm passes.");
        break;
    }

    if (result.ShouldQuit) break;
}
```
