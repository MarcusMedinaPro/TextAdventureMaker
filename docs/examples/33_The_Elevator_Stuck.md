# The Elevator Stuck

_Slice tag: Slice 33 — Limited space, stress, and communication._

## Story beats (max ~10 steps)
1) Enter the elevator.
2) It stops between floors.
3) Use the intercom.
4) Wait for help.

## Map (rough layout)
```
          N
    W           E
          S

┌────────────┐
│ Elevator   │─────┐
│  Btn, Op   │  E  │
└─────┬──────┘     │
      │            │
      │            │
┌────────────┐  ┌────────────┐
│   Lobby    │  │Maintenance │
└────────────┘  └────────────┘

Btn = Emergency button
Op = Operator (NPC)
```

## Example (stuck elevator)
```csharp
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;

Location lobby = (id: "apartment_lobby", description: "A tidy lobby with a directory and fresh paint.");
Location elevator = (id: "elevator", description: "A cramped elevator with a flickering display.");
Location maintenance = (id: "maintenance", description: "A maintenance corridor lined with panels.");

lobby.AddExit(Direction.Up, elevator);
elevator.AddExit(Direction.East, maintenance);

elevator.AddItem(new Item("button", "emergency button", "A red button under a plastic cover.")
    .SetTakeable(false));

var operatorVoice = new Npc("operator", "operator")
    .Description("A calm voice crackles over the intercom.")
    .SetDialog(new DialogNode("We're aware of the issue. Stay calm.")
        .AddOption("Ask how long it will take")
        .AddOption("Describe your situation"));

elevator.AddNpc(operatorVoice);

var state = new GameState(lobby, worldLocations: new[] { lobby, elevator, maintenance });
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
