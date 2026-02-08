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

    public IScene Participants(params string[] participants)
    {
        if (participants == null)
        {
            return this;
        }

        foreach (string participant in participants)
        {
            if (!string.IsNullOrWhiteSpace(participant))
            {
                _participants.Add(participant);
            }
        }

        return this;
    }

    public IScene Beat(int order, string eventId)
    {
        if (string.IsNullOrWhiteSpace(eventId))
        {
            return this;
        }

        _beats.Add(new SceneBeat(order, eventId));
        return this;
    }

    public SceneTransitionBuilder Transition()
    {
        return new SceneTransitionBuilder(this);
    }

    public IEnumerable<SceneBeat> Play()
    {
        return _beats
            .OrderBy(beat => beat.Order)
            .ToArray();
    }

    public bool TryGetTransition(string trigger, out SceneTransition transition)
    {
        if (string.IsNullOrWhiteSpace(trigger))
        {
            transition = new SceneTransition("", "");
            return false;
        }

        SceneTransition? match = _transitions
            .FirstOrDefault(t => string.Equals(t.Trigger, trigger, StringComparison.OrdinalIgnoreCase));

        if (match == null)
        {
            transition = new SceneTransition("", "");
            return false;
        }

        transition = match;
        return true;
    }

    internal void AddTransition(SceneTransition transition)
    {
        if (!string.IsNullOrWhiteSpace(transition.TargetSceneId) &&
            !string.IsNullOrWhiteSpace(transition.Trigger))
        {
            _transitions.Add(transition);
        }
    }
}
