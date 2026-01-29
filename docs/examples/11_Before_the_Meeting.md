# Before the Meeting

_Slice tag: Slice 11 — Language Provider (file-based). Demo focuses on swapping UI text and narration without changing game logic._

## Story beats (max ~10 steps)
1) You wake up late.
2) Grab coffee.
3) Find your notes.
4) Leave for the meeting.

## Example (swap language at runtime)
```csharp
using System;
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

var localizedDescriptions = new List<(Action<string> Apply, string Key)>();

var state = BuildGameState(localizedDescriptions);

var languagePath = Language.GetLanguageFilePath(Language.DefaultLanguageCode);
Language.SetProvider(new FileLanguageProvider(languagePath));
RefreshLocalization(localizedDescriptions);

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

    if (TryHandleLanguageSwitch(input, localizedDescriptions))
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

static GameState BuildGameState(List<(Action<string> Apply, string Key)> localizedDescriptions)
{
    var bedroom = new Location("bedroom");
    var hallway = new Location("hallway");

    localizedDescriptions.Add((text => bedroom.Description(text), "BedroomDescription"));
    localizedDescriptions.Add((text => hallway.Description(text), "HallwayDescription"));

    var coffee = new Item("coffee", "coffee");
    localizedDescriptions.Add((text => coffee.Description(text), "CoffeeDescription"));

    bedroom.AddItem(coffee);

    bedroom.AddExit(Direction.East, hallway);
    hallway.AddExit(Direction.West, bedroom);

    return new GameState(bedroom, worldLocations: new[] { bedroom, hallway });
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

bool TryHandleLanguageSwitch(string input, List<(Action<string> Apply, string Key)> localizedDescriptions)
{
    var tokens = input.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
    if (tokens.Length == 0 || !tokens[0].TextCompare("language"))
    {
        return false;
    }

    var supportedCodes = string.Join(", ", Language.SupportedLanguages.Keys.Select(code => code.ToUpperInvariant()));
    if (tokens.Length == 1)
    {
        Console.WriteLine($"Usage: language <code> (supported: {supportedCodes}).");
        return true;
    }

    var chosen = tokens[1].ToLowerInvariant();
    if (!Language.SupportedLanguages.TryGetValue(chosen, out var info))
    {
        Console.WriteLine($"Unsupported language code '{chosen}'. Supported: {supportedCodes}.");
        return true;
    }

    var path = Language.GetLanguageFilePath(chosen);
    if (!File.Exists(path))
    {
        Console.WriteLine($"Language file '{info.File}' is missing.");
        return true;
    }

    Language.SetProvider(new FileLanguageProvider(path));
    RefreshLocalization(localizedDescriptions);
    Console.WriteLine(Language.LanguageLoaded(info.DisplayName, chosen.ToUpperInvariant()));
    ShowRoom();
    return true;
}

static bool ShouldShowRoom(ICommand command, CommandResult result) =>
    result.Success && !result.ShouldQuit && (command is GoCommand or MoveCommand or LoadCommand);

static void RefreshLocalization(List<(Action<string> Apply, string Key)> localizedDescriptions)
{
    foreach (var (apply, key) in localizedDescriptions)
    {
        apply(Language.Provider.Get(key));
    }
}

```

## Language file format (key=value)
```text
# English file highlights (same keys exist for Swedish)
BedroomDescription=Your alarm blinks 08:57. A coat hangs by the door.
HallwayDescription=A quiet hallway with a mirror.
CoffeeDescription=A hot cup of coffee.
GoalLabel=Goal:
GoalIntro=Load whichever language provider you prefer before heading to the meeting.
LanguageHint=Type "language <code>" to switch languages.
RoomLabel=Room:
RoomDescriptionLabel=Description:

# Swedish file equivalents
BedroomDescription=Sovrummet doftar av kall luft och möblerad disciplin.
HallwayDescription=En stilla hall med ett spegelliknande tyst sinne.
CoffeeDescription=En rykande kopp kaffe som väntar på din uppmärksamhet.
GoalLabel=Mål:
GoalIntro=Ladda det språk du föredrar innan du går till mötet.
LanguageHint=Skriv "language <kod>" för att byta språk.
RoomLabel=Rum:
RoomDescriptionLabel=Beskrivning:
```
