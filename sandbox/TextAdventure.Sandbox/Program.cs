using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Parsing;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Helpers;
using MarcusMedina.TextAdventure.Localization;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Dsl;

var languagePath = Path.Combine(AppContext.BaseDirectory, "lang", "gamelang.sv.txt");
if (File.Exists(languagePath))
{
    Language.SetProvider(new FileLanguageProvider(languagePath));
}

var adventurePath = Path.Combine(AppContext.BaseDirectory, "forest.adventure");
var adventure = new AdventureDslParser().ParseFile(adventurePath);
var state = adventure.State;

// Locations from DSL
var entrance = adventure.Locations["entrance"];
var forest = adventure.Locations["forest"];
var cave = adventure.Locations["cave"];
var deepCave = adventure.Locations["deep_cave"];
var clearing = adventure.Locations["clearing"];
var cabin = adventure.Locations["cabin"];
var shed = adventure.Locations["shed"];

// Items and keys from DSL
var ice = adventure.Items["ice"];
var sign = adventure.Items["sign"];
var fire = adventure.Items["fire"];
var letter = adventure.Items["letter"];
var glass = adventure.Items["glass"];
var tome = adventure.Items["tome"];
var newspaper = adventure.Items["newspaper"];

ice.SetReaction(ItemAction.Take, "The cold chills your hand.")
    .SetReaction(ItemAction.Drop, "It lands with a soft thump.")
    .SetReaction(ItemAction.Use, "You take a bite. Your teeth ache.");

sign.SetTakeable(false)
    .SetReadable()
    .SetReadText("Welcome to the Dark Forest!");

fire.SetWeight(0.5f);

letter.SetReadable()
    .RequireTakeToRead()
    .RequiresToRead(s => s.Inventory.Items.Any(i => i.Id == "lantern"))
    .SetReadText("🎶 Meet me at midnight... 🎵");

glass.SetReaction(ItemAction.Take, "The glas surface is smooth")
    .SetReaction(ItemAction.Drop, "The glass bounces on the floor")
    .SetReaction(ItemAction.Destroy, "The glass shatters into 1000 pieces");

tome.SetReadable()
    .RequireTakeToRead()
    .SetReadingCost(3)
    .SetReadText("The secret to defeating the dragon is...");

newspaper.SetReadable()
    .RequireTakeToRead()
    .SetReadText("HEADLINE: Dragon spotted near village!");

// Extras (still showing different creation styles)
var extraItems = new GameItemList()
    .AddMany("Cat", "Rubber chicken", "Map", "Shovel");
extraItems["cat"].SetWeight(3f).AddAliases("kitten", "kitteh");
extraItems["rubber chicken"].SetWeight(0.8f).AddAliases("chicken", "rubberchicken");
extraItems["map"].SetWeight(0.2f).AddAliases("parchment", "chart");
extraItems["shovel"].SetWeight(2.5f).AddAliases("spade");

var itemList = new ItemList()
    .AddMany("Compass", "Blanket");
itemList["compass"].SetWeight(0.3f).AddAliases("northfinder");
itemList["blanket"].SetWeight(1.0f);

var keyList = new KeyList()
    .AddMany("watchtower key");
keyList["watchtower key"].SetWeight(0.2f).AddAliases("watchkey");

var doorList = new DoorList()
    .AddMany("watchtower door");
doorList["watchtower door"]
    .RequiresKey(keyList["watchtower key"])
    .SetReaction(DoorAction.Unlock, "The watchtower door unlocks with a click.");

var locationList = new LocationList();
var watchtower = locationList.Add("watchtower", "A wind-bitten watchtower overlooking the forest.");
var garden = locationList.Add("garden", "A quiet garden. A flat stone rests near an old gate.");
var courtyard = locationList.Add("courtyard", "A small courtyard where something waits.");

entrance.AddItem(extraItems["map"]);
entrance.AddItem(keyList["watchtower key"]);
forest.AddItem(extraItems["cat"]);
clearing.AddItem(extraItems["rubber chicken"]);
shed.AddItem(extraItems["shovel"]);
watchtower.AddItem(itemList["compass"]);
cabin.AddItem(itemList["blanket"]);
garden.AddItem(new Item("stone", "stone", "A heavy flat stone."));

forest.AddExit(Direction.NorthEast, watchtower, doorList["watchtower door"]);
clearing.AddExit(Direction.South, garden);

var gardenKey = new Key("garden_key", "iron key", "A small iron key.");
var gardenGate = new Door("garden_gate", "garden gate", "An old iron gate.")
    .RequiresKey(gardenKey)
    .SetReaction(DoorAction.Unlock, "The gate creaks open.");
garden.AddExit(Direction.Out, courtyard, gardenGate);

// Doors from DSL
var cabinDoor = adventure.Doors["cabin_door"];
var shedDoor = adventure.Doors["shed_door"];

cabinDoor
    .SetReaction(DoorAction.Unlock, "The lock clicks open.")
    .SetReaction(DoorAction.Open, "The door creaks as it swings wide.");

shedDoor.SetReaction(DoorAction.Unlock, "The shed door unlocks with a click.");

// Register extra locations for save/load
state.RegisterLocations(new[] { watchtower, garden, courtyard });

// Create NPCs
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
state.RecipeBook.Add(new ItemCombinationRecipe("ice", "fire", () => new FluidItem("water", "water", "Clear and cold.")));
var dragonAwake = false;
var dragonHunt = new Quest("dragon_hunt", "Dragon Hunt", "Find the sword and slay the dragon.")
    .AddCondition(new HasItemCondition("sword"))
    .AddCondition(new NpcStateCondition(dragon, NpcState.Dead))
    .AddCondition(new WorldFlagCondition("dragon_defeated"))
    .AddCondition(new WorldCounterCondition("villagers_saved", 1))
    .AddCondition(new RelationshipCondition("fox", 2))
    .Start();

state.Events.Subscribe(GameEventType.EnterLocation, e =>
{
    if (dragonAwake) return;
    if (e.Location != null && e.Location.Id.TextCompare("cave"))
    {
        dragonAwake = true;
        dragon.SetState(NpcState.Hostile);
        dragon.Description("A massive dragon rises, eyes blazing.");
        dragon.Dialog("The dragon roars as it wakes.");
        dragon.SetMovement(dragonPatrol);
        Console.WriteLine("\nThe dragon stirs and awakens!");
    }
});

state.Events.Subscribe(GameEventType.PickupItem, e =>
{
    if (e.Item == null) return;
    if (e.Item.Id.TextCompare("stone"))
    {
        if (!garden.Items.Contains(gardenKey) && !state.Inventory.Items.Contains(gardenKey))
        {
            garden.AddItem(gardenKey);
            Console.WriteLine("You lift the stone and find a key beneath it.");
        }
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
    if (e.Item == null) return;
    if (e.Item.Id == "apple")
    {
        state.WorldState.Increment("villagers_saved");
    }
});

state.Events.Subscribe(GameEventType.TalkToNpc, e =>
{
    if (e.Npc == null) return;
    if (e.Npc.Id == "fox")
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

Console.WriteLine("=== FOREST ADVENTURE ===");
Console.WriteLine("Find the key and unlock the cabin!");
Console.WriteLine($"Quest started: {dragonHunt.Title} - {dragonHunt.Description}");
var commands = new[]
{
    "go", "look", "talk", "attack", "flee", "read", "open", "unlock", "take", "drop", "use", "inventory", "stats", "combine", "pour", "save", "load", "quit"
};
Console.WriteLine($"Commands: {commands.CommaJoin()} (or just type a direction)\n");

var parserConfig = new KeywordParserConfig(
    quit: CommandHelper.NewCommands("quit", "exit", "q"),
    look: CommandHelper.NewCommands("look", "l", "ls"),
    inventory: CommandHelper.NewCommands("inventory", "inv", "i"),
    stats: CommandHelper.NewCommands("stats", "stat", "hp", "health"),
    open: CommandHelper.NewCommands("open"),
    unlock: CommandHelper.NewCommands("unlock"),
    take: CommandHelper.NewCommands("take", "get", "pickup", "pick"),
    drop: CommandHelper.NewCommands("drop"),
    use: CommandHelper.NewCommands("use", "eat", "bite"),
    combine: CommandHelper.NewCommands("combine", "mix"),
    pour: CommandHelper.NewCommands("pour"),
    go: CommandHelper.NewCommands("go", "move", "cd"),
    read: CommandHelper.NewCommands("read"),
    talk: CommandHelper.NewCommands("talk", "speak"),
    attack: CommandHelper.NewCommands("attack", "fight"),
    flee: CommandHelper.NewCommands("flee", "run"),
    save: CommandHelper.NewCommands("save"),
    load: CommandHelper.NewCommands("load"),
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
    allowDirectionEnumNames: true);

var parser = new KeywordParser(parserConfig);

var game = GameBuilder.Create()
    .UseState(state)
    .UseParser(parser)
    .UsePrompt("> ")
    .AddTurnStart(g =>
    {
        var lookResult = g.State.Look();
        g.Output.WriteLine($"\n{lookResult.Message}");
        var inventoryResult = g.State.InventoryView();
        g.Output.WriteLine(inventoryResult.Message);
    })
    .AddTurnEnd((g, command, result) =>
    {
        if (!g.State.WorldState.GetFlag("dragon_defeated") && !dragon.IsAlive)
        {
            g.State.WorldState.SetFlag("dragon_defeated", true);
            g.State.WorldState.AddTimeline("Dragon defeated.");
        }

        if (dragonHunt.CheckProgress(g.State))
        {
            g.Output.WriteLine($"\n*** QUEST COMPLETE: {dragonHunt.Title}! ***");
        }

        if (command is GoCommand go && result.Success)
        {
            if (g.State.IsCurrentRoomId("cabin"))
            {
                g.Output.WriteLine("\n*** CONGRATULATIONS! You found the treasure! ***");
                g.RequestStop();
            }
            else if (go.Direction == Direction.Down && g.State.CurrentLocation.Id == "entrance")
            {
                g.Output.WriteLine("Oops! You fell through a hole and ended up back at the entrance!");
            }
        }
    })
    .Build();

game.Run();

Console.WriteLine("\nThanks for playing!");
