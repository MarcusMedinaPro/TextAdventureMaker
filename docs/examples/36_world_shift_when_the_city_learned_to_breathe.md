# When the City Learned to Breathe
_Slice tag: Slice 36 — World Shift (Creepypasta style, British English)._


    ## Premise
    At night a slow, deliberate breath moves through the drains. The city is no longer dead matter. It is waiting.

    ## Arc structure
    - Slow change → The city breathes.
- Player catalyst → Your actions awaken it.
- Systems collide → Infrastructure and instinct.
- New equilibrium → You must decide how to live in it.

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

var square = new Location("square", "A city square that feels held shut.");
var alley = new Location("alley", "A narrow alley with damp stones.");
var bridge = new Location("bridge", "A bridge where the river sounds like speech.");

square.AddExit(Direction.East, alley);
alley.AddExit(Direction.North, bridge);

var valve = new Item("valve", "brass valve", "A valve that resists turning.")
    .SetReaction(ItemAction.Use, "The city exhales, slowly.");

alley.AddItem(valve);

var state = new GameState(square, worldLocations: new[] { square, alley, bridge });

valve.OnUse += _ => state.WorldState.SetFlag("city_breathes", true);

var parser = new KeywordParser(KeywordParserConfig.Default);

var game = GameBuilder.Create()
    .UseState(state)
    .UseParser(parser)
    .AddTurnStart(g =>
    {
        var look = g.State.Look();
        var shift = g.State.WorldState.GetFlag("city_breathes")
            ? "You feel the streets loosen, almost grateful."
            : "The city feels tight, like a held breath.";
        g.Output.WriteLine($"
{look.Message}
{shift}");
    })
    .AddTurnEnd((g, command, result) =>
    {
        if (g.State.WorldState.GetFlag("city_breathes") && g.State.IsCurrentRoomId("bridge"))
        {
            g.Output.WriteLine("The river answers, and the city finally learns to breathe.");
            g.RequestStop();
        }
    })
    .Build();

game.Run();
```
