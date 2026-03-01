# Custom Commands + Pronouns

_Slice tag: Slice 45 — Aliases, pronouns, and custom commands._

## Example (aliases + pronouns + custom commands)
```csharp
using System;
using System.Linq;
using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;
using static MarcusMedina.TextAdventure.Extensions.ConsoleExtensions;

// Slice 45 — Aliases + Pronouns + Custom Commands
// Tests:
// - Phrase aliases ("sit down" -> "sit")
// - Pronoun carry-over ("take jeans" then "wear them")
// - "again" repeats last command
// - Custom commands ("shoot ant", "pull rubber chicken")

Location room = (id: "room", description: "A small room with a coat rack and a rubber chicken.");
room.AddItem(new Item("jeans", "jeans", "A pair of worn jeans."));
room.AddItem(new Item("chicken", "rubber chicken", "A rubber chicken with a pulley in the middle."));

var state = new GameState(room, worldLocations: new[] { room });

var parser = new KeywordParser(KeywordParserConfigBuilder.BritishDefaults()
    .WithPhraseAlias("sit down", "sit")
    .WithPronouns("it", "them")
    .WithAgain("again", "g")
    .WithCustomCommand("shoot ant", _ => new CustomCommand("You shoot the ant. It had it coming."))
    .WithCustomCommand("pull rubber chicken", _ => new CustomCommand("You pull the chicken. A soft squeak answers."))
    .Build());

SetupC64("Custom Commands - Text Adventure Sandbox");
WriteLineC64("=== CUSTOM COMMANDS (Slice 45) ===");
WriteLineC64("Try: take jeans, wear them, again, sit down, shoot ant, pull rubber chicken.");

while (true)
{
    WriteLineC64();
    WritePromptC64("> ");
    var input = Console.ReadLine();
    if (input is null)
        break;

    var trimmed = input.Trim();
    if (string.IsNullOrWhiteSpace(trimmed))
        continue;

    var command = parser.Parse(trimmed);
    if (command is CustomCommand custom)
    {
        WriteLineC64(custom.Message);
        continue;
    }

    var result = state.Execute(command);
    if (!string.IsNullOrWhiteSpace(result.Message))
        WriteLineC64(result.Message);

    foreach (var reaction in result.ReactionsList.Where(r => !string.IsNullOrWhiteSpace(r)))
        WriteLineC64($"> {reaction}");

    if (result.ShouldQuit)
        break;
}

sealed class CustomCommand(string message) : ICommand
{
    public string Message { get; } = message;

    public CommandResult Execute(CommandContext context)
    {
        return CommandResult.Ok(Message);
    }
}
```
