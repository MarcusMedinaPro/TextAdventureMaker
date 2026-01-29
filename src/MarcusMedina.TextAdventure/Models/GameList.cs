// <copyright file="GameList.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace MarcusMedina.TextAdventure.Models;

using System.Linq;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Interfaces;

public sealed class GameList<T>(Func<string, T> factoryFromName) where T : IGameEntity
{
    private readonly Dictionary<string, T> _items = new(StringComparer.OrdinalIgnoreCase);
    private readonly Func<string, T> _factoryFromName = factoryFromName ?? throw new ArgumentNullException(nameof(factoryFromName));

    public IReadOnlyCollection<T> Items => _items.Values;

    public T Add(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return Add(_factoryFromName(name));
    }

    public T Add(T item)
    {
        ArgumentNullException.ThrowIfNull(item);
        _items[item.Id] = item;
        return item;
    }

    public GameList<T> AddMany(params string[] names)
    {
        if (names == null)
            return this;
        foreach (var name in names)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                _ = Add(name);
            }
        }

        return this;
    }

    public GameList<T> AddMany(IEnumerable<string> names)
    {
        if (names == null)
            return this;
        foreach (var name in names)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                _ = Add(name);
            }
        }

        return this;
    }

    public GameList<T> AddMany(IEnumerable<T> items)
    {
        if (items == null)
            return this;
        foreach (var item in items)
        {
            if (item != null)
            {
                _ = Add(item);
            }
        }

        return this;
    }

    public T? Find(string token) => string.IsNullOrWhiteSpace(token)
            ? default
            : _items.TryGetValue(token, out var item) ? item : _items.Values.FirstOrDefault(i => Matches(i, token));

    public T Get(string token)
    {
        var item = Find(token);
        return item == null ? throw new KeyNotFoundException($"No item found for '{token}'.") : item;
    }

    public bool TryGet(string token, out T item)
    {
        item = Find(token) ?? default!;
        return item != null;
    }

    public bool Remove(string token)
    {
        var item = Find(token);
        return item != null && _items.Remove(item.Id);
    }

    public void Clear() => _items.Clear();

    public T this[string token] => Get(token);

    public T Call(string token) => Get(token);

    private static bool Matches(T item, string token) => item is IItem itemWithAliases
            ? itemWithAliases.Matches(token)
            : item is IDoor doorWithAliases ? doorWithAliases.Matches(token) : item.Id.TextCompare(token) || item.Name.TextCompare(token);
}
