// <copyright file="ILocation.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Interfaces;

public interface ILocation
{
    string Id { get; }
    string GetDescription();
    Exit? GetExit(Direction direction);
    IReadOnlyDictionary<Direction, Exit> Exits { get; }
    IReadOnlyList<IItem> Items { get; }
    IReadOnlyList<INpc> Npcs { get; }
    void AddItem(IItem item);
    bool RemoveItem(IItem item);
    IItem? FindItem(string name);
    void AddNpc(INpc npc);
    bool RemoveNpc(INpc npc);
    INpc? FindNpc(string name);
}
