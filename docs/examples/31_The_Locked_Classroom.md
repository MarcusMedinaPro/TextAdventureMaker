# The Locked Classroom

_Slice tag: Slice 31 — Navigation + alarm constraint._

## Story beats (max ~10 steps)
1) Reach the school hallway.
2) Find the security office.
3) Get the classroom key.
4) Unlock the classroom.

## Map (rough layout)
```
          N
    W           E
          S

┌────────────┐     ┌────────────┐     ┌────────────┐
│  Security  │─────│  Hallway   │─────│ Classroom  │
│     K      │  E  │   Guard    │  E  │   (Door)   │
└────────────┘     └────────────┘     └────────────┘

K = Classroom key
Guard = NPC
```

## Example (locked classroom)
```csharp
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;

Location hallway = (id: "school_hallway", description: "A dim hallway lined with closed doors.");
Location classroom = (id: "locked_classroom", description: "A classroom dark behind the glass.");
Location security = (id: "security_office", description: "A small office with a wall of monitors.");

hallway.AddExit(Direction.West, security);

var classroomKey = new Key("classroom_key", "classroom key", "A worn key with a scratched tag.");
security.AddItem(classroomKey);

var classroomDoor = new Door("classroom_door", "classroom door", "A reinforced classroom door.")
    .RequiresKey(classroomKey);

hallway.AddExit(Direction.East, classroom, classroomDoor);

var guard = new Npc("guard", "guard")
    .Description("A night guard watches the hallway.")
    .SetDialog(new DialogNode("This wing is locked after hours.")
        .AddOption("Explain why you're here")
        .AddOption("Ask about the classroom"));

hallway.AddNpc(guard);

var state = new GameState(hallway, worldLocations: new[] { hallway, classroom, security });
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
