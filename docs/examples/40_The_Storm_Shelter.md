# The Storm Shelter

_Slice tag: Slice 40 — Shelter, coordination, and calm under pressure._

## Story beats (max ~10 steps)
1) Reach the shelter entrance.
2) Enter the shelter hall.
3) Listen to the radio.
4) Decide how to help.

## Map (rough layout)

```
          N
    W           E
          S

┌────────────┐
│ Entrance   │
│    R       │
└─────┬──────┘
      │
┌────────────┐
│    Hall    │
│   B  C     │
└────────────┘

B = Blanket
C = Coordinator
R = Radio
```

## Example (storm shelter)
```csharp
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;

Location entrance = (id: "shelter_entrance", description: "A storm shelter entrance with sandbags piled high.");
Location hall = (id: "shelter_hall", description: "A narrow hall lined with cots and blankets.");

entrance.AddExit(Direction.In, hall);

entrance.AddItem(new Item("radio", "radio", "A weather radio crackles with updates.")
    .SetTakeable(false));
hall.AddItem(new Item("blanket", "blanket", "A folded blanket on a cot."));

var coordinator = new Npc("coordinator", "coordinator")
    .Description("A shelter coordinator checks names off a list.")
    .SetDialog(new DialogNode("Find a cot and stay warm.")
        .AddOption("Ask about the storm")
        .AddOption("Offer to help"));

hall.AddNpc(coordinator);

var state = new GameState(entrance, worldLocations: new[] { entrance, hall });
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
