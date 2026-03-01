// <copyright file="EventChain.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public sealed class EventChain : IEventChain
{
    private readonly List<(Func<IGameState, bool> Condition, Action<IGameState> Action)> _steps = [];
    private int _currentIndex;

    public IEventChain Step(Func<IGameState, bool> condition, Action<IGameState> action)
    {
        ArgumentNullException.ThrowIfNull(condition);
        ArgumentNullException.ThrowIfNull(action);
        _steps.Add((condition, action));
        return this;
    }

    public bool Check(IGameState state)
    {
        if (_currentIndex >= _steps.Count)
        {
            return false;
        }

        (Func<IGameState, bool> condition, Action<IGameState> action) = _steps[_currentIndex];
        if (!condition(state))
        {
            return false;
        }

        action(state);
        _currentIndex++;
        return true;
    }
}
