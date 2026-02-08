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
using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Helpers;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;

Location office = (id: "office", description: "A quiet office with a locked terminal.");
Item note = (id: "note", name: "post-it note", description: "A hint: 0420.");
Item terminal = (id: "terminal", name: "terminal", description: "A password prompt waits.");

office.AddItem(note);

office.AddItem(terminal);

var state = new GameState(office, worldLocations: new[] { office });
var quest = new Quest("login", "Access the Terminal", "Find the hint and log in.")
    .AddCondition(new HasItemCondition("note"))
    .Start();

var parser = new KeywordParser(KeywordParserConfig.Default);

// Console setup for C64 aesthetics
Console.BackgroundColor = ConsoleColor.DarkBlue;
Console.ForegroundColor = ConsoleColor.Cyan;
Console.Title = "The Forgotten Password - Text Adventure Sandbox";
Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.Clear();
// End console setup
while (true)
{
    Console.Write("\n> ");
    var input = Console.ReadLine()?.Trim();
    if (string.IsNullOrWhiteSpace(input)) continue;

    if (input.TextCompare("login"))
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
