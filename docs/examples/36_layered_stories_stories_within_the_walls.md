# Stories Within the Walls
_Slice tag: Slice 36 — Layered Stories (Creepypasta style, British English)._


    ## Premise
    Each wallpaper layer hides an older story. Beneath the last one, the voices are still there.

    ## Arc structure
    - Surface → The newest layer.
- Deeper context → Old paper and old names.
- Hidden truth → The voices persist.
- Revelation → The house remembers.

    ## Story beats (max ~8 steps)
1) The disturbance arrives and feels personal.
2) A rule is broken or a boundary is crossed.
3) A clue reveals the scale of the problem.
4) A choice narrows the world.
5) The environment answers back.
6) A truth is forced into view.
7) A price is paid, willingly or not.
8) The ending leaves a lingering echo.

    ## Map (rough layout)
```
          N
    W           E
          S

┌────────────┐
│   Hall     │
└─────┬──────┘
      │
      │
┌────────────┐
│  Parlour   │
│    Wp      │
└─────┬──────┘
      │
      │
┌────────────┐
│  Cellar    │
│    D       │
└────────────┘

Wp = Wallpaper strip
D = Diary
```

    ## Example (detailed setup)

```csharp
using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;

var parlour = new Location("parlour", "A parlour with wallpaper that seems too thick.");
var hall = new Location("hall", "A hall with a faint smell of paint.");
var cellar = new Location("cellar", "A cellar where the walls remember.");

parlour.AddExit(Direction.North, hall);
hall.AddExit(Direction.Down, cellar);

var wallpaper = new Item("wallpaper", "wallpaper strip", "A strip of wallpaper with a second pattern beneath.")
    .SetHint("Look closer to see another story.")
    .SetProperty("layer", "1");
var diary = new Item("diary", "old diary", "A diary with pages glued together.")
    .SetReadText("We hid the truth under the second layer.")
    .RequireTakeToRead()
    .SetProperty("layer", "2");

parlour.AddItem(wallpaper);
cellar.AddItem(diary);

var state = new GameState(parlour, worldLocations: new[] { parlour, hall, cellar });

state.Events.Subscribe(GameEventType.PickupItem, e =>
{
    if (e.Item?.Id == "wallpaper")
    {
        state.WorldState.Increment("layers");
    }
    if (e.Item?.Id == "diary")
    {
        state.WorldState.Increment("layers");
    }
});

var parser = new KeywordParser(KeywordParserConfig.Default);

var game = GameBuilder.Create()
    .UseState(state)
    .UseParser(parser)
    .AddTurnStart(g =>
    {
        var look = g.State.Look();
        g.Output.WriteLine($"
{look.Message}");
    })
    .AddTurnEnd((g, command, result) =>
    {
        if (g.State.WorldState.GetCounter("layers") >= 2)
        {
            g.Output.WriteLine("You peel back the last layer and find yourself inside it.");
            g.RequestStop();
        }
    })
    .Build();

game.Run();
```
