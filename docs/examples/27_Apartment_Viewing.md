# Apartment Viewing

_Slice tag: Slice 27 — Questions + reveal loop._

## Story beats (max ~10 steps)
1) Arrive at the lobby.
2) Meet the agent.
3) Inspect the unit.
4) Ask questions and decide.

## Example (agent questions)
```csharp
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;

Location lobby = (id: "apartment_lobby", description: "A tidy lobby with a directory and fresh paint.");
Location unit = (id: "apartment_unit", description: "A bright unit with tall windows and clean floors.");
Location balcony = (id: "balcony", description: "A small balcony overlooking the street.");

lobby.AddExit(Direction.In, unit);
unit.AddExit(Direction.East, balcony);

lobby.AddItem(new Item("brochure", "brochure", "A brochure listing amenities and fees."));
unit.AddItem(new Item("inspection", "inspection list", "A checklist of items to review."));

var agent = new Npc("agent", "agent")
    .Description("An agent waits with a small stack of keys.")
    .SetDialog(new DialogNode("Any questions about the unit?")
        .AddOption("Ask about noise levels")
        .AddOption("Ask about utilities")
        .AddOption("Ask about move-in dates"));

unit.AddNpc(agent);

var state = new GameState(lobby, worldLocations: new[] { lobby, unit, balcony });
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
