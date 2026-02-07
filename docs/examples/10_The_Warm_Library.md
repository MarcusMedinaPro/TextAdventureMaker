# The Warm Library

_Slice tag: Slice 10 — Save/Load (Memento). Demo focuses on saving and restoring a cozy scene._

## Story beats (max ~10 steps)
1) You stand outside a locked library.
2) Find the key in the snow.
3) Enter the warm library.
4) Save your progress.
5) Load it later.


## Map (rough layout)
```
          N
    W           E
          S

┌────────────┐     ┌────────────┐
│            │     │            │
│  Outside   │─────│  Library   │
│            │     │            │
│     K      │     │     B      │
└────────────┘     └────────────┘

K = Library key
B = Book
```

## Example (save/load)
```csharp
using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;

GameState state = BuildGameState();
KeywordParser parser = BuildParser();
// Console setup for C64 aesthetics
Console.BackgroundColor = ConsoleColor.DarkBlue;
Console.ForegroundColor = ConsoleColor.Cyan;
Console.Title = "The Warm Library - Text Adventure Sandbox";
Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.Clear();
// End console setup

Console.WriteLine("=== THE WARM LIBRARY (Slice 10) ===");
Console.WriteLine("Goal: unlock the library door, save your progress, quit, then load and continue.");
Console.WriteLine("Type 'help' for the commands you can use.");
ShowRoom();

while (true)
{
    Console.Write("\n> ");
    string? input = Console.ReadLine()?.Trim();
    if (string.IsNullOrWhiteSpace(input))
    {
        continue;
    }

    if (IsHelp(input))
    {
        ShowHelp();
        continue;
    }

    ICommand command = parser.Parse(input);
    CommandResult result = state.Execute(command);

    DisplayResult(result);

    if (ShouldShowRoom(command, result))
    {
        ShowRoom();
    }

    if (result.ShouldQuit)
    {
        break;
    }
}

static GameState BuildGameState()
{
    Location outside = new Location("outside", "Snow falls quietly outside a locked library.");
    Location library = new Location("library", "Warm light and quiet pages surround you.");

    Key key = new Key("library_key", "library key", "Cold metal in your hand.")
        .AddAliases("key", "brass key")
        .SetReaction(ItemAction.Take, "You pocket the key. It chills your palm and your resolve.");

    Item snow = new Item("snow", "snow", "Soft, patient snowflakes that refuse to melt for your convenience.");
    _ = snow.SetTakeable(false)
        .AddAliases("snowfall", "flakes")
        .SetReaction(ItemAction.Move, "You brush the snow aside. It drifts back with quiet dignity.")
        .SetReaction(ItemAction.TakeFailed, "You attempt to gather the snow. It slips away, disinterested.");

    Item book = new Item("book", "old book", "An old book, warm from long companionship.");
    _ = book.AddAliases("book", "volume")
        .SetReaction(ItemAction.Use, "You open it at random and immediately feel better about the world.");

    outside.AddItem(key);
    outside.AddItem(snow);
    library.AddItem(book);

    Door door = new Door("library_door", "library door", "A heavy wooden door with iron fittings.")
        .AddAliases("door", "library")
        .RequiresKey(key)
        .SetReaction(DoorAction.Unlock, "The library door unlocks with a polite click.")
        .SetReaction(DoorAction.Open, "The door swings inward, warm air rolling out to greet you.")
        .SetReaction(DoorAction.UnlockFailed, "The lock refuses to concede.");

    outside.AddExit(Direction.In, library, door);
    library.AddExit(Direction.Out, outside, door);

    return new GameState(outside, worldLocations: [outside, library])
    {
        EnableFuzzyMatching = true,
        FuzzyMaxDistance = 1
    };
}

static KeywordParser BuildParser()
{
    KeywordParserConfig config = KeywordParserConfigBuilder.BritishDefaults()
        .WithLook("look", "l")
        .WithExamine("examine", "exam", "x", "inspect", "check")
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

    return new KeywordParser(config);
}

static void DisplayResult(CommandResult result)
{
    if (!string.IsNullOrWhiteSpace(result.Message))
    {
        Console.WriteLine(result.Message);
    }

    foreach (string reaction in result.ReactionsList)
    {
        if (!string.IsNullOrWhiteSpace(reaction))
        {
            Console.WriteLine($"> {reaction}");
        }
    }
}

void ShowRoom()
{
    ILocation location = state.CurrentLocation;
    Console.WriteLine();
    Console.WriteLine($"Room: {location.Id.ToProperCase()}");
    Console.WriteLine(location.GetDescription());

    string items = location.Items.CommaJoinNames(properCase: true);
    Console.WriteLine(string.IsNullOrWhiteSpace(items) ? "Items: None" : $"Items: {items}");

    List<string> exits = [.. location.Exits.Keys
        .Select(direction => direction.ToString().ToLowerInvariant().ToProperCase())];
    Console.WriteLine(exits.Count > 0 ? $"Exits: {exits.CommaJoin()}" : "Exits: None");
}

static void ShowHelp()
{
    Console.WriteLine("Commands: look, examine/check <item>, take <item>, use <item>, move <item>, unlock/open door, go <direction>, inventory, quit");
    Console.WriteLine("Save/Load: save [file], load [file] (defaults to savegame.json)");
}

static bool IsHelp(string input) => input.Lower() is "help" or "halp" or "?";

static bool ShouldShowRoom(ICommand command, CommandResult result)
{
    if (!result.Success || result.ShouldQuit)
    {
        return false;
    }

    return command switch
    {
        GoCommand => true,
        MoveCommand => true,
        LoadCommand => true,
        _ => false
    };
}
```
