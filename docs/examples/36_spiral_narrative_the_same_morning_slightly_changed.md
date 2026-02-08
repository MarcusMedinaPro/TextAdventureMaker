# The Same Morning, Slightly Changed
_Slice tag: Slice 36 — Spiral Narrative (Creepypasta style, British English)._


    ## Premise
    You wake each day to the same rain on the window. The same mug. The same time. Yet each morning something in the room is just wrong enough to notice.

    ## Arc structure
    - Repetition → The morning repeats.
- Variation → One detail shifts each loop.
- Recognition → You learn which details matter.
- Break or surrender → The loop either cracks or owns you.

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

┌────────────┐      ┌────────────┐
│  Kitchen   │      │  Hallway   │
│            │      │            │
└─────┬──────┘      └─────┬──────┘
      │                   │
      └─────────┬─────────┘
                │
         ┌────────────┐
         │  Bedroom   │
         │     S      │
         └────────────┘

S = Scrap of paper
```

    ## Example (detailed setup)

```csharp
using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;

var bedroom = new Location("bedroom", "A room that resets its shadows each time you blink.");
var kitchen = new Location("kitchen", "The kettle whistles, then forgets why.");
var hallway = new Location("hallway", "A hallway that loops back on itself.");

bedroom.AddExit(Direction.East, hallway);
hallway.AddExit(Direction.North, kitchen);
kitchen.AddExit(Direction.West, bedroom);

var scrap = new Item("scrap", "scrap of paper", "A note that seems newly written each time.")
    .SetReadText("You have done this before.")
    .RequireTakeToRead();

bedroom.AddItem(scrap);

var state = new GameState(bedroom, worldLocations: new[] { bedroom, kitchen, hallway });

state.Events.Subscribe(GameEventType.EnterLocation, e =>
{
    if (e.Location?.Id == "bedroom")
    {
        var count = state.WorldState.Increment("loops");
        state.WorldState.AddTimeline($"Loop {count}");
    }
});

var parser = new KeywordParser(KeywordParserConfig.Default);

var game = GameBuilder.Create()
    .UseState(state)
    .UseParser(parser)
    .AddTurnStart(g =>
    {
        var loops = g.State.WorldState.GetCounter("loops");
        var look = g.State.Look();
        var variation = loops switch
        {
            1 => "The sheets are warm.",
            2 => "The sheets are cold.",
            _ => "The sheets remember nothing."
        };
        g.Output.WriteLine($"
{look.Message}
{variation}");
    })
    .AddTurnEnd((g, command, result) =>
    {
        if (g.State.WorldState.GetCounter("loops") >= 3)
        {
            g.Output.WriteLine("You notice the morning is learning you back.");
            g.RequestStop();
        }
    })
    .Build();

game.Run();
```
