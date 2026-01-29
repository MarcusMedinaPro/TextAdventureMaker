// <copyright file="EventSystem.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace MarcusMedina.TextAdventure.Engine;

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Models;

public sealed class EventSystem : IEventSystem
{
    private readonly Dictionary<GameEventType, List<Action<GameEvent>>> _handlers = [];

    public void Subscribe(GameEventType type, Action<GameEvent> handler)
    {
        ArgumentNullException.ThrowIfNull(handler);
        if (!_handlers.TryGetValue(type, out var list))
        {
            list = [];
            _handlers[type] = list;
        }

        if (!list.Contains(handler))
        {
            list.Add(handler);
        }
    }

    public void Unsubscribe(GameEventType type, Action<GameEvent> handler)
    {
        if (_handlers.TryGetValue(type, out var list))
        {
            _ = list.Remove(handler);
            if (list.Count == 0)
            {
                _ = _handlers.Remove(type);
            }
        }
    }

    public void Publish(GameEvent gameEvent)
    {
        ArgumentNullException.ThrowIfNull(gameEvent);
        if (!_handlers.TryGetValue(gameEvent.Type, out var list))
            return;

        foreach (var handler in list.ToArray())
        {
            handler(gameEvent);
        }
    }
}
