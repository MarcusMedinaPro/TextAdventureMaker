using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Interfaces;

public interface IEventSystem
{
    void Subscribe(GameEventType type, Action<GameEvent> handler);
    void Unsubscribe(GameEventType type, Action<GameEvent> handler);
    void Publish(GameEvent gameEvent);
}
