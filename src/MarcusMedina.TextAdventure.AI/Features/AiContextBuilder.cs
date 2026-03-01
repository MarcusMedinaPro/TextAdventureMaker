// <copyright file="AiContextBuilder.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.AI.Features;

public static class AiContextBuilder
{
    public static NpcMovementContext BuildNpcMovementContext(
        INpc npc,
        ILocation currentLocation,
        IEnumerable<string>? goals = null,
        string? playerLocationId = null)
    {
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
            Goals: goals?.ToArray() ?? [],
            PlayerLocationId: playerLocationId);
    }

    public static StoryDirectorContext BuildStoryDirectorContext(
        IGameState state,
        IEnumerable<string>? activeQuestIds = null,
        IReadOnlyDictionary<string, bool>? flags = null,
        IReadOnlyDictionary<string, int>? counters = null)
    {
        ArgumentNullException.ThrowIfNull(state);

        ILocation current = state.CurrentLocation;
        IReadOnlyList<string> connected = current.Exits.Values
            .Select(exit => exit.Target.Id)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        return new StoryDirectorContext(
            CurrentLocationId: current.Id,
            ConnectedLocations: connected,
            ActiveQuestIds: activeQuestIds?.ToArray() ?? [],
            Flags: flags ?? new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase),
            Counters: counters ?? new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase));
    }

    public static CombatAiContext BuildCombatContext(
        INpc npc,
        IGameState state,
        ILocation currentLocation,
        IEnumerable<string> availableActions,
        IEnumerable<string>? environmentTags = null)
    {
        ArgumentNullException.ThrowIfNull(npc);
        ArgumentNullException.ThrowIfNull(state);
        ArgumentNullException.ThrowIfNull(currentLocation);
        ArgumentNullException.ThrowIfNull(availableActions);

        return new CombatAiContext(
            NpcId: npc.Id,
            NpcHealth: npc.Stats.Health,
            PlayerHealth: state.Stats.Health,
            AvailableActionIds: availableActions.ToArray(),
            CurrentLocationId: currentLocation.Id,
            EnvironmentTags: environmentTags?.ToArray() ?? []);
    }

    public static DescriptionRequest BuildRoomDescriptionRequest(
        ILocation location,
        string baselinePrompt,
        IEnumerable<DescriptionDelta>? deltas = null)
    {
        ArgumentNullException.ThrowIfNull(location);
        return new DescriptionRequest("room", location.Id, baselinePrompt, deltas?.ToArray());
    }

    public static DescriptionRequest BuildItemDescriptionRequest(
        IItem item,
        string baselinePrompt,
        IEnumerable<DescriptionDelta>? deltas = null)
    {
        ArgumentNullException.ThrowIfNull(item);
        return new DescriptionRequest("item", item.Id, baselinePrompt, deltas?.ToArray());
    }
}
