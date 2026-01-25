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
