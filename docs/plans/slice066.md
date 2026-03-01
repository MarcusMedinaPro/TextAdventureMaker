## Slice 66: Weather & Environment System

**Mål:** Dynamiskt vädersystem som påverkar gameplay, visibility och stämning.

**Referens:** `docs/plans/slice045.md` (weather idea)

### Task 66.1: IWeather Interface

```csharp
public interface IWeather
{
    string Id { get; }
    string Name { get; }
    string Description { get; }
    WeatherType Type { get; }
    WeatherIntensity Intensity { get; }

    // Effects
    float VisibilityModifier { get; }      // 0.0 = blind, 1.0 = normal
    float MovementModifier { get; }        // Speed multiplier
    float SoundModifier { get; }           // How far sound travels
    bool BlocksOutdoorTravel { get; }

    void OnStart(IGameState state);
    void OnTick(IGameState state);
    void OnEnd(IGameState state);
}

public enum WeatherType
{
    Clear,
    Cloudy,
    Rain,
    Storm,
    Snow,
    Fog,
    Wind,
    Hail,
    Sandstorm,
    Magical
}

public enum WeatherIntensity
{
    Light,
    Moderate,
    Heavy,
    Extreme
}
```

### Task 66.2: Weather Implementations

```csharp
public class RainWeather : IWeather
{
    public string Id => "rain";
    public string Name => "Rain";
    public WeatherType Type => WeatherType.Rain;
    public WeatherIntensity Intensity { get; init; } = WeatherIntensity.Moderate;

    public string Description => Intensity switch
    {
        WeatherIntensity.Light => "A light drizzle falls from the sky.",
        WeatherIntensity.Moderate => "Rain patters steadily on the ground.",
        WeatherIntensity.Heavy => "Heavy rain pours down, soaking everything.",
        WeatherIntensity.Extreme => "Torrential rain reduces visibility to near zero.",
        _ => "It's raining."
    };

    public float VisibilityModifier => Intensity switch
    {
        WeatherIntensity.Light => 0.9f,
        WeatherIntensity.Moderate => 0.7f,
        WeatherIntensity.Heavy => 0.4f,
        WeatherIntensity.Extreme => 0.1f,
        _ => 1.0f
    };

    public float MovementModifier => Intensity switch
    {
        WeatherIntensity.Light => 1.0f,
        WeatherIntensity.Moderate => 0.9f,
        WeatherIntensity.Heavy => 0.7f,
        WeatherIntensity.Extreme => 0.5f,
        _ => 1.0f
    };

    public float SoundModifier => Intensity switch
    {
        WeatherIntensity.Light => 0.9f,
        WeatherIntensity.Moderate => 0.7f,
        WeatherIntensity.Heavy => 0.5f,
        WeatherIntensity.Extreme => 0.3f,
        _ => 1.0f
    };

    public bool BlocksOutdoorTravel => Intensity == WeatherIntensity.Extreme;

    public void OnStart(IGameState state)
    {
        state.AddMessage("Rain begins to fall.");
    }

    public void OnTick(IGameState state)
    {
        // Outdoor locations get wet
        if (state.CurrentLocation.IsOutdoor)
        {
            state.CurrentLocation.SetProperty("wet", true);

            // Chance of slipping
            if (Intensity >= WeatherIntensity.Heavy && Random.Shared.NextDouble() < 0.1)
            {
                state.AddMessage("You slip on the wet ground!");
            }
        }
    }

    public void OnEnd(IGameState state)
    {
        state.AddMessage("The rain stops.");
    }
}

public class FogWeather : IWeather
{
    public string Id => "fog";
    public string Name => "Fog";
    public WeatherType Type => WeatherType.Fog;
    public WeatherIntensity Intensity { get; init; } = WeatherIntensity.Moderate;

    public string Description => Intensity switch
    {
        WeatherIntensity.Light => "A thin mist hangs in the air.",
        WeatherIntensity.Moderate => "Fog obscures distant objects.",
        WeatherIntensity.Heavy => "Thick fog surrounds you, limiting visibility.",
        WeatherIntensity.Extreme => "You can barely see your hand in front of your face.",
        _ => "It's foggy."
    };

    public float VisibilityModifier => Intensity switch
    {
        WeatherIntensity.Light => 0.7f,
        WeatherIntensity.Moderate => 0.4f,
        WeatherIntensity.Heavy => 0.2f,
        WeatherIntensity.Extreme => 0.05f,
        _ => 1.0f
    };

    public float MovementModifier => 1.0f;  // Fog doesn't slow movement
    public float SoundModifier => 1.2f;     // Sound travels oddly in fog
    public bool BlocksOutdoorTravel => false;

    public void OnStart(IGameState state)
    {
        state.AddMessage("Fog rolls in, obscuring your surroundings.");
    }

    public void OnTick(IGameState state)
    {
        // Can't see distant things
        if (Intensity >= WeatherIntensity.Heavy)
        {
            state.CurrentLocation.SetProperty("limited_visibility", true);
        }
    }

    public void OnEnd(IGameState state)
    {
        state.AddMessage("The fog lifts.");
        state.CurrentLocation.SetProperty("limited_visibility", false);
    }
}

public class StormWeather : IWeather
{
    public string Id => "storm";
    public string Name => "Storm";
    public WeatherType Type => WeatherType.Storm;
    public WeatherIntensity Intensity { get; init; } = WeatherIntensity.Heavy;

    public float VisibilityModifier => 0.2f;
    public float MovementModifier => 0.5f;
    public float SoundModifier => 0.1f;  // Thunder drowns everything
    public bool BlocksOutdoorTravel => true;

    private int _ticksSinceLastLightning = 0;

    public void OnTick(IGameState state)
    {
        _ticksSinceLastLightning++;

        // Random lightning
        if (_ticksSinceLastLightning >= 3 && Random.Shared.NextDouble() < 0.3)
        {
            _ticksSinceLastLightning = 0;
            state.AddMessage("Lightning flashes across the sky!");

            // Brief illumination
            if (state.CurrentLocation.IsOutdoor)
            {
                state.CurrentLocation.SetProperty("briefly_lit", true);
            }

            // Chance of striking something
            if (Random.Shared.NextDouble() < 0.05)
            {
                state.AddMessage("CRACK! Lightning strikes nearby!");
                state.Events.Trigger(new LightningStrikeEvent(state.CurrentLocation));
            }
        }
    }
}
```

### Task 66.3: WeatherManager

```csharp
public class WeatherManager
{
    private IWeather? _currentWeather;
    private readonly Queue<WeatherChange> _forecast = new();
    private int _weatherDuration = 0;

    public IWeather? CurrentWeather => _currentWeather;

    public void SetWeather(IWeather weather, int duration = -1)
    {
        _currentWeather?.OnEnd(GameState);
        _currentWeather = weather;
        _weatherDuration = duration;
        _currentWeather?.OnStart(GameState);
    }

    public void ClearWeather()
    {
        _currentWeather?.OnEnd(GameState);
        _currentWeather = null;
    }

    public void ScheduleWeather(IWeather weather, int ticksFromNow, int duration)
    {
        _forecast.Enqueue(new WeatherChange(weather, ticksFromNow, duration));
    }

    public void OnTick(IGameState state)
    {
        // Process current weather
        _currentWeather?.OnTick(state);

        // Check duration
        if (_weatherDuration > 0)
        {
            _weatherDuration--;
            if (_weatherDuration == 0)
            {
                ClearWeather();
            }
        }

        // Check forecast
        foreach (var forecast in _forecast)
        {
            forecast.TicksUntil--;
            if (forecast.TicksUntil <= 0)
            {
                SetWeather(forecast.Weather, forecast.Duration);
            }
        }

        // Remove triggered forecasts
        while (_forecast.Count > 0 && _forecast.Peek().TicksUntil <= 0)
        {
            _forecast.Dequeue();
        }
    }

    public string GetWeatherDescription()
    {
        if (_currentWeather == null)
            return "The weather is clear.";

        return _currentWeather.Description;
    }
}

public record WeatherChange(IWeather Weather, int TicksUntil, int Duration);
```

### Task 66.4: Location Weather Integration

```csharp
public static class LocationWeatherExtensions
{
    public static bool IsOutdoor(this ILocation location) =>
        location.HasProperty("outdoor") || !location.HasProperty("indoor");

    public static bool IsSheltered(this ILocation location) =>
        location.HasProperty("sheltered") || location.HasProperty("indoor");

    public static string GetWeatherAffectedDescription(this ILocation location, IWeather? weather)
    {
        var baseDescription = location.Description;

        if (weather == null || location.IsSheltered())
            return baseDescription;

        var weatherNote = weather.Type switch
        {
            WeatherType.Rain => " Rain falls steadily around you.",
            WeatherType.Storm => " The storm rages overhead.",
            WeatherType.Fog => " Fog swirls at your feet.",
            WeatherType.Snow => " Snow blankets the ground.",
            WeatherType.Wind => " The wind howls past you.",
            _ => ""
        };

        return baseDescription + weatherNote;
    }
}

// Builder extension
public static LocationBuilder Outdoor(this LocationBuilder builder) =>
    builder.WithProperty("outdoor", true);

public static LocationBuilder Indoor(this LocationBuilder builder) =>
    builder.WithProperty("indoor", true);

public static LocationBuilder Sheltered(this LocationBuilder builder) =>
    builder.WithProperty("sheltered", true);
```

### Task 66.5: Weather Commands

```csharp
public class WeatherCommand : ICommand
{
    public CommandResult Execute(CommandContext context)
    {
        var weather = context.State.Weather.CurrentWeather;
        var description = context.State.Weather.GetWeatherDescription();

        var sb = new StringBuilder();
        sb.AppendLine(description);

        if (weather != null)
        {
            sb.AppendLine($"Visibility: {weather.VisibilityModifier:P0}");

            if (weather.BlocksOutdoorTravel)
                sb.AppendLine("Outdoor travel is dangerous!");
        }

        return CommandResult.Ok(sb.ToString());
    }
}
```

### Task 66.6: GameBuilder Integration

```csharp
var game = new GameBuilder("Weather Demo")
    .WithWeatherSystem()
    .AddLocation("courtyard", loc => loc
        .Name("Courtyard")
        .Description("An open courtyard with a fountain.")
        .Outdoor())
    .AddLocation("hall", loc => loc
        .Name("Great Hall")
        .Description("A grand hall with high ceilings.")
        .Indoor())
    .AddWeatherEvent(evt => evt
        .AtTick(10)
        .SetWeather(new FogWeather { Intensity = WeatherIntensity.Light })
        .Duration(20))
    .AddWeatherEvent(evt => evt
        .AtTick(50)
        .SetWeather(new StormWeather())
        .Duration(30)
        .WithMessage("Dark clouds gather on the horizon..."))
    .Build();
```

### Task 66.7: Tester

```csharp
[Fact]
public void Rain_ReducesVisibility()
{
    var rain = new RainWeather { Intensity = WeatherIntensity.Heavy };

    Assert.Equal(0.4f, rain.VisibilityModifier);
}

[Fact]
public void Storm_BlocksOutdoorTravel()
{
    var storm = new StormWeather();

    Assert.True(storm.BlocksOutdoorTravel);
}

[Fact]
public void IndoorLocation_NotAffectedByWeather()
{
    var game = TestWorldBuilder.Create()
        .WithLocation("indoors", loc => loc.Indoor())
        .WithWeather(new StormWeather())
        .Build();

    // Should be able to see normally indoors
    Assert.False(game.State.CurrentLocation.GetProperty<bool>("limited_visibility"));
}
```

---
