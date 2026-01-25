using MarcusMedina.TextAdventure.Enums;

namespace MarcusMedina.TextAdventure.Interfaces;

public interface IDoor
{
    string Id { get; }
    string Name { get; }
    string GetDescription();
    DoorState State { get; }
    IKey? RequiredKey { get; }

    bool IsPassable { get; }
    IDoor Description(string text);
    string? GetReaction(DoorAction action);
    IDoor SetReaction(DoorAction action, string text);
    bool Open();
    bool Close();
    bool Lock(IKey key);
    bool Unlock(IKey key);
    bool Destroy();
}
