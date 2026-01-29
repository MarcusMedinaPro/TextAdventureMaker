using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Helpers;
using MarcusMedina.TextAdventure.Localization;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;
using TextAdventure.Sandbox;

var state = BuildGameState();
var jsonProvider = LoadLanguage("en");
Language.SetProvider(jsonProvider);
var parser = BuildParser(jsonProvider);

Console.WriteLine("=== BEFORE THE MEETING (Slice 11) ===");
Console.WriteLine($"{jsonProvider.Get("goalLabel")} {jsonProvider.Get("goalIntro")}");
Console.WriteLine($"Language: {jsonProvider.Name} ({jsonProvider.Code.ToUpperInvariant()})");
Console.WriteLine(jsonProvider.Get("languageHint"));
ShowRoom();

while (true)
{
    Console.Write("\n> ");
    var input = Console.ReadLine()?.Trim();
    if (string.IsNullOrWhiteSpace(input))
        continue;

    if (IsHelp(input))
    {
        ShowHelp();
        continue;
    }

    if (TryHandleLanguageSwitch(input))
        continue;

    var command = parser.Parse(input);

    // Handle look without target locally for proper localization
    if (command is LookCommand look && string.IsNullOrWhiteSpace(look.Target))
    {
        ShowRoom();
        continue;
    }

    var result = state.Execute(command);

    DisplayResult(result);
    if (ShouldShowRoom(command, result))
        ShowRoom();

    if (result.ShouldQuit)
    {
        Console.WriteLine(jsonProvider.Get("thanksForPlaying"));
        break;
    }
}

static GameState BuildGameState()
{
    var bedroom = new Location("bedroom");
    var hallway = new Location("hallway");

    bedroom.AddExit(Direction.East, hallway);
    hallway.AddExit(Direction.West, bedroom);

    var coffee = new Item("coffee", "coffee");
    bedroom.AddItem(coffee);

    return new GameState(bedroom, worldLocations: [bedroom, hallway]);
}

JsonLanguageProvider LoadLanguage(string code)
{
    var langPath = Path.Combine(AppContext.BaseDirectory, "lang", $"gamelang.{code}.json");
    var provider = new JsonLanguageProvider(langPath);

    // Update descriptions and aliases from language file
    foreach (var loc in state.Locations.OfType<Location>())
    {
        var desc = provider.GetDescription(loc.Id);
        if (!string.IsNullOrWhiteSpace(desc))
            loc.Description(desc);

        foreach (var item in loc.Items.OfType<Item>())
        {
            var desc2 = provider.GetDescription(item.Id);
            if (!string.IsNullOrWhiteSpace(desc2))
                item.Description(desc2);

            // Add translated name as alias so "ta kaffe" works
            var translatedName = provider.GetName(item.Id);
            if (!string.IsNullOrWhiteSpace(translatedName) && !translatedName.Equals(item.Name, StringComparison.OrdinalIgnoreCase))
                item.AddAliases(translatedName);
        }
    }

    // Also update inventory items
    foreach (var item in state.Inventory.Items.OfType<Item>())
    {
        var desc = provider.GetDescription(item.Id);
        if (!string.IsNullOrWhiteSpace(desc))
            item.Description(desc);

        var translatedName = provider.GetName(item.Id);
        if (!string.IsNullOrWhiteSpace(translatedName) && !translatedName.Equals(item.Name, StringComparison.OrdinalIgnoreCase))
            item.AddAliases(translatedName);
    }

    return provider;
}

KeywordParser BuildParser(JsonLanguageProvider provider)
{
    var config = new KeywordParserConfig(
        quit: provider.GetAllCommandAliases("quit"),
        look: provider.GetAllCommandAliases("look"),
        examine: provider.GetAllCommandAliases("examine"),
        inventory: provider.GetAllCommandAliases("inventory"),
        stats: provider.GetAllCommandAliases("stats"),
        open: provider.GetAllCommandAliases("open"),
        unlock: provider.GetAllCommandAliases("unlock"),
        take: provider.GetAllCommandAliases("take"),
        drop: provider.GetAllCommandAliases("drop"),
        use: provider.GetAllCommandAliases("use"),
        combine: provider.GetAllCommandAliases("combine"),
        pour: provider.GetAllCommandAliases("pour"),
        move: provider.GetAllCommandAliases("move"),
        go: provider.GetAllCommandAliases("go"),
        read: provider.GetAllCommandAliases("read"),
        talk: provider.GetAllCommandAliases("talk"),
        attack: provider.GetAllCommandAliases("attack"),
        flee: provider.GetAllCommandAliases("flee"),
        save: provider.GetAllCommandAliases("save"),
        load: provider.GetAllCommandAliases("load"),
        quest: provider.GetAllCommandAliases("quest"),
        all: provider.GetAllCommandAliases("all"),
        ignoreItemTokens: CommandHelper.NewCommands("up", "to", "at", "the", "a", "på", "till", "en", "ett"),
        combineSeparators: CommandHelper.NewCommands("and", "+", "och", "med"),
        pourPrepositions: CommandHelper.NewCommands("into", "in", "i"),
        directionAliases: provider.GetDirectionAliases(),
        allowDirectionEnumNames: true);

    return new KeywordParser(config);
}

void DisplayResult(CommandResult result)
{
    if (!string.IsNullOrWhiteSpace(result.Message))
    {
        var message = TranslateDirectionsInMessage(result.Message);
        Console.WriteLine(message);
    }

    foreach (var reaction in result.ReactionsList)
    {
        if (!string.IsNullOrWhiteSpace(reaction))
            Console.WriteLine($"> {reaction}");
    }
}

string TranslateDirectionsInMessage(string message)
{
    // Replace English direction names with translated ones
    foreach (Direction dir in Enum.GetValues<Direction>())
    {
        var englishName = dir.ToString();
        var translatedName = jsonProvider.GetDirectionName(dir);
        if (!englishName.Equals(translatedName, StringComparison.OrdinalIgnoreCase))
        {
            message = message.Replace(englishName, translatedName, StringComparison.OrdinalIgnoreCase);
        }
    }
    return message;
}

void ShowRoom()
{
    var location = state.CurrentLocation;
    Console.WriteLine();
    Console.WriteLine($"{jsonProvider.Get("roomLabel")} {jsonProvider.GetName(location.Id)}");
    Console.WriteLine($"{jsonProvider.Get("descriptionLabel")} {location.GetDescription()}");

    var items = location.Items
        .Select(item => jsonProvider.GetName(item.Id))
        .ToList();
    var itemsLabel = jsonProvider.Get("itemsHereLabel").TrimEnd();
    Console.WriteLine(items.Count > 0
        ? $"{itemsLabel} {items.CommaJoin()}"
        : $"{itemsLabel} {jsonProvider.Get("none")}");

    var exits = location.Exits.Keys
        .Select(dir => jsonProvider.GetDirectionName(dir))
        .ToList();
    var exitsLabel = jsonProvider.Get("exitsLabel").TrimEnd();
    Console.WriteLine(exits.Count > 0
        ? $"{exitsLabel} {exits.CommaJoin()}"
        : $"{exitsLabel} {jsonProvider.Get("none")}");
}

static void ShowHelp() => Console.WriteLine("Commands: look, take, go <direction>, inventory, save, load, language <code>, quit");

static bool IsHelp(string input) => input.Lower() is "help" or "halp" or "?" or "hjälp";

bool TryHandleLanguageSwitch(string input)
{
    var tokens = input.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
    if (tokens.Length == 0 || !tokens[0].TextCompare("language") && !tokens[0].TextCompare("språk"))
        return false;

    if (tokens.Length == 1)
    {
        Console.WriteLine("Usage: language <code> (supported: EN, SV).");
        return true;
    }

    var chosen = tokens[1].ToLowerInvariant();
    var langPath = Path.Combine(AppContext.BaseDirectory, "lang", $"gamelang.{chosen}.json");

    if (!File.Exists(langPath))
    {
        Console.WriteLine($"Language file for '{chosen}' not found.");
        return true;
    }

    jsonProvider = LoadLanguage(chosen);
    Language.SetProvider(jsonProvider);
    parser = BuildParser(jsonProvider);

    Console.WriteLine(jsonProvider.Format("languageLoaded", jsonProvider.Name, jsonProvider.Code.ToUpperInvariant()));
    ShowRoom();
    return true;
}

static bool ShouldShowRoom(ICommand command, CommandResult result) =>
    result.Success && !result.ShouldQuit && command is GoCommand or MoveCommand or LoadCommand;
