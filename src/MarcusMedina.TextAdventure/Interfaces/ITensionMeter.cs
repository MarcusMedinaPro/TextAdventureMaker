// <copyright file="ITensionMeter.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Interfaces;

public interface ITensionMeter
{
    float Current { get; }
    ITensionMeter Set(float value);
    ITensionMeter Modify(float delta);
}
