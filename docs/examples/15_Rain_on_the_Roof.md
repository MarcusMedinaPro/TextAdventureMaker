# Rain on the Roof

_Slice tag: Slice 15 — Combat as Struggle + Fluent setup. Demo focuses on a simple endurance encounter._

## Story beats (max ~10 steps)
1) Rain leaks through the roof.
2) A bucket is nearby.
3) The leak worsens.
4) Endure until it passes.

## Map (rough layout)
```
          N
    W           E
          S

┌────────────┐
│   Attic    │
│  B, Storm  │
└────────────┘

B = Bucket
Storm = NPC
```

## Example (fluent combat loop)
```csharp
using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Helpers;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;
using static MarcusMedina.TextAdventure.Extensions.ConsoleExtensions;

// Slice 15 — Pathfinder
// Tests:
// - HintCommand route guidance
// - A* pathfinder between rooms

Location attic = (id: "attic", description: "Rain drums against the roof. A leak gathers overhead.");
Location landing = (id: "landing", description: "A narrow landing with cold air from the stairs.");
Location hallway = (id: "hallway", description: "A quiet hallway with a glint of light from below.");
Location kitchen = (id: "kitchen", description: "The warm kitchen hums. A kettle ticks.");

attic.AddExit(Direction.Down, landing);
landing.AddExit(Direction.Up, attic);
landing.AddExit(Direction.Down, hallway);
hallway.AddExit(Direction.Up, landing);
hallway.AddExit(Direction.Down, kitchen);
kitchen.AddExit(Direction.Up, hallway);

GameState state = new(attic, worldLocations: [attic, landing, hallway, kitchen]);
state.SetPathfinder(new AStarPathfinder());

KeywordParser parser = new(KeywordParserConfigBuilder.BritishDefaults()
    .WithGo("go", "move")
    .WithHint("hint", "path")
    .Build());

SetupC64("Rain on the Roof - Pathfinder Sandbox");
WriteLineC64("=== RAIN ON THE ROOF (Slice 15) ===");
WriteLineC64("Goal: ask for a route to the kitchen, then follow it.");
WriteLineC64("Commands: hint kitchen, go down, go up, look, quit.");
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

    ICommand command = parser.Parse(trimmed);
    CommandResult result = state.Execute(command);

    DisplayResult(command, result);
    if (result.ShouldQuit)
        break;
}

void DisplayResult(ICommand command, CommandResult result)
{
    if (!string.IsNullOrWhiteSpace(result.Message))
        WriteLineC64(result.Message);

    foreach (string reaction in result.ReactionsList.Where(r => !string.IsNullOrWhiteSpace(r)))
        WriteLineC64($"> {reaction}");

    if (command is GoCommand && result.Success)
        ShowRoom();
}

void ShowRoom()
{
    ILocation location = state.CurrentLocation;
    WriteLineC64();
    WriteLineC64($"Room: {location.Id.ToProperCase()}");
    WriteLineC64(location.GetDescription());
    List<string> exits = location.Exits.Keys
        .Select(dir => dir.ToString().ToLowerInvariant().ToProperCase())
        .ToList();
    WriteLineC64(exits.Count > 0 ? $"Exits: {exits.CommaJoin()}" : "Exits: None");
}
```
