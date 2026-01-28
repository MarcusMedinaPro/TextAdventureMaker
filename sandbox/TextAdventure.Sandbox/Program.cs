using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Models;

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

var state = new GameState(hallway, worldLocations: [hallway, study])
{
    EnableFuzzyMatching = true,
    FuzzyMaxDistance = 1
};

Console.WriteLine("=== THE LOCKED STUDY (Slice 2) ===");
Console.WriteLine("Goal: unlock the door, enter the study, and explore.");
Console.WriteLine("Type 'look' to recheck your surroundings.");
ShowRoom();

while (true)
{
    Console.Write("\n> ");
    var input = Console.ReadLine()?.Trim();
    if (string.IsNullOrWhiteSpace(input))
        continue;

    var normalised = input.Lower();

    if (normalised is "quit" or "exit")
        break;

    if (normalised is "look" or "l")
    {
        ShowRoom();
        continue;
    }

    if (normalised.StartsWith("look "))
    {
        ShowTarget(normalised[5..]);
        continue;
    }

    if (normalised is "inventory" or "inv" or "i")
    {
        ShowInventory();
        continue;
    }

    if (normalised is "take all" or "take everything")
    {
        HandleTakeAll();
        continue;
    }

    if (normalised.StartsWith("take "))
    {
        HandleTake(normalised[5..]);
        continue;
    }

    if (normalised.StartsWith("unlock "))
    {
        HandleUnlock(normalised[7..]);
        continue;
    }

    if (normalised.StartsWith("open "))
    {
        HandleOpen(normalised[5..]);
        continue;
    }

    if (normalised.StartsWith("close "))
    {
        HandleClose(normalised[6..]);
        continue;
    }

    if (normalised.StartsWith("lock "))
    {
        HandleLock(normalised[5..]);
        continue;
    }

    if (normalised.StartsWith("destroy "))
    {
        HandleDestroy(normalised[8..]);
        continue;
    }

    if (normalised.StartsWith("turn on "))
    {
        HandleUse(normalised[8..]);
        continue;
    }

    if (normalised.StartsWith("turn off "))
    {
        Console.WriteLine("You turn it off. The lamp sulks quietly.");
        continue;
    }

    if (normalised.StartsWith("use "))
    {
        HandleUse(normalised[4..]);
        continue;
    }

    var moved = normalised switch
    {
        "east" or "e" => Move(Direction.East),
        "west" or "w" => Move(Direction.West),
        _ => false
    };

    if (moved)
        continue;

    if (normalised.StartsWith("go "))
    {
        HandleGo(normalised[3..]);
        continue;
    }

    Console.WriteLine("Try: look, take key, unlock/open/close/lock/destroy door, east/west, inventory, quit.");
}

Exit? ResolveDoorExit(string token) => token is "east" or "e"
        ? state.CurrentLocation.GetExit(Direction.East)
        : token is "west" or "w"
        ? state.CurrentLocation.GetExit(Direction.West)
        : token is "door"
        ? state.CurrentLocation.Exits.Values.FirstOrDefault(exit => exit.Door != null)
        : state.CurrentLocation.Exits.Values.FirstOrDefault(exit => exit.Door != null);

bool Move(Direction direction)
{
    if (!state.Move(direction))
    {
        Console.WriteLine(state.LastMoveError ?? "You cannot go that way.");
        return false;
    }

    ShowRoom();
    return true;
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
            if (exitDoor == null)
                return direction;
            var stateText = exitDoor.State.ToString().ToLowerInvariant().ToProperCase();
            return $"{direction} ({exitDoor.Name.ToProperCase()}, {stateText})";
        })
        .ToList();

    Console.WriteLine(exits.Count > 0 ? $"Exits: {exits.CommaJoin()}" : "Exits: None");
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

    _ = state.CurrentLocation.RemoveItem(item);
    _ = state.Inventory.Add(item);
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
        if (!item.Takeable)
            continue;
        _ = state.CurrentLocation.RemoveItem(item);
        _ = state.Inventory.Add(item);
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

    _ = exit.Door.Destroy();
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

void HandleGo(string token)
{
    if (token is "east" or "e")
    {
        _ = Move(Direction.East);
        return;
    }

    if (token is "west" or "w")
    {
        _ = Move(Direction.West);
        return;
    }

    Console.WriteLine("Go where? Try: go east.");
}