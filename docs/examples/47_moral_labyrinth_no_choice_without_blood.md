# No Choice Without Blood

_Slice tag: Slice 47 — Moral Labyrinth (Creepypasta style)._

## Premise
Varje dörr du öppnar räddar någon. Varje dörr du låter vara stängd dödar någon annan. Kartan visar inga utgångar.

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
