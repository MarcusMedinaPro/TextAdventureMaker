# From Silence to Voice
_Slice tag: Slice 36 — Transformation Arc (Creepypasta style, British English)._


    ## Premise
    The bathroom mirror shows your reflection staying silent while its lips move. After a few days you begin to understand what it is trying to say, and why it chose you.

    ## Arc structure
    - Fragmented → You cannot name what you feel.
- Shadow Confrontation → The mirror speaks back.
- Integration → You learn to answer it.
- New Self → Your voice becomes your own again.

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
│   Choir    │
│   T  C     │
└─────┬──────┘
      │
┌────────────┐
│ Corridor  │
│     M     │
└─────┬──────┘
      │
┌────────────┐
│Silence Room│
│   I  U     │
└────────────┘

C = Chorus
I = Ink vial
M = Mask
T = Thread
U = Glass cup
```

    ## Example (detailed setup)

```csharp
using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;

var silenceRoom = new Location("silence_room", "A room that swallows every sound.");
var corridor = new Location("corridor", "A corridor of mirrors that refuse to agree.");
var choir = new Location("choir", "A vaulted chamber where voices wait in the rafters.");

silenceRoom.AddExit(Direction.East, corridor);
corridor.AddExit(Direction.North, choir);

var ink = new FluidItem("ink", "ink", "A vial of ink that shivers when it is watched.");
var cup = new Glass("cup", "glass cup", "A thin glass cup with a crack along the rim.");
var mask = new Item("mask", "paper mask", "A mask that fits too well.");
var thread = new Item("thread", "silver thread", "Thread that bites your fingertips.");

silenceRoom.AddItem(ink);
silenceRoom.AddItem(cup);
corridor.AddItem(mask);
choir.AddItem(thread);

var chorus = new Npc("chorus", "chorus")
    .Description("A line of figures with mouths shut tight.")
    .SetDialog(new DialogNode("Speak, and we will answer.")
        .AddOption("Try to speak")
        .AddOption("Stay silent"));

choir.AddNpc(chorus);

var state = new GameState(silenceRoom, worldLocations: new[] { silenceRoom, corridor, choir });
state.RecipeBook.Add(new ItemCombinationRecipe("mask", "thread", () =>
    new Item("voice", "voice", "A stitched voice that sits in your chest.")
        .SetReaction(ItemAction.Use, "Your words return, raw and strange.")));

state.Events.Subscribe(GameEventType.PickupItem, e =>
{
    if (e.Item?.Id == "voice")
    {
        state.WorldState.SetFlag("found_voice", true);
    }
});

var quest = new Quest("transformation", "Recover Your Voice", "Find and use your voice.")
    .AddCondition(new WorldFlagCondition("found_voice", true))
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
            g.Output.WriteLine("You speak, and the chamber finally answers.");
            g.RequestStop();
        }
    })
    .Build();

game.Run();
```
