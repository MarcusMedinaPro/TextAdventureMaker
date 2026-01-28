# The Last Train Home

_Slice tag: Slice 4 — Items & Inventory. Demo focuses on picking up items, the inventory view, and a small choice gate at the departure platform._

A rainy platform scene where the last train is waiting. There is a ticket and a warm thermos tucked under the bench, but only the ticket will get you aboard.

## Goal

Find the ticket, decide whether to board the train, and observe the carriage.

## Map (rough layout)

```
        N
  W         E
        S

┌────────────┐     ┌────────────┐
│ Platform   │─────│ Carriage   │
│            │     │            │
│     T      │     │     W      │
└────────────┘     └────────────┘

T = Ticket
W = Window seat
```

## Story beats (max ~10 steps)

1. You arrive at the deserted platform.
2. A train pressurises behind a curtained door.
3. A ticket waits on the bench.
4. Take the ticket, and perhaps the thermos.
5. Sit, drink, or board when ready.
6. Board the train with your ticket or stay behind and watch it disappear.

## Slice 1‑4 functions tested

- `Location(id, description)`
- `Location.AddExit(direction, target, oneWay: false)`
- `GameState(startLocation, worldLocations)`
- `GameState.Inventory`
- `Item(id, name, description)`
- `Item.SetReaction(action, text)`
- `DoorAction` / `ItemAction`
- `ICommand`
- `CommandResult`
- `ICommandParser`
- `KeywordParser(config)`
- `KeywordParserConfigBuilder` / `KeywordParserConfig`
- `CommandExtensions.Execute(state, command)`
- `Direction` enum

## Demo commands (parser)

- `look` / `l`
- `examine <item>` / `x <item>`
- `take <item>`
- `inventory` / `i`
- `move <item>`
- `use <item>` / `drink <item>` / `sip <item>`
- `go in` / `in` / `out`
- `board`
- `sit`
- `stay`
- `quit` / `exit`

## Example (items + inventory)
```csharp
using System;
using System.Collections.Generic;
using System.Linq;
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

var thermos = new Item("thermos", "tea thermos", "A dented thermos smelling of black tea.")
    .SetWeight(0.6f)
    .AddAliases("thermos", "tea")
    .SetReaction(ItemAction.Use, "You take a careful sip. It warms your hands and sends a curl of steam into the rain.");

platform.AddItem(ticket);
platform.AddItem(thermos);
platform.AddExit(Direction.In, carriage, oneWay: true);
carriage.AddExit(Direction.Out, platform);

var state = new GameState(platform, worldLocations: new[] { platform, carriage })
{
    EnableFuzzyMatching = true,
    FuzzyMaxDistance = 1
};

var parserConfig = KeywordParserConfigBuilder.BritishDefaults()
    .WithLook("look", "l")
    .WithExamine("examine", "x")
    .WithMove("move", "push", "shift", "lift", "slide")
    .WithInventory("inventory", "inv", "i")
    .WithTake("take", "get", "pick")
    .WithUse("use", "drink", "sip")
    .WithGo("go", "move")
    .WithFuzzyMatching(true, 1)
    .WithIgnoreItemTokens("on", "off")
    .WithDirectionAliases(new Dictionary<string, Direction>(StringComparer.OrdinalIgnoreCase)
    {
        ["in"] = Direction.In,
        ["out"] = Direction.Out
    })
    .Build();
var parser = new KeywordParser(parserConfig);

Console.WriteLine("=== THE LAST TRAIN HOME (Slice 4) ===");
Console.WriteLine("Goal: find the ticket, board the train, or stay on the platform.");
Console.WriteLine("Commands: look, examine <item>, take <item>, inventory, move <item>, use/drink/sip <item>, go in/out, board, sit, stay, quit.");
ShowRoom();

while (true)
{
    Console.Write("\n> ");
    var input = Console.ReadLine()?.Trim();
    if (string.IsNullOrWhiteSpace(input)) continue;

    if (input.Is("stay"))
    {
        Console.WriteLine("You let the train go. You stay behind and watch the vapour trail.");
        break;
    }

    if (input.Is("sit"))
    {
        if (state.IsCurrentRoomId("carriage"))
        {
            Console.WriteLine("You take a seat and watch rain smear the window into silver.");
        }
        else
        {
            Console.WriteLine("You sit for a moment, listening to the rain against the platform tiles.");
        }
        continue;
    }

    if (input.Is("board"))
    {
        if (!state.IsCurrentRoomId("platform"))
        {
            Console.WriteLine("You are already on board the train.");
            continue;
        }

        if (state.Inventory.FindItem("ticket") is null)
        {
            Console.WriteLine("You need a ticket to board.");
            continue;
        }

        if (state.Move(Direction.In))
        {
            Console.WriteLine("You board the train. The city blurs into neon.");
            ShowRoom();
        }
        else
        {
            Console.WriteLine(state.LastMoveError ?? "You cannot go that way.");
        }
        continue;
    }

    var command = parser.Parse(input);
    var result = state.Execute(command);

    switch (command)
    {
        case LookCommand:
            ShowLookResult(result);
            break;
        default:
            WriteResult(result);
            break;
    }

    if (command is GoCommand && result.Success && !result.ShouldQuit)
    {
        ShowRoom();
    }

    if (result.ShouldQuit) break;
}

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

void ShowRoom()
{
    var location = state.CurrentLocation;
    Console.WriteLine();
    Console.WriteLine($"Room: {location.Id.ToProperCase()}");
    Console.WriteLine(location.GetDescription());

    var items = location.Items.CommaJoinNames(properCase: true);
    Console.WriteLine(string.IsNullOrWhiteSpace(items) ? "Items here: None" : $"Items here: {items}");

    var exits = location.Exits
        .Select(exit =>
        {
            var direction = exit.Key.ToString().ToLowerInvariant().ToProperCase();
            return exit.Value.Door == null
                ? direction
                : $"{direction} ({exit.Value.Door.Name.ToProperCase()}, {exit.Value.Door.State.ToString().ToProperCase()})";
        })
        .ToList();

    Console.WriteLine(exits.Count > 0 ? $"Exits: {exits.CommaJoin()}" : "Exits: None");
}

void ShowLookResult(CommandResult result)
{
    Console.WriteLine($"Room: {state.CurrentLocation.Id.ToProperCase()}");
    WriteResult(result);
}
```
