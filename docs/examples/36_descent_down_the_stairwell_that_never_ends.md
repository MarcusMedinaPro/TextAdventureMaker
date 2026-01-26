# Down the Stairwell That Never Ends
_Slice tag: Slice 36 — Descent (Creepypasta style, British English)._


    ## Premise
    The stairwell beneath the house had only three floors. Now you have passed twelve. Your phone shows no signal, yet you still hear footsteps above you.

    ## Arc structure
    - Descent → Each floor deepens the unease.
- Loss of control → Signal dies, exits vanish.
- Confrontation → Something waits below.
- Return changed → You never see stairwells the same.

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

var landing = new Location("landing", "A landing with a stair that refuses to end.");
var stair = new Location("stair", "Each step is colder than the last.");
var lower = new Location("lower", "A lower floor where the air is wet and old.");

landing.AddExit(Direction.Down, stair);
stair.AddExit(Direction.Down, lower);

var matchbook = new Item("matchbook", "matchbook", "A matchbook with one dry match.")
    .SetReaction(ItemAction.Use, "The match flares, and the dark takes a step back.");
var note = new Item("note", "warning note", "A note that is too damp to read.")
    .SetReadable()
    .RequiresToRead(state => state.WorldState.GetFlag("lit"));

landing.AddItem(matchbook);
stair.AddItem(note);

var watcher = new Npc("watcher", "watcher", NpcState.Hostile)
    .Description("Something that moves just beyond the edge of the light.");

lower.AddNpc(watcher);

var state = new GameState(landing, worldLocations: new[] { landing, stair, lower });

matchbook.OnUse += _ => state.WorldState.SetFlag("lit", true);

state.Events.Subscribe(GameEventType.EnterLocation, e =>
{
    if (e.Location?.Id == "stair" || e.Location?.Id == "lower")
    {
        state.WorldState.Increment("descent");
    }
});

var quest = new Quest("descent", "Reach the Lower Floor", "Descend far enough to hear the truth.")
    .AddCondition(new WorldCounterCondition("descent", 2))
    .Start();

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
        if (quest.CheckProgress(g.State) && g.State.IsCurrentRoomId("lower"))
        {
            g.Output.WriteLine("You reach the bottom, and it looks back.");
            g.RequestStop();
        }
    })
    .Build();

game.Run();
```
