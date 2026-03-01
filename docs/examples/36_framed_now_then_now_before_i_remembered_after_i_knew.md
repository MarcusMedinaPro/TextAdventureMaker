# Before I Remembered, After I Knew
_Slice tag: Slice 36 — Framed Narrative (Now-Then-Now) (Creepypasta style, British English)._


    ## Premise
    Now: you sit in your kitchen. Then: you were in the cellar. Now again: but your shoes are still wet with something that is not water.

    ## Arc structure
    - Now → Present calm is fragile.
- Then → The memory returns.
- Now again → The present is changed by it.

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
│  Memory    │
└─────┬──────┘
      │
      │
┌────────────┐
│ Present    │
│    P       │
└─────┬──────┘
      │ (one-way)
      ▼
┌────────────┐
│  Return    │
└────────────┘

P = Photograph
```

    ## Example (detailed setup)

```csharp
using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;

var present = new Location("present", "A kitchen where the kettle keeps restarting.");
var memory = new Location("memory", "A hallway from your childhood, too narrow to be real.");
var returnRoom = new Location("return", "The same kitchen, but now it knows your name.");

present.AddExit(Direction.North, memory);
memory.AddExit(Direction.South, returnRoom, oneWay: true);

var photograph = new Item("photograph", "photograph", "A photograph with a missing figure.")
    .SetReadText("You were there. You simply forgot.")
    .RequireTakeToRead();

present.AddItem(photograph);

var state = new GameState(present, worldLocations: new[] { present, memory, returnRoom });

state.Events.Subscribe(GameEventType.EnterLocation, e =>
{
    if (e.Location?.Id == "memory")
    {
        state.WorldState.SetFlag("flashback", true);
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
        if (g.State.WorldState.GetFlag("flashback") && g.State.IsCurrentRoomId("return"))
        {
            g.Output.WriteLine("The present folds around the memory you refused.");
            g.RequestStop();
        }
    })
    .Build();

game.Run();
```
