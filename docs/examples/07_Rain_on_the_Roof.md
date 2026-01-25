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

    if (!storm.IsAlive)
    {
        Console.WriteLine("The bucket catches the leak. The storm passes.");
        break;
    }

    if (result.ShouldQuit) break;
}
```
