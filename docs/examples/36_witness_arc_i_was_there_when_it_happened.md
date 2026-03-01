# I Was There When It Happened
_Slice tag: Slice 36 — Witness Arc (Creepypasta style, British English)._


    ## Premise
    You saw the accident. No one else remembers it. But each night you hear the crash again, at the exact same second.

    ## Arc structure
    - Observe → You watch the crash.
- Collect → Clues repeat each night.
- Assemble → You piece together the truth.
- Change → Knowing becomes your burden.

    ## Story beats (max ~8 steps)
1) The disturbance arrives and feels personal.
2) A rule is broken or a boundary is crossed.
3) A clue reveals the scale of the problem.
4) A choice narrows the world.
5) The environment answers back.
6) A truth is forced into view.
7) A price is paid, willingly or not.
8) The ending leaves a lingering echo.

    ## Map (rough layout)

```
          N
    W           E
          S

┌────────────┐
│  Archive   │
│            │
└─────┬──────┘
      │
┌────────────┐      ┌────────────┐
│ Side Street│──────│   Square   │
│     W      │      │     C      │
└────────────┘      └────────────┘

C = Camera
W = Witness
```

    ## Example (detailed setup)

```csharp
using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;

var square = new Location("square", "A square that remembers footsteps better than people.");
var sideStreet = new Location("side_street", "A side street with a burnt-out lamp.");
var archive = new Location("archive", "An archive that accepts any confession.");

square.AddExit(Direction.East, sideStreet);
sideStreet.AddExit(Direction.North, archive);

var camera = new Item("camera", "camera", "A camera with one shot left.")
    .SetHint("Use it when the moment arrives.")
    .SetReaction(ItemAction.Use, "The shutter snaps, and the night freezes.");

square.AddItem(camera);

var witness = new Npc("witness", "witness")
    .Description("A witness who cannot decide whether to speak.")
    .SetDialog(new DialogNode("I saw it, I swear.")
        .AddOption("Ask what they saw")
        .AddOption("Ask for a name"));

sideStreet.AddNpc(witness);

var state = new GameState(square, worldLocations: new[] { square, sideStreet, archive });

state.Events.Subscribe(GameEventType.TalkToNpc, e =>
{
    if (e.Npc?.Id == "witness")
    {
        state.WorldState.AddTimeline("A witness spoke.");
    }
});

state.Events.Subscribe(GameEventType.EnterLocation, e =>
{
    if (e.Location?.Id == "archive")
    {
        state.WorldState.AddTimeline("The archive listened.");
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
        if (g.State.WorldState.Timeline.Count >= 2)
        {
            g.Output.WriteLine("You were there, and now the record will not let you leave.");
            g.RequestStop();
        }
    })
    .Build();

game.Run();
```
