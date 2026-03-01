## Slice 52: Spatial Awareness System

**Mål:** Komplett rumsmedvetenhet - spelaren kan interagera med och uppfatta angränsande rum som en sammanhängande värld.

**Referens:** `docs/plans/imported/Spatial_Awareness_Revolution.md`

### Koncept

Traditionella textäventyr: rum är isolerade containrar.
Spatial Awareness: rum är delar av en sammanhängande värld med sikt, ljud och interaktion.

### Task 52.1: ISpatialContext interface

```csharp
public interface ISpatialContext
{
    ILocation CurrentLocation { get; }
    IEnumerable<AdjacentRoom> GetAdjacentRooms();
    IEnumerable<IItem> GetVisibleItems(int range = 1);
    IEnumerable<INpc> GetAudibleNpcs(int range = 2);
    bool CanSee(ILocation target);
    bool CanHear(ILocation target);
}

public record AdjacentRoom(
    Direction Direction,
    ILocation Location,
    float Visibility,      // 0.0 = kan inte se, 1.0 = full sikt
    float Audibility,      // 0.0 = ljudisolerat, 1.0 = hör allt
    string? BlockedBy      // "closed door", "stone wall", etc.
);
```

### Task 52.2: Visibility-beräkning

```csharp
public static class VisibilityCalculator
{
    public static float Calculate(Exit exit)
    {
        if (exit.Door == null)
            return 1.0f;  // Öppen passage

        return exit.Door.State switch
        {
            DoorState.Open => 1.0f,
            DoorState.Closed => exit.Door.GetProperty<bool>("transparent", false) ? 0.5f : 0.0f,
            DoorState.Locked => exit.Door.GetProperty<bool>("transparent", false) ? 0.3f : 0.0f,
            _ => 0.0f
        };
    }
}
```

### Task 52.3: Audibility-beräkning

```csharp
public static class AudibilityCalculator
{
    public static float Calculate(Exit exit)
    {
        if (exit.Door == null)
            return 1.0f;

        var soundproofing = exit.Door.GetProperty<float>("soundproofing", 0.5f);

        return exit.Door.State switch
        {
            DoorState.Open => 1.0f,
            DoorState.Closed => 1.0f - soundproofing,
            DoorState.Locked => 1.0f - soundproofing,
            _ => 0.0f
        };
    }
}
```

### Task 52.4: Enhanced Look med spatial context

```csharp
public static string GetSpatialDescription(ISpatialContext context)
{
    var sb = new StringBuilder();
    sb.AppendLine(context.CurrentLocation.GetDescription());

    foreach (var adj in context.GetAdjacentRooms().Where(a => a.Visibility > 0.3f))
    {
        var glimpse = adj.Visibility switch
        {
            >= 0.8f => $"To the {adj.Direction}, you clearly see {adj.Location.Name}.",
            >= 0.5f => $"Through the {adj.Direction}, you glimpse {adj.Location.Name}.",
            _ => $"To the {adj.Direction}, you can barely make out a room."
        };
        sb.AppendLine(glimpse);
    }

    // Ljud från angränsande rum
    foreach (var adj in context.GetAdjacentRooms().Where(a => a.Audibility > 0.5f))
    {
        var sounds = adj.Location.GetProperty<string>("ambient_sound");
        if (!string.IsNullOrEmpty(sounds))
            sb.AppendLine($"From the {adj.Direction}, you hear {sounds}.");
    }

    return sb.ToString();
}
```

### Task 52.5: Multi-room events

```csharp
// Events som sprider sig mellan rum
public class SpatialEventSystem
{
    public void PropagateEvent(string eventName, ILocation origin, int range = 1)
    {
        var visited = new HashSet<ILocation> { origin };
        var queue = new Queue<(ILocation loc, int dist)>();
        queue.Enqueue((origin, 0));

        while (queue.Count > 0)
        {
            var (loc, dist) = queue.Dequeue();
            if (dist > range) continue;

            loc.RaiseEvent(eventName, new { Distance = dist, Origin = origin });

            foreach (var exit in loc.Exits.Values)
            {
                if (!visited.Contains(exit.Target))
                {
                    visited.Add(exit.Target);
                    queue.Enqueue((exit.Target, dist + 1));
                }
            }
        }
    }
}
```

### Task 52.6: NPC-awareness

```csharp
// NPCs kan reagera på saker i angränsande rum
public class SpatialAwareNpc : Npc
{
    public override void OnTick(GameContext context)
    {
        var spatial = context.GetSpatialContext();

        // Hör spelaren i närheten?
        if (spatial.GetAdjacentRooms().Any(r => r.Location == context.State.CurrentLocation && r.Audibility > 0.5f))
        {
            SetProperty("heard_player", true);
        }

        // Ser spelaren?
        if (spatial.GetAdjacentRooms().Any(r => r.Location == context.State.CurrentLocation && r.Visibility > 0.5f))
        {
            SetProperty("saw_player", true);
        }
    }
}
```

### Task 52.7: Tester

```csharp
[Fact]
public void SpatialContext_WithOpenDoor_HasFullVisibility()
{
    var game = CreateGame();
    var context = game.GetSpatialContext();

    var north = context.GetAdjacentRooms().First(r => r.Direction == Direction.North);

    Assert.Equal(1.0f, north.Visibility);
}

[Fact]
public void SpatialContext_WithClosedGlassDoor_HasPartialVisibility()
{
    var game = CreateGame();
    var door = game.State.CurrentLocation.Exits[Direction.North].Door;
    door.State = DoorState.Closed;
    door.SetProperty("transparent", true);

    var context = game.GetSpatialContext();
    var north = context.GetAdjacentRooms().First(r => r.Direction == Direction.North);

    Assert.Equal(0.5f, north.Visibility);
}
```

### Task 52.8: Sandbox — spökhus med ljud och skuggor

Demo med spatial awareness där spelaren hör fotsteg från andra rum och ser skuggor genom springor.

---
