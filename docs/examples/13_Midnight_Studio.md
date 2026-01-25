# Midnight Studio

_Slice tag: Slice 13 — GameBuilder + Game loop (NPC ticks). Demo focuses on fluent setup and running the game._

## Story beats (max ~10 steps)
1) You enter a quiet studio at midnight.
2) A cat watches from the rafters.
3) You find a sketchbook.
4) You leave with a new idea.

## Example (GameBuilder)
```csharp
using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;

var studio = new Location("studio", "A quiet studio, lit by a single lamp.");
var hallway = new Location("hallway", "A long hallway of empty frames.");
studio.AddExit(Direction.Out, hallway);

var cat = new Npc("cat", "cat")
    .Description("A curious cat balances on the beams.")
    .SetMovement(new PatrolNpcMovement(new[] { studio, hallway }));
studio.AddNpc(cat);

var parser = new KeywordParser(KeywordParserConfig.Default);

var game = GameBuilder.Create()
    .AddLocation(studio, isStart: true)
    .AddLocation(hallway)
    .UseParser(parser)
    .AddTurnStart(g =>
    {
        var look = g.State.Look();
        g.Output.WriteLine($"\n{look.Message}");
    })
    .AddTurnEnd((g, command, result) =>
    {
        if (command is QuitCommand)
        {
            g.RequestStop();
        }
    })
    .Build();

game.Run();
```
