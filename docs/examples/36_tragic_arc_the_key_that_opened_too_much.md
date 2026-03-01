# The Key That Opened Too Much
_Slice tag: Slice 36 — Tragic Arc (Creepypasta style, British English)._


    ## Premise
    The key fits no lock you have ever seen. When you finally find the door, you wish you had never touched it. Some rooms are sealed to protect the world, not you.

    ## Arc structure
    - Hybris → Curiosity overrides caution.
- Hamartia → You use the key.
- Peripeteia → The room opens and the world shifts.
- Anagnorisis → You understand why it was sealed.
- Katharsis → You accept what cannot be undone.

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
│ Sealed Room│
│            │
└─────┬──────┘
      │
┌────────────┐      ┌────────────┐
│  Corridor  │──────│   Study    │
│     W      │      │    K J     │
└────────────┘      └────────────┘

K = Brass key
J = Journal
W = Warden
```

    ## Example (detailed setup)

```csharp
using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;

var study = new Location("study", "A study with a locked drawer and a cold fireplace.");
var corridor = new Location("corridor", "A corridor that leans towards the dark.");
var sealedRoom = new Location("sealed_room", "The sealed room smells of ash and regret.");

study.AddExit(Direction.East, corridor);

var brassKey = new Key("brass_key", "brass key", "A key that warms as you hold it.")
    .SetHint("It fits the seal.");
var journal = new Item("journal", "family journal", "A journal bound in brittle leather.")
    .SetReadText("You were warned never to turn the third latch.")
    .RequireTakeToRead();

study.AddItem(brassKey);
study.AddItem(journal);

var seal = new Door("seal", "sealed door", "A sealed door that has never been opened.")
    .RequiresKey(brassKey)
    .SetReaction(DoorAction.Open, "The seal yields with a soft crack.");

corridor.AddExit(Direction.North, sealedRoom, seal);

var warden = new Npc("warden", "warden", NpcState.Hostile)
    .Description("A warden with eyes like burnt paper.")
    .SetDialog(new DialogNode("You were told not to.")
        .AddOption("Step back")
        .AddOption("Press on"));

corridor.AddNpc(warden);

var state = new GameState(study, worldLocations: new[] { study, corridor, sealedRoom });

state.Events.Subscribe(GameEventType.CombatStart, e =>
{
    if (e.Npc?.Id == "warden")
    {
        state.WorldState.Increment("defiance");
    }
});

state.Events.Subscribe(GameEventType.OpenDoor, e =>
{
    if (e.Door?.Id == "seal")
    {
        state.WorldState.SetFlag("opened_seal", true);
        state.WorldState.AddTimeline("You opened what should have stayed closed.");
    }
});

var quest = new Quest("tragedy", "Open the Seal", "The door opens and the cost arrives.")
    .AddCondition(new WorldFlagCondition("opened_seal", true))
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
        if (quest.CheckProgress(g.State) && g.State.IsCurrentRoomId("sealed_room"))
        {
            g.Output.WriteLine("You understand the seal was not for your protection.");
            g.RequestStop();
        }
    })
    .Build();

game.Run();
```
