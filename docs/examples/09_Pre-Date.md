# Pre-Date

_Slice tag: Slice 9 — World State System. Demo focuses on flags and relationships driving outcomes._

A tiny, choice-driven scene about preparing for a date. You pick trousers, shirt, scent, beard, and hair, then face the mirror’s verdict.

## Goal

Pick your look, confirm it in the mirror, then decide whether to go.

## Map (rough layout)

```
┌────────────┐     ┌────────────┐
│  Bedroom   │─────│ Mirror Room│
│     C      │     │     M      │
└────────────┘     └────────────┘

C = Closet / choices
M = Mirror
```

## Story beats (max ~10 steps)

1. You start in the bedroom.
2. Choose trousers, shirt, blazer, perfume, beard, and hair.
3. Walk to the mirror room and check yourself.
4. If your choices match the target style, the game ends.
5. You can also stay home.

## Slice 1‑9 functions tested

- `WorldState.SetFlag`, `WorldState.GetFlag`
- `GameState.Events.Subscribe(...)` for `PickupItem` and `EnterLocation`
- `ItemAction` reactions (take/use/move)
- `KeywordParserConfigBuilder` and `KeywordParser`
- `CommandExtensions.Execute(state, command)`
- `GoCommand`, `UseCommand`, `ExamineCommand`

## Demo commands (parser)

- `look` / `l`
- `examine <item>` / `x <item>` / `check <item>`
- `take <item>` / `wear <item>` / `put on <item>`
- `use <item>`
- `go east` / `go west`
- `inventory` / `i`
- `go on date` / `stay home`
- `help`

## Example (world state flags)
```csharp
using System;
using System.Linq;
using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;

var bedroom = new Location("bedroom", "Your room is a mess, but tonight demands a certain polish.");
var mirrorRoom = new Location("mirror", "A full-length mirror stands here, prepared to deliver a verdict.");

bedroom.AddExit(Direction.East, mirrorRoom);
mirrorRoom.AddExit(Direction.West, bedroom);

Item CreateChoiceItem(string id, string name, string description, string takeText, string useText, string moveText, params string[] aliases)
{
    var item = new Item(id, name, description)
        .SetReaction(ItemAction.Take, takeText)
        .SetReaction(ItemAction.Use, useText)
        .SetReaction(ItemAction.Move, moveText);

    if (aliases.Length > 0)
    {
        item.AddAliases(aliases);
    }

    return item;
}

var jeans = CreateChoiceItem(
    "jeans",
    "jeans",
    "Dark jeans, the reliable workhorse of self-respect.",
    "You take the jeans. They seem quietly pleased.",
    "You hold them up. A casual confidence settles in.",
    "You fold the jeans with unnecessary ceremony.");

var chinos = CreateChoiceItem(
    "chinos",
    "chinos",
    "Neat chinos with just enough ambition.",
    "You take the chinos. They practically preen.",
    "You check the chinos in the light. They approve.",
    "You smooth the chinos. They repay you with crispness.");

var tshirt = CreateChoiceItem(
    "tshirt",
    "t-shirt",
    "Soft black t-shirt with effortless indifference.",
    "You take the t-shirt. It promises low maintenance.",
    "You hold the t-shirt to your chest. It fits the mood.",
    "You shake out the t-shirt. It refuses to wrinkle.",
    "tee", "shirt");

var dressShirt = CreateChoiceItem(
    "shirt",
    "dress shirt",
    "Crisp white shirt, all proper buttons and expectations.",
    "You take the dress shirt. It demands decent posture.",
    "You inspect the cuffs. They look judgemental.",
    "You smooth the shirt. It grudgingly approves.",
    "button-up", "formal shirt");

var blazer = CreateChoiceItem(
    "blazer",
    "blazer",
    "Sharp blazer with a hint of theatre.",
    "You take the blazer. Confidence levels rise modestly.",
    "You try the blazer on. The mirror will have opinions.",
    "You hang the blazer over your arm like you mean it.");

var perfumeSmoky = CreateChoiceItem(
    "perfume_smoky",
    "smoky perfume",
    "Deep, smoky scent with a whisper of bad decisions.",
    "You take the smoky perfume. Dramatic energy ensues.",
    "You spritz once. The room pauses to consider you.",
    "You gently tilt the bottle. The scent lingers.",
    "smoky", "perfume");

var perfumeFresh = CreateChoiceItem(
    "perfume_fresh",
    "fresh perfume",
    "Light, fresh scent that politely forgives your flaws.",
    "You take the fresh perfume. It smells like optimism.",
    "You spritz once. You smell like a good decision.",
    "You tilt the bottle. It glows with restraint.",
    "fresh", "perfume");

var razor = CreateChoiceItem(
    "razor",
    "razor",
    "A razor with the quiet authority of routine.",
    "You take the razor. It threatens smooth consequences.",
    "You consider shaving. The razor seems eager.",
    "You turn the razor over, feeling its weight.");

var hairShort = new Item("hair_short", "short hair", "Short and tidy, the reliable civil servant of hairstyles.")
    .SetTakeable(false)
    .SetReaction(ItemAction.Use, "You smooth your hair into a neat, short style. The mirror approves.")
    .SetReaction(ItemAction.MoveFailed, "Hair is not a movable object, no matter how you petition it.")
    .SetReaction(ItemAction.TakeFailed, "You cannot take hair; it is already yours by default.");

var hairLong = new Item("hair_long", "long hair", "Long and flowing, as though it remembers poetry.")
    .SetTakeable(false)
    .SetReaction(ItemAction.Use, "You let your hair fall long and dramatic. It immediately gains an air of legend.")
    .SetReaction(ItemAction.MoveFailed, "Hair is not a movable object, no matter how you petition it.")
    .SetReaction(ItemAction.TakeFailed, "You cannot take hair; it is already yours by default.");

var mirror = new Item("mirror", "mirror", "A tall mirror with a faintly smug reflection.")
    .AddAliases("glass")
    .SetTakeable(false)
    .SetReaction(ItemAction.Use, "You take a long look. The mirror does not blink.")
    .SetReaction(ItemAction.MoveFailed, "You tilt the mirror. It tilts back, unimpressed.")
    .SetReaction(ItemAction.TakeFailed, "You try to lift the mirror. The mirror declines.");

bedroom.AddItem(jeans);
bedroom.AddItem(chinos);
bedroom.AddItem(tshirt);
bedroom.AddItem(dressShirt);
bedroom.AddItem(blazer);
bedroom.AddItem(perfumeSmoky);
bedroom.AddItem(perfumeFresh);
bedroom.AddItem(razor);
bedroom.AddItem(hairShort);
bedroom.AddItem(hairLong);
mirrorRoom.AddItem(mirror);

var state = new GameState(bedroom, worldLocations: new[] { bedroom, mirrorRoom })
{
    EnableFuzzyMatching = true,
    FuzzyMaxDistance = 1
};

state.Events.Subscribe(GameEventType.PickupItem, e =>
{
    if (e.Item == null) return;

    switch (e.Item.Id)
    {
        case "jeans": state.WorldState.SetFlag("pants_jeans", true); break;
        case "chinos": state.WorldState.SetFlag("pants_chinos", true); break;
        case "tshirt": state.WorldState.SetFlag("shirt_tshirt", true); break;
        case "shirt": state.WorldState.SetFlag("shirt_dress", true); break;
        case "blazer": state.WorldState.SetFlag("wear_blazer", true); break;
        case "perfume_smoky": state.WorldState.SetFlag("perfume_smoky", true); break;
        case "perfume_fresh": state.WorldState.SetFlag("perfume_fresh", true); break;
    }
});

state.Events.Subscribe(GameEventType.EnterLocation, e =>
{
    if (e.Location?.Id.Is("mirror") != true) return;
    TryFinishIfReady();
});

var parser = new KeywordParser(KeywordParserConfigBuilder.BritishDefaults()
    .WithLook("look", "l")
    .WithExamine("examine", "exam", "x", "check", "inspect")
    .WithInventory("inventory", "inv", "i")
    .WithTake("take", "get", "wear", "put on", "don")
    .WithDrop("drop")
    .WithUse("use")
    .WithMove("move", "push", "shift", "slide")
    .WithGo("go")
    .WithQuit("quit", "exit")
    .WithIgnoreItemTokens("on", "off", "at", "the", "a")
    .WithFuzzyMatching(true, 1)
    .Build());

Console.WriteLine("=== PRE-DATE (Slice 9) ===");
Console.WriteLine("Goal: prepare for the date. Choose wisely, then check the mirror.");
Console.WriteLine("Type 'stay home' to end the night early.");
Console.WriteLine("Type 'help' for a quick command list.");
ShowRoom();

var gameOver = false;

while (true)
{
    Console.Write("\n> ");
    var input = Console.ReadLine()?.Trim();
    if (string.IsNullOrWhiteSpace(input)) continue;

    if (IsHelp(input))
    {
        ShowHelp();
        continue;
    }

    var normalized = input.Lower();
    if (HandleHairChoice(normalized))
    {
        continue;
    }

    if (HandleShave(normalized))
    {
        continue;
    }

    if (normalized is "stay home" or "skip date")
    {
        Console.WriteLine("You decide to stay in. Game over.");
        break;
    }

    if (IsGoOnDate(normalized))
    {
        if (TryFinishIfReady())
        {
            break;
        }

        Console.WriteLine("You are not ready. Perhaps check the mirror and reconsider your choices.");
        continue;
    }

    var command = parser.Parse(input);
    var result = state.Execute(command);
    WriteResult(result);

    if (command is GoCommand && result.Success && !result.ShouldQuit)
    {
        ShowRoom();
    }

    if (command is UseCommand { ItemName: var target } && target.Is("razor"))
    {
        state.WorldState.SetFlag("shaved", true);
    }

    if (command is UseCommand { ItemName: var useTarget } && useTarget is "mirror" or "glass")
    {
        TryFinishIfReady();
    }

    if (command is ExamineCommand { Target: var examineTarget } && examineTarget is "mirror" or "glass")
    {
        TryFinishIfReady();
    }

    if (gameOver || result.ShouldQuit) break;
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

    var items = location.Items.Select(item => item.Name.ToProperCase()).ToList();
    Console.WriteLine(items.Count > 0 ? $"Items: {items.CommaJoin()}" : "Items: None");

    var exits = location.Exits.Keys
        .Select(direction => direction.ToString().ToLowerInvariant().ToProperCase())
        .ToList();
    Console.WriteLine(exits.Count > 0 ? $"Exits: {exits.CommaJoin()}" : "Exits: None");
}

void ShowHelp()
{
    Console.WriteLine("Commands: look, examine <item>, take/wear <item>, use <item>, move <item>, inventory, go <direction>, quit");
    Console.WriteLine("Special: stay home, skip date");
    Console.WriteLine("Style: shave, choose short hair, choose long hair");
    Console.WriteLine("Story: go on date");
}

bool IsHelp(string input)
{
    var normalized = input.Lower();
    return normalized is "help" or "halp" or "?";
}

bool IsGoOnDate(string input)
{
    return input is "go on date" or "go date" or "date" or "leave" or "go out";
}

bool HandleHairChoice(string input)
{
    if (input is "choose short hair" or "short hair" or "cut hair")
    {
        state.WorldState.SetFlag("hair_short", true);
        state.WorldState.SetFlag("hair_long", false);
        Console.WriteLine("You settle on a short, tidy style. Efficient, if a touch earnest.");
        return true;
    }

    if (input is "choose long hair" or "long hair" or "let hair down" or "grow hair")
    {
        state.WorldState.SetFlag("hair_long", true);
        state.WorldState.SetFlag("hair_short", false);
        Console.WriteLine("You let your hair go long. It immediately becomes an accessory to confidence.");
        return true;
    }

    return false;
}

bool HandleShave(string input)
{
    if (input is "shave" or "shave beard" or "shave face")
    {
        state.WorldState.SetFlag("shaved", true);
        Console.WriteLine("You shave with care. The result is smooth, competent, and slightly smug.");
        return true;
    }

    if (input is "choose not shaved" or "not shaved" or "keep beard" or "keep stubble")
    {
        state.WorldState.SetFlag("shaved", false);
        Console.WriteLine("You keep your beard. It lends you a faintly dangerous calm.");
        return true;
    }

    return false;
}

bool CanGoOnDate()
{
    return state.WorldState.GetFlag("hair_long")
        && !state.WorldState.GetFlag("shaved")
        && state.WorldState.GetFlag("pants_jeans")
        && state.WorldState.GetFlag("shirt_tshirt")
        && state.WorldState.GetFlag("wear_blazer");
}

bool TryFinishIfReady()
{
    if (CanGoOnDate())
    {
        Console.WriteLine("You look perfect. Confidence unlocked.");
        Console.WriteLine("You step out for the date, dressed impeccably. The night feels full of promise.");
        gameOver = true;
        return true;
    }

    Console.WriteLine("Something feels off... perhaps reconsider your choices.");
    return false;
}
```
