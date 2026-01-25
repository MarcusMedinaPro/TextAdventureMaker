using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public sealed class FollowNpcMovement : INpcMovement
{
    public ILocation? GetNextLocation(ILocation currentLocation, IGameState state)
    {
        if (currentLocation.Id.TextCompare(state.CurrentLocation.Id)) return null;
        return state.CurrentLocation;
    }
}
