// <copyright file="IMoodSystem.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Interfaces;

public interface IMoodSystem
{
    Mood DefaultMood { get; }
    void SetMood(ILocation location, Mood mood);
    Mood GetMood(ILocation location);
    void SetLighting(ILocation location, LightLevel level);
    void SetAmbientSound(ILocation location, string sound);
    void SetSmell(ILocation location, string smell);
    void SetTemperature(ILocation location, string temperature);
    MoodDetails GetDetails(ILocation location);
    void Propagate(ILocation start, int depth = 1);
}
