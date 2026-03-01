// <copyright file="AiPluginContextFactory.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.AI.Plugin;

internal static class AiPluginContextFactory
{
    public static NpcAiContext BuildNpcContext(IGameState state, INpc npc)
    {
        ArgumentNullException.ThrowIfNull(state);
        ArgumentNullException.ThrowIfNull(npc);

        int affinity = state.WorldState.GetRelationship(npc.Id);
        NpcRelationshipSnapshot relationship = new(affinity);
        List<string> facts = [];
        if (npc.Memory.HasMet)
            facts.Add("met_player");

        return new NpcAiContext(
            NpcId: npc.Id,
            NpcName: npc.Name,
            Persona: npc.GetDescription(),
            CurrentLocationId: state.CurrentLocation.Id,
            InventoryItems: [],
            RelationshipToPlayer: relationship,
            KnownFacts: facts);
    }

    public static NpcMovementContext BuildNpcMovementContext(IGameState state, INpc npc, ILocation currentLocation)
    {
        ArgumentNullException.ThrowIfNull(state);
        ArgumentNullException.ThrowIfNull(npc);
        ArgumentNullException.ThrowIfNull(currentLocation);

        IReadOnlyList<string> reachable = currentLocation.Exits.Values
            .Where(exit => exit.IsVisible && exit.IsPassable)
            .Select(exit => exit.Target.Id)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        return new NpcMovementContext(
            NpcId: npc.Id,
            CurrentLocationId: currentLocation.Id,
            ReachableLocationIds: reachable,
            Goals: InferGoals(npc, state),
            PlayerLocationId: state.CurrentLocation.Id);
    }

    public static StoryDirectorContext BuildStoryContext(IGameState state)
    {
        ArgumentNullException.ThrowIfNull(state);

        IReadOnlyList<string> connected = state.CurrentLocation.Exits.Values
            .Where(exit => exit.IsVisible)
            .Select(exit => exit.Target.Id)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        return new StoryDirectorContext(
            CurrentLocationId: state.CurrentLocation.Id,
            ConnectedLocations: connected,
            ActiveQuestIds: [],
            Flags: new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase),
            Counters: new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase));
    }

    private static IReadOnlyList<string> InferGoals(INpc npc, IGameState state)
    {
        List<string> goals = [];
        if (npc.Stats.Health < 8)
            goals.Add("survive");

        if (state.WorldState.GetRelationship(npc.Id) < -25)
            goals.Add("avoid_player");

        if (goals.Count == 0)
            goals.Add("patrol");

        return goals;
    }
}
