# Morning Ritual

_Slice tag: Slice 1 — Location + Navigation. Demo focuses on moving between rooms in a simple routine._

A tiny, quiet demo about waking up, making coffee, finding the newspaper, and reading in the sun.

## Map (rough layout)

```
                          N
                    W           E
                          S

┌────────────┐     ┌────────────┐    ┌────────────┐
│            │     │            │    │            │
│  Bedroom   │─────│  Hallway   │────│  Kitchen   │
│            │     │            │    │            │
│            │     │            │    │     K      │
└────────────┘     └────────────┘    └─────┬──────┘
                                           │
                                           │
                                    ┌──────┴──────┐
                                    │             │
                                    │ Living Room │
                                    │             │
                                    │      C      │
                                    └─────────────┘

K = Kettle (not interactable in this slice)
C = Couch (not interactable in this slice)

```

## Slice 1 functions tested

- `Location(id, description)`
- `Location.AddExit(direction, target, oneWay: false)` (auto-creates the opposite exit)
- `Location.Exits`
- `GameState(startLocation, worldLocations)`
- `GameState.Move(direction)`
- `GameState.CurrentLocation`
- `Direction` enum

## Demo commands (manual)

- `look` — Show current room description
- `north`/`n`, `south`/`s`, `east`/`e`, `west`/`w` — Move
- `quit` — Exit the demo

## Story beats (max ~10 steps)

1. Wake up in the bedroom.
2. Step into the hallway.
3. Make coffee in the kitchen.
4. Go to the living room.

## Example (core engine + navigation only)

```csharp
using System;
using System.Linq;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;

// Locations
var bedroom = new Location("bedroom", "Soft morning light spills across the room.");
var hallway = new Location("hallway", "A quiet corridor with a small runner rug.");
var kitchen = new Location("kitchen", "A kettle hums quietly on the counter.");
var livingRoom = new Location("living_room", "A couch by the window invites you to sit.");

bedroom.AddExit(Direction.East, hallway);
hallway.AddExit(Direction.East, kitchen);
kitchen.AddExit(Direction.South, livingRoom);

// Game state
var state = new GameState(bedroom, worldLocations: new[] { bedroom, hallway, kitchen, livingRoom });

void ShowRoom()
{
    var location = state.CurrentLocation;
    Console.WriteLine($"\nRoom: {location.Id.ToProperCase()}");
    Console.WriteLine(location.GetDescription());
    var exits = location.Exits.Keys
        .Select(direction => direction.ToString().ToLowerInvariant().ToProperCase());
    Console.WriteLine($"Exits: {string.Join(", ", exits)}");
}

// Run loop (manual commands, no parser)
ShowRoom();
while (true)
{
    Console.Write("\n> ");
    var input = Console.ReadLine()?.Trim().ToLowerInvariant();
    if (string.IsNullOrWhiteSpace(input)) continue;

    if (input == "quit")
    {
        break;
    }

    if (input == "look")
    {
        ShowRoom();
        continue;
    }

    var moved = input switch
    {
        "north" or "n" => state.Move(Direction.North),
        "south" or "s" => state.Move(Direction.South),
        "east" or "e" => state.Move(Direction.East),
        "west" or "w" => state.Move(Direction.West),
        _ => false
    };

    if (moved)
    {
        ShowRoom();
    }
    else if (input is "north" or "n" or "south" or "s" or "east" or "e" or "west" or "w")
    {
        Console.WriteLine(state.LastMoveError ?? "You cannot go that way.");
    }
    else
    {
        Console.WriteLine("Try: look, north/south/east/west, quit.");
    }
}
```
