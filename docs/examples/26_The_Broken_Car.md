# The Broken Car

_Slice tag: Slice 26 — Chain of dependencies (tools, phone, fix)._

## Story beats (max ~10 steps)
1) Car dies on the roadside.
2) Find a way to call for help.
3) Get the right tools.
4) Attempt the fix.

## Map (rough layout)
```
          N
    W           E
          S

┌────────────┐     ┌────────────┐
│ Roadside   │─────│ Gas Station│
│    P       │  E  │  W, J, Mech│
└────────────┘     └────────────┘

P = Phone
W = Wrench
J = Jack
Mech = Mechanic (NPC)
```

## Example (tool chain)
```csharp
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;

Location roadside = (id: "roadside", description: "Your car sits dead on the shoulder with the hood up.");
Location gasStation = (id: "gas_station", description: "A small gas station with a service bay.");

roadside.AddExit(Direction.East, gasStation);

roadside.AddItem(new Item("phone", "phone", "Low battery, but still works."));
var wrench = new Item("wrench", "wrench", "A sturdy wrench for a stubborn bolt.");
var jack = new Item("jack", "jack", "A heavy jack for lifting the car.");

gasStation.AddItem(wrench);
gasStation.AddItem(jack);

var mechanic = new Npc("mechanic", "mechanic")
    .Description("A mechanic wipes grease from their hands.")
    .SetDialog(new DialogNode("Need a tool or a lift?")
        .AddOption("Ask for a wrench")
        .AddOption("Ask for a jack"));

gasStation.AddNpc(mechanic);

var state = new GameState(roadside, worldLocations: new[] { roadside, gasStation });
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
