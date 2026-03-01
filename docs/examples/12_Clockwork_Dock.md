# Clockwork Dock

_Slice tag: Slice 12 — DSL Parser (.adventure). Demo focuses on loading a world from a text file._

## Story beats (max ~10 steps)
1) You arrive at a ticking dock.
2) Find the brass token.
3) Open the gate.
4) Step onto the clockwork ferry.

## Map (rough layout)
```
          N
    W           E
          S

┌────────────┐     ┌────────────┐
│    Dock    │─────│   Ferry    │
│   T, G     │  In │            │
└────────────┘     └────────────┘

T = Brass token
G = Iron gate (door)
```

## Example (.adventure)
```text
world: Clockwork Dock
goal: Board the ferry
start: dock

location: dock | The dock hums with gears and steam.
item: token | brass token | Warm from the clockwork heat. | aliases=coin
door: gate | iron gate | A heavy gate of iron. | key=token
exit: in -> ferry | door=gate

location: ferry | The ferry creaks as it powers up.
```

## Example (load DSL)
```csharp
using System.IO;
using System.Text;
using MarcusMedina.TextAdventure.Dsl;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Parsing;

// this should be in a file
var sb = new StringBuilder();
sb.AppendLine("world: Clockwork Dock");
sb.AppendLine("goal: Board the ferry");
sb.AppendLine("start: dock");
sb.AppendLine();
sb.AppendLine("location: dock | The dock hums with gears and steam.");
sb.AppendLine("item: token | brass token | Warm from the clockwork heat. | aliases=coin");
sb.AppendLine("door: gate | iron gate | A heavy gate of iron. | key=token");
sb.AppendLine("exit: in -> ferry | door=gate");
sb.AppendLine();
sb.AppendLine("location: ferry | The ferry creaks as it powers up.");
var dsl = sb.ToString();

File.WriteAllText("clockwork.adventure", dsl);

var parser = new AdventureDslParser();
var adventure = parser.ParseFile("clockwork.adventure");

var state = adventure.State;
var commandParser = new KeywordParser(KeywordParserConfig.Default);

// Console setup for C64 aesthetics
Console.BackgroundColor = ConsoleColor.DarkBlue;
Console.ForegroundColor = ConsoleColor.Cyan;
Console.Title = "Clockwork Dock - Text Adventure Sandbox";
Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.Clear();
// End console setup
while (true)
{
    Console.Write("\n> ");
    var input = Console.ReadLine()?.Trim();
    if (string.IsNullOrWhiteSpace(input)) continue;

    var command = commandParser.Parse(input);
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
