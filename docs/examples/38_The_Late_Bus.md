# The Late Bus

_Slice tag: Slice 38 — Waiting loop + alternative decision._

## Story beats (max ~10 steps)
1) Arrive at the bus stop.
2) See the delay notice.
3) Decide to wait or leave.
4) Choose another path.

## Example (late bus)
```csharp
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;

Location lateBusStop = (id: "late_bus_stop", description: "A bus stop with a flickering arrival display.");

lateBusStop.AddItem(new Item("display", "arrival display", "SERVICE DELAYED flashes in amber.")
    .SetTakeable(false));

var commuter = new Npc("commuter", "commuter")
    .Description("A commuter checks the arrival screen.")
    .SetDialog(new DialogNode("They're running late again.")
        .AddOption("Wait it out")
        .AddOption("Look for another option"));

lateBusStop.AddNpc(commuter);

var state = new GameState(lateBusStop, worldLocations: new[] { lateBusStop });
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
