// <copyright file="HeroJourney.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public sealed class HeroJourney : IHeroJourney
{
    private readonly Dictionary<JourneyStage, HeroJourneyStage> _stages = new();
    private readonly List<JourneyStage> _order = [];

    public IReadOnlyList<HeroJourneyStage> Stages => _order.Select(stage => _stages[stage]).ToList();

    public HeroJourneyStage? GetStage(JourneyStage stage)
    {
        return _stages.TryGetValue(stage, out HeroJourneyStage? value) ? value : null;
    }

    public HeroJourneyStage GetOrCreateStage(JourneyStage stage)
    {
        if (!_stages.TryGetValue(stage, out HeroJourneyStage? value))
        {
            value = new HeroJourneyStage(stage);
            _stages[stage] = value;
            _order.Add(stage);
        }

        return value;
    }
}
