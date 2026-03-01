// <copyright file="JourneyValidator.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Engine;

public sealed class JourneyValidator
{
    private static readonly JourneyStage[] RequiredStages =
    [
        JourneyStage.OrdinaryWorld,
        JourneyStage.CallToAdventure,
        JourneyStage.CrossingThreshold,
        JourneyStage.Ordeal,
        JourneyStage.ReturnWithElixir
    ];

    public IReadOnlyCollection<string> Validate(IHeroJourney journey)
    {
        if (journey == null)
        {
            return ["Journey is missing."];
        }

        List<string> warnings = [];

        foreach (JourneyStage stage in RequiredStages)
        {
            if (journey.GetStage(stage) == null)
            {
                warnings.Add($"Missing stage: {stage}");
            }
        }

        if (IsOutOfOrder(journey, JourneyStage.Reward, JourneyStage.Ordeal))
        {
            warnings.Add("Reward appears before Ordeal.");
        }

        if (IsOutOfOrder(journey, JourneyStage.CrossingThreshold, JourneyStage.MeetingMentor))
        {
            warnings.Add("Mentor appears after Threshold.");
        }

        return warnings;
    }

    private static bool IsOutOfOrder(IHeroJourney journey, JourneyStage later, JourneyStage earlier)
    {
        int laterIndex = IndexOf(journey, later);
        int earlierIndex = IndexOf(journey, earlier);
        return laterIndex >= 0 && earlierIndex >= 0 && laterIndex < earlierIndex;
    }

    private static int IndexOf(IHeroJourney journey, JourneyStage stage)
    {
        for (int i = 0; i < journey.Stages.Count; i++)
        {
            if (journey.Stages[i].Stage == stage)
            {
                return i;
            }
        }

        return -1;
    }
}
