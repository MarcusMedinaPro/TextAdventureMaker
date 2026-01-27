// <copyright file="Program.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
using System;
using System.Collections.Generic;
using System.Linq;
using MarcusMedina.TextAdventure.Builders;
using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;

// Slice 6: Event System (Observer) using all previous slices

var garden = new Location(
    "garden",
    "A secluded English garden, hushed and proper, where the air smells faintly of rain and roses. A broad stone rests by a tall wrought-iron gate, as if guarding a small secret.");

var lane = new Location(
    "lane",
    "A narrow lane of damp brick and clinging ivy, intimate and shadowed, like a passage meant for quiet footsteps and lingering glances.");

var ivy = new Item(
        "ivy",
        "ivy",
        "Dark, glossy leaves trail along the wall, elegant but faintly dangerous to the touch.")
    .AddAliases("ivy", "vines")
    .HideFromItemList();

var brick = new Item(
        "brick",
        "brick",
        "A solid pink brick, cool and reassuringly heavy, smelling of old rain and time.")
    .AddAliases("brick", "bricks")
    .HideFromItemList();

var stone = new Item(
        "stone",
        "stone",
        "A wide, flat stone, properly weighty, as though it enjoys being leaned upon.")
    .AddAliases("rock", "slab")
    .SetTakeable(false)
    .Description("The surface is smooth from years of weather and hands resting there, perhaps pausing longer than strictly necessary.");

var key = KeyBuilder.Create(
        "gate_key",
        "iron key",
        "A discreet iron key hidden away beneath the stone, waiting patiently to be discovered.")
    .AddAliases("key", "iron")
    .Description("An old-fashioned iron key with a graceful bow and worn teeth, the sort that promises secrets behind a gate.")
    .Build();

garden.AddItem(stone);

var gate = DoorBuilder.Create(
        "gate",
        "gate",
        "A tall wrought-iron gate, elegant and firmly locked, its bars cold and unyielding.")
    .AddAliases("door")
    .RequiresKey(key)
    .Description("The gate stands between you and the lane beyond, proper and unbudging until persuaded by the right key.")
    .SetReaction(DoorAction.Unlock, "The lock answers with a soft, intimate click, as if acknowledging a familiar touch.")
    .SetReaction(DoorAction.Open, "The gate swings open with a slow, graceful creak, like an old lady clearing her throat.")
    .Build();

garden.AddExit(Direction.North, lane, gate);
lane.AddItem(ivy);
lane.AddItem(brick);

var state = new GameState(garden, worldLocations: new[] { garden, lane });
state.ShowItemsListOnlyWhenThereAreActuallyThingsToInteractWith = true;
state.ShowDirectionsWhenThereAreDirectionsVisibleOnly = true;
var isKeyRevealed = false;

state.Events.Subscribe(GameEventType.PickupItem, e =>
{
    if (e.Item != null && e.Item.Id == "stone")
    {
        if (!isKeyRevealed)
        {
            isKeyRevealed = true;
            garden.AddItem(key);
            Console.WriteLine("With a quiet effort, you lift the stone. Something cool and metallic waits beneath: an iron key.");
        }
    }
});

var parserConfig = KeywordParserConfigBuilder.BritishDefaults()
    .WithLook("look", "l")
    .WithInventory("inventory", "inv", "i")
    .WithTake("take", "get")
    .WithDrop("drop")
    .WithUse("use")
    .WithGo("go", "move")
    .WithUnlock("unlock")
    .WithOpen("open")
    .WithIgnoreItemTokens("on", "off")
    .WithDirectionAliases(new Dictionary<string, Direction>(StringComparer.OrdinalIgnoreCase)
    {
        ["n"] = Direction.North,
        ["north"] = Direction.North
    })
    .Build();
var parser = new KeywordParser(parserConfig);

Console.WriteLine("=== THE KEY UNDER THE STONE ===");
Console.WriteLine("Type 'help' to see commands.");

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
    Console.WriteLine();
    Console.WriteLine(new string('-', 60));
    Console.WriteLine($"You are in the {state.CurrentLocation.Id.ToProperCase()}");
    Console.WriteLine();

    var lines = result.Message?
        .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
        .ToList()
        ?? new List<string>();

    var description = lines.FirstOrDefault() ?? state.CurrentLocation.GetDescription();
    if (!string.IsNullOrWhiteSpace(description))
    {
        Console.WriteLine(description);
        Console.WriteLine();
    }

    var itemsLine = lines.FirstOrDefault(line => line.StartsWith("Items here:", StringComparison.OrdinalIgnoreCase));
    if (!string.IsNullOrWhiteSpace(itemsLine))
    {
        var items = itemsLine.Replace("Items here:", "").Trim();
        Console.WriteLine(items.Length > 0 ? $"You notice {items}" : "You notice nothing in particular.");
        Console.WriteLine();
    }

    var exitsLine = lines.FirstOrDefault(line => line.StartsWith("Exits:", StringComparison.OrdinalIgnoreCase));
    if (!string.IsNullOrWhiteSpace(exitsLine))
    {
        Console.WriteLine(exitsLine.Replace("Exits:", "Exits:"));
        Console.WriteLine();
    }

    Console.WriteLine("Hints");
    Console.WriteLine("- move stone / take stone");
    Console.WriteLine("- unlock gate / open gate");
    Console.WriteLine("- go north");
    Console.WriteLine("- inventory");
    Console.WriteLine(new string('-', 60));
}

ShowLookResult(state.Look());

while (true)
{
    Console.Write("> ");
    var input = Console.ReadLine()?.Trim();
    if (string.IsNullOrWhiteSpace(input)) continue;

    if (input.Equals("help", StringComparison.OrdinalIgnoreCase) ||
        input.Equals("halp", StringComparison.OrdinalIgnoreCase) ||
        input == "?")
    {
        Console.WriteLine("Commands: look, move/take stone, unlock/open gate, go north, inventory, quit");
        continue;
    }

    if (input.Contains("stone", StringComparison.OrdinalIgnoreCase) &&
        (input.StartsWith("move ", StringComparison.OrdinalIgnoreCase) ||
         input.StartsWith("push ", StringComparison.OrdinalIgnoreCase) ||
         input.StartsWith("shift ", StringComparison.OrdinalIgnoreCase) ||
         input.StartsWith("lift ", StringComparison.OrdinalIgnoreCase)))
    {
        if (!isKeyRevealed)
        {
            isKeyRevealed = true;
            garden.AddItem(key);
            Console.WriteLine("You shift the stone aside with a soft scrape. Beneath it lies a small iron key, cool and promising.");
        }
        else
        {
            Console.WriteLine("The stone has already been moved; it has no more secrets to offer.");
        }
        continue;
    }

    var command = parser.Parse(input);
    var result = state.Execute(command);

    if (command is LookCommand look && !string.IsNullOrWhiteSpace(look.Target))
    {
        WriteResult(result);
    }
    else if (command is LookCommand)
    {
        ShowLookResult(result);
    }
    else
    {
        WriteResult(result);
    }

    if (result.ShouldQuit) break;
}
