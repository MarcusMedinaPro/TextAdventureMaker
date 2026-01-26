# A Visit to the Other Side
_Slice tag: Slice 36 — Framed Narrative (Here-There-Here) (Creepypasta style, British English)._


    ## Premise
    You open the storeroom door. It is much larger inside. When you return, the door is in a different house.

    ## Arc structure
    - Here → The familiar room.
- There → The impossible space.
- Here again → Home is not where you left it.

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

var storeroom = new Location("storeroom", "A storeroom with a door that feels too small.");
var otherSide = new Location("other_side", "A vast room inside the same door.");
var newHouse = new Location("new_house", "You return and the door is in another house.");

storeroom.AddExit(Direction.North, otherSide);
otherSide.AddExit(Direction.South, newHouse, oneWay: true);

var chalk = new Item("chalk", "chalk", "Chalk that leaves no dust on your hands.")
    .SetReaction(ItemAction.Use, "The doorway sketches itself wider.");

storeroom.AddItem(chalk);

var state = new GameState(storeroom, worldLocations: new[] { storeroom, otherSide, newHouse });

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
        if (g.State.IsCurrentRoomId("new_house"))
        {
            g.Output.WriteLine("You return, and the here you know is no longer yours.");
            g.RequestStop();
        }
    })
    .Build();

game.Run();
```
