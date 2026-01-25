using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Engine;

public sealed class EventSystem : IEventSystem
{
    private readonly Dictionary<GameEventType, List<Action<GameEvent>>> _handlers = new();

    public void Subscribe(GameEventType type, Action<GameEvent> handler)
    {
        ArgumentNullException.ThrowIfNull(handler);
        if (!_handlers.TryGetValue(type, out var list))
        {
            list = new List<Action<GameEvent>>();
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
            list.Remove(handler);
            if (list.Count == 0)
            {
                _handlers.Remove(type);
            }
        }
    }

    public void Publish(GameEvent gameEvent)
    {
        ArgumentNullException.ThrowIfNull(gameEvent);
        if (!_handlers.TryGetValue(gameEvent.Type, out var list)) return;

        foreach (var handler in list.ToArray())
        {
            handler(gameEvent);
        }
    }
}
