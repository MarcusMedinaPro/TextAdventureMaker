# What They Saw in Me
_Slice tag: Slice 36 — Framed Narrative (Me-Them-Me) (Creepypasta style, British English)._


    ## Premise
    You remember who you are. Then you hear what they say about you through the wall. When they stop talking, you no longer know which memory is yours.

    ## Arc structure
    - Me → You know yourself.
- Them → Their story rewrites you.
- Me again → You choose which story to keep.

    ## Story beats (max ~8 steps)
1) The disturbance arrives and feels personal.
2) A rule is broken or a boundary is crossed.
3) A clue reveals the scale of the problem.
4) A choice narrows the world.
5) The environment answers back.
6) A truth is forced into view.
7) A price is paid, willingly or not.
8) The ending leaves a lingering echo.

    ## Example (detailed setup)

```csharp
using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;

var studio = new Location("studio", "A studio with two chairs and one mirror.");
var gallery = new Location("gallery", "A gallery filled with portraits that avoid your eyes.");
var studioAgain = new Location("studio_again", "The studio, now with a third chair.");

studio.AddExit(Direction.East, gallery);
gallery.AddExit(Direction.West, studioAgain, oneWay: true);

var critic = new Npc("critic", "critic")
    .Description("A critic who never looks at the work.")
    .SetDialog(new DialogNode("Tell me what you see.")
        .AddOption("Describe yourself")
        .AddOption("Ask what they see"));

gallery.AddNpc(critic);

var state = new GameState(studio, worldLocations: new[] { studio, gallery, studioAgain });

state.Events.Subscribe(GameEventType.TalkToNpc, e =>
{
    if (e.Npc?.Id == "critic")
    {
        state.WorldState.Increment("perspective");
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
        if (g.State.WorldState.GetCounter("perspective") >= 1 && g.State.IsCurrentRoomId("studio_again"))
        {
            g.Output.WriteLine("You return to yourself with their eyes still on you.");
            g.RequestStop();
        }
    })
    .Build();

game.Run();
```
