// <copyright file="MoodDetails.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;

namespace MarcusMedina.TextAdventure.Models;

public sealed class MoodDetails
{
    public Mood Mood { get; set; }
    public LightLevel? Lighting { get; set; }
    public string? AmbientSound { get; set; }
    public string? Smell { get; set; }
    public string? Temperature { get; set; }
}
