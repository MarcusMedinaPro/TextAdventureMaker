// <copyright file="FollowNpcMovement.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace MarcusMedina.TextAdventure.Models;

using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Interfaces;

public sealed class FollowNpcMovement : INpcMovement
{
    public ILocation? GetNextLocation(ILocation currentLocation, IGameState state) => currentLocation.Id.TextCompare(state.CurrentLocation.Id) ? null : state.CurrentLocation;
}
