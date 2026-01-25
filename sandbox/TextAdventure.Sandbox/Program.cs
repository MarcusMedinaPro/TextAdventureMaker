using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;

// Create locations
var entrance = new Location("entrance")
    .Description("You stand at the forest gate. It's dark and foreboding.");

var forest = new Location("forest")
    .Description("A thick forest surrounds you. Shadows stretch long between ancient trees.");

var cave = new Location("cave")
    .Description("A dark cave with glowing mushrooms. You hear dripping water.");

var clearing = new Location("clearing")
    .Description("A sunny clearing with wildflowers. A small pond reflects the sky.");

var cabin = new Location("cabin")
    .Description("Inside a cozy wooden cabin. A fire crackles in the hearth.");

// Connect locations (bi-directional auto-created)
entrance.AddExit(Direction.North, forest);
forest.AddExit(Direction.East, cave);
forest.AddExit(Direction.West, clearing);
clearing.AddExit(Direction.In, cabin);

// One-way trap!
cave.AddExit(Direction.Down, entrance, oneWay: true);

// Create game state
var state = new GameState(entrance);

Console.WriteLine("=== FOREST ADVENTURE ===");
Console.WriteLine("Type a direction (north, south, east, west, in, out, up, down) to move.");
Console.WriteLine("Type 'quit' to exit.\n");

while (true)
{
    Console.WriteLine($"\n{state.CurrentLocation.GetDescription()}");
    Console.WriteLine($"Exits: {string.Join(", ", state.CurrentLocation.Exits.Keys)}");
    Console.Write("\n> ");

    var input = Console.ReadLine()?.Trim().ToLower();

    if (string.IsNullOrEmpty(input)) continue;
    if (input == "quit" || input == "exit" || input == "q") break;

    if (Enum.TryParse<Direction>(input, true, out var direction))
    {
        if (state.Move(direction))
        {
            Console.WriteLine($"You go {direction}...");
        }
        else
        {
            Console.WriteLine("You can't go that way.");
        }
    }
    else
    {
        Console.WriteLine("Unknown command. Try: north, south, east, west, in, out, up, down, quit");
    }
}

Console.WriteLine("\nThanks for playing!");
