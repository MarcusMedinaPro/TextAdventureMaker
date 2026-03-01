## Slice 65: Test Helpers & Infrastructure

**Mål:** Förbättrad testinfrastruktur med helpers, custom assertions och bättre organisation.

**Referens:** `docs/plans/imported/Testing_Strategy.md`

### Task 65.1: TestWorldBuilder

```csharp
public class TestWorldBuilder
{
    private readonly GameBuilder _builder;
    private readonly List<Action<IGameState>> _postBuildActions = [];

    public TestWorldBuilder(string name = "TestWorld")
    {
        _builder = new GameBuilder(name);
    }

    public static TestWorldBuilder Create(string name = "TestWorld") => new(name);

    // Snabb setup för vanliga testscenarier
    public TestWorldBuilder WithSimpleWorld()
    {
        _builder
            .AddLocation("start", loc => loc.Name("Start Room").Description("A simple room."))
            .AddLocation("north_room", loc => loc.Name("North Room").AddExit(Direction.South, "start"))
            .AddLocation("east_room", loc => loc.Name("East Room").AddExit(Direction.West, "start"))
            .StartAt("start");

        // Koppla exits
        _postBuildActions.Add(state =>
        {
            state.GetLocation("start")!.AddExit(Direction.North, state.GetLocation("north_room")!);
            state.GetLocation("start")!.AddExit(Direction.East, state.GetLocation("east_room")!);
        });

        return this;
    }

    public TestWorldBuilder WithInventoryItem(string id, string name)
    {
        _postBuildActions.Add(state =>
        {
            state.Inventory.Add(new Item(id, name));
        });
        return this;
    }

    public TestWorldBuilder WithItemInRoom(string roomId, string itemId, string itemName)
    {
        _postBuildActions.Add(state =>
        {
            state.GetLocation(roomId)?.Items.Add(new Item(itemId, itemName));
        });
        return this;
    }

    public TestWorldBuilder WithNpcInRoom(string roomId, string npcId, string npcName)
    {
        _postBuildActions.Add(state =>
        {
            state.GetLocation(roomId)?.Npcs.Add(new Npc(npcId, npcName));
        });
        return this;
    }

    public TestWorldBuilder WithFlag(string flag)
    {
        _postBuildActions.Add(state => state.SetFlag(flag));
        return this;
    }

    public TestWorldBuilder WithHealth(int health)
    {
        _postBuildActions.Add(state => state.Stats.Health = health);
        return this;
    }

    public TestWorldBuilder WithCustomSetup(Action<IGameState> setup)
    {
        _postBuildActions.Add(setup);
        return this;
    }

    public IGame Build()
    {
        var game = _builder.Build();

        foreach (var action in _postBuildActions)
        {
            action(game.State);
        }

        return game;
    }
}

// Användning:
var game = TestWorldBuilder.Create()
    .WithSimpleWorld()
    .WithInventoryItem("key", "Golden Key")
    .WithFlag("door_unlocked")
    .Build();
```

### Task 65.2: Custom Assert Extensions

```csharp
public static class GameAssert
{
    // Location assertions
    public static void IsAtLocation(IGameState state, string locationId)
    {
        Assert.Equal(locationId, state.CurrentLocation.Id);
    }

    public static void HasVisited(IGameState state, string locationId)
    {
        Assert.True(state.VisitedLocations.Contains(locationId),
            $"Expected to have visited '{locationId}'");
    }

    // Inventory assertions
    public static void HasItem(IGameState state, string itemId)
    {
        Assert.True(state.Inventory.HasItem(itemId),
            $"Expected inventory to contain '{itemId}'");
    }

    public static void DoesNotHaveItem(IGameState state, string itemId)
    {
        Assert.False(state.Inventory.HasItem(itemId),
            $"Expected inventory to NOT contain '{itemId}'");
    }

    public static void InventoryCount(IGameState state, int expected)
    {
        Assert.Equal(expected, state.Inventory.Items.Count);
    }

    // Flag assertions
    public static void HasFlag(IGameState state, string flag)
    {
        Assert.True(state.HasFlag(flag),
            $"Expected flag '{flag}' to be set");
    }

    public static void DoesNotHaveFlag(IGameState state, string flag)
    {
        Assert.False(state.HasFlag(flag),
            $"Expected flag '{flag}' to NOT be set");
    }

    // Command result assertions
    public static void IsSuccess(CommandResult result)
    {
        Assert.True(result.Success, $"Expected success but got: {result.Message}");
    }

    public static void IsFailure(CommandResult result)
    {
        Assert.False(result.Success, "Expected failure but command succeeded");
    }

    public static void MessageContains(CommandResult result, string expected)
    {
        Assert.Contains(expected, result.Message, StringComparison.OrdinalIgnoreCase);
    }

    public static void IsSuccessWithMessage(CommandResult result, string expectedSubstring)
    {
        IsSuccess(result);
        MessageContains(result, expectedSubstring);
    }

    // Stats assertions
    public static void HealthEquals(IGameState state, int expected)
    {
        Assert.Equal(expected, state.Stats.Health);
    }

    public static void HealthBetween(IGameState state, int min, int max)
    {
        Assert.InRange(state.Stats.Health, min, max);
    }

    public static void IsDead(IGameState state)
    {
        Assert.True(state.Stats.Health <= 0, "Expected player to be dead");
    }

    public static void IsAlive(IGameState state)
    {
        Assert.True(state.Stats.Health > 0, "Expected player to be alive");
    }

    // NPC assertions
    public static void NpcIsInRoom(IGameState state, string npcId, string roomId)
    {
        var location = state.GetLocation(roomId);
        Assert.NotNull(location);
        Assert.True(location.Npcs.Any(n => n.Id == npcId),
            $"Expected NPC '{npcId}' to be in room '{roomId}'");
    }

    public static void NpcIsNotInRoom(IGameState state, string npcId, string roomId)
    {
        var location = state.GetLocation(roomId);
        Assert.NotNull(location);
        Assert.False(location.Npcs.Any(n => n.Id == npcId),
            $"Expected NPC '{npcId}' to NOT be in room '{roomId}'");
    }

    // Quest assertions
    public static void QuestIsActive(IGameState state, string questId)
    {
        Assert.True(state.QuestLog.IsActive(questId),
            $"Expected quest '{questId}' to be active");
    }

    public static void QuestIsCompleted(IGameState state, string questId)
    {
        Assert.True(state.QuestLog.IsCompleted(questId),
            $"Expected quest '{questId}' to be completed");
    }

    // Move assertions
    public static void CanMoveTo(IGame game, Direction direction)
    {
        var result = game.Execute($"go {direction}");
        IsSuccess(result);
    }

    public static void CannotMoveTo(IGame game, Direction direction)
    {
        var startLocation = game.State.CurrentLocation.Id;
        var result = game.Execute($"go {direction}");
        Assert.Equal(startLocation, game.State.CurrentLocation.Id);
    }
}
```

### Task 65.3: Test Fixtures

```csharp
public class GameTestFixture : IDisposable
{
    public IGame Game { get; private set; }
    public IGameState State => Game.State;

    public GameTestFixture()
    {
        Game = TestWorldBuilder.Create()
            .WithSimpleWorld()
            .Build();
    }

    public void Reset()
    {
        Game = TestWorldBuilder.Create()
            .WithSimpleWorld()
            .Build();
    }

    public CommandResult Execute(string command) => Game.Execute(command);

    public void Dispose()
    {
        // Cleanup if needed
    }
}

// Collection fixture för att dela setup mellan tester
public class SharedGameFixture : ICollectionFixture<GameTestFixture>
{
}

// Användning:
[Collection("SharedGame")]
public class NavigationTests
{
    private readonly GameTestFixture _fixture;

    public NavigationTests(GameTestFixture fixture)
    {
        _fixture = fixture;
        _fixture.Reset();
    }

    [Fact]
    public void GoNorth_MovesToNorthRoom()
    {
        var result = _fixture.Execute("go north");

        GameAssert.IsSuccess(result);
        GameAssert.IsAtLocation(_fixture.State, "north_room");
    }
}
```

### Task 65.4: Test Categories med Traits

```csharp
public static class TestCategories
{
    public const string Unit = "Unit";
    public const string Integration = "Integration";
    public const string Performance = "Performance";
    public const string Regression = "Regression";
    public const string EdgeCase = "EdgeCase";
}

// Användning:
[Fact]
[Trait("Category", TestCategories.Unit)]
public void Item_CanBeTaken()
{
    // ...
}

[Fact]
[Trait("Category", TestCategories.Integration)]
[Trait("Feature", "Combat")]
public void Combat_DealsDamageToEnemy()
{
    // ...
}

[Fact]
[Trait("Category", TestCategories.EdgeCase)]
public void Inventory_HandlesMaxCapacity()
{
    // ...
}

// Kör endast unit tests:
// dotnet test --filter "Category=Unit"
```

### Task 65.5: Parameterized Tests

```csharp
public class DirectionTests
{
    public static IEnumerable<object[]> DirectionPairs =>
    [
        [Direction.North, Direction.South],
        [Direction.South, Direction.North],
        [Direction.East, Direction.West],
        [Direction.West, Direction.East],
        [Direction.Up, Direction.Down],
        [Direction.Down, Direction.Up],
    ];

    [Theory]
    [MemberData(nameof(DirectionPairs))]
    public void GetOpposite_ReturnsCorrectDirection(Direction input, Direction expected)
    {
        var result = DirectionHelper.GetOpposite(input);
        Assert.Equal(expected, result);
    }

    public static IEnumerable<object[]> DirectionAliases =>
    [
        ["n", Direction.North],
        ["north", Direction.North],
        ["s", Direction.South],
        ["south", Direction.South],
        ["e", Direction.East],
        ["east", Direction.East],
        ["w", Direction.West],
        ["west", Direction.West],
        ["u", Direction.Up],
        ["up", Direction.Up],
        ["d", Direction.Down],
        ["down", Direction.Down],
    ];

    [Theory]
    [MemberData(nameof(DirectionAliases))]
    public void TryParse_ParsesAllAliases(string input, Direction expected)
    {
        var success = DirectionHelper.TryParse(input, out var result);

        Assert.True(success);
        Assert.Equal(expected, result);
    }
}
```

### Task 65.6: Snapshot Testing för Output

```csharp
public static class SnapshotAssert
{
    private static readonly string SnapshotDir = Path.Combine(
        AppContext.BaseDirectory, "..", "..", "..", "Snapshots");

    public static void MatchesSnapshot(string actual, string snapshotName)
    {
        var snapshotPath = Path.Combine(SnapshotDir, $"{snapshotName}.txt");

        if (!File.Exists(snapshotPath))
        {
            // Första körningen: skapa snapshot
            Directory.CreateDirectory(SnapshotDir);
            File.WriteAllText(snapshotPath, actual);
            Assert.Fail($"Snapshot created at {snapshotPath}. Re-run test to verify.");
        }

        var expected = File.ReadAllText(snapshotPath);
        Assert.Equal(expected, actual);
    }

    public static void UpdateSnapshot(string content, string snapshotName)
    {
        var snapshotPath = Path.Combine(SnapshotDir, $"{snapshotName}.txt");
        Directory.CreateDirectory(SnapshotDir);
        File.WriteAllText(snapshotPath, content);
    }
}

// Användning:
[Fact]
public void LookCommand_OutputMatchesSnapshot()
{
    var game = TestWorldBuilder.Create().WithSimpleWorld().Build();
    var result = game.Execute("look");

    SnapshotAssert.MatchesSnapshot(result.Message, "LookCommand_StartRoom");
}
```

### Task 65.7: Performance Test Helpers

```csharp
public static class PerformanceAssert
{
    public static void CompletesWithin(Action action, TimeSpan maxDuration)
    {
        var sw = Stopwatch.StartNew();
        action();
        sw.Stop();

        Assert.True(sw.Elapsed <= maxDuration,
            $"Expected to complete within {maxDuration.TotalMilliseconds}ms but took {sw.ElapsedMilliseconds}ms");
    }

    public static void CompletesWithinMs(Action action, int maxMilliseconds)
    {
        CompletesWithin(action, TimeSpan.FromMilliseconds(maxMilliseconds));
    }
}

// Användning:
[Fact]
[Trait("Category", TestCategories.Performance)]
public void Pathfinding_FindsPathIn1000RoomWorld()
{
    var game = CreateLargeWorld(1000);

    PerformanceAssert.CompletesWithinMs(() =>
    {
        game.Pathfinder.FindPath("room_0", "room_999");
    }, maxMilliseconds: 100);
}
```

### Task 65.8: Mock Helpers

```csharp
public static class MockFactory
{
    public static ILocation CreateLocation(string id, string name = null)
    {
        return new Location(id, name ?? id);
    }

    public static IItem CreateItem(string id, string name = null)
    {
        return new Item(id, name ?? id);
    }

    public static INpc CreateNpc(string id, string name = null)
    {
        return new Npc(id, name ?? id);
    }

    public static IGameState CreateState()
    {
        return TestWorldBuilder.Create().WithSimpleWorld().Build().State;
    }

    public static CommandContext CreateContext(IGameState state = null)
    {
        state ??= CreateState();
        return new CommandContext(state);
    }
}
```

### Task 65.9: Tester för test infrastructure

```csharp
[Fact]
public void TestWorldBuilder_CreatesSimpleWorld()
{
    var game = TestWorldBuilder.Create()
        .WithSimpleWorld()
        .Build();

    Assert.NotNull(game.State.CurrentLocation);
    Assert.Equal("start", game.State.CurrentLocation.Id);
    Assert.True(game.State.CurrentLocation.Exits.ContainsKey(Direction.North));
}

[Fact]
public void GameAssert_HasItem_ThrowsWhenMissing()
{
    var game = TestWorldBuilder.Create().WithSimpleWorld().Build();

    Assert.Throws<Xunit.Sdk.TrueException>(() =>
        GameAssert.HasItem(game.State, "nonexistent"));
}

[Fact]
public void PerformanceAssert_FailsWhenTooSlow()
{
    Assert.Throws<Xunit.Sdk.TrueException>(() =>
        PerformanceAssert.CompletesWithinMs(() => Thread.Sleep(100), 50));
}
```

---
