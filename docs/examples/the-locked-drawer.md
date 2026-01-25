# The Locked Drawer

A tiny, focused demo with one room, a desk, a locked drawer, a photo, and a secret.

## Story beats (max ~10 steps)
1) You enter a quiet study.
2) You notice a desk with a locked drawer.
3) Find the key under the blotter.
4) Unlock the drawer.
5) Discover a photo.
6) Read the note on the back.

## Example (core engine + simple gating)
```csharp
using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;

// Location
var study = new Location("study", "A quiet study with a heavy desk and a single lamp.");

// Items
var key = new Key("drawer_key", "small key", "A small brass key.")
    .SetWeight(0.1f);
var photo = new Item("photo", "old photo", "A faded photo of a stranger.")
    .SetReadable()
    .SetReadText("On the back: 'Meet me at the pier.'");

study.AddItem(key);

// Door-as-drawer (locked)
var drawer = new Door("drawer", "drawer", "A locked desk drawer.")
    .RequiresKey(key)
    .SetReaction(DoorAction.Unlock, "The drawer clicks open.");

// Exit as a fake container gate
study.AddExit(Direction.In, study, drawer);

// Game state
var state = new GameState(study, worldLocations: new[] { study });

// When the drawer is opened, reveal the photo
state.Events.Subscribe(GameEventType.OpenDoor, e =>
{
    if (e.Door != null && e.Door.Id == "drawer")
    {
        study.AddItem(photo);
        Console.WriteLine("Inside the drawer lies an old photo.");
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
        ["in"] = Direction.In
    },
    allowDirectionEnumNames: true);

var parser = new KeywordParser(parserConfig);
```
