# The Lost Dog

_Slice tag: Slice 30 — Multi-location follow + rescue + payoff._

## Story beats (max ~10 steps)
1) Find a lost dog flyer.
2) Search the park.
3) Approach the dog.
4) Decide what to do next.

## Map (rough layout)
```
          N
    W           E
          S

┌────────────┐     ┌────────────┐
│    Park    │─────│ NightStreet│
│  F, Dog    │  E  │            │
└────────────┘     └────────────┘

F = Flyer
Dog = NPC
```

## Example (lost dog)
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

// Slice 30 — Foreshadowing
// Tests:
// - Planting, hinting, and paying off a foreshadowing tag
// - Unhinted payoff callback

Location nightStreet = (id: "night_street", description: "A long street with patchy streetlights.");
Location park = (id: "park", description: "A small park with damp grass and a lone bench.");

nightStreet.AddExit(Direction.West, park);

park.AddItem(new Item("flyer", "flyer", "LOST DOG: small terrier, answers to Pip."));

var dog = new Npc("dog", "dog")
    .SetState(NpcState.Friendly)
    .Description("A small terrier with an anxious wag.")
    .SetDialog(new DialogNode("The dog looks up, waiting.")
        .AddOption("Show the flyer")
        .AddOption("Offer a hand to sniff"));

park.AddNpc(dog);

var state = new GameState(nightStreet, worldLocations: new[] { nightStreet, park });
var parser = new KeywordParser(KeywordParserConfig.Default);

state.Foreshadowing.Link("lost_dog", "dog");

SetupC64("The Lost Dog - Text Adventure Sandbox");
WriteLineC64("=== THE LOST DOG (Slice 30) ===");
WriteLineC64("Goal: find the flyer, then approach the dog.");
WriteLineC64("Commands: look, take flyer, talk dog, go west, quit.");
ShowRoom();

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

    var command = parser.Parse(trimmed);
    var result = state.Execute(command);

    if (command is TakeCommand && state.Inventory.FindItem("flyer") != null)
    {
        state.Foreshadowing.Plant("lost_dog");
        WriteLineC64("You pocket the flyer, a small promise tucked away.");
    }

    if (command is TalkCommand { Target: var target } && target?.TextCompare("dog") == true)
    {
        if (state.Inventory.FindItem("flyer") != null)
        {
            state.Foreshadowing.Hint("lost_dog");
            state.Foreshadowing.Payoff("lost_dog", state, s =>
            {
                WriteLineC64("You recognise the dog from the flyer, even without the hint.");
            });
        }
        else
        {
            state.Foreshadowing.Payoff("lost_dog", state, s =>
            {
                WriteLineC64("You get the feeling this dog means something, but cannot place it.");
            });
        }
    }

    if (!string.IsNullOrWhiteSpace(result.Message))
        WriteLineC64(result.Message);

    foreach (var reaction in result.ReactionsList.Where(r => !string.IsNullOrWhiteSpace(r)))
        WriteLineC64($"> {reaction}");

    if (command is GoCommand && result.Success)
        ShowRoom();
}

void ShowRoom()
{
    WriteLineC64();
    WriteLineC64($"Room: {state.CurrentLocation.Id.ToProperCase()}");
    WriteLineC64(state.CurrentLocation.GetDescription());
    var exits = state.CurrentLocation.Exits.Keys
        .Select(dir => dir.ToString().ToLowerInvariant().ToProperCase())
        .ToList();
    WriteLineC64(exits.Count > 0 ? $"Exits: {exits.CommaJoin()}" : "Exits: None");
}
```
