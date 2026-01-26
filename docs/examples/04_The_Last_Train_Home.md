# The Last Train Home

_Slice tag: Slice 4 — Items + Inventory. Demo focuses on items, pickup, inventory, and a simple choice gate._

## Story beats (max ~10 steps)
1) You arrive at the platform.
2) A ticket booth is closed.
3) Find a ticket on the bench.
4) Decide: board the train or stay.

## Example (items + inventory)
```csharp
using System;
using System.Collections.Generic;
using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;

var platform = new Location("platform", "A quiet platform. The last train waits in the rain.");
var carriage = new Location("carriage", "Warm light, tired faces, a seat by the window.");

var ticket = new Item("ticket", "train ticket", "A single-ride ticket with a faded stamp.")
    .SetWeight(0.01f)
    .AddAliases("ticket", "pass");

var tea = new Item("thermos", "tea thermos", "A dented thermos that smells of black tea.")
    .SetWeight(0.6f)
    .AddAliases("thermos", "tea")
    .SetReaction(ItemAction.Use, "You take a careful sip. It warms your hands.");

platform.AddItem(ticket);
platform.AddItem(tea);
platform.AddExit(Direction.In, carriage, oneWay: true);

var state = new GameState(platform, worldLocations: new[] { platform, carriage });

var parserConfig = KeywordParserConfigBuilder.BritishDefaults()
    .WithLook("look", "l")
    .WithInventory("inventory", "inv", "i")
    .WithTake("take", "get", "pick")
    .WithDrop("drop")
    .WithUse("use")
    .WithGo("go", "move")
    .WithIgnoreItemTokens("on", "off")
    .WithDirectionAliases(new Dictionary<string, Direction>(StringComparer.OrdinalIgnoreCase)
    {
        ["in"] = Direction.In,
        ["out"] = Direction.Out
    })
    .Build();
var parser = new KeywordParser(parserConfig);

Console.WriteLine("=== THE LAST TRAIN HOME (Slice 4) ===");
Console.WriteLine("Commands: Look, Take <Item>, Drop <Item>, Use <Item>, Inventory, Go In/Out, Board, Quit");

void WriteResult(CommandResult result)
{
    if (!string.IsNullOrWhiteSpace(result.Message))
    {
        Console.WriteLine(result.Message);
    }

    foreach (var reaction in result.ReactionsList)
    {
        if (!string.IsNullOrWhiteSpace(reaction))
        {
            Console.WriteLine($"> {reaction}");
        }
    }
}

void ShowLookResult(CommandResult result)
{
    Console.WriteLine($"Room: {state.CurrentLocation.Id.ToProperCase()}");
    WriteResult(result);
}

while (true)
{
    Console.Write("\n> ");
    var input = Console.ReadLine()?.Trim();
    if (string.IsNullOrWhiteSpace(input)) continue;

    if (input.Equals("board", StringComparison.OrdinalIgnoreCase))
    {
        var hasTicket = state.Inventory.FindItem("ticket") != null;
        if (!hasTicket)
        {
            Console.WriteLine("You need a ticket to board.");
            continue;
        }

        state.Move(Direction.In);
        Console.WriteLine("You board the train. The city fades behind you.");
        continue;
    }

    if (input.Equals("stay", StringComparison.OrdinalIgnoreCase))
    {
        Console.WriteLine("You let the train go. You stay behind.");
        break;
    }

    var command = parser.Parse(input);
    var result = state.Execute(command);

    if (command is LookCommand)
    {
        ShowLookResult(result);
    }
    else
    {
        WriteResult(result);
    }

    if (result.ShouldQuit) break;
}
```
