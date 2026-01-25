# Before the Meeting

_Slice tag: Slice 11 — Language Provider (file-based). Demo focuses on swapping UI text without changing game logic._

## Story beats (max ~10 steps)
1) You wake up late.
2) Grab coffee.
3) Find your notes.
4) Leave for the meeting.

## Example (swap language at runtime)
```csharp
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Localization;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;

// Load language from file
var languagePath = Path.Combine(AppContext.BaseDirectory, "lang", "gamelang.sv.txt");
Language.SetProvider(new FileLanguageProvider(languagePath));

// Locations
var bedroom = new Location("bedroom", "Your alarm blinks 08:57. A coat hangs by the door.");
var hallway = new Location("hallway", "A quiet hallway with a mirror.");

bedroom.AddExit(Direction.East, hallway);

// Items
var coffee = new Item("coffee", "coffee", "A hot cup of coffee.");
bedroom.AddItem(coffee);

// Game state
var state = new GameState(bedroom, worldLocations: new[] { bedroom, hallway });
state.RegisterLocations(new[] { bedroom, hallway });

// Parser
var parser = new KeywordParser(KeywordParserConfig.Default);

// Run loop (simplified)
while (true)
{
    Console.Write("\n> ");
    var input = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(input)) continue;

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
}
```

## Language file format (key=value)
```text
UnknownCommand=Okänt kommando.
ThanksForPlaying=Tack för att du spelade!
DoorLockedTemplate={0} är låst.
```
