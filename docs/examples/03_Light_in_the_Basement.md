# Light in the Basement

_Slice tag: Slice 3 — Command Pattern + Parser. Demo focuses on using the built-in parser and command objects to keep the game loop small._

A tiny demo about a locked basement door and a flashlight.

## Map (rough layout)

```
          N
    W           E
          S

┌────────────┐
│  Hallway   │
│     K      │  F
└─────┬──────┘
      │
      │
┌────────────┐
│ Basement   │
│            │
└────────────┘

K = Basement key
F = Flashlight (in inventory)
```

## Story beats (max ~10 steps)

1. You stand in the hallway.
2. The basement door is locked.
3. Use the flashlight.
4. A key glints under the table.
5. Unlock and open the door.
6. Go down.

## Slice 1 + 2 + 3 functions tested

- `Location(id, description)`
- `Location.AddExit(direction, target, oneWay: false)`
- `Location.AddExit(direction, target, door, oneWay: false)`
- `Location.Exits`
- `GameState(startLocation, worldLocations)`
- `GameState.CurrentLocation`
- `GameState.Inventory`
- `Door(id, name, description, initialState)`
- `Door.RequiresKey(key)`
- `Door.SetReaction(action, text)`
- `DoorAction` enum
- `DoorState` enum
- `Key(id, name, description)`
- `Item(id, name, description)`
- `Item.SetReaction(action, text)`
- `ItemAction` enum
- `ICommand`
- `CommandResult`
- `ICommandParser`
- `KeywordParser(config)`
- `KeywordParserConfig(...)`
- `CommandHelper.NewCommands(...)`
- `CommandExtensions.Execute(state, command)`
- `Direction` enum

## Demo commands (parser)

- `look` / `l`
- `examine <item>` / `x <item>`
- `take <item>`
- `move <item>`
- `unlock door`
- `open door`
- `go down` / `down` / `d`
- `use flashlight` / `turn on flashlight` / `turn off flashlight`
- `inventory` / `i`
- `quit` / `exit`

## Optional helpers

- `\"Steven\".SoundsLike(\"Stephen\")` → `true`
- `\"look\".SimilarTo(\"lokk\")` → `1`

## Example (parser + built-in commands)

```csharp
using System;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;
using MarcusMedina.TextAdventure.Commands;

var hallway = new Location("hallway", "It's dark. You can't see anything.");
var basement = new Location("basement", "Cold air and old stone. It smells of dust.");

var key = new Key("basement_key", "basement key", "A small key with a stamped B.")
    .AddAliases("key", "basement")
    .SetWeight(0.02f);

var flashlight = new Item("flashlight", "flashlight", "A small torch with a stiff switch.")
    .AddAliases("torch", "light")
    .SetWeight(0.2f)
    .SetReaction(ItemAction.Use, "Click. A clean circle of light blooms in the dark.");

// The key is hidden until the flashlight is used.

var basementDoor = new Door("basement_door", "basement door", "A solid door with a heavy latch.")
    .RequiresKey(key)
    .SetReaction(DoorAction.Unlock, "The lock gives way with a dry click.")
    .SetReaction(DoorAction.Open, "The door creaks open.")
    .SetReaction(DoorAction.OpenFailed, "It will not move.");

hallway.AddExit(Direction.Down, basement, basementDoor);

var state = new GameState(hallway, worldLocations: new[] { hallway, basement });
state.Inventory.Add(flashlight);
state.EnableFuzzyMatching = true;
state.FuzzyMaxDistance = 1;
var isKeyRevealed = false;
var parserConfig = KeywordParserConfigBuilder.BritishDefaults()
    .WithStats("stats")
    .WithFuzzyMatching(true, 1)
    .WithExamine("examine", "x")
    .WithTake("take", "get")
    .WithMove("move", "push", "shift", "lift", "slide")
    .WithUse("use", "turn", "switch", "light", "torch")
    .WithGo("go")
    .WithIgnoreItemTokens("on", "off")
    .WithDirectionAliases(new Dictionary<string, Direction>(StringComparer.OrdinalIgnoreCase)
    {
        ["d"] = Direction.Down,
        ["down"] = Direction.Down,
        ["u"] = Direction.Up,
        ["up"] = Direction.Up
    })
    .Build();
var parser = new KeywordParser(parserConfig);

Console.WriteLine("=== LIGHT IN THE BASEMENT (Slice 3) ===");
Console.WriteLine("Commands: Look, Examine <Item>, Take <Item>, Move <Item>, Unlock/Open Door, Go Down, Use/Turn On/Off Flashlight, Inventory, Quit");
Console.WriteLine("It's dark. You can't see anything.");

bool IsFlashlightCommand(ICommand command)
{
    if (command is not UseCommand use) return false;
    var item = state.Inventory.FindItem(use.ItemName);
    return item != null && item.Id.TextCompare("flashlight");
}

bool WantsOff(string input)
{
    return input.Contains("off", StringComparison.OrdinalIgnoreCase);
}

bool WantsOn(string input)
{
    return input.Contains("on", StringComparison.OrdinalIgnoreCase);
}

void RevealKey()
{
    if (isKeyRevealed) return;
    isKeyRevealed = true;
    hallway.AddItem(key);
    Console.WriteLine("> The brass key glints under the table.");
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

void ShowDark()
{
    Console.WriteLine("It's dark. You can't see anything.");
}

void ShowLookResult(CommandResult result)
{
    Console.WriteLine($"Room: {state.CurrentLocation.Id.ToProperCase()}");
    WriteResult(result);
}

while (true)
{
    Console.Write("\n> ");
    var input = Console.ReadLine()?.Trim();
    if (string.IsNullOrWhiteSpace(input)) continue;

    var command = parser.Parse(input);
    var lightOn = state.WorldState.GetFlag("flashlight_on");
    var isFlashlightCommand = IsFlashlightCommand(command);

    if (isFlashlightCommand && WantsOff(input))
    {
        if (lightOn)
        {
            state.WorldState.SetFlag("flashlight_on", false);
            Console.WriteLine("The light goes out.");
        }
        else
        {
            Console.WriteLine("It's already off.");
        }
        continue;
    }

    if (isFlashlightCommand && lightOn && !WantsOn(input))
    {
        state.WorldState.SetFlag("flashlight_on", false);
        Console.WriteLine("The light goes out.");
        continue;
    }

    if (command is TakeCommand && !lightOn)
    {
        ShowDark();
        continue;
    }

    var result = state.Execute(command);

    if (command is LookCommand && !lightOn)
    {
        ShowDark();
    }
    else
    {
        if (command is LookCommand)
        {
            ShowLookResult(result);
        }
        else WriteResult(result);
    }

    if (command is GoCommand && !result.ShouldQuit)
    {
        if (state.WorldState.GetFlag("flashlight_on"))
        {
            ShowLookResult(state.Look());
        }
        else
        {
            ShowDark();
        }
    }

    if (isFlashlightCommand && !lightOn)
    {
        state.WorldState.SetFlag("flashlight_on", true);
        RevealKey();
    }

    if (result.ShouldQuit) break;
}
```
