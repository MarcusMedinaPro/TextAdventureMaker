// <copyright file="HeroJourneyDslParser.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Engine;

public sealed class HeroJourneyDslParser
{
    public HeroJourney Parse(string dsl)
    {
        _ = dsl;
        return new HeroJourney();
    }
}
