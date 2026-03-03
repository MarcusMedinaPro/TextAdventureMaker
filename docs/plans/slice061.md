## Slice 61: Debug Console & Dev Tools

**Mål:** Utvecklarverktyg för testning, debugging och snabb iteration.

**Referens:** `docs/plans/imported/plan_v1_extras.md`

### Task 61.1: IDebugConsole interface

```csharp
public interface IDebugConsole
{
    bool IsEnabled { get; }
    void Enable();
    void Disable();

    CommandResult ExecuteDebugCommand(string command, IGameState state);
    IEnumerable<string> GetAvailableCommands();
}
```

### Task 61.2: Debug Commands

```csharp
public class DebugConsole : IDebugConsole
{
    public bool IsEnabled { get; private set; }

    private readonly Dictionary<string, Func<string[], IGameState, CommandResult>> _commands = new()
    {
        ["teleport"] = Teleport,
        ["give"] = GiveItem,
        ["take"] = RemoveItem,
        ["setflag"] = SetFlag,
        ["clearflag"] = ClearFlag,
        ["flags"] = ListFlags,
        ["inspect"] = Inspect,
        ["state"] = ShowState,
        ["heal"] = Heal,
        ["kill"] = KillNpc,
        ["spawn"] = SpawnNpc,
        ["time"] = SetTime,
        ["god"] = ToggleGodMode,
        ["noclip"] = ToggleNoclip,
        ["help"] = ShowHelp,
    };

    public CommandResult ExecuteDebugCommand(string input, IGameState state)
    {
        if (!IsEnabled)
            return CommandResult.Fail("Debug console is disabled.");

        var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0)
            return CommandResult.Fail("No command specified.");

        var cmd = parts[0].ToLower().TrimStart('/');
        var args = parts[1..];

        if (_commands.TryGetValue(cmd, out var handler))
            return handler(args, state);

        return CommandResult.Fail($"Unknown debug command: {cmd}");
    }

    private static CommandResult Teleport(string[] args, IGameState state)
    {
        if (args.Length == 0)
            return CommandResult.Fail("Usage: /teleport <location_id>");

        var locationId = args[0];
        var location = state.GetLocation(locationId);

        if (location == null)
            return CommandResult.Fail($"Location '{locationId}' not found.");

        state.CurrentLocation = location;
        return CommandResult.Ok($"Teleported to {location.Name}.");
    }

    private static CommandResult GiveItem(string[] args, IGameState state)
    {
        if (args.Length == 0)
            return CommandResult.Fail("Usage: /give <item_id> [quantity]");

        var itemId = args[0];
        var quantity = args.Length > 1 ? int.Parse(args[1]) : 1;

        for (int i = 0; i < quantity; i++)
        {
            var item = state.CreateItem(itemId);
            state.Inventory.Add(item);
        }

        return CommandResult.Ok($"Added {quantity}x {itemId} to inventory.");
    }

    private static CommandResult Inspect(string[] args, IGameState state)
    {
        var sb = new StringBuilder();

        if (args.Length == 0)
        {
            // Inspektera nuvarande rum
            sb.AppendLine($"=== {state.CurrentLocation.Name} ===");
            sb.AppendLine($"ID: {state.CurrentLocation.Id}");
            sb.AppendLine($"Items: {state.CurrentLocation.Items.Count}");
            sb.AppendLine($"NPCs: {state.CurrentLocation.Npcs.Count}");
            sb.AppendLine($"Exits: {string.Join(", ", state.CurrentLocation.Exits.Keys)}");
        }
        else
        {
            // Inspektera specifikt objekt
            var target = args[0];
            var item = state.FindItem(target);
            if (item != null)
            {
                sb.AppendLine($"=== Item: {item.Name} ===");
                sb.AppendLine($"ID: {item.Id}");
                sb.AppendLine($"Properties: {string.Join(", ", item.GetAllProperties().Select(p => $"{p.Key}={p.Value}"))}");
            }
        }

        return CommandResult.Ok(sb.ToString());
    }

    private static CommandResult ShowState(string[] args, IGameState state)
    {
        var sb = new StringBuilder();
        sb.AppendLine("=== Game State ===");
        sb.AppendLine($"Location: {state.CurrentLocation.Id}");
        sb.AppendLine($"Turn: {state.Turn}");
        sb.AppendLine($"Inventory: {state.Inventory.Items.Count} items");
        sb.AppendLine($"Health: {state.Stats.Health}/{state.Stats.MaxHealth}");
        sb.AppendLine($"Flags: {state.GetAllFlags().Count()}");
        sb.AppendLine($"Quests: {state.QuestLog.ActiveQuests.Count()} active");

        return CommandResult.Ok(sb.ToString());
    }
}
```

### Task 61.3: State Inspector (Tree View)

```csharp
public class StateInspector
{
    public string GenerateTreeView(IGameState state, int maxDepth = 3)
    {
        var sb = new StringBuilder();

        sb.AppendLine("GameState");
        sb.AppendLine("├── CurrentLocation");
        sb.AppendLine($"│   └── {state.CurrentLocation.Id}: {state.CurrentLocation.Name}");
        sb.AppendLine("├── Inventory");
        foreach (var item in state.Inventory.Items.Take(10))
            sb.AppendLine($"│   ├── {item.Id}: {item.Name}");
        sb.AppendLine("├── Stats");
        sb.AppendLine($"│   ├── Health: {state.Stats.Health}/{state.Stats.MaxHealth}");
        sb.AppendLine($"│   └── Energy: {state.Stats.Energy}/{state.Stats.MaxEnergy}");
        sb.AppendLine("├── Flags");
        foreach (var flag in state.GetAllFlags().Take(10))
            sb.AppendLine($"│   ├── {flag}");
        sb.AppendLine("└── World");
        sb.AppendLine($"    └── {state.World.Locations.Count} locations");

        return sb.ToString();
    }
}
```

### Task 61.4: Undo/Redo System

```csharp
public class UndoRedoSystem
{
    private readonly Stack<GameStateSnapshot> _undoStack = new();
    private readonly Stack<GameStateSnapshot> _redoStack = new();
    private const int MaxHistory = 50;

    public void SaveState(IGameState state)
    {
        _undoStack.Push(CreateSnapshot(state));
        _redoStack.Clear();

        if (_undoStack.Count > MaxHistory)
        {
            // Ta bort äldsta
            var temp = _undoStack.ToList();
            temp.RemoveAt(temp.Count - 1);
            _undoStack.Clear();
            foreach (var item in temp.AsEnumerable().Reverse())
                _undoStack.Push(item);
        }
    }

    public bool CanUndo => _undoStack.Count > 0;
    public bool CanRedo => _redoStack.Count > 0;

    public void Undo(IGameState state)
    {
        if (!CanUndo) return;

        _redoStack.Push(CreateSnapshot(state));
        var snapshot = _undoStack.Pop();
        RestoreSnapshot(state, snapshot);
    }

    public void Redo(IGameState state)
    {
        if (!CanRedo) return;

        _undoStack.Push(CreateSnapshot(state));
        var snapshot = _redoStack.Pop();
        RestoreSnapshot(state, snapshot);
    }

    private GameStateSnapshot CreateSnapshot(IGameState state) => new(
        state.CurrentLocation.Id,
        state.Inventory.Items.Select(i => i.Id).ToList(),
        state.GetAllFlags().ToList(),
        state.Stats.Health,
        state.Turn
    );

    private void RestoreSnapshot(IGameState state, GameStateSnapshot snapshot)
    {
        state.CurrentLocation = state.GetLocation(snapshot.LocationId)!;
        // ... restore other state
    }
}

public record GameStateSnapshot(
    string LocationId,
    List<string> InventoryItemIds,
    List<string> Flags,
    int Health,
    int Turn
);
```

### Task 61.5: Automated Test Runner

```csharp
public class TestScriptRunner
{
    public TestResult RunScript(string scriptPath, IGame game)
    {
        var lines = File.ReadAllLines(scriptPath);
        var results = new List<(string command, bool success, string? error)>();

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                continue;

            if (line.StartsWith("!assert"))
            {
                var assertion = line[8..].Trim();
                var assertResult = CheckAssertion(assertion, game.State);
                results.Add((line, assertResult.success, assertResult.error));
            }
            else
            {
                var result = game.Execute(line);
                results.Add((line, result.Success, result.Success ? null : result.Message));
            }
        }

        return new TestResult(
            results.All(r => r.success),
            results
        );
    }

    private (bool success, string? error) CheckAssertion(string assertion, IGameState state)
    {
        // !assert location kitchen
        // !assert has_item key
        // !assert flag door_open
        // !assert health > 50

        var parts = assertion.Split(' ');

        return parts[0] switch
        {
            "location" => (state.CurrentLocation.Id == parts[1], $"Expected location {parts[1]}, was {state.CurrentLocation.Id}"),
            "has_item" => (state.Inventory.HasItem(parts[1]), $"Expected item {parts[1]} in inventory"),
            "flag" => (state.HasFlag(parts[1]), $"Expected flag {parts[1]} to be set"),
            "health" => CheckNumericAssertion(state.Stats.Health, parts[1], int.Parse(parts[2])),
            _ => (false, $"Unknown assertion: {parts[0]}")
        };
    }
}

// Exempel testskript:
// # Test: Key puzzle
// go north
// take key
// !assert has_item key
// go south
// use key on door
// !assert flag door_unlocked
// go east
// !assert location treasure_room
```

### Task 61.6: Performance Profiler

```csharp
public class PerformanceProfiler
{
    private readonly Dictionary<string, List<TimeSpan>> _timings = [];

    public IDisposable Measure(string operation)
    {
        return new TimingScope(this, operation);
    }

    public void Report()
    {
        Console.WriteLine("=== Performance Report ===");
        foreach (var (op, timings) in _timings)
        {
            var avg = timings.Average(t => t.TotalMilliseconds);
            var max = timings.Max(t => t.TotalMilliseconds);
            Console.WriteLine($"{op}: avg={avg:F2}ms, max={max:F2}ms, calls={timings.Count}");
        }
    }

    private class TimingScope : IDisposable
    {
        private readonly PerformanceProfiler _profiler;
        private readonly string _operation;
        private readonly Stopwatch _sw;

        public TimingScope(PerformanceProfiler profiler, string operation)
        {
            _profiler = profiler;
            _operation = operation;
            _sw = Stopwatch.StartNew();
        }

        public void Dispose()
        {
            _sw.Stop();
            if (!_profiler._timings.ContainsKey(_operation))
                _profiler._timings[_operation] = [];
            _profiler._timings[_operation].Add(_sw.Elapsed);
        }
    }
}
```

### Task 61.7: Tester

```csharp
[Fact]
public void DebugConsole_TeleportChangesLocation()
{
    var game = CreateGame();
    var console = new DebugConsole();
    console.Enable();

    console.ExecuteDebugCommand("/teleport kitchen", game.State);

    Assert.Equal("kitchen", game.State.CurrentLocation.Id);
}

[Fact]
public void UndoRedo_RestoresPreviousState()
{
    var game = CreateGame();
    var undo = new UndoRedoSystem();

    undo.SaveState(game.State);
    game.Execute("go north");
    undo.SaveState(game.State);

    undo.Undo(game.State);

    Assert.Equal("start", game.State.CurrentLocation.Id);
}
```

### Task 61.8: Sandbox — dev mode äventyr

Demo med aktiverad debug console för att testa alla kommandon.

---

## Completion Checklist
- [x] Core implementation for this slice is present in the engine.
- [x] Behaviour is covered by tests and/or deterministic validation paths.
- [x] Demo document: `61_Debug_Console_Deep_Dive.md`.
- [x] Marked complete in project slice status.
