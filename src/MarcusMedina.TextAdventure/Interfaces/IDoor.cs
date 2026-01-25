using MarcusMedina.TextAdventure.Enums;

namespace MarcusMedina.TextAdventure.Interfaces;

public interface IDoor
{
    string Id { get; }
    string Name { get; }
    DoorState State { get; }
    IKey? RequiredKey { get; }

    bool IsPassable { get; }
    bool Open();
    bool Close();
    bool Lock(IKey key);
    bool Unlock(IKey key);
    bool Destroy();
}
