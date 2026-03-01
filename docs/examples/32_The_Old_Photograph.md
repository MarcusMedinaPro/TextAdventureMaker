# The Old Photograph

_Slice tag: Slice 32 — Clue chain to meaning._

## Story beats (max ~10 steps)
1) Find a locked classroom.
2) Enter the archive.
3) Discover an old photograph.
4) Follow the clue to a family room.

## Map (rough layout)
```
          N
    W           E
          S

┌────────────┐
│  Archive   │─────┐
│  Photo, Ar │     │
└─────┬──────┘     │
      │            │
      │            │
┌────────────┐  ┌────────────┐
│ Classroom  │  │ FamilyRoom │
│            │  │     Al     │
└────────────┘  └────────────┘

Photo = Photograph
Ar = Archivist (NPC)
Al = Album
```

## Example (photograph clue)
```csharp
using System;
using System.Linq;
using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;
using static MarcusMedina.TextAdventure.Extensions.ConsoleExtensions;

// Slice 32 — Bond System
// Tests:
// - Bond investment moments and payoff
// - Warning when bond is unearned

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

var bond = archivist.CreateBond("archivist_bond")
    .InvestmentMoments("share_memory", "return_photo")
    .Payoff(ctx =>
    {
        if (!ctx.Bond.IsEstablished)
        {
            WriteLineC64("The loss lands flat. You never really knew them.");
            return;
        }

        WriteLineC64("The archivist's absence leaves a deep, quiet ache.");
    });

var state = new GameState(classroom, worldLocations: new[] { classroom, archive, familyRoom });
var parser = new KeywordParser(KeywordParserConfig.Default);

SetupC64("The Old Photograph - Text Adventure Sandbox");
WriteLineC64("=== THE OLD PHOTOGRAPH (Slice 32) ===");
WriteLineC64("Goal: build a bond, then trigger the payoff.");
WriteLineC64("Commands: talk archivist, share memory, return photo, sacrifice, look, go north/east, quit.");
ShowRoom();

while (true)
{
    WriteLineC64();
    WritePromptC64("> ");
    var input = Console.ReadLine();
    if (input is null)
        break;

    var trimmed = input.Trim();
    if (string.IsNullOrWhiteSpace(trimmed))
        continue;

    if (trimmed.Is("quit") || trimmed.Is("exit"))
        break;

    if (trimmed.TextCompare("share memory"))
    {
        bond.RecordInvestment("share_memory");
        WriteLineC64("You share a memory. The archivist softens.");
        continue;
    }

    if (trimmed.TextCompare("return photo"))
    {
        bond.RecordInvestment("return_photo");
        WriteLineC64("You return a photograph to its place.");
        continue;
    }

    if (trimmed.TextCompare("sacrifice"))
    {
        bond.TriggerPayoff(state, archivist);
        if (!bond.IsEstablished)
            WriteLineC64("Warning: a bond was not established before the payoff.");
        break;
    }

    var command = parser.Parse(trimmed);
    var result = state.Execute(command);
    if (!string.IsNullOrWhiteSpace(result.Message))
        WriteLineC64(result.Message);

    foreach (var reaction in result.ReactionsList.Where(r => !string.IsNullOrWhiteSpace(r)))
        WriteLineC64($"> {reaction}");

    if (command is GoCommand && result.Success)
        ShowRoom();
}

void ShowRoom()
{
    WriteLineC64();
    WriteLineC64($"Room: {state.CurrentLocation.Id.ToProperCase()}");
    WriteLineC64(state.CurrentLocation.GetDescription());
}
```
