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
using System;
using System.Collections.Generic;
using System.Linq;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;
using static MarcusMedina.TextAdventure.Extensions.ConsoleExtensions;

// Slice 31 — Scene Transitions
// Tests:
// - Scene beats and transitions based on player triggers

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

var hallwayScene = new Scene("hallway_scene")
    .Location("school_hallway")
    .SetParticipants("guard", "player")
    .Beat(1, "guard_blocks")
    .Beat(2, "player_explains");

hallwayScene.Transition()
    .To("security_scene").Trigger("explain")
    .Or()
    .To("classroom_scene").Trigger("ask");

var securityScene = new Scene("security_scene")
    .Location("security_office")
    .SetParticipants("guard", "player")
    .Beat(1, "guard_retrieves_key")
    .Beat(2, "guard_hands_key");

securityScene.Transition()
    .To("classroom_scene").Trigger("return");

var classroomScene = new Scene("classroom_scene")
    .Location("locked_classroom")
    .SetParticipants("player")
    .Beat(1, "unlock_door")
    .Beat(2, "enter_room");

var scenes = new Dictionary<string, Scene>(StringComparer.OrdinalIgnoreCase)
{
    [hallwayScene.Id] = hallwayScene,
    [securityScene.Id] = securityScene,
    [classroomScene.Id] = classroomScene
};

Scene activeScene = hallwayScene;

SetupC64("The Locked Classroom - Text Adventure Sandbox");
WriteLineC64("=== THE LOCKED CLASSROOM (Slice 31) ===");
WriteLineC64("Goal: trigger scene transitions: explain, ask, return.");
PlayScene(activeScene);

while (true)
{
    WriteLineC64();
    WritePromptC64("> ");
    string? input = Console.ReadLine();
    if (input is null)
        break;

    string trimmed = input.Trim();
    if (string.IsNullOrWhiteSpace(trimmed))
        continue;

    if (trimmed.Is("quit") || trimmed.Is("exit"))
        break;

    if (TryTransition(trimmed, ref activeScene))
    {
        PlayScene(activeScene);
        continue;
    }

    var command = parser.Parse(trimmed);
    var result = state.Execute(command);
    if (!string.IsNullOrWhiteSpace(result.Message))
        WriteLineC64(result.Message);
}

bool TryTransition(string input, ref Scene current)
{
    var trigger = input.Lower();
    if (!current.TryGetTransition(trigger, out var transition))
        return false;

    if (!scenes.TryGetValue(transition.TargetSceneId, out var next))
        return false;

    current = next;
    return true;
}

void PlayScene(Scene scene)
{
    WriteLineC64();
    WriteLineC64($"Scene: {scene.Id.Replace("_", " ").ToProperCase()}");
    foreach (var beat in scene.Play())
    {
        WriteLineC64(RenderBeat(beat.EventId));
    }
}

static string RenderBeat(string eventId)
{
    return eventId switch
    {
        "guard_blocks" => "The guard holds up a hand. \"No entry without a reason.\"",
        "player_explains" => "You explain you need the classroom briefly.",
        "guard_retrieves_key" => "The guard rummages for the classroom key.",
        "guard_hands_key" => "The key lands in your palm.",
        "unlock_door" => "The lock turns with a soft click.",
        "enter_room" => "The classroom opens into the quiet dark.",
        _ => eventId.ToProperCase()
    };
}
```
