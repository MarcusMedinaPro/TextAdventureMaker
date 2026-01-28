## Slice 41: Testing & Validation Tools

**Mål:** Verktyg för att testa och validera textäventyr.

_Från Write Your Own Adventure Programs (Usborne):_

- "Try every possible command in every situation"
- "Ask other people to play - they try things you didn't think of"
- "Testing is for improving puzzles and making sure the game is enjoyable"

### Task 41.1: Reachability Validator

```csharp
var validator = game.CreateValidator();

// Kontrollera att alla rum går att nå
var unreachableLocations = validator.FindUnreachableLocations();
// Output: ["secret_room", "hidden_cave"]

// Kontrollera att alla items går att få tag på
var unreachableItems = validator.FindUnreachableItems();
// Output: [("magic_gem", "Requires key that doesn't exist")]

// Kontrollera att alla quests går att klara
var impossibleQuests = validator.FindImpossibleQuests();
```

### Task 41.2: Command Coverage Report

```csharp
// Lista alla möjliga kommandon per rum
foreach (var location in game.Locations)
{
    var commands = validator.GetPossibleCommands(location);
    Console.WriteLine($"{location.Name}: {string.Join(", ", commands)}");
}

// Output:
// Cave: go north, go south, take torch, look, inventory
// Forest: go east, go west, talk to hermit, attack wolf

// Hitta kommandon som aldrig används
var unusedCommands = validator.FindUnusedCommands();
```

### Task 41.3: Automated Playthrough

```csharp
// "Try everything" automation
var explorer = game.CreateExplorer();

// Random exploration
var log = explorer.RandomWalk(maxSteps: 1000);

// Exhaustive exploration (depth-first)
var allPaths = explorer.ExploreAllPaths();

// Find shortest path to win
var winPath = explorer.FindWinningPath();
if (winPath == null)
    Console.WriteLine("Warning: No winning path exists!");

// Replay a specific path
explorer.Replay(winPath);
```

### Task 41.4: Dead-end Detection

```csharp
// Hitta situationer där spelaren fastnar
var deadEnds = validator.FindDeadEnds();
// Output: [
//   "Using key on wrong door destroys key, can't proceed",
//   "Taking gem triggers collapse, blocks exit permanently"
// ]

// Hitta point-of-no-return utan varning
var noReturnPoints = validator.FindUnmarkedPointsOfNoReturn();
```

### Task 41.5: Testing Mode (Debug)

```csharp
game.EnableTestingMode();

// I testing mode:
// - "teleport <location>" - hoppa till rum
// - "spawn <item>" - skapa item
// - "godmode" - immortal
// - "showmap" - visa alla rum
// - "showitems" - lista alla items och var de är
// - "shownpcs" - lista alla NPCs och deras state
// - "skiptime <ticks>" - hoppa framåt i tid
// - "setflag <name> <value>" - sätt worldstate
// - "listcommands" - visa alla kommandon i nuvarande rum
```

### Task 41.6: Puzzle Dependency Graph

```csharp
// Generera beroendegraf för puzzles
var graph = validator.GeneratePuzzleGraph();

// Export till Mermaid
var mermaid = graph.ToMermaid();
// Output:
// graph LR
//     key --> locked_door
//     locked_door --> treasure_room
//     torch --> dark_cave
//     dark_cave --> ancient_map
//     ancient_map --> secret_entrance

// Hitta cirkulära beroenden (omöjliga puzzles)
var circles = graph.FindCircularDependencies();
```

### Task 41.7: Story Snippet Validation

_Från Procedural Storytelling:_

> "Skriv exempel på berättelser spelaren skulle berätta, fråga: vad är minsta jag kan göra för att generera fler?"

```csharp
// Definiera "target stories" - berättelser som ska kunna hända
validator.AddTargetStory("saved_village")
    .Requires("dragon_defeated")
    .Requires("villagers_alive", min: 5)
    .Requires("reputation", min: 50);

validator.AddTargetStory("tragic_hero")
    .Requires("saved_kingdom")
    .Requires("lost_loved_one");

// Verifiera att engine kan producera dem
var achievable = validator.ValidateTargetStories();
// Output: [
//   ("saved_village", true, "Achievable via: kill dragon, save 7 villagers"),
//   ("tragic_hero", false, "Missing: no way to trigger 'lost_loved_one'")
// ]
```

---
