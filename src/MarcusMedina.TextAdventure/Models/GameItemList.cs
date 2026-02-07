// <copyright file="GameItemList.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Extensions;

namespace MarcusMedina.TextAdventure.Models;

public sealed class GameItemList
{
    private readonly Dictionary<string, Item> _items = new(StringComparer.OrdinalIgnoreCase);

    public IReadOnlyCollection<Item> Items => _items.Values;

    public Item Add(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return Add(new Item(name.ToId(), name));
    }

    public Item Add(string id, string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return Add(new Item(id, name));
    }

    public Item Add(string id, string name, string description)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return Add(new Item(id, name, description));
    }

    public Item Add(Item item)
    {
        ArgumentNullException.ThrowIfNull(item);
        _items[item.Id] = item;
        return item;
    }

    public GameItemList AddMany(params string[] names)
    {
        if (names == null)
        {
            return this;
        }

        foreach (string name in names)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                _ = Add(name);
            }
        }

        return this;
    }

    public GameItemList AddMany(IEnumerable<string> names)
    {
        if (names == null)
        {
            return this;
        }

        foreach (string name in names)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                _ = Add(name);
            }
        }

        return this;
    }

    public Item? Find(string token)
    {
        return string.IsNullOrWhiteSpace(token)
            ? null
            : _items.TryGetValue(token, out Item? item) ? item : _items.Values.FirstOrDefault(i => i.Matches(token));
    }

    public Item Get(string token)
    {
        Item? item = Find(token);
        return item ?? throw new KeyNotFoundException($"No item found for '{token}'.");
    }

    public bool TryGet(string token, out Item item)
    {
        item = Find(token) ?? null!;
        return item != null;
    }

    public bool Remove(string token)
    {
        Item? item = Find(token);
        return item != null && _items.Remove(item.Id);
    }

    public void Clear()
    {
        _items.Clear();
    }

    public Item this[string token] => Get(token);

    public Item Call(string token)
    {
        return Get(token);
    }
}
