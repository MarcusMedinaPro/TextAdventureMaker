using System;
using System.Collections.Generic;
using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;

// Slice 5: The Silent Classroom
// Tests: NPC dialog, movement, auto-look after go, talk command.

var hallway = new Location("hallway", "A narrow corridor lined with lockers and a closed classroom door.");
var classroom = new Location("classroom", "A quiet classroom with polished desks and a chalkboard half-erased.");

var curiosityNode = new DialogNode("The student nods once and whispers about footsteps in the corridor.")
    .AddOption("Ask what they heard.", new DialogNode("Slow footsteps, then a door sighing open, then silence again."))
    .AddOption("Ask why they stare at the floor.", new DialogNode("They say the floor is where promises are made and forgotten."));
var offerComfortNode = new DialogNode("They crack a small smile and say the silence feels like a warm drink.")
    .AddOption("Offer to share a joke.", new DialogNode("The smile lingers, and they murmur, 'Keep the jokes coming.'"));

var student = new Npc("student", "silent student")
    .Description("A student sits in the back, voice barely above a breath.")
    .SetDialog(new DialogNode("They raise their eyes when you enter and gesture for you to speak.")
        .AddOption("Ask what keeps them here.", curiosityNode)
        .AddOption("Offer some company.", offerComfortNode));

classroom.AddNpc(student);
hallway.AddExit(Direction.East, classroom);
classroom.AddExit(Direction.West, hallway);

var state = new GameState(hallway, worldLocations: new[] { hallway, classroom })
{
    EnableFuzzyMatching = true,
    FuzzyMaxDistance = 1
};

var parser = new KeywordParser(KeywordParserConfigBuilder.BritishDefaults()
    .WithLook("look", "l")
    .WithExamine("examine", "x")
    .WithGo("go", "move")
    .WithTalk("talk", "speak")
    .WithFuzzyMatching(true, 1)
    .Build());

Console.WriteLine("=== THE SILENT CLASSROOM (Slice 5) ===");
Console.WriteLine("Goal: reach the classroom, speak with the student, and return with fresh whispers.");
Console.WriteLine("Commands: look, examine, talk student, 1-3 in dialog, go east/west, leave, quit.");
ShowRoom();

while (true)
{
    Console.Write("\n> ");
    var input = Console.ReadLine()?.Trim();
    if (string.IsNullOrWhiteSpace(input)) continue;

    if (input.Is("leave"))
    {
        Console.WriteLine("You leave the whispers for another day.");
        break;
    }

    var command = parser.Parse(input);

    if (command is TalkCommand talk && TryFindNpc(talk.Target, out var npc))
    {
        RunDialog(npc!);
        continue;
    }

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

    if (result.ShouldQuit)
    {
        break;
    }
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

    var exits = location.Exits
        .Select(exit => exit.Key.ToString().ToLowerInvariant().ToProperCase())
        .ToList();

    Console.WriteLine(location.Npcs.Count > 0
        ? $"People present: {location.Npcs.Count}"
        : "People present: None");
    Console.WriteLine(exits.Count > 0 ? $"Exits: {exits.CommaJoin()}" : "Exits: None");
}

void ShowLookResult(CommandResult result)
{
    Console.WriteLine($"Room: {state.CurrentLocation.Id.ToProperCase()}");
    WriteResult(result);
}

bool TryFindNpc(string? target, out INpc? npc)
{
    npc = null;
    if (string.IsNullOrWhiteSpace(target)) return false;
    npc = state.CurrentLocation.FindNpc(target);
    return npc != null;
}

void RunDialog(INpc npc)
{
    var node = npc.DialogRoot;
    if (node == null)
    {
        Console.WriteLine("They have nothing to say.");
        return;
    }

    while (true)
    {
        Console.WriteLine();
        Console.WriteLine(node.Text);

        if (node.Options.Count == 0)
        {
            Console.WriteLine("There is nothing more to ask.");
            break;
        }

        for (var i = 0; i < node.Options.Count; i++)
        {
            Console.WriteLine($"{i + 1}) {node.Options[i].Text}");
        }

        Console.WriteLine($"{node.Options.Count + 1}) Politely step away.");
        Console.Write("> ");
        var choice = Console.ReadLine()?.Trim();
        if (!int.TryParse(choice, out var selection))
        {
            Console.WriteLine("Pick one of the numbers.");
            continue;
        }

        if (selection == node.Options.Count + 1)
        {
            Console.WriteLine("You step back and let the silence settle.");
            break;
        }

        if (selection < 1 || selection > node.Options.Count)
        {
            Console.WriteLine("That is not a valid option.");
            continue;
        }

        node = node.Options[selection - 1].Next;
        if (node == null)
        {
            Console.WriteLine("They do not answer further.");
            break;
        }
    }
}
