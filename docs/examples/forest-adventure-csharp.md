# Forest Adventure (Classic C# style)

This version is intentionally “plain C#”: explicit constructors, explicit collections, minimal fluent chaining, and no shortcuts.

```csharp
using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;

// Items
var cabinKey = new Key("cabin_key", "brass key", "A small brass key with worn teeth.");
cabinKey.SetWeight(0.1f);
cabinKey.AddAliases("key", "brass key");

var sword = new Item("sword", "rusty sword", "A rusted blade with a dull edge.");
sword.SetWeight(3.5f);
sword.AddAliases("blade", "sword", "pointy thing");

var apple = new Item("apple", "red apple", "A crisp red apple.");
apple.SetWeight(0.4f);
apple.AddAliases("apple");

// GameItemList (CRUD, id from name)
var extraItems = new GameItemList();
extraItems.AddMany("cat", "rubber chicken", "map", "shovel");
extraItems["cat"].SetWeight(3f);
extraItems["cat"].AddAliases("kitten", "kitteh");
extraItems["rubber chicken"].SetWeight(0.8f);
extraItems["map"].SetWeight(0.2f);
extraItems["shovel"].SetWeight(2.5f);

// Wrapper lists for keys and doors
var keyList = new KeyList();
keyList.AddMany("shed key");
keyList["shed key"].SetWeight(0.2f);
keyList["shed key"].AddAliases("shedkey");

var doorList = new DoorList();
doorList.AddMany("shed door");
doorList["shed door"].RequiresKey(keyList["shed key"]);
doorList["shed door"].SetReaction(DoorAction.Unlock, "The shed door unlocks with a click.");

var glass = new Glass("glass", "glass", "A clear drinking glass.");
glass.SetWeight(0.6f);
glass.SetReaction(ItemAction.Take, "The glass surface is smooth");
glass.SetReaction(ItemAction.Drop, "The glass bounces on the floor");
glass.SetReaction(ItemAction.Destroy, "The glass shatters into 1000 pieces");

var ice = new Item("ice", "ice", "A cold chunk of ice.");
ice.SetWeight(0.5f);
ice.SetReaction(ItemAction.Take, "The cold chills your hand.");
ice.SetReaction(ItemAction.Drop, "It lands with a soft thump.");
ice.SetReaction(ItemAction.Use, "You take a bite. Your teeth ache.");

var fire = new Item("fire", "fire", "A flickering flame.");
fire.SetWeight(0.5f);

var lantern = new Item("lantern", "lantern", "A lantern that casts a warm glow.");
lantern.SetWeight(1.2f);

var sign = new Item("sign", "wooden sign", "A weathered wooden sign.");
sign.SetTakeable(false);
sign.SetReadable();
sign.SetReadText("Welcome to the Dark Forest!");

var newspaper = new Item("newspaper", "daily news", "A crinkled newspaper.");
newspaper.SetReadable();
newspaper.RequireTakeToRead();
newspaper.SetReadText("HEADLINE: Dragon spotted near village!");

var tome = new Item("tome", "ancient tome", "A heavy book with faded runes.");
tome.SetReadable();
tome.RequireTakeToRead();
tome.SetReadingCost(3);
tome.SetReadText("The secret to defeating the dragon is...");

var letter = new Item("letter", "sealed letter", "A sealed letter with red wax.");
letter.SetReadable();
letter.RequireTakeToRead();
letter.RequiresToRead(state => state.Inventory.Items.Any(i => i.Id == "lantern"));
letter.SetReadText("Meet me at midnight...");

// Locations
var entrance = new Location("entrance", "You stand at the forest gate. It's dark and foreboding.");
var forest = new Location("forest", "A thick forest surrounds you. A fox watches from the brush.");
var cave = new Location("cave", "A dark cave with glowing mushrooms. A brass key glints on the ground!");
var deepCave = new Location("deep_cave", "The cave narrows into a jagged tunnel. You hear a distant rumble.");
var clearing = new Location("clearing", "A sunny clearing with wildflowers. A small cabin stands here.");
var cabin = new Location("cabin", "Inside a cozy wooden cabin. A treasure chest sits in the corner!");
var shed = new Location("shed", "A small wooden shed with tools hanging on the walls.");

// Place items
cave.AddItem(cabinKey);
entrance.AddItem(ice);
entrance.AddItem(sign);
entrance.AddItem(extraItems["map"]);
forest.AddItem(apple);
forest.AddItem(fire);
forest.AddItem(lantern);
forest.AddItem(extraItems["cat"]);
forest.AddItem(keyList["shed key"]);
clearing.AddItem(sword);
clearing.AddItem(glass);
clearing.AddItem(tome);
clearing.AddItem(extraItems["rubber chicken"]);
shed.AddItem(extraItems["shovel"]);
cave.AddItem(letter);
cabin.AddItem(newspaper);

// Door
var cabinDoor = new Door("cabin_door", "cabin door", "A sturdy wooden door with iron hinges.");
cabinDoor.RequiresKey(cabinKey);
cabinDoor.SetReaction(DoorAction.Unlock, "The lock clicks open.");
cabinDoor.SetReaction(DoorAction.Open, "The door creaks as it swings wide.");

// Exits
entrance.AddExit(Direction.North, forest);
forest.AddExit(Direction.East, cave);
forest.AddExit(Direction.West, clearing);
cave.AddExit(Direction.North, deepCave);
clearing.AddExit(Direction.In, cabin, cabinDoor);
clearing.AddExit(Direction.North, shed, doorList["shed door"]);

// One-way trap
cave.AddExit(Direction.Down, entrance, oneWay: true);

// NPCs
var npcList = new NpcList();
npcList.AddMany("fox", "dragon");

var fox = npcList["fox"];
fox.Description("A curious fox with bright eyes.");
fox.SetDialog(new DialogNode("The fox tilts its head, listening.")
    .AddOption("Ask about the cabin")
    .AddOption("Ask about the cave"));

var dragonPatrol = new PatrolNpcMovement(new[] { cave, deepCave });
var dragon = npcList["dragon"];
dragon.SetState(NpcState.Friendly);
dragon.SetStats(new Stats(40));
dragon.Description("A massive dragon sleeps among the shadows.");
dragon.Dialog("The dragon snores softly.");
dragon.SetMovement(new NoNpcMovement());

forest.AddNpc(fox);
cave.AddNpc(dragon);

// Recipes
var recipeBook = new RecipeBook();
recipeBook.Add(new ItemCombinationRecipe("ice", "fire", () => new FluidItem("water", "water", "Clear and cold.")));

// Game state
var state = new GameState(entrance, recipeBook: recipeBook);
var locations = new List<Location> { entrance, forest, cave, deepCave, clearing, cabin, shed };
var dragonAwake = false;

// Quest
var dragonHunt = new Quest("dragon_hunt", "Dragon Hunt", "Find the sword and slay the dragon.");
dragonHunt.AddCondition(new HasItemCondition("sword"));
dragonHunt.AddCondition(new NpcStateCondition(dragon, NpcState.Dead));
dragonHunt.Start();

// Events
state.Events.Subscribe(GameEventType.EnterLocation, e =>
{
    if (dragonAwake) return;
    if (e.Location != null && e.Location.Id.Equals("cave", StringComparison.OrdinalIgnoreCase))
    {
        dragonAwake = true;
        dragon.SetState(NpcState.Hostile);
        dragon.Description("A massive dragon rises, eyes blazing.");
        dragon.Dialog("The dragon roars as it wakes.");
        dragon.SetMovement(dragonPatrol);
        Console.WriteLine("\nThe dragon stirs and awakens!");
    }
});

// Parser config
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
    attack: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "attack", "fight" },
    flee: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "flee", "run" },
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
```
