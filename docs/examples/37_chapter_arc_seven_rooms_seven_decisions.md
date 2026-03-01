# Seven Rooms, Seven Decisions
_Slice tag: Slice 37 — Chapter Arc (Creepypasta style, British English)._


    ## Premise
    Each room in the corridor demands a choice. By the seventh you no longer remember why you entered the first.

    ## Arc structure
    - Chapters 1–7 → Each room demands a decision.
- Branching → Choices alter the corridor.
- Convergence → The final door tests them all.

    ## Story beats (max ~8 steps)
1) The disturbance arrives and feels personal.
2) A rule is broken or a boundary is crossed.
3) A clue reveals the scale of the problem.
4) A choice narrows the world.
5) The environment answers back.
6) A truth is forced into view.
7) A price is paid, willingly or not.
8) The ending leaves a lingering echo.

    ## Map (rough layout)

```
          N
    W           E
          S

┌────────────┐
│   Room 3   │
│            │
└─────┬──────┘
      │
┌────────────┐
│   Room 2   │
│            │
└─────┬──────┘
      │
┌────────────┐
│   Room 1   │
│            │
└────────────┘
```

    ## Example (detailed setup)

```csharp
using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;

var room1 = new Location("room1", "Room one waits with a single choice.");
var room2 = new Location("room2", "Room two repeats the question differently.");
var room3 = new Location("room3", "Room three smells of candle smoke.");

room1.AddExit(Direction.North, room2);
room2.AddExit(Direction.North, room3);

var state = new GameState(room1, worldLocations: new[] { room1, room2, room3 });
var parser = new KeywordParser(KeywordParserConfig.Default);

while (true)
{
    Console.Write("
> ");
    var input = Console.ReadLine()?.Trim();
    if (string.IsNullOrWhiteSpace(input)) continue;

    if (string.Equals(input, "chapter", StringComparison.OrdinalIgnoreCase))
    {
        Console.WriteLine($"Chapter: {state.WorldState.GetCounter("chapter") + 1}");
        continue;
    }

    var command = parser.Parse(input);
    var result = state.Execute(command);

    if (!string.IsNullOrWhiteSpace(result.Message))
    {
        Console.WriteLine(result.Message);
    }

    foreach (var reaction in result.ReactionsList)
    {
        if (!string.IsNullOrWhiteSpace(reaction))
        {
            Console.WriteLine($"> {reaction}");
        }
    }

    if (result.ShouldQuit) break;

    if (state.IsCurrentRoomId("room2"))
    {
        state.WorldState.SetFlag("chapter1_done", true);
        state.WorldState.SetRelationship("chapter", 1);
    }

    if (state.IsCurrentRoomId("room3"))
    {
        state.WorldState.SetFlag("chapter2_done", true);
        state.WorldState.Increment("chapter");
        Console.WriteLine("The corridor converges. One more decision remains.");
        break;
    }
}
```
