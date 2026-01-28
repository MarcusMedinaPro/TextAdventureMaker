# The Synonym Studio

_Slice tag: Slice 5.7 — Synonym System. Demo focuses on synonym mapping in the parser and item aliases._

## Story beats (max ~10 steps)
1) You enter a small studio with a blade on the table.
2) Try different verbs: get/grab/pickup, inspect/check, walk/head.
3) Use synonyms and watch the parser normalize your input.
4) Try a typo to see a suggestion.

## Map (rough layout)
```
          N
    W           E
          S

┌────────────┐     ┌────────────┐
│            │     │            │
│  Studio    │─────│  Hallway   │
│            │     │            │
│     B      │     │            │
└────────────┘     └────────────┘

B = Blade (aliases: sword, weapon)
```

## Example (synonyms + aliases)
```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;

// Slice 5.7 — Synonym System
// New functions tested:
// - KeywordParserConfigBuilder.AddSynonyms
// - Item.AddAliases
// - Fuzzy suggestions (Did you mean?)

var studio = new Location("studio", "A sparse studio with a single table. The air is expectant.");
var hallway = new Location("hallway", "A quiet hallway that rewards curiosity.");

studio.AddExit(Direction.East, hallway);
hallway.AddExit(Direction.West, studio);

var blade = new Item("blade", "blade", "A blade with more reputation than history.")
    .AddAliases("sword", "weapon")
    .SetReaction(ItemAction.Take, "It feels a touch theatrical.")
    .SetReaction(ItemAction.Use, "You test the weight. It promises drama.");

studio.AddItem(blade);

var state = new GameState(studio, worldLocations: new[] { studio, hallway })
{
    EnableFuzzyMatching = true,
    FuzzyMaxDistance = 1
};

var parserConfig = KeywordParserConfigBuilder.BritishDefaults()
    .WithLook("look", "l")
    .WithExamine("examine", "exam", "x")
    .WithInventory("inventory", "inv", "i")
    .WithTake("take")
    .WithUse("use")
    .WithGo("go")
    .WithQuit("quit", "exit")
    .WithIgnoreItemTokens("on", "off", "at", "the", "a")
    .AddSynonyms("take", "get", "grab", "pick", "pickup")
    .AddSynonyms("examine", "inspect", "check", "view")
    .AddSynonyms("go", "walk", "move", "head", "travel")
    .AddSynonyms("use", "swing", "brandish")
    .WithFuzzyMatching(true, 1)
    .Build();

var parser = new KeywordParser(parserConfig);

Console.WriteLine("=== THE SYNONYM STUDIO (Slice 5.7) ===");
Console.WriteLine("Goal: try synonyms like 'grab blade', 'inspect sword', 'walk east'.");
Console.WriteLine("Type 'help' for a quick list.");
ShowRoom();

while (true)
{
    Console.Write("\n> ");
    var input = Console.ReadLine()?.Trim();
    if (string.IsNullOrWhiteSpace(input)) continue;

    if (IsHelp(input))
    {
        ShowHelp();
        continue;
    }

    var command = parser.Parse(input);
    var result = state.Execute(command);

    WriteResult(result);

    if (command is GoCommand && !result.ShouldQuit)
    {
        WriteResult(state.Look());
    }

    if (result.ShouldQuit) break;
}

void ShowRoom()
{
    var location = state.CurrentLocation;
    Console.WriteLine();
    Console.WriteLine($"Room: {location.Id.ToProperCase()}");
    Console.WriteLine(location.GetDescription());

    var items = location.Items.CommaJoinNames(properCase: true);
    Console.WriteLine(string.IsNullOrWhiteSpace(items) ? "Items: None" : $"Items: {items}");

    var exits = location.Exits.Keys
        .Select(direction => direction.ToString().ToLowerInvariant().ToProperCase())
        .ToList();
    Console.WriteLine(exits.Count > 0 ? $"Exits: {exits.CommaJoin()}" : "Exits: None");
}

void WriteResult(CommandResult result)
{
    if (string.IsNullOrWhiteSpace(result.Message))
    {
        return;
    }

    var message = result.Message.Trim();
    Console.WriteLine(message);

    foreach (var reaction in result.ReactionsList)
    {
        if (string.IsNullOrWhiteSpace(reaction))
        {
            continue;
        }

        var trimmed = reaction.Trim();
        if (trimmed.Equals(message, StringComparison.OrdinalIgnoreCase))
        {
            continue;
        }

        if (trimmed.StartsWith(message, StringComparison.OrdinalIgnoreCase))
        {
            var remainder = trimmed[message.Length..].TrimStart();
            remainder = remainder.TrimStart('.', '!', '?', '-', ':', ';', ',').TrimStart();
            if (!string.IsNullOrWhiteSpace(remainder))
            {
                Console.WriteLine($"> {remainder}");
            }
            continue;
        }

        Console.WriteLine($"> {trimmed}");
    }
}

void ShowHelp()
{
    Console.WriteLine("Commands: look, examine <item>, take <item>, use <item>, go <direction>, inventory, quit");
    Console.WriteLine("Synonyms: get/grab/pickup, inspect/check, walk/head, swing/brandish");
}

bool IsHelp(string input)
{
    var normalized = input.Lower();
    return normalized is "help" or "halp" or "?";
}
```
