# If the Alarm Rings

_Slice tag: Slice 51 — Prescriptive (Creepypasta style)._

## Premise
Givet: huset är tomt. När: klockan slår 03:17. Då: gå inte upp till vinden, hur mycket det än låter som om någon gråter där.

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
