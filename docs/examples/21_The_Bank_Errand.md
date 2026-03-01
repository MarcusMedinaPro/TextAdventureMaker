# The Bank Errand

_Slice tag: Slice 21 — Transaction flow. Demo focuses on a small stateful interaction with a teller._

## Story beats (max ~10 steps)
1) You enter the bank.
2) Take a number.
3) Wait for your turn.
4) Resolve a simple issue.

## Map (rough layout)
```
          N
    W           E
          S

┌────────────┐
│  Counter   │
│  Teller    │
└─────┬──────┘
      │
      │
┌────────────┐
│   Lobby    │
│     T      │
└────────────┘

T = Ticket
Teller = NPC
```

## Example (dialog + state checks)
```csharp
using System.Linq;
using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;
using static MarcusMedina.TextAdventure.Extensions.ConsoleExtensions;

var lobby = new Location("bank_lobby", "A quiet bank lobby with a ticket machine.");
var counter = new Location("bank_counter", "A teller waits behind the counter.");

var counterDoor = new Door("counter_door", "counter door", "A half-door with a brass latch.", DoorState.Open);
lobby.AddExit(Direction.North, counter, counterDoor);

var ticket = new Item("ticket", "number ticket", "Your place in line.")
    .SetHint("Take it to be called.");
lobby.AddItem(ticket);

var teller = new Npc("teller", "teller")
    .Description("A patient teller taps the desk.")
    .SetDialog(new DialogNode("How can I help you today?")
        .AddOption("Fix my account issue")
        .AddOption("Ask about fees"));

counter.AddNpc(teller);

var state = new GameState(lobby, worldLocations: new[] { lobby, counter });

var time = new TimeSystem()
    .Enable()
    .SetStartTime(TimeOfDay.Day)
    .SetTicksPerDay(8)
    .SetMaxMoves(12)
    .OnPhase(TimeOfDay.Night, s =>
    {
        counterDoor.Close();
        s.WorldState.SetFlag("bank_closed", true);
    })
    .OnPhase(TimeOfDay.Day, s =>
    {
        counterDoor.Open();
        s.WorldState.SetFlag("bank_closed", false);
    })
    .OnMovesRemaining(3, s => s.WorldState.SetFlag("closing_warning", true))
    .OnMovesExhausted(s => s.WorldState.SetFlag("time_up", true));

state.SetTimeSystem(time);

ITimedChallenge timedChallenge = time.CreateTimedChallenge("resolve_issue")
    .MaxMovesLimit(5)
    .OnStart(_ => WriteLineC64("The teller leans in. You have 5 moves to resolve the issue."))
    .OnMovesRemaining(2, _ => WriteLineC64("Only 2 moves left to resolve the issue."))
    .OnSuccess(_ => WriteLineC64("The teller nods. Your issue is resolved."))
    .OnFailure(_ => WriteLineC64("Time runs out. The teller motions you to return tomorrow."));

state.Events.Subscribe(GameEventType.PickupItem, e =>
{
    if (e.Item?.Id == "ticket")
    {
        state.WorldState.SetFlag("has_ticket", true);
    }
});

var parser = new KeywordParser(KeywordParserConfig.Default);

SetupC64("The Bank Errand - Text Adventure Sandbox");
WriteLineC64("=== THE BANK ERRAND (Slice 21) ===");
WriteLineC64("Goal: take a ticket, reach the counter, then resolve the issue in time.");
WriteLineC64("Commands: look, take ticket, go north, resolve, quit.");
ShowRoom();

while (true)
{
    WriteLineC64();
    WritePromptC64("> ");
    string? input = Console.ReadLine();
    if (input is null)
        break;

    string trimmed = input.Trim();
    if (string.IsNullOrWhiteSpace(trimmed))
        continue;

    if (trimmed.Is("quit") || trimmed.Is("exit"))
        break;

    if (trimmed.TextCompare("resolve"))
    {
        if (timedChallenge.IsActive)
            timedChallenge.Succeed(state);
        else
            WriteLineC64("The teller isn't ready to hear you yet.");

        if (!timedChallenge.IsActive)
            break;

        continue;
    }

    ICommand command = parser.Parse(trimmed);
    CommandResult result = state.Execute(command);
    if (!string.IsNullOrWhiteSpace(result.Message))
        WriteLineC64(result.Message);

    foreach (string reaction in result.ReactionsList.Where(r => !string.IsNullOrWhiteSpace(r)))
        WriteLineC64($"> {reaction}");

    if (command is GoCommand && result.Success)
        ShowRoom();

    if (state.IsCurrentRoomId("bank_counter") && !timedChallenge.IsActive)
    {
        timedChallenge.Start(state);
    }

    if (state.WorldState.GetFlag("closing_warning"))
    {
        WriteLineC64("The guard taps his watch. Closing soon.");
        state.WorldState.SetFlag("closing_warning", false);
    }

    if (state.WorldState.GetFlag("time_up"))
    {
        WriteLineC64("The bank closes. You will have to return tomorrow.");
        break;
    }
}

void ShowRoom()
{
    WriteLineC64();
    WriteLineC64($"Room: {state.CurrentLocation.Id.ToProperCase()}");
    WriteLineC64(state.CurrentLocation.GetDescription());
    List<string> exits = state.CurrentLocation.Exits.Keys
        .Select(dir => dir.ToString().ToLowerInvariant().ToProperCase())
        .ToList();
    WriteLineC64(exits.Count > 0 ? $"Exits: {exits.CommaJoin()}" : "Exits: None");
}
```
