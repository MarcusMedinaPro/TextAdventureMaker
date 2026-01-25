using MarcusMedina.TextAdventure.Enums;

namespace MarcusMedina.TextAdventure.Interfaces;

public interface IGameState
{
    ILocation CurrentLocation { get; }
    bool Move(Direction direction);
}
