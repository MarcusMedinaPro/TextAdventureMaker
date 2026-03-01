## Slice 46: Consumable Items — Eat, Drink & Healing

**Goal:** Wire up the existing food/healing properties on `IItem` to actual gameplay via new `EatCommand` and `DrinkCommand`, integrate with `IStats.Heal()`, and add poison-over-time damage.

### Background

The foundation already exists but is disconnected:
- `IItem` has `IsFood`, `IsPoisoned`, `HealAmount`, `Amount`, `DecreaseAmount()`
- `IStats` has `Heal(int)` and `Damage(int)`
- `UseCommand` calls `item.Use()` which triggers `DecreaseAmount()` but does NOT check `IsFood` or apply healing
- There is no `EatCommand` or `DrinkCommand`
- There is no poison-over-time mechanic

### What This Slice Adds

1. **`EatCommand`** — consume a food item from inventory
2. **`DrinkCommand`** — consume a drinkable item from inventory
3. **`IItem.IsDrinkable`** + `Item.SetDrinkable()` — distinguish food from drink
4. **Healing on consume** — `Stats.Heal(item.HealAmount)` when eating/drinking
5. **Poison effect** — if `IsPoisoned`, apply damage over N turns via a turn-end handler
6. **Amount integration** — consuming decreases amount; item removed at 0
7. **`ConsumableExtensions`** — fluent helpers for building consumable items

### New Files

| File | Purpose |
|------|---------|
| `Commands/EatCommand.cs` | Parse & execute "eat [item]" |
| `Commands/DrinkCommand.cs` | Parse & execute "drink [item]" |
| `Models/PoisonEffect.cs` | Tracks active poison (damage per turn, remaining turns) |

### Modified Files

| File | Change |
|------|--------|
| `Interfaces/IItem.cs` | Add `bool IsDrinkable { get; }`, `IItem SetDrinkable(bool)` |
| `Models/Item.cs` | Add `_isDrinkable` field, `SetDrinkable()`, update `Clone()` |
| `Interfaces/IGameState.cs` | Add `IReadOnlyList<PoisonEffect> ActivePoisons { get; }` |
| `Engine/GameState.cs` | Track active poisons, tick them each turn |
| `Localization/Language.cs` | Add eat/drink/poison messages |

### API Design

```csharp
// Creating consumable items
var bread = new Item("bread", "Bread", "A crusty loaf.")
    .SetFood()
    .SetHealAmount(5)
    .SetAmount(3);

var ale = new Item("ale", "Ale", "A frothy pint.")
    .SetDrinkable()
    .SetHealAmount(2);

var poisonedWine = new Item("wine", "Wine", "A suspiciously dark vintage.")
    .SetDrinkable()
    .SetPoisoned()
    .SetPoisonDamage(3, turns: 4);  // 3 damage per turn for 4 turns

// Commands
// > eat bread
// You eat the Bread. You feel better. (+5 health)

// > drink ale
// You drink the Ale. Refreshing! (+2 health)

// > drink wine
// You drink the Wine. Something tastes wrong...
// [next turn] The poison courses through you. (-3 health)
```

### Poison Effect Design

```csharp
public sealed class PoisonEffect(string sourceName, int damagePerTurn, int remainingTurns)
{
    public string SourceName { get; } = sourceName;
    public int DamagePerTurn { get; } = damagePerTurn;
    public int RemainingTurns { get; private set; } = remainingTurns;
    public bool IsExpired => RemainingTurns <= 0;

    public int Tick()
    {
        if (IsExpired) return 0;
        RemainingTurns--;
        return DamagePerTurn;
    }
}
```

### EatCommand Logic

```
1. Find item in inventory (with fuzzy matching if enabled)
2. If not found → fail "You don't have that."
3. If not IsFood → fail "You can't eat that."
4. Call item.Use() (triggers DecreaseAmount + events)
5. If HealAmount > 0 → state.Stats.Heal(item.HealAmount)
6. If IsPoisoned → add PoisonEffect to state.ActivePoisons
7. If amount == 0 → remove from inventory
8. Return success message with heal amount
```

### DrinkCommand Logic

Same as EatCommand but checks `IsDrinkable` instead of `IsFood`.

### Poison Tick (Turn-End Handler)

```
For each active poison:
  damage = poison.Tick()
  state.Stats.Damage(damage)
  output message: "The poison from {source} burns. (-{damage} health)"
  if expired → remove from list
If health == 0 → trigger death/game-over hook
```

### Required Extensions (Existing)

- `StringExtensions.TextCompare()` — for item matching
- `FuzzyMatcher` — for fuzzy item lookup
- `Language.EntityName()` — for display names

### New Extension Methods

```csharp
// In a new ConsumableExtensions.cs or added to GameEntityExtensions.cs
public static Item AsFood(this Item item, int healAmount) =>
    (Item)item.SetFood().SetHealAmount(healAmount);

public static Item AsDrink(this Item item, int healAmount) =>
    (Item)item.SetDrinkable().SetHealAmount(healAmount);

public static Item WithPoison(this Item item, int damagePerTurn, int turns) =>
    (Item)item.SetPoisoned().SetPoisonDamage(damagePerTurn, turns);
```

### Parser Keywords

| Keyword | Aliases | Command |
|---------|---------|---------|
| `eat` | `consume`, `munch`, `devour` | `EatCommand` |
| `drink` | `sip`, `gulp`, `quaff` | `DrinkCommand` |

### Tests

- Eat food item → heals correctly
- Eat non-food → fails with message
- Eat item not in inventory → fails
- Eat last of stacked item → removed from inventory
- Drink drinkable → heals
- Drink non-drinkable → fails
- Eat poisoned food → applies poison effect
- Poison ticks damage each turn
- Poison expires after N turns
- Eating with Amount=1 removes item

---

## Implementation Checklist (Engine)
- [ ] `IItem.IsDrinkable` + `Item.SetDrinkable()`
- [ ] `IItem.SetPoisonDamage(int, int)` + backing fields
- [ ] `PoisonEffect` model
- [ ] `EatCommand`
- [ ] `DrinkCommand`
- [ ] `GameState` poison tracking + turn tick
- [ ] Parser registration for eat/drink keywords
- [ ] Language strings for eat/drink/poison messages
- [ ] Extension helpers (`AsFood`, `AsDrink`, `WithPoison`)
- [ ] Clone() updated for new fields
- [ ] Tests (minimum 10)

## Example Checklist (docs/examples)
- [ ] Sandbox demo: `46_Consumables.md`

## Dependencies
- None (builds on existing IItem, IStats, GameState)
- Pairs well with Slice 47 (Stackable Items) for stacked consumables
