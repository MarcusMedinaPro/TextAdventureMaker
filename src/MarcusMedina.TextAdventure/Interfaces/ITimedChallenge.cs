// <copyright file="ITimedChallenge.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace MarcusMedina.TextAdventure.Interfaces;

public interface ITimedChallenge
{
    string Id { get; }
    bool IsActive { get; }
    int MaxMoves { get; }
    int MovesRemaining { get; }
    int MovesUsed { get; }

    void Fail(IGameState state);

    ITimedChallenge MaxMovesLimit(int maxMoves);

    ITimedChallenge OnFailure(Action<IGameState> handler);

    ITimedChallenge OnMovesRemaining(int movesRemaining, Action<IGameState> handler);

    ITimedChallenge OnStart(Action<IGameState> handler);

    ITimedChallenge OnSuccess(Action<IGameState> handler);

    void Start(IGameState state);

    void Succeed(IGameState state);
}