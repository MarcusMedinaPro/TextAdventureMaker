// <copyright file="ITimedSpawn.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;

namespace MarcusMedina.TextAdventure.Interfaces;

public interface ITimedSpawn
{
    string ItemId { get; }
    string? MessageText { get; }
    ITimedSpawn AppearsAt(int tick);
    ITimedSpawn AppearsAt(TimePhase phase);
    ITimedSpawn AppearsWhen(Func<IGameState, bool> predicate);
    ITimedSpawn DisappearsAfter(int ticks);
    ITimedSpawn DisappearsAt(TimePhase phase);
    ITimedSpawn Message(string text);
    ITimedSpawn Or();
}
