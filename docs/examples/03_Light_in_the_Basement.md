# Light in the Basement

_Slice tag: Slice 3 — Command Pattern + Parser. Demo focuses on parsing commands and driving actions via ICommand._

## Story beats (max ~10 steps)
1) You stand at the top of basement stairs.
2) The basement is dark.
3) Find the flashlight.
4) Turn it on.
5) Descend.
6) You can finally see what was there.

## Example (commands + parser)
```csharp
using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Helpers;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;

// Locations
var hallway = new Location("hallway", "A narrow hallway. The basement stairs lead down.");
var basement = new Location("basement", "Pitch black. You hear a faint hum.");

hallway.AddExit(Direction.Down, basement);
basement.AddExit(Direction.Up, hallway);

// Items
var flashlight = new Item("flashlight", "flashlight", "A small flashlight.");
var switcher = new Item("switch", "flashlight switch", "A tiny switch.");

hallway.AddItem(flashlight);

// Game state
var state = new GameState(hallway, worldLocations: new[] { hallway, basement });

// Use-command hook (minimal): flip a flag when flashlight is used
state.Events.Subscribe(GameEventType.PickupItem, e =>
{
    if (e.Item != null && e.Item.Id == "flashlight")
    {
        state.WorldState.SetFlag("has_flashlight", true);
    }
});

// Parser config (slice 3 core commands)
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
        ["down"] = Direction.Down,
        ["up"] = Direction.Up
    },
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

    if (command is GoCommand go && go.Direction == Direction.Down)
    {
        var hasLight = state.WorldState.GetFlag("has_flashlight");
        Console.WriteLine(hasLight
            ? "You descend and the darkness gives way to shapes."
            : "It's too dark. You hesitate on the steps.");
    }

    if (result.ShouldQuit) break;
}
```
