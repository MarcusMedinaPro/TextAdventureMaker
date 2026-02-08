# The Bar Fight

_Slice tag: Slice 28 — Escalation + de-escalation choice._

## Story beats (max ~10 steps)
1) Enter a noisy bar.
2) Notice rising tension.
3) Choose to calm things down or escalate.
4) Leave the bar.

## Map (rough layout)
```
          N
    W           E
          S

┌────────────┐
│    Bar     │
│  St, G     │
│  Bnc, Brw  │
└─────┬──────┘
      │
      │
┌────────────┐
│   Alley    │
└────────────┘

St = Stool
G = Glass
Bnc = Bouncer (NPC)
Brw = Brawler (NPC)
```

## Example (tension choice)
```csharp
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;

Location bar = (id: "bar", description: "A noisy bar with clinking glasses.");
Location alley = (id: "bar_alley", description: "A narrow alley behind the bar.");

bar.AddExit(Direction.South, alley);

bar.AddItem(new Item("stool", "bar stool", "A sturdy stool bolted to the floor.").SetTakeable(false));
bar.AddItem(new Item("glass", "glass", "An empty glass with a chipped rim."));

var bouncer = new Npc("bouncer", "bouncer")
    .Description("A bouncer watches the room with folded arms.")
    .SetDialog(new DialogNode("Keep it calm in here.")
        .AddOption("Back off")
        .AddOption("Ask for help"));

var brawler = new Npc("brawler", "brawler")
    .SetState(NpcState.Hostile)
    .SetStats(new Stats(10))
    .Description("A restless patron looks for a fight.")
    .SetDialog(new DialogNode("What are you looking at?")
        .AddOption("Apologize and step away")
        .AddOption("Stand your ground"));

bar.AddNpc(bouncer);
bar.AddNpc(brawler);

var state = new GameState(bar, worldLocations: new[] { bar, alley });
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

game.Run();
```
