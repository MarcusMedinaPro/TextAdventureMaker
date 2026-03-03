## Slice 70: Food, Hunger & Survival

**Mål:** Mat, hunger, törst och överlevnadsmekanik.

**Referens:** `docs/plans/slice045.md` (food/healing idea)

### Task 70.1: Survival Stats

```csharp
public interface ISurvivalStats
{
    int Hunger { get; }           // 0 = starving, 100 = full
    int Thirst { get; }           // 0 = dehydrated, 100 = hydrated
    int Energy { get; }           // 0 = exhausted, 100 = rested
    int Temperature { get; }      // Degrees, affected by environment

    bool IsStarving { get; }
    bool IsDehydrated { get; }
    bool IsExhausted { get; }

    void Eat(int amount);
    void Drink(int amount);
    void Rest(int amount);
    void OnTick();
}

public class SurvivalStats : ISurvivalStats
{
    public int Hunger { get; private set; } = 100;
    public int Thirst { get; private set; } = 100;
    public int Energy { get; private set; } = 100;
    public int Temperature { get; private set; } = 37;  // Body temp

    public int HungerDecayRate { get; init; } = 2;
    public int ThirstDecayRate { get; init; } = 3;
    public int EnergyDecayRate { get; init; } = 1;

    public bool IsStarving => Hunger <= 0;
    public bool IsDehydrated => Thirst <= 0;
    public bool IsExhausted => Energy <= 0;

    public event Action<string>? OnStatusChanged;
    public event Action<string>? OnCriticalStatus;

    public void Eat(int amount)
    {
        var oldHunger = Hunger;
        Hunger = Math.Clamp(Hunger + amount, 0, 100);

        if (oldHunger <= 20 && Hunger > 20)
            OnStatusChanged?.Invoke("Your hunger pangs subside.");
    }

    public void Drink(int amount)
    {
        var oldThirst = Thirst;
        Thirst = Math.Clamp(Thirst + amount, 0, 100);

        if (oldThirst <= 20 && Thirst > 20)
            OnStatusChanged?.Invoke("Your thirst is quenched.");
    }

    public void Rest(int amount)
    {
        Energy = Math.Clamp(Energy + amount, 0, 100);
    }

    public void OnTick()
    {
        // Decay over time
        Hunger = Math.Max(0, Hunger - HungerDecayRate);
        Thirst = Math.Max(0, Thirst - ThirstDecayRate);

        // Warnings
        if (Hunger == 20)
            OnStatusChanged?.Invoke("You're getting hungry.");
        if (Thirst == 20)
            OnStatusChanged?.Invoke("You're getting thirsty.");
        if (Energy == 20)
            OnStatusChanged?.Invoke("You're getting tired.");

        // Critical
        if (Hunger == 0)
            OnCriticalStatus?.Invoke("You're starving! Find food immediately!");
        if (Thirst == 0)
            OnCriticalStatus?.Invoke("You're severely dehydrated!");
    }
}
```

### Task 70.2: IEdible & IDrinkable

```csharp
public interface IEdible : IItem
{
    int NutritionValue { get; }
    int HydrationValue { get; }
    bool IsPoisonous { get; }
    int PoisonDamage { get; }
    bool IsRaw { get; }
    bool IsRotten { get; }
    int SpoilsAfterTicks { get; }

    ConsumeResult Consume(ICharacter consumer);
}

public interface IDrinkable : IItem
{
    int HydrationValue { get; }
    bool IsAlcoholic { get; }
    int AlcoholContent { get; }
    bool IsPoisonous { get; }

    ConsumeResult Drink(ICharacter consumer);
}

public record ConsumeResult(
    bool Success,
    string Message,
    int HungerRestored,
    int ThirstRestored,
    int HealthEffect,
    IStatusEffect? AppliedEffect
);
```

### Task 70.3: Food Implementation

```csharp
public class Food : Item, IEdible
{
    public int NutritionValue { get; init; } = 20;
    public int HydrationValue { get; init; } = 0;
    public bool IsPoisonous { get; init; } = false;
    public int PoisonDamage { get; init; } = 0;
    public bool IsRaw { get; init; } = false;
    public bool IsRotten { get; private set; } = false;
    public int SpoilsAfterTicks { get; init; } = -1;  // -1 = never

    private int _ticksExisted = 0;

    public ConsumeResult Consume(ICharacter consumer)
    {
        if (IsRotten)
        {
            consumer.Survival.Eat(NutritionValue / 2);
            consumer.StatusEffects.Apply(new PoisonEffect { Duration = 5 });
            return new ConsumeResult(
                true,
                $"You eat the rotten {Name}. It tastes terrible and makes you feel sick.",
                NutritionValue / 2, 0, -10, new PoisonEffect());
        }

        if (IsPoisonous)
        {
            consumer.Survival.Eat(NutritionValue);
            consumer.Stats.Health -= PoisonDamage;
            consumer.StatusEffects.Apply(new PoisonEffect());
            return new ConsumeResult(
                true,
                $"You eat the {Name}. Something doesn't feel right...",
                NutritionValue, 0, -PoisonDamage, new PoisonEffect());
        }

        if (IsRaw)
        {
            // Raw food gives less nutrition and might cause sickness
            consumer.Survival.Eat(NutritionValue / 2);
            if (Random.Shared.NextDouble() < 0.3)
            {
                consumer.StatusEffects.Apply(new NauseaEffect());
                return new ConsumeResult(
                    true,
                    $"You eat the raw {Name}. Your stomach churns.",
                    NutritionValue / 2, 0, 0, new NauseaEffect());
            }
        }

        consumer.Survival.Eat(NutritionValue);
        consumer.Survival.Drink(HydrationValue);

        var message = NutritionValue switch
        {
            >= 50 => $"You eat the {Name}. Delicious and filling!",
            >= 30 => $"You eat the {Name}. It satisfies your hunger.",
            _ => $"You eat the {Name}. It's a small snack."
        };

        return new ConsumeResult(true, message, NutritionValue, HydrationValue, 0, null);
    }

    public void OnTick()
    {
        if (SpoilsAfterTicks < 0 || IsRotten)
            return;

        _ticksExisted++;
        if (_ticksExisted >= SpoilsAfterTicks)
        {
            IsRotten = true;
            Name = $"Rotten {Name}";
        }
    }
}
```

### Task 70.4: Common Foods

```csharp
public static class Foods
{
    public static Food Apple() => new()
    {
        Id = "apple",
        Name = "Apple",
        Description = "A crisp, red apple.",
        NutritionValue = 15,
        HydrationValue = 5,
        SpoilsAfterTicks = 100
    };

    public static Food Bread() => new()
    {
        Id = "bread",
        Name = "Bread",
        Description = "A loaf of fresh bread.",
        NutritionValue = 30,
        SpoilsAfterTicks = 50
    };

    public static Food CookedMeat() => new()
    {
        Id = "cooked_meat",
        Name = "Cooked Meat",
        Description = "Well-cooked meat.",
        NutritionValue = 50,
        SpoilsAfterTicks = 30
    };

    public static Food RawMeat() => new()
    {
        Id = "raw_meat",
        Name = "Raw Meat",
        Description = "Uncooked meat. Should be cooked first.",
        NutritionValue = 40,
        IsRaw = true,
        SpoilsAfterTicks = 15
    };

    public static Food Berries() => new()
    {
        Id = "berries",
        Name = "Wild Berries",
        Description = "A handful of wild berries.",
        NutritionValue = 10,
        HydrationValue = 3
    };

    public static Food PoisonousMushroom() => new()
    {
        Id = "poison_mushroom",
        Name = "Strange Mushroom",
        Description = "A mushroom with unusual colouring.",
        NutritionValue = 5,
        IsPoisonous = true,
        PoisonDamage = 20
    };

    public static Food HealingHerb() => new()
    {
        Id = "healing_herb",
        Name = "Healing Herb",
        Description = "A herb with restorative properties.",
        NutritionValue = 5
    };
}
```

### Task 70.5: Beverages

```csharp
public class Beverage : Item, IDrinkable
{
    public int HydrationValue { get; init; } = 20;
    public bool IsAlcoholic { get; init; } = false;
    public int AlcoholContent { get; init; } = 0;
    public bool IsPoisonous { get; init; } = false;

    public ConsumeResult Drink(ICharacter consumer)
    {
        if (IsPoisonous)
        {
            consumer.Survival.Drink(HydrationValue / 2);
            consumer.Stats.Health -= 15;
            consumer.StatusEffects.Apply(new PoisonEffect());
            return new ConsumeResult(
                true,
                $"You drink the {Name}. It burns your throat!",
                0, HydrationValue / 2, -15, new PoisonEffect());
        }

        if (IsAlcoholic)
        {
            consumer.Survival.Drink(HydrationValue);
            consumer.StatusEffects.Apply(new IntoxicatedEffect { Strength = AlcoholContent });
            return new ConsumeResult(
                true,
                $"You drink the {Name}. You feel a bit lightheaded.",
                0, HydrationValue, 0, new IntoxicatedEffect());
        }

        consumer.Survival.Drink(HydrationValue);
        return new ConsumeResult(
            true,
            $"You drink the {Name}. Refreshing!",
            0, HydrationValue, 0, null);
    }
}

public static class Beverages
{
    public static Beverage Water() => new()
    {
        Id = "water",
        Name = "Water",
        Description = "Clean, fresh water.",
        HydrationValue = 30
    };

    public static Beverage Ale() => new()
    {
        Id = "ale",
        Name = "Ale",
        Description = "A mug of frothy ale.",
        HydrationValue = 15,
        IsAlcoholic = true,
        AlcoholContent = 2
    };

    public static Beverage Wine() => new()
    {
        Id = "wine",
        Name = "Wine",
        Description = "A bottle of red wine.",
        HydrationValue = 10,
        IsAlcoholic = true,
        AlcoholContent = 4
    };

    public static Beverage HealingPotion() => new()
    {
        Id = "healing_potion",
        Name = "Healing Potion",
        Description = "A glowing red potion.",
        HydrationValue = 5
    };
}
```

### Task 70.6: Cooking System

```csharp
public interface ICookable : IItem
{
    IItem Cook();
    int CookingTime { get; }
}

public static class CookingExtensions
{
    public static bool CanCook(this IItem item) => item is ICookable;

    public static IItem Cook(this ICookable item, IItem? cookingAppliance = null)
    {
        if (item is Food food && food.IsRaw)
        {
            return new Food
            {
                Id = $"cooked_{food.Id}",
                Name = $"Cooked {food.Name}",
                Description = $"Well-cooked {food.Name.ToLower()}.",
                NutritionValue = food.NutritionValue,  // Full nutrition when cooked
                IsRaw = false,
                SpoilsAfterTicks = food.SpoilsAfterTicks * 2  // Lasts longer cooked
            };
        }

        return item as IItem ?? throw new InvalidOperationException();
    }
}
```

### Task 70.7: Commands

```csharp
public class EatCommand : ICommand
{
    public CommandResult Execute(CommandContext context, string itemName)
    {
        var item = context.State.Player.Inventory.FindItem(itemName);
        if (item == null)
            return CommandResult.Fail($"You don't have any {itemName}.");

        if (item is not IEdible food)
            return CommandResult.Fail($"You can't eat the {item.Name}.");

        var result = food.Consume(context.State.Player);

        if (result.Success)
            context.State.Player.Inventory.Remove(item);

        return CommandResult.Ok(result.Message);
    }
}

public class DrinkCommand : ICommand
{
    public CommandResult Execute(CommandContext context, string itemName)
    {
        var item = context.State.Player.Inventory.FindItem(itemName);
        if (item == null)
            return CommandResult.Fail($"You don't have any {itemName}.");

        if (item is not IDrinkable beverage)
            return CommandResult.Fail($"You can't drink the {item.Name}.");

        var result = beverage.Drink(context.State.Player);

        if (result.Success)
            context.State.Player.Inventory.Remove(item);

        return CommandResult.Ok(result.Message);
    }
}

public class HungerCommand : ICommand
{
    public CommandResult Execute(CommandContext context)
    {
        var survival = context.State.Player.Survival;

        var sb = new StringBuilder();
        sb.AppendLine("=== Survival Status ===");
        sb.AppendLine($"Hunger: {GetBar(survival.Hunger)} {survival.Hunger}%");
        sb.AppendLine($"Thirst: {GetBar(survival.Thirst)} {survival.Thirst}%");
        sb.AppendLine($"Energy: {GetBar(survival.Energy)} {survival.Energy}%");

        return CommandResult.Ok(sb.ToString());
    }

    private string GetBar(int value) =>
        $"[{new string('█', value / 10)}{new string('░', 10 - value / 10)}]";
}
```

### Task 70.8: Tester

```csharp
[Fact]
public void Eating_RestoresHunger()
{
    var player = CreatePlayer();
    player.Survival.Hunger = 50;

    var apple = Foods.Apple();
    apple.Consume(player);

    Assert.Equal(65, player.Survival.Hunger);
}

[Fact]
public void PoisonousFood_CausesDamage()
{
    var player = CreatePlayer();
    var initialHealth = player.Stats.Health;

    var mushroom = Foods.PoisonousMushroom();
    mushroom.Consume(player);

    Assert.True(player.Stats.Health < initialHealth);
    Assert.True(player.StatusEffects.Has("poison"));
}

[Fact]
public void Food_SpoilsOverTime()
{
    var meat = new Food
    {
        Name = "Meat",
        SpoilsAfterTicks = 5
    };

    for (int i = 0; i < 5; i++)
        meat.OnTick();

    Assert.True(meat.IsRotten);
}

[Fact]
public void Survival_DecaysOverTime()
{
    var survival = new SurvivalStats();

    survival.OnTick();

    Assert.True(survival.Hunger < 100);
    Assert.True(survival.Thirst < 100);
}
```

---

## Completion Checklist
- [x] Core implementation for this slice is present in the engine.
- [x] Behaviour is covered by tests and/or deterministic validation paths.
- [x] Demo document: `70_Cold_Night_Survival.md`.
- [x] Marked complete in project slice status.
