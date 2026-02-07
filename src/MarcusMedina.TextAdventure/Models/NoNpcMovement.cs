// <copyright file="NoNpcMovement.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public sealed class NoNpcMovement : INpcMovement
{
    public ILocation? GetNextLocation(ILocation currentLocation, IGameState state)
    {
        return null;
    }
}
