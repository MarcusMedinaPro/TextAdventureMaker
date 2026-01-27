# Predator ("Pre‑Date‑Or")

_Slice tag: Slice 9 — World State System. Demo focuses on flags/relationships driving the outcome._

A tiny, choice-driven demo about getting ready for a date. The player chooses pants, shirt, perfume, shaving, and hair length. The winning combo is:
**long hair + beard + jeans + t‑shirt + blazer**.

## Story beats (max ~10 steps)
1) Wake up in your room. Big date tonight (or skip it).
2) Pick pants (jeans vs chinos).
3) Pick shirt (t‑shirt vs dress shirt).
4) Grab a blazer.
5) Choose perfume (smoky vs fresh).
6) Choose beard (shave vs keep).
7) Choose hair (short vs long).
8) Check the mirror.
9) If the winning combo is set, confidence boost.
10) Decide: go on the date or skip it (skip ends the game).

## Example (core engine + simple worldstate flags)
```csharp
using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Helpers;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;

// Locations
var bedroom = new Location("bedroom", "Your room is a mess, but tonight matters.");
var mirrorRoom = new Location("mirror", "A full-length mirror stands here.");

bedroom.AddExit(Direction.East, mirrorRoom);
mirrorRoom.AddExit(Direction.West, bedroom);

// Items (choices)
var jeans = new Item("jeans", "jeans", "Dark jeans.");
var chinos = new Item("chinos", "chinos", "Neat chinos.");
var tshirt = new Item("tshirt", "t-shirt", "Soft black t‑shirt.");
var dressShirt = new Item("shirt", "dress shirt", "Crisp white shirt.");
var blazer = new Item("blazer", "blazer", "Sharp blazer.");
var perfumeSmoky = new Item("perfume_smoky", "smoky perfume", "Deep, smoky scent.");
var perfumeFresh = new Item("perfume_fresh", "fresh perfume", "Light, fresh scent.");
var razor = new Item("razor", "razor", "For shaving.");
var hairShort = new Item("hair_short", "short hair", "Short and tidy.");
var hairLong = new Item("hair_long", "long hair", "Long and flowing.");

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

// Game state
var state = new GameState(bedroom, worldLocations: new[] { bedroom, mirrorRoom });

// WorldState hooks (choices)
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
        case "razor": state.WorldState.SetFlag("shaved", true); break;
        case "hair_short": state.WorldState.SetFlag("hair_short", true); break;
        case "hair_long": state.WorldState.SetFlag("hair_long", true); break;
    }
});

// Mirror check
state.Events.Subscribe(GameEventType.EnterLocation, e =>
{
    if (e.Location == null || e.Location.Id != "mirror") return;

    var win = state.WorldState.GetFlag("hair_long")
        && !state.WorldState.GetFlag("shaved")
        && state.WorldState.GetFlag("pants_jeans")
        && state.WorldState.GetFlag("shirt_tshirt")
        && state.WorldState.GetFlag("wear_blazer");

    if (win)
    {
        Console.WriteLine("You look perfect. Confidence unlocked.");
    }
    else
    {
        Console.WriteLine("Something feels off... maybe rethink your choices.");
    }
});

// Optional: skip the date entirely (ends the game)
// (Handled in the loop below)

// Parser config (minimal)
var parserConfig = new KeywordParserConfig(
    quit: CommandHelper.NewCommands("quit"),
    look: CommandHelper.NewCommands("look"),
    examine: CommandHelper.NewCommands("examine"),
    inventory: CommandHelper.NewCommands("inventory"),
    stats: CommandHelper.NewCommands("stats"),
    open: CommandHelper.NewCommands("open"),
    unlock: CommandHelper.NewCommands("unlock"),
    take: CommandHelper.NewCommands("take"),
    drop: CommandHelper.NewCommands("drop"),
    use: CommandHelper.NewCommands("use"),
    combine: CommandHelper.NewCommands("combine"),
    pour: CommandHelper.NewCommands("pour"),
    move: CommandHelper.NewCommands("move", "push", "shift", "lift", "slide"),
    go: CommandHelper.NewCommands("go"),
    read: CommandHelper.NewCommands("read"),
    talk: CommandHelper.NewCommands("talk"),
    attack: CommandHelper.NewCommands("attack"),
    flee: CommandHelper.NewCommands("flee"),
    save: CommandHelper.NewCommands("save"),
    load: CommandHelper.NewCommands("load"),
    quest: CommandHelper.NewCommands("quest"),
    all: CommandHelper.NewCommands("all"),
    ignoreItemTokens: CommandHelper.NewCommands(),
    combineSeparators: CommandHelper.NewCommands(),
    pourPrepositions: CommandHelper.NewCommands(),
    directionAliases: new Dictionary<string, Direction>(StringComparer.OrdinalIgnoreCase)
    {
        ["e"] = Direction.East,
        ["w"] = Direction.West
    },
    allowDirectionEnumNames: true,
    enableFuzzyMatching: true,
    fuzzyMaxDistance: 1);

var parser = new KeywordParser(parserConfig);

// Input loop
while (true)
{
    Console.Write("\n> ");
    var input = Console.ReadLine()?.Trim();
    if (string.IsNullOrEmpty(input)) continue;

    if (input.TextCompare("skip date") || input.TextCompare("stay home"))
    {
        state.WorldState.SetFlag("skip_date", true);
        Console.WriteLine("You decide to stay in. Game over.");
        break;
    }

    var command = parser.Parse(input);
    var result = state.Execute(command);
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

    if (result.ShouldQuit) break;
}
```
