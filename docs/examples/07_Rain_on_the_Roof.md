# Rain upon the Roof

_Slice tag: Slice 7 — Combat (Strategy). Demo focuses on a lightweight struggle using the combat system, plus Move as a non-combat action._

## Story beats (max ~10 steps)
1) Rain lashes the attic roof.
2) A bucket is nearby, too heavy to lift.
3) A training dummy waits in the corner.
4) Move the bucket to end the storm.
5) Try a few combat commands on the dummy.

## Example (combat + move)
```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;

var attic = new Location(
    "attic",
    "Rain caresses the roof with silver fingers. A persistent little leak taps a scandalously intimate rhythm upon the floorboards.");

var rain = new Item(
        "rain",
        "rain",
        "Cold raindrops strike your eyes. They sting, your vision blurs, and for a breathless moment you are left blind, lashes heavy with tears.")
    .SetTakeable(false)
    .HideFromItemList();

var roof = new Item("roof", "roof", "Moist with the night's insistence, the roof glistens as though varnished by the storm itself.")
    .SetTakeable(false)
    .HideFromItemList();

var floor = new Item("floor", "floor", "It's wet... and now your feet are too.")
    .AddAliases("floorboards", "boards")
    .SetTakeable(false)
    .HideFromItemList();

var puddle = new Item("puddle", "puddle", "It's wet.")
    .SetTakeable(false)
    .HideFromItemList();

var leak = new Item("leak", "leak", "It's wet and flowing.")
    .SetTakeable(false)
    .SetReaction(ItemAction.TakeFailed, "You take a quick run to the toilet to pee.")
    .HideFromItemList();

var rhythm = new Item("rhythm", "rhythm", "It's soothing and harmonious.")
    .AddAliases("rythm", "beat", "rain rhythm")
    .SetTakeable(false)
    .HideFromItemList();

var feet = new Item("feet", "feet", "Five toes each... amazing!")
    .AddAliases("foot", "toes", "toe")
    .SetTakeable(false)
    .HideFromItemList();

var fingers = new Item("fingers", "fingers", "5 of each! Yay!")
    .AddAliases("finger")
    .SetTakeable(false)
    .HideFromItemList();

var silverFingers = new Item("silver_fingers", "silver fingers", "Looks like the hand that Peter Pettigrew got from Voldemort.")
    .AddAliases("silver", "silver finger", "silvery fingers")
    .SetTakeable(false)
    .HideFromItemList();

var bucket = new Item(
        "bucket",
        "bucket",
        "A dented metal bucket with a certain weary dignity - too heavy to lift, but perfectly suited to be coaxed into position.")
    .SetTakeable(false)
    .SetReaction(ItemAction.TakeFailed, "It is far too substantial to be carried, darling, but you could certainly persuade it to slide into place.")
    .SetReaction(ItemAction.Move, "You draw the bucket beneath the leak with deliberate grace. At last, the dripping finds its willing accomplice.");

attic.AddItem(rain);
attic.AddItem(roof);
attic.AddItem(floor);
attic.AddItem(puddle);
attic.AddItem(leak);
attic.AddItem(rhythm);
attic.AddItem(feet);
attic.AddItem(fingers);
attic.AddItem(silverFingers);
attic.AddItem(bucket);

var storm = new Npc("storm", "storm", NpcState.Hostile, stats: new Stats(18))
    .Description("A relentless, sensual downpour that demands endurance rather than defiance.");

var dummy = new Npc("dummy", "spooky training dummy", NpcState.Neutral, stats: new Stats(12))
    .Description("A crash test dummy slumped in the corner, patient and uncomplaining.");

var brokenDummy = new Item("broken_dummy", "broken dummy", "You sure showed it who's the boss.")
    .AddAliases("dummy", "training dummy")
    .SetTakeable(false);

attic.AddNpc(storm);
attic.AddNpc(dummy);

var state = new GameState(attic, worldLocations: new[] { attic });
state.EnableFuzzyMatching = true;
state.FuzzyMaxDistance = 1;

var bucketPlaced = false;
var brokenDummyPlaced = false;

bucket.OnMove += _ =>
{
    if (!bucketPlaced)
    {
        bucketPlaced = true;
        storm.Stats.Damage(storm.Stats.Health);
        storm.SetState(NpcState.Dead);
        return;
    }

    bucket.SetReaction(ItemAction.Move, "The bucket is already stationed beneath the leak, performing its quiet, devoted duty.");
};

var parserConfig = KeywordParserConfigBuilder.BritishDefaults()
    .WithLook("look", "l")
    .WithExamine("examine", "exam", "x")
    .WithMove("move", "push", "shift", "lift", "slide")
    .WithInventory("inventory", "inv", "i")
    .WithTake("take", "get")
    .WithDrop("drop")
    .WithUse("use")
    .WithAttack("attack", "fight", "strike", "kick", "hit")
    .WithFlee("flee", "run")
    .WithGo("go", "move")
    .WithFuzzyMatching(true, 1)
    .WithIgnoreItemTokens("on", "off", "at", "the", "a")
    .Build();

var parser = new KeywordParser(parserConfig);

Console.WriteLine("=== RAIN UPON THE ROOF (Slice 7) ===");
Console.WriteLine("Type 'help' if you require a gentlemanly reminder of your options.");
ShowLookResult(state.Look());

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
    Console.WriteLine();
    Console.WriteLine(new string('-', 60));
    Console.WriteLine($"You are in the {state.CurrentLocation.Id.ToProperCase()}");
    Console.WriteLine();

    var lines = result.Message?
        .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
        .ToList()
        ?? new List<string>();

    if (lines.Count > 0 && lines[0].StartsWith("I think you mean", StringComparison.OrdinalIgnoreCase))
    {
        Console.WriteLine(lines[0]);
        Console.WriteLine();
        lines.RemoveAt(0);
    }

    var description = lines.FirstOrDefault() ?? state.CurrentLocation.GetDescription();
    if (!string.IsNullOrWhiteSpace(description))
    {
        Console.WriteLine(description);
        Console.WriteLine();
    }

    if (dummy.IsAlive)
    {
        Console.WriteLine("The training dummy is here, patient and uncomplaining.");
        Console.WriteLine();
    }

    var itemsLine = lines.FirstOrDefault(line => line.StartsWith("Items here:", StringComparison.OrdinalIgnoreCase));
    if (!string.IsNullOrWhiteSpace(itemsLine))
    {
        var items = itemsLine.Replace("Items here:", "").Trim();
        Console.WriteLine(items.Length > 0 ? $"You notice {items}" : "You notice nothing of particular allure.");
        Console.WriteLine();
    }

    var exitsLine = lines.FirstOrDefault(line => line.StartsWith("Exits:", StringComparison.OrdinalIgnoreCase));
    if (!string.IsNullOrWhiteSpace(exitsLine))
    {
        Console.WriteLine(exitsLine.Replace("Exits:", "Exits:"));
        Console.WriteLine();
    }

    Console.WriteLine("Hints");
    Console.WriteLine("- move bucket");
    Console.WriteLine("- attack dummy / flee");
    Console.WriteLine("- look / examine bucket");
    Console.WriteLine("- inventory");
    Console.WriteLine(new string('-', 60));
}

var rnd = new Random();

while (true)
{
    if (dummy.IsAlive)
    {
        var dummyRandom = rnd.Next(100);

        if (dummyRandom > 70)
        {
            switch (rnd.Next(4))
            {
                case 0:
                    Console.WriteLine("For a fleeting instant, you are quite certain the dummy is watching you.");
                    break;
                case 1:
                    Console.WriteLine("You could swear the dummy has shifted its weight, though you cannot say how or when.");
                    break;
                case 2:
                    Console.WriteLine("The light catches the dummy's glassy eye, and it seems to glimmer with a dreadful awareness.");
                    break;
                case 3:
                    Console.WriteLine("A faint creak comes from the corner, as though cloth and porcelain have subtly rearranged themselves.");
                    break;
            }
        }
    }

    Console.Write("> ");
    var input = Console.ReadLine()?.Trim();
    if (string.IsNullOrWhiteSpace(input)) continue;

    if (input.Equals("help", StringComparison.OrdinalIgnoreCase) ||
        input.Equals("halp", StringComparison.OrdinalIgnoreCase) ||
        input == "?")
    {
        Console.WriteLine("Commands: look, examine, move <item>, take <item>, attack <npc>, flee, inventory, quit");
        continue;
    }

    if (input.Equals("kick the bucket", StringComparison.OrdinalIgnoreCase) ||
        input.Equals("kick bucket", StringComparison.OrdinalIgnoreCase))
    {
        Console.WriteLine("You promptly expire, in the purely figurative and linguistically traditional sense.");
        continue;
    }

    if (input.Equals("kill dummy", StringComparison.OrdinalIgnoreCase) ||
        input.Equals("kill the dummy", StringComparison.OrdinalIgnoreCase))
    {
        Console.WriteLine("You try to strangle the dummy but realise it doesn't breathe.");
        continue;
    }

    if (input.Equals("kiss dummy", StringComparison.OrdinalIgnoreCase) ||
        input.Equals("kiss the dummy", StringComparison.OrdinalIgnoreCase))
    {
        Console.WriteLine("Eeew! No!");
        continue;
    }

    if (input.Equals("hug dummy", StringComparison.OrdinalIgnoreCase) ||
        input.Equals("hug the dummy", StringComparison.OrdinalIgnoreCase))
    {
        Console.WriteLine("The dummy pushes you away and shakes its head.\nA dark voice in the distance says, \"Eeew no! I am a married spooky dummy.\"");
        continue;
    }

    if (input.Equals("take a leak", StringComparison.OrdinalIgnoreCase) ||
        input.Equals("take leak", StringComparison.OrdinalIgnoreCase))
    {
        Console.WriteLine("You take a quick run to the toilet to pee.");
        continue;
    }

    if (input.StartsWith("listen", StringComparison.OrdinalIgnoreCase) &&
        (input.Contains("rain", StringComparison.OrdinalIgnoreCase) ||
         input.Contains("rhythm", StringComparison.OrdinalIgnoreCase) ||
         input.Contains("rythm", StringComparison.OrdinalIgnoreCase)))
    {
        Console.WriteLine("It's soothing and harmonious.");
        continue;
    }

    var command = parser.Parse(input);
    var result = state.Execute(command);

    if (command is LookCommand look && !string.IsNullOrWhiteSpace(look.Target))
    {
        WriteResult(result);
    }
    else if (command is LookCommand)
    {
        ShowLookResult(result);
    }
    else
    {
        WriteResult(result);
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

    if (result.ShouldQuit) break;
}
```
