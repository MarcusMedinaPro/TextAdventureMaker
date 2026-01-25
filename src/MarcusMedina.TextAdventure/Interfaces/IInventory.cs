using MarcusMedina.TextAdventure.Enums;

namespace MarcusMedina.TextAdventure.Interfaces;

public interface IInventory
{
    InventoryLimitType LimitType { get; }
    int MaxCount { get; }
    float MaxWeight { get; }
    int Count { get; }
    float TotalWeight { get; }
    IReadOnlyList<IItem> Items { get; }

    bool CanAdd(IItem item);
    bool Add(IItem item);
    bool Remove(IItem item);
    IItem? FindItem(string name);
}
