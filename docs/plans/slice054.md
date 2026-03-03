## Slice 54: Player History System

**Mål:** Spåra spelarens fullständiga resa genom spelet för storytelling, achievements och analytics.

**Referens:** `docs/plans/imported/Player_History_System.md`

### Task 54.1: IPlayerHistory interface

```csharp
public interface IPlayerHistory
{
    IReadOnlyList<HistoryEntry> Entries { get; }

    void Record(HistoryEventType type, object data);

    IEnumerable<HistoryEntry> GetByType(HistoryEventType type);
    IEnumerable<ILocation> GetVisitedLocations();
    IEnumerable<INpc> GetMetNpcs();
    IEnumerable<IItem> GetAcquiredItems();
    TimeSpan GetTotalPlayTime();
    int GetCommandCount();

    string GenerateSummary();
}

public record HistoryEntry(
    DateTime Timestamp,
    int Turn,
    HistoryEventType Type,
    string Description,
    object? Data
);

public enum HistoryEventType
{
    LocationVisited,
    ItemAcquired,
    ItemDropped,
    ItemUsed,
    NpcMet,
    NpcTalked,
    NpcKilled,
    QuestStarted,
    QuestCompleted,
    QuestFailed,
    CombatStarted,
    CombatEnded,
    DoorOpened,
    DoorLocked,
    Achievement,
    Death,
    Custom
}
```

### Task 54.2: PlayerHistory implementation

```csharp
public class PlayerHistory : IPlayerHistory
{
    private readonly List<HistoryEntry> _entries = [];
    private readonly DateTime _startTime = DateTime.Now;
    private int _turn = 0;

    public IReadOnlyList<HistoryEntry> Entries => _entries.AsReadOnly();

    public void Record(HistoryEventType type, object data)
    {
        var description = GenerateDescription(type, data);
        _entries.Add(new HistoryEntry(DateTime.Now, _turn, type, description, data));
    }

    public void IncrementTurn() => _turn++;

    public IEnumerable<ILocation> GetVisitedLocations() =>
        _entries
            .Where(e => e.Type == HistoryEventType.LocationVisited)
            .Select(e => e.Data)
            .OfType<ILocation>()
            .Distinct();

    public int GetCommandCount() => _turn;

    public TimeSpan GetTotalPlayTime() => DateTime.Now - _startTime;

    private static string GenerateDescription(HistoryEventType type, object data) => type switch
    {
        HistoryEventType.LocationVisited when data is ILocation loc => $"Visited {loc.Name}",
        HistoryEventType.ItemAcquired when data is IItem item => $"Acquired {item.Name}",
        HistoryEventType.NpcMet when data is INpc npc => $"Met {npc.Name}",
        HistoryEventType.QuestCompleted when data is IQuest quest => $"Completed quest: {quest.Name}",
        _ => data?.ToString() ?? type.ToString()
    };
}
```

### Task 54.3: Automatic history recording

```csharp
// I GameState, koppla events till history
public class GameState
{
    public IPlayerHistory History { get; } = new PlayerHistory();

    public void OnLocationEntered(ILocation location)
    {
        if (!History.GetVisitedLocations().Contains(location))
        {
            History.Record(HistoryEventType.LocationVisited, location);
        }
    }

    public void OnItemAcquired(IItem item)
    {
        History.Record(HistoryEventType.ItemAcquired, item);
    }

    public void OnNpcMet(INpc npc)
    {
        if (!History.GetMetNpcs().Contains(npc))
        {
            History.Record(HistoryEventType.NpcMet, npc);
        }
    }
}
```

### Task 54.4: Story Summary Generator

```csharp
public class StorySummaryGenerator
{
    public string Generate(IPlayerHistory history)
    {
        var sb = new StringBuilder();
        sb.AppendLine("=== Your Journey ===\n");

        // Statistik
        sb.AppendLine($"Time played: {history.GetTotalPlayTime():hh\\:mm\\:ss}");
        sb.AppendLine($"Commands entered: {history.GetCommandCount()}");
        sb.AppendLine($"Locations visited: {history.GetVisitedLocations().Count()}");
        sb.AppendLine($"NPCs met: {history.GetMetNpcs().Count()}");
        sb.AppendLine();

        // Key moments
        sb.AppendLine("Key moments:");
        var keyMoments = history.Entries
            .Where(e => e.Type is HistoryEventType.QuestCompleted
                                or HistoryEventType.NpcKilled
                                or HistoryEventType.Achievement)
            .OrderBy(e => e.Turn);

        foreach (var moment in keyMoments)
        {
            sb.AppendLine($"  Turn {moment.Turn}: {moment.Description}");
        }

        return sb.ToString();
    }
}
```

### Task 54.5: Achievement System

```csharp
public interface IAchievement
{
    string Id { get; }
    string Name { get; }
    string Description { get; }
    bool IsUnlocked { get; }
    Func<IPlayerHistory, bool> Condition { get; }
}

public class AchievementSystem
{
    private readonly List<IAchievement> _achievements = [];

    public void Register(IAchievement achievement) => _achievements.Add(achievement);

    public IEnumerable<IAchievement> CheckAchievements(IPlayerHistory history)
    {
        foreach (var achievement in _achievements.Where(a => !a.IsUnlocked))
        {
            if (achievement.Condition(history))
            {
                achievement.Unlock();
                history.Record(HistoryEventType.Achievement, achievement);
                yield return achievement;
            }
        }
    }
}

// Exempel-achievements
public static class Achievements
{
    public static IAchievement Explorer => new Achievement(
        "explorer",
        "Explorer",
        "Visit 10 different locations",
        h => h.GetVisitedLocations().Count() >= 10
    );

    public static IAchievement Pacifist => new Achievement(
        "pacifist",
        "Pacifist",
        "Complete the game without killing anyone",
        h => !h.GetByType(HistoryEventType.NpcKilled).Any()
    );
}
```

### Task 54.6: HistoryCommand

```csharp
public class HistoryCommand : ICommand
{
    public CommandResult Execute(CommandContext context)
    {
        var generator = new StorySummaryGenerator();
        var summary = generator.Generate(context.State.History);
        return CommandResult.Ok(summary);
    }
}
```

### Task 54.7: Tester

```csharp
[Fact]
public void History_TracksVisitedLocations()
{
    var game = CreateGame();
    game.Execute("go north");
    game.Execute("go south");
    game.Execute("go north");

    var visited = game.State.History.GetVisitedLocations();

    Assert.Equal(2, visited.Count());
}

[Fact]
public void Achievement_UnlocksWhenConditionMet()
{
    var history = new PlayerHistory();
    var system = new AchievementSystem();
    system.Register(Achievements.Explorer);

    for (int i = 0; i < 10; i++)
    {
        history.Record(HistoryEventType.LocationVisited, new Location($"room{i}"));
    }

    var unlocked = system.CheckAchievements(history);

    Assert.Single(unlocked);
    Assert.Equal("Explorer", unlocked.First().Name);
}
```

### Task 54.8: Sandbox — äventyr med achievements

Demo som visar journey summary och unlockade achievements vid spelets slut.

---

## Completion Checklist
- [x] Core implementation for this slice is present in the engine.
- [x] Behaviour is covered by tests and/or deterministic validation paths.
- [x] Demo document: `54_The_Diary_of_Returns.md`.
- [x] Marked complete in project slice status.
