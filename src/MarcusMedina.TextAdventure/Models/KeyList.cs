// <copyright file="KeyList.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace MarcusMedina.TextAdventure.Models;

using MarcusMedina.TextAdventure.Extensions;

public sealed class KeyList
{
    private readonly GameList<Key> _keys = new(name => new Key(name.ToId(), name));

    public IReadOnlyCollection<Key> Items => _keys.Items;

    public Key Add(string name) => _keys.Add(name);
    public Key Add(Key key) => _keys.Add(key);
    public KeyList AddMany(params string[] names)
    {
        _ = _keys.AddMany(names);
        return this;
    }
    public KeyList AddMany(IEnumerable<string> names)
    {
        _ = _keys.AddMany(names);
        return this;
    }

    public Key? Find(string token) => _keys.Find(token);
    public Key Get(string token) => _keys.Get(token);
    public bool TryGet(string token, out Key key) => _keys.TryGet(token, out key);
    public bool Remove(string token) => _keys.Remove(token);
    public void Clear() => _keys.Clear();

    public Key this[string token] => _keys[token];
    public Key Call(string token) => _keys.Call(token);
}
