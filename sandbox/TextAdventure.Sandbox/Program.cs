using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Parsing;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Helpers;
using MarcusMedina.TextAdventure.Localization;
using MarcusMedina.TextAdventure.Interfaces;

// Create items (all styles are valid)
Key cabinKey = (id: "cabin_key", name: "brass key", description: "A small brass key with worn teeth.");
cabinKey.SetWeight(0.1f).AddAliases("key", "brass key");

Item sword = (id: "sword", name: "rusty sword", description: "A rusted blade with a dull edge.");
sword.SetWeight(3.5f).AddAliases("blade", "sword", "pointy thing");

Item apple = (id: "apple", name: "red apple", description: "A crisp red apple.");
apple.SetWeight(0.4f).AddAliases("apple");

Glass glass = (id: "glass", name: "glass", description: "A clear drinking glass.");
glass.SetWeight(0.6f)
    .SetReaction(ItemAction.Take, "The glas surface is smooth")
    .SetReaction(ItemAction.Drop, "The glass bounces on the floor") // can we destroy it from this reaction?
    .SetReaction(ItemAction.Destroy, "The glass shatters into 1000 pieces");
    ;

Item ice = (id: "ice", name: "ice", description: "A cold chunk of ice.");
ice.SetWeight(0.5f)
    .SetReaction(ItemAction.Take, "The cold chills your hand.")
    .SetReaction(ItemAction.Drop, "It lands with a soft thump.")
    .SetReaction(ItemAction.Use, "You take a bite. Your teeth ache.");

Item fire = (id: "fire", name: "fire", description: "A flickering flame.");
fire.SetWeight(0.5f);

Item lantern = (id: "lantern", name: "lantern", description: "A lantern that casts a warm glow.");
lantern.SetWeight(1.2f);

Item sign = (id: "sign", name: "wooden sign", description: "A weathered wooden sign.");
sign.SetTakeable(false)
    .SetReadable()
    .SetReadText("Welcome to the Dark Forest!");

Item newspaper = (id: "newspaper", name: "daily news", description: "A crinkled newspaper.");
newspaper.SetReadable()
    .RequireTakeToRead()
    .SetReadText("HEADLINE: Dragon spotted near village!");

Item tome = (id: "tome", name: "ancient tome", description: "A heavy book with faded runes.");
tome.SetReadable()
    .RequireTakeToRead()
    .SetReadingCost(3)
    .SetReadText("The secret to defeating the dragon is...");

Item letter = (id: "letter", name: "sealed letter", description: "A sealed letter with red wax.");
letter.SetReadable()
    .RequireTakeToRead()
    .RequiresToRead(s => s.Inventory.Items.Any(i => i.Id == "lantern"))
    .SetReadText("Meet me at midnight...");

// Create locations
Location entrance = (
    id: "entrance", 
    description: "You stand at the forest gate. It's dark and foreboding."
    );
Location forest = (
    id: "forest", 
    description: "A thick forest surrounds you. A fox watches from the brush."
    );
Location cave = (
    id: "cave", 
    description: "A dark cave with glowing mushrooms. A brass key glints on the ground!"
    );
Location deepCave = (
    id: "deep_cave",
    description: "The cave narrows into a jagged tunnel. You hear a distant rumble."
    );
Location clearing = (
    id: "clearing", description: "A sunny clearing with wildflowers. A small cabin stands here."
    );
Location cabin = (
    id: "cabin", 
    description: "Inside a cozy wooden cabin. A treasure chest sits in the corner!"
    );

// Place items
cave.AddItem(cabinKey);
entrance.AddItem(ice);
entrance.AddItem(sign);
forest.AddItem(apple);
forest.AddItem(fire);
forest.AddItem(lantern);
clearing.AddItem(sword);
clearing.AddItem(glass);
clearing.AddItem(tome);
cave.AddItem(letter);
cabin.AddItem(newspaper);

// Create NPCs
var fox = new Npc("fox", "fox")
    .Description("A curious fox with bright eyes.")
    .SetDialog(new DialogNode("The fox tilts its head, listening.")
        .AddOption("Ask about the cabin")
        .AddOption("Ask about the cave"));

var dragon = new Npc("dragon", "dragon", NpcState.Hostile)
    .Description("A massive dragon with ember-bright eyes.")
    .Dialog("The dragon rumbles a warning.");
dragon.SetMovement(new PatrolNpcMovement(new[] { cave, deepCave }));

forest.AddNpc(fox);
cave.AddNpc(dragon);

// Create locked door
Door cabinDoor = (id: "cabin_door", name: "cabin door", description: "A sturdy wooden door with iron hinges.");
cabinDoor.RequiresKey(cabinKey);
cabinDoor
    .SetReaction(DoorAction.Unlock, "The lock clicks open.")
    .SetReaction(DoorAction.Open, "The door creaks as it swings wide.");

// Connect locations
entrance.AddExit(Direction.North, forest);
forest.AddExit(Direction.East, cave);
forest.AddExit(Direction.West, clearing);
cave.AddExit(Direction.North, deepCave);
clearing.AddExit(Direction.In, cabin, cabinDoor);  // Locked!

// One-way trap
cave.AddExit(Direction.Down, entrance, oneWay: true);

// Game state
var recipeBook = new RecipeBook()
    .Add(new ItemCombinationRecipe("ice", "fire", () => new FluidItem("water", "water", "Clear and cold.")));
var state = new GameState(entrance, recipeBook: recipeBook);
var locations = new List<Location> { entrance, forest, cave, deepCave, clearing, cabin };

Console.WriteLine("=== FOREST ADVENTURE ===");
Console.WriteLine("Find the key and unlock the cabin!");
var commands = new[]
{
    "go", "look", "talk", "read", "open", "unlock", "take", "drop", "use", "inventory", "stats", "combine", "pour", "quit"
};
Console.WriteLine($"Commands: {commands.CommaJoin()} (or just type a direction)\n");

var parserConfig = new KeywordParserConfig(
    quit: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "quit", "exit", "q" },
    look: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "look", "l", "ls" },
    inventory: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "inventory", "inv", "i" },
    stats: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "stats", "stat", "hp", "health" },
    open: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "open" },
    unlock: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "unlock" },
    take: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "take", "get", "pickup", "pick" },
    drop: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "drop" },
    use: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "use", "eat", "bite" },
    combine: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "combine", "mix" },
    pour: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "pour" },
    go: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "go", "move", "cd" },
    read: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "read" },
    talk: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "talk", "speak" },
    all: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "all" },
    ignoreItemTokens: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "up", "to" },
    combineSeparators: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "and", "+" },
    pourPrepositions: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "into", "in" },
    directionAliases: new Dictionary<string, Direction>(StringComparer.OrdinalIgnoreCase)
    {
        ["n"] = Direction.North,
        ["s"] = Direction.South,
        ["e"] = Direction.East,
        ["w"] = Direction.West,
        ["ne"] = Direction.NorthEast,
        ["nw"] = Direction.NorthWest,
        ["se"] = Direction.SouthEast,
        ["sw"] = Direction.SouthWest,
        ["u"] = Direction.Up,
        ["d"] = Direction.Down,
        ["in"] = Direction.In,
        ["out"] = Direction.Out
    },
    allowDirectionEnumNames: true);

var parser = new KeywordParser(parserConfig);

void MoveNpcs()
{
    var moves = new List<(INpc npc, Location from, Location to)>();

    foreach (var location in locations)
    {
        foreach (var npc in location.Npcs.ToList())
        {
            var next = npc.GetNextLocation(location, state);
            if (next is Location nextLocation && !ReferenceEquals(nextLocation, location))
            {
                moves.Add((npc, location, nextLocation));
            }
        }
    }

    foreach (var (npc, from, to) in moves)
    {
        from.RemoveNpc(npc);
        to.AddNpc(npc);
    }
}

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

    MoveNpcs();

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
