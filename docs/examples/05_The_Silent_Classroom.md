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
using static MarcusMedina.TextAdventure.Extensions.ConsoleExtensions;

// Slice 5 — Rule-Based Dialog
// Tests:
// - Dialog tree options
// - Rule-based dialog selection (first meeting, memory, priority)

Location hallway = new("hallway", "A narrow corridor lined with lockers and a closed classroom door.");
Location classroom = new("classroom", "A quiet classroom with polished desks and a chalkboard half-erased.");

var curiosityNode = new DialogNode("The student nods once and whispers about footsteps in the corridor.")
    .AddOption("Ask what they heard.", new DialogNode("Slow footsteps, then a door sighing open, then silence again."))
    .AddOption("Ask why they stare at the floor.", new DialogNode("They say the floor is where promises are made and forgotten."));
var offerComfortNode = new DialogNode("They crack a small smile and say the silence feels like a warm drink.")
    .AddOption("Offer to share a joke.", new DialogNode("The smile lingers, and they murmur, 'Keep the jokes coming.'"));

Npc student = new("student", "silent student")
    .Description("A student sits in the back, voice barely above a breath.")
    .SetDialog(new DialogNode("They raise their eyes when you enter and gesture for you to speak.")
        .AddOption("Ask what keeps them here.", curiosityNode)
        .AddOption("Offer some company.", offerComfortNode));

student.AddDialogRule("first_greeting")
    .When(ctx => !ctx.NpcMemory.HasSaid("first_greeting"))
    .Say("The student nods once. \"You are early,\" they say.")
    .Then(ctx => ctx.NpcMemory.MarkSaid("first_greeting"));

student.AddDialogRule("promise_memory")
    .When(ctx => ctx.NpcMemory.HasSaid("first_greeting"))
    .When(ctx => ctx.NpcMemory.GetCounter("promise") < 1)
    .Say("They glance at the floor. \"Promises live down there.\"")
    .Then(ctx => ctx.NpcMemory.Increment("promise"));

classroom.AddNpc(student);
hallway.AddExit(Direction.East, classroom);
classroom.AddExit(Direction.West, hallway);

GameState state = new(hallway, worldLocations: [hallway, classroom])
{
    EnableFuzzyMatching = true,
    FuzzyMaxDistance = 1
};

KeywordParser parser = new(KeywordParserConfigBuilder.BritishDefaults()
    .WithLook("look", "l")
    .WithExamine("examine", "x")
    .WithGo("go", "move")
    .WithTalk("talk", "speak")
    .WithFuzzyMatching(true, 1)
    .Build());

SetupC64("THE SILENT CLASSROOM (Slice 5) - Text Adventure Sandbox");
WriteLineC64("=== THE SILENT CLASSROOM (Slice 5) ===");
WriteLineC64("Goal: reach the classroom, speak with the student, and return with fresh whispers.");
WriteLineC64("Test: first talk triggers rule-based dialog, later talks show the dialog tree.");
WriteLineC64("Commands: look, examine, talk student, 1-3 in dialog, go east/west, leave, quit.");
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

    if (trimmed.Is("leave"))
    {
        WriteLineC64("You leave the whispers for another day.");
        break;
    }

    ICommand command = parser.Parse(trimmed);

    if (command is TalkCommand { Target: var target } && TryFindNpc(target, out INpc npc))
    {
        if (TryRunRuleDialog(npc))
            continue;

        RunDialog(npc);
        continue;
    }

    CommandResult result = state.Execute(command);

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
        WriteLineC64(result.Message);
    }

    foreach (var reaction in result.ReactionsList)
    {
        if (!string.IsNullOrWhiteSpace(reaction))
        {
            WriteLineC64($"> {reaction}");
        }
    }
}

void ShowRoom()
{
    ILocation location = state.CurrentLocation;
    WriteLineC64();
    WriteLineC64($"Room: {location.Id.ToProperCase()}");
    WriteLineC64(location.GetDescription());

    List<string> exits = location.Exits
        .Select(exit => exit.Key.ToString().ToLowerInvariant().ToProperCase())
        .ToList();

    WriteLineC64(location.Npcs.Count > 0
        ? $"People present: {location.Npcs.Count}"
        : "People present: None");
    WriteLineC64(exits.Count > 0 ? $"Exits: {exits.CommaJoin()}" : "Exits: None");
}

void ShowLookResult(CommandResult result)
{
    WriteLineC64($"Room: {state.CurrentLocation.Id.ToProperCase()}");
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
        WriteLineC64("They have nothing to say.");
        return;
    }

    while (true)
    {
        WriteLineC64();
        WriteLineC64(node.Text);

        if (node.Options.Count == 0)
        {
            WriteLineC64("There is nothing more to ask.");
            break;
        }

        for (var i = 0; i < node.Options.Count; i++)
        {
            WriteLineC64($"{i + 1}) {node.Options[i].Text}");
        }

        WriteLineC64($"{node.Options.Count + 1}) Politely step away.");
        WritePromptC64("> ");
        string? choice = Console.ReadLine()?.Trim();
        if (!int.TryParse(choice, out var selection))
        {
            WriteLineC64("Pick one of the numbers.");
            continue;
        }

        if (selection == node.Options.Count + 1)
        {
            WriteLineC64("You step back and let the silence settle.");
            break;
        }

        if (selection < 1 || selection > node.Options.Count)
        {
            WriteLineC64("That is not a valid option.");
            continue;
        }

        node = node.Options[selection - 1].Next;
        if (node == null)
        {
            WriteLineC64("They do not answer further.");
            break;
        }
    }
}

bool TryRunRuleDialog(INpc npc)
{
    string? ruleText = npc.GetRuleBasedDialog(state);
    if (string.IsNullOrWhiteSpace(ruleText))
        return false;

    WriteLineC64(ruleText);
    return true;
}
```
