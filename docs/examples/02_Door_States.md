# The Door States

_Slice tag: Slice 2.1 — Door States. Demo focuses on door state transitions (open/close/lock/unlock/destroy)._ 

## Story beats (max ~10 steps)
1) You stand in the hallway.
2) A locked door leads east.
3) Find the key.
4) Unlock, open, and enter the study.
5) Try closing or locking the door.
6) Optionally destroy the door.

## Map (rough layout)
```
          N
    W           E
          S

┌────────────┐     ┌────────────┐
│            │     │            │
│  Hallway   │─────│   Study    │
│            │     │            │
│     K      │     │     P      │
└────────────┘     └────────────┘

K = Brass key
P = Photo
```

## Example (door state transitions)
```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Models;

// Slice 2.1 — Door States
// New functions tested:
// - Door states (Open/Closed/Locked/Destroyed)
// - Door actions (Open/Close/Lock/Unlock/Destroy)
// - Location.AddExit with door

// Locations
var hallway = new Location("hallway", "A quiet hallway with a cold draft.");
var study = new Location("study", "A small study with a single lamp and a faded photo.");

// Items
var key = new Key("study_key", "brass key", "A small brass key with a worn bow.")
    .AddAliases("key", "brass");

var photo = new Item("photo", "old photo", "A faded photo of someone you almost remember.")
    .AddAliases("photo", "picture")
    .SetTakeable(false);

var lamp = new Item("lamp", "brass lamp", "A brass lamp with a stubborn little switch.")
    .AddAliases("lamp", "light")
    .SetTakeable(false)
    .SetReaction(ItemAction.TakeFailed, "The lamp is fixed in place, and far too proud to be moved.")
    .SetReaction(ItemAction.Use, "You click the lamp on. The study warms instantly.")
    .SetReaction(ItemAction.Move, "You nudge the lamp. It stays put, a creature of habit.");

hallway.AddItem(key);
study.AddItem(photo);
study.AddItem(lamp);

// Door
var door = new Door("study_door", "study door", "A sturdy door with a stubborn lock.", DoorState.Locked)
    .RequiresKey(key)
    .SetReaction(DoorAction.Unlock, "The lock clicks open.")
    .SetReaction(DoorAction.UnlockFailed, "The key refuses to oblige.")
    .SetReaction(DoorAction.Open, "The door opens with a reluctant creak.")
    .SetReaction(DoorAction.OpenFailed, "The door refuses to budge.")
    .SetReaction(DoorAction.Close, "The door settles shut with a soft thud.")
    .SetReaction(DoorAction.Lock, "The lock catches with a satisfying clunk.")
    .SetReaction(DoorAction.Destroy, "You deliver a decisive blow. The door gives up entirely.");

hallway.AddExit(Direction.East, study, door);

var state = new GameState(hallway, worldLocations: new[] { hallway, study })
{
    EnableFuzzyMatching = true,
    FuzzyMaxDistance = 1
};

Console.WriteLine("=== THE DOOR STATES (Slice 2.1) ===");
Console.WriteLine("Goal: unlock the door, enter the study, and experiment with the door states.");
ShowRoom();

while (true)
{
    Console.Write("\n> ");
    var input = Console.ReadLine()?.Trim();
    if (string.IsNullOrWhiteSpace(input)) continue;

    var normalized = input.Lower();

    if (normalized is "quit" or "exit")
    {
        break;
    }

    if (normalized is "look" or "l")
    {
        ShowRoom();
        continue;
    }

    if (normalized.StartsWith("look "))
    {
        ShowTarget(normalized[5..]);
        continue;
    }

    if (normalized is "inventory" or "inv" or "i")
    {
        ShowInventory();
        continue;
    }

    if (normalized is "take all" or "take everything")
    {
        HandleTakeAll();
        continue;
    }

    if (normalized.StartsWith("take "))
    {
        HandleTake(normalized[5..]);
        continue;
    }

    if (normalized.StartsWith("unlock "))
    {
        HandleUnlock(normalized[7..]);
        continue;
    }

    if (normalized.StartsWith("open "))
    {
        HandleOpen(normalized[5..]);
        continue;
    }

    if (normalized.StartsWith("close "))
    {
        HandleClose(normalized[6..]);
        continue;
    }

    if (normalized.StartsWith("lock "))
    {
        HandleLock(normalized[5..]);
        continue;
    }

    if (normalized.StartsWith("destroy "))
    {
        HandleDestroy(normalized[8..]);
        continue;
    }

    if (normalized.StartsWith("turn on "))
    {
        HandleUse(normalized[8..]);
        continue;
    }

    if (normalized.StartsWith("turn off "))
    {
        Console.WriteLine("You turn it off. The lamp sulks quietly.");
        continue;
    }

    if (normalized.StartsWith("use "))
    {
        HandleUse(normalized[4..]);
        continue;
    }

    if (normalized is "east" or "e")
    {
        Move(Direction.East);
        continue;
    }

    if (normalized is "west" or "w")
    {
        Move(Direction.West);
        continue;
    }

    if (normalized.StartsWith("go "))
    {
        HandleGo(normalized[3..]);
        continue;
    }

    Console.WriteLine("Try: look, take key, unlock/open/close/lock/destroy door, east/west, inventory, quit.");
}

void ShowRoom()
{
    var location = state.CurrentLocation;
    Console.WriteLine();
    Console.WriteLine($"Room: {location.Id.ToProperCase()}");
    Console.WriteLine(location.GetDescription());

    var items = location.Items.CommaJoinNames(properCase: true);
    Console.WriteLine(string.IsNullOrWhiteSpace(items) ? "Items: None" : $"Items: {items}");

    var exits = location.Exits
        .Select(exit =>
        {
            var direction = exit.Key.ToString().ToLowerInvariant().ToProperCase();
            var exitDoor = exit.Value.Door;
            if (exitDoor == null) return direction;
            var stateText = exitDoor.State.ToString().ToLowerInvariant().ToProperCase();
            return $"{direction} ({exitDoor.Name.ToProperCase()}, {stateText})";
        })
        .ToList();

    Console.WriteLine(exits.Count > 0 ? $"Exits: {exits.CommaJoin()}" : "Exits: None");
}

Exit? ResolveDoorExit(string token)
{
    if (token is "east" or "e") return state.CurrentLocation.GetExit(Direction.East);
    if (token is "west" or "w") return state.CurrentLocation.GetExit(Direction.West);

    if (token is "door")
    {
        var withDoor = state.CurrentLocation.Exits.Values.FirstOrDefault(exit => exit.Door != null);
        return withDoor;
    }

    return state.CurrentLocation.Exits.Values.FirstOrDefault(exit => exit.Door != null);
}

void ShowTarget(string token)
{
    if (string.IsNullOrWhiteSpace(token))
    {
        Console.WriteLine("Look at what?");
        return;
    }

    if (token is "door" or "east" or "e" or "west" or "w")
    {
        var exit = ResolveDoorExit(token);
        if (exit?.Door == null)
        {
            Console.WriteLine("There is no door here.");
            return;
        }

        Console.WriteLine($"{exit.Door.Name.ToProperCase()}: {exit.Door.GetDescription()} ({exit.Door.State.ToString().ToLowerInvariant().ToProperCase()})");
        return;
    }

    var item = state.CurrentLocation.FindItem(token) ?? state.Inventory.FindItem(token);
    if (item == null)
    {
        Console.WriteLine($"You do not see a {token} here.");
        return;
    }

    Console.WriteLine($"{item.Name.ToProperCase()}: {item.GetDescription()}");
}

void ShowInventory()
{
    var items = state.Inventory.Items.CommaJoinNames(properCase: true);
    Console.WriteLine(string.IsNullOrWhiteSpace(items) ? "You carry nothing." : $"You carry: {items}");
}

void HandleTake(string token)
{
    if (string.IsNullOrWhiteSpace(token))
    {
        Console.WriteLine("Take what?");
        return;
    }

    var item = state.CurrentLocation.FindItem(token);
    if (item == null)
    {
        Console.WriteLine($"There is no {token} here.");
        return;
    }

    if (!item.Takeable)
    {
        var reaction = item.GetReaction(ItemAction.TakeFailed);
        Console.WriteLine(reaction ?? $"You cannot take the {item.Name.ToProperCase()}.");
        return;
    }

    state.CurrentLocation.RemoveItem(item);
    state.Inventory.Add(item);
    Console.WriteLine($"You take the {item.Name.ToProperCase()}.");
}

void HandleTakeAll()
{
    if (state.CurrentLocation.Items.Count == 0)
    {
        Console.WriteLine("There is nothing to take.");
        return;
    }

    var taken = new List<IItem>();
    foreach (var item in state.CurrentLocation.Items.ToList())
    {
        if (!item.Takeable) continue;
        state.CurrentLocation.RemoveItem(item);
        state.Inventory.Add(item);
        taken.Add(item);
    }

    Console.WriteLine(taken.Count > 0
        ? $"You take: {taken.CommaJoinNames(properCase: true)}."
        : "There is nothing here you can take.");
}

void HandleUnlock(string token)
{
    var exit = ResolveDoorExit(token);
    if (exit?.Door == null)
    {
        Console.WriteLine("There is no door to unlock.");
        return;
    }

    var keyInInventory = state.Inventory.Items.OfType<Key>().FirstOrDefault();
    if (keyInInventory == null)
    {
        Console.WriteLine("You do not have a key.");
        return;
    }

    var unlocked = exit.Door.Unlock(keyInInventory);
    Console.WriteLine(unlocked
        ? (exit.Door.GetReaction(DoorAction.Unlock) ?? "Unlocked.")
        : (exit.Door.GetReaction(DoorAction.UnlockFailed) ?? "It will not unlock."));
}

void HandleOpen(string token)
{
    var exit = ResolveDoorExit(token);
    if (exit?.Door == null)
    {
        Console.WriteLine("There is no door to open.");
        return;
    }

    var opened = exit.Door.Open();
    Console.WriteLine(opened
        ? (exit.Door.GetReaction(DoorAction.Open) ?? "Opened.")
        : (exit.Door.GetReaction(DoorAction.OpenFailed) ?? "It will not open."));
}

void HandleClose(string token)
{
    var exit = ResolveDoorExit(token);
    if (exit?.Door == null)
    {
        Console.WriteLine("There is no door to close.");
        return;
    }

    var closed = exit.Door.Close();
    Console.WriteLine(closed
        ? (exit.Door.GetReaction(DoorAction.Close) ?? "Closed.")
        : "It will not close.");
}

void HandleLock(string token)
{
    var exit = ResolveDoorExit(token);
    if (exit?.Door == null)
    {
        Console.WriteLine("There is no door to lock.");
        return;
    }

    var keyInInventory = state.Inventory.Items.OfType<Key>().FirstOrDefault();
    if (keyInInventory == null)
    {
        Console.WriteLine("You do not have a key.");
        return;
    }

    var locked = exit.Door.Lock(keyInInventory);
    Console.WriteLine(locked
        ? (exit.Door.GetReaction(DoorAction.Lock) ?? "Locked.")
        : "It will not lock.");
}

void HandleDestroy(string token)
{
    var exit = ResolveDoorExit(token);
    if (exit?.Door == null)
    {
        Console.WriteLine("There is no door to destroy.");
        return;
    }

    exit.Door.Destroy();
    Console.WriteLine(exit.Door.GetReaction(DoorAction.Destroy) ?? "Destroyed.");
}

void HandleUse(string token)
{
    if (string.IsNullOrWhiteSpace(token))
    {
        Console.WriteLine("Use what?");
        return;
    }

    var item = state.CurrentLocation.FindItem(token) ?? state.Inventory.FindItem(token);
    if (item == null)
    {
        Console.WriteLine($"You do not see a {token} here.");
        return;
    }

    item.Use();
    var reaction = item.GetReaction(ItemAction.Use);
    Console.WriteLine(reaction ?? $"You use the {item.Name.ToProperCase()}.");
}

void Move(Direction direction)
{
    var moved = state.Move(direction);
    if (moved)
    {
        ShowRoom();
    }
    else
    {
        Console.WriteLine(state.LastMoveError ?? "You cannot go that way.");
    }
}

void HandleGo(string token)
{
    if (token is "east" or "e")
    {
        Move(Direction.East);
        return;
    }

    if (token is "west" or "w")
    {
        Move(Direction.West);
        return;
    }

    Console.WriteLine("Go where? Try: go east.");
}
```
