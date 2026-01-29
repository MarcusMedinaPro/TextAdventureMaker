// <copyright file="NoNpcMovement.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace MarcusMedina.TextAdventure.Models;

using MarcusMedina.TextAdventure.Interfaces;

public sealed class NoNpcMovement : INpcMovement
{
    public ILocation? GetNextLocation(ILocation currentLocation, IGameState state) => null;
}
