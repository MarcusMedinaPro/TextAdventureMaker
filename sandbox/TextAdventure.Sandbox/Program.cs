// <copyright file="Program.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
using System;
using System.Collections.Generic;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Helpers;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Parsing;
using MarcusMedina.TextAdventure.Interfaces;

// Slice 1 + 2 + 3: Location + Navigation + Doors + Keys + Parser

var hallway = new Location("hallway", "It's dark. You can't see anything.");
var basement = new Location("basement", "Cold air and old stone. It smells of dust.");

var key = new Key("basement_key", "basement key", "A small key with a stamped B.")
    .AddAliases("key", "basement")
    .SetWeight(0.02f);

var flashlight = new Item("flashlight", "flashlight", "A small torch with a stiff switch.")
    .AddAliases("torch", "light", "flash", "lamp")
    .SetWeight(0.2f)
    .SetReaction(ItemAction.Use, "Click. A clean circle of light blooms in the dark.");

// The key is hidden until the flashlight is used.

var basementDoor = new Door("basement_door", "basement door", "A solid door with a heavy latch.")
    .RequiresKey(key)
    .SetReaction(DoorAction.Unlock, "The lock gives way with a dry click.")
    .SetReaction(DoorAction.Open, "The door creaks open.")
    .SetReaction(DoorAction.OpenFailed, "It will not move.");

hallway.AddExit(Direction.Down, basement, basementDoor);

var state = new GameState(hallway, worldLocations: new[] { hallway, basement });
state.Inventory.Add(flashlight);
var parserConfig = new KeywordParserConfig(
    quit: CommandHelper.NewCommands("quit", "exit", "q"),
    look: CommandHelper.NewCommands("look", "l", "ls"),
    inventory: CommandHelper.NewCommands("inventory", "inv", "i"),
    stats: CommandHelper.NewCommands("stats", "stat", "hp", "health"),
    open: CommandHelper.NewCommands("open"),
    unlock: CommandHelper.NewCommands("unlock"),
    take: CommandHelper.NewCommands("take", "get", "pickup", "pick"),
    drop: CommandHelper.NewCommands("drop"),
    use: CommandHelper.NewCommands("use", "turn", "switch", "light"),
    combine: CommandHelper.NewCommands("combine", "mix"),
    pour: CommandHelper.NewCommands("pour"),
    go: CommandHelper.NewCommands("go", "move"),
    read: CommandHelper.NewCommands("read"),
    talk: CommandHelper.NewCommands("talk", "speak"),
    attack: CommandHelper.NewCommands("attack", "fight"),
    flee: CommandHelper.NewCommands("flee", "run"),
    save: CommandHelper.NewCommands("save"),
    load: CommandHelper.NewCommands("load"),
    all: CommandHelper.NewCommands("all"),
    ignoreItemTokens: CommandHelper.NewCommands("up", "to", "on", "off"),
    combineSeparators: CommandHelper.NewCommands("and", "+"),
    pourPrepositions: CommandHelper.NewCommands("into", "in"),
    directionAliases: new Dictionary<string, Direction>(StringComparer.OrdinalIgnoreCase)
    {
        ["n"] = Direction.North,
        ["s"] = Direction.South,
        ["e"] = Direction.East,
        ["w"] = Direction.West,
        ["u"] = Direction.Up,
        ["d"] = Direction.Down,
        ["in"] = Direction.In,
        ["out"] = Direction.Out
    },
    allowDirectionEnumNames: true);
var parser = new KeywordParser(parserConfig);

Console.WriteLine("=== LIGHT IN THE BASEMENT (Slice 3) ===");
Console.WriteLine("Commands: Look, Take <Item>, Unlock/Open Door, Go Down, Use/Turn On/Off Flashlight, Inventory, Quit");

bool IsFlashlightCommand(ICommand command)
{
    if (command is not UseCommand use) return false;
    var item = state.Inventory.FindItem(use.ItemName);
    return item != null && item.Id.TextCompare("flashlight");
}

bool WantsOff(string input)
{
    return input.Contains("off", StringComparison.OrdinalIgnoreCase);
}

void RevealKey()
{
    if (hallway.Items.Contains(key)) return;
    hallway.AddItem(key);
    Console.WriteLine("> The brass key glints under the table.");
}

bool HandleFlashlightUse(string input, ICommand command)
{
    if (!IsFlashlightCommand(command)) return false;

    var lightOn = state.WorldState.GetFlag("flashlight_on");
    if (WantsOff(input))
    {
        if (lightOn)
        {
            state.WorldState.SetFlag("flashlight_on", false);
            Console.WriteLine("The light goes out.");
        }
        else
        {
            Console.WriteLine("It's already off.");
        }
        return true;
    }

    if (!lightOn)
    {
        state.WorldState.SetFlag("flashlight_on", true);
    }

    RevealKey();
    return false;
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

void ShowDark()
{
    Console.WriteLine("It's dark. You can't see anything.");
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

    var command = parser.Parse(input);
    var result = state.Execute(command);

    var lightOn = state.WorldState.GetFlag("flashlight_on");

    if (command is LookCommand && !lightOn)
    {
        ShowDark();
    }
    else
    {
        if (command is LookCommand)
        {
            ShowLookResult(result);
        }
        else WriteResult(result);
    }

    if (HandleFlashlightUse(input, command)) continue;

    if (result.ShouldQuit) break;
}
