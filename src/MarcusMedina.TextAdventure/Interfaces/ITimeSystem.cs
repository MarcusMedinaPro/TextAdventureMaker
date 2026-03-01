// <copyright file="ITimeSystem.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Interfaces;

using MarcusMedina.TextAdventure.Enums;

public interface ITimeSystem
{
    int CurrentDay { get; }
    TimePhase CurrentPhase { get; }
    int CurrentTick { get; }
    TimeOfDay CurrentTimeOfDay { get; }
    bool Enabled { get; }
    int? MaxMoves { get; }
    int? MovesRemaining { get; }
    int MovesUsed { get; }
    int TicksPerDay { get; }

    ITimedChallenge CreateTimedChallenge(string id);

    ITimeSystem Enable();

    ITimeSystem OnMovesExhausted(Action<IGameState> handler);

    ITimeSystem OnMovesRemaining(int movesRemaining, Action<IGameState> handler);

    ITimeSystem OnPhase(TimeOfDay phase, Action<IGameState> handler);

    ITimeSystem SetMaxMoves(int maxMoves);

    ITimeSystem SetStartTime(TimeOfDay startTime);

    ITimeSystem SetTicksPerDay(int ticksPerDay);

    void Tick(IGameState state);
}