# TextAdventureMaker

A fluent, SOLID C# library for creating text adventures. Build interactive fiction with locations, items, NPCs, quests, combat, and more.

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![.NET](https://img.shields.io/badge/.NET-10.0-blue.svg)](https://dotnet.microsoft.com/)

## Features

- **Fluent API** - Chain methods for readable game setup
- **Bi-directional Exits** - Automatically creates return paths
- **Doors & Keys** - Lock/unlock mechanics with state management
- **Items & Inventory** - Takeable items, weight limits, containers
- **NPCs & Dialogue** - Dialog trees, NPC movement patterns, relationships
- **Combat System** - Turn-based combat with stats and flee mechanics
- **Quest System** - Multi-condition quests with progress tracking
- **Event System** - Subscribe to game events (pickup, enter room, etc.)
- **Save/Load** - JSON-based game state persistence
- **DSL Parser** - Define worlds in simple `.adventure` text files
- **Localization** - Swap UI text via JSON language files (txt deprecated)

## Quick Start

```csharp
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;

// Create locations using implicit conversion
Location entrance = (id: "entrance", description: "You stand at the forest gate.");
Location forest = (id: "forest", description: "A thick forest surrounds you.");

// Bi-directional exit (auto-creates forest -> south -> entrance)
entrance.AddExit(Direction.North, forest);

// Add items
Item key = (id: "brass_key", name: "brass key", description: "A small brass key.");
entrance.AddItem(key);

// Create and run game
var state = new GameState(entrance);
var parser = new KeywordParser(KeywordParserConfig.Default);

var game = GameBuilder.Create()
    .UseState(state)
    .UseParser(parser)
    .AddTurnStart(g => g.Output.WriteLine(g.State.Look().Message))
    .Build();

game.Run();
```

## DSL Format

Define worlds in `.adventure` files:

```
world: Dark Forest
goal: Find the key and unlock the cabin
start: entrance

location: entrance | You stand at the forest gate.
item: torch | torch | A flickering torch.
exit: north -> forest

location: forest | A thick forest surrounds you.
key: cabin_key | brass key | A small brass key.
exit: in -> cabin | door=cabin_door

door: cabin_door | cabin door | A sturdy wooden door. | key=cabin_key

location: cabin | A cozy cabin with a fireplace.
```

Load and run:

```csharp
var adventure = new AdventureDslParser().ParseFile("game.adventure");
var game = GameBuilder.Create()
    .UseState(adventure.State)
    .UseParser(new KeywordParser(KeywordParserConfig.Default))
    .Build();

game.Run();
```

## Examples

See the [docs/examples](docs/examples) folder for complete runnable demos:

| Demo | Features |
|------|----------|
| Morning Ritual | Basic navigation |
| The Locked Drawer | Doors & keys |
| Light in the Basement | Command parser |
| The Last Train Home | Items & inventory |
| The Silent Classroom | NPCs & dialogue |
| The Key Under the Stone | Event system |
| Rain on the Roof | Combat system |
| The Forgotten Password | Quest system |
| Pre-Date | World state & flags |
| The Warm Library | Save/Load |

## Installation

```bash
dotnet add package MarcusMedina.TextAdventure
```

## Project Structure

```
src/
  MarcusMedina.TextAdventure/      # Core engine
  MarcusMedina.TextAdventure.AI/   # AI command parsing (Ollama)
tests/
  MarcusMedina.TextAdventure.Tests/
sandbox/
  TextAdventure.Sandbox/           # Demo console app
docs/
  examples/                        # Runnable demo stories
  dsl/                             # DSL documentation
```

## License

MIT License - see [LICENSE](LICENSE) for details.

## Author

Marcus Ackre Medina
