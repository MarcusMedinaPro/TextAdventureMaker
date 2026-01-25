# Job Interview

_Slice tag: Slice 23 — Structured dialog. Demo focuses on a formal interview flow._

## Story beats (max ~10 steps)
1) Wait in the lobby.
2) Meet the interviewer.
3) Answer questions.
4) Handle nerves.

## Example (dialog choices)
```csharp
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;

Location lobby = (id: "interview_lobby", description: "A calm lobby with a glass of water.");
Location room = (id: "interview_room", description: "A quiet interview room with two chairs.");

lobby.AddExit(Direction.In, room);

var interviewer = new Npc("interviewer", "interviewer")
    .Description("An interviewer with a kind smile.")
    .SetDialog(new DialogNode("Thanks for coming. Ready to begin?")
        .AddOption("Talk about your experience")
        .AddOption("Ask about the team")
        .AddOption("Admit you're nervous"));

room.AddNpc(interviewer);

var state = new GameState(lobby, worldLocations: new[] { lobby, room });
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
