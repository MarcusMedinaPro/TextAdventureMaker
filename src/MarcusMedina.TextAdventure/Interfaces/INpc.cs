using MarcusMedina.TextAdventure.Enums;

namespace MarcusMedina.TextAdventure.Interfaces;

public interface INpc
{
    string Id { get; }
    string Name { get; }
    string GetDescription();
    NpcState State { get; }
    bool IsAlive { get; }
    INpcMovement Movement { get; }
    IDialogNode? DialogRoot { get; }

    INpc Description(string text);
    INpc SetState(NpcState state);
    INpc SetMovement(INpcMovement movement);
    ILocation? GetNextLocation(ILocation currentLocation, IGameState state);
    INpc Dialog(string text);
    INpc SetDialog(IDialogNode? dialog);
}
