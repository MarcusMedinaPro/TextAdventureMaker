// <copyright file="VisibilityCalculator.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Engine;

/// <summary>
/// Calculates how visible the room beyond an exit is, based on door state and transparency.
/// </summary>
public static class VisibilityCalculator
{
    public static float Calculate(Exit exit) => exit.Door switch
    {
        null => 1.0f,
        { State: DoorState.Open } => 1.0f,
        { State: DoorState.Closed } door => door.GetBoolProperty("transparent") ? 0.5f : 0.0f,
        { State: DoorState.Locked } door => door.GetBoolProperty("transparent") ? 0.3f : 0.0f,
        _ => 0.0f
    };
}
