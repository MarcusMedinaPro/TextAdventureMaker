// <copyright file="CharacterArc.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public sealed class CharacterArc : ICharacterArc
{
    private readonly List<(int Index, string Id, Trait Unlocks)> _milestones = [];
    private Action<IGameState>? _onComplete;
    private int _currentIndex;

    public string Id { get; }
    public Trait StartTrait { get; private set; }
    public Trait EndTrait { get; private set; }
    public Trait CurrentTrait { get; private set; }

    public CharacterArc(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        Id = id;
    }

    public ICharacterArc StartState(Trait trait)
    {
        StartTrait = trait;
        CurrentTrait = trait;
        return this;
    }

    public ICharacterArc EndState(Trait trait)
    {
        EndTrait = trait;
        return this;
    }

    public ICharacterArc Milestone(int index, string id, Trait unlocks)
    {
        _milestones.Add((index, id, unlocks));
        return this;
    }

    public ICharacterArc OnComplete(Action<IGameState> action)
    {
        _onComplete = action;
        return this;
    }

    public bool Advance(string milestoneId, IGameState state)
    {
        if (_currentIndex >= _milestones.Count)
        {
            return false;
        }

        (int index, string id, Trait unlocks) = _milestones[_currentIndex];
        if (!id.Equals(milestoneId, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        CurrentTrait = unlocks;
        _currentIndex++;

        if (_currentIndex >= _milestones.Count)
        {
            CurrentTrait = EndTrait;
            _onComplete?.Invoke(state);
        }

        return true;
    }
}
