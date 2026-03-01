## Slice 72: Machines & Electronics

**Mål:** Elektriska och mekaniska maskiner som kan interageras med.

**Referens:** `docs/plans/slice045.md` (electrical machines idea)

### Task 72.1: IMachine Interface

```csharp
public interface IMachine : IItem
{
    MachineState State { get; }
    bool IsPowered { get; }
    bool RequiresPower { get; }
    PowerType PowerType { get; }

    // Fuel-based machines
    bool RequiresFuel { get; }
    string? FuelItemId { get; }
    int FuelLevel { get; }
    int MaxFuel { get; }

    CommandResult TurnOn();
    CommandResult TurnOff();
    CommandResult Use(ICharacter user, string? parameter = null);
    CommandResult Repair(IItem? tool = null);
    CommandResult Refuel(IItem fuel);

    void OnTick();
}

public enum MachineState
{
    Off,
    On,
    Running,
    Broken,
    OutOfFuel,
    NoPower
}

public enum PowerType
{
    None,
    Battery,
    Electric,
    Steam,
    Magic,
    Solar
}
```

### Task 72.2: Machine Base Implementation

```csharp
public class Machine : Item, IMachine
{
    public MachineState State { get; protected set; } = MachineState.Off;
    public bool RequiresPower { get; init; } = true;
    public PowerType PowerType { get; init; } = PowerType.Electric;

    public bool RequiresFuel { get; init; } = false;
    public string? FuelItemId { get; init; }
    public int FuelLevel { get; protected set; }
    public int MaxFuel { get; init; } = 100;
    public int FuelConsumptionRate { get; init; } = 1;

    public int Durability { get; protected set; } = 100;
    public int MaxDurability { get; init; } = 100;

    public bool IsPowered
    {
        get
        {
            if (!RequiresPower)
                return true;

            if (RequiresFuel && FuelLevel <= 0)
                return false;

            // Check for power source in location
            return CheckPowerAvailable();
        }
    }

    public event Action<IMachine>? OnStateChanged;
    public event Action<IMachine, string>? OnOutput;

    public virtual CommandResult TurnOn()
    {
        if (State == MachineState.Broken)
            return CommandResult.Fail($"The {Name} is broken and needs repair.");

        if (!IsPowered)
        {
            State = RequiresFuel ? MachineState.OutOfFuel : MachineState.NoPower;
            return CommandResult.Fail($"The {Name} has no power.");
        }

        State = MachineState.On;
        OnStateChanged?.Invoke(this);
        return CommandResult.Ok($"You turn on the {Name}.");
    }

    public virtual CommandResult TurnOff()
    {
        if (State == MachineState.Off)
            return CommandResult.Fail($"The {Name} is already off.");

        State = MachineState.Off;
        OnStateChanged?.Invoke(this);
        return CommandResult.Ok($"You turn off the {Name}.");
    }

    public virtual CommandResult Use(ICharacter user, string? parameter = null)
    {
        if (State != MachineState.On && State != MachineState.Running)
            return CommandResult.Fail($"The {Name} needs to be turned on first.");

        State = MachineState.Running;
        return CommandResult.Ok($"You use the {Name}.");
    }

    public virtual CommandResult Repair(IItem? tool = null)
    {
        if (State != MachineState.Broken)
            return CommandResult.Ok($"The {Name} doesn't need repair.");

        if (tool == null || !tool.HasTag("repair_tool"))
            return CommandResult.Fail("You need a repair tool.");

        Durability = MaxDurability / 2;  // Repairs to 50%
        State = MachineState.Off;
        return CommandResult.Ok($"You repair the {Name}. It should work now.");
    }

    public virtual CommandResult Refuel(IItem fuel)
    {
        if (!RequiresFuel)
            return CommandResult.Fail($"The {Name} doesn't need fuel.");

        if (fuel.Id != FuelItemId)
            return CommandResult.Fail($"The {Name} doesn't run on {fuel.Name}.");

        var amount = fuel.GetProperty<int>("fuel_amount", 10);
        FuelLevel = Math.Min(FuelLevel + amount, MaxFuel);

        if (State == MachineState.OutOfFuel)
            State = MachineState.Off;

        return CommandResult.Ok($"You refuel the {Name}. Fuel: {FuelLevel}/{MaxFuel}");
    }

    public virtual void OnTick()
    {
        if (State != MachineState.Running)
            return;

        // Consume fuel
        if (RequiresFuel)
        {
            FuelLevel -= FuelConsumptionRate;
            if (FuelLevel <= 0)
            {
                FuelLevel = 0;
                State = MachineState.OutOfFuel;
                OnStateChanged?.Invoke(this);
            }
        }

        // Wear down
        Durability--;
        if (Durability <= 0)
        {
            Durability = 0;
            State = MachineState.Broken;
            OnStateChanged?.Invoke(this);
        }
    }

    protected virtual bool CheckPowerAvailable()
    {
        if (PowerType == PowerType.None)
            return true;

        // Check location for power source
        var location = GameWorld.Instance.GetLocationContaining(this);
        return location?.HasPower(PowerType) ?? false;
    }
}
```

### Task 72.3: Specific Machine Types

```csharp
public class Lamp : Machine, ILightSource
{
    public LightType Type => LightType.Warm;
    public float Intensity { get; init; } = 1.0f;
    public float Radius { get; init; } = 3.0f;
    public bool IsLit => State == MachineState.On || State == MachineState.Running;
    public int? FuelRemaining => RequiresFuel ? FuelLevel : null;

    public Lamp()
    {
        Id = "lamp";
        Name = "Electric Lamp";
        RequiresPower = true;
        PowerType = PowerType.Electric;
    }

    public void Light() => TurnOn();
    public void Extinguish() => TurnOff();
}

public class Computer : Machine
{
    public bool IsLoggedIn { get; private set; }
    public string? CurrentUser { get; private set; }
    private readonly List<string> _files = [];

    public Computer()
    {
        Id = "computer";
        Name = "Computer Terminal";
        RequiresPower = true;
        PowerType = PowerType.Electric;
    }

    public override CommandResult Use(ICharacter user, string? parameter = null)
    {
        if (State != MachineState.On)
            return CommandResult.Fail("The computer is off.");

        if (parameter == null)
            return CommandResult.Ok("The screen displays a login prompt.");

        var parts = parameter.Split(' ', 2);
        var command = parts[0].ToLower();

        return command switch
        {
            "login" => Login(parts.Length > 1 ? parts[1] : null),
            "logout" => Logout(),
            "dir" or "ls" => ListFiles(),
            "read" => ReadFile(parts.Length > 1 ? parts[1] : null),
            "help" => ShowHelp(),
            _ => CommandResult.Fail($"Unknown command: {command}")
        };
    }

    private CommandResult Login(string? password)
    {
        if (password == "1234" || password == "admin")
        {
            IsLoggedIn = true;
            CurrentUser = "admin";
            return CommandResult.Ok("Login successful. Welcome, admin.");
        }
        return CommandResult.Fail("Access denied. Invalid password.");
    }

    private CommandResult Logout()
    {
        IsLoggedIn = false;
        CurrentUser = null;
        return CommandResult.Ok("Logged out.");
    }

    private CommandResult ListFiles()
    {
        if (!IsLoggedIn)
            return CommandResult.Fail("Please log in first.");

        return CommandResult.Ok($"Files: {string.Join(", ", _files)}");
    }

    private CommandResult ReadFile(string? filename)
    {
        if (!IsLoggedIn)
            return CommandResult.Fail("Please log in first.");

        // Return file content based on filename
        return CommandResult.Ok($"Contents of {filename}...");
    }

    private CommandResult ShowHelp()
    {
        return CommandResult.Ok("Commands: login, logout, dir, read <file>, help");
    }
}

public class Generator : Machine
{
    public int PowerOutput { get; init; } = 100;
    public bool IsPoweringLocation { get; private set; }

    public Generator()
    {
        Id = "generator";
        Name = "Diesel Generator";
        RequiresFuel = true;
        FuelItemId = "diesel";
        PowerType = PowerType.None;  // It provides power, doesn't need it
    }

    public override CommandResult TurnOn()
    {
        var result = base.TurnOn();
        if (result.Success)
        {
            IsPoweringLocation = true;
            var location = GameWorld.Instance.GetLocationContaining(this);
            location?.SetPowerSource(this);
        }
        return result;
    }

    public override CommandResult TurnOff()
    {
        var result = base.TurnOff();
        if (result.Success)
        {
            IsPoweringLocation = false;
            var location = GameWorld.Instance.GetLocationContaining(this);
            location?.RemovePowerSource(this);
        }
        return result;
    }
}

public class VendingMachine : Machine
{
    private readonly Dictionary<string, (IItem Item, int Price, int Stock)> _inventory = [];

    public VendingMachine()
    {
        Id = "vending_machine";
        Name = "Vending Machine";
        RequiresPower = true;
        PowerType = PowerType.Electric;
    }

    public void AddProduct(IItem item, int price, int stock)
    {
        _inventory[item.Id] = (item.Clone(), price, stock);
    }

    public override CommandResult Use(ICharacter user, string? parameter = null)
    {
        if (State != MachineState.On)
            return CommandResult.Fail("The vending machine is off.");

        if (parameter == null)
        {
            var sb = new StringBuilder();
            sb.AppendLine("=== Vending Machine ===");
            foreach (var (id, (item, price, stock)) in _inventory)
            {
                var status = stock > 0 ? $"${price}" : "SOLD OUT";
                sb.AppendLine($"  {item.Name}: {status}");
            }
            return CommandResult.Ok(sb.ToString());
        }

        // Try to buy
        var product = _inventory.FirstOrDefault(p =>
            p.Value.Item.Name.Contains(parameter, StringComparison.OrdinalIgnoreCase));

        if (product.Key == null)
            return CommandResult.Fail($"No product called '{parameter}'.");

        var (purchaseItem, purchasePrice, purchaseStock) = product.Value;

        if (purchaseStock <= 0)
            return CommandResult.Fail("That item is sold out.");

        var money = user.GetMoney();
        if (money < purchasePrice)
            return CommandResult.Fail($"You need ${purchasePrice} but only have ${money}.");

        user.SpendMoney(purchasePrice);
        user.Inventory.Add(purchaseItem.Clone());
        _inventory[product.Key] = (purchaseItem, purchasePrice, purchaseStock - 1);

        return CommandResult.Ok($"You buy a {purchaseItem.Name} for ${purchasePrice}.");
    }
}
```

### Task 72.4: Power System

```csharp
public static class LocationPowerExtensions
{
    public static bool HasPower(this ILocation location, PowerType type)
    {
        // Check for power sources
        var generators = location.Items.OfType<Generator>()
            .Where(g => g.State == MachineState.Running);

        if (generators.Any())
            return true;

        // Check for power property
        return location.GetProperty<bool>($"has_{type.ToString().ToLower()}_power");
    }

    public static void SetPowerSource(this ILocation location, Generator generator)
    {
        location.SetProperty("has_electric_power", true);
        location.SetProperty("power_source", generator.Id);
    }

    public static void RemovePowerSource(this ILocation location, Generator generator)
    {
        var currentSource = location.GetProperty<string>("power_source");
        if (currentSource == generator.Id)
        {
            location.SetProperty("has_electric_power", false);
            location.SetProperty("power_source", null);
        }
    }

    public static LocationBuilder WithPower(this LocationBuilder builder, PowerType type = PowerType.Electric)
    {
        builder.WithProperty($"has_{type.ToString().ToLower()}_power", true);
        return builder;
    }
}
```

### Task 72.5: Commands

```csharp
public class TurnOnCommand : ICommand
{
    public CommandResult Execute(CommandContext context, string machineName)
    {
        var machine = FindMachine(context, machineName);
        if (machine == null)
            return CommandResult.Fail($"There's no {machineName} here.");

        return machine.TurnOn();
    }
}

public class TurnOffCommand : ICommand
{
    public CommandResult Execute(CommandContext context, string machineName)
    {
        var machine = FindMachine(context, machineName);
        if (machine == null)
            return CommandResult.Fail($"There's no {machineName} here.");

        return machine.TurnOff();
    }
}

public class OperateCommand : ICommand
{
    public CommandResult Execute(CommandContext context, string machineName, string? parameter = null)
    {
        var machine = FindMachine(context, machineName);
        if (machine == null)
            return CommandResult.Fail($"There's no {machineName} here.");

        return machine.Use(context.State.Player, parameter);
    }
}

public class RepairCommand : ICommand
{
    public CommandResult Execute(CommandContext context, string machineName)
    {
        var machine = FindMachine(context, machineName);
        if (machine == null)
            return CommandResult.Fail($"There's no {machineName} here.");

        var tool = context.State.Player.Inventory.Items
            .FirstOrDefault(i => i.HasTag("repair_tool"));

        return machine.Repair(tool);
    }
}

public class SmashCommand : ICommand
{
    public CommandResult Execute(CommandContext context, string machineName)
    {
        var machine = FindMachine(context, machineName);
        if (machine == null)
            return CommandResult.Fail($"There's no {machineName} here.");

        if (machine is Machine m)
        {
            m.Durability = 0;
            m.State = MachineState.Broken;
            return CommandResult.Ok($"You smash the {machine.Name}. It's broken now.");
        }

        return CommandResult.Fail($"You can't smash the {machineName}.");
    }
}
```

### Task 72.6: GameBuilder Integration

```csharp
var game = new GameBuilder("Machine Demo")
    .AddLocation("control_room", loc => loc
        .Name("Control Room")
        .Description("Banks of computers line the walls.")
        .WithPower()
        .AddMachine(new Computer())
        .AddMachine(new Lamp { Name = "Desk Lamp" }))
    .AddLocation("basement", loc => loc
        .Name("Basement")
        .Description("A dark basement with a generator.")
        .AddMachine(new Generator { FuelLevel = 50 })
        .AddItem("diesel", item => item
            .Name("Diesel Canister")
            .WithProperty("fuel_amount", 30)))
    .AddLocation("lobby", loc => loc
        .Name("Lobby")
        .Description("The building's lobby.")
        .AddMachine(new VendingMachine()
            .WithProduct(new Item("soda", "Soda"), 2, 5)
            .WithProduct(new Item("snack", "Snack Bar"), 3, 3)))
    .Build();
```

### Task 72.7: Tester

```csharp
[Fact]
public void Machine_RequiresPower()
{
    var lamp = new Lamp { RequiresPower = true };

    var result = lamp.TurnOn();

    Assert.False(result.Success);
    Assert.Equal(MachineState.NoPower, lamp.State);
}

[Fact]
public void Generator_ProvidesPower()
{
    var location = CreateLocation();
    var generator = new Generator { FuelLevel = 50 };
    location.Items.Add(generator);

    generator.TurnOn();

    Assert.True(location.HasPower(PowerType.Electric));
}

[Fact]
public void Machine_ConsumesFuel()
{
    var generator = new Generator
    {
        FuelLevel = 10,
        FuelConsumptionRate = 2
    };

    generator.TurnOn();
    generator.OnTick();

    Assert.Equal(8, generator.FuelLevel);
}

[Fact]
public void Machine_BreaksWhenDurabilityDepleted()
{
    var machine = new Machine
    {
        Durability = 1,
        RequiresPower = false
    };

    machine.TurnOn();
    machine.Use(CreatePlayer());
    machine.OnTick();

    Assert.Equal(MachineState.Broken, machine.State);
}

[Fact]
public void VendingMachine_SellsProducts()
{
    var player = CreatePlayer();
    player.SetMoney(10);

    var vending = new VendingMachine();
    vending.AddProduct(new Item("soda", "Soda"), 2, 5);
    vending.TurnOn();

    var result = vending.Use(player, "soda");

    Assert.True(result.Success);
    Assert.Equal(8, player.GetMoney());
    Assert.True(player.Inventory.HasItem("soda"));
}
```

---
