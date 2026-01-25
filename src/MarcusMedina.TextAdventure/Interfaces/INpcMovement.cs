namespace MarcusMedina.TextAdventure.Interfaces;

public interface INpcMovement
{
    ILocation? GetNextLocation(ILocation currentLocation, IGameState state);
}
