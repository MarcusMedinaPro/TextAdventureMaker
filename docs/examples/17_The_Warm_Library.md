# The Warm Library

_Slice tag: Slice 17 — Calm progression. Demo focuses on a locked door and a cozy payoff._

## Story beats (max ~10 steps)
1) You stand outside a locked library.
2) Find the key in the courtyard.
3) Unlock the door.
4) Step into the warmth.

## Map (rough layout)
```
          N
    W           E
          S

┌────────────┐     ┌────────────┐
│  Outside   │─────│  Library   │
│     K      │  In │            │
└────────────┘     └────────────┘

K = Library key
```

## Example (fluent, minimal loop)
```csharp
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;

Location outside = (id: "outside", description: "Snow falls quietly outside a locked library.");
Location library = (id: "library", description: "Warm light and quiet pages surround you.");

var key = new Key("library_key", "library key", "Cold metal in your hand.");
outside.AddItem(key);

var door = new Door("library_door", "library door", "A heavy wooden door.")
    .RequiresKey(key)
    .SetReaction(DoorAction.Unlock, "The library door unlocks.");

outside.AddExit(Direction.In, library, door);

var state = new GameState(outside, worldLocations: new[] { outside, library });
var parser = new KeywordParser(KeywordParserConfig.Default);

var game = GameBuilder.Create()
    .UseState(state)
    .UseParser(parser)
    .AddTurnStart(g =>
    {
        var look = g.State.Look();
        g.Output.WriteLine($"\n{look.Message}");
    })
    .Build();

// Console setup for C64 aesthetics
Console.BackgroundColor = ConsoleColor.DarkBlue;
Console.ForegroundColor = ConsoleColor.Cyan;
Console.Title = "The Warm Library - Text Adventure Sandbox";
Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.Clear();
// End console setup
game.Run();
```
