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

// Slice 7: Rain upon the Roof
// Tests: MoveCommand + bucket state, AttackCommand, FleeCommand, NPC states, auto-look after go.

var attic = new Location(
    "attic",
    "Rain caresses the roof with silver fingers. A steady leak taps scandalously intimate rhythms on the floorboards.");

var rain = new Item("rain", "rain", "Cold drops sting the eyes; you can hardly see.")
    .SetTakeable(false)
    .HideFromItemList();

var bucket = new Item(
        "bucket",
        "bucket",
        "A dented metal bucket, too heavy to lift but perfectly suited to slide beneath the leak.")
    .SetTakeable(false)
    .SetReaction(ItemAction.TakeFailed, "It is far too substantial to be carried, darling, but you could certainly persuade it to slide into place.")
    .SetReaction(ItemAction.Move, "You draw the bucket beneath the leak with deliberate grace. The dripping finally finds a worthy accomplice.");

attic.AddItem(rain);
attic.AddItem(bucket);

var storm = new Npc("storm", "storm", NpcState.Hostile, stats: new Stats(18))
    .Description("A relentless downpour that demands endurance rather than defiance.");
var dummy = new Npc("dummy", "training dummy", NpcState.Neutral, stats: new Stats(12))
    .Description("A crash-test dummy slumped in the corner, patient and uncomplaining.");
var brokenDummy = new Item("broken_dummy", "broken dummy", "You exhibited excellent form. The dummy cannot be taken.")
    .AddAliases("dummy", "training dummy")
    .SetTakeable(false);

attic.AddNpc(storm);
attic.AddNpc(dummy);

var state = new GameState(attic, worldLocations: new[] { attic })
{
    EnableFuzzyMatching = true,
    FuzzyMaxDistance = 1
};

var bucketPlaced = false;
var brokenDummyPlaced = false;

bucket.OnMove += _ =>
{
    if (bucketPlaced)
    {
        bucket.SetReaction(ItemAction.Move, "The bucket is already stationed beneath the leak, performing its quiet, devoted duty.");
        return;
    }

    bucketPlaced = true;
    storm.Stats.Damage(storm.Stats.Health);
    storm.SetState(NpcState.Dead);
};

var parser = new KeywordParser(KeywordParserConfigBuilder.BritishDefaults()
    .WithLook("look", "l")
    .WithExamine("examine", "exam", "x")
    .WithMove("move", "push", "slide")
    .WithAttack("attack", "fight", "strike")
    .WithFlee("flee", "run")
    .WithInventory("inventory", "inv", "i")
    .WithGo("go", "travel")
    .WithUse("use")
    .WithFuzzyMatching(true, 1)
    .WithIgnoreItemTokens("on", "off", "at", "the")
    .WithDirectionAliases(new Dictionary<string, Direction>(StringComparer.OrdinalIgnoreCase)
    {
        ["south"] = Direction.South,
        ["s"] = Direction.South
    })
    .Build());

Console.WriteLine("=== RAIN UPON THE ROOF (Slice 7) ===");
Console.WriteLine("Goal: shepherd the bucket beneath the leak, calm the storm, tease the dummy, and keep listening to the patter.");
Console.WriteLine("Commands: look, examine, move bucket, attack dummy, flee, inventory, go south, quit.");
ShowRoom();

var random = new Random();

while (true)
{
    MaybeWhisper(dummy, random);

    Console.Write("\n> ");
    var input = Console.ReadLine()?.Trim();
    if (string.IsNullOrWhiteSpace(input))
    {
        continue;
    }

    if (HandlePlayfulInput(input))
    {
        continue;
    }

    var command = parser.Parse(input);
    var result = state.Execute(command);

    switch (command)
    {
        case LookCommand:
            ShowLookResult(result);
            break;
        default:
            WriteResult(result);
            break;
    }

    if (command is GoCommand && result.Success && !result.ShouldQuit)
    {
        ShowRoom();
    }

    if (!dummy.IsAlive && !brokenDummyPlaced)
    {
        attic.RemoveNpc(dummy);
        attic.AddItem(brokenDummy);
        brokenDummyPlaced = true;
    }

    if (!storm.IsAlive)
    {
        Console.WriteLine("The bucket receives the final, obedient drops. The storm, at last, withdraws.");
        break;
    }

    if (result.ShouldQuit)
    {
        break;
    }
}

void MaybeWhisper(INpc npc, Random random)
{
    if (!npc.IsAlive)
    {
        return;
    }

    if (random.Next(100) <= 70)
    {
        return;
    }

    var whispers = new[]
    {
        "For a fleeting instant, you are sure the dummy is watching you.",
        "A faint creak comes from the corner, though you cannot locate its source.",
        "You swear the dummy tipped its head.",
        "The attic groans as though the dummy stretches.",
    };

    Console.WriteLine(whispers[random.Next(whispers.Length)]);
}

bool HandlePlayfulInput(string input)
{
    var lower = input.Lower();

    if (lower is ("help" or "halp" or "?"))
    {
        Console.WriteLine("Commands: look, examine, move bucket, attack dummy, flee, inventory, go south, quit");
        return true;
    }

    if (lower is ("kick bucket" or "kick the bucket"))
    {
        Console.WriteLine("You promptly expire, in the purely figurative and strictly humorous manner.");
        return true;
    }

    if (lower is "kiss dummy" or (lower is "kiss the dummy"))
    {
        Console.WriteLine("Eeew! No!");
        return true;
    }

    if (lower is "hug dummy" or (lower is "hug the dummy"))
    {
        Console.WriteLine("The dummy pushes you away and shakes its head, muttering 'Eeew no, I am a married spooky dummy.'");
        return true;
    }

    if (lower.StartsWith("listen") && (lower.Contains("rain") || lower.Contains("rhythm")))
    {
        Console.WriteLine("The rhythm soothes you.");
        return true;
    }

    return false;
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

void ShowRoom()
{
    var location = state.CurrentLocation;
    Console.WriteLine();
    Console.WriteLine($"Room: {location.Id.ToProperCase()}");
    Console.WriteLine(location.GetDescription());

    var items = location.Items.CommaJoinNames(properCase: true);
    Console.WriteLine(string.IsNullOrWhiteSpace(items) ? "Items here: None" : $"Items here: {items}");

    var people = location.Npcs.Count > 0
        ? $"People present: {location.Npcs.Count} ({location.Npcs.Select(npc => npc.Name).CommaJoin()})"
        : "People present: None";
    Console.WriteLine(people);

    var exits = location.Exits
        .Select(exit => exit.Key.ToString().ToLowerInvariant().ToProperCase())
        .ToList();
    Console.WriteLine(exits.Count > 0 ? $"Exits: {exits.CommaJoin()}" : "Exits: None");
}

void ShowLookResult(CommandResult result)
{
    Console.WriteLine($"Room: {state.CurrentLocation.Id.ToProperCase()}");
    WriteResult(result);
}
