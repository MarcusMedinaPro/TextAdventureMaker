// <copyright file="Inventory.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public class Inventory(InventoryLimitType limitType = InventoryLimitType.Unlimited, int maxCount = 0, float maxWeight = 0f) : IInventory
{
    private readonly List<IItem> _items = [];

    public InventoryLimitType LimitType { get; private set; } = limitType;
    public int MaxCount { get; private set; } = maxCount;
    public float MaxWeight { get; private set; } = maxWeight;
    public int Count => _items.Count;
    public float TotalWeight => _items.Sum(i =>
        i.IsStackable ? i.Weight * (i.Amount ?? 1) : i.Weight);
    public IReadOnlyList<IItem> Items => _items;

    public bool CanAdd(IItem item) =>
        LimitType switch
        {
            InventoryLimitType.Unlimited => true,
            InventoryLimitType.ByCount => item.IsStackable && FindById(item.Id) is not null || Count + 1 <= MaxCount,
            InventoryLimitType.ByWeight => TotalWeight + item.Weight * (item.IsStackable ? (item.Amount ?? 1) : 1) <= MaxWeight,
            _ => true
        };

    public bool Add(IItem item)
    {
        if (!CanAdd(item))
            return false;

        if (item.IsStackable)
        {
            IItem? existing = FindById(item.Id);
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

    public bool Remove(IItem item) => _items.Remove(item);

    public IItem? FindItem(string name) =>
        string.IsNullOrWhiteSpace(name) ? null : _items.FirstOrDefault(i => i.Matches(name));

    public IItem? FindById(string id) =>
        string.IsNullOrWhiteSpace(id) ? null : _items.FirstOrDefault(i =>
            string.Equals(i.Id, id, StringComparison.OrdinalIgnoreCase));

    public void Clear() => _items.Clear();
}
