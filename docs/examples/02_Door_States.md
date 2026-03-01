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
using static MarcusMedina.TextAdventure.Extensions.ConsoleExtensions;

// Slice 2 — Doors + Keys
// Tests:
// - Door states (Open/Closed/Locked/Destroyed)
// - Door actions (Open/Close/Lock/Unlock/Destroy)
// - Door events (OnOpen/OnClose/OnLock/OnUnlock/OnDestroy)
// - Location.AddExit with door

// Locations
Location hallway = new("hallway", "A quiet hallway with a cold draught.");
Location study = new("study", "A small study with a single lamp and a faded photo.");

// Items
Key key = new("study_key", "brass key", "A small brass key with a worn bow.")
    .AddAliases("key", "brass");

Item photo = new("photo", "old photo", "A faded photo of someone you almost remember.")
    .AddAliases("photo", "picture")
    .SetTakeable(false);

Item lamp = new("lamp", "brass lamp", "A brass lamp with a stubborn little switch.")
    .AddAliases("lamp", "light")
    .SetTakeable(false)
    .SetReaction(ItemAction.TakeFailed, "The lamp is fixed in place, and far too proud to be moved.")
    .SetReaction(ItemAction.Use, "You click the lamp on. The study warms instantly.")
    .SetReaction(ItemAction.Move, "You nudge the lamp. It stays put, a creature of habit.");

hallway.AddItem(key);
study.AddItem(photo);
study.AddItem(lamp);

// Door
Door door = new("study_door", "study door", "A sturdy door with a stubborn lock.", DoorState.Locked)
    .RequiresKey(key)
    .SetReaction(DoorAction.Unlock, "The lock clicks open.")
    .SetReaction(DoorAction.UnlockFailed, "The key refuses to oblige.")
    .SetReaction(DoorAction.Open, "The door opens with a reluctant creak.")
    .SetReaction(DoorAction.OpenFailed, "The door refuses to budge.")
    .SetReaction(DoorAction.Close, "The door settles shut with a soft thud.")
    .SetReaction(DoorAction.Lock, "The lock catches with a satisfying clunk.")
    .SetReaction(DoorAction.Destroy, "You deliver a decisive blow. The door gives up entirely.");

door.OnOpen += HandleDoorOpen;
door.OnClose += HandleDoorClose;
door.OnLock += HandleDoorLock;
door.OnUnlock += HandleDoorUnlock;
door.OnDestroy += HandleDoorDestroy;

hallway.AddExit(Direction.East, study, door);

GameState state = new(hallway, worldLocations: [hallway, study])
{
    EnableFuzzyMatching = true,
    FuzzyMaxDistance = 1
};

SetupC64("THE DOOR STATES (Slice 2) - Text Adventure Sandbox");
WriteLineC64("=== THE DOOR STATES (Slice 2) ===");
WriteLineC64("Goal: unlock the door, enter the study, and experiment with the door states.");
WriteLineC64("Test: door event hooks should announce each door state change.");
ShowRoom();

while (true)
{
    WriteLineC64();
    WritePromptC64("> ");
    string? input = Console.ReadLine();
    if (input is null)
        break;

    string trimmed = input.Trim();
    if (string.IsNullOrWhiteSpace(trimmed))
        continue;

    if (HandleUtility(trimmed, out bool quit))
    {
        if (quit)
            break;

        continue;
    }

    HandleCommand(trimmed);
}

void ShowRoom()
{
    ILocation location = state.CurrentLocation;
    WriteLineC64();
    WriteLineC64($"Room: {location.Id.ToProperCase()}");
    WriteLineC64(location.GetDescription());

    string items = location.Items.CommaJoinNames(properCase: true);
    WriteLineC64(string.IsNullOrWhiteSpace(items) ? "Items: None" : $"Items: {items}");

    List<string> exits = location.Exits
        .Select(exit => FormatExit(exit.Key, exit.Value))
        .ToList();

    WriteLineC64(exits.Count > 0 ? $"Exits: {exits.CommaJoin()}" : "Exits: None");
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
        WriteLineC64("Look at what?");
        return;
    }

    if (token is "door" or "east" or "e" or "west" or "w")
    {
        var exit = ResolveDoorExit(token);
        if (exit?.Door == null)
        {
            WriteLineC64("There is no door here.");
            return;
        }

        string stateText = exit.Door.State.ToString().ToLowerInvariant().ToProperCase();
        WriteLineC64($"{exit.Door.Name.ToProperCase()}: {exit.Door.GetDescription()} ({stateText})");
        return;
    }

    var item = state.CurrentLocation.FindItem(token) ?? state.Inventory.FindItem(token);
    if (item == null)
    {
        WriteLineC64($"You do not see a {token} here.");
        return;
    }

    WriteLineC64($"{item.Name.ToProperCase()}: {item.GetDescription()}");
}

void ShowInventory()
{
    string items = state.Inventory.Items.CommaJoinNames(properCase: true);
    WriteLineC64(string.IsNullOrWhiteSpace(items) ? "You carry nothing." : $"You carry: {items}");
}

void HandleTake(string token)
{
    if (string.IsNullOrWhiteSpace(token))
    {
        WriteLineC64("Take what?");
        return;
    }

    var item = state.CurrentLocation.FindItem(token);
    if (item == null)
    {
        WriteLineC64($"There is no {token} here.");
        return;
    }

    if (!item.Takeable)
    {
        var reaction = item.GetReaction(ItemAction.TakeFailed);
        WriteLineC64(reaction ?? $"You cannot take the {item.Name.ToProperCase()}.");
        return;
    }

    state.CurrentLocation.RemoveItem(item);
    state.Inventory.Add(item);
    WriteLineC64($"You take the {item.Name.ToProperCase()}.");
}

void HandleTakeAll()
{
    if (state.CurrentLocation.Items.Count == 0)
    {
        WriteLineC64("There is nothing to take.");
        return;
    }

    List<IItem> taken = [];
    foreach (IItem item in state.CurrentLocation.Items.ToList())
    {
        if (!item.Takeable)
            continue;

        state.CurrentLocation.RemoveItem(item);
        state.Inventory.Add(item);
        taken.Add(item);
    }

    WriteLineC64(taken.Count > 0
        ? $"You take: {taken.CommaJoinNames(properCase: true)}."
        : "There is nothing here you can take.");
}

void HandleUnlock(string token)
{
    var exit = ResolveDoorExit(token);
    if (exit?.Door == null)
    {
        WriteLineC64("There is no door to unlock.");
        return;
    }

    var keyInInventory = state.Inventory.Items.OfType<Key>().FirstOrDefault();
    if (keyInInventory == null)
    {
        WriteLineC64("You do not have a key.");
        return;
    }

    bool unlocked = exit.Door.Unlock(keyInInventory);
    WriteLineC64(unlocked
        ? (exit.Door.GetReaction(DoorAction.Unlock) ?? "Unlocked.")
        : (exit.Door.GetReaction(DoorAction.UnlockFailed) ?? "It will not unlock."));
}

void HandleOpen(string token)
{
    var exit = ResolveDoorExit(token);
    if (exit?.Door == null)
    {
        WriteLineC64("There is no door to open.");
        return;
    }

    bool opened = exit.Door.Open();
    WriteLineC64(opened
        ? (exit.Door.GetReaction(DoorAction.Open) ?? "Opened.")
        : (exit.Door.GetReaction(DoorAction.OpenFailed) ?? "It will not open."));
}

void HandleClose(string token)
{
    var exit = ResolveDoorExit(token);
    if (exit?.Door == null)
    {
        WriteLineC64("There is no door to close.");
        return;
    }

    bool closed = exit.Door.Close();
    WriteLineC64(closed
        ? (exit.Door.GetReaction(DoorAction.Close) ?? "Closed.")
        : "It will not close.");
}

void HandleLock(string token)
{
    var exit = ResolveDoorExit(token);
    if (exit?.Door == null)
    {
        WriteLineC64("There is no door to lock.");
        return;
    }

    var keyInInventory = state.Inventory.Items.OfType<Key>().FirstOrDefault();
    if (keyInInventory == null)
    {
        WriteLineC64("You do not have a key.");
        return;
    }

    bool locked = exit.Door.Lock(keyInInventory);
    WriteLineC64(locked
        ? (exit.Door.GetReaction(DoorAction.Lock) ?? "Locked.")
        : "It will not lock.");
}

void HandleDestroy(string token)
{
    var exit = ResolveDoorExit(token);
    if (exit?.Door == null)
    {
        WriteLineC64("There is no door to destroy.");
        return;
    }

    exit.Door.Destroy();
    WriteLineC64(exit.Door.GetReaction(DoorAction.Destroy) ?? "Destroyed.");
}

void HandleUse(string token)
{
    if (string.IsNullOrWhiteSpace(token))
    {
        WriteLineC64("Use what?");
        return;
    }

    var item = state.CurrentLocation.FindItem(token) ?? state.Inventory.FindItem(token);
    if (item == null)
    {
        WriteLineC64($"You do not see a {token} here.");
        return;
    }

    item.Use();
    string? reaction = item.GetReaction(ItemAction.Use);
    WriteLineC64(reaction ?? $"You use the {item.Name.ToProperCase()}.");
}

void Move(Direction direction)
{
    bool moved = state.Move(direction);
    if (!moved)
    {
        WriteLineC64(state.LastMoveError ?? "You cannot go that way.");
        return;
    }

    ShowRoom();
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

    WriteLineC64("Go where? Try: go east.");
}

bool HandleUtility(string trimmed, out bool quit)
{
    string lower = trimmed.Lower();
    if (lower is "quit" or "exit")
    {
        quit = true;
        return true;
    }

    quit = false;

    if (lower is "look" or "l")
    {
        ShowRoom();
        return true;
    }

    if (lower is "inventory" or "inv" or "i")
    {
        ShowInventory();
        return true;
    }

    if (lower is "take all" or "take everything")
    {
        HandleTakeAll();
        return true;
    }

    return false;
}

void HandleCommand(string trimmed)
{
    if (trimmed.StartsWithIgnoreCase("look "))
    {
        ShowTarget(trimmed[5..]);
        return;
    }

    if (trimmed.StartsWithIgnoreCase("take "))
    {
        HandleTake(trimmed[5..]);
        return;
    }

    if (trimmed.StartsWithIgnoreCase("unlock "))
    {
        HandleUnlock(trimmed[7..]);
        return;
    }

    if (trimmed.StartsWithIgnoreCase("open "))
    {
        HandleOpen(trimmed[5..]);
        return;
    }

    if (trimmed.StartsWithIgnoreCase("close "))
    {
        HandleClose(trimmed[6..]);
        return;
    }

    if (trimmed.StartsWithIgnoreCase("lock "))
    {
        HandleLock(trimmed[5..]);
        return;
    }

    if (trimmed.StartsWithIgnoreCase("destroy "))
    {
        HandleDestroy(trimmed[8..]);
        return;
    }

    if (trimmed.StartsWithIgnoreCase("turn on "))
    {
        HandleUse(trimmed[8..]);
        return;
    }

    if (trimmed.StartsWithIgnoreCase("turn off "))
    {
        WriteLineC64("You turn it off. The lamp sulks quietly.");
        return;
    }

    if (trimmed.StartsWithIgnoreCase("use "))
    {
        HandleUse(trimmed[4..]);
        return;
    }

    if (trimmed is "east" or "e")
    {
        Move(Direction.East);
        return;
    }

    if (trimmed is "west" or "w")
    {
        Move(Direction.West);
        return;
    }

    if (trimmed.StartsWithIgnoreCase("go "))
    {
        HandleGo(trimmed[3..]);
        return;
    }

    WriteLineC64("Try: look, take key, unlock/open/close/lock/destroy door, east/west, inventory, quit.");
}

string FormatExit(Direction direction, Exit exit)
{
    string directionName = direction.ToString().ToLowerInvariant().ToProperCase();
    if (exit.Door == null)
        return directionName;

    string stateText = exit.Door.State.ToString().ToLowerInvariant().ToProperCase();
    return $"{directionName} ({exit.Door.Name.ToProperCase()}, {stateText})";
}

void HandleDoorOpen(IDoor updated) =>
    WriteLineC64($"Event: {updated.Name.ToProperCase()} opened.");

void HandleDoorClose(IDoor updated) =>
    WriteLineC64($"Event: {updated.Name.ToProperCase()} closed.");

void HandleDoorLock(IDoor updated) =>
    WriteLineC64($"Event: {updated.Name.ToProperCase()} locked.");

void HandleDoorUnlock(IDoor updated) =>
    WriteLineC64($"Event: {updated.Name.ToProperCase()} unlocked.");

void HandleDoorDestroy(IDoor updated) =>
    WriteLineC64($"Event: {updated.Name.ToProperCase()} destroyed.");
```
