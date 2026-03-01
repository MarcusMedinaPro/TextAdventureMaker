# 👁️ Directional Looking System - "Look North" Features

## Klassisk Textäventyr Feature

### Traditional commands som vi ska stödja:
```
look north    → Kika in i rummet norrut
look door     → Undersök dörren/passagen
look south    → Titta söderut genom öppningen
examine east  → Detaljerat kika österut
peek west     → Försiktig titt västerut
```

## 🎯 Pathway Integration med Look System

### Core implementation:
```csharp
public class LookSystem
{
    public LookResult LookInDirection(Player player, Direction direction, Room currentRoom)
    {
        var pathway = currentRoom.GetPathway(direction);

        if (pathway == null)
        {
            return LookResult.NoExit($"Det finns ingen öppning åt {direction.ToSwedish()}.");
        }

        // Olika resultat beroende på pathway state
        if (pathway.IsHidden)
        {
            return LookResult.NoExit($"Du ser inget särskilt åt {direction.ToSwedish()}.");
        }

        if (!pathway.IsOpen)
        {
            return LookResult.BlockedView(
                $"En stängd dörr blockerar sikten åt {direction.ToSwedish()}.",
                pathway.GetDoorDescription());
        }

        if (pathway.IsLocked && !pathway.IsTransparent)
        {
            return LookResult.BlockedView(
                $"En låst dörr hindrar dig från att se åt {direction.ToSwedish()}.",
                pathway.GetDoorDescription());
        }

        // Success! Vi kan se in i nästa rum
        var targetRoom = pathway.GetTargetRoom(currentRoom);
        return LookResult.Success(GetRoomGlimpse(targetRoom, pathway, direction));
    }

    private string GetRoomGlimpse(Room targetRoom, Pathway pathway, Direction direction)
    {
        var glimpse = new StringBuilder();

        // Grundläggande beskrivning
        glimpse.AppendLine($"Genom {pathway.GetDescription()} ser du:");
        glimpse.AppendLine(targetRoom.GetShortDescription());

        // Synliga objekt
        var visibleObjects = targetRoom.GetVisibleObjects()
            .Where(obj => obj.IsVisibleFromDistance)
            .Take(3); // Begränsa för realisms skull

        if (visibleObjects.Any())
        {
            glimpse.AppendLine($"Du kan urskilja: {string.Join(", ", visibleObjects.Select(o => o.Name))}.");
        }

        // NPCs som syns
        var visibleNPCs = targetRoom.GetNPCs()
            .Where(npc => npc.IsVisible && !npc.IsHiding)
            .Take(2);

        if (visibleNPCs.Any())
        {
            glimpse.AppendLine($"Du ser: {string.Join(" och ", visibleNPCs.Select(npc => npc.Name))}.");
        }

        // Atmosphere och mood
        if (targetRoom.HasProperty("dark") && !player.HasLightSource())
        {
            glimpse.AppendLine("Rummet verkar vara i mörker.");
        }

        if (targetRoom.HasProperty("smoky"))
        {
            glimpse.AppendLine("Rök gör det svårt att se tydligt.");
        }

        return glimpse.ToString().Trim();
    }
}

public class LookResult
{
    public bool Success { get; set; }
    public string Description { get; set; }
    public string DoorDescription { get; set; }
    public LookResultType Type { get; set; }

    public static LookResult Success(string description) => new()
    {
        Success = true,
        Description = description,
        Type = LookResultType.CanSee
    };

    public static LookResult BlockedView(string message, string doorDesc) => new()
    {
        Success = false,
        Description = message,
        DoorDescription = doorDesc,
        Type = LookResultType.Blocked
    };

    public static LookResult NoExit(string message) => new()
    {
        Success = false,
        Description = message,
        Type = LookResultType.NoExit
    };
}

public enum LookResultType
{
    CanSee,     // Kan se in i rummet
    Blocked,    // Dörr/hindrr blockerar sikt
    NoExit      // Ingen öppning åt det hållet
}
```

## 🚪 Enhanced Pathway Properties

### Pathway utökad med visual properties:
```csharp
public class Pathway
{
    // Existing properties...
    public bool IsLocked { get; set; } = false;
    public bool IsOpen { get; set; } = true;

    // NEW: Visual properties för looking
    public bool IsTransparent { get; set; } = false;  // Glas, galler, etc.
    public bool AllowsPeeking { get; set; } = true;   // Kan man kika genom?
    public float VisibilityFactor { get; set; } = 1.0f; // 0.0-1.0 hur mycket man ser

    // Door/passage descriptions
    public string ClosedDescription { get; set; } = "en stängd dörr";
    public string OpenDescription { get; set; } = "en öppen dörr";
    public string LockedDescription { get; set; } = "en låst dörr";

    public string GetDescription()
    {
        if (IsLocked) return LockedDescription;
        if (!IsOpen) return ClosedDescription;
        return OpenDescription;
    }

    // Distance viewing modifiers
    public List<DistanceViewModifier> ViewModifiers { get; set; } = new();
}

public class DistanceViewModifier
{
    public string Type { get; set; } // "fog", "darkness", "bars", "glass"
    public float Intensity { get; set; } // 0.0-1.0
    public string Description { get; set; }
}

// Usage examples:
pathway.IsTransparent = true; // Glass door
pathway.VisibilityFactor = 0.3f; // Frosted glass
pathway.ViewModifiers.Add(new DistanceViewModifier
{
    Type = "bars",
    Intensity = 0.7f,
    Description = "genom järngaller"
});
```

## 🎮 Practical Usage Examples

### Basic directional looking:
```csharp
// Player commands and responses
"look north" →
"Genom en öppen dörr ser du:
Ett rymligt vardagsrum med bekväma fåtöljer framför en öppen spis.
Du kan urskilja: ett soffbord, en bokhylla.
Du ser: en katt som sover i fåtöljen."

"look door" →
"Du undersöker dörren norrut. Det är en tung ekdörr med mässingshandtag."

"look south" →
"Det finns ingen öppning åt söder."

"peek east" →
"Genom järngallret ser du:
En mörk cell. Det är svårt att urskilja detaljer genom gallret."
```

### Advanced scenarios:
```csharp
// Frosted glass bathroom door
bathroom_door.IsTransparent = true;
bathroom_door.VisibilityFactor = 0.2f;
bathroom_door.ClosedDescription = "en frostad glasdörr";

"look east" →
"Genom en frostad glasdörr ser du:
Bara suddiga konturer. Du kan ana att någon rör sig därinne."

// Prison bars
cell_gate.IsTransparent = true;
cell_gate.IsLocked = true;
cell_gate.LockedDescription = "ett låst järngaller";

"look north" →
"Genom ett låst järngaller ser du:
En mörk fängelsecell med halm på golvet.
Du kan urskilja: en träbänk, en vattenskål."

// Keyhole peeping
study_door.AllowsPeeking = true;
study_door.IsLocked = true;
study_door.VisibilityFactor = 0.1f;

"peek through keyhole" →
"Genom nyckelhålet ser du:
En del av ett skrivbord. Någon sitter och arbetar vid det."
```

## 🔍 Parser Integration

### Smart command parsing:
```csharp
public class DirectionalLookParser
{
    public ParsedCommand ParseLookCommand(string input)
    {
        var patterns = new Dictionary<Regex, Func<Match, ParsedCommand>>
        {
            // "look north", "look n"
            [new Regex(@"look (north|south|east|west|n|s|e|w|up|down)")] = match =>
                new ParsedCommand("look_direction", parameter: match.Groups[1].Value),

            // "look door", "examine door"
            [new Regex(@"(look|examine) (door|passage|opening|exit)")] = match =>
                new ParsedCommand("look_door", parameter: match.Groups[2].Value),

            // "peek north", "peek through door"
            [new Regex(@"peek (through )?(north|south|east|west|door|keyhole)")] = match =>
                new ParsedCommand("peek", parameter: match.Groups[2].Value),

            // "look through window"
            [new Regex(@"look through (.*)")] = match =>
                new ParsedCommand("look_through", parameter: match.Groups[1].Value)
        };

        foreach (var (pattern, parser) in patterns)
        {
            var match = pattern.Match(input.ToLower());
            if (match.Success)
                return parser(match);
        }

        return new ParsedCommand("unknown");
    }
}
```

## 🎭 Atmospheric Enhancements

### Room visibility modifiers:
```csharp
public class Room
{
    public Dictionary<string, float> VisibilityModifiers { get; set; } = new();

    public string GetShortDescription(float visibilityFactor = 1.0f)
    {
        var baseDescription = ShortDescription;

        if (visibilityFactor < 0.3f)
            return "Du kan bara ana vaga konturer.";

        if (visibilityFactor < 0.6f)
            return $"Du kan delvis urskilja: {baseDescription.ToLower()}";

        return baseDescription;
    }

    public List<GameObject> GetVisibleObjects(float visibilityFactor = 1.0f)
    {
        return Objects
            .Where(obj => obj.IsVisible)
            .Where(obj => obj.VisibilityThreshold <= visibilityFactor)
            .OrderBy(obj => obj.Prominence) // Stora/viktiga objekt syns först
            .ToList();
    }
}

public class GameObject
{
    public float VisibilityThreshold { get; set; } = 0.5f; // Hur tydligt måste det vara för att synas?
    public int Prominence { get; set; } = 5; // 1-10, hur uppenbar är objektet?
    public bool IsVisibleFromDistance { get; set; } = true;

    // Stora objekt syns lättare
    public void SetSize(ObjectSize size)
    {
        Size = size;
        switch (size)
        {
            case ObjectSize.Tiny:
                VisibilityThreshold = 0.8f;
                Prominence = 2;
                break;
            case ObjectSize.Small:
                VisibilityThreshold = 0.6f;
                Prominence = 4;
                break;
            case ObjectSize.Large:
                VisibilityThreshold = 0.3f;
                Prominence = 8;
                break;
            case ObjectSize.Huge:
                VisibilityThreshold = 0.1f;
                Prominence = 10;
                break;
        }
    }
}
```

## 🌟 Special Look Scenarios

### Conditional visibility:
```csharp
public class ConditionalLookModifier
{
    public Func<Player, GameContext, bool> Condition { get; set; }
    public string ModifiedDescription { get; set; }
    public float VisibilityChange { get; set; }
}

// Player med fackla ser mer
var torchModifier = new ConditionalLookModifier
{
    Condition = (player, ctx) => player.HasActiveLightSource(),
    VisibilityChange = 0.4f,
    ModifiedDescription = "Din fackla lyser upp rummet och avslöjar detaljer."
};

// Magiker ser magiska aura
var magicSightModifier = new ConditionalLookModifier
{
    Condition = (player, ctx) => player.HasSkill("Magic Sight"),
    ModifiedDescription = "Du ser en svag magisk aura omkring något i rummet."
};
```

### Time-based visibility:
```csharp
public class TimeBasedVisibility
{
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public string TimeBasedDescription { get; set; }
}

// Different descriptions based on time of day
var bedroom = new Room("bedroom")
    .AddTimeBasedVisibility(new TimeBasedVisibility
    {
        StartTime = TimeSpan.FromHours(22),
        EndTime = TimeSpan.FromHours(6),
        TimeBasedDescription = "Månljuset filtreras genom gardinerna och kastar skuggor över rummet."
    });
```

## 🧪 Testing Strategy

### Look system tests:
```csharp
[TestClass]
public class DirectionalLookTests
{
    [TestMethod]
    public void LookNorth_WithOpenDoor_ShowsTargetRoom()
    {
        // Arrange
        var world = CreateTestWorld();
        var kitchen = world.GetRoom("kitchen");
        var living = world.GetRoom("living");
        var pathway = world.AddPathway("door", "kitchen", Direction.North, "living");

        var player = new Player();
        var lookSystem = new LookSystem();

        // Act
        var result = lookSystem.LookInDirection(player, Direction.North, kitchen);

        // Assert
        Assert.IsTrue(result.Success);
        Assert.Contains("vardagsrum", result.Description.ToLower());
    }

    [TestMethod]
    public void LookNorth_WithClosedDoor_ShowsDoorDescription()
    {
        // Arrange
        var world = CreateTestWorld();
        var kitchen = world.GetRoom("kitchen");
        var pathway = world.AddPathway("door", "kitchen", Direction.North, "living")
            .Close();

        var player = new Player();
        var lookSystem = new LookSystem();

        // Act
        var result = lookSystem.LookInDirection(player, Direction.North, kitchen);

        // Assert
        Assert.IsFalse(result.Success);
        Assert.AreEqual(LookResultType.Blocked, result.Type);
        Assert.Contains("stängd dörr", result.Description.ToLower());
    }

    [TestMethod]
    public void LookNorth_NoExit_ReturnsAppropriateMessage()
    {
        // Arrange
        var world = CreateTestWorld();
        var kitchen = world.GetRoom("kitchen");
        var player = new Player();
        var lookSystem = new LookSystem();

        // Act
        var result = lookSystem.LookInDirection(player, Direction.East, kitchen);

        // Assert
        Assert.IsFalse(result.Success);
        Assert.AreEqual(LookResultType.NoExit, result.Type);
    }
}
```

## 🎯 Integration med Slice 1

### Core features för initial implementation:
1. **Basic "look north"** - kika genom öppna pathways
2. **Closed door handling** - visa att dörr blockerar sikt
3. **No exit messages** - hantera obefintliga utgångar
4. **Simple room glimpses** - kort beskrivning av target room

### Future enhancements (senare slices):
- Transparency och visibility factors
- Conditional modifiers (light sources, skills)
- Time-based descriptions
- Complex atmospheric effects

---

## 🚀 Benefits av Look System

### För Spelare:
✅ **Immersive exploration** - kika innan man går
✅ **Strategic planning** - se vad som väntar
✅ **Rich atmosphere** - upptäck miljön gradvis
✅ **Classic feel** - autentisk textäventyr-känsla

### För World Builders:
✅ **Richer storytelling** - flera layers av beskrivning
✅ **Atmospheric control** - styra vad spelaren ser när
✅ **Puzzle possibilities** - visibility som game mechanic
✅ **Immersion tools** - bygga tension och mystery

**"Look north" är en PERFEKT feature för Slice 1! Klassisk, användbar och inte för komplex.** 👁️🎯

Ska vi lägga till detta som en core navigation-feature? Det passar perfekt med pathway-systemet! 🚀