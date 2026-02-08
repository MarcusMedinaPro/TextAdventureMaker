// <copyright file="IScene.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Interfaces;

public interface IScene
{
    string Id { get; }
    string? LocationId { get; }
    IReadOnlyCollection<string> Participants { get; }
    IReadOnlyList<SceneBeat> Beats { get; }
    IReadOnlyList<SceneTransition> Transitions { get; }
    IScene Location(string locationId);
    IScene Participants(params string[] participants);
    IScene Beat(int order, string eventId);
    SceneTransitionBuilder Transition();
    IEnumerable<SceneBeat> Play();
    bool TryGetTransition(string trigger, out SceneTransition transition);
}
