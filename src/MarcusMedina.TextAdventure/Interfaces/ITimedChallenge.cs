// <copyright file="ITimedChallenge.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace MarcusMedina.TextAdventure.Interfaces;

public interface ITimedChallenge
{
    string Id { get; }
    int MaxMoves { get; }
    int MovesUsed { get; }
    int MovesRemaining { get; }
    bool IsActive { get; }

    ITimedChallenge MaxMovesLimit(int maxMoves);
    ITimedChallenge OnStart(Action<IGameState> handler);
    ITimedChallenge OnMovesRemaining(int movesRemaining, Action<IGameState> handler);
    ITimedChallenge OnSuccess(Action<IGameState> handler);
    ITimedChallenge OnFailure(Action<IGameState> handler);

    void Start(IGameState state);
    void Succeed(IGameState state);
    void Fail(IGameState state);
}
