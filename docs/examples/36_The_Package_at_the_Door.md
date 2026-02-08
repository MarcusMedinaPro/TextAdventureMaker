# The Package at the Door

_Slice tag: Slice 36 — Trust/choice with consequence._

## Story beats (max ~10 steps)
1) Hear a knock.
2) Find a package on the doorstep.
3) Decide whether to accept it.
4) Ask a neighbour for context.

## Map (rough layout)
```
          N
    W           E
          S

┌────────────┐
│ Doorstep   │
│ Pkg, N     │
└────────────┘

Pkg = Package
N = Neighbour (NPC)
```

## Example (package choice)
```csharp
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;

Location doorstep = (id: "doorstep", description: "A doorstep with a damp welcome mat.");

doorstep.AddItem(new Item("package", "package", "A small package with a smudged label."));

var neighbour = new Npc("neighbour", "neighbour")
    .Description("A neighbour peers from a cracked door.")
    .SetDialog(new DialogNode("Was this package left for you?")
        .AddOption("Accept the package")
        .AddOption("Ask who dropped it off"));

doorstep.AddNpc(neighbour);

var state = new GameState(doorstep, worldLocations: new[] { doorstep });
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

game.Run();
```
