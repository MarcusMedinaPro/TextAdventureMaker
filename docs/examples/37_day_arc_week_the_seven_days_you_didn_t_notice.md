# The Seven Days You Didn’t Notice
_Slice tag: Slice 37 — Day Arc Week (Creepypasta style, British English)._


    ## Premise
    Monday: everything is normal, except no one meets your gaze. Tuesday: the clock is wrong by the same amount. Wednesday: the same song plays on different stations at once. Thursday: your name appears in a meeting record you do not remember. Friday: your reflection lingers too long. Saturday: footsteps are heard in the flat, but follow no one. Sunday: you wake to the sound of coffee brewing, even though you are still in bed.

    ## Arc structure
    - Monday–Sunday → Each day reveals a new fracture.
- Accumulation → The week becomes a pattern.
- Realisation → You have been counted, not living.

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
│ Wednesday  │
│            │
└─────┬──────┘
      │
┌────────────┐
│  Tuesday   │
│            │
└─────┬──────┘
      │
┌────────────┐
│   Monday   │
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

var monday = new Location("monday", "Monday: no one meets your gaze.");
var tuesday = new Location("tuesday", "Tuesday: the clock slips by a minute.");
var wednesday = new Location("wednesday", "Wednesday: the song plays everywhere at once.");

monday.AddExit(Direction.North, tuesday);
tuesday.AddExit(Direction.North, wednesday);

var state = new GameState(monday, worldLocations: new[] { monday, tuesday, wednesday });
var parser = new KeywordParser(KeywordParserConfig.Default);

while (true)
{
    Console.Write("
> ");
    var input = Console.ReadLine()?.Trim();
    if (string.IsNullOrWhiteSpace(input)) continue;

    if (string.Equals(input, "day", StringComparison.OrdinalIgnoreCase))
    {
        Console.WriteLine($"Day: {state.WorldState.GetCounter("day") + 1}");
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

    if (state.IsCurrentRoomId("tuesday"))
    {
        state.WorldState.Increment("day");
    }

    if (state.IsCurrentRoomId("wednesday"))
    {
        state.WorldState.Increment("day");
        Console.WriteLine("The week begins to repeat itself.");
        break;
    }
}
```
