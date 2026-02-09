# The Late Bus

_Slice tag: Slice 38 — Waiting loop + alternative decision._

## Story beats (max ~10 steps)
1) Arrive at the bus stop.
2) See the delay notice.
3) Decide to wait or leave.
4) Choose another path.

## Map (rough layout)

```
          N
    W           E
          S

┌────────────┐
│  Bus Stop  │
│   D  C     │
└────────────┘

C = Commuter
D = Arrival display
```

## Example (late bus)
```csharp
using System;
using System.Linq;
using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;
using static MarcusMedina.TextAdventure.Extensions.ConsoleExtensions;

// Slice 38 — Timed Spawns + Doors
// Tests:
// - TimedSpawn appearance
// - TimedDoor open/close based on ticks

Location lateBusStop = (id: "late_bus_stop", description: "A bus stop with a flickering arrival display.");
Location busLane = (id: "bus_lane", description: "The bus lane glows under the streetlights.");

lateBusStop.AddItem(new Item("display", "arrival display", "SERVICE DELAYED flashes in amber.")
    .SetTakeable(false));

var commuter = new Npc("commuter", "commuter")
    .Description("A commuter checks the arrival screen.")
    .SetDialog(new DialogNode("They're running late again.")
        .AddOption("Wait it out")
        .AddOption("Look for another option"));

lateBusStop.AddNpc(commuter);

Door busDoor = new("bus_door", "bus doors", "The bus doors hiss as they open.");
lateBusStop.AddExit(Direction.North, busLane, busDoor);

TimedSpawn busSpawn = lateBusStop.AddTimedSpawn("bus")
    .AppearsAt(3)
    .Message("Headlights cut through the rain. The bus arrives.");

TimedDoor timedDoor = new("bus_door")
    .OpensAt(3)
    .ClosesAt(5)
    .Message("The bus doors open.")
    .ClosedMessage("The bus pulls away.");

var state = new GameState(lateBusStop, worldLocations: new[] { lateBusStop, busLane });
var parser = new KeywordParser(KeywordParserConfig.Default);
var time = new TimeSystem().Enable().SetTicksPerDay(8);
state.SetTimeSystem(time);

SetupC64("The Late Bus - Text Adventure Sandbox");
WriteLineC64("=== THE LATE BUS (Slice 38) ===");
WriteLineC64("Goal: wait for the bus to arrive, then go north.");
WriteLineC64("Commands: wait, look, go north, quit.");
ShowRoom();

while (true)
{
    WriteLineC64();
    WritePromptC64("> ");
    var input = Console.ReadLine();
    if (input is null)
        break;

    var trimmed = input.Trim();
    if (string.IsNullOrWhiteSpace(trimmed))
        continue;

    if (trimmed.Is("quit") || trimmed.Is("exit"))
        break;

    if (trimmed.TextCompare("wait"))
    {
        TickTimedObjects();
        WriteLineC64("You wait...");
        continue;
    }

    var command = parser.Parse(trimmed);
    var result = state.Execute(command);
    if (!string.IsNullOrWhiteSpace(result.Message))
        WriteLineC64(result.Message);

    foreach (var reaction in result.ReactionsList.Where(r => !string.IsNullOrWhiteSpace(r)))
        WriteLineC64($"> {reaction}");

    if (command is GoCommand && result.Success)
        ShowRoom();
}

void TickTimedObjects()
{
    state.TimeSystem.Tick(state);

    if (busSpawn.AppearTicks.Contains(state.TimeSystem.CurrentTick) && lateBusStop.FindItem("bus") == null)
    {
        lateBusStop.AddItem(new Item("bus", "bus", "The bus waits with doors open."));
        if (!string.IsNullOrWhiteSpace(busSpawn.MessageText))
            WriteLineC64(busSpawn.MessageText);
    }

    if (timedDoor.OpenTicks.Contains(state.TimeSystem.CurrentTick))
    {
        _ = busDoor.Open();
        if (!string.IsNullOrWhiteSpace(timedDoor.MessageText))
            WriteLineC64(timedDoor.MessageText);
    }

    if (timedDoor.CloseTicks.Contains(state.TimeSystem.CurrentTick))
    {
        _ = busDoor.Close();
        if (!string.IsNullOrWhiteSpace(timedDoor.ClosedMessageText))
            WriteLineC64(timedDoor.ClosedMessageText);
    }
}

void ShowRoom()
{
    WriteLineC64();
    WriteLineC64($"Room: {state.CurrentLocation.Id.ToProperCase()}");
    WriteLineC64(state.CurrentLocation.GetDescription());
    var exits = state.CurrentLocation.Exits.Keys
        .Select(dir => dir.ToString().ToLowerInvariant().ToProperCase())
        .ToList();
    WriteLineC64(exits.Count > 0 ? $"Exits: {exits.CommaJoin()}" : "Exits: None");
}
```
