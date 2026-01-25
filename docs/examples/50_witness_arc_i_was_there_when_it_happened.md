# I Was There When It Happened

_Slice tag: Slice 50 — Witness Arc (Creepypasta style)._

## Premise
Du såg olyckan. Ingen annan minns den. Men varje natt hör du bilen krascha igen, exakt samma sekund.

## Story beats (max ~6 steps)
    1) Introduce the disturbance.
    2) Offer a single, uneasy choice.
    3) Reveal a subtle change in the world.
    4) Force a consequence or realization.
    5) Close on an unresolved echo.

    ## Example (minimal setup)
    ```csharp
    using MarcusMedina.TextAdventure.Engine;
    using MarcusMedina.TextAdventure.Enums;
    using MarcusMedina.TextAdventure.Extensions;
    using MarcusMedina.TextAdventure.Models;
    using MarcusMedina.TextAdventure.Parsing;

    Location start = (id: "start", description: "A quiet room with a wrong feeling.");
    Location threshold = (id: "threshold", description: "A place you shouldn't have reached.");

    start.AddExit(Direction.North, threshold);
    start.AddItem(new Item("note", "note", "A note that shouldn't be here."));

    var state = new GameState(start, worldLocations: new[] { start, threshold });
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
        .Build();

    game.Run();
    ```
