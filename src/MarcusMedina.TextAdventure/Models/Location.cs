using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Helpers;
using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public class Location : ILocation
{
    public string Id { get; }
    private string _description = "";
    private readonly Dictionary<Direction, Exit> _exits = new();
    private readonly List<IItem> _items = new();

    public IReadOnlyDictionary<Direction, Exit> Exits => _exits;
    public IReadOnlyList<IItem> Items => _items;

    public Location(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
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
        _exits[direction] = new Exit(target, null, oneWay);

        if (!oneWay && target is Location targetLoc)
        {
            var opposite = DirectionHelper.GetOpposite(direction);
            targetLoc._exits[opposite] = new Exit(this);
        }

        return this;
    }

    public Location AddExit(Direction direction, ILocation target, IDoor door, bool oneWay = false)
    {
        _exits[direction] = new Exit(target, door, oneWay);

        if (!oneWay && target is Location targetLoc)
        {
            var opposite = DirectionHelper.GetOpposite(direction);
            targetLoc._exits[opposite] = new Exit(this, door); // Same door from other side
        }

        return this;
    }

    public Exit? GetExit(Direction direction)
    {
        return _exits.TryGetValue(direction, out var exit) ? exit : null;
    }

    public void AddItem(IItem item)
    {
        _items.Add(item);
    }

    public bool RemoveItem(IItem item)
    {
        return _items.Remove(item);
    }

    public IItem? FindItem(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return null;
        return _items.FirstOrDefault(i => i.Matches(name));
    }

    public static implicit operator Location(string id) => new(id);
}
