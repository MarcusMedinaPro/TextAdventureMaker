# Before the Meeting

_Slice tag: Slice 18 — Quiet prep scene. Demo focuses on small actions and atmosphere._

## Story beats (max ~10 steps)
1) You enter a calm office.
2) Grab coffee and papers.
3) Check yourself in the mirror.
4) Head into the meeting.

## Map (rough layout)
```
          N
    W           E
          S

┌────────────┐
│  Meeting   │
│     M      │
└─────┬──────┘
      │
      │
┌────────────┐
│   Office   │
│   C, P     │
└────────────┘

C = Coffee
P = Papers
M = Mirror
```

## Example (fluent setup)
```csharp
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;

Location office = (id: "office", description: "A quiet office. Notes lie by a warm cup.");
Location meeting = (id: "meeting", description: "A small meeting room. A mirror hangs by the door.");

office.AddExit(Direction.North, meeting);

Item coffee = (id: "coffee", name: "coffee", description: "A hot cup of coffee.");
Item papers = (id: "papers", name: "papers", description: "Notes for the meeting.");
Item mirror = (id: "mirror", name: "mirror", description: "A mirror for a quick check.");
mirror.SetTakeable(false);

office.AddItem(coffee);
office.AddItem(papers);
meeting.AddItem(mirror);

var state = new GameState(office, worldLocations: new[] { office, meeting });
var parser = new KeywordParser(KeywordParserConfig.Default);

var game = GameBuilder.Create()
    .UseState(state)
    .UseParser(parser)
    .AddTurnStart(g =>
    {
        var look = g.State.Look();
        g.Output.WriteLine($"\n{look.Message}");
    })
    .Build();

// Console setup for C64 aesthetics
Console.BackgroundColor = ConsoleColor.DarkBlue;
Console.ForegroundColor = ConsoleColor.Cyan;
Console.Title = "Before the Meeting - Text Adventure Sandbox";
Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.Clear();
// End console setup
game.Run();
```
