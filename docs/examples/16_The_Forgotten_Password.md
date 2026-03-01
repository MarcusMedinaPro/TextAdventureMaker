# The Forgotten Password

_Slice tag: Slice 16 — Quest System + Fluent flow. Demo focuses on a short quest check and a custom "login" step._

## Story beats (max ~10 steps)
1) You sit at a locked computer.
2) Find a post‑it note.
3) Recall the password.
4) Log in.

## Map (rough layout)
```
          N
    W           E
          S

┌────────────┐
│   Office   │
│  N, T      │
└────────────┘

N = Note
T = Terminal
```

## Example (quest + custom input)
```csharp
using MarcusMedina.TextAdventure.AI;
using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Helpers;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;
using static MarcusMedina.TextAdventure.Extensions.ConsoleExtensions;

// Slice 16 — AI Parser
// Tests:
// - Ollama command parser builder + fallback parser
// - "Go somewhere dark" resolved by fallback (AI parser stub)

Location office = (id: "office", description: "A quiet office with a locked terminal.");
Location serverRoom = (id: "server_room", description: "A humming, dark server room.");
Item note = (id: "note", name: "post-it note", description: "A hint: 0420.");
Item terminal = (id: "terminal", name: "terminal", description: "A password prompt waits.");

office.AddItem(note);
office.AddItem(terminal);
office.AddExit(Direction.Down, serverRoom);
serverRoom.AddExit(Direction.Up, office);

GameState state = new(office, worldLocations: [office, serverRoom]);
Quest quest = new("login", "Access the Terminal", "Find the hint and log in.")
    .AddCondition(new HasItemCondition("note"))
    .Start();

KeywordParser fallback = new(KeywordParserConfigBuilder.BritishDefaults()
    .WithGo("go", "move")
    .WithLook("look", "l")
    .Build());

ICommandParser parser = new OllamaCommandParserBuilder()
    .WithEndpoint("http://localhost:11434")
    .WithModel("llama3")
    .WithSystemPrompt("Translate natural language into adventure commands.")
    .WithFallback(fallback)
    .Build();

SetupC64("The Forgotten Password - Text Adventure Sandbox");
WriteLineC64("=== THE FORGOTTEN PASSWORD (Slice 16) ===");
WriteLineC64("Goal: find the note, then login. Try: go somewhere dark.");
WriteLineC64("Commands: look, take note, go down, login, quit.");

while (true)
{
    WriteLineC64();
    WritePromptC64("> ");
    string? input = Console.ReadLine();
    if (input is null)
        break;

    string trimmed = input.Trim();
    if (string.IsNullOrWhiteSpace(trimmed))
        continue;

    if (trimmed.TextCompare("login"))
    {
        if (quest.CheckProgress(state))
        {
            WriteLineC64("Access granted.");
            break;
        }

        WriteLineC64("You don't remember the password yet.");
        continue;
    }

    ICommand command = parser.Parse(trimmed);
    CommandResult result = state.Execute(command);

    if (!string.IsNullOrWhiteSpace(result.Message))
        WriteLineC64(result.Message);

    foreach (string reaction in result.ReactionsList.Where(r => !string.IsNullOrWhiteSpace(r)))
        WriteLineC64($"> {reaction}");

    if (result.ShouldQuit)
        break;
}
```
