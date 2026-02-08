// <copyright file="GameValidator.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Engine;

public sealed class GameValidator(Game game)
{
    private readonly Game _game = game ?? throw new ArgumentNullException(nameof(game));
    private readonly List<TargetStory> _targetStories = [];

    public IReadOnlyCollection<string> FindUnreachableLocations()
    {
        HashSet<string> visited = new(StringComparer.OrdinalIgnoreCase);
        Queue<ILocation> queue = new();
        queue.Enqueue(_game.State.CurrentLocation);
        visited.Add(_game.State.CurrentLocation.Id);

        while (queue.Count > 0)
        {
            ILocation current = queue.Dequeue();
            foreach (Exit exit in current.Exits.Values)
            {
                if (visited.Add(exit.Target.Id))
                {
                    queue.Enqueue(exit.Target);
                }
            }
        }

        return _game.State.Locations
            .Select(location => location.Id)
            .Where(id => !visited.Contains(id))
            .ToList();
    }

    public IReadOnlyCollection<(string ItemId, string Reason)> FindUnreachableItems()
    {
        return [];
    }

    public IReadOnlyCollection<string> FindImpossibleQuests()
    {
        return [];
    }

    public IReadOnlyCollection<string> GetPossibleCommands(ILocation location)
    {
        List<string> commands = ["look", "inventory"];
        foreach (Direction direction in location.Exits.Keys)
        {
            commands.Add($"go {direction.ToString().ToLowerInvariant()}");
        }

        foreach (IItem item in location.Items)
        {
            commands.Add($"take {item.Name.ToLowerInvariant()}");
        }

        foreach (INpc npc in location.Npcs)
        {
            commands.Add($"talk to {npc.Name.ToLowerInvariant()}");
        }

        return commands;
    }

    public IReadOnlyCollection<string> FindUnusedCommands()
    {
        return [];
    }

    public IReadOnlyCollection<string> FindDeadEnds()
    {
        return [];
    }

    public IReadOnlyCollection<string> FindUnmarkedPointsOfNoReturn()
    {
        return [];
    }

    public PuzzleGraph GeneratePuzzleGraph()
    {
        return new PuzzleGraph();
    }

    public TargetStoryBuilder AddTargetStory(string id)
    {
        TargetStory story = new(id);
        _targetStories.Add(story);
        return new TargetStoryBuilder(story);
    }

    public IReadOnlyCollection<TargetStoryResult> ValidateTargetStories()
    {
        List<TargetStoryResult> results = [];
        foreach (TargetStory story in _targetStories)
        {
            List<string> missing = [];
            foreach (TargetStoryRequirement requirement in story.Requirements)
            {
                if (string.IsNullOrWhiteSpace(requirement.Key))
                {
                    continue;
                }

                if (requirement.Min.HasValue && requirement.Min.Value > 0)
                {
                    int value = _game.State.WorldState.GetCounter(requirement.Key);
                    if (value < requirement.Min.Value)
                    {
                        missing.Add($"{requirement.Key} < {requirement.Min.Value}");
                    }
                }
                else if (!_game.State.WorldState.GetFlag(requirement.Key))
                {
                    missing.Add(requirement.Key);
                }
            }

            bool achievable = missing.Count == 0;
            string details = achievable
                ? "Achievable with current world state."
                : $"Missing: {string.Join(", ", missing)}";
            results.Add(new TargetStoryResult(story.Id, achievable, details));
        }

        return results;
    }
}
