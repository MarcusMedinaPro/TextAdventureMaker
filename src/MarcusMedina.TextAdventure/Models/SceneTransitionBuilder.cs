// <copyright file="SceneTransitionBuilder.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Models;

public sealed class SceneTransitionBuilder(Scene scene)
{
    private SceneTransition? _pending;

    public SceneTransitionBuilder To(string sceneId)
    {
        if (string.IsNullOrWhiteSpace(sceneId))
        {
            _pending = null;
            return this;
        }

        _pending = new SceneTransition(sceneId, "");
        return this;
    }

    public SceneTransitionBuilder Trigger(string trigger)
    {
        if (_pending == null || string.IsNullOrWhiteSpace(trigger))
        {
            _pending = null;
            return this;
        }

        scene.AddTransition(new SceneTransition(_pending.TargetSceneId, trigger));
        _pending = null;
        return this;
    }

    public SceneTransitionBuilder Or()
    {
        _pending = null;
        return this;
    }
}
