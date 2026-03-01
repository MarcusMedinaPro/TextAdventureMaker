// <copyright file="AiFeaturePrompts.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Extensions;

namespace MarcusMedina.TextAdventure.AI.Features;

public static class AiFeaturePrompts
{
    public static string BuildNpcDialoguePrompt(NpcAiContext context, string playerInput)
    {
        string inventory = context.InventoryItems.Count == 0 ? "none" : context.InventoryItems.CommaJoin();
        string facts = context.KnownFacts is null || context.KnownFacts.Count == 0 ? "none" : context.KnownFacts.CommaJoin();

        return
            """
            You are roleplaying as an NPC in a text adventure.
            Return exactly one line in this format:
            delta:<-5..5>; reply:<short in-character reply>
            """.Trim()
            + "\n"
            + $"NPC: {context.NpcName} ({context.NpcId})\n"
            + $"Persona: {context.Persona}\n"
            + $"Location: {context.CurrentLocationId}\n"
            + $"Inventory: {inventory}\n"
            + $"Known facts: {facts}\n"
            + $"Relationship: affinity={context.RelationshipToPlayer.Affinity}, trust={context.RelationshipToPlayer.Trust}, fear={context.RelationshipToPlayer.Fear}, respect={context.RelationshipToPlayer.Respect}\n"
            + $"Player says: {playerInput}";
    }

    public static string BuildNpcMovementPrompt(NpcMovementContext context)
    {
        string goals = context.Goals.Count == 0 ? "none" : context.Goals.CommaJoin();
        string reachable = context.ReachableLocationIds.Count == 0 ? "none" : context.ReachableLocationIds.CommaJoin();
        string playerLoc = context.PlayerLocationId ?? "unknown";

        return
            """
            Decide NPC movement.
            Return exactly one line:
            move:<locationId>; reason:<short reason>
            Choose only from allowed locations.
            """.Trim()
            + "\n"
            + $"NpcId: {context.NpcId}\n"
            + $"Current: {context.CurrentLocationId}\n"
            + $"Reachable: {reachable}\n"
            + $"Goals: {goals}\n"
            + $"PlayerLocation: {playerLoc}";
    }

    public static string BuildStoryDirectorPrompt(StoryDirectorContext context)
    {
        string map = context.ConnectedLocations.Count == 0 ? "none" : context.ConnectedLocations.CommaJoin();
        string quests = context.ActiveQuestIds.Count == 0 ? "none" : context.ActiveQuestIds.CommaJoin();
        string flags = context.Flags.Count == 0 ? "none" : string.Join(", ", context.Flags.Select(kvp => $"{kvp.Key}={kvp.Value}"));
        string counters = context.Counters is null || context.Counters.Count == 0
            ? "none"
            : string.Join(", ", context.Counters.Select(kvp => $"{kvp.Key}={kvp.Value}"));

        return
            """
            Propose the next story beat.
            Return exactly one line:
            event:<id>; location:<locationId>; summary:<short summary>; consequences:<comma separated or none>
            """.Trim()
            + "\n"
            + $"CurrentLocation: {context.CurrentLocationId}\n"
            + $"ConnectedLocations: {map}\n"
            + $"ActiveQuests: {quests}\n"
            + $"Flags: {flags}\n"
            + $"Counters: {counters}";
    }

    public static string BuildCombatPrompt(CombatAiContext context)
    {
        string actions = context.AvailableActionIds.Count == 0 ? "none" : context.AvailableActionIds.CommaJoin();
        string tags = context.EnvironmentTags is null || context.EnvironmentTags.Count == 0 ? "none" : context.EnvironmentTags.CommaJoin();

        return
            """
            Decide one combat action.
            Return exactly one line:
            action:<actionId>; target:<optional>; reason:<short reason>
            Choose only from allowed actions.
            """.Trim()
            + "\n"
            + $"NpcId: {context.NpcId}\n"
            + $"NpcHealth: {context.NpcHealth}\n"
            + $"PlayerHealth: {context.PlayerHealth}\n"
            + $"Location: {context.CurrentLocationId}\n"
            + $"EnvironmentTags: {tags}\n"
            + $"AvailableActions: {actions}";
    }

    public static string BuildDescriptionPrompt(DescriptionRequest request)
    {
        string deltas = request.Deltas is null || request.Deltas.Count == 0
            ? "none"
            : string.Join(", ", request.Deltas.Select(x => $"{x.Tag}={x.Value}"));

        return
            """
            Write one concise British-English description sentence.
            Keep it stable and concrete.
            Return only the description text.
            """.Trim()
            + "\n"
            + $"EntityType: {request.EntityType}\n"
            + $"EntityId: {request.EntityId}\n"
            + $"Baseline: {request.BaselinePrompt}\n"
            + $"Deltas: {deltas}";
    }
}
