# The Silent Classroom

_Slice tag: Slice 5 — NPCs + Dialog + Movement. Demo focuses on talking to an NPC and a simple dialogue loop._

## Map (rough layout)
```
┌────────────┐
│ Classroom  │
│  Student   │
└────────────┘
```

## Story beats (max ~10 steps)
1) You enter an empty classroom.
2) A silent student sits in the back.
3) Talk to them.
4) Ask a few questions.
5) End the conversation politely.

## Example (NPC + dialog loop)
```csharp
using System;
using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;

var classroom = new Location("classroom", "Empty desks, a chalkboard, and a heavy silence.");

var student = new Npc("student", "student")
    .Description("A student sits in the back, staring at the floor.")
    .SetDialog(new DialogNode("We heard something in the hallway...")
        .AddOption("Ask what they heard.", new DialogNode("Footsteps. Slow, heavy. Then a door."))
        .AddOption("Ask where everyone went.", new DialogNode("They left in a hurry. No one looked back.")));

classroom.AddNpc(student);

var state = new GameState(classroom, worldLocations: new[] { classroom });
state.EnableFuzzyMatching = true;
state.FuzzyMaxDistance = 1;
var parser = new KeywordParser(KeywordParserConfigBuilder.BritishDefaults()
    .WithMove("move", "push", "shift", "lift", "slide")
    .WithFuzzyMatching(true, 1)
    .Build());

Console.WriteLine("=== THE SILENT CLASSROOM (Slice 5) ===");
Console.WriteLine("Commands: Look, Examine <Item>, Move <Item>, Talk <Npc>, Inventory, Quit");

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

void RunDialog(INpc npc)
{
    var root = npc.DialogRoot;
    if (root == null)
    {
        Console.WriteLine("They have nothing to say.");
        return;
    }

    while (true)
    {
        Console.WriteLine(root.Text);
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
```
