// <copyright file="Scene.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public sealed class Scene(string id) : IScene
{
    private readonly List<SceneBeat> _beats = [];
    private readonly List<SceneTransition> _transitions = [];
    private readonly HashSet<string> _participants = new(StringComparer.OrdinalIgnoreCase);

    public string Id { get; } = id ?? "";
    public string? LocationId { get; private set; }
    public IReadOnlyCollection<string> Participants => _participants;
    public IReadOnlyList<SceneBeat> Beats => _beats;
    public IReadOnlyList<SceneTransition> Transitions => _transitions;

    public IScene Location(string locationId)
    {
        LocationId = locationId ?? "";
        return this;
    }

    public IScene SetParticipants(params string[] participants)
    {
        if (participants is null)
            return this;

        foreach (var participant in participants)
        {
            if (!string.IsNullOrWhiteSpace(participant))
                _participants.Add(participant);
        }

        return this;
    }

    public IScene Beat(int order, string eventId)
    {
        if (string.IsNullOrWhiteSpace(eventId))
            return this;

        _beats.Add(new SceneBeat(order, eventId));
        return this;
    }

    public SceneTransitionBuilder Transition() => new(this);

    public IEnumerable<SceneBeat> Play() =>
        [.. _beats.OrderBy(beat => beat.Order)];

    public bool TryGetTransition(string trigger, out SceneTransition transition)
    {
        if (string.IsNullOrWhiteSpace(trigger))
        {
            transition = new SceneTransition("", "");
            return false;
        }

        if (_transitions.FirstOrDefault(t => string.Equals(t.Trigger, trigger, StringComparison.OrdinalIgnoreCase)) is { } match)
        {
            transition = match;
            return true;
        }

        transition = new SceneTransition("", "");
        return false;
    }

    internal void AddTransition(SceneTransition transition)
    {
        if (!string.IsNullOrWhiteSpace(transition.TargetSceneId) &&
            !string.IsNullOrWhiteSpace(transition.Trigger))
            _transitions.Add(transition);
    }
}
