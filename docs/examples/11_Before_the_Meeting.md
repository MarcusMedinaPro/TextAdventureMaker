# Before the Meeting

_Slice tag: Slice 11 — Language Provider (file-based). Demo focuses on swapping UI text and narration without changing game logic._

## Story beats (max ~10 steps)
1) You wake up late.
2) Grab coffee.
3) Find your notes.
4) Leave for the meeting.

## Example (swap language at runtime)
```csharp
using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Helpers;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;
using static MarcusMedina.TextAdventure.Extensions.ConsoleExtensions;

GameState state = BuildGameState();
JsonLanguageProvider jsonProvider = LoadLanguage(Language.DefaultLanguageCode);
Language.SetProvider(jsonProvider);
KeywordParser parser = BuildParser(jsonProvider);

// Console setup for C64 aesthetics
Console.BackgroundColor = ConsoleColor.DarkBlue;
Console.ForegroundColor = ConsoleColor.Cyan;
Console.Title = "BEFORE THE MEETING (Slice 11) - Text Adventure Sandbox";
Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.Clear();
// End console setup

WriteLineC64("=== BEFORE THE MEETING (Slice 11) ===");
WriteLineC64($"{Language.GoalLabel} {Language.GoalIntro}");
WriteLineC64("Test: swap language at runtime and confirm room/item text updates.");
WriteLineC64($"Language file: {Path.GetFileName(Language.GetLanguageFilePath(jsonProvider.Code))}");
WriteLineC64(Language.LanguageHint);
int hour = 8;
int minutes = 30;

ShowRoom();
while (true)
{
    Tick_Tock();

    WriteLineC64();
    WritePromptC64("> ");
    string? input = Console.ReadLine();
    if (input is null)
    {
        break;
    }

    input = input.Trim();
    if (string.IsNullOrWhiteSpace(input))
    {
        continue;
    }

    if (IsHelp(input))
    {
        ShowHelp();
        continue;
    }

    if (TryHandleLocalActions(input))
    {
        continue;
    }

    if (TryHandleLanguageSwitch(input))
    {
        continue;
    }

    ICommand command = parser.Parse(input);

    if (command is LookCommand look && string.IsNullOrWhiteSpace(look.Target))
    {
        Tick_Tock_reverse();
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
        WriteLineC64(Language.ThanksForPlaying);
        break;
    }
}

void Tick_Tock()
{
    minutes += 1;
    if (minutes >= 60)
    {
        minutes = 0;
        hour = (hour + 1) % 24;
    }
}

void Tick_Tock_reverse()
{
    minutes -= 1;
    if (minutes < 0)
    {
        minutes = 59;
        hour = (hour + 23) % 24;
    }
}

static GameState BuildGameState()
{
    Location bedroom = new("bedroom");
    Location hallway = new("hallway");

    Item coffee = new Item("coffee", "coffee");
    Item coat = new Item("coat", "coat", "An old coat. Best donated to a thrift shop.");
    _ = coat
        .HideFromItemList()
        .SetTakeable(false)
        .SetReaction(ItemAction.TakeFailed, "The coat is hopelessly out of fashion.");
    Item mirror = new Item("mirror", "mirror", "You see yourself reflected.");
    _ = mirror
        .HideFromItemList()
        .SetTakeable(false)
        .SetReaction(ItemAction.TakeFailed, "It is far too large to carry.")
        .SetReaction(ItemAction.Destroy, "A terrible idea. Seven years of bad luck await. Better not do it!");
    bedroom.AddItem(coffee);
    bedroom.AddItem(coat);
    hallway.AddItem(mirror);

    _ = bedroom.AddExit(Direction.East, hallway);
    _ = hallway.AddExit(Direction.West, bedroom);

    return new GameState(bedroom, worldLocations: new[] { bedroom, hallway });
}

JsonLanguageProvider LoadLanguage(string code)
{
    string langPath = Path.Combine(AppContext.BaseDirectory, "lang", $"gamelang.{code}.json");
    JsonLanguageProvider provider = new(langPath);

    foreach (Location loc in state.Locations.OfType<Location>())
    {
        string desc = provider.GetDescription(loc.Id);
        if (!string.IsNullOrWhiteSpace(desc))
        {
            _ = loc.Description(desc);
        }

        foreach (Item item in loc.Items.OfType<Item>())
        {
            string itemDesc = provider.GetDescription(item.Id);
            if (!string.IsNullOrWhiteSpace(itemDesc))
            {
                _ = item.Description(itemDesc);
            }

            string translatedName = provider.GetName(item.Id);
            if (!string.IsNullOrWhiteSpace(translatedName) && !translatedName.Equals(item.Name, StringComparison.OrdinalIgnoreCase))
            {
                _ = item.AddAliases(translatedName);
            }
        }
    }

    foreach (Item item in state.Inventory.Items.OfType<Item>())
    {
        string itemDesc = provider.GetDescription(item.Id);
        if (!string.IsNullOrWhiteSpace(itemDesc))
        {
            _ = item.Description(itemDesc);
        }

        string translatedName = provider.GetName(item.Id);
        if (!string.IsNullOrWhiteSpace(translatedName) && !translatedName.Equals(item.Name, StringComparison.OrdinalIgnoreCase))
        {
            _ = item.AddAliases(translatedName);
        }
    }

    ApplyReaction("coat", ItemAction.TakeFailed, provider.Get("coatTakeFailed"));
    ApplyReaction("mirror", ItemAction.TakeFailed, provider.Get("mirrorTakeFailed"));
    ApplyReaction("mirror", ItemAction.Destroy, provider.Get("mirrorDestroy"));

    return provider;
}

void ApplyReaction(string itemId, ItemAction action, string reaction)
{
    if (string.IsNullOrWhiteSpace(reaction))
    {
        return;
    }

    Item? item = state.Locations
        .SelectMany(location => location.Items)
        .OfType<Item>()
        .FirstOrDefault(candidate => candidate.Id.TextCompare(itemId));
    if (item != null)
    {
        _ = item.SetReaction(action, reaction);
    }
}

static KeywordParser BuildParser(JsonLanguageProvider provider)
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
        WriteLineC64(TranslateDirectionsInMessage(result.Message));
    }

    foreach (string reaction in result.ReactionsList)
    {
        if (!string.IsNullOrWhiteSpace(reaction))
        {
            WriteLineC64($"> {reaction}");
        }
    }
}

string TranslateDirectionsInMessage(string message)
{
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
    WriteLineC64();
    WriteLineC64($"{Language.RoomLabel} {jsonProvider.GetName(location.Id)}");
    string desc = string.Format(location.GetDescription(), hour, minutes);
    WriteLineC64($"{Language.RoomDescriptionLabel} {desc}");

    List<string> items = location.GetRoomItems(showAll: false);
    string itemsLabel = Language.ItemsHereLabel.TrimEnd();
    WriteLineC64(items.Count > 0
        ? $"{itemsLabel} {items.CommaJoin()}"
        : $"{itemsLabel} {Language.None}");

    List<string> exits = location.GetRoomExits();
    string exitsLabel = Language.ExitsLabel.TrimEnd();
    WriteLineC64(exits.Count > 0
        ? $"{exitsLabel} {exits.CommaJoin()}"
        : $"{exitsLabel} {Language.None}");
}


static void ShowHelp()
{
    WriteLineC64("Commands: look, take, go <direction>, inventory, save, load, language/lang <code>, quit");
}

static bool IsHelp(string input)
{
    return input.Lower() is "help" or "halp" or "?";
}


bool TryHandleLanguageSwitch(string input)
{
    string[] tokens = input.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
    if (tokens.Length == 0 || !IsLanguageCommand(tokens[0]))
    {
        return false;
    }

    string supportedCodes = string.Join(", ", Language.SupportedLanguages.Keys.Select(code => code.ToUpperInvariant()));
    if (tokens.Length == 1)
    {
        WriteLineC64($"Usage: language <code> (supported: {supportedCodes}).");
        return true;
    }

    string chosen = tokens[1].ToLowerInvariant();
    if (!Language.SupportedLanguages.TryGetValue(chosen, out (string File, string DisplayName) info))
    {
        WriteLineC64($"Unsupported language code '{chosen}'. Supported: {supportedCodes}.");
        return true;
    }

    string path = Language.GetLanguageFilePath(chosen);
    if (!File.Exists(path))
    {
        WriteLineC64($"Language file '{info.File}' is missing.");
        return true;
    }

    jsonProvider = LoadLanguage(chosen);
    Language.SetProvider(jsonProvider);
    parser = BuildParser(jsonProvider);

    WriteLineC64(Language.Provider.Format("LanguageLoadedTemplate", jsonProvider.Name, jsonProvider.Code.ToUpperInvariant()));
    Tick_Tock_reverse();
    ShowRoom();
    return true;
}

bool TryHandleLocalActions(string input)
{
    string trimmed = input.Trim();
    if (string.IsNullOrWhiteSpace(trimmed))
    {
        return false;
    }

    string lower = trimmed.Lower();
    if (!IsDestroyCommand(lower))
    {
        return false;
    }

    string[] parts = trimmed.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
    if (parts.Length < 2)
    {
        WriteLineC64(Language.NothingToLookAt);
        return true;
    }

    string target = parts[1];
    IItem? item = state.CurrentLocation.FindItem(target) ?? state.Inventory.FindItem(target);
    if (item == null)
    {
        WriteLineC64(Language.NoSuchItemHere);
        return true;
    }

    string? reaction = item.GetReaction(ItemAction.Destroy);
    WriteLineC64(string.IsNullOrWhiteSpace(reaction)
        ? "You can't bring yourself to do that."
        : reaction);
    return true;
}

static bool IsDestroyCommand(string input)
{
    return input.StartsWith("destroy ")
        || input.StartsWith("smash ")
        || input.StartsWith("krossa ")
        || input.StartsWith("förstör ")
        || input.StartsWith("sla ")
        || input.StartsWith("slå ");
}

static bool IsLanguageCommand(string token)
{
    return token.TextCompare("language") || token.TextCompare("lang") || token.TextCompare("språk");
}

static bool ShouldShowRoom(ICommand command, CommandResult result)
{
    return result.Success && !result.ShouldQuit && command is GoCommand or MoveCommand or LoadCommand;
}
```

## Language file format (JSON)
```json
{
  "meta": { "code": "sv", "name": "Svenska" },
  "labels": {
    "roomLabel": "Rum:",
    "descriptionLabel": "Beskrivning:",
    "itemsHereLabel": "Föremål här:",
    "exitsLabel": "Utgångar:",
    "goalLabel": "Mål:"
  },
  "messages": {
    "goalIntro": "Ladda den språkleverantör du föredrar innan du går till mötet.",
    "languageHint": "Skriv \"language <code>\" för att byta språk."
  },
  "names": {
    "bedroom": "Sovrum",
    "hallway": "Korridor",
    "coffee": "Kaffe"
  },
  "descriptions": {
    "bedroom": "Ditt alarm blinkar 08:57. En kappa hänger vid dörren.",
    "hallway": "En tyst korridor med en spegel.",
    "coffee": "En het kopp kaffe."
  }
}
```

_Note: `.txt` language files are deprecated. Use `.json` going forward._
