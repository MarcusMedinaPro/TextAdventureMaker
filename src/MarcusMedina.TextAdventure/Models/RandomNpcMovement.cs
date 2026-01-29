// <copyright file="RandomNpcMovement.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace MarcusMedina.TextAdventure.Models;

using System.Linq;
using MarcusMedina.TextAdventure.Interfaces;

public sealed class RandomNpcMovement : INpcMovement
{
    private readonly Random _random;

    public RandomNpcMovement(Random? random = null) => _random = random ?? new Random();

    public ILocation? GetNextLocation(ILocation currentLocation, IGameState state)
    {
        var exits = currentLocation.Exits.Values
            .Where(exit => exit.IsPassable)
            .ToList();

        if (exits.Count == 0)
            return null;

        var nextExit = exits[_random.Next(exits.Count)];
        return nextExit.Target;
    }
}
