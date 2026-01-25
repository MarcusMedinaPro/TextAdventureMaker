using MarcusMedina.TextAdventure.Enums;

namespace MarcusMedina.TextAdventure.Interfaces;

public interface INpc : IGameEntity
{
    new string Id { get; }
    new string Name { get; }
    string GetDescription();
    NpcState State { get; }
    bool IsAlive { get; }
    INpcMovement Movement { get; }
    IDialogNode? DialogRoot { get; }
    IStats Stats { get; }

    INpc Description(string text);
    INpc SetState(NpcState state);
    INpc SetMovement(INpcMovement movement);
    ILocation? GetNextLocation(ILocation currentLocation, IGameState state);
    INpc Dialog(string text);
    INpc SetDialog(IDialogNode? dialog);
    INpc SetStats(IStats stats);
}
