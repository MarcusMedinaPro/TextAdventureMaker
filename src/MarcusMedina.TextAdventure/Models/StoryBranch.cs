// <copyright file="StoryBranch.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public sealed class StoryBranch : IStoryBranch
{
    private readonly List<Func<IGameState, bool>> _conditions = [];
    private readonly List<Action<IGameState>> _consequences = [];

    public string Id { get; }
    public bool IsCompleted { get; private set; }

    public StoryBranch(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        Id = id;
    }

    public IStoryBranch Condition(Func<IGameState, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        _conditions.Add(predicate);
        return this;
    }

    public IStoryBranch Consequence(Action<IGameState> consequence)
    {
        ArgumentNullException.ThrowIfNull(consequence);
        _consequences.Add(consequence);
        return this;
    }

    public bool Evaluate(IGameState state)
    {
        if (IsCompleted)
        {
            return false;
        }

        if (_conditions.Count == 0 || _conditions.All(predicate => predicate(state)))
        {
            foreach (Action<IGameState> consequence in _consequences)
            {
                consequence(state);
            }

            IsCompleted = true;
            return true;
        }

        return false;
    }
}
