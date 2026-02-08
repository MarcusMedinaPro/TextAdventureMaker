// <copyright file="IStoryBranch.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Interfaces;

public interface IStoryBranch
{
    string Id { get; }
    bool IsCompleted { get; }
    IStoryBranch Condition(Func<IGameState, bool> predicate);
    IStoryBranch Consequence(Action<IGameState> consequence);
    bool Evaluate(IGameState state);
}
