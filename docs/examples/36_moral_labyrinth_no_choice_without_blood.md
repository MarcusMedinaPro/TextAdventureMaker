# No Choice Without Blood
_Slice tag: Slice 36 — Moral Labyrinth (Creepypasta style, British English)._


    ## Premise
    Every door you open saves someone. Every door you leave closed kills someone else. The map shows no exits.

    ## Arc structure
    - No right choices → Every door costs someone.
- Accumulated debt → The weight builds.
- Situational truth → There is no clean rescue.
- Aftermath → You live with what you chose.

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
│  Left Room │
│            │
└─────┬──────┘
      │
┌────────────┐
│  Chamber   │
│     T      │
└─────┬──────┘
      │
┌────────────┐
│ Right Room │
│            │
└────────────┘

T = Bone token (in chamber)
```

    ## Example (detailed setup)

```csharp
using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;

var chamber = new Location("chamber", "A chamber with two doors and no promise of mercy.");
var leftDoorRoom = new Location("left_room", "A room that smells of iron and wool.");
var rightDoorRoom = new Location("right_room", "A room that smells of rain and ash.");

var leftDoor = new Door("left_door", "left door", "A door marked with a pale hand.");
var rightDoor = new Door("right_door", "right door", "A door marked with a black hand.");

chamber.AddExit(Direction.West, leftDoorRoom, leftDoor);
chamber.AddExit(Direction.East, rightDoorRoom, rightDoor);

var token = new Item("token", "bone token", "A token that feels like a choice.")
    .SetReaction(ItemAction.Use, "The token grows heavier when you decide.");
chamber.AddItem(token);

var state = new GameState(chamber, worldLocations: new[] { chamber, leftDoorRoom, rightDoorRoom });

state.Events.Subscribe(GameEventType.EnterLocation, e =>
{
    if (e.Location?.Id == "left_room")
    {
        state.WorldState.SetFlag("choice_left", true);
    }
    if (e.Location?.Id == "right_room")
    {
        state.WorldState.SetFlag("choice_right", true);
    }
});

var quest = new Quest("labyrinth", "Choose a Door", "Any choice costs you.")
    .AddCondition(new AnyOfCondition(new[]
    {
        new WorldFlagCondition("choice_left", true),
        new WorldFlagCondition("choice_right", true)
    }))
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
            g.Output.WriteLine("There is no clean way through. Only a way through.");
            g.RequestStop();
        }
    })
    .Build();

game.Run();
```
