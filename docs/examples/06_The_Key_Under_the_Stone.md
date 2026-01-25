# The Key Under the Stone

_Slice tag: Slice 6 — Event System (Observer). Demo focuses on events revealing items and reacting to actions._

## Story beats (max ~10 steps)
1) You stand in a small garden.
2) A stone rests near a gate.
3) Move the stone.
4) Find the hidden key.
5) Unlock the gate.

## Example (events + doors)
```csharp
using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;

// Location
var garden = new Location("garden", "A quiet garden with a heavy stone near a gate.");

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
        ["n"] = Direction.North
    },
    allowDirectionEnumNames: true);

var parser = new KeywordParser(parserConfig);
```
