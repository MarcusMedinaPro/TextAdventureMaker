# The Midnight Phone Call

_Slice tag: Slice 35 — Remote dialog + branching response._

## Story beats (max ~10 steps)
1) Phone rings at night.
2) Decide whether to answer.
3) Respond to the caller.
4) End the call.

## Map (rough layout)
```
          N
    W           E
          S

┌────────────┐
│  Bedroom   │
│  Phone, C  │
└────────────┘

Phone = Landline
C = Caller (NPC)
```

## Example (midnight call)
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

// Slice 35 — Dramatic Irony
// Tests:
// - Player learns a secret, NPC does not
// - Available actions derived from the knowledge gap

Location bedroom = (id: "bedroom", description: "A small bedroom lit by the glow of a phone screen.");

bedroom.AddItem(new Item("landline", "phone", "A phone buzzing with an incoming call.")
    .SetTakeable(false));

var caller = new Npc("caller", "caller")
    .Description("A familiar voice waits on the line.")
    .SetDialog(new DialogNode("Is now a good time to talk?")
        .AddOption("Answer calmly")
        .AddOption("Ask who this is")
        .AddOption("Hang up"));

bedroom.AddNpc(caller);

var state = new GameState(bedroom, worldLocations: new[] { bedroom });
var parser = new KeywordParser(KeywordParserConfig.Default);

SetupC64("The Midnight Phone Call - Text Adventure Sandbox");
WriteLineC64("=== THE MIDNIGHT PHONE CALL (Slice 35) ===");
WriteLineC64("Commands: answer, listen, warn, talk caller, look, quit.");
ShowRoom();

state.DramaticIrony.RegisterAction("caller_is_traitor", "warn");

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

    if (trimmed.TextCompare("answer") || trimmed.TextCompare("listen"))
    {
        state.DramaticIrony.PlayerLearn("caller_is_traitor");
        WriteLineC64("You catch the slip: the caller knows more than they should.");
        ShowAvailableActions();
        continue;
    }

    if (trimmed.TextCompare("warn"))
    {
        if (state.DramaticIrony.GetAvailableActions().Contains("warn"))
        {
            WriteLineC64("You warn the caller. The line goes quiet.");
            break;
        }

        WriteLineC64("You have nothing concrete to warn about.");
        continue;
    }

    var command = parser.Parse(trimmed);
    var result = state.Execute(command);
    if (!string.IsNullOrWhiteSpace(result.Message))
        WriteLineC64(result.Message);

    foreach (var reaction in result.ReactionsList.Where(r => !string.IsNullOrWhiteSpace(r)))
        WriteLineC64($"> {reaction}");
}

void ShowAvailableActions()
{
    var actions = state.DramaticIrony.GetAvailableActions();
    if (actions.Count == 0)
    {
        WriteLineC64("No special actions available.");
        return;
    }

    WriteLineC64($"New actions: {actions.CommaJoin()}");
}

void ShowRoom()
{
    WriteLineC64();
    WriteLineC64($"Room: {state.CurrentLocation.Id.ToProperCase()}");
    WriteLineC64(state.CurrentLocation.GetDescription());
}
```
