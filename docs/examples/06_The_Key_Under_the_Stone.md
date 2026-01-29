# The Key Under the Stone

_Slice tag: Slice 6 — Event System (Observer). This demo shows how to subscribe to game events, reveal hidden content, and react to door actions while keeping an automatic look after every successful go._

A quiet garden circles a locked gate. A sodden stone is the only obvious thing to touch.

## Goal

Lift the stone, reveal the hidden key through an event, unlock the gate, and step into the courtyard.

## Map (rough layout)

```
          N
    W           E
          S

┌────────────────────┐     ┌────────────┐
│                    │     │            │
│      Garden        │─────│  Courtyard │
│                    │     │            │
│        S           │     │     M      │
│                    │     │            │
└─────────┬──────────┘     └────────────┘
          │
       [Gate]

S = Stone (reveals key when moved)
M = Moonlit mosaic beyond the gate
```

## Story beats (max ~10 steps)

1. You begin in the garden beside the ornate gate.
2. The gate is locked and the courtyard lies just beyond.
3. Move the heavy stone to trigger the pickup event.
4. The revealed key glints and falls into reach.
5. Unlock and open the gate.
6. Go north into the courtyard while the room description refreshes automatically.

## Slice 1‑6 functions tested

- `GameState(startLocation, worldLocations)`
- `GameState.Events.Subscribe(...)`
- `GameEventType` / `GameEvent`
- `Location.AddExit(direction, target, door)`
- `Door(id, name, description, initialState)`
- `Door.RequiresKey(key)`
- `Door.SetReaction(action, text)`
- `Item(id, name, description)`
- `Item.SetReaction(action, text)`
- `Item.Move()`
- `Key(id, name, description)`
- `ICommand`, `CommandResult`
- `MoveCommand`, `GoCommand`
- `CommandExtensions.Execute(state, command)`
- `KeywordParser`, `KeywordParserConfigBuilder`
- `KeywordParserConfigBuilder.WithDirectionAliases(...)`
- `Direction` enum

## Demo commands (parser)

- `look` / `l`
- `examine <feature>` / `x <feature>`
- `move stone` / `push stone`
- `take key`
- `unlock gate` / `open lock`
- `open gate`
- `go north` / `north` / `n`
- `go south` / `south` / `s`
- `inventory` / `i`
- `quit` / `exit`

## Example (events, doors, auto-look)
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

var garden = new Location("garden", "A quiet garden with a weathered gate and patient silence.");
var courtyard = new Location("courtyard", "A sheltered courtyard beyond the gate, lit by moonlight.");

var stone = new Item("stone", "stone", "A heavy flat stone with moss on one edge.")
    .AddAliases("slab", "stoned")
    .SetReaction(ItemAction.Take, "Duuuuude! That is so groovy.")
    .SetReaction(ItemAction.Move, "The stone scrapes across the soil and lifts tipsily.");
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
    .WithTake("take", "grab", "get")
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
```
