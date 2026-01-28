using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;

// Slice 3: Light in the Basement
// Tests: parser commands, door states, hidden key reveal, torch use, auto-look on move.

var hallway = new Location("hallway", "It is dark. You cannot see a thing.");
var basement = new Location("basement", "Cold air and old stone. It smells of dust.");

var key = new Key("basement_key", "basement key", "A small key with a stamped B.")
    .AddAliases("key", "basement")
    .SetWeight(0.02f);

var torch = new Item("torch", "torch", "A small torch with a stiff switch.")
    .AddAliases("flashlight", "light")
    .SetWeight(0.2f)
    .SetReaction(ItemAction.Use, "Click. A clean circle of light blooms in the dark.");

// The key is hidden until the torch is used.

var basementDoor = new Door("basement_door", "basement door", "A solid door with a heavy latch.")
    .RequiresKey(key)
    .SetReaction(DoorAction.Unlock, "The lock gives way with a dry click.")
    .SetReaction(DoorAction.Open, "The door creaks open.")
    .SetReaction(DoorAction.OpenFailed, "It will not move.");

hallway.AddExit(Direction.Down, basement, basementDoor);

var state = new GameState(hallway, worldLocations: [hallway, basement])
{
    EnableFuzzyMatching = true,
    FuzzyMaxDistance = 1
};
state.Inventory.Add(torch);

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
Console.WriteLine("Goal: find the basement key, unlock the door, and go down.");
Console.WriteLine("Commands: look, examine <item>, take <item>, move <item>, unlock/open door, go down, use/turn on/off torch, inventory, quit.");
ShowDark();

while (true)
{
    Console.Write("\n> ");
    var input = Console.ReadLine()?.Trim();
    if (string.IsNullOrWhiteSpace(input))
        continue;

    var command = parser.Parse(input);
    var lightOn = state.WorldState.GetFlag("torch_on");
    var wantsOff = WantsOff(input);
    var wantsOn = WantsOn(input);
    var isTorchCommand = IsTorchCommand(command);

    if (isTorchCommand && wantsOff)
    {
        if (lightOn)
        {
            state.WorldState.SetFlag("torch_on", false);
            Console.WriteLine("The light goes out.");
        }
        else
        {
            Console.WriteLine("It is already off.");
        }

        continue;
    }

    if (isTorchCommand && lightOn && !wantsOn)
    {
        state.WorldState.SetFlag("torch_on", false);
        Console.WriteLine("The light goes out.");
        continue;
    }

    if (command is TakeCommand && !lightOn)
    {
        ShowDark();
        continue;
    }

    var result = state.Execute(command);

    switch (command)
    {
        case LookCommand when !lightOn:
            ShowDark();
            break;

        case LookCommand:
            ShowLookResult(result);
            break;

        default:
            WriteResult(result);
            break;
    }

    if (command is GoCommand && !result.ShouldQuit)
    {
        if (state.WorldState.GetFlag("torch_on"))
        {
            ShowLookResult(state.Look());
        }
        else
        {
            ShowDark();
        }
    }

    if (isTorchCommand && !lightOn)
    {
        state.WorldState.SetFlag("torch_on", true);
        RevealKey();
    }

    if (result.ShouldQuit)
        break;
}

bool IsTorchCommand(ICommand command) => command is UseCommand { ItemName: var name }
        && state.Inventory.FindItem(name)?.Id.Is("torch") == true;

bool WantsOff(string input) => input.Contains("off", StringComparison.OrdinalIgnoreCase);
bool WantsOn(string input) => input.Contains("on", StringComparison.OrdinalIgnoreCase);

void RevealKey()
{
    if (isKeyRevealed)
        return;
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
    Console.WriteLine($"Room: {state.CurrentLocation.Id.ToProperCase()}");
    Console.WriteLine("It is dark. You cannot see a thing.");
}

void ShowLookResult(CommandResult result)
{
    Console.WriteLine($"Room: {state.CurrentLocation.Id.ToProperCase()}");
    WriteResult(result);
}