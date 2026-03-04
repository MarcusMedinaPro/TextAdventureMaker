// <copyright file="GameEvent.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Models;

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;

public sealed class GameEvent(
    GameEventType type,
    IGameState state,
    ILocation? location = null,
    IItem? item = null,
    INpc? npc = null,
    IDoor? door = null,
    string? detail = null)
{
    public GameEventType Type { get; } = type;
    public IGameState State { get; } = state ?? throw new ArgumentNullException(nameof(state));
    public ILocation? Location { get; } = location;
    public IItem? Item { get; } = item;
    public INpc? Npc { get; } = npc;
    public IDoor? Door { get; } = door;
    public string? Detail { get; } = detail;
}