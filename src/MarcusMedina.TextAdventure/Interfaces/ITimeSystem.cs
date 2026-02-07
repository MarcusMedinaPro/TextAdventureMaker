// <copyright file="ITimeSystem.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;

namespace MarcusMedina.TextAdventure.Interfaces;

public interface ITimeSystem
{
    bool Enabled { get; }
    int CurrentTick { get; }
    int CurrentDay { get; }
    int TicksPerDay { get; }
    TimeOfDay CurrentTimeOfDay { get; }
    TimePhase CurrentPhase { get; }
    int? MaxMoves { get; }
    int MovesUsed { get; }
    int? MovesRemaining { get; }

    ITimeSystem Enable();
    ITimeSystem SetStartTime(TimeOfDay startTime);
    ITimeSystem SetTicksPerDay(int ticksPerDay);
    ITimeSystem SetMaxMoves(int maxMoves);

    ITimeSystem OnPhase(TimeOfDay phase, Action<IGameState> handler);
    ITimeSystem OnMovesRemaining(int movesRemaining, Action<IGameState> handler);
    ITimeSystem OnMovesExhausted(Action<IGameState> handler);

    ITimedChallenge CreateTimedChallenge(string id);
    void Tick(IGameState state);
}
