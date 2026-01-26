# The Bank Errand

_Slice tag: Slice 21 — Transaction flow. Demo focuses on a small stateful interaction with a teller._

## Story beats (max ~10 steps)
1) You enter the bank.
2) Take a number.
3) Wait for your turn.
4) Resolve a simple issue.

## Example (dialog + state checks)
```csharp
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;

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

state.Events.Subscribe(GameEventType.PickupItem, e =>
{
    if (e.Item?.Id == "ticket")
    {
        state.WorldState.SetFlag("has_ticket", true);
    }
});

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
        if (g.State.WorldState.GetFlag("closing_warning"))
        {
            g.Output.WriteLine("The guard taps his watch. Closing soon.");
            g.State.WorldState.SetFlag("closing_warning", false);
        }

        if (g.State.WorldState.GetFlag("time_up"))
        {
            g.Output.WriteLine("The bank closes. You will have to return tomorrow.");
            g.RequestStop();
        }
    })
    .Build();

game.Run();
```
