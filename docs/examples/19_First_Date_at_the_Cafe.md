# First Date at the Café

_Slice tag: Slice 19 — Dialog choices + small consequences. Demo focuses on conversational flow._

## Story beats (max ~10 steps)
1) You sit at a small table.
2) Make small talk.
3) Order coffee.
4) Decide whether to ask for a second date.

## Example (NPC dialog)
```csharp
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;

Location cafe = (id: "cafe", description: "A warm café with soft light and a small table.");

var date = new Npc("date", "date")
    .Description("A calm smile across the table.")
    .SetDialog(new DialogNode("The café is quiet. They wait for your first words.")
        .AddOption("Ask about their day")
        .AddOption("Compliment their outfit")
        .AddOption("Order coffee"));

cafe.AddNpc(date);

cafe.AddItem(new Item("menu", "menu", "A small menu with handwritten specials.").SetTakeable(false));

var state = new GameState(cafe, worldLocations: new[] { cafe });
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

// Console setup for C64 aesthetics
Console.BackgroundColor = ConsoleColor.DarkBlue;
Console.ForegroundColor = ConsoleColor.Cyan;
Console.Title = "First Date at the Café - Text Adventure Sandbox";
Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.Clear();
// End console setup
game.Run();
```
