using MarcusMedina.TextAdventure.Enums;

namespace MarcusMedina.TextAdventure.Interfaces;

public interface INpc
{
    string Id { get; }
    string Name { get; }
    string GetDescription();
    NpcState State { get; }
    bool IsAlive { get; }

    INpc Description(string text);
    INpc SetState(NpcState state);
}
