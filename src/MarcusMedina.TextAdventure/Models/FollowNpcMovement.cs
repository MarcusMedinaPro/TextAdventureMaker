// <copyright file="FollowNpcMovement.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public sealed class FollowNpcMovement : INpcMovement
{
    public ILocation? GetNextLocation(ILocation currentLocation, IGameState state)
    {
        if (currentLocation.Id.TextCompare(state.CurrentLocation.Id)) return null;
        return state.CurrentLocation;
    }
}
