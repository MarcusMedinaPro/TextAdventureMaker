// <copyright file="IHeroJourney.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Interfaces;

public interface IHeroJourney
{
    IReadOnlyList<HeroJourneyStage> Stages { get; }
    HeroJourneyStage? GetStage(JourneyStage stage);
}
