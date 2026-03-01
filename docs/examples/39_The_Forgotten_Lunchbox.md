# The Forgotten Lunchbox

_Slice tag: Slice 39 — Memory loop + retrieval._

## Story beats (max ~10 steps)
1) Return to the school.
2) Find the cafeteria.
3) Spot the forgotten lunchbox.
4) Decide what to do with it.

## Map (rough layout)

```
          N
    W           E
          S

┌────────────┐
│ Cafeteria  │
│   L  J     │
└────────────┘

J = Janitor
L = Lunchbox
```

## Example (lunchbox)
```csharp
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;

Location cafeteria = (id: "cafeteria", description: "A quiet cafeteria with stacked chairs.");

cafeteria.AddItem(new Item("lunchbox", "lunchbox", "A forgotten lunchbox with a sticker."));

var janitor = new Npc("janitor", "janitor")
    .Description("A janitor folds up a caution sign.")
    .SetDialog(new DialogNode("Looking for something?")
        .AddOption("Ask about the lunchbox")
        .AddOption("Ask about the hallway"));

cafeteria.AddNpc(janitor);

var state = new GameState(cafeteria, worldLocations: new[] { cafeteria });
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
