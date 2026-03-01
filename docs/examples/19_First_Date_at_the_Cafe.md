# First Date at the Café

_Slice tag: Slice 19 — Dialog choices + small consequences. Demo focuses on conversational flow._

## Story beats (max ~10 steps)
1) You sit at a small table.
2) Make small talk.
3) Order coffee.
4) Decide whether to ask for a second date.

## Map (rough layout)
```
          N
    W           E
          S

┌────────────┐
│    Café    │
│  Date, M   │
└────────────┘

Date = NPC
M = Menu
```

## Example (NPC dialog)
```csharp
using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;
using static MarcusMedina.TextAdventure.Extensions.ConsoleExtensions;

// Slice 19 — Multi-Stage Quest
// Tests:
// - Multi-stage quest progression
// - Optional objectives and alternative paths
// - Failure consequence

Location cafe = (id: "cafe", description: "A warm café with soft light and a small table.");

Npc date = new("date", "date")
    .Description("A calm smile across the table.")
    .SetDialog(new DialogNode("The café is quiet. They wait for your first words.")
        .AddOption("Ask about their day")
        .AddOption("Compliment their outfit")
        .AddOption("Order coffee"));

cafe.AddNpc(date);

cafe.AddItem(new Item("menu", "menu", "A small menu with handwritten specials.").SetTakeable(false));

GameState state = new(cafe, worldLocations: [cafe]);
KeywordParser parser = new(KeywordParserConfig.Default);

MultiStageQuest quest = new("first_date", "First Date", "Steer the date through a few small moments.");
quest.AddStage("settle")
    .RequireObjective("read_menu")
    .OptionalObjective("compliment")
    .OnComplete(_ => WriteLineC64("You settle in, the menu between you."))
    .OnFailure(_ => WriteLineC64("The moment slips away. The date ends early."));

quest.AddStage("order")
    .RequireObjective("order_coffee")
    .AlternativePath("order_tea")
    .OnComplete(_ => WriteLineC64("The drinks arrive, warming the table."));

quest.AddStage("ask_out")
    .RequireObjective("ask_out")
    .AlternativePath("leave")
    .OnComplete(_ => WriteLineC64("You ask for a second date. A smile answers you."));

SetupC64("First Date at the Cafe - Text Adventure Sandbox");
WriteLineC64("=== FIRST DATE AT THE CAFE (Slice 19) ===");
WriteLineC64("Goal: progress through the quest stages without ending the date early.");
WriteLineC64("Commands: read menu, compliment, order coffee, order tea, ask out, leave, look, quit.");

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

    if (trimmed.Is("quit") || trimmed.Is("exit"))
        break;

    if (HandleQuestAction(trimmed))
        continue;

    ICommand command = parser.Parse(trimmed);
    CommandResult result = state.Execute(command);
    if (!string.IsNullOrWhiteSpace(result.Message))
        WriteLineC64(result.Message);

    foreach (string reaction in result.ReactionsList.Where(r => !string.IsNullOrWhiteSpace(r)))
        WriteLineC64($"> {reaction}");

    if (result.ShouldQuit)
        break;
}

bool HandleQuestAction(string input)
{
    QuestStage? stage = quest.CurrentStage;
    if (stage == null)
    {
        WriteLineC64("The date has reached its end.");
        return true;
    }

    if (input.TextCompare("read menu"))
        stage.CompleteObjective("read_menu");
    else if (input.TextCompare("compliment"))
        stage.CompleteObjective("compliment");
    else if (input.TextCompare("order coffee"))
        stage.CompleteObjective("order_coffee");
    else if (input.TextCompare("order tea"))
        stage.CompleteAlternative("order_tea");
    else if (input.TextCompare("ask out"))
        stage.CompleteObjective("ask_out");
    else if (input.TextCompare("leave"))
        stage.CompleteAlternative("leave");
    else if (input.TextCompare("storm out"))
    {
        stage.Fail(state);
        return true;
    }
    else
    {
        return false;
    }

    _ = quest.CheckProgress(state);
    return true;
}

void ShowRoom()
{
    WriteLineC64();
    WriteLineC64($"Room: {state.CurrentLocation.Id.ToProperCase()}");
    WriteLineC64(state.CurrentLocation.GetDescription());
    WriteLineC64("A menu and a quiet conversation wait.");
}
```
