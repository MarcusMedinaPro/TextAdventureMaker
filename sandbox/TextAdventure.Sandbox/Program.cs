using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Helpers;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;
using TextAdventure.Sandbox;

GameState state = BuildGameState();
JsonLanguageProvider jsonProvider = LoadLanguage("en");
Language.SetProvider(jsonProvider);
KeywordParser parser = BuildParser(jsonProvider);

Console.WriteLine("=== BEFORE THE MEETING (Slice 11) ===");
Console.WriteLine($"{jsonProvider.Get("goalLabel")} {jsonProvider.Get("goalIntro")}");
Console.WriteLine($"Language: {jsonProvider.Name} ({jsonProvider.Code.ToUpperInvariant()})");
Console.WriteLine(jsonProvider.Get("languageHint"));
ShowRoom();

while (true)
{
    Console.Write("\n> ");
    string? input = Console.ReadLine()?.Trim();
    if (string.IsNullOrWhiteSpace(input))
    {
        continue;
    }

    if (IsHelp(input))
    {
        ShowHelp();
        continue;
    }

    if (TryHandleLanguageSwitch(input))
    {
        continue;
    }

    ICommand command = parser.Parse(input);

    // Handle look without target locally for proper localization
    if (command is LookCommand look && string.IsNullOrWhiteSpace(look.Target))
    {
        ShowRoom();
        continue;
    }

    CommandResult result = state.Execute(command);

    DisplayResult(result);
    if (ShouldShowRoom(command, result))
    {
        ShowRoom();
    }

    if (result.ShouldQuit)
    {
        Console.WriteLine(jsonProvider.Get("thanksForPlaying"));
        break;
    }
}

static GameState BuildGameState()
{
    Location bedroom = new("bedroom");
    Location hallway = new("hallway");

    _ = bedroom.AddExit(Direction.East, hallway);
    _ = hallway.AddExit(Direction.West, bedroom);

    Item coffee = new("coffee", "coffee");
    bedroom.AddItem(coffee);

    return new GameState(bedroom, worldLocations: [bedroom, hallway]);
}

JsonLanguageProvider LoadLanguage(string code)
{
    string langPath = Path.Combine(AppContext.BaseDirectory, "lang", $"gamelang.{code}.json");
    JsonLanguageProvider provider = new(langPath);

    // Update descriptions and aliases from language file
    foreach (Location loc in state.Locations.OfType<Location>())
    {
        string desc = provider.GetDescription(loc.Id);
        if (!string.IsNullOrWhiteSpace(desc))
        {
            _ = loc.Description(desc);
        }

        foreach (Item item in loc.Items.OfType<Item>())
        {
            string desc2 = provider.GetDescription(item.Id);
            if (!string.IsNullOrWhiteSpace(desc2))
            {
                _ = item.Description(desc2);
            }

            // Add translated name as alias so "ta kaffe" works
            string translatedName = provider.GetName(item.Id);
            if (!string.IsNullOrWhiteSpace(translatedName) && !translatedName.Equals(item.Name, StringComparison.OrdinalIgnoreCase))
            {
                _ = item.AddAliases(translatedName);
            }
        }
    }

    // Also update inventory items
    foreach (Item item in state.Inventory.Items.OfType<Item>())
    {
        string desc = provider.GetDescription(item.Id);
        if (!string.IsNullOrWhiteSpace(desc))
        {
            _ = item.Description(desc);
        }

        string translatedName = provider.GetName(item.Id);
        if (!string.IsNullOrWhiteSpace(translatedName) && !translatedName.Equals(item.Name, StringComparison.OrdinalIgnoreCase))
        {
            _ = item.AddAliases(translatedName);
        }
    }

    return provider;
}

KeywordParser BuildParser(JsonLanguageProvider provider)
{
    KeywordParserConfig config = new(
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
        string message = TranslateDirectionsInMessage(result.Message);
        Console.WriteLine(message);
    }

    foreach (string reaction in result.ReactionsList)
    {
        if (!string.IsNullOrWhiteSpace(reaction))
        {
            Console.WriteLine($"> {reaction}");
        }
    }
}

string TranslateDirectionsInMessage(string message)
{
    // Replace English direction names with translated ones
    foreach (Direction dir in Enum.GetValues<Direction>())
    {
        string englishName = dir.ToString();
        string translatedName = jsonProvider.GetDirectionName(dir);
        if (!englishName.Equals(translatedName, StringComparison.OrdinalIgnoreCase))
        {
            message = message.Replace(englishName, translatedName, StringComparison.OrdinalIgnoreCase);
        }
    }
    return message;
}

void ShowRoom()
{
    ILocation location = state.CurrentLocation;
    Console.WriteLine();
    Console.WriteLine($"{jsonProvider.Get("roomLabel")} {jsonProvider.GetName(location.Id)}");
    Console.WriteLine($"{jsonProvider.Get("descriptionLabel")} {location.GetDescription()}");

    List<string> items = [.. location.Items.Select(item => jsonProvider.GetName(item.Id))];
    string itemsLabel = jsonProvider.Get("itemsHereLabel").TrimEnd();
    Console.WriteLine(items.Count > 0
        ? $"{itemsLabel} {items.CommaJoin()}"
        : $"{itemsLabel} {jsonProvider.Get("none")}");

    List<string> exits = [.. location.Exits.Keys.Select(dir => jsonProvider.GetDirectionName(dir))];
    string exitsLabel = jsonProvider.Get("exitsLabel").TrimEnd();
    Console.WriteLine(exits.Count > 0
        ? $"{exitsLabel} {exits.CommaJoin()}"
        : $"{exitsLabel} {jsonProvider.Get("none")}");
}

static void ShowHelp()
{
    Console.WriteLine("Commands: look, take, go <direction>, inventory, save, load, language <code>, quit");
}

static bool IsHelp(string input)
{
    return input.Lower() is "help" or "halp" or "?" or "hjälp";
}

bool TryHandleLanguageSwitch(string input)
{
    string[] tokens = input.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
    if (tokens.Length == 0 || (!tokens[0].TextCompare("language") && !tokens[0].TextCompare("språk")))
    {
        return false;
    }

    if (tokens.Length == 1)
    {
        Console.WriteLine("Usage: language <code> (supported: EN, SV).");
        return true;
    }

    string chosen = tokens[1].ToLowerInvariant();
    string langPath = Path.Combine(AppContext.BaseDirectory, "lang", $"gamelang.{chosen}.json");

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

static bool ShouldShowRoom(ICommand command, CommandResult result)
{
    return result.Success && !result.ShouldQuit && command is GoCommand or MoveCommand or LoadCommand;
}