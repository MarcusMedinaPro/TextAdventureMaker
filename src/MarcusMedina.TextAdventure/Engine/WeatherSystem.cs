// <copyright file="WeatherSystem.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;

namespace MarcusMedina.TextAdventure.Engine;

public sealed class WeatherSystem
{
    public WeatherState Current { get; private set; } = WeatherState.Clear;

    public WeatherSystem SetWeather(WeatherState state)
    {
        Current = state;
        return this;
    }
}
