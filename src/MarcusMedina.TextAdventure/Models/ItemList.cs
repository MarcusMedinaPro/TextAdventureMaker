// <copyright file="ItemList.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Extensions;

namespace MarcusMedina.TextAdventure.Models;

public sealed class ItemList
{
    private readonly GameList<Item> _items = new(name => new Item(name.ToId(), name));

    public IReadOnlyCollection<Item> Items => _items.Items;

    public Item Add(string name)
    {
        return _items.Add(name);
    }

    public Item Add(Item item)
    {
        return _items.Add(item);
    }

    public ItemList AddMany(params string[] names)
    {
        _ = _items.AddMany(names);
        return this;
    }
    public ItemList AddMany(IEnumerable<string> names)
    {
        _ = _items.AddMany(names);
        return this;
    }

    public Item? Find(string token)
    {
        return _items.Find(token);
    }

    public Item Get(string token)
    {
        return _items.Get(token);
    }

    public bool TryGet(string token, out Item item)
    {
        return _items.TryGet(token, out item);
    }

    public bool Remove(string token)
    {
        return _items.Remove(token);
    }

    public void Clear()
    {
        _items.Clear();
    }

    public Item this[string token] => _items[token];
    public Item Call(string token)
    {
        return _items.Call(token);
    }
}
