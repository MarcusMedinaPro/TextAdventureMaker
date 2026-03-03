## Slice 68: Stealth & Detection System

**Mål:** Smyg- och upptäcktssystem för att passera NPCs utan att bli sedd.

**Referens:** `docs/plans/slice045.md` (sneak idea)

### Task 68.1: IDetectable Interface

```csharp
public interface IDetectable
{
    float Visibility { get; }         // 0.0 = invisible, 1.0 = fully visible
    float NoiseLevel { get; }         // 0.0 = silent, 1.0 = loud
    bool IsHidden { get; }
    bool IsSneaking { get; }

    void StartSneaking();
    void StopSneaking();
    void Hide(IHidingSpot spot);
    void Unhide();
}

public interface IHidingSpot
{
    string Id { get; }
    string Name { get; }
    float CoverQuality { get; }       // 0.0 = no cover, 1.0 = perfect cover
    int Capacity { get; }             // How many can hide here
    bool IsOccupied { get; }
}
```

### Task 68.2: Detection System

```csharp
public class DetectionSystem
{
    public float BaseDetectionRange { get; init; } = 10f;
    public float NoiseMultiplier { get; init; } = 2f;

    public bool CanDetect(INpc observer, IDetectable target, ILocation location)
    {
        if (target.IsHidden)
            return TryDetectHidden(observer, target, location);

        var distance = CalculateDistance(observer, target);
        var effectiveRange = BaseDetectionRange * observer.Perception;

        // Visibility check
        var visibilityThreshold = target.Visibility * GetLightingModifier(location);
        if (distance > effectiveRange * visibilityThreshold)
            return false;

        // Noise check (NPCs can hear even if they can't see)
        var noiseRange = target.NoiseLevel * NoiseMultiplier * BaseDetectionRange;
        if (distance <= noiseRange)
            return true;

        return visibilityThreshold > 0.5f;
    }

    private bool TryDetectHidden(INpc observer, IDetectable target, ILocation location)
    {
        var spot = target.CurrentHidingSpot;
        if (spot == null)
            return false;

        // Check against cover quality
        var detectionChance = (1f - spot.CoverQuality) * observer.Perception;

        // Search action increases detection
        if (observer.IsSearching)
            detectionChance *= 2f;

        return Random.Shared.NextDouble() < detectionChance;
    }

    private float GetLightingModifier(ILocation location)
    {
        if (location.IsDark())
            return 0.2f;

        if (location.HasProperty("dim_light"))
            return 0.6f;

        return 1.0f;
    }
}
```

### Task 68.3: Player Stealth Implementation

```csharp
public class StealthComponent : IDetectable
{
    private readonly ICharacter _owner;
    private IHidingSpot? _hidingSpot;

    public float BaseVisibility { get; init; } = 1.0f;
    public float BaseNoiseLevel { get; init; } = 0.5f;

    public float Visibility
    {
        get
        {
            var vis = BaseVisibility;

            if (IsSneaking)
                vis *= 0.5f;

            if (IsHidden)
                vis *= (1f - _hidingSpot!.CoverQuality);

            // Equipment modifiers
            if (_owner.HasEquipped("dark_cloak"))
                vis *= 0.7f;

            if (_owner.HasEquipped("boots_of_silence"))
                vis *= 0.9f;

            return Math.Clamp(vis, 0f, 1f);
        }
    }

    public float NoiseLevel
    {
        get
        {
            var noise = BaseNoiseLevel;

            if (IsSneaking)
                noise *= 0.3f;

            if (IsHidden)
                noise *= 0.1f;

            // Equipment modifiers
            if (_owner.HasEquipped("heavy_armor"))
                noise *= 1.5f;

            if (_owner.HasEquipped("boots_of_silence"))
                noise *= 0.2f;

            // Running is louder
            if (_owner.IsRunning)
                noise *= 2f;

            return Math.Clamp(noise, 0f, 1f);
        }
    }

    public bool IsHidden => _hidingSpot != null;
    public bool IsSneaking { get; private set; }
    public IHidingSpot? CurrentHidingSpot => _hidingSpot;

    public void StartSneaking()
    {
        IsSneaking = true;
        _owner.AddMessage("You begin to move quietly.");
    }

    public void StopSneaking()
    {
        IsSneaking = false;
        _owner.AddMessage("You stop sneaking.");
    }

    public void Hide(IHidingSpot spot)
    {
        if (spot.IsOccupied)
        {
            _owner.AddMessage($"The {spot.Name} is already occupied.");
            return;
        }

        _hidingSpot = spot;
        _owner.AddMessage($"You hide behind the {spot.Name}.");
    }

    public void Unhide()
    {
        if (_hidingSpot == null)
        {
            _owner.AddMessage("You're not hiding.");
            return;
        }

        _owner.AddMessage($"You emerge from behind the {_hidingSpot.Name}.");
        _hidingSpot = null;
    }
}
```

### Task 68.4: Hiding Spots

```csharp
public class HidingSpot : IHidingSpot
{
    public string Id { get; init; } = "";
    public string Name { get; init; } = "";
    public float CoverQuality { get; init; } = 0.5f;
    public int Capacity { get; init; } = 1;

    private readonly List<IDetectable> _occupants = [];
    public bool IsOccupied => _occupants.Count >= Capacity;

    public void Enter(IDetectable target)
    {
        if (!IsOccupied)
            _occupants.Add(target);
    }

    public void Leave(IDetectable target)
    {
        _occupants.Remove(target);
    }
}

public static class HidingSpots
{
    public static HidingSpot Bushes() => new()
    {
        Id = "bushes",
        Name = "thick bushes",
        CoverQuality = 0.7f,
        Capacity = 2
    };

    public static HidingSpot Barrel() => new()
    {
        Id = "barrel",
        Name = "barrel",
        CoverQuality = 0.9f,
        Capacity = 1
    };

    public static HidingSpot Shadows() => new()
    {
        Id = "shadows",
        Name = "shadows",
        CoverQuality = 0.5f,
        Capacity = 3
    };

    public static HidingSpot Curtain() => new()
    {
        Id = "curtain",
        Name = "curtain",
        CoverQuality = 0.6f,
        Capacity = 1
    };

    public static HidingSpot UnderBed() => new()
    {
        Id = "under_bed",
        Name = "space under the bed",
        CoverQuality = 0.8f,
        Capacity = 1
    };
}
```

### Task 68.5: NPC Detection Behaviour

```csharp
public class GuardBehaviour : INpcBehaviour
{
    private readonly DetectionSystem _detection = new();

    public AlertLevel AlertLevel { get; private set; } = AlertLevel.Relaxed;
    public IDetectable? LastSeenTarget { get; private set; }
    public ILocation? LastSeenLocation { get; private set; }

    public void OnTick(INpc npc, IGameState state)
    {
        var player = state.Player as IDetectable;
        if (player == null)
            return;

        var detected = _detection.CanDetect(npc, player, state.CurrentLocation);

        if (detected)
        {
            OnPlayerDetected(npc, player, state);
        }
        else if (AlertLevel > AlertLevel.Relaxed)
        {
            // Gradually calm down
            DecreaseAlertLevel(npc);
        }
    }

    private void OnPlayerDetected(INpc npc, IDetectable player, IGameState state)
    {
        LastSeenTarget = player;
        LastSeenLocation = state.CurrentLocation;

        switch (AlertLevel)
        {
            case AlertLevel.Relaxed:
                AlertLevel = AlertLevel.Suspicious;
                npc.Say("Hm? Did I hear something?");
                break;

            case AlertLevel.Suspicious:
                AlertLevel = AlertLevel.Alerted;
                npc.Say("Who's there?!");
                break;

            case AlertLevel.Alerted:
                AlertLevel = AlertLevel.Hostile;
                npc.Say("Intruder! Stop right there!");
                state.Events.Trigger(new PlayerDetectedEvent(npc, state.CurrentLocation));
                break;
        }
    }

    private void DecreaseAlertLevel(INpc npc)
    {
        AlertLevel = AlertLevel switch
        {
            AlertLevel.Hostile => AlertLevel.Alerted,
            AlertLevel.Alerted => AlertLevel.Suspicious,
            AlertLevel.Suspicious => AlertLevel.Relaxed,
            _ => AlertLevel.Relaxed
        };

        if (AlertLevel == AlertLevel.Relaxed)
            npc.Say("Must have been my imagination...");
    }
}

public enum AlertLevel
{
    Relaxed,      // Normal behaviour
    Suspicious,   // Looking around
    Alerted,      // Actively searching
    Hostile       // Combat/chase mode
}
```

### Task 68.6: Stealth Commands

```csharp
public class SneakCommand : ICommand
{
    public CommandResult Execute(CommandContext context)
    {
        var stealth = context.State.Player.Stealth;

        if (stealth.IsSneaking)
        {
            stealth.StopSneaking();
            return CommandResult.Ok("You stop sneaking.");
        }

        stealth.StartSneaking();
        return CommandResult.Ok("You begin moving quietly.");
    }
}

public class HideCommand : ICommand
{
    public CommandResult Execute(CommandContext context, string? spotName = null)
    {
        var stealth = context.State.Player.Stealth;

        if (stealth.IsHidden)
        {
            stealth.Unhide();
            return CommandResult.Ok("You emerge from hiding.");
        }

        // Find hiding spot
        var spots = context.State.CurrentLocation.HidingSpots;

        if (!spots.Any())
            return CommandResult.Fail("There's nowhere to hide here.");

        IHidingSpot spot;
        if (spotName == null)
        {
            spot = spots.FirstOrDefault(s => !s.IsOccupied)
                ?? throw new InvalidOperationException("All hiding spots are occupied.");
        }
        else
        {
            spot = spots.FirstOrDefault(s =>
                s.Name.Contains(spotName, StringComparison.OrdinalIgnoreCase))
                ?? throw new InvalidOperationException($"No hiding spot called '{spotName}'.");
        }

        if (spot.IsOccupied)
            return CommandResult.Fail($"The {spot.Name} is already occupied.");

        stealth.Hide(spot);
        return CommandResult.Ok($"You hide behind the {spot.Name}.");
    }
}

public class CrawlCommand : ICommand
{
    public CommandResult Execute(CommandContext context, Direction direction)
    {
        var stealth = context.State.Player.Stealth;

        // Force sneaking while crawling
        var wasSneaking = stealth.IsSneaking;
        stealth.StartSneaking();

        // Move with reduced noise
        var result = context.State.MovePlayer(direction);

        if (!wasSneaking)
            stealth.StopSneaking();

        if (result.Success)
            return CommandResult.Ok($"You crawl quietly to the {direction}.");

        return result;
    }
}
```

### Task 68.7: Location Extensions

```csharp
public static class LocationStealthExtensions
{
    public static LocationBuilder AddHidingSpot(this LocationBuilder builder, IHidingSpot spot)
    {
        builder.Location.HidingSpots.Add(spot);
        return builder;
    }

    public static LocationBuilder WithBushes(this LocationBuilder builder) =>
        builder.AddHidingSpot(HidingSpots.Bushes());

    public static LocationBuilder WithBarrel(this LocationBuilder builder) =>
        builder.AddHidingSpot(HidingSpots.Barrel());

    public static LocationBuilder WithShadows(this LocationBuilder builder) =>
        builder.AddHidingSpot(HidingSpots.Shadows());
}
```

### Task 68.8: GameBuilder Integration

```csharp
var game = new GameBuilder("Stealth Demo")
    .WithStealthSystem()
    .AddLocation("courtyard", loc => loc
        .Name("Castle Courtyard")
        .Description("Guards patrol this open area.")
        .WithBushes()
        .AddNpc("guard", npc => npc
            .Name("Castle Guard")
            .WithBehaviour(new GuardBehaviour())
            .Perception(0.7f)))
    .AddLocation("warehouse", loc => loc
        .Name("Warehouse")
        .Description("Crates and barrels line the walls.")
        .WithBarrel()
        .WithShadows()
        .AddNpc("watchman", npc => npc
            .Name("Night Watchman")
            .WithBehaviour(new GuardBehaviour())
            .Perception(0.5f)))
    .AddItem("dark_cloak", item => item
        .Name("Dark Cloak")
        .Description("A cloak that helps you blend into shadows.")
        .IsEquippable()
        .WithProperty("visibility_modifier", 0.7f))
    .AddItem("boots_of_silence", item => item
        .Name("Boots of Silence")
        .Description("Magical boots that muffle your footsteps.")
        .IsEquippable()
        .WithProperty("noise_modifier", 0.2f))
    .Build();
```

### Task 68.9: Tester

```csharp
[Fact]
public void Sneaking_ReducesVisibility()
{
    var player = CreatePlayer();
    var stealth = new StealthComponent(player);

    var normalVisibility = stealth.Visibility;
    stealth.StartSneaking();
    var sneakingVisibility = stealth.Visibility;

    Assert.True(sneakingVisibility < normalVisibility);
}

[Fact]
public void Hiding_ReducesVisibilityBasedOnCover()
{
    var player = CreatePlayer();
    var stealth = new StealthComponent(player);
    var barrel = HidingSpots.Barrel(); // 0.9 cover quality

    stealth.Hide(barrel);

    Assert.True(stealth.Visibility < 0.2f);
}

[Fact]
public void Guard_DetectsLoudPlayer()
{
    var guard = CreateGuard(perception: 1.0f);
    var player = CreatePlayer();
    var detection = new DetectionSystem();

    player.IsRunning = true; // Loud!

    var detected = detection.CanDetect(guard, player.Stealth, CreateLocation());

    Assert.True(detected);
}

[Fact]
public void Guard_MissesSneakingPlayer()
{
    var guard = CreateGuard(perception: 0.3f);
    var player = CreatePlayer();
    var detection = new DetectionSystem();

    player.Stealth.StartSneaking();
    player.Stealth.Hide(HidingSpots.Barrel());

    var detected = detection.CanDetect(guard, player.Stealth, CreateDarkLocation());

    Assert.False(detected);
}

[Fact]
public void AlertLevel_IncreasesOnRepeatedDetection()
{
    var behaviour = new GuardBehaviour();
    var guard = CreateGuard();
    var state = CreateGameState();

    // First detection
    behaviour.OnTick(guard, state);
    Assert.Equal(AlertLevel.Suspicious, behaviour.AlertLevel);

    // Second detection
    behaviour.OnTick(guard, state);
    Assert.Equal(AlertLevel.Alerted, behaviour.AlertLevel);
}
```

---

## Completion Checklist
- [x] Core implementation for this slice is present in the engine.
- [x] Behaviour is covered by tests and/or deterministic validation paths.
- [x] Demo document: `68_Shadow_in_the_Storage_Room.md`.
- [x] Marked complete in project slice status.
