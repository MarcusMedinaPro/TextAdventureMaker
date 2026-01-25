using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Parsing;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Helpers;
using MarcusMedina.TextAdventure.Localization;

// Create items (all styles are valid)
var cabinKey = new Key("cabin_key", "brass key");
cabinKey.SetWeight(0.1f);
cabinKey.AddAliases("key", "brass key");

var sword = ItemFactory.NewItem("rusty sword", 3.5f, "blade", "sword","pointy thing");

Item apple = "red apple";
apple.SetWeight(0.4f).AddAliases("apple");

var glass = new Glass("glass", "glass").SetWeight(0.6f);
var ice = new Item("ice", "ice")
    .SetWeight(0.5f)
    .SetReaction(ItemAction.Take, "The cold chills your hand.")
    .SetReaction(ItemAction.Drop, "It lands with a soft thump.")
    .SetReaction(ItemAction.Use, "You take a bite. Your teeth ache.");
var fire = new Item("fire", "fire").SetWeight(0.5f);

// Create locations
Location entrance = "entrance";
entrance.Description("You stand at the forest gate. It's dark and foreboding.");

Location forest = "forest";
forest.Description("A thick forest surrounds you. Shadows stretch long between ancient trees.");

Location cave = "cave";
cave.Description("A dark cave with glowing mushrooms. A brass key glints on the ground!");

Location clearing = "clearing";
clearing.Description("A sunny clearing with wildflowers. A small cabin stands here.");

Location cabin = "cabin";
cabin.Description("Inside a cozy wooden cabin. A treasure chest sits in the corner!");

// Place items
cave.AddItem(cabinKey);
entrance.AddItem(ice);
forest.AddItem(apple);
forest.AddItem(fire);
clearing.AddItem(sword);
clearing.AddItem(glass);

// Create locked door
Door cabinDoor = "cabin door";
cabinDoor.RequiresKey(cabinKey);

// Connect locations
entrance.AddExit(Direction.North, forest);
forest.AddExit(Direction.East, cave);
forest.AddExit(Direction.West, clearing);
clearing.AddExit(Direction.In, cabin, cabinDoor);  // Locked!

// One-way trap
cave.AddExit(Direction.Down, entrance, oneWay: true);

// Game state
var recipeBook = new RecipeBook()
    .Add(new ItemCombinationRecipe("ice", "fire", () => new FluidItem("water", "water")));
var state = new GameState(entrance, recipeBook: recipeBook);

Console.WriteLine("=== FOREST ADVENTURE ===");
Console.WriteLine("Find the key and unlock the cabin!");
var commands = new[]
{
    "go", "look", "open", "unlock", "take", "drop", "use", "inventory", "stats", "combine", "pour", "quit"
};
Console.WriteLine($"Commands: {commands.CommaJoin()} (or just type a direction)\n");

var parser = new KeywordParser();

while (true)
{
    var lookResult = state.Look();
    Console.WriteLine($"\n{lookResult.Message}");
    var inventoryResult = state.InventoryView();
    Console.WriteLine(inventoryResult.Message);

    Console.Write("\n> ");
    var input = Console.ReadLine()?.Trim().Lower();

    if (string.IsNullOrEmpty(input)) continue;

    var command = parser.Parse(input);
    var result = state.Execute(command);

    if (!string.IsNullOrWhiteSpace(result.Message))
    {
        Console.WriteLine(result.Message);
    }

    foreach (var reaction in result.ReactionsList)
    {
        if (!string.IsNullOrWhiteSpace(reaction))
        {
            Console.WriteLine($"> {reaction}");
        }
    }

    if (result.ShouldQuit)
    {
        break;
    }

    if (command is GoCommand go && result.Success)
    {
        // Win condition
        if (state.IsCurrentRoomId("cabin"))
        {
            Console.WriteLine("\n*** CONGRATULATIONS! You found the treasure! ***");
            break;
        }

        // Fall trap
        if (go.Direction == Direction.Down && state.CurrentLocation.Id == "entrance")
        {
            Console.WriteLine("Oops! You fell through a hole and ended up back at the entrance!");
        }
    }
}

Console.WriteLine("\nThanks for playing!");
