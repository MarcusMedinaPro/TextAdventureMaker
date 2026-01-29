# The Forgotten Password

_Slice tag: Slice 8 — Quest System. Demo focuses on quest log updates, quest conditions, and a tiny login flow._

A quiet office, a stubborn door, and a password that clearly expected to be forgotten.

## Goal

Reveal the note, learn the password, fetch the server key, and log in at the terminal.

## Map (rough layout)

```
[Break Room]
     |
  [Office] --(locked)--> [Server Room]
```

## Story beats (max ~10 steps)

1. You begin at your office desk.
2. Move the chair to reveal a hidden note.
3. Read the note to learn the password.
4. Collect the server key from the break room.
5. Unlock the server door and reach the terminal.
6. Log in to complete the quest.

## Slice 1‑8 functions tested

- `Quest`, `QuestLog`, `QuestState`
- `HasItemCondition`, `AllOfCondition`, `WorldFlagCondition`
- `QuestLog.CheckAll(state)`
- `Location.AddExit(direction, target, door)`
- `Door(id, name, description, state)` + `Door.RequiresKey(key)`
- `Item.SetReaction(action, text)`
- `Item.SetReadText(...)`, `Item.RequireTakeToRead()`
- `ReadCommand`, `QuestCommand`
- `KeywordParser`, `KeywordParserConfigBuilder`
- `CommandExtensions.Execute(state, command)`

## Demo commands (parser)

- `look` / `l`
- `examine <item>` / `x <item>`
- `move chair`
- `take note` / `get note`
- `read note`
- `unlock door` / `open door`
- `quest` / `journal`
- `inventory` / `i`
- `go north/east/west/south`
- `log in` / `login` / `use terminal`
- `quit` / `exit`

## Example (quest log + conditions)
```csharp
using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;

// Slice 8: The Forgotten Password
// Tests: quest log, quest conditions, read command, chair reveal, auto-look after go.

var world = BuildWorld();
ConfigureQuests(world);
ConfigureNoteReveal(world);

Console.WriteLine("=== THE FORGOTTEN PASSWORD (Slice 8) ===");
Console.WriteLine("Goal: reveal the note, learn the password, fetch the server key, and log in.");
Console.WriteLine("Commands: look, examine, move chair, take/read note, unlock/open door, go north/east, quest, inventory, log in, quit.");
ShowRoom(world.State);

while (true)
{
    Console.Write("
> ");
    var input = Console.ReadLine()?.Trim();
    if (string.IsNullOrWhiteSpace(input))
        continue;

    if (IsHelp(input))
    {
        Console.WriteLine("Commands: look, examine, move chair, take/read note, unlock/open door, go <direction>, quest, inventory, log in, quit");
        continue;
    }

    if (TryHandleSit(input, world))
    {
        continue;
    }

    if (TryHandleLogin(input, world))
    {
        UpdateQuestProgress(world);
        if (world.State.WorldState.GetFlag("logged_in"))
        {
            Console.WriteLine("You are in. The terminal hums to life, grudgingly impressed.");
        }

        continue;
    }

    var command = world.Parser.Parse(input);
    var result = world.State.Execute(command);

    switch (command)
    {
        case LookCommand { Target: not null }:
            WriteResult(result);
            break;
        case LookCommand:
            ShowLookResult(world.State, result);
            break;
        default:
            WriteResult(result);
            break;
    }

    if (command is GoCommand && result.Success && !result.ShouldQuit)
    {
        ShowRoom(world.State);
    }

    if (command is ReadCommand { Target: var target } && world.Note.Matches(target))
    {
        world.State.WorldState.SetFlag("knows_password", true);
    }

    UpdateQuestProgress(world);

    if (result.ShouldQuit)
    {
        break;
    }
}

static DemoWorld BuildWorld()
{
    var office = new Location(
        "office",
        "A quiet office with a humming monitor and a desk chair of dubious sturdiness. A secure door to the east leads to the server room.");

    var breakRoom = new Location(
        "break room",
        "A cramped break room with a kettle that has long since surrendered its dignity.");

    var serverRoom = new Location(
        "server room",
        "A low, cool room of blinking lights. The terminal waits with theatrical patience for a password.");

    var chair = new Item("chair", "desk chair", "A wheeled chair with a slightly loose castor, loyal in sentiment if not in stability.");
    chair.AddAliases("chair");
    chair.SetTakeable(false);
    chair.SetReaction(ItemAction.Move, "The chair rolls back with a dignified squeak, as if it has always intended to help.");
    chair.SetReaction(ItemAction.Use, "You sit. The chair creaks in a manner best described as concerned. You get up again.");
    chair.SetReaction(ItemAction.TakeFailed, "You attempt to hoist it. The chair refuses to dignify the effort.");

    var monitor = new Item("monitor", "monitor", "A perfectly ordinary monitor, humming faintly as though it has opinions.");
    monitor.AddAliases("screen", "display");
    monitor.SetTakeable(false);
    monitor.SetReaction(ItemAction.TakeFailed, "You make a show of lifting it. The desk makes a show of keeping it.");
    monitor.SetReaction(ItemAction.MoveFailed, "You nudge the monitor. It does not budge, as if bolted by principle.");
    monitor.SetReaction(ItemAction.Use, "You tap the screen. It remains politely noncommittal.");

    var note = new Item("note", "post-it note", "A yellow note with a hurried scrawl, doing its best to look inconspicuous.");
    note.AddAliases("note", "post-it", "sticky note", "post it");
    note.SetWeight(0.01f);
    note.SetReadText("PASSWORD: HAWTHORN");
    note.RequireTakeToRead();
    note.SetReaction(ItemAction.Take, "You take the note; it offers no resistance whatsoever.");
    note.SetReaction(ItemAction.Read, "You commit the password to memory, like a dutiful clerk.");
    note.SetReaction(ItemAction.Move, "You nudge the note and it flutters back into place, pretending it was never moved.");
    note.SetReaction(ItemAction.Use, "You consider employing the note in some grand scheme, then think better of it.");

    var serverKey = new Key("server_key", "server key", "A small brass key tagged 'Server Room'.");
    serverKey.AddAliases("key", "brass key");
    serverKey.SetWeight(0.02f);
    serverKey.SetReaction(ItemAction.Take, "You pocket the key. It feels oddly heavier than its size should allow.");
    serverKey.SetReaction(ItemAction.Move, "The key skitters across the counter, gleaming with misplaced importance.");
    serverKey.SetReaction(ItemAction.Use, "You roll the key between your fingers. It remains a key.");
    serverKey.SetReaction(ItemAction.Drop, "The key lands with a prim little clink.");

    var terminal = new Item("terminal", "terminal", "A squat terminal with a blinking cursor that feels faintly judgemental.");
    terminal.AddAliases("computer", "console", "screen");
    terminal.SetTakeable(false);
    terminal.SetReaction(ItemAction.TakeFailed, "It is bolted to the desk and refuses to budge.");
    terminal.SetReaction(ItemAction.MoveFailed, "You push the terminal. It does not move. It judges you for trying.");

    var kettle = new Item("kettle", "kettle", "A kettle of noble lineage, now retired and faintly insulted.");
    kettle.AddAliases("tea kettle", "old kettle");
    kettle.SetTakeable(false);
    kettle.SetReaction(ItemAction.MoveFailed, "You nudge the kettle. It rattles with theatrical displeasure.");
    kettle.SetReaction(ItemAction.Use, "You flick the switch. The kettle answers with a weary sigh and no heat.");
    kettle.SetReaction(ItemAction.TakeFailed, "You lift it. It is far too attached to the counter to comply.");

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

    var state = new GameState(office, worldLocations: [office, breakRoom, serverRoom])
    {
        EnableFuzzyMatching = true,
        FuzzyMaxDistance = 1,
        ShowItemsListOnlyWhenThereAreActuallyThingsToInteractWith = true,
        ShowDirectionsWhenThereAreDirectionsVisibleOnly = true
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

    return new DemoWorld(
        office,
        breakRoom,
        serverRoom,
        chair,
        note,
        serverKey,
        terminal,
        serverDoor,
        state,
        parser);
}

static void ConfigureQuests(DemoWorld world)
{
    var questFindNote = new Quest("find_note", "Find the note", "Locate the password hint.")
        .AddCondition(new HasItemCondition("note"))
        .Start();

    var questLogIn = new Quest("log_in", "Log in to the terminal", "Enter the password and access the system.")
        .AddCondition(new AllOfCondition(
        [
            new WorldFlagCondition("knows_password"),
            new WorldFlagCondition("logged_in")
        ]))
        .Start();

    world.State.Quests.AddRange([questFindNote, questLogIn]);

    world.QuestStates = world.State.Quests.Quests
        .ToDictionary(quest => quest.Id, quest => quest.State, StringComparer.OrdinalIgnoreCase);
}

static void ConfigureNoteReveal(DemoWorld world)
{
    world.Chair.OnMove += _ =>
    {
        if (world.NoteRevealed)
        {
            _ = world.Chair.SetReaction(ItemAction.Move, "The chair is already out of the way.");
            return;
        }

        world.NoteRevealed = true;
        world.Office.AddItem(world.Note);
        Console.WriteLine("> You spot a post-it note tucked beneath the desk, as if embarrassed to be noticed.");
    };
}

static void WriteResult(CommandResult result)
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

static void ShowRoom(GameState state)
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

static void ShowLookResult(GameState state, CommandResult result)
{
    Console.WriteLine($"Room: {state.CurrentLocation.Id.ToProperCase()}");
    WriteResult(result);
}

static void UpdateQuestProgress(DemoWorld world)
{
    var updated = world.State.Quests.CheckAll(world.State);
    if (!updated)
    {
        return;
    }

    foreach (var quest in world.State.Quests.Quests)
    {
        if (!world.QuestStates.TryGetValue(quest.Id, out var previous) || previous != quest.State)
        {
            world.QuestStates[quest.Id] = quest.State;
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

static bool IsHelp(string input)
{
    var normalized = input.Lower();
    return normalized is "help" or "halp" or "?";
}

static bool TryHandleLogin(string input, DemoWorld world)
{
    if (!IsLoginInput(input))
    {
        return false;
    }

    if (!world.State.IsCurrentRoomId(world.ServerRoom.Id))
    {
        Console.WriteLine("The terminal is not within reach from here, no matter how you squint.");
        return true;
    }

    if (world.State.WorldState.GetFlag("logged_in"))
    {
        Console.WriteLine("You are already logged in, and the terminal remembers it.");
        return true;
    }

    if (!world.State.WorldState.GetFlag("knows_password"))
    {
        Console.WriteLine("The terminal prompts for a password you do not yet remember.");
        return true;
    }

    world.State.WorldState.SetFlag("logged_in", true);
    return true;
}

static bool IsLoginInput(string input)
{
    var normalized = input.Lower();
    return normalized is "login" or "log in" or "log-in" or "use terminal" or "use computer" or "enter password" or "type password";
}

static bool TryHandleSit(string input, DemoWorld world)
{
    var normalized = input.Lower();
    var wantsSit = normalized is "sit" or "sit down" or "sit on chair" or "sit on the chair" or "sit in chair" or "sit in the chair";
    if (!wantsSit)
        return false;

    if (!world.State.IsCurrentRoomId(world.Office.Id))
    {
        Console.WriteLine("There is nowhere suitable to sit here.");
        return true;
    }

    Console.WriteLine("You sit. The chair creaks in a manner best described as concerned. You get up again.");
    return true;
}

sealed class DemoWorld(
    Location office,
    Location breakRoom,
    Location serverRoom,
    Item chair,
    Item note,
    Key serverKey,
    Item terminal,
    Door serverDoor,
    GameState state,
    KeywordParser parser)
{
    public Location Office { get; } = office;
    public Location BreakRoom { get; } = breakRoom;
    public Location ServerRoom { get; } = serverRoom;
    public Item Chair { get; } = chair;
    public Item Note { get; } = note;
    public Key ServerKey { get; } = serverKey;
    public Item Terminal { get; } = terminal;
    public Door ServerDoor { get; } = serverDoor;
    public GameState State { get; } = state;
    public KeywordParser Parser { get; } = parser;

    public bool NoteRevealed { get; set; }
    public Dictionary<string, QuestState> QuestStates { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}
```
