## Slice 55: Economic & Store System

**Mål:** Komplett ekonomisystem med valutor, butiker, handel och dynamiska priser.

**Referens:** `docs/plans/imported/Economic_Store_System.md`

### Task 55.1: Currency System

```csharp
public interface ICurrency
{
    string Id { get; }
    string Name { get; }
    string Symbol { get; }
    int SubunitRatio { get; }  // 100 för kronor→ören
}

public class Wallet
{
    private readonly Dictionary<string, int> _balances = [];

    public int GetBalance(string currencyId) =>
        _balances.TryGetValue(currencyId, out var bal) ? bal : 0;

    public bool CanAfford(string currencyId, int amount) =>
        GetBalance(currencyId) >= amount;

    public bool TrySpend(string currencyId, int amount)
    {
        if (!CanAfford(currencyId, amount)) return false;
        _balances[currencyId] -= amount;
        return true;
    }

    public void Add(string currencyId, int amount) =>
        _balances[currencyId] = GetBalance(currencyId) + amount;

    public string Format(string currencyId, ICurrency currency) =>
        $"{currency.Symbol}{GetBalance(currencyId)}";
}
```

### Task 55.2: IStore interface

```csharp
public interface IStore
{
    string Id { get; }
    string Name { get; }
    string Description { get; }
    IReadOnlyList<StoreItem> Inventory { get; }
    string CurrencyId { get; }

    bool IsOpen { get; }
    float PriceModifier { get; }  // 1.0 = normalt, 1.5 = 50% dyrare

    BuyResult TryBuy(IGameState state, string itemId, int quantity = 1);
    SellResult TrySell(IGameState state, IItem item, int quantity = 1);
}

public record StoreItem(
    string ItemId,
    string Name,
    int BasePrice,
    int Stock,           // -1 = unlimited
    int MaxPerCustomer   // -1 = unlimited
);

public record BuyResult(bool Success, string Message, IItem? Item = null);
public record SellResult(bool Success, string Message, int? Amount = null);
```

### Task 55.3: Store implementation

```csharp
public class Store : IStore
{
    private readonly List<StoreItem> _inventory = [];
    private readonly Dictionary<string, int> _purchaseCounts = [];

    public string Id { get; init; } = "";
    public string Name { get; init; } = "";
    public string Description { get; init; } = "";
    public IReadOnlyList<StoreItem> Inventory => _inventory.AsReadOnly();
    public string CurrencyId { get; init; } = "gold";
    public bool IsOpen { get; set; } = true;
    public float PriceModifier { get; set; } = 1.0f;

    public void AddItem(StoreItem item) => _inventory.Add(item);

    public BuyResult TryBuy(IGameState state, string itemId, int quantity = 1)
    {
        if (!IsOpen)
            return new BuyResult(false, "The store is closed.");

        var storeItem = _inventory.FirstOrDefault(i => i.ItemId == itemId);
        if (storeItem == null)
            return new BuyResult(false, "Item not found.");

        if (storeItem.Stock != -1 && storeItem.Stock < quantity)
            return new BuyResult(false, "Not enough in stock.");

        var purchased = _purchaseCounts.GetValueOrDefault(itemId, 0);
        if (storeItem.MaxPerCustomer != -1 && purchased + quantity > storeItem.MaxPerCustomer)
            return new BuyResult(false, $"You can only buy {storeItem.MaxPerCustomer} of these.");

        var totalPrice = (int)(storeItem.BasePrice * quantity * PriceModifier);

        if (!state.Wallet.TrySpend(CurrencyId, totalPrice))
            return new BuyResult(false, $"You need {totalPrice} gold.");

        // Uppdatera stock och purchase count
        if (storeItem.Stock != -1)
        {
            var idx = _inventory.IndexOf(storeItem);
            _inventory[idx] = storeItem with { Stock = storeItem.Stock - quantity };
        }
        _purchaseCounts[itemId] = purchased + quantity;

        var item = CreateItem(storeItem);
        state.Inventory.Add(item);

        return new BuyResult(true, $"You bought {item.Name} for {totalPrice} gold.", item);
    }

    public SellResult TrySell(IGameState state, IItem item, int quantity = 1)
    {
        if (!IsOpen)
            return new SellResult(false, "The store is closed.");

        var sellPrice = (int)(item.GetProperty<int>("value", 1) * 0.5f);  // 50% av värde

        state.Inventory.Remove(item);
        state.Wallet.Add(CurrencyId, sellPrice);

        return new SellResult(true, $"You sold {item.Name} for {sellPrice} gold.", sellPrice);
    }
}
```

### Task 55.4: Dynamic Pricing

```csharp
public class DynamicPricingSystem
{
    public float CalculateModifier(IStore store, IGameState state)
    {
        var modifier = 1.0f;

        // Reputation påverkar pris
        var reputation = state.GetFactionReputation(store.Id);
        modifier *= reputation switch
        {
            >= 50 => 0.9f,   // 10% rabatt vid hög reputation
            >= 0 => 1.0f,    // Normalpris
            < 0 => 1.2f,     // 20% dyrare vid låg reputation
            _ => 1.0f
        };

        // Supply/demand
        // Ont om varor = dyrare
        var totalStock = store.Inventory.Sum(i => i.Stock == -1 ? 100 : i.Stock);
        if (totalStock < 10)
            modifier *= 1.3f;

        // Tid på dygnet
        if (state.TimeSystem.TimeOfDay == TimeOfDay.Night)
            modifier *= 1.1f;

        return modifier;
    }
}
```

### Task 55.5: BuyCommand & SellCommand

```csharp
public class BuyCommand(string itemName) : ICommand
{
    public CommandResult Execute(CommandContext context)
    {
        var store = context.State.CurrentLocation.GetProperty<IStore>("store");
        if (store == null)
            return CommandResult.Fail("There's no store here.");

        var result = store.TryBuy(context.State, itemName);
        return result.Success
            ? CommandResult.Ok(result.Message)
            : CommandResult.Fail(result.Message);
    }
}

public class SellCommand(string itemName) : ICommand
{
    public CommandResult Execute(CommandContext context)
    {
        var store = context.State.CurrentLocation.GetProperty<IStore>("store");
        if (store == null)
            return CommandResult.Fail("There's no store here.");

        var item = context.State.Inventory.FindItem(itemName);
        if (item == null)
            return CommandResult.Fail($"You don't have a {itemName}.");

        var result = store.TrySell(context.State, item);
        return result.Success
            ? CommandResult.Ok(result.Message)
            : CommandResult.Fail(result.Message);
    }
}
```

### Task 55.6: ShopCommand - visa butikens inventory

```csharp
public class ShopCommand : ICommand
{
    public CommandResult Execute(CommandContext context)
    {
        var store = context.State.CurrentLocation.GetProperty<IStore>("store");
        if (store == null)
            return CommandResult.Fail("There's no store here.");

        if (!store.IsOpen)
            return CommandResult.Ok($"{store.Name} is closed.");

        var sb = new StringBuilder();
        sb.AppendLine($"=== {store.Name} ===");
        sb.AppendLine(store.Description);
        sb.AppendLine();
        sb.AppendLine("For sale:");

        foreach (var item in store.Inventory.Where(i => i.Stock != 0))
        {
            var price = (int)(item.BasePrice * store.PriceModifier);
            var stock = item.Stock == -1 ? "∞" : item.Stock.ToString();
            sb.AppendLine($"  {item.Name} - {price} gold (stock: {stock})");
        }

        sb.AppendLine();
        sb.AppendLine($"Your gold: {context.State.Wallet.GetBalance(store.CurrencyId)}");

        return CommandResult.Ok(sb.ToString());
    }
}
```

### Task 55.7: Tester

```csharp
[Fact]
public void Store_BuyItem_DeductsGoldAndAddsItem()
{
    var game = CreateGameWithStore();
    game.State.Wallet.Add("gold", 100);

    var result = game.Execute("buy sword");

    Assert.True(result.Success);
    Assert.Contains(game.State.Inventory.Items, i => i.Name == "sword");
    Assert.True(game.State.Wallet.GetBalance("gold") < 100);
}

[Fact]
public void Store_BuyItem_FailsWithoutEnoughGold()
{
    var game = CreateGameWithStore();
    game.State.Wallet.Add("gold", 5);

    var result = game.Execute("buy sword");  // costs 50

    Assert.False(result.Success);
    Assert.Contains("need", result.Message.ToLower());
}
```

### Task 55.8: Sandbox — bymarknaden

Demo med flera butiker, olika valutor och dynamisk prissättning baserat på reputation.

---

## Completion Checklist
- [x] Core implementation for this slice is present in the engine.
- [x] Behaviour is covered by tests and/or deterministic validation paths.
- [x] Demo document: `55_The_Empty_Till.md`.
- [x] Marked complete in project slice status.
