## Slice 67: Vehicles & Teleporters

**Mål:** Fordon och teleporters som transportmedel mellan locations.

**Referens:** `docs/plans/slice045.md` (vehicles idea)

### Task 67.1: IVehicle Interface

```csharp
public interface IVehicle : IItem
{
    string VehicleType { get; }           // "car", "boat", "horse"
    bool IsBoarded { get; }
    int Capacity { get; }                 // Passengers
    int CurrentPassengers { get; }

    // Requirements
    bool RequiresFuel { get; }
    string? FuelItemId { get; }
    int FuelLevel { get; }
    int MaxFuel { get; }

    // Movement
    IReadOnlyList<string> AllowedTerrains { get; }
    float SpeedMultiplier { get; }

    // Actions
    CommandResult Board(ICharacter character);
    CommandResult Disembark(ICharacter character);
    CommandResult Drive(Direction direction);
    CommandResult Refuel(IItem fuel);
}

public enum VehicleState
{
    Parked,
    Moving,
    Damaged,
    OutOfFuel
}
```

### Task 67.2: Vehicle Base Implementation

```csharp
public class Vehicle : Item, IVehicle
{
    public string VehicleType { get; init; } = "vehicle";
    public bool IsBoarded { get; private set; }
    public int Capacity { get; init; } = 1;
    public int CurrentPassengers { get; private set; }

    public bool RequiresFuel { get; init; } = false;
    public string? FuelItemId { get; init; }
    public int FuelLevel { get; private set; }
    public int MaxFuel { get; init; } = 100;

    public IReadOnlyList<string> AllowedTerrains { get; init; } = ["road", "path"];
    public float SpeedMultiplier { get; init; } = 2.0f;

    public VehicleState State { get; private set; } = VehicleState.Parked;

    private readonly List<ICharacter> _passengers = [];

    public CommandResult Board(ICharacter character)
    {
        if (IsBoarded && CurrentPassengers >= Capacity)
            return CommandResult.Fail($"The {Name} is full.");

        if (RequiresFuel && FuelLevel <= 0)
            return CommandResult.Fail($"The {Name} is out of fuel.");

        _passengers.Add(character);
        CurrentPassengers = _passengers.Count;
        IsBoarded = true;

        character.SetProperty("in_vehicle", Id);

        return CommandResult.Ok($"You board the {Name}.");
    }

    public CommandResult Disembark(ICharacter character)
    {
        if (!_passengers.Contains(character))
            return CommandResult.Fail("You're not in this vehicle.");

        _passengers.Remove(character);
        CurrentPassengers = _passengers.Count;
        IsBoarded = _passengers.Count > 0;

        character.SetProperty("in_vehicle", null);

        return CommandResult.Ok($"You exit the {Name}.");
    }

    public CommandResult Drive(Direction direction)
    {
        if (!IsBoarded)
            return CommandResult.Fail($"You need to board the {Name} first.");

        if (RequiresFuel && FuelLevel <= 0)
        {
            State = VehicleState.OutOfFuel;
            return CommandResult.Fail($"The {Name} is out of fuel!");
        }

        // Check terrain
        var targetLocation = GetTargetLocation(direction);
        if (targetLocation == null)
            return CommandResult.Fail("You can't go that way.");

        var terrain = targetLocation.GetProperty<string>("terrain") ?? "ground";
        if (!AllowedTerrains.Contains(terrain))
            return CommandResult.Fail($"The {Name} can't travel on {terrain}.");

        // Consume fuel
        if (RequiresFuel)
            FuelLevel--;

        State = VehicleState.Moving;

        return CommandResult.Ok($"You drive the {Name} {direction}.");
    }

    public CommandResult Refuel(IItem fuel)
    {
        if (!RequiresFuel)
            return CommandResult.Fail($"The {Name} doesn't need fuel.");

        if (fuel.Id != FuelItemId)
            return CommandResult.Fail($"The {Name} doesn't run on {fuel.Name}.");

        var amount = fuel.GetProperty<int>("fuel_amount", 10);
        FuelLevel = Math.Min(FuelLevel + amount, MaxFuel);

        return CommandResult.Ok($"You refuel the {Name}. Fuel: {FuelLevel}/{MaxFuel}");
    }
}
```

### Task 67.3: Common Vehicle Types

```csharp
public static class Vehicles
{
    public static Vehicle Horse() => new()
    {
        Id = "horse",
        Name = "Horse",
        VehicleType = "horse",
        Capacity = 1,
        RequiresFuel = false,
        AllowedTerrains = ["road", "path", "grass", "forest"],
        SpeedMultiplier = 2.0f
    };

    public static Vehicle Car() => new()
    {
        Id = "car",
        Name = "Car",
        VehicleType = "car",
        Capacity = 4,
        RequiresFuel = true,
        FuelItemId = "gasoline",
        MaxFuel = 50,
        AllowedTerrains = ["road", "highway"],
        SpeedMultiplier = 5.0f
    };

    public static Vehicle Boat() => new()
    {
        Id = "boat",
        Name = "Boat",
        VehicleType = "boat",
        Capacity = 6,
        RequiresFuel = false,
        AllowedTerrains = ["water", "river", "lake", "sea"],
        SpeedMultiplier = 1.5f
    };

    public static Vehicle Bicycle() => new()
    {
        Id = "bicycle",
        Name = "Bicycle",
        VehicleType = "bicycle",
        Capacity = 1,
        RequiresFuel = false,
        AllowedTerrains = ["road", "path"],
        SpeedMultiplier = 1.5f
    };
}
```

### Task 67.4: ITeleporter Interface

```csharp
public interface ITeleporter : IItem
{
    string DestinationId { get; }
    bool IsBidirectional { get; }
    bool RequiresActivation { get; }
    string? ActivationItemId { get; }

    CommandResult Teleport(ICharacter character);
}

public class Teleporter : Item, ITeleporter
{
    public string DestinationId { get; init; } = "";
    public bool IsBidirectional { get; init; } = true;
    public bool RequiresActivation { get; init; } = false;
    public string? ActivationItemId { get; init; }

    public bool IsActive { get; private set; } = true;
    public int Cooldown { get; init; } = 0;
    private int _currentCooldown = 0;

    public event Action<ICharacter, ILocation, ILocation>? OnTeleport;

    public CommandResult Teleport(ICharacter character)
    {
        if (RequiresActivation && !IsActive)
            return CommandResult.Fail($"The {Name} is not active.");

        if (_currentCooldown > 0)
            return CommandResult.Fail($"The {Name} is recharging. ({_currentCooldown} turns remaining)");

        var destination = GameWorld.GetLocation(DestinationId);
        if (destination == null)
            return CommandResult.Fail($"The {Name} flickers but nothing happens.");

        var origin = character.CurrentLocation;

        // Perform teleport
        character.MoveTo(destination);
        _currentCooldown = Cooldown;

        OnTeleport?.Invoke(character, origin, destination);

        return CommandResult.Ok($"You step through the {Name} and emerge in {destination.Name}.");
    }

    public CommandResult Activate(IItem? key = null)
    {
        if (!RequiresActivation)
            return CommandResult.Ok($"The {Name} is always active.");

        if (ActivationItemId != null && key?.Id != ActivationItemId)
            return CommandResult.Fail($"The {Name} requires a specific item to activate.");

        IsActive = true;
        return CommandResult.Ok($"The {Name} hums to life.");
    }

    public void OnTick()
    {
        if (_currentCooldown > 0)
            _currentCooldown--;
    }
}
```

### Task 67.5: Teleporter Types

```csharp
public static class Teleporters
{
    public static Teleporter Portal(string destinationId) => new()
    {
        Id = $"portal_to_{destinationId}",
        Name = "Mystical Portal",
        DestinationId = destinationId,
        IsBidirectional = true
    };

    public static Teleporter MagicMirror(string destinationId) => new()
    {
        Id = "magic_mirror",
        Name = "Magic Mirror",
        DestinationId = destinationId,
        RequiresActivation = true,
        ActivationItemId = "mirror_key"
    };

    public static Teleporter WarpPad(string destinationId, int cooldown = 5) => new()
    {
        Id = "warp_pad",
        Name = "Warp Pad",
        DestinationId = destinationId,
        Cooldown = cooldown
    };

    public static Teleporter Elevator(string destinationId) => new()
    {
        Id = "elevator",
        Name = "Elevator",
        DestinationId = destinationId,
        IsBidirectional = true
    };
}
```

### Task 67.6: Commands

```csharp
public class BoardCommand : ICommand
{
    public CommandResult Execute(CommandContext context, string vehicleName)
    {
        var vehicle = context.State.CurrentLocation.Items
            .OfType<IVehicle>()
            .FirstOrDefault(v => v.Name.Contains(vehicleName, StringComparison.OrdinalIgnoreCase));

        if (vehicle == null)
            return CommandResult.Fail($"There's no {vehicleName} here.");

        return vehicle.Board(context.State.Player);
    }
}

public class ExitVehicleCommand : ICommand
{
    public CommandResult Execute(CommandContext context)
    {
        var vehicleId = context.State.Player.GetProperty<string>("in_vehicle");
        if (vehicleId == null)
            return CommandResult.Fail("You're not in a vehicle.");

        var vehicle = context.State.FindItem(vehicleId) as IVehicle;
        return vehicle?.Disembark(context.State.Player)
            ?? CommandResult.Fail("Vehicle not found.");
    }
}

public class DriveCommand : ICommand
{
    public CommandResult Execute(CommandContext context, Direction direction)
    {
        var vehicleId = context.State.Player.GetProperty<string>("in_vehicle");
        if (vehicleId == null)
            return CommandResult.Fail("You're not in a vehicle.");

        var vehicle = context.State.FindItem(vehicleId) as IVehicle;
        if (vehicle == null)
            return CommandResult.Fail("Vehicle not found.");

        var result = vehicle.Drive(direction);
        if (result.Success)
        {
            // Move player with vehicle
            context.State.MovePlayer(direction);
        }

        return result;
    }
}

public class EnterCommand : ICommand
{
    public CommandResult Execute(CommandContext context, string targetName)
    {
        // Check for teleporter first
        var teleporter = context.State.CurrentLocation.Items
            .OfType<ITeleporter>()
            .FirstOrDefault(t => t.Name.Contains(targetName, StringComparison.OrdinalIgnoreCase));

        if (teleporter != null)
            return teleporter.Teleport(context.State.Player);

        // Check for vehicle
        var vehicle = context.State.CurrentLocation.Items
            .OfType<IVehicle>()
            .FirstOrDefault(v => v.Name.Contains(targetName, StringComparison.OrdinalIgnoreCase));

        if (vehicle != null)
            return vehicle.Board(context.State.Player);

        return CommandResult.Fail($"You can't enter {targetName}.");
    }
}
```

### Task 67.7: GameBuilder Integration

```csharp
var game = new GameBuilder("Transport Demo")
    .AddLocation("stable", loc => loc
        .Name("Stable")
        .Description("A rustic stable with hay on the floor.")
        .AddVehicle(Vehicles.Horse()))
    .AddLocation("garage", loc => loc
        .Name("Garage")
        .Description("A modern garage with a car.")
        .AddVehicle(Vehicles.Car())
        .AddItem("gasoline", item => item
            .Name("Jerry Can of Gasoline")
            .WithProperty("fuel_amount", 20)))
    .AddLocation("dock", loc => loc
        .Name("Dock")
        .Description("A wooden dock extending into the lake.")
        .Terrain("water")
        .AddVehicle(Vehicles.Boat()))
    .AddLocation("tower_base", loc => loc
        .Name("Tower Base")
        .Description("The base of a tall wizard's tower.")
        .AddTeleporter(Teleporters.Portal("tower_top")))
    .AddLocation("tower_top", loc => loc
        .Name("Tower Top")
        .Description("The top of the tower with a stunning view.")
        .AddTeleporter(Teleporters.Portal("tower_base")))
    .Build();
```

### Task 67.8: Tester

```csharp
[Fact]
public void Horse_CanTravelOnGrass()
{
    var horse = Vehicles.Horse();

    Assert.Contains("grass", horse.AllowedTerrains);
}

[Fact]
public void Car_RequiresFuel()
{
    var car = Vehicles.Car();

    Assert.True(car.RequiresFuel);
    Assert.Equal("gasoline", car.FuelItemId);
}

[Fact]
public void Teleporter_MovesPlayerToDestination()
{
    var game = TestWorldBuilder.Create()
        .WithLocation("start", loc => loc
            .AddTeleporter(Teleporters.Portal("end")))
        .WithLocation("end")
        .StartAt("start")
        .Build();

    var teleporter = game.State.CurrentLocation.Items.OfType<ITeleporter>().First();
    var result = teleporter.Teleport(game.State.Player);

    Assert.True(result.Success);
    Assert.Equal("end", game.State.CurrentLocation.Id);
}

[Fact]
public void Vehicle_BoardAndDisembark()
{
    var horse = Vehicles.Horse();
    var player = MockFactory.CreatePlayer();

    var boardResult = horse.Board(player);
    Assert.True(boardResult.Success);
    Assert.True(horse.IsBoarded);

    var exitResult = horse.Disembark(player);
    Assert.True(exitResult.Success);
    Assert.False(horse.IsBoarded);
}
```

---

## Completion Checklist
- [x] Core implementation for this slice is present in the engine.
- [x] Behaviour is covered by tests and/or deterministic validation paths.
- [x] Demo document: `67_The_Abandoned_Tram_Depot.md`.
- [x] Marked complete in project slice status.
