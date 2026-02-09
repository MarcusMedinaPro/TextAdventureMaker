# The Broken Car

_Slice tag: Slice 26 — Chain of dependencies (tools, phone, fix)._

## Story beats (max ~10 steps)
1) Car dies on the roadside.
2) Find a way to call for help.
3) Get the right tools.
4) Attempt the fix.

## Map (rough layout)
```
          N
    W           E
          S

┌────────────┐     ┌────────────┐
│ Roadside   │─────│ Gas Station│
│    P       │  E  │  W, J, Mech│
└────────────┘     └────────────┘

P = Phone
W = Wrench
J = Jack
Mech = Mechanic (NPC)
```

## Example (tool chain)
```csharp
using System.Linq;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;
using static MarcusMedina.TextAdventure.Extensions.ConsoleExtensions;

// Slice 26 — Mood System
// Tests:
// - Mood details (lighting/sound/smell/temperature)
// - Mood propagation to adjacent rooms
// - Mood-modified description output

Location roadside = (id: "roadside", description: "Your car sits dead on the shoulder with the hood up.");
Location gasStation = (id: "gas_station", description: "A small gas station with a service bay.");

roadside.AddExit(Direction.East, gasStation);

roadside.AddItem(new Item("phone", "phone", "Low battery, but still works."));
var wrench = new Item("wrench", "wrench", "A sturdy wrench for a stubborn bolt.");
var jack = new Item("jack", "jack", "A heavy jack for lifting the car.");

gasStation.AddItem(wrench);
gasStation.AddItem(jack);

var mechanic = new Npc("mechanic", "mechanic")
    .Description("A mechanic wipes grease from their hands.")
    .SetDialog(new DialogNode("Need a tool or a lift?")
        .AddOption("Ask for a wrench")
        .AddOption("Ask for a jack"));

gasStation.AddNpc(mechanic);

var state = new GameState(roadside, worldLocations: new[] { roadside, gasStation });
var parser = new KeywordParser(KeywordParserConfig.Default);

var moodSystem = new MoodSystem();
moodSystem.SetMood(roadside, Mood.Foreboding);
moodSystem.SetLighting(roadside, LightLevel.Dim);
moodSystem.SetAmbientSound(roadside, "wind against metal");
moodSystem.SetSmell(roadside, "hot oil and wet asphalt");
moodSystem.SetTemperature(roadside, "cold");
moodSystem.Propagate(roadside, depth: 1);

var game = GameBuilder.Create()
    .UseState(state)
    .UseParser(parser)
    .AddTurnStart(g =>
    {
        var location = g.State.CurrentLocation;
        var mood = moodSystem.GetDetails(location);
        var description = DescribeWithMood(location.GetDescription(), mood.Mood);
        g.Output.WriteLine($"\nRoom: {location.Id.ToProperCase()}");
        g.Output.WriteLine(description);
        g.Output.WriteLine($"Mood: {mood.Mood} | Light: {mood.Lighting} | Sound: {mood.AmbientSound} | Smell: {mood.Smell} | Temp: {mood.Temperature}");
    })
    .Build();

SetupC64("The Broken Car - Text Adventure Sandbox");
game.Run();

static string DescribeWithMood(string description, Mood mood)
{
    return mood switch
    {
        Mood.Foreboding => $"{description} Shadows gather at the edges of the road.",
        Mood.Tense => $"{description} The air feels taut, waiting for a snap.",
        Mood.Terrifying => $"{description} Every noise feels closer than it should.",
        Mood.Hopeful => $"{description} A small warmth pushes back the dread.",
        _ => description
    };
}
```
