using MarcusMedina.TextAdventure.Enums;

namespace MarcusMedina.TextAdventure.Interfaces;

public interface ILocation
{
    string Id { get; }
    string GetDescription();
    ILocation? GetExit(Direction direction);
    IReadOnlyDictionary<Direction, ILocation> Exits { get; }
}
