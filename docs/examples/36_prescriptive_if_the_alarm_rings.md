# If the Alarm Rings
_Slice tag: Slice 36 — Prescriptive Structure (Creepypasta style, British English)._


    ## Premise
    Given: the house is empty. When: the clock strikes 03:17. Then: do not go into the loft, no matter how much it sounds like someone is crying.

    ## Arc structure
    - Given → The house is empty.
- When → 03:17 arrives.
- Then → Do not go to the loft.

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
│  Safe Room │
│            │
└─────┬──────┘
      │
┌────────────┐
│   Stair    │
│            │
└─────┬──────┘
      │
┌────────────┐
│   Lobby    │
│     A B    │
└────────────┘

A = Alarm box
B = Badge
```

    ## Example (detailed setup)

```csharp
using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;

var lobby = new Location("lobby", "A lobby with a red alarm box.");
var stair = new Location("stair", "Stairs that echo each step.");
var safeRoom = new Location("safe_room", "A safe room with a seal on the door.");

lobby.AddExit(Direction.North, stair);
stair.AddExit(Direction.North, safeRoom);

var badge = new Item("badge", "badge", "A badge with a faded crest.");
var alarm = new Item("alarm", "alarm box", "A box that should only be opened once.")
    .SetReaction(ItemAction.Use, "The alarm rings. The rules begin.");

lobby.AddItem(badge);
lobby.AddItem(alarm);

var state = new GameState(lobby, worldLocations: new[] { lobby, stair, safeRoom });

badge.OnTake += _ => state.WorldState.SetFlag("given", true);
alarm.OnUse += _ => state.WorldState.SetFlag("when", true);

state.Events.Subscribe(GameEventType.EnterLocation, e =>
{
    if (e.Location?.Id == "safe_room")
    {
        state.WorldState.SetFlag("then", true);
    }
});

var quest = new Quest("prescriptive", "Follow the Steps", "Given, when, then.")
    .AddCondition(new WorldFlagCondition("given", true))
    .AddCondition(new WorldFlagCondition("when", true))
    .AddCondition(new WorldFlagCondition("then", true))
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
        if (quest.CheckProgress(g.State))
        {
            g.Output.WriteLine("The steps complete themselves, exactly as written.");
            g.RequestStop();
        }
    })
    .Build();

game.Run();
```
