// <copyright file="AudibilityCalculator.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Engine;

/// <summary>
/// Calculates how audible sounds from the room beyond an exit are, based on door state and soundproofing.
/// </summary>
public static class AudibilityCalculator
{
    public static float Calculate(Exit exit) => exit.Door switch
    {
        null => 1.0f,
        { State: DoorState.Open } => 1.0f,
        var door => 1.0f - door.GetProperty<float>("soundproofing", 0.5f)
    };
}
