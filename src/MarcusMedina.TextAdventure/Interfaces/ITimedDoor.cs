// <copyright file="ITimedDoor.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;

namespace MarcusMedina.TextAdventure.Interfaces;

public interface ITimedDoor
{
    string DoorId { get; }
    string? MessageText { get; }
    string? ClosedMessageText { get; }
    ITimedDoor OpensAt(int tick);
    ITimedDoor ClosesAt(int tick);
    ITimedDoor OpensAt(TimePhase phase);
    ITimedDoor ClosesAt(TimePhase phase);
    ITimedDoor OpensWhen(Func<IGameState, bool> predicate);
    ITimedDoor ClosesWhen(Func<IGameState, bool> predicate);
    ITimedDoor PermanentlyOpensWhen(Func<IGameState, bool> predicate);
    ITimedDoor Message(string text);
    ITimedDoor ClosedMessage(string text);
    ITimedDoor Or();
}
