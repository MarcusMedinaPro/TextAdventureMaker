// <copyright file="IInventory.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace MarcusMedina.TextAdventure.Interfaces;

using MarcusMedina.TextAdventure.Enums;

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
    void Clear();
}
