using MarcusMedina.TextAdventure.Interfaces;
using System.Linq;

namespace MarcusMedina.TextAdventure.Models;

public sealed class RandomNpcMovement : INpcMovement
{
    private readonly Random _random;

    public RandomNpcMovement(Random? random = null)
    {
        _random = random ?? new Random();
    }

    public ILocation? GetNextLocation(ILocation currentLocation, IGameState state)
    {
        var exits = currentLocation.Exits.Values
            .Where(exit => exit.IsPassable)
            .ToList();

        if (exits.Count == 0) return null;

        var nextExit = exits[_random.Next(exits.Count)];
        return nextExit.Target;
    }
}
