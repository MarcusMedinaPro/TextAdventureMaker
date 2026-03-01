## Slice 47: Stackable Items & Inventory Grouping

**Goal:** Make stackable items behave as a single inventory entry with an amount, auto-merge on pickup, split on drop, and display grouped in inventory listings.

### Background

The foundation exists but is not wired:
- `IItem.IsStackable` and `Item.SetStackable()` exist
- `IItem.Amount` and `Item.SetAmount(int)` / `DecreaseAmount()` exist
- `IItem.OnAmountEmpty` event exists
- `Inventory.Add()` always adds a new entry — no merging
- `InventoryCommand` lists each item individually — no grouping
- `TakeCommand` / `DropCommand` have no stack awareness

### What This Slice Adds

1. **Stack merging on Add** — when a stackable item is added to inventory and an item with the same Id already exists, increase the amount instead of adding a duplicate
2. **Stack splitting on Drop** — "drop 2 arrows" drops a partial stack
3. **Grouped inventory display** — "Arrows (x7)" instead of seven separate entries
4. **Take-all stack awareness** — picking up a stackable item from a room merges with existing stack
5. **Partial take** — "take 3 arrows" from a room stack of 10 leaves 7 behind
6. **Grammar integration** — "an arrow" vs "7 arrows" using existing `GrammarExtensions`

### Modified Files

| File | Change |
|------|--------|
| `Models/Inventory.cs` | Override `Add()` to merge stackable items; add `FindById()` |
| `Interfaces/IInventory.cs` | Add `IItem? FindById(string id)` |
| `Commands/TakeCommand.cs` | Support "take 3 [item]" partial take; merge stacks |
| `Commands/DropCommand.cs` | Support "drop 3 [item]" partial drop; split stacks |
| `Commands/InventoryCommand.cs` | Group stackable items in display |
| `Commands/TakeAllCommand.cs` | Stack-aware bulk take |
| `Commands/DropAllCommand.cs` | Stack-aware bulk drop |
| `Localization/Language.cs` | Amount-aware item display strings |

### New Files

| File | Purpose |
|------|---------|
| `Extensions/StackExtensions.cs` | Fluent helpers for stack operations |

### API Design

```csharp
// Creating stackable items
var arrow = new Item("arrow", "Arrow", "A wooden arrow.")
    .SetStackable()
    .SetAmount(10)
    .SetWeight(0.1f);

var coin = new Item("coin", "Gold Coin")
    .SetStackable()
    .SetAmount(50)
    .SetWeight(0.01f);

// Room setup — place a stack
room.AddItem(arrow);

// Player takes some
// > take 3 arrows
// You take 3 Arrows.
// (room now has 7 arrows, player has 3)

// Player takes more from another room
// > take 5 arrows
// (merges: player now has 8 arrows in one stack)

// Inventory display
// > inventory
// You are carrying:
//   Arrows (x8)
//   Gold Coins (x50)
//   Rusty Sword
```

### Inventory.Add() — Stack Merge Logic

```csharp
public bool Add(IItem item)
{
    if (!CanAdd(item))
        return false;

    if (item.IsStackable)
    {
        var existing = FindById(item.Id);
        if (existing is not null)
        {
            int newAmount = (existing.Amount ?? 1) + (item.Amount ?? 1);
            existing.SetAmount(newAmount);
            return true;
        }
    }

    _items.Add(item);
    return true;
}
```

### Partial Take Logic (TakeCommand)

```
1. Parse input for optional amount: "take 3 arrows" → amount=3, target="arrows"
2. Find item in room
3. If item is stackable and amount < item.Amount:
   a. Create clone with requested amount
   b. Decrease room item amount by requested amount
   c. Add clone to inventory (merges if exists)
4. If amount >= item.Amount or not stackable:
   a. Normal take (remove from room, add to inventory)
5. If item not found → fail
```

### Partial Drop Logic (DropCommand)

```
1. Parse input for optional amount: "drop 3 arrows" → amount=3, target="arrows"
2. Find item in inventory
3. If item is stackable and amount < item.Amount:
   a. Decrease inventory item amount
   b. Create clone with dropped amount
   c. Add to room (merge with existing room stack if present)
4. If amount >= item.Amount:
   a. Normal drop (remove from inventory, add to room)
```

### Inventory Display Grouping

```csharp
// In InventoryCommand.Execute():
foreach (var item in state.Inventory.Items)
{
    string display = item switch
    {
        { IsStackable: true, Amount: > 1 } =>
            $"  {item.Name.Plural(item.Amount.Value)} (x{item.Amount})",
        _ => $"  {Language.EntityName(item)}"
    };
    // ...
}
```

### StackExtensions

```csharp
public static class StackExtensions
{
    /// <summary>Create a stackable item with amount and weight-per-unit.</summary>
    public static Item AsStack(this Item item, int amount, float weightPerUnit = 0f) =>
        (Item)item.SetStackable()
                  .SetAmount(amount)
                  .SetWeight(weightPerUnit);

    /// <summary>Try to merge source into target stack. Returns true if merged.</summary>
    public static bool TryMerge(this IItem target, IItem source)
    {
        if (!target.IsStackable || !source.IsStackable)
            return false;
        if (!string.Equals(target.Id, source.Id, StringComparison.OrdinalIgnoreCase))
            return false;

        int total = (target.Amount ?? 1) + (source.Amount ?? 1);
        target.SetAmount(total);
        return true;
    }

    /// <summary>Split amount from a stack. Returns a new item with the split amount.</summary>
    public static IItem? SplitStack(this IItem item, int amount)
    {
        if (!item.IsStackable || !item.Amount.HasValue)
            return null;
        if (amount <= 0 || amount >= item.Amount.Value)
            return null;

        var clone = item.Clone();
        clone.SetAmount(amount);
        item.SetAmount(item.Amount.Value - amount);
        return clone;
    }
}
```

### Parser Changes

The parser needs to recognise numeric prefixes:
- `take 3 arrows` → `TakeCommand("arrows", amount: 3)`
- `drop 5 coins` → `DropCommand("coins", amount: 5)`
- `take arrows` → `TakeCommand("arrows", amount: null)` (take all)

Add optional `Amount` property to `TakeCommand` and `DropCommand`.

### Weight Calculation

For stackable items, weight should be per-unit:
- `TotalWeight` in inventory = `item.Weight * (item.Amount ?? 1)` for stackable items
- `Inventory.TotalWeight` calculation needs updating

```csharp
public float TotalWeight => _items.Sum(i =>
    i.IsStackable ? i.Weight * (i.Amount ?? 1) : i.Weight);
```

### Tests

- Add stackable item to empty inventory → stored with amount
- Add stackable item to inventory with same Id → amounts merge
- Add stackable item to inventory with different Id → separate entries
- Take partial stack from room → room retains remainder
- Take full stack from room → room item removed
- Drop partial stack → inventory retains remainder, room gets new stack
- Drop full stack → removed from inventory
- Inventory display shows grouped amounts
- Weight calculation correct for stacks
- Non-stackable items unaffected by all changes
- Merge into existing room stack on drop
- TakeAll with stacks

---

## Implementation Checklist (Engine)
- [x] `IInventory.FindById(string id)`
- [x] `Inventory.Add()` stack merge logic
- [x] `Inventory.TotalWeight` stack-aware calculation
- [x] `TakeCommand` partial take support (amount parsing)
- [x] `DropCommand` partial drop support (amount parsing)
- [x] `InventoryCommand` grouped display
- [x] `TakeAllCommand` stack awareness
- [x] `DropAllCommand` stack awareness
- [x] `StackExtensions.cs` (`AsStack`, `TryMerge`, `SplitStack`)
- [x] Language strings for amount-aware messages
- [x] Parser: numeric prefix recognition for take/drop
- [x] Tests (minimum 12)

## Example Checklist (docs/examples)
- [ ] Sandbox demo: `47_Stackable_Items.md`

## Dependencies
- None (builds on existing IItem, Inventory)
- Pairs well with Slice 46 (Consumables) for stacked food/potions
