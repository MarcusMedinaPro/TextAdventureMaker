using MarcusMedina.TextAdventure.Extensions;
using System.Linq;

namespace MarcusMedina.TextAdventure.Models;

public sealed class LocationList
{
    private readonly Dictionary<string, Location> _locations = new(StringComparer.OrdinalIgnoreCase);

    public IReadOnlyCollection<Location> Items => _locations.Values;

    public Location Add(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return Add(new Location(name.ToId(), name));
    }

    public Location Add(string name, string description)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return Add(new Location(name.ToId(), description ?? ""));
    }

    public Location Add(Location location)
    {
        ArgumentNullException.ThrowIfNull(location);
        _locations[location.Id] = location;
        return location;
    }

    public LocationList AddMany(params string[] names)
    {
        if (names == null) return this;
        foreach (var name in names)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                Add(name);
            }
        }
        return this;
    }

    public LocationList AddMany(IEnumerable<string> names)
    {
        if (names == null) return this;
        foreach (var name in names)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                Add(name);
            }
        }
        return this;
    }

    public Location? Find(string token)
    {
        if (string.IsNullOrWhiteSpace(token)) return null;
        if (_locations.TryGetValue(token, out var location)) return location;
        return _locations.Values.FirstOrDefault(l => l.Id.TextCompare(token));
    }

    public Location Get(string token)
    {
        var location = Find(token);
        if (location == null)
        {
            throw new KeyNotFoundException($"No location found for '{token}'.");
        }
        return location;
    }

    public bool TryGet(string token, out Location location)
    {
        location = Find(token) ?? null!;
        return location != null;
    }

    public bool Remove(string token)
    {
        var location = Find(token);
        if (location == null) return false;
        return _locations.Remove(location.Id);
    }

    public void Clear() => _locations.Clear();

    public Location this[string token] => Get(token);
    public Location Call(string token) => Get(token);
}
