using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public class Inventory : IInventory
{
    private readonly List<IItem> _items = new();

    public InventoryLimitType LimitType { get; private set; }
    public int MaxCount { get; private set; }
    public float MaxWeight { get; private set; }
    public int Count => _items.Count;
    public float TotalWeight => _items.Sum(i => i.Weight);
    public IReadOnlyList<IItem> Items => _items;

    public Inventory(InventoryLimitType limitType = InventoryLimitType.Unlimited, int maxCount = 0, float maxWeight = 0f)
    {
        LimitType = limitType;
        MaxCount = maxCount;
        MaxWeight = maxWeight;
    }

    public bool CanAdd(IItem item)
    {
        return LimitType switch
        {
            InventoryLimitType.Unlimited => true,
            InventoryLimitType.ByCount => Count + 1 <= MaxCount,
            InventoryLimitType.ByWeight => TotalWeight + item.Weight <= MaxWeight,
            _ => true
        };
    }

    public bool Add(IItem item)
    {
        if (!CanAdd(item)) return false;
        _items.Add(item);
        return true;
    }

    public bool Remove(IItem item)
    {
        return _items.Remove(item);
    }

    public IItem? FindItem(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return null;
        return _items.FirstOrDefault(i => i.Matches(name));
    }
}
