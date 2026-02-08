# The Power Outage

_Slice tag: Slice 34 — Light/dark gating + sequential fixes._

## Story beats (max ~10 steps)
1) Step into a dark hallway.
2) Find a flashlight.
3) Reach the utility room.
4) Flip the breaker.

## Map (rough layout)
```
          N
    W           E
          S

┌────────────┐
│ Dark Hall  │─────┐
│    F       │  E  │
└─────┬──────┘     │
      │            │
      │            │
┌────────────┐  ┌────────────┐
│Maintenance │  │ UtilityRm  │
│            │  │  Br, Tech  │
└────────────┘  └────────────┘

F = Flashlight
Br = Breaker box
Tech = Technician (NPC)
```

## Example (power restore)
```csharp
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;

Location maintenance = (id: "maintenance", description: "A maintenance corridor lined with panels.");
Location darkHall = (id: "dark_hall", description: "A hallway swallowed by the power outage.");
Location utility = (id: "utility_room", description: "A utility room with humming equipment.");

maintenance.AddExit(Direction.North, darkHall);
darkHall.AddExit(Direction.East, utility);

darkHall.AddItem(new Item("flashlight", "flashlight", "A heavy flashlight with weak batteries."));
utility.AddItem(new Item("breaker", "breaker box", "A row of breakers labeled by zone.")
    .SetTakeable(false));

var technician = new Npc("technician", "technician")
    .Description("A technician flips through a clipboard.")
    .SetDialog(new DialogNode("We can route power back to this wing.")
        .AddOption("Ask which breaker to flip")
        .AddOption("Offer to help"));

utility.AddNpc(technician);

var state = new GameState(maintenance, worldLocations: new[] { maintenance, darkHall, utility });
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
