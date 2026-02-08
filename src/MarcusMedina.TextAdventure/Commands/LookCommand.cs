// <copyright file="LookCommand.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Helpers;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;
using MarcusMedina.TextAdventure.Models;
using System.Text;

namespace MarcusMedina.TextAdventure.Commands;

public class LookCommand : ICommand
{
    public string? Target { get; }

    public LookCommand(string? target = null)
    {
        Target = target;
    }

    public CommandResult Execute(CommandContext context)
    {
        ILocation location = context.State.CurrentLocation;
        if (!string.IsNullOrWhiteSpace(Target))
        {
            return ExecuteTarget(context, Target);
        }

        string description = location.GetDescription();
        IEnumerable<KeyValuePair<Direction, Exit>> exitsSource = location.Exits
            .Where(e => e.Value.IsVisible)
            .Where(e => !context.State.ShowDirectionsWhenThereAreDirectionsVisibleOnly ||
                        e.Value.Door == null ||
                        e.Value.Door.State != DoorState.Locked);

        IEnumerable<string> exits = exitsSource
            .Select(e => e.Value.Door != null
                ? $"{e.Key} ({Language.EntityName(e.Value.Door)}: {e.Value.Door.State})"
                : e.Key.ToString());

        StringBuilder builder = new();
        if (!string.IsNullOrWhiteSpace(description))
        {
            _ = builder.Append(description.Trim());
        }

        _ = builder.Append(builder.Length > 0 ? "\n" : string.Empty);
        _ = builder.Append(Language.HealthStatus(context.State.Stats.Health, context.State.Stats.MaxHealth));
        _ = builder.Append("\n");
        List<string> items = location.Items
            .Where(i => !i.HiddenFromItemList)
            .Select(i => Language.ItemWithWeight(Language.EntityName(i), i.Weight))
            .ToList();
        if (!context.State.ShowItemsListOnlyWhenThereAreActuallyThingsToInteractWith || items.Count > 0)
        {
            _ = builder.Append(Language.ItemsHereLabel);
            _ = builder.Append(items.Count > 0 ? items.CommaJoin() : Language.None);
            _ = builder.Append("\n");
        }

        List<string> exitsList = exits.ToList();
        if (!context.State.ShowDirectionsWhenThereAreDirectionsVisibleOnly || exitsList.Count > 0)
        {
            _ = builder.Append(Language.ExitsLabel);
            _ = builder.Append(exitsList.Count > 0 ? exitsList.CommaJoin() : Language.None);
        }

        return CommandResult.Ok(builder.ToString());
    }

    internal static CommandResult ExecuteTarget(CommandContext context, string target)
    {
        ILocation location = context.State.CurrentLocation;
        IItem? item = location.FindItem(target) ?? context.State.Inventory.FindItem(target);
        string? suggestion = null;
        if (item == null && context.State.EnableFuzzyMatching && !FuzzyMatcher.IsLikelyCommandToken(target))
        {
            IEnumerable<IItem> candidates = location.Items.Concat(context.State.Inventory.Items);
            IItem? best = FuzzyMatcher.FindBestItem(candidates, target, context.State.FuzzyMaxDistance);
            if (best != null)
            {
                item = best;
                suggestion = Language.EntityName(best);
            }
        }

        if (item != null)
        {
            string description = item.GetDescription();
            CommandResult result = CommandResult.Ok(string.IsNullOrWhiteSpace(description)
                ? Language.ItemDescription(Language.EntityName(item))
                : description);
            return suggestion != null ? result.WithSuggestion(suggestion) : result;
        }

        IDoor? door = location.Exits.Values
            .Select(e => e.Door)
            .FirstOrDefault(d => d != null && (target.TextCompare("door") || d.Matches(target)));

        if (door == null && context.State.EnableFuzzyMatching && !FuzzyMatcher.IsLikelyCommandToken(target))
        {
            IEnumerable<IDoor> doors = location.Exits.Values.Select(e => e.Door).Where(d => d != null).Cast<IDoor>();
            door = FuzzyMatcher.FindBestDoor(doors, target, context.State.FuzzyMaxDistance);
            if (door != null)
            {
                suggestion = Language.EntityName(door);
            }
        }

        if (door != null)
        {
            string description = door.GetDescription();
            CommandResult result = CommandResult.Ok(string.IsNullOrWhiteSpace(description)
                ? Language.ItemDescription(Language.EntityName(door))
                : description);
            return suggestion != null ? result.WithSuggestion(suggestion) : result;
        }

        IKey? key = location.Exits.Values
            .Select(e => e.Door?.RequiredKey)
            .FirstOrDefault(k => k != null && k.Name.TextCompare(target));

        if (key == null && context.State.EnableFuzzyMatching && !FuzzyMatcher.IsLikelyCommandToken(target))
        {
            IEnumerable<IItem> keys = location.Exits.Values
                .Select(e => e.Door?.RequiredKey)
                .Where(k => k != null)
                .Cast<IItem>();
            key = FuzzyMatcher.FindBestItem(keys, target, context.State.FuzzyMaxDistance) as IKey;
            if (key != null)
            {
                suggestion = Language.EntityName(key);
            }
        }

        if (key != null)
        {
            string description = key.GetDescription();
            CommandResult result = CommandResult.Ok(string.IsNullOrWhiteSpace(description)
                ? Language.ItemDescription(Language.EntityName(key))
                : description);
            return suggestion != null ? result.WithSuggestion(suggestion) : result;
        }

        if (target.TextCompare(location.Id) || target.TextCompare("here") || target.TextCompare("room"))
        {
            string description = location.GetDescription();
            return CommandResult.Ok(string.IsNullOrWhiteSpace(description)
                ? Language.NothingToLookAt
                : description);
        }

        return CommandResult.Fail(Language.NothingToLookAt, GameError.ItemNotFound);
    }
}
