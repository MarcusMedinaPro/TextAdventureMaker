# The Last Train Home

_Slice tag: Slice 4 — Items + Inventory. Demo focuses on items, pickup, inventory, and a simple choice gate._

## Story beats (max ~10 steps)
1) You arrive at the platform.
2) A ticket booth is closed.
3) Find a ticket on the bench.
4) Decide: board the train or stay.

## Example (items + inventory)
```csharp
using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Helpers;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;

// Locations
var platform = new Location("platform", "A quiet platform. The last train waits.");

// Items
var ticket = new Item("ticket", "train ticket", "A single-ride ticket.");
platform.AddItem(ticket);

// Game state
var state = new GameState(platform, worldLocations: new[] { platform });

// Parser config (slice 4 core commands)
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

// Simple choice loop
while (true)
{
    Console.Write("\n> ");
    var input = Console.ReadLine()?.Trim();
    if (string.IsNullOrEmpty(input)) continue;

    if (string.Equals(input, "board", StringComparison.OrdinalIgnoreCase))
    {
        var hasTicket = state.Inventory.Items.Any(i => i.Id == "ticket");
        Console.WriteLine(hasTicket
            ? "You board the train. The city fades behind you."
            : "You need a ticket to board.");
        break;
    }

    if (string.Equals(input, "stay", StringComparison.OrdinalIgnoreCase))
    {
        Console.WriteLine("You let the train go. You stay behind.");
        break;
    }

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
