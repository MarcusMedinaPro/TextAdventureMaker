// <copyright file="IInventory.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Interfaces;

using MarcusMedina.TextAdventure.Enums;

public interface IInventory
{
    int Count { get; }
    IReadOnlyList<IItem> Items { get; }
    InventoryLimitType LimitType { get; }
    int MaxCount { get; }
    float MaxWeight { get; }
    float TotalWeight { get; }

    bool Add(IItem item);

    bool CanAdd(IItem item);

    void Clear();

    IItem? FindItem(string name);

    bool Remove(IItem item);
}