# The Guard Who Lowered His Weapon
_Slice tag: Slice 28 — Character Arc (Creepypasta style, British English)._


    ## Premise
    The guard stopped aiming at the doorway when he realised the threat was not there. It stood behind him.

    ## Arc structure
    - Mask → The guard holds his aim.
- Crack → Doubt enters.
- Choice → He lowers the weapon.
- Change → He sees the real threat.

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

var checkpoint = new Location("checkpoint", "A checkpoint with a single light and a single guard.");
var yard = new Location("yard", "A yard that feels too open to be safe.");

checkpoint.AddExit(Direction.North, yard);

var badge = new Item("badge", "old badge", "A badge with the guard's old name.")
    .SetHint("Show it to him.");

checkpoint.AddItem(badge);

var guard = new Npc("guard", "guard", NpcState.Hostile)
    .Description("A guard who keeps his weapon raised.")
    .SetDialog(new DialogNode("Step back.")
        .AddOption("Show the badge")
        .AddOption("Say nothing"));

checkpoint.AddNpc(guard);

var state = new GameState(checkpoint, worldLocations: new[] { checkpoint, yard });

state.Events.Subscribe(GameEventType.TalkToNpc, e =>
{
    if (e.Npc?.Id == "guard" && state.Inventory.FindItem("badge") != null)
    {
        guard.SetState(NpcState.Friendly);
        state.WorldState.AddTimeline("The guard lowered his weapon.");
    }
});

var quest = new Quest("guard_arc", "Lower the Weapon", "Change the guard's stance.")
    .AddCondition(new NpcStateCondition(guard, NpcState.Friendly))
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
            g.Output.WriteLine("He lowers the weapon and looks past you for the first time.");
            g.RequestStop();
        }
    })
    .Build();

game.Run();
```
