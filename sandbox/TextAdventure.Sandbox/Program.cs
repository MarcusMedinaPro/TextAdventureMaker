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

// Slice 6: The Key Under the Stone
// Tests: event subscriptions, hidden key reveal, door unlock, auto-look after go.

var garden = new Location("garden", "A quiet garden with a weathered gate and patient silence.");
var courtyard = new Location("courtyard", "A sheltered courtyard beyond the gate, lit by moonlight.");

var stone = new Item("stone", "stone", "A heavy flat stone with moss on one edge.")
    .AddAliases("slab")
    .SetReaction(ItemAction.Move, "The stone scrapes across the soil and tilts.");
var key = new Key("garden_key", "iron key", "A cold iron key hidden beneath the stone.")
    .AddAliases("key", "iron")
    .SetWeight(0.02f);

var gate = new Door("garden_gate", "garden gate", "A barred gate choked with ivy.", DoorState.Locked)
    .RequiresKey(key)
    .SetReaction(DoorAction.Unlock, "The lock gives way with a croak of rust.")
    .SetReaction(DoorAction.Open, "The gate swings open in a creak of metal.")
    .SetReaction(DoorAction.OpenFailed, "The gate resists while the lock refuses to budge.");

garden.AddItem(stone);
garden.AddExit(Direction.North, courtyard, gate);
courtyard.AddExit(Direction.South, garden, gate);

var state = new GameState(garden, worldLocations: new[] { garden, courtyard })
{
    EnableFuzzyMatching = true,
    FuzzyMaxDistance = 1
};

var keyRevealed = false;
state.Events.Subscribe(GameEventType.PickupItem, e =>
{
    if (keyRevealed) return;
    if (e.Item?.Id.Is("stone") != true) return;

    keyRevealed = true;
    garden.AddItem(key);
    Console.WriteLine("> As you move the stone, a rusted key glints free of the earth.");
});
state.Events.Subscribe(GameEventType.UnlockDoor, e =>
{
    if (e.Door?.Id.Is("garden_gate") == true)
    {
        Console.WriteLine("> The gate rattles as the lock gives up.");
    }
});

var parser = new KeywordParser(KeywordParserConfigBuilder.BritishDefaults()
    .WithLook("look", "l")
    .WithExamine("examine", "x")
    .WithMove("move", "push", "shift", "slide")
    .WithTake("take", "grab")
    .WithOpen("open", "pull")
    .WithUnlock("unlock", "unseal")
    .WithGo("go", "travel")
    .WithInventory("inventory", "inv", "i")
    .WithUse("use")
    .WithFuzzyMatching(true, 1)
    .WithDirectionAliases(new Dictionary<string, Direction>(StringComparer.OrdinalIgnoreCase)
    {
        ["n"] = Direction.North,
        ["north"] = Direction.North,
        ["s"] = Direction.South,
        ["south"] = Direction.South
    })
    .Build());

Console.WriteLine("=== THE KEY UNDER THE STONE (Slice 6) ===");
Console.WriteLine("Goal: reveal the hidden key, unlock the gate, and step into the courtyard.");
Console.WriteLine("Commands: look, examine, move stone, take key, unlock gate, open gate, go north/south, inventory, quit.");
ShowRoom();

while (true)
{
    Console.Write("\n> ");
    var input = Console.ReadLine()?.Trim();
    if (string.IsNullOrWhiteSpace(input)) continue;

    var command = parser.Parse(input);
    var result = state.Execute(command);
    PrintResult(result);

    var movedStone = command is MoveCommand { Target: var moveTarget } && moveTarget.Is("stone") && result.Success;
    if (movedStone)
    {
        state.Events.Publish(new GameEvent(GameEventType.PickupItem, state, state.CurrentLocation, item: stone));
    }

    if (command is GoCommand && result.Success && !result.ShouldQuit)
    {
        ShowRoom();
    }

    if (result.ShouldQuit) break;
}

void PrintResult(CommandResult result)
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
