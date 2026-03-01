// <copyright file="Inventory.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Models;

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;

public class Inventory(InventoryLimitType limitType = InventoryLimitType.Unlimited, int maxCount = 0, float maxWeight = 0f) : IInventory
{
    private readonly List<IItem> _items = [];

    public int Count => _items.Count;
    public IReadOnlyList<IItem> Items => _items;
    public InventoryLimitType LimitType { get; private set; } = limitType;
    public int MaxCount { get; private set; } = maxCount;
    public float MaxWeight { get; private set; } = maxWeight;
    public float TotalWeight => _items.Sum(i => i.Weight);

    public bool Add(IItem item)
    {
        if (!CanAdd(item))
        {
            return false;
        }

        _items.Add(item);
        return true;
    }

    public bool CanAdd(IItem item) => LimitType switch
    {
        InventoryLimitType.Unlimited => true,
        InventoryLimitType.ByCount => Count + 1 <= MaxCount,
        InventoryLimitType.ByWeight => TotalWeight + item.Weight <= MaxWeight,
        _ => true
    };

    public void Clear() => _items.Clear();

    public IItem? FindItem(string name) => string.IsNullOrWhiteSpace(name) ? null : _items.FirstOrDefault(i => i.Matches(name));

    public bool Remove(IItem item) => _items.Remove(item);
}
