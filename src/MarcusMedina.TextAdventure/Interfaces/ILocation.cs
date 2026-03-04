// <copyright file="ILocation.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Interfaces;

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Models;

public interface ILocation : IPropertyBag
{
    IReadOnlyDictionary<Direction, Exit> Exits { get; }
    string Id { get; }
    string Name { get; }
    IReadOnlyList<IItem> Items { get; }

    IReadOnlyList<INpc> Npcs { get; }

    void AddItem(IItem item);

    void AddNpc(INpc npc);

    IItem? FindItem(string name);

    INpc? FindNpc(string name);

    string GetDescription();

    Exit? GetExit(Direction direction);

    bool RemoveItem(IItem item);

    bool RemoveNpc(INpc npc);
}