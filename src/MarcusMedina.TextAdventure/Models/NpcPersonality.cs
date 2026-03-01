// <copyright file="NpcPersonality.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Models;

/// <summary>
/// Defines an NPC's personality using the Big Five personality traits model.
/// Each trait ranges from 0.0 (low) to 1.0 (high).
/// </summary>
public sealed record NpcPersonality(
    float Openness = 0.5f,
    float Conscientiousness = 0.5f,
    float Extraversion = 0.5f,
    float Agreeableness = 0.5f,
    float Neuroticism = 0.5f
)
{
    /// <summary>
    /// Indicates if this NPC is willing to help others.
    /// </summary>
    public bool IsWillingToHelp => Agreeableness > 0.6f;

    /// <summary>
    /// Indicates if this NPC is suspicious or paranoid.
    /// </summary>
    public bool IsSuspicious => Neuroticism > 0.6f && Agreeableness < 0.4f;

    /// <summary>
    /// Indicates if this NPC is talkative and social.
    /// </summary>
    public bool IsTalkative => Extraversion > 0.6f;

    /// <summary>
    /// Indicates if this NPC is reserved and quiet.
    /// </summary>
    public bool IsReserved => Extraversion < 0.4f;

    /// <summary>
    /// Gets a mood modifier string based on the relationship and personality.
    /// </summary>
    public string GetMoodModifier(float relationship)
    {
        if (relationship > 50 && Extraversion > 0.5f)
            return "warmly";

        if (relationship < -20 && Agreeableness < 0.4f)
            return "coldly";

        if (Neuroticism > 0.7f)
            return "nervously";

        return "";
    }
}
