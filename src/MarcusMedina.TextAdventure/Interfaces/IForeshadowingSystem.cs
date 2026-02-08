// <copyright file="IForeshadowingSystem.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Interfaces;

public interface IForeshadowingSystem
{
    IReadOnlyCollection<string> Unpaid { get; }
    IReadOnlyCollection<string> Unhinted { get; }
    void Plant(string tag);
    void Hint(string tag);
    void Link(string tag, string linkId);
    IReadOnlyCollection<string> TagsFor(string linkId);
    void Payoff(string tag, IGameState? state = null, Action<IGameState>? missedHintCallback = null);
}
