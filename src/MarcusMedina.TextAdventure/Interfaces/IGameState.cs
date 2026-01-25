using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Interfaces;

public interface IGameState
{
    ILocation CurrentLocation { get; }
    bool Move(Direction direction);
    bool IsCurrentRoomId(string id);
    GameError LastMoveErrorCode { get; }
    RecipeBook RecipeBook { get; }
}
