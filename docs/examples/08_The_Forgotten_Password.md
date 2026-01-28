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

// Slice 8 — Quest System
// New functions tested:
// - Quest, QuestLog, QuestState
// - Quest conditions + visitor (HasItemCondition, AllOfCondition, WorldFlagCondition)
// - KeywordParserConfigBuilder + KeywordParser
// - ReadCommand, QuestCommand
// - Item reactions (Move/Read) + WorldState flags
// - Door + Key integration with parser commands

var office = new Location(
    "office",
    "A quiet office with a humming monitor and a desk chair of dubious sturdiness. A secure door to the east leads to the server room.");

var breakRoom = new Location(
    "break room",
    "A cramped break room with a kettle that has long since surrendered its dignity.");

var serverRoom = new Location(
    "server room",
    "A low, cool room of blinking lights. The terminal waits with theatrical patience for a password.");

var chair = new Item(
        "chair",
        "desk chair",
        "A wheeled chair with a slightly loose castor, loyal in sentiment if not in stability.")
    .AddAliases("chair")
    .SetTakeable(false)
    .SetReaction(ItemAction.Move, "The chair rolls back with a dignified squeak, as if it has always intended to help.")
    .SetReaction(ItemAction.Use, "You sit. The chair creaks in a manner best described as concerned. You get up again.")
    .SetReaction(ItemAction.TakeFailed, "You attempt to hoist it. The chair refuses to dignify the effort.");

var monitor = new Item(
        "monitor",
        "monitor",
        "A perfectly ordinary monitor, humming faintly as though it has opinions.")
    .AddAliases("screen", "display")
    .SetTakeable(false)
    .SetReaction(ItemAction.TakeFailed, "You make a show of lifting it. The desk makes a show of keeping it.")
    .SetReaction(ItemAction.MoveFailed, "You nudge the monitor. It does not budge, as if bolted by principle.")
    .SetReaction(ItemAction.Use, "You tap the screen. It remains politely noncommittal.");

var note = new Item(
        "note",
        "post-it note",
        "A yellow note with a hurried scrawl, doing its best to look inconspicuous.")
    .AddAliases("note", "post-it", "sticky note", "post it")
    .SetWeight(0.01f)
    .SetReadText("PASSWORD: HAWTHORN")
    .RequireTakeToRead()
    .SetReaction(ItemAction.Take, "You take the note; it offers no resistance whatsoever.")
    .SetReaction(ItemAction.Read, "You commit the password to memory, like a dutiful clerk.")
    .SetReaction(ItemAction.Move, "You nudge the note and it flutters back into place, pretending it was never moved.")
    .SetReaction(ItemAction.Use, "You consider employing the note in some grand scheme, then think better of it.");

var serverKey = new Key("server_key", "server key", "A small brass key tagged 'Server Room'.")
    .AddAliases("key", "brass key")
    .SetWeight(0.02f)
    .SetReaction(ItemAction.Take, "You pocket the key. It feels oddly heavier than its size should allow.")
    .SetReaction(ItemAction.Move, "The key skitters across the counter, gleaming with misplaced importance.")
    .SetReaction(ItemAction.Use, "You roll the key between your fingers. It remains a key.")
    .SetReaction(ItemAction.Drop, "The key lands with a prim little clink.");

var terminal = new Item(
        "terminal",
        "terminal",
        "A squat terminal with a blinking cursor that feels faintly judgemental.")
    .AddAliases("computer", "console", "screen")
    .SetTakeable(false)
    .SetReaction(ItemAction.TakeFailed, "It is bolted to the desk and refuses to budge.")
    .SetReaction(ItemAction.MoveFailed, "You push the terminal. It does not move. It judges you for trying.");

var kettle = new Item(
        "kettle",
        "kettle",
        "A kettle of noble lineage, now retired and faintly insulted.")
    .AddAliases("tea kettle", "old kettle")
    .SetTakeable(false)
    .SetReaction(ItemAction.MoveFailed, "You nudge the kettle. It rattles with theatrical displeasure.")
    .SetReaction(ItemAction.Use, "You flick the switch. The kettle answers with a weary sigh and no heat.")
    .SetReaction(ItemAction.TakeFailed, "You lift it. It is far too attached to the counter to comply.");
var serverDoor = new Door("server_door", "server door", "A slim security door with a key reader.", DoorState.Locked)
    .AddAliases("door", "server", "security door")
    .SetReaction(DoorAction.Unlock, "The reader blinks green.")
    .SetReaction(DoorAction.Open, "The door slides open with a soft hiss.")
    .SetReaction(DoorAction.OpenFailed, "The door gives a very polite refusal.")
    .SetReaction(DoorAction.UnlockFailed, "The reader remains unimpressed.")
    .RequiresKey(serverKey);

office.AddItem(chair);
office.AddItem(monitor);

breakRoom.AddItem(serverKey);
breakRoom.AddItem(kettle);

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
    Console.WriteLine("> You spot a post-it note tucked beneath the desk, as if embarrassed to be noticed.");
};

var parserConfig = KeywordParserConfigBuilder.BritishDefaults()
    .WithLook("look", "l","ls")
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

    if (TryHandleSit(input, state, chair))
    {
        continue;
    }

    if (TryHandleLogin(input, state))
    {
        UpdateQuestProgress();
        if (state.WorldState.GetFlag("logged_in"))
        {
            Console.WriteLine("You are in. The terminal hums to life, grudgingly impressed.");
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
                    Console.WriteLine("You have done it. The terminal yields, and the system is—if not yours—at least temporarily compliant.");
                    Environment.Exit(0);
                }
            }
        }
    }
}

bool IsHelp(string input)
{
    var normalized = input.Lower();
    return normalized is "help" or "halp" or "?";
}

bool TryHandleLogin(string input, GameState gameState)
{
    if (!IsLoginInput(input))
    {
        return false;
    }

    if (!gameState.IsCurrentRoomId(serverRoom.Id))
    {
        Console.WriteLine("The terminal is not within reach from here, no matter how you squint.");
        return true;
    }

    if (gameState.WorldState.GetFlag("logged_in"))
    {
        Console.WriteLine("You are already logged in, and the terminal remembers it.");
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
    var normalized = input.Lower();
    return normalized is "login" or "log in" or "log-in" or "use terminal" or "use computer" or "enter password" or "type password";
}

bool TryHandleSit(string input, GameState gameState, IItem chairItem)
{
    if (!IsSitInput(input)) return false;

    if (!gameState.IsCurrentRoomId(office.Id))
    {
        Console.WriteLine("There is nowhere suitable to sit here.");
        return true;
    }

    if (!gameState.CurrentLocation.Items.Contains(chairItem))
    {
        Console.WriteLine("The chair seems to have wandered off.");
        return true;
    }

    var reaction = chairItem.GetReaction(ItemAction.Use);
    Console.WriteLine(reaction ?? "You sit for a moment, collecting yourself.");
    return true;
}

bool IsSitInput(string input)
{
    var normalized = input.Lower();
    return normalized is "sit" or "sit down" or "sit on chair" or "sit on the chair" or "sit in chair" or "sit in the chair";
}
```
