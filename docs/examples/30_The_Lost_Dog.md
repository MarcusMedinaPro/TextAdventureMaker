# The Lost Dog

_Slice tag: Slice 30 — Multi-location follow + rescue + payoff._

## Story beats (max ~10 steps)
1) Find a lost dog flyer.
2) Search the park.
3) Approach the dog.
4) Decide what to do next.

## Example (lost dog)
```csharp
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;

Location nightStreet = (id: "night_street", description: "A long street with patchy streetlights.");
Location park = (id: "park", description: "A small park with damp grass and a lone bench.");

nightStreet.AddExit(Direction.West, park);

park.AddItem(new Item("flyer", "flyer", "LOST DOG: small terrier, answers to Pip."));

var dog = new Npc("dog", "dog")
    .SetState(NpcState.Friendly)
    .Description("A small terrier with an anxious wag.")
    .SetDialog(new DialogNode("The dog looks up, waiting.")
        .AddOption("Show the flyer")
        .AddOption("Offer a hand to sniff"));

park.AddNpc(dog);

var state = new GameState(nightStreet, worldLocations: new[] { nightStreet, park });
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
