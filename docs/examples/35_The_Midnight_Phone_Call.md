# The Midnight Phone Call

_Slice tag: Slice 35 — Remote dialog + branching response._

## Story beats (max ~10 steps)
1) Phone rings at night.
2) Decide whether to answer.
3) Respond to the caller.
4) End the call.

## Example (midnight call)
```csharp
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;

Location bedroom = (id: "bedroom", description: "A small bedroom lit by the glow of a phone screen.");

bedroom.AddItem(new Item("landline", "phone", "A phone buzzing with an incoming call.")
    .SetTakeable(false));

var caller = new Npc("caller", "caller")
    .Description("A familiar voice waits on the line.")
    .SetDialog(new DialogNode("Is now a good time to talk?")
        .AddOption("Answer calmly")
        .AddOption("Ask who this is")
        .AddOption("Hang up"));

bedroom.AddNpc(caller);

var state = new GameState(bedroom, worldLocations: new[] { bedroom });
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
