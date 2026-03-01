// <copyright file="ContextualDialogSystem.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

/// <summary>
/// Generates contextual dialogue responses based on NPC personality, memory, and relationships.
/// </summary>
public sealed class ContextualDialogSystem
{
    /// <summary>
    /// Generates a contextually appropriate response from an NPC.
    /// </summary>
    public string GenerateResponse(INpc npc, DialogContext context)
    {
        var memory = npc.Memory;
        var personality = npc.Personality;
        var relationship = context.State.Factions?.GetReputation(npc.Id) ?? 0f;

        // Select responses based on context
        var responses = npc.DialogRules
            .Where(r => r.Matches(context))
            .OrderByDescending(r => r.PriorityValue)
            .ToList();

        if (responses.Count == 0)
            return GetDefaultResponse(npc, personality);

        var response = responses.First();

        // Modify based on personality and relationship
        var text = response.GetText(context) ?? "";
        var mood = personality.GetMoodModifier(relationship);

        if (!string.IsNullOrEmpty(mood))
            text = $"{npc.Name} says {mood}: \"{text}\"";
        else
            text = $"{npc.Name} says: \"{text}\"";

        return text;
    }

    private static string GetDefaultResponse(INpc npc, NpcPersonality personality)
    {
        if (personality.IsTalkative)
            return $"{npc.Name} seems eager to chat but doesn't know about that.";

        if (personality.IsReserved)
            return $"{npc.Name} shrugs silently.";

        return $"{npc.Name} doesn't seem to know about that.";
    }
}
