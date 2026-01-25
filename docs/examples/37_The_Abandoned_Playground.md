# The Abandoned Playground

_Slice tag: Slice 37 — Atmosphere and delayed reveals._

## Story beats (max ~10 steps)
1) Enter the playground.
2) Notice the old swing.
3) Find a lost toy.
4) Decide whether to take it.

## Example (playground)
```csharp
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;

Location playground = (id: "playground", description: "An abandoned playground with creaking swings.");

playground.AddItem(new Item("swing", "swing", "A swing creaks in the wind.")
    .SetTakeable(false));
playground.AddItem(new Item("toy", "stuffed toy", "A worn stuffed toy with a stitched tag."));

var state = new GameState(playground, worldLocations: new[] { playground });
var parser = new KeywordParser(KeywordParserConfig.Default);

var game = GameBuilder.Create()
    .UseState(state)
    .UseParser(parser)
    .AddTurnStart(g =>
    {
        var look = g.State.Look();
        g.Output.WriteLine($"\n{look.Message}");
    })
    .Build();

game.Run();
```
