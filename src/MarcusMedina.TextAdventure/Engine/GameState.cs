using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Engine;

public class GameState : IGameState
{
    public ILocation CurrentLocation { get; private set; }

    public GameState(ILocation startLocation)
    {
        CurrentLocation = startLocation;
    }

    public bool Move(Direction direction)
    {
        var target = CurrentLocation.GetExit(direction);
        if (target == null) return false;
        CurrentLocation = target;
        return true;
    }
}
