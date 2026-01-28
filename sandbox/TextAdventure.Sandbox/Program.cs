using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;

// Slice 10 — Save/Load (Memento)
// New functions tested:
// - SaveCommand, LoadCommand
// - JsonSaveSystem persistence via GameMemento
// - KeywordParserConfigBuilder + KeywordParser

// Locations
var outside = new Location("outside", "Snow falls quietly outside a locked library.");
var library = new Location("library", "Warm light and quiet pages surround you.");

outside.AddExit(Direction.In, library);
library.AddExit(Direction.Out, outside);

// Items
var key = new Key("library_key", "library key", "Cold metal in your hand.")
    .AddAliases("key", "brass key")
    .SetReaction(ItemAction.Take, "You pocket the key. It chills your palm and your resolve.");

var snow = new Item("snow", "snow", "Soft, patient snowflakes that refuse to melt for your convenience.")
    .SetTakeable(false)
    .AddAliases("snowfall", "flakes")
    .SetReaction(ItemAction.Move, "You brush the snow aside. It drifts back with quiet dignity.")
    .SetReaction(ItemAction.TakeFailed, "You attempt to gather the snow. It slips away, disinterested.");

var book = new Item("book", "old book", "An old book, warm from long companionship.")
    .AddAliases("book", "volume")
    .SetReaction(ItemAction.Use, "You open it at random and immediately feel better about the world.");

outside.AddItem(key);
outside.AddItem(snow);
library.AddItem(book);

// Door
var door = new Door("library_door", "library door", "A heavy wooden door with iron fittings.")
    .RequiresKey(key)
    .SetReaction(DoorAction.Unlock, "The library door unlocks with a polite click.")
    .SetReaction(DoorAction.Open, "The door swings inward, warm air rolling out to greet you.")
    .SetReaction(DoorAction.UnlockFailed, "The lock refuses to concede.");

outside.AddExit(Direction.In, library, door);

// Game state
var state = new GameState(outside, worldLocations: new[] { outside, library })
{
    EnableFuzzyMatching = true,
    FuzzyMaxDistance = 1
};

// Parser config (save/load enabled)
var parserConfig = KeywordParserConfigBuilder.BritishDefaults()
    .WithLook("look", "l")
    .WithExamine("examine", "exam", "x", "check", "inspect")
    .WithInventory("inventory", "inv", "i")
    .WithTake("take", "get")
    .WithDrop("drop")
    .WithUse("use")
    .WithMove("move", "push", "shift", "slide")
    .WithGo("go")
    .WithOpen("open")
    .WithUnlock("unlock")
    .WithSave("save")
    .WithLoad("load")
    .WithQuit("quit", "exit")
    .WithIgnoreItemTokens("on", "off", "at", "the", "a")
    .WithFuzzyMatching(true, 1)
    .Build();

var parser = new KeywordParser(parserConfig);

Console.WriteLine("=== THE WARM LIBRARY (Slice 10) ===");
Console.WriteLine("Goal: get inside, save, quit, load, and continue.");
Console.WriteLine("Type 'help' for a quick command list.");
ShowRoom();

while (true)
{
    Console.Write("\n> ");
    var input = Console.ReadLine()?.Trim();
    if (string.IsNullOrEmpty(input)) continue;

    if (IsHelp(input))
    {
        ShowHelp();
        continue;
    }

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

void ShowRoom()
{
    var location = state.CurrentLocation;
    Console.WriteLine();
    Console.WriteLine($"Room: {location.Id.ToProperCase()}");
    Console.WriteLine(location.GetDescription());

    var items = location.Items.Select(item => item.Name.ToProperCase()).ToList();
    Console.WriteLine(items.Count > 0
        ? $"Items: {items.CommaJoin()}"
        : "Items: None");

    var exits = location.Exits.Keys
        .Select(direction => direction.ToString().ToLowerInvariant().ToProperCase())
        .ToList();
    Console.WriteLine(exits.Count > 0
        ? $"Exits: {exits.CommaJoin()}"
        : "Exits: None");
}

void ShowHelp()
{
    Console.WriteLine("Commands: look, examine/check <item>, take <item>, use <item>, move <item>, unlock/open door, go <direction>, inventory, quit");
    Console.WriteLine("Save/Load: save, load (defaults to savegame.json)");
}

bool IsHelp(string input)
{
    var normalized = input.Lower();
    return normalized is "help" or "halp" or "?";
}
