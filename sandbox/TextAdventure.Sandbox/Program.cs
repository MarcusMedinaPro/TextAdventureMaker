// <copyright file="Program.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
using System;
using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;

// Slice 5: NPCs + Dialog + Movement (dialog loop)

var classroom = new Location("classroom", "Empty desks, a chalkboard, and a heavy silence.");
var chalkboardDrawings = new[]
{
    "You drew a cat.",
    "You drew a smilie.",
    "You wrote 1337! on the chalkboard.",
    "You drew a heart."
};
var chalkboardLook = new[]
{
    "There's a cat drawn on it.",
    "There's a smilie drawn on it.",
    "Leet! is written on it.",
    "There's a heart drawn on it."
};
string? chalkboardDrawing = null;
string? chalkboardLookText = null;

var student = new Npc("student", "student")
    .Description("A student sits in the back, staring at the floor.")
    .SetDialog(new DialogNode("We heard something in the hallway...")
        .AddOption("Ask what they heard.", new DialogNode("Footsteps. Slow, heavy. Then a door."))
        .AddOption("Ask where everyone went.", new DialogNode("They left in a hurry. No one looked back.")));

classroom.AddNpc(student);

var chalkboard = new Item("chalkboard", "chalkboard", "A dusty chalkboard waits at the front.")
    .AddAliases("board", "chalk", "blackboard")
    .SetTakeable(false);
classroom.AddItem(chalkboard);

var state = new GameState(classroom, worldLocations: new[] { classroom });
var parser = new KeywordParser(KeywordParserConfigBuilder.BritishDefaults().Build());

Console.WriteLine("=== THE SILENT CLASSROOM (Slice 5) ===");
Console.WriteLine("Commands: Look, Talk <Npc>, Inventory, Quit");

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

    if (state.CurrentLocation.FindNpc("student") != null)
    {
        Console.WriteLine("The student is here, sitting quietly.");
    }
}

void RunDialog(MarcusMedina.TextAdventure.Interfaces.INpc npc)
{
    var root = npc.DialogRoot;
    if (root == null)
    {
        Console.WriteLine("They have nothing to say.");
        return;
    }

    var showIntro = true;
    while (true)
    {
        if (showIntro)
        {
            Console.WriteLine(root.Text);
            showIntro = false;
        }
        for (var i = 0; i < root.Options.Count; i++)
        {
            Console.WriteLine($"{i + 1}) {root.Options[i].Text}");
        }

        Console.WriteLine($"{root.Options.Count + 1}) OK thanks, bye.");
        Console.Write("> ");
        var input = Console.ReadLine()?.Trim();
        if (!int.TryParse(input, out var choice))
        {
            Console.WriteLine("Pick a number.");
            continue;
        }

        if (choice == root.Options.Count + 1)
        {
            Console.WriteLine("You leave the student alone.");
            break;
        }

        if (choice < 1 || choice > root.Options.Count)
        {
            Console.WriteLine("Pick a valid option.");
            continue;
        }

        var reply = root.Options[choice - 1].Next;
        Console.WriteLine(reply?.Text ?? "They say nothing more.");
    }
}

ShowLookResult(state.Look());

while (true)
{
    Console.Write("> ");
    var input = Console.ReadLine()?.Trim();
    if (string.IsNullOrWhiteSpace(input)) continue;

    if (input.Equals("sit", StringComparison.OrdinalIgnoreCase) ||
        input.StartsWith("sit ", StringComparison.OrdinalIgnoreCase))
    {
        Console.WriteLine("You sit by the desk and let the silence settle.");
        continue;
    }

    if (input.Equals("listen", StringComparison.OrdinalIgnoreCase))
    {
        Console.WriteLine("You hear the old radiator tick and the faint hum of lights.");
        continue;
    }

    if (input.Equals("draw on chalkboard", StringComparison.OrdinalIgnoreCase) ||
        input.Equals("draw on board", StringComparison.OrdinalIgnoreCase) ||
        input.Equals("draw on blackboard", StringComparison.OrdinalIgnoreCase))
    {
        if (chalkboardDrawing != null)
        {
            Console.WriteLine("You erase the chalkboard first.");
        }

        var index = Random.Shared.Next(chalkboardDrawings.Length);
        chalkboardDrawing = chalkboardDrawings[index];
        chalkboardLookText = chalkboardLook[index];
        Console.WriteLine(chalkboardDrawing);
        continue;
    }

    if (input.Equals("clear chalkboard", StringComparison.OrdinalIgnoreCase) ||
        input.Equals("clear board", StringComparison.OrdinalIgnoreCase) ||
        input.Equals("clear blackboard", StringComparison.OrdinalIgnoreCase) ||
        input.Equals("erase chalkboard", StringComparison.OrdinalIgnoreCase) ||
        input.Equals("erase board", StringComparison.OrdinalIgnoreCase) ||
        input.Equals("erase blackboard", StringComparison.OrdinalIgnoreCase))
    {
        if (chalkboardDrawing == null)
        {
            Console.WriteLine("It's already clean.");
        }
        else
        {
            chalkboardDrawing = null;
            chalkboardLookText = null;
            Console.WriteLine("You erase the chalkboard.");
        }
        continue;
    }

    if (input.Equals("look at chalkboard", StringComparison.OrdinalIgnoreCase) ||
        input.Equals("look at board", StringComparison.OrdinalIgnoreCase) ||
        input.Equals("look at blackboard", StringComparison.OrdinalIgnoreCase) ||
        input.Equals("look chalkboard", StringComparison.OrdinalIgnoreCase) ||
        input.Equals("look board", StringComparison.OrdinalIgnoreCase) ||
        input.Equals("look blackboard", StringComparison.OrdinalIgnoreCase))
    {
        if (chalkboardLookText == null)
        {
            Console.WriteLine("The chalkboard is clean.");
        }
        else
        {
            Console.WriteLine(chalkboardLookText);
        }
        continue;
    }

    var command = parser.Parse(input);
    if (command is TalkCommand talk && talk.Target != null && talk.Target.TextCompare("student"))
    {
        RunDialog(student);
        continue;
    }

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
