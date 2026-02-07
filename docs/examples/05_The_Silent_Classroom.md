# The Silent Classroom

_Slice tag: Slice 5 — NPCs, Dialog & Movement. This demo shows a conversational tree, NPC metadata, and moving between connected rooms with an automatic look after every successful move._

A hushed corridor opens into a classroom where a single student keeps vigil. The space feels as quiet as the whispers emerging from the desk.

## Goal

Speak with the student, explore the quiet classroom, then return to the hallway with a story to repeat.

## Map (rough layout)

```
┌────────────┐     ┌────────────┐
│ Hallway    │────>│ Classroom  │
│            │     │            │
└────────────┘     └────────────┘
```

## Story beats (max ~10 steps)

1. You begin in the corridor outside the classroom.
2. Step inside and meet the silently watchful student.
3. Talk to the student and choose between a few carefully worded questions.
4. Immerse yourself in the replies and either ask more or step away.
5. Leave the classroom with new whispers tucked in your pocket.

## Slice 1‑5 functions tested

- `Location(id, description)`
- `Location.AddExit(direction, target, oneWay: false)`
- `Location.AddNpc(npc)`
- `Npc(id, name)`
- `Npc.Description("text")`
- `Npc.SetDialog(IDialogNode)`
- `DialogNode`, `DialogOption`
- `TalkCommand`
- `CommandResult`
- `KeywordParser`
- `KeywordParserConfigBuilder` / `KeywordParserConfig`
- `GoCommand`
- `CommandExtensions.Execute(state, command)`

## Demo commands (parser)

- `look` / `l`
- `examine <feature>` / `x <feature>`
- `talk student`
- Once in dialog, type `1`, `2`, etc. to pick an option, or the final number to step away.
- `go east` / `east`
- `go west` / `west`
- `leave`
- `quit` / `exit`

## Example (NPC dialog & movement)
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

// Console setup for C64 aesthetics
Console.BackgroundColor = ConsoleColor.DarkBlue;
Console.ForegroundColor = ConsoleColor.Cyan;
Console.Title = "THE SILENT CLASSROOM (Slice 5) - Text Adventure Sandbox";
Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.Clear();
// End console setup
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

    if (command is TalkCommand { Target: var target } && TryFindNpc(target, out var npc))
    {
        RunDialog(npc);
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

bool TryFindNpc(string? target, out INpc npc)
{
    npc = default!;
    if (string.IsNullOrWhiteSpace(target))
    {
        return false;
    }

    var found = state.CurrentLocation.FindNpc(target);
    if (found == null)
    {
        return false;
    }

    npc = found;
    return true;
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
```
