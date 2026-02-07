# Job Interview

_Slice tag: Slice 23 — Structured dialog. Demo focuses on a formal interview flow._

## Story beats (max ~10 steps)
1) Wait in the lobby.
2) Meet the interviewer.
3) Answer questions.
4) Handle nerves.

## Example (dialog choices)
```csharp
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;

var lobby = new Location("interview_lobby", "A calm lobby with a glass of water.");
var room = new Location("interview_room", "A quiet interview room with two chairs.");

lobby.AddExit(Direction.North, room);

var interviewer = new Npc("interviewer", "interviewer")
    .Description("An interviewer with a kind smile.")
    .SetDialog(new DialogNode("Tell me about yourself.")
        .AddOption("Speak about teamwork")
        .AddOption("Speak about pressure"));

room.AddNpc(interviewer);

var state = new GameState(lobby, worldLocations: new[] { lobby, room });

state.RandomEvents
    .Enable()
    .SetTriggerChance(0.4)
    .AddEvent("lift_bell", 2, s => s.WorldState.AddTimeline("The lift bell rings somewhere above."),
        s => s.IsCurrentRoomId("interview_lobby"))
    .AddEvent("phone_buzz", 3, s => s.WorldState.AddTimeline("Your phone buzzes once, then stops."))
    .SetCooldown("phone_buzz", 3);

var parser = new KeywordParser(KeywordParserConfig.Default);
var lastTimelineCount = 0;

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
        if (g.State.WorldState.Timeline.Count > lastTimelineCount)
        {
            for (var i = lastTimelineCount; i < g.State.WorldState.Timeline.Count; i++)
            {
                g.Output.WriteLine(g.State.WorldState.Timeline[i]);
            }
            lastTimelineCount = g.State.WorldState.Timeline.Count;
        }
    })
    .Build();

// Console setup for C64 aesthetics
Console.BackgroundColor = ConsoleColor.DarkBlue;
Console.ForegroundColor = ConsoleColor.Cyan;
Console.Title = "Job Interview - Text Adventure Sandbox";
Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.Clear();
// End console setup
game.Run();
```
