# The Forgotten Password

_Slice tag: Slice 8 — Quest System. Demo focuses on quest conditions and completion._

## Story beats (max ~10 steps)
1) You sit at a locked computer.
2) Find a post‑it note.
3) Recall the password.
4) Log in.

## Example (quest conditions)
```csharp
using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Helpers;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;

// Location
var office = new Location("office", "A quiet office with a locked computer.");

// Items
var note = new Item("note", "post‑it note", "A note with a hint: 0420.");
office.AddItem(note);

// Game state
var state = new GameState(office, worldLocations: new[] { office });

// Quest: retrieve note
var quest = new Quest("login", "Access the Computer", "Find the password hint and log in.")
    .AddCondition(new HasItemCondition("note"))
    .Start();

// Parser config (minimal)
var parserConfig = new KeywordParserConfig(
    quit: CommandHelper.NewCommands("quit"),
    look: CommandHelper.NewCommands("look"),
    examine: CommandHelper.NewCommands("examine"),
    inventory: CommandHelper.NewCommands("inventory"),
    stats: CommandHelper.NewCommands("stats"),
    open: CommandHelper.NewCommands("open"),
    unlock: CommandHelper.NewCommands("unlock"),
    take: CommandHelper.NewCommands("take"),
    drop: CommandHelper.NewCommands("drop"),
    use: CommandHelper.NewCommands("use"),
    combine: CommandHelper.NewCommands("combine"),
    pour: CommandHelper.NewCommands("pour"),
    move: CommandHelper.NewCommands("move", "push", "shift", "lift", "slide"),
    go: CommandHelper.NewCommands("go"),
    read: CommandHelper.NewCommands("read"),
    talk: CommandHelper.NewCommands("talk"),
    attack: CommandHelper.NewCommands("attack"),
    flee: CommandHelper.NewCommands("flee"),
    save: CommandHelper.NewCommands("save"),
    load: CommandHelper.NewCommands("load"),
    all: CommandHelper.NewCommands("all"),
    ignoreItemTokens: CommandHelper.NewCommands(),
    combineSeparators: CommandHelper.NewCommands(),
    pourPrepositions: CommandHelper.NewCommands(),
    directionAliases: new Dictionary<string, Direction>(StringComparer.OrdinalIgnoreCase),
    allowDirectionEnumNames: true,
    enableFuzzyMatching: true,
    fuzzyMaxDistance: 1);

var parser = new KeywordParser(parserConfig);

// Input loop (simplified)
while (true)
{
    Console.Write("\n> ");
    var input = Console.ReadLine()?.Trim();
    if (string.IsNullOrEmpty(input)) continue;

    if (string.Equals(input, "login", StringComparison.OrdinalIgnoreCase))
    {
        if (quest.CheckProgress(state))
        {
            Console.WriteLine("Access granted.");
            break;
        }
        Console.WriteLine("You don't remember the password yet.");
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
}
```
