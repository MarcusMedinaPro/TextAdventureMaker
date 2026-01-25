# The Night Walk Home

_Slice tag: Slice 29 — Tension + risk assessment beats._

## Story beats (max ~10 steps)
1) Leave the bar.
2) Walk through a darker street.
3) Decide how to handle the tension.
4) Reach home.

## Example (night walk)
```csharp
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;

Location barAlley = (id: "bar_alley", description: "A narrow alley behind the bar.");
Location nightStreet = (id: "night_street", description: "A long street with patchy streetlights.");
Location underpass = (id: "underpass", description: "A shadowy underpass humming with distant traffic.");
Location frontPorch = (id: "front_porch", description: "A quiet front porch with a locked gate.");

barAlley.AddExit(Direction.East, nightStreet);
nightStreet.AddExit(Direction.East, underpass);
underpass.AddExit(Direction.North, frontPorch);

nightStreet.AddItem(new Item("streetlight", "streetlight", "A flickering streetlight buzzes overhead.")
    .SetTakeable(false));
underpass.AddItem(new Item("whistle", "whistle", "A small whistle on a frayed cord."));

var stranger = new Npc("stranger", "stranger")
    .SetState(NpcState.Hostile)
    .SetStats(new Stats(8))
    .Description("A stranger lingers in the shadows.")
    .SetDialog(new DialogNode("You lost?")
        .AddOption("Keep walking")
        .AddOption("Ask for directions"));

nightStreet.AddNpc(stranger);

var state = new GameState(barAlley, worldLocations: new[] { barAlley, nightStreet, underpass, frontPorch });
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
