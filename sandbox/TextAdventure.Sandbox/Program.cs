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

// Slice 8: The Forgotten Password
// Tests: quest log, quest conditions, read command, chair reveal, auto-look after go.

var office = new Location(
    "office",
    "A quiet office with a humming monitor and a desk chair of dubious sturdiness. A secure door to the east leads to the server room.");

var breakRoom = new Location(
    "break room",
    "A cramped break room with a kettle that has long since surrendered its dignity.");

var serverRoom = new Location(
    "server room",
    "A low, cool room of blinking lights. The terminal waits with theatrical patience for a password.");

var chair = new Item("chair", "desk chair", "A wheeled chair with a slightly loose castor, loyal in sentiment if not in stability.")
    .AddAliases("chair")
    .SetTakeable(false)
    .SetReaction(ItemAction.Move, "The chair rolls back with a dignified squeak, as if it has always intended to help.")
    .SetReaction(ItemAction.Use, "You sit. The chair creaks in a manner best described as concerned. You get up again.")
    .SetReaction(ItemAction.TakeFailed, "You attempt to hoist it. The chair refuses to dignify the effort.");

var monitor = new Item("monitor", "monitor", "A perfectly ordinary monitor, humming faintly as though it has opinions.")
    .AddAliases("screen", "display")
    .SetTakeable(false)
    .SetReaction(ItemAction.TakeFailed, "You make a show of lifting it. The desk makes a show of keeping it.")
    .SetReaction(ItemAction.MoveFailed, "You nudge the monitor. It does not budge, as if bolted by principle.")
    .SetReaction(ItemAction.Use, "You tap the screen. It remains politely noncommittal.");

var note = new Item("note", "post-it note", "A yellow note with a hurried scrawl, doing its best to look inconspicuous.")
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

var terminal = new Item("terminal", "terminal", "A squat terminal with a blinking cursor that feels faintly judgemental.")
    .AddAliases("computer", "console", "screen")
    .SetTakeable(false)
    .SetReaction(ItemAction.TakeFailed, "It is bolted to the desk and refuses to budge.")
    .SetReaction(ItemAction.MoveFailed, "You push the terminal. It does not move. It judges you for trying.");

var kettle = new Item("kettle", "kettle", "A kettle of noble lineage, now retired and faintly insulted.")
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

var parser = new KeywordParser(KeywordParserConfigBuilder.BritishDefaults()
    .WithLook("look", "l", "ls")
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
    .Build());

Console.WriteLine("=== THE FORGOTTEN PASSWORD (Slice 8) ===");
Console.WriteLine("Goal: reveal the note, learn the password, fetch the server key, and log in.");
Console.WriteLine("Commands: look, examine, move chair, take/read note, unlock/open door, go north/east, quest, inventory, log in, quit.");
ShowRoom();

while (true)
{
    Console.Write("\n> ");
    var input = Console.ReadLine()?.Trim();
    if (string.IsNullOrWhiteSpace(input)) continue;

    if (IsHelp(input))
    {
        Console.WriteLine("Commands: look, examine, move chair, take/read note, unlock/open door, go <direction>, quest, inventory, log in, quit");
        continue;
    }

    if (TryHandleSit(input, state))
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

    switch (command)
    {
        case LookCommand { Target: not null }:
            WriteResult(result);
            break;
        case LookCommand:
            ShowLookResult(result);
            break;
        default:
            WriteResult(result);
            break;
    }

    if (command is GoCommand && result.Success && !result.ShouldQuit)
    {
        ShowRoom();
    }

    if (command is ReadCommand { Target: var target } && note.Matches(target))
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

void ShowRoom()
{
    var location = state.CurrentLocation;
    Console.WriteLine();
    Console.WriteLine($"Room: {location.Id.ToProperCase()}");
    Console.WriteLine(location.GetDescription());

    var items = location.Items.CommaJoinNames(properCase: true);
    Console.WriteLine(string.IsNullOrWhiteSpace(items) ? "Items here: None" : $"Items here: {items}");

    var exits = location.Exits
        .Select(exit => exit.Key.ToString().ToLowerInvariant().ToProperCase())
        .ToList();
    Console.WriteLine(exits.Count > 0 ? $"Exits: {exits.CommaJoin()}" : "Exits: None");
}

void ShowLookResult(CommandResult result)
{
    Console.WriteLine($"Room: {state.CurrentLocation.Id.ToProperCase()}");
    WriteResult(result);
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
                if (quest.Id.Is("log_in"))
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

bool TryHandleSit(string input, GameState gameState)
{
    var normalized = input.Lower();
    var wantsSit = normalized is "sit" or "sit down" or "sit on chair" or "sit on the chair" or "sit in chair" or "sit in the chair";
    if (!wantsSit) return false;

    if (!gameState.IsCurrentRoomId(office.Id))
    {
        Console.WriteLine("There is nowhere suitable to sit here.");
        return true;
    }

    Console.WriteLine("You sit. The chair creaks in a manner best described as concerned. You get up again.");
    return true;
}
