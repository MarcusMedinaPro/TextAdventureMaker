// <copyright file="ContainerItem.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace MarcusMedina.TextAdventure.Models;

using MarcusMedina.TextAdventure.Interfaces;

public class ContainerItem<T>(string id, string name, int maxCount = 0) : Item(id, name), IContainer<T>
{
    private readonly List<T> _contents = [];

    public int MaxCount { get; private set; } = maxCount;
    public IReadOnlyList<T> Contents => _contents;

    public bool CanAdd(T item) => MaxCount <= 0 || _contents.Count + 1 <= MaxCount;

    public bool Add(T item)
    {
        if (!CanAdd(item))
            return false;
        _contents.Add(item);
        return true;
    }

    public bool Remove(T item) => _contents.Remove(item);
}
