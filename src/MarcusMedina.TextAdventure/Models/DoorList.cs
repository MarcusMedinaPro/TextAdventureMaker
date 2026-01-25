// <copyright file="DoorList.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
using MarcusMedina.TextAdventure.Extensions;

namespace MarcusMedina.TextAdventure.Models;

public sealed class DoorList
{
    private readonly GameList<Door> _doors = new(name => new Door(name.ToId(), name));

    public IReadOnlyCollection<Door> Items => _doors.Items;

    public Door Add(string name) => _doors.Add(name);
    public Door Add(Door door) => _doors.Add(door);
    public DoorList AddMany(params string[] names)
    {
        _doors.AddMany(names);
        return this;
    }
    public DoorList AddMany(IEnumerable<string> names)
    {
        _doors.AddMany(names);
        return this;
    }

    public Door? Find(string token) => _doors.Find(token);
    public Door Get(string token) => _doors.Get(token);
    public bool TryGet(string token, out Door door) => _doors.TryGet(token, out door);
    public bool Remove(string token) => _doors.Remove(token);
    public void Clear() => _doors.Clear();

    public Door this[string token] => _doors[token];
    public Door Call(string token) => _doors.Call(token);
}
