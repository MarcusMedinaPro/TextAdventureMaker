## Slice 4: Items + Inventory

**Mål:** Items i rum, plocka upp, släpp, visa inventory. Containers och kombinationer.

**Notis:** Lägg till stöd för synonymer/alias på IItem (t.ex. `Aliases: string[]`) och använd i parser/kommandon.
**Notis:** När nya item-subklasser (t.ex. Weapon/Potion) införs, ge dem fluent overrides för SetWeight/SetTakeable/AddAliases så chaining behåller typen.

### Task 4.1: IItem + Item (Factory + Prototype) ✅

- `Takeable: bool`
- `Weight: float` (optional)
- Events: OnTake, OnDrop, OnUse, OnDestroy

### Task 4.2: IInventory + Inventory ✅

- Configurable limits: ByWeight, ByCount, Unlimited
- `TakeAll()` method

### Task 4.3: Item Decorators (RustyModifier, EnchantedModifier) ✅

### Task 4.4: IContainer<T> — items that hold other items ✅

- `Glass : IContainer<IFluid>`
- `Chest : IContainer<IItem>`

### Task 4.5: Item Combinations ✅

- `ice + fire → destroy both, create water`
- Recipe system for crafting

### Task 4.6: TakeCommand, TakeAllCommand, DropCommand, InventoryCommand, UseCommand ✅

### Task 4.7: Readable Items med villkor ✅

```csharp
// Skylt - läs utan att ta
var sign = new Item("sign", "Wooden Sign")
    .SetTakeable(false)
    .SetReadable(true)
    .SetReadText("Welcome to the Dark Forest!");

// Tidning - måste ta för att läsa
var newspaper = new Item("newspaper", "Daily News")
    .SetTakeable(true)
    .SetReadable(true)
    .RequireTakeToRead()
    .SetReadText("HEADLINE: Dragon spotted near village!");

// Bok - kan läsa men tar tid (förbrukar drag)
var tome = new Item("tome", "Ancient Tome")
    .SetReadable(true)
    .RequireTakeToRead()
    .SetReadingCost(3)  // tar 3 drag att läsa
    .SetReadText("The secret to defeating the dragon is...");

// Hemligt brev - kräver ljus
var letter = new Item("letter", "Sealed Letter")
    .SetReadable(true)
    .RequireTakeToRead()
    .RequiresToRead(ctx => ctx.HasLight())
    .SetReadText("Meet me at midnight...");
```

**ReadCommand:**

```csharp
// "read sign" → visar text direkt
// "read newspaper" → "You need to pick it up first."
// "read newspaper" (i inventory) → visar text
// "read tome" → "You spend 3 turns reading... [text]"
// "read letter" (mörkt) → "It's too dark to read."
```

### Task 4.8: Sandbox — plocka upp svärd, häll vatten i glas, kombinera items, läs skylt/tidning ✅

---

## Implementation checklist (engine)
- [x] `IItem` + `Item` (prototype + factory helpers)
- [x] Item reactions: `OnTake`, `OnDrop`, `OnUse`, `OnDestroy`
- [x] Item aliases/synonyms support
- [x] `IInventory` + `Inventory` (limits by weight/count/unlimited)
- [x] `TakeAll()` on inventory
- [x] Item decorators (`RustyModifier`, `EnchantedModifier`)
- [x] `IContainer<T>` + `Glass` + `Chest`
- [x] Item combinations with `RecipeBook` + `ItemCombinationRecipe`
- [x] Commands: `TakeCommand`, `TakeAllCommand`, `DropCommand`, `InventoryCommand`, `UseCommand`
- [x] Readable items + conditions + `ReadCommand`

## Example checklist (docs/examples)
- [x] Pick up items + inventory view (`04_The_Last_Train_Home.md`)
- [x] Use item reactions (`04_The_Last_Train_Home.md`)
- [x] Containers (glass) (`04-10_Forest_Adventure_Core.md`)
- [x] Item combinations/recipes (`04-10_Forest_Adventure_Core.md`)
- [x] Readable items + read costs/requirements (`04-10_Forest_Adventure_Core.md`)
- [ ] `TakeAll` demonstrated explicitly
