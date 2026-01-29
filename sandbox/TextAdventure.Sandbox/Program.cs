using System.Collections.Generic;
using System.IO;
using System.Linq;
using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Localization;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;

var languagePath = GetLanguagePath(LanguageSupport.DefaultLanguageCode);
Language.SetProvider(new FileLanguageProvider(languagePath));

var state = BuildGameState();
var parser = new KeywordParser(KeywordParserConfig.Default);

Console.WriteLine("=== BEFORE THE MEETING (Slice 11) ===");
Console.WriteLine($"{Language.GoalLabel} {Language.GoalIntro}");
Console.WriteLine($"Language file: {Path.GetFileName(languagePath)}");
Console.WriteLine(Language.LanguageHint);
ShowRoom();

while (true)
{
    Console.Write("\n> ");
    var input = Console.ReadLine()?.Trim();
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

    var command = parser.Parse(input);
    var result = state.Execute(command);

    DisplayResult(result);
    if (ShouldShowRoom(command, result))
    {
        ShowRoom();
    }

    if (result.ShouldQuit)
    {
        Console.WriteLine(Language.ThanksForPlaying);
        break;
    }
}

static GameState BuildGameState()
{
    var bedroom = new Location("bedroom", "Your alarm blinks 08:57. A coat hangs by the door.");
    var hallway = new Location("hallway", "A quiet hallway with a mirror.");

    var coffee = new Item("coffee", "coffee", "A hot cup of coffee.");
    bedroom.AddItem(coffee);

    bedroom.AddExit(Direction.East, hallway);
    hallway.AddExit(Direction.West, bedroom);

    return new GameState(bedroom, worldLocations: [bedroom, hallway]);
}

static void DisplayResult(CommandResult result)
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
    Console.WriteLine($"{Language.RoomLabel} {location.Id.ToProperCase()}");
    Console.WriteLine($"{Language.RoomDescriptionLabel} {location.GetDescription()}");

    var items = location.Items.CommaJoinNames(properCase: true);
    Console.WriteLine(string.IsNullOrWhiteSpace(items)
        ? FormatLabel(Language.ItemsHereLabel, Language.None)
        : FormatLabel(Language.ItemsHereLabel, items));

    var exits = location.Exits.Keys
        .Select(direction => direction.ToString().ToLowerInvariant().ToProperCase())
        .ToList();
    Console.WriteLine(exits.Count > 0
        ? FormatLabel(Language.ExitsLabel, exits.CommaJoin())
        : FormatLabel(Language.ExitsLabel, Language.None));
}

static string FormatLabel(string label, string value) => $"{label.TrimEnd()} {value}";

static void ShowHelp() => Console.WriteLine("Commands: look, take, go <direction>, inventory, save, load, language <code>, quit");

static bool IsHelp(string input) => input.Lower() is "help" or "halp" or "?";

bool TryHandleLanguageSwitch(string input)
{
    var tokens = input.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
    if (tokens.Length == 0 || !tokens[0].TextCompare("language"))
    {
        return false;
    }

    var supportedCodes = string.Join(", ", LanguageSupport.SupportedLanguages.Keys.Select(code => code.ToUpperInvariant()));
    if (tokens.Length == 1)
    {
        Console.WriteLine($"Usage: language <code> (supported: {supportedCodes}).");
        return true;
    }

    var chosen = tokens[1].ToLowerInvariant();
    if (!LanguageSupport.SupportedLanguages.TryGetValue(chosen, out var info))
    {
        Console.WriteLine($"Unsupported language code '{chosen}'. Supported: {supportedCodes}.");
        return true;
    }

    var path = GetLanguagePath(chosen);
    if (!File.Exists(path))
    {
        Console.WriteLine($"Language file '{info.File}' is missing.");
        return true;
    }

    Language.SetProvider(new FileLanguageProvider(path));
    Console.WriteLine(Language.LanguageLoaded(info.DisplayName, chosen.ToUpperInvariant()));
    ShowRoom();
    return true;
}

static string GetLanguagePath(string code) =>
    Path.Combine(AppContext.BaseDirectory, "lang", LanguageSupport.SupportedLanguages.TryGetValue(code, out var info) ? info.File : LanguageSupport.SupportedLanguages[LanguageSupport.DefaultLanguageCode].File);

static bool ShouldShowRoom(ICommand command, CommandResult result) =>
    result.Success && !result.ShouldQuit && (command is GoCommand or MoveCommand or LoadCommand);

static class LanguageSupport
{
    public const string DefaultLanguageCode = "en";
    public static readonly IReadOnlyDictionary<string, (string File, string DisplayName)> SupportedLanguages =
        new Dictionary<string, (string File, string DisplayName)>(StringComparer.OrdinalIgnoreCase)
        {
            ["en"] = ("gamelang.en.txt", "English"),
            ["sv"] = ("gamelang.sv.txt", "Swedish")
        };
}
