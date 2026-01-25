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

    public Item Add(string name) => _items.Add(name);
    public Item Add(Item item) => _items.Add(item);
    public ItemList AddMany(params string[] names)
    {
        _items.AddMany(names);
        return this;
    }
    public ItemList AddMany(IEnumerable<string> names)
    {
        _items.AddMany(names);
        return this;
    }

    public Item? Find(string token) => _items.Find(token);
    public Item Get(string token) => _items.Get(token);
    public bool TryGet(string token, out Item item) => _items.TryGet(token, out item);
    public bool Remove(string token) => _items.Remove(token);
    public void Clear() => _items.Clear();

    public Item this[string token] => _items[token];
    public Item Call(string token) => _items.Call(token);
}
