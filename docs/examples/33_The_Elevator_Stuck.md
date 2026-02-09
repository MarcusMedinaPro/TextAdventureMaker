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
using System;
using System.Linq;
using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;
using static MarcusMedina.TextAdventure.Extensions.ConsoleExtensions;

// Slice 33 — Narrative Voice
// Tests:
// - Voice and tense transforms
// - Switching narrative voice during play

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

SetupC64("The Elevator Stuck - Text Adventure Sandbox");
WriteLineC64("=== THE ELEVATOR STUCK (Slice 33) ===");
WriteLineC64("Commands: look, voice first/second/third, tense past/present, quit.");
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

    if (TrySetVoice(trimmed) || TrySetTense(trimmed))
    {
        ShowRoom();
        continue;
    }

    var command = parser.Parse(trimmed);
    var result = state.Execute(command);
    if (!string.IsNullOrWhiteSpace(result.Message))
        WriteLineC64(state.NarrativeVoice.Transform(result.Message));

    foreach (var reaction in result.ReactionsList.Where(r => !string.IsNullOrWhiteSpace(r)))
        WriteLineC64($"> {state.NarrativeVoice.Transform(reaction)}");

    if (command is GoCommand && result.Success)
        ShowRoom();
}

bool TrySetVoice(string input)
{
    if (input.TextCompare("voice first"))
        state.NarrativeVoice.SetVoice(Voice.FirstPerson);
    else if (input.TextCompare("voice second"))
        state.NarrativeVoice.SetVoice(Voice.SecondPerson);
    else if (input.TextCompare("voice third"))
        state.NarrativeVoice.SetVoice(Voice.ThirdPerson).Subject("The traveler");
    else
        return false;

    return true;
}

bool TrySetTense(string input)
{
    if (input.TextCompare("tense past"))
        state.NarrativeVoice.SetTense(Tense.Past);
    else if (input.TextCompare("tense present"))
        state.NarrativeVoice.SetTense(Tense.Present);
    else
        return false;

    return true;
}

void ShowRoom()
{
    WriteLineC64();
    WriteLineC64(state.NarrativeVoice.Transform($"Room: {state.CurrentLocation.Id.ToProperCase()}"));
    WriteLineC64(state.NarrativeVoice.Transform(state.CurrentLocation.GetDescription()));
}
```
