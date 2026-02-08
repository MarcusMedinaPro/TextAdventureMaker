// <copyright file="TensionMeter.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Engine;

public sealed class TensionMeter : ITensionMeter
{
    public float Current { get; private set; }

    public ITensionMeter Set(float value)
    {
        Current = Math.Clamp(value, 0f, 1f);
        return this;
    }

    public ITensionMeter Modify(float delta)
    {
        return Set(Current + delta);
    }
}
