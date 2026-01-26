# The Street I No Longer Recognise
_Slice tag: Slice 36 — Familiar to Foreign (Creepypasta style, British English)._


    ## Premise
    You walk home from work. Same street, same house numbers. Yet every window is dark, and no mirror shows your reflection.

    ## Arc structure
    - Familiar → The street is yours.
- Transition → Shadows swallow the windows.
- Foreign → Mirrors refuse you.
- Return with insight → You no longer feel at home.

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

var homeStreet = new Location("home_street", "Your street looks familiar, down to the bins.");
var passage = new Location("passage", "A passage that narrows into the unknown.");
var foreignStreet = new Location("foreign_street", "A street with the right shops, all wrong.");

homeStreet.AddExit(Direction.East, passage);
passage.AddExit(Direction.East, foreignStreet);

var ticket = new Item("ticket", "bus ticket", "A ticket stamped with a date that has not happened yet.");
var map = new Item("map", "folded map", "A map that keeps shifting when you look away.")
    .SetReadText("It names your street with the wrong name.")
    .RequireTakeToRead();

homeStreet.AddItem(ticket);
passage.AddItem(map);

var stranger = new Npc("stranger", "stranger")
    .Description("A stranger who speaks in a local accent you do not know.")
    .SetDialog(new DialogNode("You are further from home than you think.")
        .AddOption("Ask for directions")
        .AddOption("Ask your own name"));

foreignStreet.AddNpc(stranger);

var state = new GameState(homeStreet, worldLocations: new[] { homeStreet, passage, foreignStreet });

state.Events.Subscribe(GameEventType.EnterLocation, e =>
{
    if (e.Location?.Id == "foreign_street")
    {
        state.WorldState.SetFlag("foreign", true);
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
        if (g.State.WorldState.GetFlag("foreign"))
        {
            g.Output.WriteLine("The familiar street becomes foreign in your mouth.");
            g.RequestStop();
        }
    })
    .Build();

game.Run();
```
