## Slice 62: Countdown & Deadline Events

**Mål:** Tidsbegränsade utmaningar med nedräkning, deadlines och konsekvenser.

**Referens:** `docs/plans/imported/timedstuff.md`

### Task 62.1: ICountdown interface

```csharp
public interface ICountdown
{
    string Id { get; }
    string Name { get; }
    int RemainingTicks { get; }
    bool IsExpired { get; }
    bool IsPaused { get; }

    void Tick();
    void Pause();
    void Resume();
    void Reset();

    event Action<ICountdown>? OnTick;
    event Action<ICountdown>? OnExpired;
    event Action<ICountdown, int>? OnWarning;  // Vid vissa trösklar
}
```

### Task 62.2: Countdown implementation

```csharp
public class Countdown : ICountdown
{
    public string Id { get; init; } = "";
    public string Name { get; init; } = "";
    public int RemainingTicks { get; private set; }
    public bool IsExpired => RemainingTicks <= 0;
    public bool IsPaused { get; private set; }

    private readonly int _initialTicks;
    private readonly int[] _warningThresholds;

    public event Action<ICountdown>? OnTick;
    public event Action<ICountdown>? OnExpired;
    public event Action<ICountdown, int>? OnWarning;

    public Countdown(int ticks, params int[] warningAt)
    {
        _initialTicks = ticks;
        RemainingTicks = ticks;
        _warningThresholds = warningAt;
    }

    public void Tick()
    {
        if (IsPaused || IsExpired) return;

        RemainingTicks--;
        OnTick?.Invoke(this);

        if (_warningThresholds.Contains(RemainingTicks))
            OnWarning?.Invoke(this, RemainingTicks);

        if (IsExpired)
            OnExpired?.Invoke(this);
    }

    public void Pause() => IsPaused = true;
    public void Resume() => IsPaused = false;
    public void Reset() => RemainingTicks = _initialTicks;
}
```

### Task 62.3: DeadlineEvent

```csharp
public class DeadlineEvent
{
    public string Id { get; init; } = "";
    public string Description { get; init; } = "";
    public int Deadline { get; init; }  // Vilken turn
    public bool CanBePrevented { get; init; } = true;

    private readonly Action<IGameState> _onDeadline;
    private readonly Func<IGameState, bool>? _preventCondition;

    public DeadlineEvent(int deadline, Action<IGameState> onDeadline, Func<IGameState, bool>? preventIf = null)
    {
        Deadline = deadline;
        _onDeadline = onDeadline;
        _preventCondition = preventIf;
    }

    public void Check(IGameState state)
    {
        if (state.Turn < Deadline) return;

        // Kolla om spelaren förhindrat eventet
        if (_preventCondition != null && _preventCondition(state))
            return;

        _onDeadline(state);
    }
}

// Exempel:
// var flood = new DeadlineEvent(
//     deadline: 30,
//     onDeadline: state => state.CurrentLocation.SetProperty("flooded", true),
//     preventIf: state => state.HasFlag("dam_repaired")
// );
```

### Task 62.4: EscapeTimer

```csharp
public class EscapeTimer : ICountdown
{
    public string Id { get; init; } = "";
    public string Name { get; init; } = "Escape!";
    public int RemainingTicks { get; private set; }
    public bool IsExpired => RemainingTicks <= 0;
    public bool IsPaused { get; private set; }

    private readonly string _safeLocationId;
    private readonly Action<IGameState> _onFailure;

    public event Action<ICountdown>? OnTick;
    public event Action<ICountdown>? OnExpired;
    public event Action<ICountdown, int>? OnWarning;

    public EscapeTimer(int ticks, string safeLocation, Action<IGameState> onFailure)
    {
        RemainingTicks = ticks;
        _safeLocationId = safeLocation;
        _onFailure = onFailure;
    }

    public void Tick()
    {
        if (IsPaused || IsExpired) return;

        RemainingTicks--;
        OnTick?.Invoke(this);

        if (IsExpired)
            OnExpired?.Invoke(this);
    }

    public bool CheckEscaped(IGameState state)
    {
        if (state.CurrentLocation.Id == _safeLocationId)
        {
            IsPaused = true;  // Escaped!
            return true;
        }
        return false;
    }

    public void TriggerFailure(IGameState state)
    {
        _onFailure(state);
    }

    public void Pause() => IsPaused = true;
    public void Resume() => IsPaused = false;
    public void Reset() { }
}
```

### Task 62.5: CountdownDisplay

```csharp
public class CountdownDisplay
{
    public string GetWarningMessage(ICountdown countdown)
    {
        return countdown.RemainingTicks switch
        {
            <= 3 => $"⚠️ CRITICAL: {countdown.Name} - {countdown.RemainingTicks} turns remaining!",
            <= 5 => $"⚠️ WARNING: {countdown.Name} - {countdown.RemainingTicks} turns remaining!",
            <= 10 => $"⏰ {countdown.Name} - {countdown.RemainingTicks} turns remaining.",
            _ => $"{countdown.Name}: {countdown.RemainingTicks} turns."
        };
    }

    public string GetProgressBar(ICountdown countdown, int initialTicks)
    {
        var percent = (float)countdown.RemainingTicks / initialTicks;
        var filled = (int)(percent * 20);
        var empty = 20 - filled;

        var bar = new string('█', filled) + new string('░', empty);
        var color = percent switch
        {
            <= 0.2f => "red",
            <= 0.5f => "yellow",
            _ => "green"
        };

        return $"[{bar}] {countdown.RemainingTicks}";
    }
}
```

### Task 62.6: Timed Challenge Manager

```csharp
public class TimedChallengeManager
{
    private readonly List<ICountdown> _activeCountdowns = [];
    private readonly List<DeadlineEvent> _deadlines = [];

    public void AddCountdown(ICountdown countdown)
    {
        _activeCountdowns.Add(countdown);

        countdown.OnExpired += c =>
        {
            _activeCountdowns.Remove(c);
        };
    }

    public void AddDeadline(DeadlineEvent deadline)
    {
        _deadlines.Add(deadline);
    }

    public void OnTurnEnd(IGameState state)
    {
        // Tick alla countdowns
        foreach (var countdown in _activeCountdowns.ToList())
        {
            countdown.Tick();
        }

        // Kolla deadlines
        foreach (var deadline in _deadlines.ToList())
        {
            deadline.Check(state);
        }
    }

    public IEnumerable<string> GetActiveTimers()
    {
        var display = new CountdownDisplay();
        return _activeCountdowns.Select(c => display.GetWarningMessage(c));
    }
}
```

### Task 62.7: Room Transformation Events

```csharp
public class RoomTransformEvent
{
    public string LocationId { get; init; } = "";
    public int TriggerAtTurn { get; init; }
    public string NewState { get; init; } = "";
    public string Message { get; init; } = "";

    private readonly Dictionary<string, object> _propertyChanges = [];

    public RoomTransformEvent WithProperty(string key, object value)
    {
        _propertyChanges[key] = value;
        return this;
    }

    public void Apply(IGameState state)
    {
        var location = state.GetLocation(LocationId);
        if (location == null) return;

        foreach (var (key, value) in _propertyChanges)
        {
            location.SetProperty(key, value);
        }

        if (state.CurrentLocation.Id == LocationId)
        {
            // Spelaren är i rummet - visa meddelande
            state.AddMessage(Message);
        }
    }
}

// Exempel:
// new RoomTransformEvent
// {
//     LocationId = "sand_chamber",
//     TriggerAtTurn = 30,
//     NewState = "filled_with_sand",
//     Message = "The sand has now filled the entire chamber!"
// }.WithProperty("passable", false)
//  .WithProperty("description", "The chamber is completely filled with sand.");
```

### Task 62.8: Tester

```csharp
[Fact]
public void Countdown_ExpiresAtZero()
{
    var countdown = new Countdown(5);
    var expired = false;
    countdown.OnExpired += _ => expired = true;

    for (int i = 0; i < 5; i++)
        countdown.Tick();

    Assert.True(expired);
    Assert.True(countdown.IsExpired);
}

[Fact]
public void Countdown_TriggersWarningsAtThresholds()
{
    var countdown = new Countdown(10, warningAt: [5, 3, 1]);
    var warnings = new List<int>();
    countdown.OnWarning += (_, remaining) => warnings.Add(remaining);

    for (int i = 0; i < 10; i++)
        countdown.Tick();

    Assert.Contains(5, warnings);
    Assert.Contains(3, warnings);
    Assert.Contains(1, warnings);
}

[Fact]
public void DeadlineEvent_CanBePrevented()
{
    var triggered = false;
    var deadline = new DeadlineEvent(
        5,
        _ => triggered = true,
        state => state.HasFlag("prevented")
    );

    var state = CreateState();
    state.SetFlag("prevented");
    state.Turn = 5;

    deadline.Check(state);

    Assert.False(triggered);
}
```

### Task 62.9: Sandbox — tickande bomb

Demo med bomb som måste desarmeras innan tiden går ut.

---

## Completion Checklist
- [x] Core implementation for this slice is present in the engine.
- [x] Behaviour is covered by tests and/or deterministic validation paths.
- [x] Demo document: `62_The_Midnight_Deadline.md`.
- [x] Marked complete in project slice status.
