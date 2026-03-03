## Slice 63: Chase & Pursuit System

**Mål:** NPCs som jagar, följer eller flyr från spelaren.

**Referens:** `docs/plans/imported/timedstuff.md`

### Task 63.1: IChaseAI interface

```csharp
public interface IChaseAI
{
    ChaseState State { get; }
    INpc Chaser { get; }
    ILocation? Target { get; }

    void Update(IGameState gameState);
    void StartChase(ILocation target);
    void StopChase();
    int GetDistanceToTarget(IGameState state);
}

public enum ChaseState
{
    Idle,
    Chasing,
    Searching,
    Caught,
    LostTarget,
    Fleeing
}
```

### Task 63.2: ChaseAI implementation

```csharp
public class ChaseAI : IChaseAI
{
    public ChaseState State { get; private set; } = ChaseState.Idle;
    public INpc Chaser { get; }
    public ILocation? Target { get; private set; }

    private readonly int _detectionRange;
    private readonly int _giveUpAfter;
    private int _turnsSinceLastSeen;

    public ChaseAI(INpc chaser, int detectionRange = 2, int giveUpAfter = 5)
    {
        Chaser = chaser;
        _detectionRange = detectionRange;
        _giveUpAfter = giveUpAfter;
    }

    public void StartChase(ILocation target)
    {
        Target = target;
        State = ChaseState.Chasing;
        _turnsSinceLastSeen = 0;
    }

    public void StopChase()
    {
        State = ChaseState.Idle;
        Target = null;
    }

    public void Update(IGameState gameState)
    {
        if (State == ChaseState.Idle) return;

        var playerLocation = gameState.CurrentLocation;
        var chaserLocation = Chaser.CurrentLocation;

        // Kolla om vi fångat spelaren
        if (chaserLocation.Id == playerLocation.Id)
        {
            State = ChaseState.Caught;
            return;
        }

        var distance = GetDistanceToTarget(gameState);

        // Kan vi se spelaren?
        if (distance <= _detectionRange)
        {
            Target = playerLocation;
            _turnsSinceLastSeen = 0;
            State = ChaseState.Chasing;

            // Rör oss mot spelaren
            MoveTowardsTarget(gameState);
        }
        else
        {
            _turnsSinceLastSeen++;

            if (_turnsSinceLastSeen > _giveUpAfter)
            {
                State = ChaseState.LostTarget;
                return;
            }

            State = ChaseState.Searching;
            // Fortsätt mot senast kända position
            if (Target != null)
                MoveTowardsTarget(gameState);
        }
    }

    private void MoveTowardsTarget(IGameState gameState)
    {
        if (Target == null) return;

        var path = gameState.Pathfinder.FindPath(Chaser.CurrentLocation, Target);
        if (path.Count > 1)
        {
            var nextLocation = path[1];
            Chaser.MoveTo(nextLocation);
        }
    }

    public int GetDistanceToTarget(IGameState state)
    {
        if (Target == null) return int.MaxValue;

        var path = state.Pathfinder.FindPath(Chaser.CurrentLocation, Target);
        return path.Count - 1;  // -1 för att exkludera startposition
    }
}
```

### Task 63.3: FleeAI

```csharp
public class FleeAI : IChaseAI
{
    public ChaseState State { get; private set; } = ChaseState.Idle;
    public INpc Chaser { get; }  // Den som flyr
    public ILocation? Target { get; private set; }  // Den som jagas (spelaren)

    private readonly int _fearRange;

    public FleeAI(INpc fleeing, int fearRange = 3)
    {
        Chaser = fleeing;
        _fearRange = fearRange;
    }

    public void StartChase(ILocation target) => Target = target;
    public void StopChase() => State = ChaseState.Idle;

    public void Update(IGameState gameState)
    {
        var playerLocation = gameState.CurrentLocation;
        var npcLocation = Chaser.CurrentLocation;

        var distance = GetDistanceToTarget(gameState);

        if (distance <= _fearRange)
        {
            State = ChaseState.Fleeing;
            FleeFromPlayer(gameState);
        }
        else
        {
            State = ChaseState.Idle;
        }
    }

    private void FleeFromPlayer(IGameState gameState)
    {
        var playerLocation = gameState.CurrentLocation;
        var npcLocation = Chaser.CurrentLocation;

        // Hitta den utgång som är längst bort från spelaren
        var bestExit = npcLocation.Exits
            .Select(e => new
            {
                Exit = e,
                Distance = gameState.Pathfinder.FindPath(e.Value.Target, playerLocation).Count
            })
            .OrderByDescending(x => x.Distance)
            .FirstOrDefault();

        if (bestExit != null)
        {
            Chaser.MoveTo(bestExit.Exit.Value.Target);
        }
    }

    public int GetDistanceToTarget(IGameState state)
    {
        var path = state.Pathfinder.FindPath(Chaser.CurrentLocation, state.CurrentLocation);
        return path.Count - 1;
    }
}
```

### Task 63.4: Chase Event Messages

```csharp
public class ChaseNarrator
{
    public string GetMessage(IChaseAI chase, IGameState state)
    {
        var distance = chase.GetDistanceToTarget(state);
        var npcName = chase.Chaser.Name;

        return chase.State switch
        {
            ChaseState.Chasing when distance == 1 =>
                $"{npcName} is right behind you!",
            ChaseState.Chasing when distance == 2 =>
                $"You hear {npcName} getting closer!",
            ChaseState.Chasing =>
                $"You can hear {npcName} in pursuit somewhere nearby.",
            ChaseState.Searching =>
                $"{npcName} is searching for you...",
            ChaseState.Caught =>
                $"{npcName} has caught up with you!",
            ChaseState.LostTarget =>
                $"{npcName} seems to have lost your trail.",
            ChaseState.Fleeing =>
                $"{npcName} flees in terror!",
            _ => ""
        };
    }

    public string GetTensionDescription(int distance)
    {
        return distance switch
        {
            1 => "Your heart pounds. They're almost upon you!",
            2 => "Footsteps echo close behind.",
            3 => "You can hear something following.",
            4 => "Distant sounds suggest you're being tracked.",
            _ => ""
        };
    }
}
```

### Task 63.5: Chase Manager

```csharp
public class ChaseManager
{
    private readonly List<IChaseAI> _activeChases = [];

    public void StartChase(INpc chaser, IGameState state)
    {
        var ai = new ChaseAI(chaser);
        ai.StartChase(state.CurrentLocation);
        _activeChases.Add(ai);

        chaser.SetProperty("chasing_player", true);
    }

    public void StartFlee(INpc fleeing, IGameState state)
    {
        var ai = new FleeAI(fleeing);
        ai.StartChase(state.CurrentLocation);
        _activeChases.Add(ai);

        fleeing.SetProperty("fleeing_from_player", true);
    }

    public void Update(IGameState state)
    {
        var narrator = new ChaseNarrator();

        foreach (var chase in _activeChases.ToList())
        {
            chase.Update(state);

            var message = narrator.GetMessage(chase, state);
            if (!string.IsNullOrEmpty(message))
                state.AddMessage(message);

            // Ta bort avslutade chases
            if (chase.State is ChaseState.Caught or ChaseState.LostTarget)
            {
                _activeChases.Remove(chase);
            }
        }
    }

    public bool IsBeingChased => _activeChases.Any(c => c.State == ChaseState.Chasing);

    public IEnumerable<INpc> GetChasers() =>
        _activeChases.Where(c => c.State == ChaseState.Chasing).Select(c => c.Chaser);
}
```

### Task 63.6: Chase Triggers

```csharp
public class ChaseTrigger
{
    public string NpcId { get; init; } = "";
    public Func<IGameState, bool> Condition { get; init; } = _ => true;
    public string TriggerMessage { get; init; } = "";

    public bool ShouldTrigger(IGameState state)
    {
        return Condition(state);
    }
}

// Exempel triggers:
// new ChaseTrigger
// {
//     NpcId = "guard",
//     Condition = state => state.HasFlag("stole_treasure"),
//     TriggerMessage = "Stop, thief!"
// }

// new ChaseTrigger
// {
//     NpcId = "monster",
//     Condition = state => state.CurrentLocation.HasProperty("dark") && !state.HasActiveLight(),
//     TriggerMessage = "Something stirs in the darkness..."
// }
```

### Task 63.7: Stealth Integration

```csharp
public class StealthSystem
{
    public bool CanHide(IGameState state)
    {
        return state.CurrentLocation.GetProperty<bool>("has_hiding_spots", false);
    }

    public bool TryHide(IGameState state, ChaseManager chaseManager)
    {
        if (!CanHide(state))
            return false;

        var stealth = state.Stats.GetStat("stealth");
        var roll = new Random().Next(1, 20) + stealth;

        if (roll >= 15)  // DC 15 to hide
        {
            // Alla chasers tappar bort spelaren
            foreach (var chaser in chaseManager.GetChasers())
            {
                var chase = GetChaseFor(chaseManager, chaser);
                if (chase != null)
                {
                    chase.StopChase();
                    state.AddMessage($"You hide from {chaser.Name}.");
                }
            }
            return true;
        }

        state.AddMessage("You try to hide but fail!");
        return false;
    }

    private IChaseAI? GetChaseFor(ChaseManager manager, INpc chaser)
    {
        // Implementation
        return null;
    }
}
```

### Task 63.8: Tester

```csharp
[Fact]
public void ChaseAI_MovesTowardsPlayer()
{
    var game = CreateGameWithConnectedRooms();
    var guard = CreateNpc("guard", game.State.GetLocation("room_a"));
    var chase = new ChaseAI(guard);

    game.State.CurrentLocation = game.State.GetLocation("room_c");
    chase.StartChase(game.State.CurrentLocation);
    chase.Update(game.State);

    // Guard should have moved closer
    Assert.Equal("room_b", guard.CurrentLocation.Id);
}

[Fact]
public void ChaseAI_CatchesPlayerWhenSameRoom()
{
    var game = CreateGame();
    var guard = CreateNpc("guard", game.State.CurrentLocation);
    var chase = new ChaseAI(guard);

    chase.StartChase(game.State.CurrentLocation);
    chase.Update(game.State);

    Assert.Equal(ChaseState.Caught, chase.State);
}

[Fact]
public void FleeAI_MovesAwayFromPlayer()
{
    var game = CreateGameWithConnectedRooms();
    var rabbit = CreateNpc("rabbit", game.State.GetLocation("room_b"));
    var flee = new FleeAI(rabbit);

    game.State.CurrentLocation = game.State.GetLocation("room_a");
    flee.StartChase(game.State.CurrentLocation);
    flee.Update(game.State);

    // Rabbit should have moved further away
    Assert.Equal("room_c", rabbit.CurrentLocation.Id);
}
```

### Task 63.9: Sandbox — jaktscenario

Demo med vakt som jagar spelaren genom en labyrint.

---

## Completion Checklist
- [x] Core implementation for this slice is present in the engine.
- [x] Behaviour is covered by tests and/or deterministic validation paths.
- [x] Demo document: `63_Run_From_The_Collector.md`.
- [x] Marked complete in project slice status.
