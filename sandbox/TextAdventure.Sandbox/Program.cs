using System;
using System.Linq;
using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;
using static MarcusMedina.TextAdventure.Extensions.ConsoleExtensions;

// Slice 46 — Consumable Items: Eat, Drink & Healing
// Tests:
// - Eat food to heal
// - Drink beverages to heal
// - Poisoned items apply damage over turns
// - Amount decreases on consume; removed at 0
// - Stacked consumables work with Slice 47

Location kitchen = (id: "kitchen", description: "A warm kitchen. The smell of freshly baked bread fills the air.");
Location cellar = (id: "cellar", description: "A dark cellar lined with dusty wine racks.");

kitchen.AddExit(Direction.Down, cellar);
cellar.AddExit(Direction.Up, kitchen);

// Food items
var bread = new Item("bread", "Bread", "A crusty loaf of bread.")
    .AsFood(5)
    .SetAmount(3)
    .SetStackable()
    .SetWeight(0.3f)
    .SetTakeable(true);

var apple = new Item("apple", "Apple", "A crisp red apple.")
    .AsFood(3)
    .SetTakeable(true);

// Drinkable items
var ale = new Item("ale", "Ale", "A frothy pint of ale.")
    .AsDrink(2)
    .SetTakeable(true);

var poisonedWine = new Item("wine", "Wine", "A suspiciously dark vintage.")
    .SetDrinkable()
    .SetPoisoned()
    .SetPoisonDamage(3, 4)
    .SetTakeable(true);

// Non-consumable for contrast
var knife = new Item("knife", "Kitchen Knife", "A sharp kitchen knife.")
    .SetWeight(0.5f)
    .SetTakeable(true);

kitchen.AddItem(bread);
kitchen.AddItem(apple);
kitchen.AddItem(ale);
kitchen.AddItem(knife);
cellar.AddItem(poisonedWine);

var state = new GameState(kitchen, worldLocations: [kitchen, cellar]);

var parser = new KeywordParser(KeywordParserConfigBuilder.BritishDefaults().Build());

SetupC64("Consumable Items - Text Adventure Sandbox");
WriteLineC64("=== CONSUMABLE ITEMS (Slice 46) ===");
WriteLineC64("The kitchen has bread (x3), an apple, ale, and a knife.");
WriteLineC64("The cellar (down) has a suspicious bottle of wine.");
WriteLineC64();
WriteLineC64("Try: take bread, eat bread, take ale, drink ale");
WriteLineC64("     go down, take wine, drink wine (poison!)");
WriteLineC64("     stats, eat knife (can't eat that), quit");

state.ShowRoom();

while (true)
{
    // Tick poisons at the start of each turn
    var poisonResults = state.TickPoisons();
    foreach (var (sourceName, damage) in poisonResults)
    {
        WriteLineC64(MarcusMedina.TextAdventure.Localization.Language.PoisonTick(sourceName, damage));
    }

    if (state.Stats.Health <= 0)
    {
        WriteLineC64("You collapse from the poison. Game over.");
        break;
    }

    WriteLineC64();
    WritePromptC64("> ");
    var input = Console.ReadLine();
    if (input is null)
        break;

    var trimmed = input.Trim();
    if (string.IsNullOrWhiteSpace(trimmed))
        continue;

    var command = parser.Parse(trimmed);
    var result = state.Execute(command);

    state.DisplayResult(command, result);

    if (result.ShouldQuit)
        break;
}
