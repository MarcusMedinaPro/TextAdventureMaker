// <copyright file="LookCommand.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace MarcusMedina.TextAdventure.Commands;

using System.Text;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Helpers;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;

public class LookCommand : ICommand
{
    public string? Target { get; }

    public LookCommand(string? target = null) => Target = target;

    public CommandResult Execute(CommandContext context)
    {
        var location = context.State.CurrentLocation;
        if (!string.IsNullOrWhiteSpace(Target))
        {
            return ExecuteTarget(context, Target);
        }

        var description = location.GetDescription();
        var exitsSource = location.Exits
            .Where(e => !context.State.ShowDirectionsWhenThereAreDirectionsVisibleOnly ||
                        e.Value.Door == null ||
                        e.Value.Door.State != DoorState.Locked);

        var exits = exitsSource
            .Select(e => e.Value.Door != null
                ? $"{e.Key} ({e.Value.Door.Name}: {e.Value.Door.State})"
                : e.Key.ToString());

        var builder = new StringBuilder();
        if (!string.IsNullOrWhiteSpace(description))
        {
            _ = builder.Append(description.Trim());
        }

        _ = builder.Append(builder.Length > 0 ? "\n" : string.Empty);
        _ = builder.Append(Language.HealthStatus(context.State.Stats.Health, context.State.Stats.MaxHealth));
        _ = builder.Append("\n");
        var items = location.Items
            .Where(i => !i.HiddenFromItemList)
            .Select(i => Language.ItemWithWeight(i.Name, i.Weight))
            .ToList();
        if (!context.State.ShowItemsListOnlyWhenThereAreActuallyThingsToInteractWith || items.Count > 0)
        {
            _ = builder.Append(Language.ItemsHereLabel);
            _ = builder.Append(items.Count > 0 ? items.CommaJoin() : Language.None);
            _ = builder.Append("\n");
        }

        var exitsList = exits.ToList();
        if (!context.State.ShowDirectionsWhenThereAreDirectionsVisibleOnly || exitsList.Count > 0)
        {
            _ = builder.Append(Language.ExitsLabel);
            _ = builder.Append(exitsList.Count > 0 ? exitsList.CommaJoin() : Language.None);
        }

        return CommandResult.Ok(builder.ToString());
    }

    internal static CommandResult ExecuteTarget(CommandContext context, string target)
    {
        var location = context.State.CurrentLocation;
        var item = location.FindItem(target) ?? context.State.Inventory.FindItem(target);
        string? suggestion = null;
        if (item == null && context.State.EnableFuzzyMatching && !FuzzyMatcher.IsLikelyCommandToken(target))
        {
            var candidates = location.Items.Concat(context.State.Inventory.Items);
            var best = FuzzyMatcher.FindBestItem(candidates, target, context.State.FuzzyMaxDistance);
            if (best != null)
            {
                item = best;
                suggestion = best.Name;
            }
        }

        if (item != null)
        {
            var description = item.GetDescription();
            var result = CommandResult.Ok(string.IsNullOrWhiteSpace(description)
                ? Language.ItemDescription(item.Name)
                : description);
            return suggestion != null ? result.WithSuggestion(suggestion) : result;
        }

        var door = location.Exits.Values
            .Select(e => e.Door)
            .FirstOrDefault(d => d != null && (target.TextCompare("door") || d.Matches(target)));

        if (door == null && context.State.EnableFuzzyMatching && !FuzzyMatcher.IsLikelyCommandToken(target))
        {
            var doors = location.Exits.Values.Select(e => e.Door).Where(d => d != null).Cast<IDoor>();
            door = FuzzyMatcher.FindBestDoor(doors, target, context.State.FuzzyMaxDistance);
            if (door != null)
            {
                suggestion = door.Name;
            }
        }

        if (door != null)
        {
            var description = door.GetDescription();
            var result = CommandResult.Ok(string.IsNullOrWhiteSpace(description)
                ? Language.ItemDescription(door.Name)
                : description);
            return suggestion != null ? result.WithSuggestion(suggestion) : result;
        }

        var key = location.Exits.Values
            .Select(e => e.Door?.RequiredKey)
            .FirstOrDefault(k => k != null && k.Name.TextCompare(target));

        if (key == null && context.State.EnableFuzzyMatching && !FuzzyMatcher.IsLikelyCommandToken(target))
        {
            var keys = location.Exits.Values
                .Select(e => e.Door?.RequiredKey)
                .Where(k => k != null)
                .Cast<IItem>();
            key = FuzzyMatcher.FindBestItem(keys, target, context.State.FuzzyMaxDistance) as IKey;
            if (key != null)
            {
                suggestion = key.Name;
            }
        }

        if (key != null)
        {
            var description = key.GetDescription();
            var result = CommandResult.Ok(string.IsNullOrWhiteSpace(description)
                ? Language.ItemDescription(key.Name)
                : description);
            return suggestion != null ? result.WithSuggestion(suggestion) : result;
        }

        if (target.TextCompare(location.Id) || target.TextCompare("here") || target.TextCompare("room"))
        {
            var description = location.GetDescription();
            return CommandResult.Ok(string.IsNullOrWhiteSpace(description)
                ? Language.NothingToLookAt
                : description);
        }

        return CommandResult.Fail(Language.NothingToLookAt, GameError.ItemNotFound);
    }
}
