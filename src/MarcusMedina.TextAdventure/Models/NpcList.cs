// <copyright file="NpcList.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace MarcusMedina.TextAdventure.Models;

using MarcusMedina.TextAdventure.Extensions;

public sealed class NpcList
{
    private readonly GameList<Npc> _npcs = new(name => new Npc(name.ToId(), name));

    public IReadOnlyCollection<Npc> Items => _npcs.Items;

    public Npc Add(string name) => _npcs.Add(name);
    public Npc Add(Npc npc) => _npcs.Add(npc);
    public NpcList AddMany(params string[] names)
    {
        _ = _npcs.AddMany(names);
        return this;
    }
    public NpcList AddMany(IEnumerable<string> names)
    {
        _ = _npcs.AddMany(names);
        return this;
    }

    public Npc? Find(string token) => _npcs.Find(token);
    public Npc Get(string token) => _npcs.Get(token);
    public bool TryGet(string token, out Npc npc) => _npcs.TryGet(token, out npc);
    public bool Remove(string token) => _npcs.Remove(token);
    public void Clear() => _npcs.Clear();

    public Npc this[string token] => _npcs[token];
    public Npc Call(string token) => _npcs.Call(token);
}
