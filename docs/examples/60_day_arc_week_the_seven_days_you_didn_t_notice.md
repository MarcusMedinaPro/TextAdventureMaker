# The Seven Days You Didn’t Notice

_Slice tag: Slice 60 — Day Arc Week (Creepypasta style)._

## Premise
Måndag: Allt är normalt, förutom att ingen möter din blick. Tisdag: Klockan går fel, alltid lika mycket. Onsdag: Samma låt spelas samtidigt på olika stationer. Torsdag: Ditt namn står i ett mötesprotokoll du inte minns. Fredag: Din spegelbild stannar kvar för länge. Lördag: Steg hörs i lägenheten, men följer ingen. Söndag: Du vaknar till ljudet av kaffe som bryggs – fast du fortfarande ligger i sängen.

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
