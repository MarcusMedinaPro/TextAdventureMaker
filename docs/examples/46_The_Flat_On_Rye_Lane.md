# The Flat on Rye Lane

_Slice tag: Slice 46 — Reality distortion via DynamicDescription, foreshadowing, and world-state flags._

## Story beats (max ~10 steps)
1) Arrive at the flat. Something feels wrong.
2) Explore the hallway. Notice the wallpaper is... breathing.
3) Enter the kitchen. The clock runs backwards.
4) Find a note in the bedroom. The handwriting is yours.
5) The bathroom mirror shows someone behind you. No one is there.
6) Return to the hallway. The front door is gone.
7) Read the note again. The text has changed.
8) Find the hidden door behind the wallpaper.
9) Step through. The flat resets. You are outside again.

## Map (rough layout)

```
          N
    W           E
          S

                    ┌─────────┐
                    │ Bathroom│
                    │   M     │
                    └────┬────┘
                         │
┌──────────┐    ┌────────────────┐    ┌──────────┐
│ Bedroom  ├────┤    Hallway     ├────┤ Kitchen  │
│   N      │    │   W            │    │   C      │
└──────────┘    └────────────────┘    └──────────┘

M = Mirror (fixed)
N = Note
W = Wallpaper (fixed, hides door)
C = Clock (fixed)
```

## Example (the flat on rye lane)
```csharp
using System;
using System.Linq;
using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;
using static MarcusMedina.TextAdventure.Extensions.ConsoleExtensions;

// === THE FLAT ON RYE LANE ===
// A creepypasta about a flat that changes around you.
// Features: DynamicDescription, WorldState flags, foreshadowing

// --- Locations ---
Location hallway = (id: "hallway", description: "A narrow hallway with peeling wallpaper. The air smells of damp and something older.");
Location kitchen = (id: "kitchen", description: "A cramped kitchen. A clock on the wall ticks loudly.");
Location bedroom = (id: "bedroom", description: "A small bedroom. The single bed is neatly made, as if expecting you.");
Location bathroom = (id: "bathroom", description: "A cold bathroom. A cracked mirror hangs above the basin.");

hallway.AddExit(Direction.East, kitchen);
hallway.AddExit(Direction.West, bedroom);
hallway.AddExit(Direction.North, bathroom);
kitchen.AddExit(Direction.West, hallway);
bedroom.AddExit(Direction.East, hallway);
bathroom.AddExit(Direction.South, hallway);

// --- Dynamic descriptions that shift with world state ---
hallway.SetDynamicDescription(new DynamicDescription()
    .FirstVisit("You step inside. The door clicks shut behind you. A narrow hallway stretches ahead, wallpaper peeling in long strips. The pattern almost looks like it's moving.")
    .When(s => s.WorldState.GetFlag("note_read") && !s.WorldState.GetFlag("door_gone"),
        "The hallway feels narrower than before. The wallpaper ripples gently, as though something beneath it is breathing.")
    .When(s => s.WorldState.GetFlag("door_gone"),
        "The hallway is impossibly narrow now. Where the front door was, there is only smooth, unbroken wall. The wallpaper pulses with a slow rhythm.")
    .When(s => s.WorldState.GetFlag("wallpaper_torn"),
        "The torn wallpaper reveals a small door set into the wall, painted the same colour as the plaster. It was always here.")
    .Default("A narrow hallway. The wallpaper pattern shifts when you aren't looking directly at it."));

kitchen.SetDynamicDescription(new DynamicDescription()
    .FirstVisit("The kitchen light flickers on by itself. A clock on the wall ticks—but the second hand moves backwards. The sink drips in time with it.")
    .When(s => s.WorldState.GetFlag("door_gone"),
        "The kitchen feels colder. The clock has stopped. The dripping continues, though the tap is dry.")
    .Default("A cramped kitchen. The clock ticks backwards. You try not to watch it."));

bedroom.SetDynamicDescription(new DynamicDescription()
    .FirstVisit("The bedroom door opens with a sigh. The bed is made with hospital corners. A folded note sits on the pillow, your name written on it in familiar handwriting.")
    .When(s => s.WorldState.GetFlag("note_read"),
        "The bedroom feels smaller. The pillow still has the indent where the note lay. You could swear the walls have moved closer together.")
    .Default("A small bedroom. The bed is neatly made. The walls are closer than you remember."));

bathroom.SetDynamicDescription(new DynamicDescription()
    .FirstVisit("You push open the bathroom door. The mirror above the basin is cracked in a starburst pattern. For a moment, in the fractured reflection, there is someone standing behind you. You spin round. No one.")
    .When(s => s.WorldState.GetFlag("mirror_checked"),
        "The bathroom. You avoid looking at the mirror, but you can feel it watching you.")
    .Default("A cold bathroom. The cracked mirror catches fragments of the room at wrong angles."));

// --- Items ---
var clock = new Item("clock", "Clock", "A wall clock. The second hand ticks backwards with mechanical precision.")
    .SetTakeable(false);

var note = new Item("note", "Note", "A folded note. Your name is written on the front in your own handwriting.")
    .SetReadable()
    .SetReadText("'You moved in last Tuesday. You will move in last Tuesday. The flat remembers. Do not look behind the wallpaper.'")
    .SetTakeable(true);

var mirror = new Item("mirror", "Mirror", "A cracked mirror. Your reflection stares back from a dozen fragments, each at a slightly different angle.")
    .SetTakeable(false);

var wallpaper = new Item("wallpaper", "Wallpaper", "Faded floral wallpaper, peeling at the edges. The pattern seems to shift when viewed from the corner of your eye.")
    .SetTakeable(false);

kitchen.AddItem(clock);
bedroom.AddItem(note);
bathroom.AddItem(mirror);
hallway.AddItem(wallpaper);

// --- Game state ---
var state = new GameState(hallway, worldLocations: [hallway, kitchen, bedroom, bathroom]);

// --- Foreshadowing ---
state.Foreshadowing.Plant("the_flat_remembers");
state.Foreshadowing.Plant("you_were_always_here");
state.Foreshadowing.Plant("the_door_behind_the_wallpaper");

// --- Parser ---
var parser = new KeywordParser(KeywordParserConfigBuilder.BritishDefaults()
    .Build());

// --- Custom logic ---
int turnCount = 0;

SetupC64("The Flat on Rye Lane");
WriteLineC64("=== THE FLAT ON RYE LANE ===");
WriteLineC64();
WriteLineC64("You answered an advert for a flat on Rye Lane, Peckham.");
WriteLineC64("The rent was suspiciously cheap. The landlord never appeared.");
WriteLineC64("The key was under the mat, as promised.");
WriteLineC64();
WriteLineC64("Goal: Explore the flat. Find a way out.");
WriteLineC64();

state.ShowRoom();

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

    // Custom: tear wallpaper / pull wallpaper
    if (trimmed.TextCompare("tear wallpaper") || trimmed.TextCompare("pull wallpaper")
        || trimmed.TextCompare("rip wallpaper") || trimmed.TextCompare("destroy wallpaper"))
    {
        if (!state.IsCurrentRoomId("hallway"))
        {
            WriteLineC64("There is no wallpaper here to tear.");
            continue;
        }

        if (state.WorldState.GetFlag("wallpaper_torn"))
        {
            WriteLineC64("The wallpaper is already torn away. The small door waits.");
            continue;
        }

        if (!state.WorldState.GetFlag("door_gone"))
        {
            WriteLineC64("You pick at the wallpaper. It tears easily but reveals only bare plaster beneath. Nothing yet.");
            continue;
        }

        state.WorldState.SetFlag("wallpaper_torn", true);
        state.Foreshadowing.Payoff("the_door_behind_the_wallpaper");
        WriteLineC64("You tear at the wallpaper in long strips. Behind it, set flush into the wall, is a small door. It was painted over, but the outline is unmistakable. It has always been here.");
        WriteLineC64();
        WriteLineC64("Type 'open door' to step through.");
        continue;
    }

    // Custom: open the hidden door
    if (trimmed.TextCompare("open door") || trimmed.TextCompare("go door") || trimmed.TextCompare("enter door"))
    {
        if (!state.WorldState.GetFlag("wallpaper_torn"))
        {
            WriteLineC64("Which door?");
            continue;
        }

        state.Foreshadowing.Payoff("you_were_always_here");
        WriteLineC64();
        WriteLineC64("You open the small door and step through.");
        WriteLineC64("Beyond it is... Rye Lane. Evening. The streetlamps are on.");
        WriteLineC64("You are standing outside number 14. The key is under the mat.");
        WriteLineC64("You don't remember walking here. But then, you never do.");
        WriteLineC64();
        WriteLineC64("The flat remembers.");
        WriteLineC64();
        WriteLineC64("=== THE END ===");
        break;
    }

    var command = parser.Parse(trimmed);
    var result = state.Execute(command);

    // Track reading the note
    if (command is ReadCommand && result.Success && state.WorldState.GetFlag("note_read") == false)
    {
        state.WorldState.SetFlag("note_read", true);
        state.Foreshadowing.Hint("the_flat_remembers");
        state.Foreshadowing.Hint("the_door_behind_the_wallpaper");
    }

    // Track looking at mirror
    if (state.IsCurrentRoomId("bathroom") && command is LookCommand)
    {
        state.WorldState.SetFlag("mirror_checked", true);
    }

    state.DisplayResult(command, result);

    // After a few turns, the front door vanishes
    turnCount++;
    if (turnCount == 6 && !state.WorldState.GetFlag("door_gone"))
    {
        state.WorldState.SetFlag("door_gone", true);
        state.Foreshadowing.Payoff("the_flat_remembers");
        WriteLineC64();
        WriteLineC64("You hear a soft click from the hallway. Like a lock turning — but from the inside.");
        WriteLineC64("Something has changed.");
    }

    // Hint after door is gone
    if (turnCount == 9 && state.WorldState.GetFlag("door_gone") && !state.WorldState.GetFlag("wallpaper_torn"))
    {
        WriteLineC64();
        WriteLineC64("The wallpaper in the hallway ripples. Something is behind it. You are certain now.");
    }

    if (result.ShouldQuit)
        break;
}
```
