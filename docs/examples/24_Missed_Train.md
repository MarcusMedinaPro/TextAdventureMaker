# Missed Train

_Slice tag: Slice 24 — Path choice + alternate routes. Demo focuses on choosing a new way home after missing the train._

## Story beats (max ~10 steps)
1) Arrive at the station too late.
2) Check the departure board.
3) Choose an alternate route.
4) Leave via bus or footbridge.

## Example (route choice)
```csharp
using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;

var hall = new Location("station_hall", "A quiet station hall with a flickering departure board.");
var platform = new Location("platform", "The last train's lights disappear into the night.");
var sideStreet = new Location("side_street", "A side street with rain-slick pavement.");
var footbridge = new Location("footbridge", "A narrow footbridge over the tracks.");

hall.AddExit(Direction.North, platform);
hall.AddExit(Direction.East, sideStreet);
sideStreet.AddExit(Direction.North, footbridge);

var map = new Item("map", "folded map", "A map of nearby streets with pencil marks.")
    .SetReadText("The footbridge leads to a shortcut.")
    .RequireTakeToRead();

hall.AddItem(map);

var state = new GameState(hall, worldLocations: new[] { hall, platform, sideStreet, footbridge });
var parser = new KeywordParser(KeywordParserConfig.Default);

var game = GameBuilder.Create()
    .UseState(state)
    .UseParser(parser)
    .AddTurnStart(g =>
    {
        var look = g.State.Look();
        g.Output.WriteLine($"
{look.Message}");
    })
    .AddTurnEnd((g, command, result) =>
    {
        if (command is LookCommand && g.State.Inventory.FindItem("map") != null)
        {
            var discovered = string.Join(", ", g.State.LocationDiscovery.DiscoveredLocations.OrderBy(x => x));
            g.Output.WriteLine($"Map notes: {discovered}");
        }
    })
    .Build();

game.Run();
```
