# Map Generator

_Slice tag: Slice 43 — ASCII map rendering._

## Story beats (max ~10 steps)
1) Create a small three-room map.
2) Render the ASCII map.
3) Display it on look.

## Example (map generator)
```csharp
using System;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Tools;
using static MarcusMedina.TextAdventure.Extensions.ConsoleExtensions;

// Slice 43 — Map Generator
// Tests:
// - MapGenerator.Render from GameState

Location foyer = (id: "foyer", description: "A small foyer.");
Location hall = (id: "hall", description: "A narrow hall.");
Location attic = (id: "attic", description: "A dusty attic.");

foyer.AddExit(Direction.North, hall);
hall.AddExit(Direction.Up, attic);

var state = new GameState(foyer, worldLocations: new[] { foyer, hall, attic });

SetupC64("Map Generator - Text Adventure Sandbox");
WriteLineC64(MapGenerator.Render(state));
```
