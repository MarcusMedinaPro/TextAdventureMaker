# Validation Tools

_Slice tag: Slice 41 — Testing & validation tools._

## Story beats (max ~10 steps)
1) Build a tiny two-room map.
2) Leave a third room unreachable.
3) Run validator and explorer tools.

## Map (rough layout)
```
          N
    W           E
          S

┌────────────┐     ┌────────────┐
│  Start     │─────│  Hall      │
└────────────┘     └────────────┘

┌────────────┐
│  Hidden    │
└────────────┘
```

## Example (validator + explorer)
```csharp
using System;
using System.Linq;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;
using static MarcusMedina.TextAdventure.Extensions.ConsoleExtensions;

// Slice 41 — Validation & Testing Tools
// Tests:
// - Reachability validator
// - Random explorer walk
// - Testing mode toggle

Location start = (id: "start", description: "A simple starting room.");
Location hall = (id: "hall", description: "A corridor with a single exit.");
Location hidden = (id: "hidden", description: "A room that is never connected.");

start.AddExit(Direction.East, hall);

var state = new GameState(start, worldLocations: new[] { start, hall, hidden });
var parser = new KeywordParser(KeywordParserConfig.Default);

var game = GameBuilder.Create()
    .UseState(state)
    .UseParser(parser)
    .Build();

SetupC64("Validation Tools - Text Adventure Sandbox");

var validator = game.CreateValidator();
var unreachable = validator.FindUnreachableLocations();
WriteLineC64($"Unreachable locations: {unreachable.CommaJoin()}");

var explorer = game.CreateExplorer();
var walk = explorer.RandomWalk(maxSteps: 5);
WriteLineC64($"Random walk: {string.Join(" -> ", walk)}");

game.EnableTestingMode();
WriteLineC64($"Testing mode: {game.State.TestingModeEnabled}");
```
