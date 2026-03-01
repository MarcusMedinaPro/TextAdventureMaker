## Slice 1: Projekt-setup + Location + Navigation

**Mål:** Spelaren kan röra sig mellan rum. Sandbox visar det.

### Task 1.1: Skapa solution och projekt

**Step 1: Skapa projektstruktur**

```bash
cd /mnt/c/git/MarcusMedina/TextAdventure
dotnet new sln -n TextAdventure
mkdir -p src/MarcusMedina.TextAdventure
mkdir -p src/MarcusMedina.TextAdventure.AI
mkdir -p tests/MarcusMedina.TextAdventure.Tests
mkdir -p sandbox/TextAdventure.Sandbox
dotnet new classlib -n MarcusMedina.TextAdventure -o src/MarcusMedina.TextAdventure
dotnet new classlib -n MarcusMedina.TextAdventure.AI -o src/MarcusMedina.TextAdventure.AI
dotnet new xunit -n MarcusMedina.TextAdventure.Tests -o tests/MarcusMedina.TextAdventure.Tests
dotnet new console -n TextAdventure.Sandbox -o sandbox/TextAdventure.Sandbox
dotnet sln add TextAdventure.slnx src/MarcusMedina.TextAdventure
dotnet sln add TextAdventure.slnx src/MarcusMedina.TextAdventure.AI
dotnet sln add TextAdventure.slnx tests/MarcusMedina.TextAdventure.Tests
dotnet sln add TextAdventure.slnx sandbox/TextAdventure.Sandbox
dotnet add tests/MarcusMedina.TextAdventure.Tests reference src/MarcusMedina.TextAdventure
dotnet add sandbox/TextAdventure.Sandbox reference src/MarcusMedina.TextAdventure
dotnet add src/MarcusMedina.TextAdventure.AI reference src/MarcusMedina.TextAdventure
```

**Step 2: Commit**

```bash
git init && git add . && git commit -m "chore: initial project structure"
```

---

### Task 1.2: ILocation + Location (bi-directional exits)

**Files:**

- Create: `src/MarcusMedina.TextAdventure/Interfaces/ILocation.cs`
- Create: `src/MarcusMedina.TextAdventure/Models/Location.cs`
- Create: `src/MarcusMedina.TextAdventure/Enums/Direction.cs`
- Create: `src/MarcusMedina.TextAdventure/Helpers/DirectionHelper.cs`
- Test: `tests/MarcusMedina.TextAdventure.Tests/LocationTests.cs`

**Step 1: Write tests**

```csharp
// LocationTests.cs
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Enums;

namespace MarcusMedina.TextAdventure.Tests;

public class LocationTests
{
    [Fact]
    public void Location_ShouldHaveId()
    {
        var loc = new Location("cave");
        Assert.Equal("cave", loc.Id);
    }

    [Fact]
    public void Location_ShouldHaveDescription()
    {
        var loc = new Location("cave")
            .Description("A dark cave with glowing mushrooms");

        Assert.Equal("A dark cave with glowing mushrooms", loc.GetDescription());
    }

    [Fact]
    public void AddExit_ShouldCreateBidirectionalPassage()
    {
        var hall = new Location("hall");
        var bedroom = new Location("bedroom");

        hall.AddExit(Direction.North, bedroom);

        Assert.Equal(bedroom, hall.GetExit(Direction.North));
        Assert.Equal(hall, bedroom.GetExit(Direction.South)); // Auto-created!
    }

    [Fact]
    public void AddExit_OneWay_ShouldNotCreateReturnPath()
    {
        var hall = new Location("hall");
        var pit = new Location("pit");

        hall.AddExit(Direction.Down, pit, oneWay: true);

        Assert.Equal(pit, hall.GetExit(Direction.Down));
        Assert.Null(pit.GetExit(Direction.Up)); // No return!
    }
}
```

**Step 2: Write implementation**

```csharp
// Enums/Direction.cs
namespace MarcusMedina.TextAdventure.Enums;

public enum Direction
{
    North, South, East, West, Up, Down,
    NorthEast, NorthWest, SouthEast, SouthWest
}
```

```csharp
// Helpers/DirectionHelper.cs
namespace MarcusMedina.TextAdventure.Helpers;

public static class DirectionHelper
{
    public static Direction GetOpposite(Direction direction) => direction switch
    {
        Direction.North => Direction.South,
        Direction.South => Direction.North,
        Direction.East => Direction.West,
        Direction.West => Direction.East,
        Direction.Up => Direction.Down,
        Direction.Down => Direction.Up,
        Direction.NorthEast => Direction.SouthWest,
        Direction.NorthWest => Direction.SouthEast,
        Direction.SouthEast => Direction.NorthWest,
        Direction.SouthWest => Direction.NorthEast,
        _ => throw new ArgumentOutOfRangeException(nameof(direction))
    };
}
```

```csharp
// Interfaces/ILocation.cs
using MarcusMedina.TextAdventure.Enums;

namespace MarcusMedina.TextAdventure.Interfaces;

public interface ILocation
{
    string Id { get; }
    string GetDescription();
    ILocation? GetExit(Direction direction);
    IReadOnlyDictionary<Direction, ILocation> Exits { get; }
}
```

```csharp
// Models/Location.cs
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Helpers;
using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public class Location : ILocation
{
    public string Id { get; }
    private string _description = "";
    private readonly Dictionary<Direction, ILocation> _exits = new();

    public IReadOnlyDictionary<Direction, ILocation> Exits => _exits;

    public Location(string id)
    {
        Id = id;
    }

    public Location Description(string text)
    {
        _description = text;
        return this;
    }

    public string GetDescription() => _description;

    public Location AddExit(Direction direction, ILocation target, bool oneWay = false)
    {
        _exits[direction] = target;

        if (!oneWay && target is Location targetLoc)
        {
            var opposite = DirectionHelper.GetOpposite(direction);
            targetLoc._exits[opposite] = this;
        }

        return this;
    }

    public ILocation? GetExit(Direction direction)
    {
        return _exits.TryGetValue(direction, out var loc) ? loc : null;
    }
}
```

---

## Implementation checklist (engine)
- [x] `Direction` enum
- [x] `DirectionHelper.GetOpposite(...)`
- [x] `ILocation` interface
- [x] `Location` model with `Id`, `Description`, `Exits`
- [x] `Location.AddExit(direction, target, oneWay)`
- [x] `Location.GetExit(direction)`
- [x] `GameState` with `CurrentLocation` and `Move(direction)`
- [x] Basic console sandbox exists

## Example checklist (docs/examples)
- [x] Multiple rooms connected with exits (`01_Morning_Ritual.md`)
- [x] Movement by cardinal directions (`01_Morning_Ritual.md`)
- [x] `look`/room description in sandbox (`01_Morning_Ritual.md`)

**Step 3: Run tests, verify pass**

Run: `dotnet test tests/MarcusMedina.TextAdventure.Tests`
Expected: PASS

**Step 4: Commit**

```bash
git add . && git commit -m "feat: add Location with bi-directional exits"
```

---

### Task 1.3: IGameState + Navigation

**Files:**

- Create: `src/MarcusMedina.TextAdventure/Interfaces/IGameState.cs`
- Create: `src/MarcusMedina.TextAdventure/Engine/GameState.cs`
- Test: `tests/MarcusMedina.TextAdventure.Tests/NavigationTests.cs`

**Step 1: Write failing test**

```csharp
// NavigationTests.cs
namespace MarcusMedina.TextAdventure.Tests;

public class NavigationTests
{
    [Fact]
    public void Player_CanMoveNorth()
    {
        var entrance = new Location("entrance");
        var forest = new Location("forest");
        entrance.AddExit(Direction.North, forest);
        forest.AddExit(Direction.South, entrance);

        var state = new GameState(entrance);
        var moved = state.Move(Direction.North);

        Assert.True(moved);
        Assert.Equal(forest, state.CurrentLocation);
    }

    [Fact]
    public void Player_CannotMoveWhereNoExit()
    {
        var entrance = new Location("entrance");
        var state = new GameState(entrance);
        var moved = state.Move(Direction.North);

        Assert.False(moved);
        Assert.Equal(entrance, state.CurrentLocation);
    }
}
```

**Step 2: Implement GameState**

```csharp
// Interfaces/IGameState.cs
namespace MarcusMedina.TextAdventure.Interfaces;

public interface IGameState
{
    ILocation CurrentLocation { get; }
    bool Move(Direction direction);
}
```

```csharp
// Engine/GameState.cs
namespace MarcusMedina.TextAdventure.Engine;

public class GameState : IGameState
{
    public ILocation CurrentLocation { get; private set; }

    public GameState(ILocation startLocation)
    {
        CurrentLocation = startLocation;
    }

    public bool Move(Direction direction)
    {
        var target = CurrentLocation.GetExit(direction);
        if (target == null) return false;
        CurrentLocation = target;
        return true;
    }
}
```

**Step 3: Run tests, commit**

---

### Task 1.4: Sandbox - Enkel navigation

**Files:**

- Modify: `sandbox/TextAdventure.Sandbox/Program.cs`

```csharp
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;

var entrance = new Location("entrance")
    .Description("You stand at the forest gate. It's dark.");

var forest = new Location("forest")
    .Description("A thick forest surrounds you. Shadows stretch long.");

var cave = new Location("cave")
    .Description("A dark cave with glowing mushrooms.");

// Bi-directional: entrance <-> forest <-> cave
entrance.AddExit(Direction.North, forest);  // Auto-creates forest -> South -> entrance
forest.AddExit(Direction.East, cave);       // Auto-creates cave -> West -> forest

var state = new GameState(entrance);

while (true)
{
    Console.WriteLine($"\n{state.CurrentLocation.GetDescription()}");
    Console.WriteLine("Exits: " + string.Join(", ", state.CurrentLocation.Exits.Keys));
    Console.Write("> ");

    var input = Console.ReadLine()?.Trim().ToLower();
    if (input == "quit") break;

    if (Enum.TryParse<Direction>(input, true, out var dir))
    {
        if (!state.Move(dir))
            Console.WriteLine("You can't go that way.");
    }
    else
    {
        Console.WriteLine("Unknown command.");
    }
}
```

**Commit:** `feat: sandbox with basic navigation`

---
