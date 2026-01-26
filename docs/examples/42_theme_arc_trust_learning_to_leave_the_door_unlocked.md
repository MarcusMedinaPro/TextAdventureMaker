# Learning to Leave the Door Unlocked
_Slice tag: Slice 42 — Theme Arc Trust (Creepypasta style, British English)._


    ## Premise
    Each night you lock the door. Each morning you find it unlocked. At last you do not dare to lock it again.

    ## Arc structure
    - Isolation → You lock everything.
- Doubt → The locks fail anyway.
- Tested trust → You leave the door open.
- Resolution → You accept the risk.

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

var hallway = new Location("hallway", "A narrow hallway with a stubborn lock.");
var porch = new Location("porch", "A porch where the night waits politely.");

var key = new Key("front_key", "front key", "A key that has never been lost.")
    .SetHint("It still fits.");
var letter = new Item("letter", "letter", "A letter asking you to leave the door unlocked.")
    .SetReadText("Trust is a door you open twice.")
    .RequireTakeToRead();

hallway.AddItem(key);
hallway.AddItem(letter);

var frontDoor = new Door("front_door", "front door", "A front door with a chipped white frame.")
    .RequiresKey(key)
    .SetReaction(DoorAction.Open, "The lock clicks like a held breath.");

hallway.AddExit(Direction.Out, porch, frontDoor);

var neighbour = new Npc("neighbour", "neighbour")
    .Description("A neighbour who always remembers your name.")
    .SetDialog(new DialogNode("You can leave it tonight. I will watch.")
        .AddOption("Say yes")
        .AddOption("Say no"));

porch.AddNpc(neighbour);

var state = new GameState(hallway, worldLocations: new[] { hallway, porch });

state.Events.Subscribe(GameEventType.TalkToNpc, e =>
{
    if (e.Npc?.Id == "neighbour")
    {
        state.WorldState.Increment("trust");
    }
});

state.Events.Subscribe(GameEventType.UnlockDoor, e =>
{
    if (e.Door?.Id == "front_door")
    {
        state.WorldState.Increment("trust");
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
        if (g.State.WorldState.GetCounter("trust") >= 1 && g.State.IsCurrentRoomId("porch"))
        {
            g.Output.WriteLine("You leave the door unlatched and try to believe it will stay that way.");
            g.RequestStop();
        }
    })
    .Build();

game.Run();
```
