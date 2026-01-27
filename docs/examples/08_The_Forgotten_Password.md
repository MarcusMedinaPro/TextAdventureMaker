# The Forgotten Password

_Slice tag: Slice 8 — Quest System. Demo focuses on quest log updates, quest conditions, and a tiny login flow._

## Story beats (max ~10 steps)
1) You are back at your office desk.
2) Move the chair to reveal a hidden note.
3) Read the note to learn the password.
4) Grab the server key from the break room.
5) Unlock the server door and reach the terminal.
6) Log in to finish the quest.

## Map (rough layout)
```
[Break Room]
     |
  [Office] --(locked)--> [Server Room]
```

## Example (quest log + conditions)
```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;

var office = new Location(
    "office",
    "A quiet office with a humming monitor and a stubborn desk chair. A secure door to the east leads to the server room.");

var breakRoom = new Location(
    "break room",
    "A cramped break room with a kettle that has long since given up.");

var serverRoom = new Location(
    "server room",
    "A low, cool room of blinking lights. The terminal waits for a password.");

var chair = new Item(
        "chair",
        "desk chair",
        "A wheeled chair with a slightly loose castor, loyal but squeaky.")
    .AddAliases("chair")
    .SetTakeable(false)
    .SetReaction(ItemAction.Move, "The chair rolls back with a polite squeak.");

var note = new Item(
        "note",
        "post-it note",
        "A yellow note with a hurried scrawl.")
    .AddAliases("note", "post-it", "sticky note", "post it")
    .SetWeight(0.01f)
    .SetReadText("PASSWORD: HAWTHORN")
    .RequireTakeToRead()
    .SetReaction(ItemAction.Read, "You commit the password to memory.");

var serverKey = new Key("server_key", "server key", "A small brass key tagged 'Server Room'.")
    .AddAliases("key", "brass key")
    .SetWeight(0.02f);

var terminal = new Item(
        "terminal",
        "terminal",
        "A squat terminal with a blinking cursor that feels faintly judgemental.")
    .AddAliases("computer", "console", "screen")
    .SetTakeable(false)
    .SetReaction(ItemAction.TakeFailed, "It is bolted to the desk and refuses to budge.");

var serverDoor = new Door("server_door", "server door", "A slim security door with a key reader.", DoorState.Locked)
    .AddAliases("door", "server", "security door")
    .SetReaction(DoorAction.Unlock, "The reader blinks green.")
    .SetReaction(DoorAction.Open, "The door slides open with a soft hiss.")
    .RequiresKey(serverKey);

office.AddItem(chair);

breakRoom.AddItem(serverKey);

serverRoom.AddItem(terminal);

office.AddExit(Direction.North, breakRoom);
breakRoom.AddExit(Direction.South, office);

office.AddExit(Direction.East, serverRoom, serverDoor);
serverRoom.AddExit(Direction.West, office, serverDoor);

var state = new GameState(office, worldLocations: new[] { office, breakRoom, serverRoom })
{
    EnableFuzzyMatching = true,
    FuzzyMaxDistance = 1,
    ShowItemsListOnlyWhenThereAreActuallyThingsToInteractWith = true,
    ShowDirectionsWhenThereAreDirectionsVisibleOnly = true
};

var questFindNote = new Quest("find_note", "Find the note", "Locate the password hint.")
    .AddCondition(new HasItemCondition("note"))
    .Start();

var questLogIn = new Quest("log_in", "Log in to the terminal", "Enter the password and access the system.")
    .AddCondition(new AllOfCondition(new IQuestCondition[]
    {
        new WorldFlagCondition("knows_password"),
        new WorldFlagCondition("logged_in")
    }))
    .Start();

state.Quests.AddRange(new[] { questFindNote, questLogIn });

var questStates = state.Quests.Quests
    .ToDictionary(quest => quest.Id, quest => quest.State, StringComparer.OrdinalIgnoreCase);

var noteRevealed = false;

chair.OnMove += _ =>
{
    if (noteRevealed)
    {
        chair.SetReaction(ItemAction.Move, "The chair is already out of the way.");
        return;
    }

    noteRevealed = true;
    office.AddItem(note);
    Console.WriteLine("> You spot a post-it note stuck beneath the desk.");
};

var parserConfig = KeywordParserConfigBuilder.BritishDefaults()
    .WithLook("look", "l")
    .WithExamine("examine", "exam", "x")
    .WithMove("move", "push", "shift", "slide")
    .WithInventory("inventory", "inv", "i")
    .WithTake("take", "get")
    .WithDrop("drop")
    .WithUse("use")
    .WithRead("read")
    .WithQuest("quest", "quests", "journal")
    .WithUnlock("unlock")
    .WithOpen("open")
    .WithGo("go", "move")
    .WithFuzzyMatching(true, 1)
    .WithIgnoreItemTokens("on", "off", "at", "the", "a")
    .Build();

var parser = new KeywordParser(parserConfig);

Console.WriteLine("=== THE FORGOTTEN PASSWORD (Slice 8) ===");
Console.WriteLine("Type 'help' if you want a brief reminder of commands.");
ShowLookResult(state.Look());

while (true)
{
    Console.Write("> ");
    var input = Console.ReadLine()?.Trim();
    if (string.IsNullOrWhiteSpace(input))
    {
        continue;
    }

    if (IsHelp(input))
    {
        Console.WriteLine("Commands: look, examine, move <item>, take <item>, read <item>, unlock/open door, go <direction>, inventory, quest, quit");
        continue;
    }

    if (TryHandleLogin(input, state))
    {
        UpdateQuestProgress();
        if (state.WorldState.GetFlag("logged_in"))
        {
            Console.WriteLine("You are in. The terminal hums to life.");
        }
        continue;
    }

    var command = parser.Parse(input);
    var result = state.Execute(command);

    if (command is LookCommand look && !string.IsNullOrWhiteSpace(look.Target))
    {
        WriteResult(result);
    }
    else if (command is LookCommand)
    {
        ShowLookResult(result);
    }
    else
    {
        WriteResult(result);
    }

    if (command is ReadCommand read && note.Matches(read.Target))
    {
        state.WorldState.SetFlag("knows_password", true);
    }

    UpdateQuestProgress();

    if (result.ShouldQuit)
    {
        break;
    }
}

void WriteResult(CommandResult result)
{
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
}

void ShowLookResult(CommandResult result)
{
    Console.WriteLine();
    Console.WriteLine(new string('-', 60));
    Console.WriteLine($"You are in the {state.CurrentLocation.Id.ToProperCase()}");
    Console.WriteLine();

    var lines = result.Message?
        .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
        .ToList()
        ?? new List<string>();

    if (lines.Count > 0 && lines[0].StartsWith("I think you mean", StringComparison.OrdinalIgnoreCase))
    {
        Console.WriteLine(lines[0]);
        Console.WriteLine();
        lines.RemoveAt(0);
    }

    var description = lines.FirstOrDefault() ?? state.CurrentLocation.GetDescription();
    if (!string.IsNullOrWhiteSpace(description))
    {
        Console.WriteLine(description);
        Console.WriteLine();
    }

    var itemsLine = lines.FirstOrDefault(line => line.StartsWith("Items here:", StringComparison.OrdinalIgnoreCase));
    if (!string.IsNullOrWhiteSpace(itemsLine))
    {
        var items = itemsLine.Replace("Items here:", "").Trim();
        Console.WriteLine(items.Length > 0 ? $"You notice {items}" : "You notice nothing of interest.");
        Console.WriteLine();
    }

    var exitsLine = lines.FirstOrDefault(line => line.StartsWith("Exits:", StringComparison.OrdinalIgnoreCase));
    if (!string.IsNullOrWhiteSpace(exitsLine))
    {
        Console.WriteLine(exitsLine);
        Console.WriteLine();
    }

    Console.WriteLine("Hints");
    Console.WriteLine("- move chair");
    Console.WriteLine("- read note");
    Console.WriteLine("- quest");
    Console.WriteLine("- log in");
    Console.WriteLine(new string('-', 60));
}

void UpdateQuestProgress()
{
    var updated = state.Quests.CheckAll(state);
    if (!updated)
    {
        return;
    }

    foreach (var quest in state.Quests.Quests)
    {
        if (!questStates.TryGetValue(quest.Id, out var previous) || previous != quest.State)
        {
            questStates[quest.Id] = quest.State;
            if (quest.State == QuestState.Completed)
            {
                Console.WriteLine($"Quest complete: {quest.Title}");
                if (quest.Id.Equals("log_in", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("You have done it. The terminal yields, and the system is yours.");
                    Environment.Exit(0);
                }
            }
        }
    }
}

bool IsHelp(string input)
{
    return input.Equals("help", StringComparison.OrdinalIgnoreCase) ||
           input.Equals("halp", StringComparison.OrdinalIgnoreCase) ||
           input == "?";
}

bool TryHandleLogin(string input, GameState gameState)
{
    if (!IsLoginInput(input))
    {
        return false;
    }

    if (!gameState.IsCurrentRoomId(serverRoom.Id))
    {
        Console.WriteLine("The terminal is not within reach from here.");
        return true;
    }

    if (gameState.WorldState.GetFlag("logged_in"))
    {
        Console.WriteLine("You are already logged in.");
        return true;
    }

    if (!gameState.WorldState.GetFlag("knows_password"))
    {
        Console.WriteLine("The terminal prompts for a password you do not yet remember.");
        return true;
    }

    gameState.WorldState.SetFlag("logged_in", true);
    return true;
}

bool IsLoginInput(string input)
{
    return input.Equals("login", StringComparison.OrdinalIgnoreCase) ||
           input.Equals("log in", StringComparison.OrdinalIgnoreCase) ||
           input.Equals("log-in", StringComparison.OrdinalIgnoreCase) ||
           input.Equals("use terminal", StringComparison.OrdinalIgnoreCase) ||
           input.Equals("use computer", StringComparison.OrdinalIgnoreCase) ||
           input.Equals("enter password", StringComparison.OrdinalIgnoreCase) ||
           input.Equals("type password", StringComparison.OrdinalIgnoreCase);
}
```
