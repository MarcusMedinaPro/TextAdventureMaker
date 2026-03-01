## Slice 69: Light & Darkness System

**Mål:** Ljus och mörker som påverkar synlighet, beskrivningar och gameplay.

**Referens:** `docs/plans/slice052.md` (visibility), `docs/plans/slice068.md` (stealth)

### Task 69.1: ILightSource Interface

```csharp
public interface ILightSource : IItem
{
    LightType Type { get; }
    float Intensity { get; }          // 0.0 = no light, 1.0 = full brightness
    float Radius { get; }             // How far light reaches
    bool IsLit { get; }
    int? FuelRemaining { get; }       // null = infinite

    void Light();
    void Extinguish();
    void OnTick();
}

public enum LightType
{
    None,
    Ambient,      // General lighting
    Directional,  // Flashlight, focused
    Warm,         // Fire, candles
    Cold,         // Magic, moonlight
    Magical       // Special effects
}
```

### Task 69.2: LightSource Implementation

```csharp
public class LightSource : Item, ILightSource
{
    public LightType Type { get; init; } = LightType.Warm;
    public float Intensity { get; init; } = 1.0f;
    public float Radius { get; init; } = 1.0f;
    public bool IsLit { get; private set; }
    public int? FuelRemaining { get; private set; }
    public int? MaxFuel { get; init; }

    public event Action<ILightSource>? OnLightStateChanged;
    public event Action<ILightSource>? OnFuelDepleted;

    public void Light()
    {
        if (IsLit)
            return;

        if (FuelRemaining == 0)
        {
            OnFuelDepleted?.Invoke(this);
            return;
        }

        IsLit = true;
        OnLightStateChanged?.Invoke(this);
    }

    public void Extinguish()
    {
        if (!IsLit)
            return;

        IsLit = false;
        OnLightStateChanged?.Invoke(this);
    }

    public void OnTick()
    {
        if (!IsLit || FuelRemaining == null)
            return;

        FuelRemaining--;

        if (FuelRemaining <= 0)
        {
            FuelRemaining = 0;
            Extinguish();
            OnFuelDepleted?.Invoke(this);
        }
    }

    public void Refuel(int amount)
    {
        if (MaxFuel == null)
            return;

        FuelRemaining = Math.Min((FuelRemaining ?? 0) + amount, MaxFuel.Value);
    }
}
```

### Task 69.3: Common Light Sources

```csharp
public static class LightSources
{
    public static LightSource Torch() => new()
    {
        Id = "torch",
        Name = "Torch",
        Description = "A wooden torch wrapped in oil-soaked cloth.",
        Type = LightType.Warm,
        Intensity = 0.8f,
        Radius = 2.0f,
        MaxFuel = 50,
        FuelRemaining = 50
    };

    public static LightSource Lantern() => new()
    {
        Id = "lantern",
        Name = "Lantern",
        Description = "A brass lantern with a glass enclosure.",
        Type = LightType.Warm,
        Intensity = 1.0f,
        Radius = 3.0f,
        MaxFuel = 100,
        FuelRemaining = 100
    };

    public static LightSource Candle() => new()
    {
        Id = "candle",
        Name = "Candle",
        Description = "A simple wax candle.",
        Type = LightType.Warm,
        Intensity = 0.4f,
        Radius = 1.0f,
        MaxFuel = 30,
        FuelRemaining = 30
    };

    public static LightSource MagicOrb() => new()
    {
        Id = "magic_orb",
        Name = "Glowing Orb",
        Description = "A softly glowing magical sphere.",
        Type = LightType.Magical,
        Intensity = 0.7f,
        Radius = 2.5f,
        MaxFuel = null,  // Infinite
        FuelRemaining = null
    };

    public static LightSource Flashlight() => new()
    {
        Id = "flashlight",
        Name = "Flashlight",
        Description = "A battery-powered electric torch.",
        Type = LightType.Directional,
        Intensity = 1.0f,
        Radius = 4.0f,
        MaxFuel = 200,
        FuelRemaining = 200
    };
}
```

### Task 69.4: Location Lighting

```csharp
public enum LightingLevel
{
    PitchBlack,     // 0.0 - Can't see anything
    VeryDark,       // 0.1-0.2 - Vague shapes only
    Dark,           // 0.2-0.4 - Can see nearby objects
    Dim,            // 0.4-0.6 - Reduced visibility
    Normal,         // 0.6-0.8 - Standard lighting
    Bright,         // 0.8-1.0 - Well lit
    Blinding        // > 1.0 - Too bright to see
}

public static class LocationLightingExtensions
{
    public static LightingLevel GetLightingLevel(this ILocation location, IGameState state)
    {
        var baseLighting = location.GetProperty<float>("base_lighting", 0.0f);

        // Add ambient light sources in room
        var roomLights = location.Items
            .OfType<ILightSource>()
            .Where(l => l.IsLit)
            .Sum(l => l.Intensity * 0.5f);

        // Add player's light source
        var playerLight = state.Player.Inventory.Items
            .OfType<ILightSource>()
            .Where(l => l.IsLit)
            .Sum(l => l.Intensity);

        // Time of day affects outdoor locations
        if (location.IsOutdoor())
        {
            baseLighting += state.TimeSystem.GetDaylightLevel();
        }

        var totalLight = baseLighting + roomLights + playerLight;

        return totalLight switch
        {
            <= 0.0f => LightingLevel.PitchBlack,
            <= 0.2f => LightingLevel.VeryDark,
            <= 0.4f => LightingLevel.Dark,
            <= 0.6f => LightingLevel.Dim,
            <= 0.8f => LightingLevel.Normal,
            <= 1.0f => LightingLevel.Bright,
            _ => LightingLevel.Blinding
        };
    }

    public static bool IsDark(this ILocation location, IGameState state) =>
        location.GetLightingLevel(state) <= LightingLevel.Dark;

    public static bool CanSee(this ILocation location, IGameState state) =>
        location.GetLightingLevel(state) >= LightingLevel.Dim;

    public static string GetLightingDescription(this ILocation location, IGameState state)
    {
        return location.GetLightingLevel(state) switch
        {
            LightingLevel.PitchBlack => "It is pitch black. You can't see a thing.",
            LightingLevel.VeryDark => "It is very dark. You can barely make out vague shapes.",
            LightingLevel.Dark => "It is dark. You can see nearby objects dimly.",
            LightingLevel.Dim => "The light is dim here.",
            LightingLevel.Normal => "",  // Don't mention normal lighting
            LightingLevel.Bright => "The area is brightly lit.",
            LightingLevel.Blinding => "The light is blindingly bright!",
            _ => ""
        };
    }
}
```

### Task 69.5: Darkness Effects on Commands

```csharp
public class DarknessAwareLookCommand : ICommand
{
    public CommandResult Execute(CommandContext context)
    {
        var location = context.State.CurrentLocation;
        var lighting = location.GetLightingLevel(context.State);

        var sb = new StringBuilder();

        // Lighting description first
        var lightDesc = location.GetLightingDescription(context.State);
        if (!string.IsNullOrEmpty(lightDesc))
            sb.AppendLine(lightDesc);

        // Can't see in pitch black
        if (lighting == LightingLevel.PitchBlack)
            return CommandResult.Ok(sb.ToString());

        // Room name
        sb.AppendLine($"**{location.Name}**");

        // Room description (possibly truncated in darkness)
        if (lighting <= LightingLevel.VeryDark)
        {
            sb.AppendLine("You can barely make out your surroundings...");
        }
        else
        {
            sb.AppendLine(location.Description);
        }

        // Items (only visible if enough light)
        if (lighting >= LightingLevel.Dim)
        {
            var visibleItems = location.Items
                .Where(i => !i.IsHidden)
                .ToList();

            if (visibleItems.Any())
            {
                sb.AppendLine();
                sb.AppendLine("You see:");
                foreach (var item in visibleItems)
                    sb.AppendLine($"  - {item.Name}");
            }
        }
        else if (lighting >= LightingLevel.Dark)
        {
            // Can only see large/obvious items
            var obviousItems = location.Items
                .Where(i => i.HasProperty("large") || i.HasProperty("glowing"))
                .ToList();

            if (obviousItems.Any())
            {
                sb.AppendLine("You can make out:");
                foreach (var item in obviousItems)
                    sb.AppendLine($"  - {item.Name}");
            }
        }

        // Exits (always show, but might be vague)
        if (lighting >= LightingLevel.Dim)
        {
            var exits = location.Exits.Keys;
            if (exits.Any())
                sb.AppendLine($"\nExits: {string.Join(", ", exits)}");
        }
        else
        {
            sb.AppendLine("\nYou're not sure where the exits are.");
        }

        return CommandResult.Ok(sb.ToString());
    }
}

public class DarknessAwareTakeCommand : ICommand
{
    public CommandResult Execute(CommandContext context, string itemName)
    {
        var lighting = context.State.CurrentLocation.GetLightingLevel(context.State);

        if (lighting <= LightingLevel.VeryDark)
        {
            // Fumbling in the dark - chance of failure
            if (Random.Shared.NextDouble() < 0.5)
                return CommandResult.Fail("You fumble in the darkness but can't find anything.");
        }

        // Normal take logic
        var item = context.State.CurrentLocation.FindItem(itemName);
        if (item == null)
        {
            return lighting <= LightingLevel.Dark
                ? CommandResult.Fail("You can't find that in the darkness.")
                : CommandResult.Fail($"There's no {itemName} here.");
        }

        context.State.Player.Inventory.Add(item);
        context.State.CurrentLocation.Items.Remove(item);

        return CommandResult.Ok($"You take the {item.Name}.");
    }
}
```

### Task 69.6: Light Commands

```csharp
public class LightCommand : ICommand
{
    public CommandResult Execute(CommandContext context, string itemName)
    {
        var item = context.State.Player.Inventory.FindItem(itemName)
            ?? context.State.CurrentLocation.FindItem(itemName);

        if (item is not ILightSource lightSource)
            return CommandResult.Fail($"You can't light {itemName}.");

        if (lightSource.IsLit)
            return CommandResult.Fail($"The {item.Name} is already lit.");

        if (lightSource.FuelRemaining == 0)
            return CommandResult.Fail($"The {item.Name} has no fuel.");

        lightSource.Light();
        return CommandResult.Ok($"You light the {item.Name}. It casts a warm glow.");
    }
}

public class ExtinguishCommand : ICommand
{
    public CommandResult Execute(CommandContext context, string itemName)
    {
        var item = context.State.Player.Inventory.FindItem(itemName)
            ?? context.State.CurrentLocation.FindItem(itemName);

        if (item is not ILightSource lightSource)
            return CommandResult.Fail($"You can't extinguish {itemName}.");

        if (!lightSource.IsLit)
            return CommandResult.Fail($"The {item.Name} isn't lit.");

        lightSource.Extinguish();
        return CommandResult.Ok($"You extinguish the {item.Name}.");
    }
}

public class RefuelCommand : ICommand
{
    public CommandResult Execute(CommandContext context, string lightName, string fuelName)
    {
        var light = context.State.Player.Inventory.FindItem(lightName) as LightSource;
        if (light == null)
            return CommandResult.Fail($"You don't have a {lightName}.");

        var fuel = context.State.Player.Inventory.FindItem(fuelName);
        if (fuel == null)
            return CommandResult.Fail($"You don't have any {fuelName}.");

        var fuelAmount = fuel.GetProperty<int>("fuel_amount", 10);
        light.Refuel(fuelAmount);

        context.State.Player.Inventory.Remove(fuel);

        return CommandResult.Ok($"You refuel the {light.Name}. Fuel: {light.FuelRemaining}/{light.MaxFuel}");
    }
}
```

### Task 69.7: GameBuilder Integration

```csharp
var game = new GameBuilder("Darkness Demo")
    .WithLightingSystem()
    .AddLocation("entrance", loc => loc
        .Name("Cave Entrance")
        .Description("Daylight streams in from outside.")
        .BaseLighting(0.8f)
        .Outdoor()
        .AddExit(Direction.North, "dark_tunnel"))
    .AddLocation("dark_tunnel", loc => loc
        .Name("Dark Tunnel")
        .Description("A narrow tunnel stretches into darkness.")
        .BaseLighting(0.0f)  // Pitch black
        .AddExit(Direction.South, "entrance")
        .AddExit(Direction.North, "chamber"))
    .AddLocation("chamber", loc => loc
        .Name("Underground Chamber")
        .Description("A large chamber with strange markings on the walls.")
        .BaseLighting(0.1f)  // Very dark
        .AddItem(LightSources.MagicOrb())
        .AddExit(Direction.South, "dark_tunnel"))
    .AddItem("torch", LightSources.Torch())
    .AddItem("oil_flask", item => item
        .Name("Oil Flask")
        .Description("A flask of lamp oil.")
        .WithProperty("fuel_amount", 25))
    .StartAt("entrance")
    .WithStartingItem("torch")
    .Build();
```

### Task 69.8: Tester

```csharp
[Fact]
public void Torch_CanBeLit()
{
    var torch = LightSources.Torch();

    Assert.False(torch.IsLit);

    torch.Light();

    Assert.True(torch.IsLit);
}

[Fact]
public void Torch_ConsumesFuel()
{
    var torch = LightSources.Torch();
    var initialFuel = torch.FuelRemaining;

    torch.Light();
    torch.OnTick();

    Assert.Equal(initialFuel - 1, torch.FuelRemaining);
}

[Fact]
public void Torch_ExtinguishesWhenFuelDepleted()
{
    var torch = new LightSource
    {
        MaxFuel = 1,
        FuelRemaining = 1
    };

    torch.Light();
    torch.OnTick();

    Assert.False(torch.IsLit);
    Assert.Equal(0, torch.FuelRemaining);
}

[Fact]
public void DarkLocation_RequiresLightToSee()
{
    var game = TestWorldBuilder.Create()
        .WithLocation("dark_room", loc => loc.BaseLighting(0.0f))
        .StartAt("dark_room")
        .Build();

    Assert.True(game.State.CurrentLocation.IsDark(game.State));
    Assert.False(game.State.CurrentLocation.CanSee(game.State));
}

[Fact]
public void LitTorch_IlluminatesDarkRoom()
{
    var game = TestWorldBuilder.Create()
        .WithLocation("dark_room", loc => loc.BaseLighting(0.0f))
        .StartAt("dark_room")
        .Build();

    var torch = LightSources.Torch();
    torch.Light();
    game.State.Player.Inventory.Add(torch);

    Assert.False(game.State.CurrentLocation.IsDark(game.State));
    Assert.True(game.State.CurrentLocation.CanSee(game.State));
}
```

---
