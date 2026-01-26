# The Locked Drawer

_Slice tag: Slice 2 — Doors + Keys. Demo focuses on locked doors and keys blocking progress._

A tiny, focused demo about a locked study door, a brass key, and a secret inside.

## Map (rough layout)
```
          N
    W           E
          S

┌────────────┐     ┌────────────┐
│            │     │            │
│  Hallway   │─────│   Study    │
│            │     │            │
│     K      │     │      P     │
└────────────┘     └────────────┘

K = Brass key
P = Photo
```

## Story beats (max ~10 steps)
1) You start in the hallway.
2) You spot a key by the window.
3) The study door is locked.
4) Unlock the study door with the key.
5) Open the door and step inside.
6) Find the photo on the desk.

## Slice 1 + 2 functions tested
- `Location(id, description)`
- `Location.AddExit(direction, target, oneWay: false)`
- `Location.AddExit(direction, target, door, oneWay: false)`
- `Location.Exits`
- `Location.GetExit(direction)`
- `GameState(startLocation, worldLocations)`
- `GameState.Move(direction)`
- `GameState.CurrentLocation`
- `GameState.Inventory`
- `string.ToProperCase()`
- `EnumerableExtensions.CommaJoin()`
- `Door(id, name, description, initialState)`
- `Door.RequiresKey(key)`
- `Door.Unlock(key)`
- `Door.Open()`
- `Door.Close()`
- `Door.Lock(key)`
- `Door.Destroy()`
- `Door.State`
- `Door.SetReaction(action, text)`
- `Door.GetReaction(action)`
- `DoorAction` enum
- `DoorState` enum
- `Key(id, name, description)`
- `ItemAction` enum
- `Direction` enum

## Demo commands (manual)
- `look` — Show room description and exits
- `look <item>` — Inspect an item or door
- `go <direction>` — Move by direction
- `north`/`n`, `south`/`s`, `east`/`e`, `west`/`w` — Move
- `take <item>` — Pick up an item if present
- `unlock door` / `unlock east` — Unlock the door
- `open door` / `open east` — Open the door
- `close door` / `close east` — Close the door
- `lock door` / `lock east` — Lock the door
- `destroy door` / `destroy east` / `destroy <item>` — Destroy a door or item
- `inventory` / `i` — Show what you carry
- `quit` / `exit` — Exit the demo

## Example (core engine + doors/keys only)
```csharp
using System;
using System.Linq;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;

// Locations
var hallway = new Location("hallway", "A quiet hallway with a window and a small table.");
var study = new Location("study", "A quiet study with a heavy desk and a single lamp.");

var key = new Key("study_key", "brass key", "A small brass key with a worn bow.")
    .SetWeight(0.01f)
    .AddAliases("key", "brass");

var photo = new Item("photo", "old photo", "A faded photo of a stranger.")
    .SetWeight(0.05f)
    .AddAliases("photo", "picture", "image", "old photo", "old");

hallway.AddItem(key);
study.AddItem(photo);

var studyDoor = new Door("study_door", "study door", "A sturdy door with a brass plate.")
    .RequiresKey(key)
    .SetReaction(DoorAction.Unlock, "The lock clicks.")
    .SetReaction(DoorAction.UnlockFailed, "The key does not fit.")
    .SetReaction(DoorAction.Open, "The door swings open.")
    .SetReaction(DoorAction.OpenFailed, "The door refuses to budge.")
    .SetReaction(DoorAction.Close, "The door clicks shut.")
    .SetReaction(DoorAction.Lock, "You hear the lock catch.")
    .SetReaction(DoorAction.Destroy, "In a Hulk inspired rage you attack the door. Splinters fly. CRASH, BANG, KABOOM!!!\nThe hinges snap and the door slumps aside.");

hallway.AddExit(Direction.East, study, studyDoor);

var state = new GameState(hallway, worldLocations: new[] { hallway, study });

Console.WriteLine("=== THE LOCKED STUDY (Slice 2) ===");
Console.WriteLine("Commands: Look, Look <Item>, Go <Direction>, Take <Item>, Unlock/Open/Close/Lock/Destroy Door, Inventory, North/South/East/West, Quit");

void ShowRoom()
{
    var location = state.CurrentLocation;
    Console.WriteLine($"\nRoom: {location.Id.ToProperCase()}");
    Console.WriteLine(location.GetDescription());

    var items = location.Items.Select(item => item.Name.ToProperCase()).ToList();
    Console.WriteLine(items.Count > 0
        ? $"In the room you can see the following things: {items.CommaJoin()}"
        : "Items: None");

    var exits = location.Exits
        .Select(kvp =>
        {
            var direction = kvp.Key.ToString().ToLowerInvariant().ToProperCase();
            var door = kvp.Value.Door;
            if (door == null) return direction;
            var stateText = door.State.ToString().ToLowerInvariant().ToProperCase();
            return $"{direction} ({door.Name.ToProperCase()}, {stateText})";
        });

    Console.WriteLine($"Exits: {exits.CommaJoin()}");
}

Direction? ParseDirection(string token)
{
    return token switch
    {
        "north" or "n" => Direction.North,
        "south" or "s" => Direction.South,
        "east" or "e" => Direction.East,
        "west" or "w" => Direction.West,
        _ => null
    };
}

Direction? ResolveDoorDirection(string token)
{
    if (token == "door")
    {
        var doorExits = state.CurrentLocation.Exits
            .Where(kvp => kvp.Value.Door != null)
            .Select(kvp => kvp.Key)
            .ToList();

        if (doorExits.Count == 1) return doorExits[0];
        return null;
    }

    return ParseDirection(token);
}

bool TryShowDoor(string token)
{
    var direction = ResolveDoorDirection(token);
    if (direction == null) return false;

    var exit = state.CurrentLocation.GetExit(direction.Value);
    if (exit?.Door == null) return false;

    var door = exit.Door;
    Console.WriteLine($"{door.Name.ToProperCase()}: {door.GetDescription()} ({door.State.ToString().ToLowerInvariant().ToProperCase()})");
    return true;
}

void ShowInventory()
{
    var items = state.Inventory.Items.Select(i => i.Name);
    if (!items.Any())
    {
        Console.WriteLine("You carry nothing.");
        return;
    }

    var totalWeight = state.Inventory.TotalWeight;
    Console.WriteLine($"You carry: {items.CommaJoin()} (Total weight: {totalWeight:0.##})");
}

bool HandleLook(string input)
{
    if (input == "look")
    {
        ShowRoom();
        return true;
    }

    if (!input.StartsWith("look ")) return false;

    var token = input.Replace("look ", "").Trim();
    if (string.IsNullOrWhiteSpace(token))
    {
        Console.WriteLine("Look at what?");
        return true;
    }

    if (TryShowDoor(token))
    {
        return true;
    }

    var item = state.CurrentLocation.FindItem(token) ?? state.Inventory.FindItem(token);
    if (item == null)
    {
        Console.WriteLine($"You do not see a {token} here.");
        return true;
    }

    Console.WriteLine($"{item.Name.ToProperCase()}: {item.GetDescription()}");
    return true;
}

bool HandleInventory(string input)
{
    if (input is not ("inventory" or "i")) return false;
    ShowInventory();
    return true;
}

bool HandleTake(string input)
{
    if (!input.StartsWith("take ")) return false;

    var token = input.Replace("take ", "").Trim();
    if (string.IsNullOrWhiteSpace(token))
    {
        Console.WriteLine("Take what? Try: take key.");
        return true;
    }

    var foundItem = state.CurrentLocation.FindItem(token);
    if (foundItem == null)
    {
        Console.WriteLine($"There is no {token} here.");
        return true;
    }

    state.CurrentLocation.RemoveItem(foundItem);
    state.Inventory.Add(foundItem);
    Console.WriteLine($"You take the {foundItem.Name.ToProperCase()}.");
    return true;
}

bool HandleUnlock(string input)
{
    if (!input.StartsWith("unlock ")) return false;

    var token = input.Replace("unlock ", "").Trim();
    var direction = ResolveDoorDirection(token);
    if (direction == null)
    {
        Console.WriteLine("Unlock which door? Try: unlock east.");
        return true;
    }

    var exit = state.CurrentLocation.GetExit(direction.Value);
    if (exit?.Door == null)
    {
        Console.WriteLine("There is no door that way.");
        return true;
    }

    var keyInInventory = state.Inventory.Items.OfType<Key>().FirstOrDefault();
    if (keyInInventory == null)
    {
        Console.WriteLine("You do not have a key.");
        return true;
    }

    var unlocked = exit.Door.Unlock(keyInInventory);
    if (unlocked)
    {
        Console.WriteLine(exit.Door.GetReaction(DoorAction.Unlock) ?? "Unlocked.");
    }
    else
    {
        Console.WriteLine(exit.Door.GetReaction(DoorAction.UnlockFailed) ?? "It will not unlock.");
    }

    return true;
}

bool HandleOpen(string input)
{
    if (!input.StartsWith("open ")) return false;

    var token = input.Replace("open ", "").Trim();
    var direction = ResolveDoorDirection(token);
    if (direction == null)
    {
        Console.WriteLine("Open which door? Try: open east.");
        return true;
    }

    var exit = state.CurrentLocation.GetExit(direction.Value);
    if (exit?.Door == null)
    {
        Console.WriteLine("There is no door that way.");
        return true;
    }

    var opened = exit.Door.Open();
    Console.WriteLine(opened
        ? (exit.Door.GetReaction(DoorAction.Open) ?? "Opened.")
        : (exit.Door.GetReaction(DoorAction.OpenFailed) ?? "It will not open."));
    return true;
}

bool HandleClose(string input)
{
    if (!input.StartsWith("close ")) return false;

    var token = input.Replace("close ", "").Trim();
    var direction = ResolveDoorDirection(token);
    if (direction == null)
    {
        Console.WriteLine("Close which door? Try: close east.");
        return true;
    }

    var exit = state.CurrentLocation.GetExit(direction.Value);
    if (exit?.Door == null)
    {
        Console.WriteLine("There is no door that way.");
        return true;
    }

    var closed = exit.Door.Close();
    Console.WriteLine(closed
        ? (exit.Door.GetReaction(DoorAction.Close) ?? "Closed.")
        : "It will not close.");
    return true;
}

bool HandleLock(string input)
{
    if (!input.StartsWith("lock ")) return false;

    var token = input.Replace("lock ", "").Trim();
    var direction = ResolveDoorDirection(token);
    if (direction == null)
    {
        Console.WriteLine("Lock which door? Try: lock east.");
        return true;
    }

    var exit = state.CurrentLocation.GetExit(direction.Value);
    if (exit?.Door == null)
    {
        Console.WriteLine("There is no door that way.");
        return true;
    }

    var keyInInventory = state.Inventory.Items.OfType<Key>().FirstOrDefault();
    if (keyInInventory == null)
    {
        Console.WriteLine("You do not have a key.");
        return true;
    }

    var locked = exit.Door.Lock(keyInInventory);
    Console.WriteLine(locked
        ? (exit.Door.GetReaction(DoorAction.Lock) ?? "Locked.")
        : "It will not lock.");
    return true;
}

bool HandleDestroy(string input)
{
    if (!input.StartsWith("destroy ")) return false;

    var token = input.Replace("destroy ", "").Trim();
    if (string.IsNullOrWhiteSpace(token))
    {
        Console.WriteLine("Destroy what? Try: destroy door.");
        return true;
    }

    var direction = ResolveDoorDirection(token);
    if (direction != null)
    {
        var exit = state.CurrentLocation.GetExit(direction.Value);
        if (exit?.Door == null)
        {
            Console.WriteLine("There is no door that way.");
            return true;
        }

        exit.Door.Destroy();
        Console.WriteLine(exit.Door.GetReaction(DoorAction.Destroy) ?? "Destroyed.");
        return true;
    }

    var item = state.CurrentLocation.FindItem(token) ?? state.Inventory.FindItem(token);
    if (item == null)
    {
        Console.WriteLine($"You do not see a {token} here.");
        return true;
    }

    var reaction = item.GetReaction(ItemAction.Destroy);
    if (string.IsNullOrWhiteSpace(reaction))
    {
        Console.WriteLine($"Don't be silly. You are not destroying the {item.Name.ToProperCase()}.");
        return true;
    }

    if (state.Inventory.Items.Contains(item))
    {
        state.Inventory.Remove(item);
    }
    else
    {
        state.CurrentLocation.RemoveItem(item);
    }

    Console.WriteLine(reaction);
    return true;
}

bool HandleGo(string input)
{
    if (!input.StartsWith("go ")) return false;

    var token = input.Replace("go ", "").Trim();
    var direction = ParseDirection(token);
    if (direction == null)
    {
        Console.WriteLine("Go where? Try: go east.");
        return true;
    }

    var moved = state.Move(direction.Value);
    if (moved)
    {
        ShowRoom();
    }
    else
    {
        Console.WriteLine(state.LastMoveError ?? "You cannot go that way.");
    }

    return true;
}

ShowRoom();

while (true)
{
    Console.Write("\n> ");
    var input = Console.ReadLine()?.Trim().ToLowerInvariant();
    if (string.IsNullOrWhiteSpace(input)) continue;

    if (input == "quit" || input == "exit" || input == "/exit" || input == "/quit") break;

    if (HandleLook(input)) continue;
    if (HandleInventory(input)) continue;

    if (input == "take")
    {
        Console.WriteLine("Take what? Try: take key.");
        continue;
    }

    if (HandleTake(input)) continue;
    if (HandleUnlock(input)) continue;
    if (HandleOpen(input)) continue;
    if (HandleClose(input)) continue;
    if (HandleLock(input)) continue;
    if (HandleDestroy(input)) continue;

    var directionInput = ParseDirection(input);
    if (directionInput != null)
    {
        var moved = state.Move(directionInput.Value);
        if (moved)
        {
            ShowRoom();
        }
        else
        {
            Console.WriteLine(state.LastMoveError ?? "You cannot go that way.");
        }
        continue;
    }

    if (HandleGo(input)) continue;

    if (input.StartsWith("use "))
    {
        Console.WriteLine("Try: unlock door, open door, or lock door.");
        continue;
    }

    Console.WriteLine("Try: look, look <item>, go <direction>, take <item>, unlock/open/close/lock/destroy door, inventory, quit.");
}
```
