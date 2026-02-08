# Keeping the Light On
_Slice tag: Slice 36 — Caretaker Arc (Creepypasta style, British English)._


    ## Premise
    You keep a lighthouse no one visits. Yet each time you light it, a response signal comes from the dark sea.

    ## Arc structure
    - Repair → Keep the light alive.
- Heal → Respond to the signals.
- Protect → Keep the sea at bay.
- Fight entropy → The light fades unless you stay.

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
│  Landing   │
└─────┬──────┘
      │
      │
┌────────────┐
│   Hall     │─────┐
│ Caretaker  │  N  │
└────────────┘     │
                   │
               ┌────────────┐
               │   Store    │
               │ O, L, W, F │
               └────────────┘

O = Oil
L = Lamp
W = Wire
F = Fuse
Caretaker = NPC
```

    ## Example (detailed setup)

```csharp
using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;

var hall = new Location("hall", "A hall where every bulb flickers in sympathy.");
var store = new Location("store", "A store room with tools and old spare parts.");
var landing = new Location("landing", "A landing that only looks safe when lit.");

hall.AddExit(Direction.North, store);
hall.AddExit(Direction.Up, landing);

var oil = new FluidItem("oil", "lamp oil", "A tin of oil that smells of rust.");
var lamp = new Glass("lamp", "glass lamp", "An empty lamp with a soot-darkened wick.");
var wire = new Item("wire", "copper wire", "Wire with frayed ends.");
var fuse = new Item("fuse", "fuse", "A fuse that might still hold.");

store.AddItem(oil);
store.AddItem(lamp);
store.AddItem(wire);
store.AddItem(fuse);

var caretaker = new Npc("caretaker", "caretaker")
    .Description("A caretaker with hands that remember how to fix things.")
    .SetDialog(new DialogNode("Keep it burning, even if it burns you.")
        .AddOption("Ask what broke")
        .AddOption("Ask how to fix it"));

hall.AddNpc(caretaker);

var state = new GameState(hall, worldLocations: new[] { hall, store, landing });
state.RecipeBook.Add(new ItemCombinationRecipe("wire", "fuse", () =>
    new Item("patch", "lamp patch", "A patch that keeps the light alive.")
        .SetHint("Use it on the lamp.")));

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
        if (g.State.Inventory.FindItem("patch") != null && lamp.Contents.Count > 0)
        {
            g.Output.WriteLine("The lamp holds. The landing is safe for now.");
            g.RequestStop();
        }
    })
    .Build();

game.Run();
```
