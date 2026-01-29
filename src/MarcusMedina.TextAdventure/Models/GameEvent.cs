// <copyright file="GameEvent.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace MarcusMedina.TextAdventure.Models;

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;

public sealed class GameEvent
{
    public GameEventType Type { get; }
    public IGameState State { get; }
    public ILocation? Location { get; }
    public IItem? Item { get; }
    public INpc? Npc { get; }
    public IDoor? Door { get; }
    public string? Detail { get; }

    public GameEvent(
        GameEventType type,
        IGameState state,
        ILocation? location = null,
        IItem? item = null,
        INpc? npc = null,
        IDoor? door = null,
        string? detail = null)
    {
        ArgumentNullException.ThrowIfNull(state);
        Type = type;
        State = state;
        Location = location;
        Item = item;
        Npc = npc;
        Door = door;
        Detail = detail;
    }
}
