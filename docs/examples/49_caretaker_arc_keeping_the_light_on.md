# Keeping the Light On

    _Slice tag: Slice 49 — Caretaker Arc (Creepypasta style, British English)._


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

    ## Example (detailed setup)
    ```csharp
    using MarcusMedina.TextAdventure.Engine;
    using MarcusMedina.TextAdventure.Enums;
    using MarcusMedina.TextAdventure.Extensions;
    using MarcusMedina.TextAdventure.Models;
    using MarcusMedina.TextAdventure.Parsing;

    Location room = (id: "room", description: "A small room with a low, uneasy hum.");
    Location hall = (id: "hall", description: "A corridor that feels longer than it should.");
    Location threshold = (id: "threshold", description: "A place you should not have reached.");

    room.AddExit(Direction.North, hall);
    hall.AddExit(Direction.North, threshold);

    room.AddItem(new Item("note", "note", "A note written in your own hand."));
    hall.AddItem(new Item("key", "key", "A cold key with no teeth."));

    var watcher = new Npc("watcher", "watcher")
        .Description("A still figure that might be a shadow.")
        .SetDialog(new DialogNode("You are not late, only early.")
            .AddOption("Ask who they are")
            .AddOption("Say nothing"));

    hall.AddNpc(watcher);

    var state = new GameState(room, worldLocations: new[] { room, hall, threshold });
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
            if (g.State.CurrentLocation.Id == "threshold")
            {
                g.Output.WriteLine("The air tastes of iron and rain.");
            }
        })
        .Build();

    game.Run();
    ```
