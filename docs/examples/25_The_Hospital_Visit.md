# The Hospital Visit

_Slice tag: Slice 25 — Staged access, check-in, and results flow._

## Story beats (max ~10 steps)
1) Arrive at the hospital entrance.
2) Check in at reception.
3) Wait to be called.
4) Receive results in the exam room.

## Example (check-in flow)
```csharp
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;

Location entrance = (id: "hospital_entrance", description: "Automatic doors slide open to a bright lobby.");
Location reception = (id: "reception", description: "A reception desk with a stack of forms.");
Location waitingRoom = (id: "waiting_room", description: "Plastic chairs and a quiet TV loop.");
Location examRoom = (id: "exam_room", description: "A clean exam room with a curtained bed.");

entrance.AddExit(Direction.In, reception);
reception.AddExit(Direction.East, waitingRoom);
waitingRoom.AddExit(Direction.North, examRoom);

reception.AddItem(new Item("forms", "intake forms", "Paperwork asking the usual questions."));
waitingRoom.AddItem(new Item("magazine", "magazine", "Outdated magazines and a crossword."));
examRoom.AddItem(new Item("results", "test results", "The results are normal.").SetTakeable(false));

var receptionist = new Npc("receptionist", "receptionist")
    .Description("A receptionist with a calm voice.")
    .SetDialog(new DialogNode("Can I get your name and date of birth?")
        .AddOption("Hand over the clipboard")
        .AddOption("Ask about the wait time"));

reception.AddNpc(receptionist);

var state = new GameState(entrance, worldLocations: new[] { entrance, reception, waitingRoom, examRoom });
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
