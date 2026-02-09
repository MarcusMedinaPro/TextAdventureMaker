# The Night Walk Home

_Slice tag: Slice 29 — Tension + risk assessment beats._

## Story beats (max ~10 steps)
1) Leave the bar.
2) Walk through a darker street.
3) Decide how to handle the tension.
4) Reach home.

## Map (rough layout)
```
          N
    W           E
          S

                    ┌────────────┐
                    │ FrontPorch │
                    └─────┬──────┘
                          │
                          │
┌────────────┐     ┌────────────┐     ┌────────────┐
│  Bar Alley │─────│ NightStreet│─────│ Underpass  │
│            │  E  │  L, Str    │  E  │     W      │
└────────────┘     └────────────┘     └────────────┘

L = Streetlight
W = Whistle
Str = Stranger (NPC)
```

## Example (night walk)
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

// Slice 29 — Tension Meter
// Tests:
// - Tension meter adjustments
// - Simple safe zone lowers tension

Location barAlley = (id: "bar_alley", description: "A narrow alley behind the bar.");
Location nightStreet = (id: "night_street", description: "A long street with patchy streetlights.");
Location underpass = (id: "underpass", description: "A shadowy underpass humming with distant traffic.");
Location frontPorch = (id: "front_porch", description: "A quiet front porch with a locked gate.");

barAlley.AddExit(Direction.East, nightStreet);
nightStreet.AddExit(Direction.East, underpass);
underpass.AddExit(Direction.North, frontPorch);

nightStreet.AddItem(new Item("streetlight", "streetlight", "A flickering streetlight buzzes overhead.")
    .SetTakeable(false));
underpass.AddItem(new Item("whistle", "whistle", "A small whistle on a frayed cord."));

var stranger = new Npc("stranger", "stranger")
    .SetState(NpcState.Hostile)
    .SetStats(new Stats(8))
    .Description("A stranger lingers in the shadows.")
    .SetDialog(new DialogNode("You lost?")
        .AddOption("Keep walking")
        .AddOption("Ask for directions"));

nightStreet.AddNpc(stranger);

var state = new GameState(barAlley, worldLocations: new[] { barAlley, nightStreet, underpass, frontPorch });
var parser = new KeywordParser(KeywordParserConfig.Default);
var tension = new TensionMeter().Set(0.2f);

SetupC64("The Night Walk Home - Text Adventure Sandbox");
WriteLineC64("=== THE NIGHT WALK HOME (Slice 29) ===");
WriteLineC64("Goal: reach the front porch while managing rising tension.");
WriteLineC64("Commands: look, go east, go north, take whistle, quit.");
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

    var command = parser.Parse(trimmed);
    var result = state.Execute(command);

    if (!string.IsNullOrWhiteSpace(result.Message))
        WriteLineC64(result.Message);

    foreach (var reaction in result.ReactionsList.Where(r => !string.IsNullOrWhiteSpace(r)))
        WriteLineC64($"> {reaction}");

    if (command is GoCommand && result.Success)
    {
        AdjustTension();
        ShowRoom();
    }
}

void AdjustTension()
{
    if (state.IsCurrentRoomId("underpass"))
    {
        _ = tension.Modify(0.2f);
        return;
    }

    if (state.IsCurrentRoomId("front_porch"))
    {
        _ = tension.Modify(-0.3f);
        return;
    }

    _ = tension.Modify(0.05f);
}

void ShowRoom()
{
    WriteLineC64();
    WriteLineC64($"Room: {state.CurrentLocation.Id.ToProperCase()}");
    WriteLineC64(state.CurrentLocation.GetDescription());
    WriteLineC64($"Tension: {tension.Current:0.00}");
    var exits = state.CurrentLocation.Exits.Keys
        .Select(dir => dir.ToString().ToLowerInvariant().ToProperCase())
        .ToList();
    WriteLineC64(exits.Count > 0 ? $"Exits: {exits.CommaJoin()}" : "Exits: None");
}
```
