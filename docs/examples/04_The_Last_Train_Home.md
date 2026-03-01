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
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;
using static MarcusMedina.TextAdventure.Extensions.ConsoleExtensions;

// Slice 4 — Take All
// Tests:
// - TakeAll command (explicit take all/everything)
// - Inventory and room refresh after actions

Location platform = new("platform", "A quiet platform. The last train waits in the rain.");
Location carriage = new("carriage", "Warm light, tired faces, a seat by the window.");

Item ticket = new("ticket", "train ticket", "A single-ride ticket with a faded stamp.")
    .SetWeight(0.01f)
    .AddAliases("ticket", "pass");

Item thermos = new("thermos", "tea thermos", "A dented thermos smelling of black tea.")
    .SetWeight(0.6f)
    .AddAliases("thermos", "tea")
    .SetReaction(ItemAction.Use, "You take a careful sip. It warms your hands and sends a curl of steam into the rain.");

platform.AddItem(ticket);
platform.AddItem(thermos);
platform.AddExit(Direction.In, carriage, oneWay: true);
carriage.AddExit(Direction.Out, platform);

GameState state = new(platform, worldLocations: [platform, carriage])
{
    EnableFuzzyMatching = true,
    FuzzyMaxDistance = 1
};

KeywordParserConfig parserConfig = KeywordParserConfigBuilder.BritishDefaults()
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
KeywordParser parser = new(parserConfig);

SetupC64("THE LAST TRAIN HOME (Slice 4) - Text Adventure Sandbox");
WriteLineC64("=== THE LAST TRAIN HOME (Slice 4) ===");
WriteLineC64("Goal: gather your things (try take all), then board the train or stay on the platform.");
WriteLineC64("Commands: look, examine <item>, take <item>, take all, inventory, move <item>, use/drink/sip <item>, go in/out, board, sit, stay, quit.");
ShowRoom();

while (true)
{
    WriteLineC64();
    WritePromptC64("> ");
    string? input = Console.ReadLine();
    if (input is null)
        break;

    string trimmed = input.Trim();
    if (string.IsNullOrWhiteSpace(trimmed))
        continue;

    if (trimmed.Is("stay"))
    {
        WriteLineC64("You let the train go. You stay behind and watch the vapour trail.");
        break;
    }

    if (trimmed.Is("sit"))
    {
        if (state.IsCurrentRoomId("carriage"))
        {
            WriteLineC64("You take a seat and watch rain smear the window into silver.");
        }
        else
        {
            WriteLineC64("You sit for a moment, listening to the rain against the platform tiles.");
        }
        continue;
    }

    if (trimmed.Is("board"))
    {
        if (!state.IsCurrentRoomId("platform"))
        {
            WriteLineC64("You are already on board the train.");
            continue;
        }

        if (state.Inventory.FindItem("ticket") is null)
        {
            WriteLineC64("You need a ticket to board.");
            continue;
        }

        if (state.Move(Direction.In))
        {
            WriteLineC64("You board the train. The city blurs into neon.");
            ShowRoom();
        }
        else
        {
            WriteLineC64(state.LastMoveError ?? "You cannot go that way.");
        }
        continue;
    }

    ICommand command = parser.Parse(trimmed);
    CommandResult result = state.Execute(command);

    DisplayResult(command, result);

    if (result.ShouldQuit)
        break;
}

void DisplayResult(ICommand command, CommandResult result)
{
    if (command is LookCommand)
    {
        ShowLookResult(result);
        return;
    }

    if (!string.IsNullOrWhiteSpace(result.Message))
        WriteLineC64(result.Message);

    foreach (string reaction in result.ReactionsList.Where(r => !string.IsNullOrWhiteSpace(r)))
        WriteLineC64($"> {reaction}");

    if (command is GoCommand && result.Success && !result.ShouldQuit)
        ShowRoom();
}

void ShowRoom()
{
    ILocation location = state.CurrentLocation;
    WriteLineC64();
    WriteLineC64($"Room: {location.Id.ToProperCase()}");
    WriteLineC64(location.GetDescription());

    string items = location.Items.CommaJoinNames(properCase: true);
    WriteLineC64(string.IsNullOrWhiteSpace(items) ? "Items here: None" : $"Items here: {items}");

    List<string> exits = location.Exits
        .Select(exit => FormatExit(exit.Key, exit.Value))
        .ToList();

    WriteLineC64(exits.Count > 0 ? $"Exits: {exits.CommaJoin()}" : "Exits: None");
}

void ShowLookResult(CommandResult result)
{
    WriteLineC64($"Room: {state.CurrentLocation.Id.ToProperCase()}");
    DisplayResult(new UnknownCommand(), result);
}

string FormatExit(Direction direction, Exit exit)
{
    string directionName = direction.ToString().ToLowerInvariant().ToProperCase();
    return exit.Door == null
        ? directionName
        : $"{directionName} ({exit.Door.Name.ToProperCase()}, {exit.Door.State.ToString().ToProperCase()})";
}
```
