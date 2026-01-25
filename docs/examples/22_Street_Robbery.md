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

Location alley = (id: "alley", description: "A dim alley. Footsteps echo behind you.");

var mugger = new Npc("mugger", "mugger", NpcState.Hostile, stats: new Stats(15))
    .Description("A shadow steps forward, hand in pocket.")
    .SetDialog(new DialogNode("Give me your wallet.")
        .AddOption("Try to talk your way out")
        .AddOption("Throw a coin and run"));

alley.AddNpc(mugger);

var state = new GameState(alley, worldLocations: new[] { alley });
var parser = new KeywordParser(KeywordParserConfig.Default);

var game = GameBuilder.Create()
    .UseState(state)
    .UseParser(parser)
    .AddTurnStart(g =>
    {
        var look = g.State.Look();
        g.Output.WriteLine($"\n{look.Message}");
    })
    .AddTurnEnd((g, command, result) =>
    {
        if (!mugger.IsAlive)
        {
            g.Output.WriteLine("You break free and escape the alley.");
            g.RequestStop();
        }
    })
    .Build();

game.Run();
```
