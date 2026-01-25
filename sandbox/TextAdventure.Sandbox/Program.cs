using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;

// Create key
var cabinKey = new Key("cabin_key", "brass key");

// Create locations
var entrance = new Location("entrance")
    .Description("You stand at the forest gate. It's dark and foreboding.");

var forest = new Location("forest")
    .Description("A thick forest surrounds you. Shadows stretch long between ancient trees.");

var cave = new Location("cave")
    .Description("A dark cave with glowing mushrooms. A brass key glints on the ground!");

var clearing = new Location("clearing")
    .Description("A sunny clearing with wildflowers. A small cabin stands here.");

var cabin = new Location("cabin")
    .Description("Inside a cozy wooden cabin. A treasure chest sits in the corner!");

// Create locked door
var cabinDoor = new Door("cabin_door", "cabin door").RequiresKey(cabinKey);

// Connect locations
entrance.AddExit(Direction.North, forest);
forest.AddExit(Direction.East, cave);
forest.AddExit(Direction.West, clearing);
clearing.AddExit(Direction.In, cabin, cabinDoor);  // Locked!

// One-way trap
cave.AddExit(Direction.Down, entrance, oneWay: true);

// Game state
var state = new GameState(entrance);
var hasKey = false;

Console.WriteLine("=== FOREST ADVENTURE ===");
Console.WriteLine("Find the key and unlock the cabin!");
Console.WriteLine("Commands: north/south/east/west/in/out/up/down, open, unlock, quit\n");

while (true)
{
    Console.WriteLine($"\n{state.CurrentLocation.GetDescription()}");

    // Show exits with door status
    var exits = state.CurrentLocation.Exits
        .Select(e => e.Value.Door != null
            ? $"{e.Key} ({e.Value.Door.Name}: {e.Value.Door.State})"
            : e.Key.ToString());
    Console.WriteLine($"Exits: {string.Join(", ", exits)}");

    if (hasKey) Console.WriteLine("[You have: brass key]");

    Console.Write("\n> ");
    var input = Console.ReadLine()?.Trim().ToLower();

    if (string.IsNullOrEmpty(input)) continue;
    if (input == "quit" || input == "q") break;

    // Pick up key in cave
    if (state.CurrentLocation.Id == "cave" && !hasKey && (input == "take key" || input == "get key" || input == "take"))
    {
        hasKey = true;
        Console.WriteLine("You pick up the brass key!");
        continue;
    }

    // Unlock door
    if (input == "unlock" || input == "unlock door")
    {
        var exit = state.CurrentLocation.Exits.Values.FirstOrDefault(e => e.Door != null);
        if (exit?.Door == null)
        {
            Console.WriteLine("There's no door here.");
        }
        else if (!hasKey)
        {
            Console.WriteLine("You don't have a key.");
        }
        else if (exit.Door.Unlock(cabinKey))
        {
            Console.WriteLine($"You unlock the {exit.Door.Name}!");
        }
        else
        {
            Console.WriteLine("That key doesn't fit.");
        }
        continue;
    }

    // Open door
    if (input == "open" || input == "open door")
    {
        var exit = state.CurrentLocation.Exits.Values.FirstOrDefault(e => e.Door != null);
        if (exit?.Door == null)
        {
            Console.WriteLine("There's no door here.");
        }
        else if (exit.Door.Open())
        {
            Console.WriteLine($"You open the {exit.Door.Name}.");
        }
        else
        {
            Console.WriteLine($"The {exit.Door.Name} is locked!");
        }
        continue;
    }

    // Move
    if (Enum.TryParse<Direction>(input, true, out var direction))
    {
        if (state.Move(direction))
        {
            Console.WriteLine($"You go {direction}...");

            // Win condition
            if (state.CurrentLocation.Id == "cabin")
            {
                Console.WriteLine("\n*** CONGRATULATIONS! You found the treasure! ***");
                break;
            }

            // Fall trap
            if (input == "down" && state.CurrentLocation.Id == "entrance")
            {
                Console.WriteLine("Oops! You fell through a hole and ended up back at the entrance!");
            }
        }
        else
        {
            Console.WriteLine(state.LastMoveError);
        }
    }
    else
    {
        Console.WriteLine("Unknown command.");
    }
}

Console.WriteLine("\nThanks for playing!");
