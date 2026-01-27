# Forest Adventure (Core API, no shortcuts)

_Slice tag: Slice 4–10 — Foundation demo spanning items, inventory, reactions, NPCs, combat, quests, worldstate, save/load._

This version uses the core API and fluent methods, but **no implicit operators**, **no helper shortcuts**, and **no extensions**.

```csharp
using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Helpers;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;

// Items
var cabinKey = new Key("cabin_key", "brass key", "A small brass key with worn teeth.")
    .SetWeight(0.1f)
    .AddAliases("key", "brass key");

var sword = new Item("sword", "rusty sword", "A rusted blade with a dull edge.")
    .SetWeight(3.5f)
    .AddAliases("blade", "sword", "pointy thing");

var apple = new Item("apple", "red apple", "A crisp red apple.")
    .SetWeight(0.4f)
    .AddAliases("apple");

// GameItemList (CRUD, id from name)
var extraItems = new GameItemList()
    .AddMany("cat", "rubber chicken", "map", "shovel");
extraItems["cat"].SetWeight(3f).AddAliases("kitten", "kitteh");
extraItems["rubber chicken"].SetWeight(0.8f).AddAliases("chicken", "rubberchicken");
extraItems["map"].SetWeight(0.2f).AddAliases("parchment", "chart");
extraItems["shovel"].SetWeight(2.5f).AddAliases("spade");

// Wrapper lists for keys and doors
var keyList = new KeyList()
    .AddMany("shed key");
keyList["shed key"].SetWeight(0.2f).AddAliases("shedkey");

var doorList = new DoorList()
    .AddMany("shed door");
doorList["shed door"]
    .RequiresKey(keyList["shed key"])
    .SetReaction(DoorAction.Unlock, "The shed door unlocks with a click.");

var glass = new Glass("glass", "glass", "A clear drinking glass.")
    .SetWeight(0.6f)
    .SetReaction(ItemAction.Take, "The glass surface is smooth")
    .SetReaction(ItemAction.Drop, "The glass bounces on the floor")
    .SetReaction(ItemAction.Destroy, "The glass shatters into 1000 pieces");

var ice = new Item("ice", "ice", "A cold chunk of ice.")
    .SetWeight(0.5f)
    .SetReaction(ItemAction.Take, "The cold chills your hand.")
    .SetReaction(ItemAction.Drop, "It lands with a soft thump.")
    .SetReaction(ItemAction.Use, "You take a bite. Your teeth ache.");

var fire = new Item("fire", "fire", "A flickering flame.")
    .SetWeight(0.5f);

var lantern = new Item("lantern", "lantern", "A lantern that casts a warm glow.")
    .SetWeight(1.2f);

var sign = new Item("sign", "wooden sign", "A weathered wooden sign.")
    .SetTakeable(false)
    .SetReadable()
    .SetReadText("Welcome to the Dark Forest!");

var newspaper = new Item("newspaper", "daily news", "A crinkled newspaper.")
    .SetReadable()
    .RequireTakeToRead()
    .SetReadText("HEADLINE: Dragon spotted near village!");

var tome = new Item("tome", "ancient tome", "A heavy book with faded runes.")
    .SetReadable()
    .RequireTakeToRead()
    .SetReadingCost(3)
    .SetReadText("The secret to defeating the dragon is...");

var letter = new Item("letter", "sealed letter", "A sealed letter with red wax.")
    .SetReadable()
    .RequireTakeToRead()
    .RequiresToRead(state => state.Inventory.Items.Any(i => i.Id == "lantern"))
    .SetReadText("Meet me at midnight...");

// Locations
var entrance = new Location("entrance", "You stand at the forest gate. It's dark and foreboding.");
var forest = new Location("forest", "A thick forest surrounds you. A fox watches from the brush.");
var cave = new Location("cave", "A dark cave with glowing mushrooms. A brass key glints on the ground!");
var deepCave = new Location("deep_cave", "The cave narrows into a jagged tunnel. You hear a distant rumble.");
var clearing = new Location("clearing", "A sunny clearing with wildflowers. A small cabin stands here.");
var cabin = new Location("cabin", "Inside a cozy wooden cabin. A treasure chest sits in the corner!");
var locationList = new LocationList();
var shed = locationList.Add("shed", "A small wooden shed with tools hanging on the walls.");

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
var cabinDoor = new Door("cabin_door", "cabin door", "A sturdy wooden door with iron hinges.")
    .RequiresKey(cabinKey)
    .SetReaction(DoorAction.Unlock, "The lock clicks open.")
    .SetReaction(DoorAction.Open, "The door creaks as it swings wide.");

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
var npcList = new NpcList()
    .AddMany("fox", "dragon");
var fox = npcList["fox"]
    .Description("A curious fox with bright eyes.")
    .SetDialog(new DialogNode("The fox tilts its head, listening.")
        .AddOption("Ask about the cabin")
        .AddOption("Ask about the cave"));

var dragonPatrol = new PatrolNpcMovement(new[] { cave, deepCave });
var dragon = npcList["dragon"]
    .SetState(NpcState.Friendly)
    .SetStats(new Stats(40))
    .Description("A massive dragon sleeps among the shadows.")
    .Dialog("The dragon snores softly.")
    .SetMovement(new NoNpcMovement());

forest.AddNpc(fox);
cave.AddNpc(dragon);

// Recipes
var recipeBook = new RecipeBook()
    .Add(new ItemCombinationRecipe("ice", "fire", () => new FluidItem("water", "water", "Clear and cold.")));

// Game state
var state = new GameState(entrance, recipeBook: recipeBook);
var locations = new List<Location> { entrance, forest, cave, deepCave, clearing, cabin, shed };
state.RegisterLocations(locations);
var dragonAwake = false;

// Quest
var dragonHunt = new Quest("dragon_hunt", "Dragon Hunt", "Find the sword and slay the dragon.")
    .AddCondition(new HasItemCondition("sword"))
    .AddCondition(new NpcStateCondition(dragon, NpcState.Dead))
    .AddCondition(new WorldFlagCondition("dragon_defeated"))
    .AddCondition(new WorldCounterCondition("villagers_saved", 1))
    .AddCondition(new RelationshipCondition("fox", 2))
    .Start();

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

state.Events.Subscribe(GameEventType.EnterLocation, e =>
{
    state.WorldState.Increment("days_elapsed");
    if (e.Location != null)
    {
        state.WorldState.AddTimeline($"Entered {e.Location.Id}.");
    }
});

state.Events.Subscribe(GameEventType.PickupItem, e =>
{
    if (e.Item != null && e.Item.Id == "apple")
    {
        state.WorldState.Increment("villagers_saved");
    }
});

state.Events.Subscribe(GameEventType.TalkToNpc, e =>
{
    if (e.Npc != null && e.Npc.Id == "fox")
    {
        var reputation = state.WorldState.GetRelationship("fox") + 1;
        state.WorldState.SetRelationship("fox", reputation);
        if (reputation >= 2)
        {
            e.Npc.SetDialog(new DialogNode("The fox seems to trust you now.")
                .AddOption("Ask about the dragon")
                .AddOption("Ask about the shed"));
        }
    }
});

// Parser config
var parserConfig = new KeywordParserConfig(
    quit: CommandHelper.NewCommands("quit", "exit", "q"),
    look: CommandHelper.NewCommands("look", "l", "ls"),
    examine: CommandHelper.NewCommands("examine", "x"),
    inventory: CommandHelper.NewCommands("inventory", "inv", "i"),
    stats: CommandHelper.NewCommands("stats", "stat", "hp", "health"),
    open: CommandHelper.NewCommands("open"),
    unlock: CommandHelper.NewCommands("unlock"),
    take: CommandHelper.NewCommands("take", "get", "pickup", "pick"),
    drop: CommandHelper.NewCommands("drop"),
    use: CommandHelper.NewCommands("use", "eat", "bite"),
    combine: CommandHelper.NewCommands("combine", "mix"),
    pour: CommandHelper.NewCommands("pour"),
    move: CommandHelper.NewCommands("move", "push", "shift", "lift", "slide"),
    go: CommandHelper.NewCommands("go", "move", "cd"),
    read: CommandHelper.NewCommands("read"),
    talk: CommandHelper.NewCommands("talk", "speak"),
    attack: CommandHelper.NewCommands("attack", "fight"),
    flee: CommandHelper.NewCommands("flee", "run"),
    save: CommandHelper.NewCommands("save"),
    load: CommandHelper.NewCommands("load"),
    quest: CommandHelper.NewCommands("quest"),
    all: CommandHelper.NewCommands("all"),
    ignoreItemTokens: CommandHelper.NewCommands("up", "to"),
    combineSeparators: CommandHelper.NewCommands("and", "+"),
    pourPrepositions: CommandHelper.NewCommands("into", "in"),
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
    allowDirectionEnumNames: true,
    enableFuzzyMatching: true,
    fuzzyMaxDistance: 1);

var parser = new KeywordParser(parserConfig);

while (true)
{
    Console.Write("\n> ");
    var input = Console.ReadLine()?.Trim();
    if (string.IsNullOrWhiteSpace(input)) continue;

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

    if (result.ShouldQuit) break;
}
```
