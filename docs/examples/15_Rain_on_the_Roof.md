# Rain on the Roof

_Slice tag: Slice 15 — Combat as Struggle + Fluent setup. Demo focuses on a simple endurance encounter._

## Story beats (max ~10 steps)
1) Rain leaks through the roof.
2) A bucket is nearby.
3) The leak worsens.
4) Endure until it passes.

## Map (rough layout)
```
          N
    W           E
          S

┌────────────┐
│   Attic    │
│  B, Storm  │
└────────────┘

B = Bucket
Storm = NPC
```

## Example (fluent combat loop)
```csharp
using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Helpers;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;

Location attic = (id: "attic", description: "Rain drums against the roof. A leak gathers overhead.");
Item bucket = (id: "bucket", name: "bucket", description: "A metal bucket.");
attic.AddItem(bucket);

var storm = new Npc("storm", "storm", NpcState.Hostile, stats: new Stats(12))
    .Description("A relentless leak you must endure.")
    .SetMovement(new NoNpcMovement());

attic.AddNpc(storm);

var state = new GameState(attic, worldLocations: new[] { attic });
var parser = new KeywordParser(KeywordParserConfig.Default);

var game = GameBuilder.Create()
    .UseState(state)
    .UseParser(parser)
    .AddTurnStart(g =>
    {
        var look = g.State.Look();
        g.Output.WriteLine($"\n{look.Message}");
    })
    .AddTurnEnd((g, command, result) =>
    {
        if (!storm.IsAlive)
        {
            g.Output.WriteLine("The bucket catches the leak. The storm passes.");
            g.RequestStop();
        }
    })
    .Build();

// Console setup for C64 aesthetics
Console.BackgroundColor = ConsoleColor.DarkBlue;
Console.ForegroundColor = ConsoleColor.Cyan;
Console.Title = "Rain on the Roof - Text Adventure Sandbox";
Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.Clear();
// End console setup
game.Run();
```
