using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Interfaces;
using System.Linq;

namespace MarcusMedina.TextAdventure.Models;

public sealed class PatrolNpcMovement : INpcMovement
{
    private readonly List<ILocation> _route;
    private int _index;

    public PatrolNpcMovement(IEnumerable<ILocation> route)
    {
        ArgumentNullException.ThrowIfNull(route);
        _route = route.Where(location => location != null).ToList();

        if (_route.Count == 0)
        {
            throw new ArgumentException("Route cannot be empty.", nameof(route));
        }
    }

    public ILocation? GetNextLocation(ILocation currentLocation, IGameState state)
    {
        var matchIndex = _route.FindIndex(location => location.Id.TextCompare(currentLocation.Id));
        if (matchIndex >= 0)
        {
            _index = matchIndex;
        }

        var nextIndex = (_index + 1) % _route.Count;
        _index = nextIndex;
        return _route[_index];
    }
}
