# The Late Platform

_Slice tag: Slice 5 — NPCs + Dialog + Movement. Demo focuses on an NPC with dialogue, patrol timing, and room-specific hints._

A quiet station at night. A guard paces between the concourse and platform.

## Map (rough layout)
```
          N
    W           E
          S

┌────────────┐     ┌────────────┐
│ Concourse  │─────│  Platform  │
│   Guard    │     │            │
└────────────┘     └────────────┘
```

## Story beats (max ~10 steps)
1) You arrive in the concourse.
2) The guard offers directions.
3) Find the ticket.
4) Unlock the gate and head to the platform.
5) The guard patrols between rooms.

## Slice 1 + 2 + 3 + 4 + 5 functions tested
- `Location(id, description)`
- `Location.AddExit(direction, target, oneWay: false)`
- `Location.AddExit(direction, target, door, oneWay: false)`
- `Location.AddNpc(npc)`
- `Npc(id, name, state)`
- `Npc.Description(text)`
- `Npc.SetDialog(dialogNode)`
- `Npc.SetMovement(movement)`
- `DialogNode(text).AddOption(text, next)`
- `PatrolNpcMovement(route)`
- `Game(state, parser)` + manual loop
- `Door(id, name, description, initialState)`
- `Door.RequiresKey(key)`
- `Door.SetReaction(action, text)`
- `Key(id, name, description)`
- `ICommandParser` + `KeywordParser(config)`
- `KeywordParserConfigBuilder.BritishDefaults().Build()`
- `CommandExtensions.Execute(state, command)`
- `Direction` enum

## Demo commands (parser)
- `look` / `l`
- `talk guard`
- `take ticket`
- `unlock gate`
- `open gate`
- `go east` / `e`
- `inventory` / `i`
- `quit` / `exit`

## Example (NPCs + dialog + movement)
```csharp
using System;
using System.Linq;
using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;

var concourse = new Location("concourse", "A draughty concourse with a flickering board.");
var platform = new Location("platform", "The platform is quiet. The last train waits.");

var ticket = new Key("ticket", "ticket", "A paper ticket stamped with today's date.")
    .SetWeight(0.01f)
    .AddAliases("pass", "paper");

var board = new Item("board", "board", "A flickering departures board. The last train is waiting.")
    .AddAliases("board", "departures", "schedule")
    .SetTakeable(false);

var gate = new Door("gate", "ticket gate", "A narrow gate with a tired turnstile.")
    .RequiresKey(ticket)
    .SetReaction(DoorAction.Unlock, "The gate clicks open.")
    .SetReaction(DoorAction.Open, "You push through the gate.")
    .SetReaction(DoorAction.OpenFailed, "The gate refuses to budge.");

concourse.AddItem(ticket);
concourse.AddItem(board);
concourse.AddExit(Direction.East, platform, gate);

var guard = new Npc("guard", "guard")
    .Description("A station guard in a dark coat watches the concourse.")
    .SetMovement(new NoNpcMovement())
    .SetDialog(
        new DialogNode("Evening. Need a ticket or directions?")
            .AddOption("Ticket.", new DialogNode("Check the bench by the board."))
            .AddOption("Platform?", new DialogNode("Through the gate to the east.")));

concourse.AddNpc(guard);

var state = new GameState(concourse, worldLocations: new[] { concourse, platform });
var parser = new KeywordParser(KeywordParserConfigBuilder.BritishDefaults().Build());

Console.WriteLine("=== THE LATE PLATFORM (Slice 5) ===");
Console.WriteLine("Commands: Look, Talk <Npc>, Take <Item>, Unlock/Open Gate, Go East/West, Inventory, Quit");

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
    Console.WriteLine($"Room: {state.CurrentLocation.Id.ToProperCase()}");
    var lines = result.Message?
        .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
        .Where(line => !line.StartsWith("Health:", StringComparison.OrdinalIgnoreCase))
        .ToList()
        ?? new List<string>();

    if (lines.Count > 0)
    {
        Console.WriteLine(lines[0]);
    }

    if (state.CurrentLocation.FindNpc("guard") != null)
    {
        var guardLines = new[]
        {
            "The guard is here, he watches the concourse.",
            "The guard is here, he watches his watch.",
            "The guard is here, he takes a sip of his pocket flask.",
            "The guard is here, he scratches his head and looks at the concourse."
        };

        Console.WriteLine(guardLines[Random.Shared.Next(guardLines.Length)]);
    }

    foreach (var line in lines.Skip(1))
    {
        Console.WriteLine(line);
    }

    if (state.CurrentLocation.Id.TextCompare("concourse"))
    {
        Console.WriteLine("Try: talk guard, take ticket, unlock gate, open gate, go east.");
    }
    else if (state.CurrentLocation.Id.TextCompare("platform"))
    {
        Console.WriteLine("Try: look board, go west, board, sit.");
    }
}

var lastNpcLocations = state.Locations
    .SelectMany(location => location.Npcs.Select(npc => (npc, location)))
    .ToDictionary(entry => entry.npc.Id, entry => entry.location);

var guardStepsUntilMove = Random.Shared.Next(3, 5);
var isInDialog = false;

ILocation? FindGuardLocation()
{
    return state.Locations.FirstOrDefault(location => location.Npcs.Contains(guard));
}

void TickNpcMovement(ICommand command)
{
    if (isInDialog) return;

    if (command is TalkCommand talk && talk.Target != null && talk.Target.TextCompare("guard"))
    {
        guardStepsUntilMove = Random.Shared.Next(3, 5);
    }
    else
    {
        guardStepsUntilMove--;
    }

    if (guardStepsUntilMove <= 0)
    {
        var current = FindGuardLocation();
        if (current != null)
        {
            var target = ReferenceEquals(current, concourse) ? platform : concourse;
            current.RemoveNpc(guard);
            target.AddNpc(guard);
        }

        guardStepsUntilMove = Random.Shared.Next(3, 5);
    }

    foreach (var location in state.Locations)
    {
        foreach (var npc in location.Npcs)
        {
            lastNpcLocations.TryGetValue(npc.Id, out var previous);
            if (previous != null && !ReferenceEquals(previous, location))
            {
                if (ReferenceEquals(location, state.CurrentLocation))
                {
                    Console.WriteLine($"{npc.Name.ToProperCase()} arrives.");
                }
                else if (ReferenceEquals(previous, state.CurrentLocation))
                {
                    Console.WriteLine($"{npc.Name.ToProperCase()} heads out.");
                }
            }

            lastNpcLocations[npc.Id] = location;
        }
    }
}

void RunDialog(INpc npc)
{
    var root = npc.DialogRoot;
    if (root == null)
    {
        Console.WriteLine("They have nothing to say.");
        return;
    }

    isInDialog = true;
    guardStepsUntilMove = Random.Shared.Next(3, 5);

    while (true)
    {
        Console.WriteLine(root.Text);
        for (var i = 0; i < root.Options.Count; i++)
        {
            Console.WriteLine($"{i + 1}) {root.Options[i].Text}");
        }

        Console.WriteLine($"{root.Options.Count + 1}) OK thanks, bye.");
        Console.Write("> ");
        var input = Console.ReadLine()?.Trim();
        if (!int.TryParse(input, out var choice))
        {
            Console.WriteLine("Pick a number.");
            continue;
        }

        if (choice == root.Options.Count + 1)
        {
            Console.WriteLine("Bye, have a nice travel.");
            break;
        }

        if (choice < 1 || choice > root.Options.Count)
        {
            Console.WriteLine("Pick a valid option.");
            continue;
        }

        var selected = root.Options[choice - 1].Text;
        if (selected.Contains("ticket", StringComparison.OrdinalIgnoreCase))
        {
            var hasTicket = state.Inventory.FindItem("ticket") != null;
            Console.WriteLine(hasTicket
                ? "Use it on the gate."
                : "Check the bench by the board.");
        }
        else if (selected.Contains("platform", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("Through the gate to the east.");
        }
    }

    isInDialog = false;
}

ShowLookResult(state.Look());

while (true)
{
    Console.Write("> ");
    var input = Console.ReadLine()?.Trim();
    if (string.IsNullOrWhiteSpace(input)) continue;

    if (input.StartsWith("use ", StringComparison.OrdinalIgnoreCase) &&
        input.Contains("ticket", StringComparison.OrdinalIgnoreCase) &&
        (input.Contains("gate", StringComparison.OrdinalIgnoreCase) || input.Contains("east", StringComparison.OrdinalIgnoreCase)))
    {
        var door = state.CurrentLocation.Exits.Values.FirstOrDefault(exit => exit.Door != null)?.Door;
        if (door == null)
        {
            Console.WriteLine("There is no gate here.");
            continue;
        }

        if (door.State == DoorState.Locked)
        {
            var unlockResult = state.Execute(new UnlockCommand());
            WriteResult(unlockResult);
            if (!unlockResult.Success || unlockResult.ShouldQuit)
            {
                if (!unlockResult.ShouldQuit)
                {
                    TickNpcMovement(new UnlockCommand());
                }
                continue;
            }
        }

        if (door.State == DoorState.Closed)
        {
            var openResult = state.Execute(new OpenCommand());
            WriteResult(openResult);
        }

        TickNpcMovement(new UnlockCommand());
        continue;
    }

    var command = parser.Parse(input);
    if (command is TalkCommand talk && talk.Target != null && talk.Target.TextCompare("guard"))
    {
        RunDialog(guard);
        continue;
    }

    var result = state.Execute(command);

    if (command is LookCommand)
    {
        ShowLookResult(result);
    }
    else
    {
        WriteResult(result);
    }

    if (result.ShouldQuit) break;

    TickNpcMovement(command);
}
```
