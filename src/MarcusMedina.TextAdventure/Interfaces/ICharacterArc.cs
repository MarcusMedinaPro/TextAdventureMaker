// <copyright file="ICharacterArc.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;

namespace MarcusMedina.TextAdventure.Interfaces;

public interface ICharacterArc
{
    string Id { get; }
    Trait StartTrait { get; }
    Trait EndTrait { get; }
    Trait CurrentTrait { get; }
    ICharacterArc StartState(Trait trait);
    ICharacterArc EndState(Trait trait);
    ICharacterArc Milestone(int index, string id, Trait unlocks);
    ICharacterArc OnComplete(Action<IGameState> action);
    bool Advance(string milestoneId, IGameState state);
}
