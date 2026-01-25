using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Engine;

public class GameState : IGameState
{
    public ILocation CurrentLocation { get; private set; }
    public string? LastMoveError { get; private set; }

    public GameState(ILocation startLocation)
    {
        CurrentLocation = startLocation;
    }

    public bool Move(Direction direction)
    {
        LastMoveError = null;
        var exit = CurrentLocation.GetExit(direction);

        if (exit == null)
        {
            LastMoveError = "You can't go that way.";
            return false;
        }

        if (!exit.IsPassable)
        {
            LastMoveError = exit.Door?.State == DoorState.Locked
                ? $"The {exit.Door.Name} is locked."
                : $"The {exit.Door?.Name} is closed.";
            return false;
        }

        CurrentLocation = exit.Target;
        return true;
    }
}
