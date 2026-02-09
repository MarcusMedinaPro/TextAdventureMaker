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
using System;
using System.Linq;
using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;
using static MarcusMedina.TextAdventure.Extensions.ConsoleExtensions;

// Slice 34 — Agency Tracking
// Tests:
// - Registering choices with weights
// - Agency score affecting story outcome

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

SetupC64("The Power Outage - Text Adventure Sandbox");
WriteLineC64("=== THE POWER OUTAGE (Slice 34) ===");
WriteLineC64("Goal: choose how proactive you are and see the agency score shift.");
WriteLineC64("Commands: help, wait, look, go north/east, quit.");
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

    if (trimmed.TextCompare("help"))
    {
        state.Agency.Register("HelpTechnician", weight: 5);
        WriteLineC64("You take initiative and help reroute the power.");
        ReportAgency();
        continue;
    }

    if (trimmed.TextCompare("wait"))
    {
        state.Agency.Register("WaitedPassively", weight: -2);
        WriteLineC64("You wait for someone else to act.");
        ReportAgency();
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

void ReportAgency()
{
    WriteLineC64($"Agency score: {state.Agency.AgencyScore}");
    WriteLineC64(state.Agency.AgencyScore >= 3
        ? "You shape the outcome with decisive action."
        : "The story moves on, mostly without you.");
}

void ShowRoom()
{
    WriteLineC64();
    WriteLineC64($"Room: {state.CurrentLocation.Id.ToProperCase()}");
    WriteLineC64(state.CurrentLocation.GetDescription());
}
```
