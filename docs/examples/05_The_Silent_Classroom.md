# The Silent Classroom

_Slice tag: Slice 5 — NPCs + Dialog + Movement. Demo focuses on talking to an NPC and simple dialog._

## Story beats (max ~10 steps)
1) You enter an empty classroom.
2) A silent student sits in the back.
3) Talk to them.
4) Choose a dialog option.
5) Learn what happened.

## Example (NPC + dialog)
```csharp
using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Helpers;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;

// Location
var classroom = new Location("classroom", "Empty desks, a chalkboard, and a heavy silence.");

// NPCs
var npcList = new NpcList().AddMany("student");
var student = npcList["student"]
    .Description("A student sits in the back, staring at the floor.")
    .SetDialog(new DialogNode("We heard something in the hallway...")
        .AddOption("Ask what they heard")
        .AddOption("Ask where everyone went"));

classroom.AddNpc(student);

// Game state
var state = new GameState(classroom, worldLocations: new[] { classroom });

// Parser config (talk + look + go)
var parserConfig = new KeywordParserConfig(
    quit: CommandHelper.NewCommands("quit"),
    look: CommandHelper.NewCommands("look"),
    inventory: CommandHelper.NewCommands("inventory"),
    stats: CommandHelper.NewCommands("stats"),
    open: CommandHelper.NewCommands("open"),
    unlock: CommandHelper.NewCommands("unlock"),
    take: CommandHelper.NewCommands("take"),
    drop: CommandHelper.NewCommands("drop"),
    use: CommandHelper.NewCommands("use"),
    combine: CommandHelper.NewCommands("combine"),
    pour: CommandHelper.NewCommands("pour"),
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
    allowDirectionEnumNames: true);

var parser = new KeywordParser(parserConfig);

// Input loop (simplified)
while (true)
{
    Console.Write("\n> ");
    var input = Console.ReadLine()?.Trim();
    if (string.IsNullOrEmpty(input)) continue;

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
