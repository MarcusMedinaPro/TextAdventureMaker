# The Old Photograph

_Slice tag: Slice 32 — Clue chain to meaning._

## Story beats (max ~10 steps)
1) Find a locked classroom.
2) Enter the archive.
3) Discover an old photograph.
4) Follow the clue to a family room.

## Example (photograph clue)
```csharp
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;

Location classroom = (id: "locked_classroom", description: "A classroom dark behind the glass.");
Location archive = (id: "photo_archive", description: "A small archive with labeled boxes and old frames.");
Location familyRoom = (id: "family_room", description: "A quiet room with a soft lamp and a worn sofa.");

classroom.AddExit(Direction.North, archive);
archive.AddExit(Direction.East, familyRoom);

archive.AddItem(new Item("photograph", "old photograph", "A faded photo of a family by a lake."));
familyRoom.AddItem(new Item("album", "photo album", "An album with a page marked by a ribbon."));

var archivist = new Npc("archivist", "archivist")
    .Description("An archivist carefully dusts a frame.")
    .SetDialog(new DialogNode("Looking for a specific year?")
        .AddOption("Ask about the old photograph")
        .AddOption("Ask about the family room"));

archive.AddNpc(archivist);

var state = new GameState(classroom, worldLocations: new[] { classroom, archive, familyRoom });
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
