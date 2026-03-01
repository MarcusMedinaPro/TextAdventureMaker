# 🗺️ Map Generator System - Procedural World Creation

## Revolutionary Map Generation för TAF

### Core Concept: Smart Procedural Generation
```csharp
// Inte bara random rooms - semantiskt medvetna maps!
var generator = new TAFMapGenerator()
    .WithTheme("haunted_mansion")
    .WithSize(MapSize.Medium)
    .WithComplexity(ComplexityLevel.Intermediate)
    .WithSpatialAwareness(true) // Använder vårt spatial system!
    .Generate();
```

## 🏗️ Generator Architecture

### Semantic Room Templates
```csharp
public class RoomTemplate
{
    public string Id { get; set; }
    public string ThemeCategory { get; set; } // "residential", "commercial", "supernatural"
    public RoomPurpose Purpose { get; set; } // Kitchen, Bedroom, Boss, etc.
    public List<Direction> RequiredExits { get; set; } = new();
    public List<Direction> PreferredExits { get; set; } = new();
    public List<Direction> ForbiddenExits { get; set; } = new();

    // Spatial awareness properties
    public bool SupportsDirectionalActions { get; set; } = true;
    public List<string> DefaultObjects { get; set; } = new();
    public AtmosphereSettings Atmosphere { get; set; } = new();

    // Generation constraints
    public int MinDistanceFromStart { get; set; } = 0;
    public int MaxDistanceFromStart { get; set; } = 99;
    public List<string> RequiredNearbyRooms { get; set; } = new();
    public List<string> ForbiddenNearbyRooms { get; set; } = new();
}

// Example templates
var kitchenTemplate = new RoomTemplate
{
    Id = "modern_kitchen",
    ThemeCategory = "residential",
    Purpose = RoomPurpose.Kitchen,
    RequiredExits = { Direction.North }, // Måste ha åtminstone en utgång
    PreferredExits = { Direction.East, Direction.West }, // Bra att ha
    ForbiddenExits = { Direction.Down }, // Kök har sällan källartrappa
    DefaultObjects = { "stove", "refrigerator", "kitchen_table", "knife" },
    RequiredNearbyRooms = { "dining_room", "living_room" },
    ForbiddenNearbyRooms = { "bathroom", "bedroom" } // Realistiska begränsningar
};
```

### Theme-Based Generation
```csharp
public class ThemeDefinition
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    public List<RoomTemplate> AvailableRooms { get; set; } = new();
    public Dictionary<string, float> RoomProbabilities { get; set; } = new();
    public List<ObjectTemplate> AvailableObjects { get; set; } = new();
    public PathwayStyleSettings PathwayStyles { get; set; } = new();

    // Spatial awareness för tema
    public List<DirectionalActionTemplate> ThemeActions { get; set; } = new();
    public AtmosphereProfile DefaultAtmosphere { get; set; } = new();
}

// Exempel: Haunted Mansion Theme
var hauntedMansion = new ThemeDefinition
{
    Id = "haunted_mansion",
    Name = "Spökhus",
    Description = "Ett kusligt viktorianskt herrgård fyllt med övernaturliga fenomen",

    RoomProbabilities = new()
    {
        ["grand_hall"] = 1.0f,      // Alltid en grand hall
        ["library"] = 0.8f,        // Troligtvis bibliotek
        ["ballroom"] = 0.6f,       // Möjligtvis ballsal
        ["secret_passage"] = 0.4f, // Några hemliga passager
        ["basement"] = 0.9f,       // Nästan alltid källare
        ["attic"] = 0.7f,          // Oftast vind
        ["servant_quarters"] = 0.5f
    },

    ThemeActions = new()
    {
        new DirectionalActionTemplate
        {
            Verb = "throw",
            Objects = { "holy_water", "crucifix", "salt" },
            Description = "Kasta heliga föremål för att avvärja onda andar",
            SuccessMessages = { "Det onda drar sig undan!", "Skuggorna viker tillbaka!" }
        },
        new DirectionalActionTemplate
        {
            Verb = "shout",
            Description = "Ropa för att locka eller skrämma andar",
            EffectRadius = 3, // Rösten når 3 rum bort
            AlertsEnemies = true
        }
    }
};
```

## 🎯 Smart Generation Algorithms

### Constraint Satisfaction Approach
```csharp
public class MapGenerationEngine
{
    public GeneratedMap Generate(ThemeDefinition theme, MapGenerationSettings settings)
    {
        var map = new GeneratedMap();
        var constraints = BuildConstraints(theme, settings);

        // Step 1: Place mandatory rooms
        PlaceMandatoryRooms(map, theme, constraints);

        // Step 2: Use constraint satisfaction för remaining rooms
        var solver = new ConstraintSolver();
        var solution = solver.Solve(constraints);

        // Step 3: Generate pathways with spatial awareness
        GeneratePathways(map, theme);

        // Step 4: Populate with objects och NPCs
        PopulateRooms(map, theme);

        // Step 5: Add directional actions och spatial features
        AddSpatialFeatures(map, theme);

        return map;
    }

    private void PlaceMandatoryRooms(GeneratedMap map, ThemeDefinition theme, ConstraintSet constraints)
    {
        // Hitta rum med probability = 1.0 (mandatory)
        var mandatoryRooms = theme.RoomProbabilities
            .Where(kvp => kvp.Value >= 1.0f)
            .Select(kvp => kvp.Key);

        foreach (var roomType in mandatoryRooms)
        {
            var template = theme.GetRoomTemplate(roomType);
            var position = FindOptimalPosition(map, template, constraints);

            var room = CreateRoomFromTemplate(template, position);
            map.AddRoom(room);

            // Update constraints baserat på placed room
            constraints.UpdateForPlacedRoom(room);
        }
    }
}
```

### Spatial Flow Generation
```csharp
public class SpatialFlowGenerator
{
    public void GenerateNaturalFlow(GeneratedMap map, ThemeDefinition theme)
    {
        // Skapa naturliga rörelsemönster
        var flowAnalyzer = new PlayerFlowAnalyzer();
        var movements = flowAnalyzer.PredictPlayerMovements(map);

        foreach (var movement in movements)
        {
            // Placera intressanta directional actions längs predicted paths
            if (movement.Frequency > 0.7f) // Högtrafikerat område
            {
                AddDirectionalOpportunities(movement.Path, theme);
            }
        }
    }

    private void AddDirectionalOpportunities(List<Room> path, ThemeDefinition theme)
    {
        for (int i = 0; i < path.Count - 1; i++)
        {
            var currentRoom = path[i];
            var nextRoom = path[i + 1];
            var direction = currentRoom.GetDirectionTo(nextRoom);

            // Lägg till spatial awareness opportunities
            if (ShouldAddThrowOpportunity(currentRoom, nextRoom, theme))
            {
                AddThrowableObjects(currentRoom, direction);
            }

            if (ShouldAddLookOpportunity(currentRoom, nextRoom))
            {
                ConfigureVisualSightlines(currentRoom, direction);
            }
        }
    }
}
```

## 🎮 Gameplay-Driven Generation

### Puzzle Integration
```csharp
public class PuzzleIntegratedGenerator
{
    public void IntegratePuzzles(GeneratedMap map, DifficultyLevel difficulty)
    {
        var puzzleTemplates = GetPuzzleTemplates(difficulty);

        foreach (var puzzle in puzzleTemplates)
        {
            var suitableRooms = FindSuitableRoomsForPuzzle(map, puzzle);
            var selectedRoom = suitableRooms.GetRandom();

            ApplyPuzzleToRoom(selectedRoom, puzzle);

            // Exempel: Key-lock puzzle med spatial awareness
            if (puzzle.Type == PuzzleType.KeyLock)
            {
                // Göm nyckeln i ett annat rum
                var keyRoom = FindRoomAtDistance(selectedRoom, puzzle.KeyDistance);

                // Skapa directional hints
                AddDirectionalHints(selectedRoom, keyRoom, puzzle);

                // "throw key north" opportunity när man hittar nyckeln
                keyRoom.AddObject(puzzle.Key);
                selectedRoom.GetPathway(GetDirectionTo(keyRoom))
                    .OnPlayerApproach += ShowKeyThrowOpportunity;
            }
        }
    }
}
```

### Dynamic Difficulty Scaling
```csharp
public class DynamicDifficultyGenerator
{
    public void AdjustMapDifficulty(GeneratedMap map, PlayerProfile profile)
    {
        var skillLevel = AnalyzePlayerSkill(profile);

        if (skillLevel.SpatialAwareness > 0.8f)
        {
            // Erfaren spelare - lägg till advanced directional challenges
            AddAdvancedSpatialChallenges(map);
        }

        if (skillLevel.PuzzleSolving < 0.4f)
        {
            // Nybörjare - förenkla och lägg till hints
            SimplifyExistingPuzzles(map);
            AddSpatialHints(map);
        }
    }

    private void AddAdvancedSpatialChallenges(GeneratedMap map)
    {
        // Multi-room directional actions
        foreach (var room in map.GetRooms())
        {
            if (room.HasProperty("strategic_position"))
            {
                // "fire arrow north" som träffar fiende 2 rum bort
                AddLongRangeDirectionalAction(room, ActionType.RangedCombat, range: 2);

                // "shout east" som aktiverar mechanism i adjacent room
                AddCommunicationBasedPuzzle(room, Direction.East);
            }
        }
    }
}
```

## 🗂️ Template System

### JSON-Based Room Templates
```json
{
  "themes": {
    "modern_house": {
      "name": "Modernt Hem",
      "rooms": {
        "kitchen": {
          "description": "Ett modernt kök med rostfria apparater",
          "objects": ["stove", "refrigerator", "microwave", "knife_block"],
          "required_exits": ["north"],
          "preferred_exits": ["east", "west"],
          "spatial_actions": [
            {
              "verb": "throw",
              "objects": ["kitchen_knife", "pan"],
              "description": "Kasta köksredskap för självförsvar"
            }
          ]
        },
        "living_room": {
          "description": "Ett mysigt vardagsrum med stora fönster",
          "objects": ["sofa", "tv", "coffee_table", "lamp"],
          "required_exits": ["south"],
          "atmosphere": {
            "lighting": "bright",
            "mood": "comfortable"
          },
          "spatial_actions": [
            {
              "verb": "look",
              "special_visibility": true,
              "description": "Stora fönster ger excellent sikt utåt"
            }
          ]
        }
      }
    }
  }
}
```

### Procedural Object Placement
```csharp
public class IntelligentObjectPlacer
{
    public void PlaceObjects(Room room, RoomTemplate template, ThemeDefinition theme)
    {
        // Context-aware object placement
        foreach (var objectId in template.DefaultObjects)
        {
            var objectTemplate = theme.GetObjectTemplate(objectId);
            var placement = CalculateOptimalPlacement(room, objectTemplate);

            var gameObject = CreateFromTemplate(objectTemplate);

            // Spatial awareness integration
            if (objectTemplate.SupportsDirectionalActions)
            {
                ConfigureDirectionalActions(gameObject, room);
            }

            room.AddObject(gameObject, placement);
        }

        // Add theme-specific random objects
        var randomObjects = SelectRandomObjects(theme, room.Purpose);
        foreach (var obj in randomObjects)
        {
            room.AddObject(obj);
        }
    }

    private void ConfigureDirectionalActions(GameObject obj, Room room)
    {
        // Exempel: Kniv i kök kan kastas mot hot
        if (obj.Id == "kitchen_knife")
        {
            obj.AddDirectionalAction("throw", directions: Direction.All);
            obj.SetThrowRange(1); // Kan kastas till adjacent room
            obj.SetThrowDamage(moderate: true);
        }

        // Lamp kan kastas för distraction
        if (obj.Type == ObjectType.Light)
        {
            obj.AddDirectionalAction("throw", directions: Direction.All);
            obj.OnThrowImpact += (target, direction) =>
            {
                if (target.Type == RoomType.Dark)
                    target.AddTemporaryLight(duration: TimeSpan.FromMinutes(5));
            };
        }
    }
}
```

## 🔧 Integration med TAF Core

### Generator API för Game Builders
```csharp
// Simple API för basic generation
var world = TAFMapGenerator.Quick()
    .Theme("haunted_mansion")
    .Size(10, 15) // 10x15 grid
    .Generate();

// Advanced API för custom generation
var world = new TAFMapGenerator()
    .WithTheme("custom_theme")
    .WithCustomRoomTemplates(myTemplates)
    .WithSpatialAwarenessLevel(SpatialLevel.Advanced)
    .WithPuzzleDensity(0.3f) // 30% av rum har puzzles
    .WithDirectionalActionDensity(0.7f) // 70% har directional opportunities
    .WithSeed(12345) // Reproducible generation
    .Generate();

// Integration med existing TAF world
var existingWorld = LoadWorld("my_world.taf");
var expansion = TAFMapGenerator.CreateExpansion()
    .For(existingWorld)
    .ConnectTo("existing_room_id", Direction.North)
    .WithTheme("basement_complex")
    .Generate();

existingWorld.AddExpansion(expansion);
```

### Live Generation Support
```csharp
public class LiveMapGenerator
{
    // Generate map as player explores
    public Room GenerateAdjacentRoom(Room currentRoom, Direction direction, PlayerProfile profile)
    {
        var theme = DetermineThemeFromContext(currentRoom);
        var template = SelectAppropriateTemplate(currentRoom, direction, theme, profile);

        var newRoom = GenerateRoomFromTemplate(template);

        // Skapa pathway med spatial awareness
        var pathway = world.CreatePathway(currentRoom, direction, newRoom)
            .WithSpatialActions(theme.GetDirectionalActions())
            .WithVisualSightlines(template.VisibilitySettings);

        return newRoom;
    }
}
```

## 🧪 Testing & Validation

### Generated Map Quality Tests
```csharp
[TestClass]
public class MapGeneratorTests
{
    [TestMethod]
    public void GeneratedMap_HasNoIsolatedRooms()
    {
        var generator = new TAFMapGenerator();
        var map = generator.WithTheme("test_theme").Generate();

        var validator = new MapValidator();
        var isolated = validator.FindIsolatedRooms(map);

        Assert.AreEqual(0, isolated.Count, "Generated map contains isolated rooms");
    }

    [TestMethod]
    public void GeneratedMap_HasAppropriateDirectionalActions()
    {
        var generator = new TAFMapGenerator();
        var map = generator
            .WithTheme("action_adventure")
            .WithDirectionalActionDensity(0.8f)
            .Generate();

        var actionsCount = map.GetRooms()
            .SelectMany(r => r.GetAvailableDirectionalActions())
            .Count();

        var expectedMinActions = map.GetRooms().Count * 0.6f; // Åtminstone 60% coverage
        Assert.IsTrue(actionsCount >= expectedMinActions);
    }
}
```

## 🚀 Advanced Features

### AI-Powered Generation
```csharp
public class AIEnhancedGenerator
{
    private readonly IOpenAIClient _aiClient;

    public async Task<string> GenerateRoomDescription(RoomTemplate template, ThemeDefinition theme)
    {
        var prompt = $@"
            Generate a vivid room description for a {theme.Name} themed text adventure.
            Room type: {template.Purpose}
            Available objects: {string.Join(", ", template.DefaultObjects)}
            Atmosphere: {template.Atmosphere}

            Make it immersive and include hints about possible directional actions.
            Swedish language, 2-3 sentences.
        ";

        return await _aiClient.GenerateText(prompt);
    }

    public async Task<List<DirectionalActionTemplate>> GenerateContextualActions(Room room, ThemeDefinition theme)
    {
        var prompt = $@"
            Given a {theme.Name} themed room with objects: {string.Join(", ", room.Objects.Select(o => o.Name))}

            Suggest 3-5 directional actions that would be natural and exciting in this context.
            Focus on tactical, defensive, or puzzle-solving opportunities.

            Format: verb|object|directions|description
        ";

        var response = await _aiClient.GenerateText(prompt);
        return ParseDirectionalActions(response);
    }
}
```

### Performance Optimization
```csharp
public class OptimizedGenerator
{
    private readonly ConcurrentDictionary<string, RoomTemplate> _templateCache = new();
    private readonly ThreadLocal<Random> _randomGenerator = new(() => new Random());

    public async Task<GeneratedMap> GenerateAsync(ThemeDefinition theme, MapGenerationSettings settings)
    {
        // Parallel generation av room templates
        var roomTasks = theme.RoomProbabilities
            .Where(kvp => _randomGenerator.Value.NextDouble() < kvp.Value)
            .Select(async kvp => await GenerateRoomAsync(kvp.Key, theme))
            .ToArray();

        var rooms = await Task.WhenAll(roomTasks);

        // Sequential pathway generation (kräver room positions)
        var map = new GeneratedMap();
        map.AddRooms(rooms);

        GenerateOptimalPathways(map, theme);

        return map;
    }
}
```

---

## 🎯 Integration med Slice Development

### Slice 1: Basic Generation
- Simple room templates
- Basic pathway creation
- Theme-based room selection
- Manual object placement

### Slice 3: Spatial Integration
- Directional action templates
- Look system integration
- Basic spatial awareness

### Slice 5: Advanced Generation
- AI-powered descriptions
- Dynamic difficulty scaling
- Live generation support

### Slice 7: Full AI Integration
- Context-aware generation
- Player behavior analysis
- Adaptive content creation

**Map Generator blir the foundation för endless content creation i TAF! 🗺️🚀**

Detta system låter utvecklare fokusera på game logic istället för att manuellt bygga worlds, medan det ändå behåller full kontroll över quality och theme coherence!