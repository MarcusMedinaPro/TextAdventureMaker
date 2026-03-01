# The Abandoned Playground

_Slice tag: Slice 37 — Atmosphere and delayed reveals._

## Story beats (max ~10 steps)
1) Enter the playground.
2) Notice the old swing.
3) Find a lost toy.
4) Decide whether to take it.

## Map (rough layout)

```
          N
    W           E
          S

┌────────────┐
│ Playground │
│   S  T     │
└────────────┘

S = Swing (fixed)
T = Stuffed toy
```

## Example (playground)
```csharp
using System;
using System.Linq;
using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;
using static MarcusMedina.TextAdventure.Extensions.ConsoleExtensions;

// Slice 37 — Chapter System
// Tests:
// - Chapter builder + objectives
// - Chapter progress UI
// - Branching objectives

Location playground = (id: "playground", description: "An abandoned playground with creaking swings.");

playground.AddItem(new Item("swing", "swing", "A swing creaks in the wind.")
    .SetTakeable(false));
playground.AddItem(new Item("toy", "stuffed toy", "A worn stuffed toy with a stitched tag."));

var state = new GameState(playground, worldLocations: new[] { playground });
var parser = new KeywordParser(KeywordParserConfig.Default);

var chapterSystem = new ChapterBuilder()
    .Chapter("arrival", "The Playground")
        .Objectives(o => o
            .Required("notice_swing")
            .Required("find_toy"))
        .OnComplete(ctx => WriteLineC64("Chapter complete: you have noticed what's left behind."))
    .Chapter("choice", "The Decision")
        .Objectives(o => o
            .Branch("take_toy", leadsTo: "ending_take")
            .Branch("leave_toy", leadsTo: "ending_leave"))
    .Chapter("ending_take", "The Keeper")
        .Objectives(o => o.Required("keep_toy"))
    .Chapter("ending_leave", "The Letting Go")
        .Objectives(o => o.Required("leave_toy_done"))
    .Build();

state.SetChapterSystem(chapterSystem);
state.Chapters.ActivateChapter("arrival");

var renderer = new ChapterProgressRenderer();

SetupC64("The Abandoned Playground - Text Adventure Sandbox");
WriteLineC64("=== THE ABANDONED PLAYGROUND (Slice 37) ===");
WriteLineC64("Commands: look, take toy, leave toy, quit.");
ShowRoom();
RenderChapter();

while (true)
{
    WriteLineC64();
    WritePromptC64("> ");
    var input = Console.ReadLine();
    if (input is null)
        break;

    var trimmed = input.Trim();
    if (string.IsNullOrWhiteSpace(trimmed))
        continue;

    if (trimmed.Is("quit") || trimmed.Is("exit"))
        break;

    if (trimmed.TextCompare("leave toy"))
    {
        state.Chapters.CompleteObjective("leave_toy");
        state.Chapters.ActivateChapter("ending_leave");
        state.Chapters.CompleteObjective("leave_toy_done");
        RenderChapter();
        continue;
    }

    var command = parser.Parse(trimmed);
    var result = state.Execute(command);

    if (command is LookCommand)
    {
        state.Chapters.CompleteObjective("notice_swing");
        ShowRoom();
        RenderChapter();
    }

    if (command is TakeCommand && state.Inventory.FindItem("toy") != null)
    {
        state.Chapters.CompleteObjective("find_toy");
        state.Chapters.ActivateChapter("choice");
        state.Chapters.CompleteObjective("take_toy");
        state.Chapters.ActivateChapter("ending_take");
        state.Chapters.CompleteObjective("keep_toy");
        RenderChapter();
    }

    if (!string.IsNullOrWhiteSpace(result.Message))
        WriteLineC64(result.Message);
}

void RenderChapter()
{
    if (state.Chapters.CurrentChapter == null)
        return;

    WriteLineC64();
    WriteLineC64(renderer.Render(state.Chapters.CurrentChapter));
}

void ShowRoom()
{
    WriteLineC64();
    WriteLineC64($"Room: {state.CurrentLocation.Id.ToProperCase()}");
    WriteLineC64(state.CurrentLocation.GetDescription());
}
```
