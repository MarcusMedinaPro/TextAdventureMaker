using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Interfaces;

public interface ILocation
{
    string Id { get; }
    string GetDescription();
    Exit? GetExit(Direction direction);
    IReadOnlyDictionary<Direction, Exit> Exits { get; }
}
