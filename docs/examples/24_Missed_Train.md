# Missed Train

_Slice tag: Slice 24 — Path choice + alternate routes. Demo focuses on choosing a new way home after missing the train._

## Story beats (max ~10 steps)
1) Arrive at the station too late.
2) Check the departure board.
3) Choose an alternate route.
4) Leave via bus or footbridge.

## Example (route choice)
```csharp
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;

Location cafe = (id: "cafe", description: "A warm cafe near the station.");
Location stationHall = (id: "station_hall", description: "A quiet station hall with a flickering board.");
Location platform = (id: "platform", description: "The last train's lights disappear.");
Location sideStreet = (id: "side_street", description: "A side street with rain-slick pavement.");
Location busStop = (id: "bus_stop", description: "A lonely bus stop with a torn schedule.");
Location footbridge = (id: "footbridge", description: "A narrow footbridge over the tracks.");

cafe.AddExit(Direction.South, stationHall);
stationHall.AddExit(Direction.East, platform);
stationHall.AddExit(Direction.West, sideStreet);
sideStreet.AddExit(Direction.South, busStop);
platform.AddExit(Direction.North, footbridge);
footbridge.AddExit(Direction.East, busStop); // alternate path back around

stationHall.AddItem(new Item("board", "departure board", "The next train is marked CANCELLED.")
    .SetTakeable(false));

var state = new GameState(cafe, worldLocations: new[] { cafe, stationHall, platform, sideStreet, busStop, footbridge });
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
