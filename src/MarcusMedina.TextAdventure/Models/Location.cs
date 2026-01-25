using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Helpers;
using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public class Location : ILocation
{
    public string Id { get; }
    private string _description = "";
    private readonly Dictionary<Direction, ILocation> _exits = new();

    public IReadOnlyDictionary<Direction, ILocation> Exits => _exits;

    public Location(string id)
    {
        Id = id;
    }

    public Location Description(string text)
    {
        _description = text;
        return this;
    }

    public string GetDescription() => _description;

    public Location AddExit(Direction direction, ILocation target, bool oneWay = false)
    {
        _exits[direction] = target;

        if (!oneWay && target is Location targetLoc)
        {
            var opposite = DirectionHelper.GetOpposite(direction);
            targetLoc._exits[opposite] = this;
        }

        return this;
    }

    public ILocation? GetExit(Direction direction)
    {
        return _exits.TryGetValue(direction, out var loc) ? loc : null;
    }
}
