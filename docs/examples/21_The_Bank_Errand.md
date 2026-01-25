# The Bank Errand

_Slice tag: Slice 21 — Transaction flow. Demo focuses on a small stateful interaction with a teller._

## Story beats (max ~10 steps)
1) You enter the bank.
2) Take a number.
3) Wait for your turn.
4) Resolve a simple issue.

## Example (dialog + state checks)
```csharp
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;

Location lobby = (id: "bank_lobby", description: "A quiet bank lobby with a ticket machine.");
Location counter = (id: "bank_counter", description: "A teller waits behind the counter.");

lobby.AddExit(Direction.North, counter);

Item ticket = (id: "ticket", name: "number ticket", description: "Your place in line.");
lobby.AddItem(ticket);

var teller = new Npc("teller", "teller")
    .Description("A patient teller taps the desk.")
    .SetDialog(new DialogNode("How can I help you today?")
        .AddOption("Fix my account issue")
        .AddOption("Ask about fees"));

counter.AddNpc(teller);

var state = new GameState(lobby, worldLocations: new[] { lobby, counter });
var parser = new KeywordParser(KeywordParserConfig.Default);

var game = GameBuilder.Create()
    .UseState(state)
    .UseParser(parser)
    .AddTurnStart(g =>
    {
        var look = g.State.Look();
        g.Output.WriteLine($"\n{look.Message}");
    })
    .AddTurnEnd((g, command, result) =>
    {
        if (g.State.Inventory.Items.Any(i => i.Id == "ticket"))
        {
            g.State.WorldState.SetFlag("has_ticket", true);
        }
    })
    .Build();

game.Run();
```
