## Slice 64: Status Effects System

**Mål:** Temporära tillståndseffekter som påverkar spelaren och NPCs (förgiftad, brinnande, välsignad, etc.)

**Referens:** `docs/plans/imported/moar.md`

### Task 64.1: IStatusEffect interface

```csharp
public interface IStatusEffect
{
    string Id { get; }
    string Name { get; }
    string Description { get; }
    StatusEffectType Type { get; }
    int Duration { get; }          // -1 = permanent tills borttaget
    int RemainingDuration { get; }
    bool IsExpired { get; }

    void OnApply(ICharacter target);
    void OnTick(ICharacter target);
    void OnRemove(ICharacter target);
    void OnStack(IStatusEffect existing);  // När samma effekt appliceras igen
}

public enum StatusEffectType
{
    Buff,       // Positiv effekt
    Debuff,     // Negativ effekt
    Neutral     // Varken eller
}
```

### Task 64.2: StatusEffect base implementation

```csharp
public abstract class StatusEffect : IStatusEffect
{
    public string Id { get; init; } = "";
    public string Name { get; init; } = "";
    public string Description { get; init; } = "";
    public StatusEffectType Type { get; init; } = StatusEffectType.Neutral;
    public int Duration { get; init; } = -1;
    public int RemainingDuration { get; protected set; }
    public bool IsExpired => Duration > 0 && RemainingDuration <= 0;

    public int StackCount { get; protected set; } = 1;
    public int MaxStacks { get; init; } = 1;

    public virtual void OnApply(ICharacter target)
    {
        RemainingDuration = Duration;
    }

    public virtual void OnTick(ICharacter target)
    {
        if (Duration > 0)
            RemainingDuration--;
    }

    public virtual void OnRemove(ICharacter target) { }

    public virtual void OnStack(IStatusEffect existing)
    {
        if (StackCount < MaxStacks)
        {
            StackCount++;
            RemainingDuration = Duration;  // Refresh duration
        }
    }
}
```

### Task 64.3: Common Status Effects

```csharp
public class PoisonEffect : StatusEffect
{
    public int DamagePerTick { get; init; } = 5;

    public PoisonEffect()
    {
        Id = "poison";
        Name = "Poisoned";
        Description = "Taking damage over time from poison.";
        Type = StatusEffectType.Debuff;
        Duration = 5;
        MaxStacks = 3;
    }

    public override void OnTick(ICharacter target)
    {
        base.OnTick(target);
        var damage = DamagePerTick * StackCount;
        target.Stats.Health -= damage;
        target.AddMessage($"The poison deals {damage} damage.");
    }

    public override void OnRemove(ICharacter target)
    {
        target.AddMessage("The poison fades from your system.");
    }
}

public class BurningEffect : StatusEffect
{
    public int DamagePerTick { get; init; } = 10;

    public BurningEffect()
    {
        Id = "burning";
        Name = "Burning";
        Description = "On fire! Taking heavy damage.";
        Type = StatusEffectType.Debuff;
        Duration = 3;
    }

    public override void OnTick(ICharacter target)
    {
        base.OnTick(target);
        target.Stats.Health -= DamagePerTick;
        target.AddMessage("The flames sear your flesh!");
    }

    public override void OnRemove(ICharacter target)
    {
        target.AddMessage("The flames die out.");
    }
}

public class BlessedEffect : StatusEffect
{
    public int DamageBonus { get; init; } = 5;
    public int DefenseBonus { get; init; } = 3;

    public BlessedEffect()
    {
        Id = "blessed";
        Name = "Blessed";
        Description = "Divine protection surrounds you.";
        Type = StatusEffectType.Buff;
        Duration = 10;
    }

    public override void OnApply(ICharacter target)
    {
        base.OnApply(target);
        target.Stats.ModifyDamage(DamageBonus);
        target.Stats.ModifyDefense(DefenseBonus);
        target.AddMessage("A warm light envelops you.");
    }

    public override void OnRemove(ICharacter target)
    {
        target.Stats.ModifyDamage(-DamageBonus);
        target.Stats.ModifyDefense(-DefenseBonus);
        target.AddMessage("The blessing fades.");
    }
}

public class InvisibleEffect : StatusEffect
{
    public InvisibleEffect()
    {
        Id = "invisible";
        Name = "Invisible";
        Description = "You cannot be seen by enemies.";
        Type = StatusEffectType.Buff;
        Duration = 5;
    }

    public override void OnApply(ICharacter target)
    {
        base.OnApply(target);
        target.SetProperty("visible", false);
        target.AddMessage("You fade from sight.");
    }

    public override void OnRemove(ICharacter target)
    {
        target.SetProperty("visible", true);
        target.AddMessage("You become visible again.");
    }
}

public class StunnedEffect : StatusEffect
{
    public StunnedEffect()
    {
        Id = "stunned";
        Name = "Stunned";
        Description = "Unable to act.";
        Type = StatusEffectType.Debuff;
        Duration = 2;
    }

    public override void OnApply(ICharacter target)
    {
        base.OnApply(target);
        target.SetProperty("can_act", false);
        target.AddMessage("You are stunned!");
    }

    public override void OnRemove(ICharacter target)
    {
        target.SetProperty("can_act", true);
        target.AddMessage("You shake off the stun.");
    }
}

public class RegeneratingEffect : StatusEffect
{
    public int HealPerTick { get; init; } = 3;

    public RegeneratingEffect()
    {
        Id = "regenerating";
        Name = "Regenerating";
        Description = "Slowly healing over time.";
        Type = StatusEffectType.Buff;
        Duration = 10;
    }

    public override void OnTick(ICharacter target)
    {
        base.OnTick(target);
        var healed = Math.Min(HealPerTick, target.Stats.MaxHealth - target.Stats.Health);
        if (healed > 0)
        {
            target.Stats.Health += healed;
            target.AddMessage($"You regenerate {healed} health.");
        }
    }
}
```

### Task 64.4: StatusEffectManager

```csharp
public class StatusEffectManager
{
    private readonly List<IStatusEffect> _effects = [];
    private readonly ICharacter _owner;

    public StatusEffectManager(ICharacter owner)
    {
        _owner = owner;
    }

    public IReadOnlyList<IStatusEffect> ActiveEffects => _effects.AsReadOnly();

    public void Apply(IStatusEffect effect)
    {
        var existing = _effects.FirstOrDefault(e => e.Id == effect.Id);

        if (existing != null)
        {
            effect.OnStack(existing);
        }
        else
        {
            effect.OnApply(_owner);
            _effects.Add(effect);
        }
    }

    public void Remove(string effectId)
    {
        var effect = _effects.FirstOrDefault(e => e.Id == effectId);
        if (effect != null)
        {
            effect.OnRemove(_owner);
            _effects.Remove(effect);
        }
    }

    public void RemoveAll(StatusEffectType type)
    {
        var toRemove = _effects.Where(e => e.Type == type).ToList();
        foreach (var effect in toRemove)
        {
            effect.OnRemove(_owner);
            _effects.Remove(effect);
        }
    }

    public bool Has(string effectId) => _effects.Any(e => e.Id == effectId);

    public void OnTick()
    {
        foreach (var effect in _effects.ToList())
        {
            effect.OnTick(_owner);

            if (effect.IsExpired)
            {
                effect.OnRemove(_owner);
                _effects.Remove(effect);
            }
        }
    }

    public string GetStatusSummary()
    {
        if (!_effects.Any())
            return "No active effects.";

        return string.Join(", ", _effects.Select(e =>
            e.Duration > 0 ? $"{e.Name} ({e.RemainingDuration})" : e.Name));
    }
}
```

### Task 64.5: Integration med Stats

```csharp
public interface ICharacter
{
    IStats Stats { get; }
    StatusEffectManager StatusEffects { get; }
    void AddMessage(string message);
    void SetProperty(string key, object value);
}

// I GameState eller Player:
public class Player : ICharacter
{
    public IStats Stats { get; } = new Stats();
    public StatusEffectManager StatusEffects { get; }

    public Player()
    {
        StatusEffects = new StatusEffectManager(this);
    }
}

// I game loop:
public void OnTurnEnd()
{
    State.Player.StatusEffects.OnTick();

    foreach (var npc in State.CurrentLocation.Npcs)
    {
        npc.StatusEffects.OnTick();
    }
}
```

### Task 64.6: Commands för Status Effects

```csharp
public class StatusCommand : ICommand
{
    public CommandResult Execute(CommandContext context)
    {
        var summary = context.State.Player.StatusEffects.GetStatusSummary();
        return CommandResult.Ok($"Status effects: {summary}");
    }
}

public class CureCommand(string effectId) : ICommand
{
    public CommandResult Execute(CommandContext context)
    {
        if (!context.State.Player.StatusEffects.Has(effectId))
            return CommandResult.Fail($"You don't have {effectId}.");

        context.State.Player.StatusEffects.Remove(effectId);
        return CommandResult.Ok($"Cured {effectId}.");
    }
}
```

### Task 64.7: Items som applicerar effekter

```csharp
// Poison Dagger
var poisonDagger = new Item("poison_dagger", "Poison Dagger")
    .OnUse((ctx, target) =>
    {
        if (target is INpc npc)
        {
            npc.StatusEffects.Apply(new PoisonEffect());
            return "You strike with the poisoned blade!";
        }
        return "Nothing to strike.";
    });

// Healing Potion
var healingPotion = new Item("healing_potion", "Healing Potion")
    .IsConsumable()
    .OnUse((ctx, _) =>
    {
        ctx.State.Player.StatusEffects.Apply(new RegeneratingEffect { Duration = 5 });
        ctx.State.Player.StatusEffects.RemoveAll(StatusEffectType.Debuff);
        return "You drink the potion. Warmth spreads through your body.";
    });

// Torch (prevents darkness debuff)
var torch = new Item("torch", "Torch")
    .IsLightSource()
    .OnEquip((ctx) =>
    {
        ctx.State.Player.StatusEffects.Remove("blinded");
    });
```

### Task 64.8: Tester

```csharp
[Fact]
public void PoisonEffect_DealsDamageOverTime()
{
    var player = CreatePlayer(health: 100);
    player.StatusEffects.Apply(new PoisonEffect { DamagePerTick = 5, Duration = 3 });

    player.StatusEffects.OnTick();
    Assert.Equal(95, player.Stats.Health);

    player.StatusEffects.OnTick();
    Assert.Equal(90, player.Stats.Health);

    player.StatusEffects.OnTick();
    Assert.Equal(85, player.Stats.Health);

    // Effect expired
    Assert.False(player.StatusEffects.Has("poison"));
}

[Fact]
public void StatusEffect_StacksCorrectly()
{
    var player = CreatePlayer();
    player.StatusEffects.Apply(new PoisonEffect { MaxStacks = 3 });
    player.StatusEffects.Apply(new PoisonEffect { MaxStacks = 3 });
    player.StatusEffects.Apply(new PoisonEffect { MaxStacks = 3 });

    var poison = player.StatusEffects.ActiveEffects.First();
    Assert.Equal(3, ((PoisonEffect)poison).StackCount);
}

[Fact]
public void BlessedEffect_ModifiesStats()
{
    var player = CreatePlayer();
    var baseDamage = player.Stats.Damage;

    player.StatusEffects.Apply(new BlessedEffect { DamageBonus = 5 });

    Assert.Equal(baseDamage + 5, player.Stats.Damage);

    player.StatusEffects.Remove("blessed");

    Assert.Equal(baseDamage, player.Stats.Damage);
}
```

### Task 64.9: Sandbox — giftig grotta

Demo med giftiga fiender, brinnande fällor och välsignande altare.

---

## Completion Checklist
- [x] Core implementation for this slice is present in the engine.
- [x] Behaviour is covered by tests and/or deterministic validation paths.
- [x] Demo document: `64_Poison_in_the_Rain.md`.
- [x] Marked complete in project slice status.
