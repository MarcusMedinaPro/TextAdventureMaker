# Street Robbery

_Slice tag: Slice 22 — Threat + branching response. Demo focuses on a simple confrontation flow._

## Story beats (max ~10 steps)
1) You enter a dim alley.
2) A threat blocks your path.
3) Choose to flee, fight, or talk.

## Example (combat or talk)
```csharp
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;

var alley = new Location("alley", "A dim alley. Footsteps echo behind you.");
var street = new Location("street", "A wet street with a closed shop and a flickering lamp.");

alley.AddExit(Direction.North, street);

var coin = new Item("coin", "coin", "A single coin with a dull shine.");
alley.AddItem(coin);

var mugger = new Npc("mugger", "mugger", NpcState.Hostile)
    .Description("A mugger with a bruised jaw and a steady stare.")
    .SetDialog(new DialogNode("Wallet. Now.")
        .AddOption("Hand over the coin")
        .AddOption("Refuse"));

var passerby = new Npc("passerby", "passerby")
    .Description("A passerby who pretends not to see you.")
    .SetDialog(new DialogNode("I didn't see anything.")
        .AddOption("Ask for help")
        .AddOption("Let them go"));

alley.AddNpc(mugger);
street.AddNpc(passerby);

var state = new GameState(alley, worldLocations: new[] { alley, street });

state.Factions.AddFaction("locals")
    .WithNpcs("passerby")
    .OnReputationThreshold(5, s => s.WorldState.SetFlag("locals_trust", true));

state.Factions.AddFaction("muggers")
    .WithNpcs("mugger")
    .OnReputationThreshold(-3, s => s.WorldState.SetFlag("muggers_hunt", true));

state.Events.Subscribe(GameEventType.TalkToNpc, e =>
{
    if (e.Npc?.Id == "passerby")
    {
        state.Factions.ModifyReputation("locals", 2, state);
    }
});

state.Events.Subscribe(GameEventType.CombatStart, e =>
{
    if (e.Npc?.Id == "mugger")
    {
        state.Factions.ModifyReputation("locals", 2, state);
        state.Factions.ModifyReputation("muggers", -2, state);
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
        if (g.State.WorldState.GetFlag("locals_trust"))
        {
            g.Output.WriteLine("Word travels fast. The locals start watching your back.");
            g.State.WorldState.SetFlag("locals_trust", false);
        }

        if (g.State.WorldState.GetFlag("muggers_hunt"))
        {
            g.Output.WriteLine("The alley goes quiet. The muggers have marked you.");
            g.State.WorldState.SetFlag("muggers_hunt", false);
        }
    })
    .Build();

// Console setup for C64 aesthetics
Console.BackgroundColor = ConsoleColor.DarkBlue;
Console.ForegroundColor = ConsoleColor.Cyan;
Console.Title = "Street Robbery - Text Adventure Sandbox";
Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.Clear();
// End console setup
game.Run();
```
