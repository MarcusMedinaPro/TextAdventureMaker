# 🛤️ Pathway-Based Connections - Single Source of Truth

## 🚨 Problem med Dual Door Objects

### Traditionell approach (problematisk):
```csharp
// Två separata dörr-objekt = SYNC NIGHTMARE
var doorNorth = new Door("toilet_to_bathroom", locked: true);
var doorSouth = new Door("bathroom_to_toilet", locked: true);

toilet.AddExit(Direction.North, bathroom, doorNorth);
bathroom.AddExit(Direction.South, toilet, doorSouth);

// Problem: Låser upp toilet-sidan men glömmer bathroom-sidan!
doorNorth.Unlock();  // toilet → bathroom är unlocked
// doorSouth är fortfarande locked! bathroom → toilet blocked!

// Resultat: Asymmetrisk passage = BUG FRÅN HELVETET
```

### Problemen:
❌ **Dubbel state** - två objekt som måste synkas
❌ **Asymmetriska bugs** - unlock ena sidan, glöm andra
❌ **Memory waste** - dubbla objekt för samma logiska connection
❌ **Logic complexity** - måste hålla koll på båda sidor

## ✅ Pathway Object - Single Source of Truth

### Smart unified design:
```csharp
public class Pathway
{
    public string Id { get; set; }
    public Room FromRoom { get; set; }
    public Room ToRoom { get; set; }
    public Direction Direction { get; set; }
    public Direction OppositeDirection => Direction.GetOpposite();

    // Single state för båda riktningar
    public bool IsLocked { get; set; } = false;
    public bool IsOpen { get; set; } = true;
    public string RequiredKey { get; set; }
    public bool IsHidden { get; set; } = false;
    public bool IsBlocked { get; set; } = false;

    // Conditional access
    public Func<Player, bool> CanAccess { get; set; } = _ => true;
    public string BlockedMessage { get; set; } = "Du kan inte gå åt det hållet.";

    // State methods (affects both directions)
    public Pathway Lock() { IsLocked = true; return this; }
    public Pathway Unlock() { IsLocked = false; return this; }
    public Pathway Close() { IsOpen = false; return this; }
    public Pathway Open() { IsOpen = true; return this; }
    public Pathway Hide() { IsHidden = true; return this; }
    public Pathway Show() { IsHidden = false; return this; }

    public MovementResult TryPass(Player player, Direction attemptedDirection)
    {
        if (!CanAccess(player))
            return MovementResult.Blocked(BlockedMessage);

        if (IsLocked && !player.HasItem(RequiredKey))
            return MovementResult.Blocked($"Passagen är låst. Du behöver {RequiredKey}.");

        if (!IsOpen)
            return MovementResult.Blocked("Passagen är stängd.");

        if (IsBlocked)
            return MovementResult.Blocked("Vägen är blockerad.");

        return MovementResult.Success();
    }
}
```

### World-level pathway management:
```csharp
public class World
{
    private readonly Dictionary<string, Pathway> _pathways = new();
    private readonly Dictionary<string, Room> _rooms = new();

    public Pathway AddPathway(string id, string fromRoomId, Direction direction, string toRoomId)
    {
        var fromRoom = _rooms[fromRoomId];
        var toRoom = _rooms[toRoomId];

        var pathway = new Pathway
        {
            Id = id,
            FromRoom = fromRoom,
            ToRoom = toRoom,
            Direction = direction
        };

        _pathways[id] = pathway;

        // Register pathway med båda rummen
        fromRoom.RegisterPathway(direction, pathway);
        toRoom.RegisterPathway(pathway.OppositeDirection, pathway);

        return pathway;
    }

    public Pathway GetPathway(string id) => _pathways[id];

    // Global pathway control
    public void LockPathway(string id) => _pathways[id].Lock();
    public void UnlockPathway(string id) => _pathways[id].Unlock();
}
```

## 🎯 Usage Examples

### Enkel pathway creation:
```csharp
var world = new World();

// Skapa rum
world.AddRoom("toilet", "Ett litet toalettrum");
world.AddRoom("bathroom", "Ett rymligt badrum med badkar");

// EN pathway = båda riktningar
var pathway = world.AddPathway(
    id: "toilet_bathroom_door",
    fromRoomId: "toilet",
    direction: Direction.North,
    toRoomId: "bathroom"
);

// Nu fungerar båda riktningar automatiskt:
// toilet → north → bathroom
// bathroom → south → toilet
```

### Fluent pathway configuration:
```csharp
world.AddPathway("secret_passage", "library", Direction.Up, "tower")
    .Lock()
    .RequireKey("ancient_key")
    .Hide()  // Dold passage tills den upptäcks
    .SetBlockedMessage("Du ser ingen väg uppåt härifrån.");

world.AddPathway("drawbridge", "castle", Direction.East, "courtyard")
    .SetCondition(player => world.GetFlag("drawbridge_lowered"))
    .SetBlockedMessage("Drawbridgen är upphissad. Du kan inte komma över vallgraven.");
```

### Dynamic pathway control:
```csharp
// Game events som påverkar pathways
void OnPlayerPullsLever(Player player)
{
    var bridge = world.GetPathway("drawbridge");

    if (bridge.IsBlocked)
    {
        bridge.Show().Unlock();
        world.SetFlag("drawbridge_lowered", true);
        player.Output("Drawbridgen sänks med ett skrammel!");
    }
    else
    {
        bridge.Hide().Lock();
        world.SetFlag("drawbridge_lowered", false);
        player.Output("Drawbridgen höjs upp igen!");
    }
}

void OnPlayerFindsSecretButton(Player player)
{
    var secret = world.GetPathway("secret_passage");
    secret.Show();
    player.Output("En dold dörr glider upp i bokhyllan!");
}
```

## 🏗️ Room Integration

### Simplified Room class:
```csharp
public class Room
{
    private readonly Dictionary<Direction, Pathway> _pathways = new();

    internal void RegisterPathway(Direction direction, Pathway pathway)
    {
        _pathways[direction] = pathway;
    }

    public bool HasExit(Direction direction)
    {
        return _pathways.ContainsKey(direction) &&
               !_pathways[direction].IsHidden;
    }

    public Room GetConnectedRoom(Direction direction)
    {
        if (!_pathways.TryGetValue(direction, out var pathway))
            return null;

        return pathway.FromRoom == this ? pathway.ToRoom : pathway.FromRoom;
    }

    public MovementResult TryMove(Player player, Direction direction)
    {
        if (!_pathways.TryGetValue(direction, out var pathway))
            return MovementResult.Blocked("Du kan inte gå åt det hållet.");

        return pathway.TryPass(player, direction);
    }

    public List<Direction> GetAvailableExits(Player player)
    {
        return _pathways
            .Where(kvp => !kvp.Value.IsHidden && kvp.Value.CanAccess(player))
            .Select(kvp => kvp.Key)
            .ToList();
    }
}
```

## 🎮 Advanced Pathway Features

### Conditional pathways:
```csharp
// Pathway som bara finns vissa tider
world.AddPathway("night_passage", "graveyard", Direction.North, "crypts")
    .SetCondition(player => world.GameTime.IsNight)
    .SetBlockedMessage("Passagen till kryptorna är bara öppen på natten.");

// Pathway baserat på player stats
world.AddPathway("narrow_gap", "cave", Direction.East, "chamber")
    .SetCondition(player => player.Stats.Agility > 15)
    .SetBlockedMessage("Du är för klumpig för att ta dig genom den smala springan.");

// Pathway som förändras över tid
world.AddPathway("rising_water", "shore", Direction.South, "island")
    .SetCondition(player => world.GameTime.Hour < 18) // Tidvatten
    .SetBlockedMessage("Vattennivån är för hög. Vänta tills lågvatten.");
```

### Pathway events & triggers:
```csharp
public class Pathway
{
    public event Action<Player, Direction> OnPlayerPass;
    public event Action<Player, Direction> OnPlayerBlocked;

    public MovementResult TryPass(Player player, Direction attemptedDirection)
    {
        var result = ValidatePassage(player);

        if (result.Success)
        {
            OnPlayerPass?.Invoke(player, attemptedDirection);
        }
        else
        {
            OnPlayerBlocked?.Invoke(player, attemptedDirection);
        }

        return result;
    }
}

// Usage:
pathway.OnPlayerPass += (player, dir) =>
{
    if (dir == Direction.North)
        player.Output("Du hör dörren stängas bakom dig med ett brak.");
};

pathway.OnPlayerBlocked += (player, dir) =>
{
    world.TriggerEvent("player_blocked_at_door", player, pathway);
};
```

### One-way pathways:
```csharp
public class OneWayPathway : Pathway
{
    public Direction AllowedDirection { get; set; }

    public override MovementResult TryPass(Player player, Direction attemptedDirection)
    {
        if (attemptedDirection != AllowedDirection)
            return MovementResult.Blocked("Du kan bara gå åt ett håll genom denna passage.");

        return base.TryPass(player, attemptedDirection);
    }
}

// Usage:
world.AddOneWayPathway("waterfall", "cliff", Direction.Down, "pool")
    .SetBlockedMessage("Du kan hoppa ner, men inte klättra upp igen.");
```

## 🧪 Testing Benefits

### Single point of testing:
```csharp
[TestClass]
public class PathwayTests
{
    [TestMethod]
    public void Pathway_WhenLocked_BlocksBothDirections()
    {
        // Arrange
        var world = CreateTestWorld();
        var pathway = world.AddPathway("test", "room1", Direction.North, "room2");
        var player = new Player();

        // Act
        pathway.Lock();

        // Assert - testa båda riktningar med EN test
        var resultNorth = world.GetRoom("room1").TryMove(player, Direction.North);
        var resultSouth = world.GetRoom("room2").TryMove(player, Direction.South);

        Assert.IsFalse(resultNorth.Success);
        Assert.IsFalse(resultSouth.Success);
    }

    [TestMethod]
    public void Pathway_WhenUnlocked_AllowsBothDirections()
    {
        // Arrange
        var world = CreateTestWorld();
        var pathway = world.AddPathway("test", "room1", Direction.North, "room2")
            .Lock();
        var player = new Player().WithItem("key");

        // Act
        pathway.Unlock();

        // Assert
        var resultNorth = world.GetRoom("room1").TryMove(player, Direction.North);
        var resultSouth = world.GetRoom("room2").TryMove(player, Direction.South);

        Assert.IsTrue(resultNorth.Success);
        Assert.IsTrue(resultSouth.Success);
    }
}
```

## 🚀 Benefits av Pathway Design

### Technical Benefits:
✅ **Single Source of Truth** - ett objekt, en state
✅ **Zero Sync Issues** - omöjligt att få asymmetriska states
✅ **Memory Efficient** - hälften så många objekt
✅ **Simpler Logic** - en pathway att kontrollera
✅ **Easier Testing** - testa en pathway = båda riktningar

### Game Design Benefits:
✅ **Consistent Player Experience** - passage fungerar samma åt båda hållen
✅ **Simpler World Building** - definiera logiska connections
✅ **Event System Ready** - triggers för passage events
✅ **Dynamic Worlds** - pathways kan förändras under runtime

### Developer Benefits:
✅ **No Sync Bugs** - fysiskt omöjligt att få olika states
✅ **Clean API** - `world.LockPathway("door")` låser båda riktningar
✅ **Easy Debugging** - en pathway att inspektera
✅ **Fluent Configuration** - kedja pathway properties

---

**Pathway-objektet är EN REVOLUTIONERANDE insikt! 🧠✨**

Detta eliminerar en hel kategori av bugs och gör world-building mycket cleanare. Perfect för Slice 1!

Ska vi göra Pathway till core-komponenten i vår Room navigation? 🛤️🚀