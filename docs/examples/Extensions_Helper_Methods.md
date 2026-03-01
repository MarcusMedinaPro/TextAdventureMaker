# Extension Helper Methods Reference

Common patterns extracted into reusable extensions so sandbox examples stay clean and DRY.

---

## ConsoleExtensions.SetupC64

**File:** `Extensions/ConsoleExtensions.cs`

Sets up the console with C64 aesthetics (dark blue background, cyan text, UTF-8 encoding).

```csharp
using MarcusMedina.TextAdventure.Extensions;

// Before (repeated in every slice):
Console.BackgroundColor = ConsoleColor.DarkBlue;
Console.ForegroundColor = ConsoleColor.Cyan;
Console.Title = "My Game - Text Adventure Sandbox";
Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.Clear();

// After:
ConsoleExtensions.SetupC64("My Game");
```

**Signature:**
```csharp
public static void SetupC64(string title = "Text Adventure")
```

---

## CommandResult.WriteToConsole

**File:** `Commands/CommandResultExtensions.cs`

Writes the result message and any reactions to the console. Reactions are prefixed with `> `.

```csharp
using MarcusMedina.TextAdventure.Commands;

// Before (repeated in 16+ slices):
if (!string.IsNullOrWhiteSpace(result.Message))
    Console.WriteLine(result.Message);
foreach (var reaction in result.ReactionsList)
{
    if (!string.IsNullOrWhiteSpace(reaction))
        Console.WriteLine($"> {reaction}");
}

// After:
result.WriteToConsole();
```

**Signature:**
```csharp
public static void WriteToConsole(this CommandResult result)
```

---

## CommandResult.ShouldAutoLook

**File:** `Commands/CommandResultExtensions.cs`

Returns `true` when the command result indicates the room display should be refreshed (after go, move, or load commands).

```csharp
// Before (repeated in 8+ slices):
if (command is GoCommand && result.Success && !result.ShouldQuit)
    ShowRoom();

// After:
if (result.ShouldAutoLook(command))
    state.ShowRoom();
```

**Signature:**
```csharp
public static bool ShouldAutoLook(this CommandResult result, ICommand command)
```

Returns `true` for `GoCommand`, `MoveCommand`, and `LoadCommand` when `Success == true` and `ShouldQuit == false`.

---

## GameState.ShowRoom

**File:** `Extensions/CommandExtensions.cs`

Prints the current room: name, description, items, NPCs, and exits. Uses door-aware exit formatting.

```csharp
using MarcusMedina.TextAdventure.Extensions;

// Before (repeated in 14+ slices):
void ShowRoom()
{
    var location = state.CurrentLocation;
    Console.WriteLine();
    Console.WriteLine($"Room: {location.Id.ToProperCase()}");
    Console.WriteLine(location.GetDescription());
    var items = location.GetRoomItems();
    Console.WriteLine(items.Count > 0 ? $"Items: {items.CommaJoin()}" : "Items: None");
    var exits = location.GetRoomExits();
    Console.WriteLine(exits.Count > 0 ? $"Exits: {exits.CommaJoin()}" : "Exits: None");
}

// After:
state.ShowRoom();
```

**Signature:**
```csharp
public static void ShowRoom(this GameState state, ILanguageProvider? provider = null)
```

Output example:
```
Room: Dark Cave
A damp cave with glowing mushrooms.
Items: Brass Key, Torch
You see: Guard, Merchant
Exits: North, East (Cabin Door, Locked)
```

---

## GameState.ShowLookResult

**File:** `Extensions/CommandExtensions.cs`

Prints the room name header followed by the command result. Used for displaying look results with a title.

```csharp
// Before (repeated in 7+ slices):
Console.WriteLine($"Room: {state.CurrentLocation.Id.ToProperCase()}");
WriteResult(result);

// After:
state.ShowLookResult(result);
```

**Signature:**
```csharp
public static void ShowLookResult(this GameState state, CommandResult result)
```

---

## GameState.DisplayResult

**File:** `Extensions/CommandExtensions.cs`

Context-aware display: uses `ShowLookResult` for look commands, `WriteToConsole` for everything else, and automatically calls `ShowRoom` after go/move/load.

```csharp
// Before (repeated in 6+ slices):
switch (command)
{
    case LookCommand:
        ShowLookResult(result);
        break;
    default:
        WriteResult(result);
        break;
}
if (command is GoCommand && result.Success)
    ShowRoom();

// After:
state.DisplayResult(command, result);
```

**Signature:**
```csharp
public static void DisplayResult(this GameState state, ICommand command, CommandResult result)
```

---

## string.IsHelpRequest

**File:** `Extensions/StringExtensions.cs`

Returns `true` when input matches common help keywords: `"help"`, `"halp"`, or `"?"`.

```csharp
using MarcusMedina.TextAdventure.Extensions;

// Before (repeated in 6+ slices):
if (input.Lower() is "help" or "halp" or "?")
{
    ShowHelp();
    continue;
}

// After:
if (input.IsHelpRequest())
{
    ShowHelp();
    continue;
}
```

**Signature:**
```csharp
public static bool IsHelpRequest(this string? text)
```

---

## ILocation.GetRoomExitsWithDoors

**File:** `Extensions/LocationExtensions.cs`

Returns exit names with door state info, e.g. `"East (Cabin Door, Locked)"`.

```csharp
// Before (repeated in 3+ slices):
var exits = location.Exits
    .Select(exit =>
    {
        var direction = exit.Key.ToString().ToLowerInvariant().ToProperCase();
        var door = exit.Value.Door;
        if (door == null) return direction;
        var stateText = door.State.ToString().ToLowerInvariant().ToProperCase();
        return $"{direction} ({door.Name.ToProperCase()}, {stateText})";
    })
    .ToList();

// After:
var exits = location.GetRoomExitsWithDoors();
```

**Signature:**
```csharp
public static List<string> GetRoomExitsWithDoors(this ILocation location, ILanguageProvider? provider = null)
```

---

## ILocation.GetRoomNpcs

**File:** `Extensions/LocationExtensions.cs`

Returns NPC names in the location.

```csharp
// Before:
var npcs = location.Npcs.Select(n => n.Name).ToList();

// After:
var npcs = location.GetRoomNpcs();
```

**Signature:**
```csharp
public static List<string> GetRoomNpcs(this ILocation location, ILanguageProvider? provider = null)
```

---

## ILocation.ShowRoom

**File:** `Extensions/LocationExtensions.cs`

Prints the full room display directly from a location object. Used by `GameState.ShowRoom()` internally but can also be called standalone.

```csharp
location.ShowRoom();
// or with options:
location.ShowRoom(showAllItems: true, provider: myLanguageProvider);
```

**Signature:**
```csharp
public static void ShowRoom(this ILocation location, bool showAllItems = false, ILanguageProvider? provider = null)
```

---

## Complete Game Loop Example

Here is a minimal game loop using all the new extensions:

```csharp
using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;

// Setup
ConsoleExtensions.SetupC64("Forest Adventure");

Location entrance = (id: "entrance", description: "You stand at the forest gate.");
Location forest = (id: "forest", description: "A thick forest surrounds you.");
entrance.AddExit(Direction.North, forest);

var state = new GameState(entrance);
var parser = KeywordParserConfigBuilder.Default().Build().CreateParser();

Console.WriteLine("=== FOREST ADVENTURE ===");
Console.WriteLine("Explore the forest!\n");
state.ShowRoom();

while (true)
{
    Console.Write("\n> ");
    var input = Console.ReadLine()?.Trim();
    if (string.IsNullOrEmpty(input)) continue;

    if (input.IsHelpRequest())
    {
        Console.WriteLine("Commands: go, look, take, drop, inventory, quit");
        continue;
    }

    var command = parser.Parse(input);
    var result = state.Execute(command);

    if (result.ShouldQuit) break;

    state.DisplayResult(command, result);
}

Console.WriteLine("\nThanks for playing!");
```
