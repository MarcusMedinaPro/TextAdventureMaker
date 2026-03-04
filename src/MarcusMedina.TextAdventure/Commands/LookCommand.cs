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

public class LookCommand(string? target = null) : ICommand
{
    public string? Target { get; } = target;

    public CommandResult Execute(CommandContext context)
    {
        var location = context.State.CurrentLocation;
        if (!string.IsNullOrWhiteSpace(Target))
            return ExecuteTarget(context, Target);

        var description = location is Location loc
            ? loc.GetDescription(context.State)
            : location.GetDescription();
        var exitsSource = location.Exits
            .Where(e => e.Value.IsVisible)
            .Where(e => !context.State.ShowDirectionsWhenThereAreDirectionsVisibleOnly ||
                        e.Value.Door is null ||
                        e.Value.Door.State != DoorState.Locked);

        var exits = exitsSource
            .Select(e => e.Value.Door is not null
                ? $"{e.Key} ({Language.EntityName(e.Value.Door)}: {e.Value.Door.State})"
                : e.Key.ToString());

        var builder = new StringBuilder();
        if (!string.IsNullOrWhiteSpace(description))
            _ = builder.Append(description.Trim());

        _ = builder.Append(builder.Length > 0 ? "\n" : string.Empty);
        _ = builder.Append(Language.HealthStatus(context.State.Stats.Health, context.State.Stats.MaxHealth));
        _ = builder.Append("\n");

        List<string> presenceDescriptions = location.Items
            .Select(i => i.PresenceDescription)
            .Where(text => !string.IsNullOrWhiteSpace(text))
            .Select(text => text!.Trim())
            .ToList();

        foreach (string presence in presenceDescriptions)
        {
            _ = builder.Append(presence);
            _ = builder.Append("\n");
        }

        List<string> items = FormatItems(location.Items.Where(i => !i.HiddenFromItemList)).ToList();
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

        // Check if target is a direction (for "look north" style commands)
        if (DirectionHelper.TryParse(target, out var direction))
            return LookInDirection(context, direction);

        if (target.TextCompare(location.Id) || target.TextCompare("here") || target.TextCompare("room"))
        {
            string roomDesc = location.GetDescription();
            return CommandResult.Ok(string.IsNullOrWhiteSpace(roomDesc) ? Language.NothingToLookAt : roomDesc);
        }

        string? suggestion = null;

        // Items (room + inventory)
        IItem? item = location.FindItem(target) ?? context.State.Inventory.FindItem(target);
        IEnumerable<IItem> itemCandidates = location.Items.Concat(context.State.Inventory.Items);
        (item, string? rawItemSuggestion) = FuzzyItemResolver.Resolve(context.State, itemCandidates, item, target);
        if (rawItemSuggestion is not null)
            suggestion = rawItemSuggestion;

        if (item is not null)
        {
            string description = item.GetDescription();
            string baseDescription = string.IsNullOrWhiteSpace(description)
                ? Language.ItemDescription(Language.EntityName(item))
                : description;

            if (item is IContainer<IItem> container)
            {
                string contentsMsg = container.Contents.Count == 0
                    ? Language.ContainerIsEmpty
                    : string.Join(", ", container.Contents
                        .Where(i => !i.HiddenFromItemList)
                        .Select(i => i.Name));
                string full = container.Contents.Count == 0
                    ? $"{baseDescription}\n{Language.ContainerIsEmpty}"
                    : $"{baseDescription}\n{Language.ContainerContents(item.Name, contentsMsg)}";
                return CommandResult.Ok(full).WithOptionalSuggestion(suggestion);
            }

            return CommandResult.Ok(baseDescription).WithOptionalSuggestion(suggestion);
        }

        // NPCs, doors, and keys via IExaminable dispatch
        IExaminable? examinable = FindExaminable(location, target);

        if (examinable is null && context.State.EnableFuzzyMatching && !FuzzyMatcher.IsLikelyCommandToken(target))
            examinable = FuzzyFindExaminable(location, target, context.State.FuzzyMaxDistance, ref suggestion);

        if (examinable is not null)
        {
            string description = examinable.GetDescription();
            string entityName = examinable is IGameEntity entity ? Language.EntityName(entity) : target;
            CommandResult result = CommandResult.Ok(string.IsNullOrWhiteSpace(description)
                ? Language.ItemDescription(entityName)
                : description);
            return result.WithOptionalSuggestion(suggestion);
        }

        return CommandResult.Fail(Language.NothingToLookAt, GameError.ItemNotFound);
    }

    private static IExaminable? FindExaminable(ILocation location, string target) =>
        location.Npcs.OfType<IExaminable>().FirstOrDefault(e => e.Matches(target))
        ?? (IExaminable?)location.Exits.Values
            .Select(e => e.Door)
            .FirstOrDefault(d => d is not null && (target.TextCompare("door") || d.Matches(target)))
        ?? location.Exits.Values
            .Select(e => e.Door?.RequiredKey)
            .OfType<IExaminable>()
            .FirstOrDefault(e => e.Matches(target));

    private static IExaminable? FuzzyFindExaminable(ILocation location, string target, int maxDistance, ref string? suggestion)
    {
        INpc? bestNpc = FuzzyMatcher.FindBestNpc(location.Npcs, target, maxDistance);
        if (bestNpc is not null)
        {
            suggestion = Language.EntityName(bestNpc);
            return bestNpc;
        }

        IEnumerable<IDoor> doors = location.Exits.Values.Select(e => e.Door).OfType<IDoor>();
        IDoor? bestDoor = FuzzyMatcher.FindBestDoor(doors, target, maxDistance);
        if (bestDoor is not null)
        {
            suggestion = Language.EntityName(bestDoor);
            return bestDoor;
        }

        IEnumerable<IItem> keys = location.Exits.Values
            .Select(e => e.Door?.RequiredKey)
            .OfType<IItem>();
        IKey? bestKey = FuzzyMatcher.FindBestItem(keys, target, maxDistance) as IKey;
        if (bestKey is not null)
        {
            suggestion = Language.EntityName(bestKey);
            return bestKey;
        }

        return null;
    }

    private static CommandResult LookInDirection(CommandContext context, Direction direction)
    {
        var location = context.State.CurrentLocation;

        if (!location.Exits.TryGetValue(direction, out var exit))
            return CommandResult.Fail($"There is no exit to the {direction.ToString().ToLowerInvariant()}.", GameError.NoExitInDirection);

        // Closed door blocks view
        if (exit.Door is not null && exit.Door.State != DoorState.Open)
        {
            var doorDesc = exit.Door.GetProperty<bool>("transparent", false) ? "frosted " : string.Empty;
            var stateText = exit.Door.State.ToString().ToLowerInvariant();
            return CommandResult.Ok($"A {doorDesc}{stateText} {exit.Door.Name.ToLowerInvariant()} blocks your view.");
        }

        // Can see into adjacent room
        var targetLocation = exit.Target;
        var glimpse = targetLocation.GetRoomGlimpse();
        return CommandResult.Ok($"Looking {direction.ToString().ToLowerInvariant()}, you see:\n{glimpse}");
    }

    private static IEnumerable<string> FormatItems(IEnumerable<IItem> items)
    {
        List<IItem> materialised = items.ToList();
        IEnumerable<IGrouping<string, IItem>> stacked = materialised
            .Where(item => item.IsStackable)
            .GroupBy(item => item.Id, StringComparer.OrdinalIgnoreCase);

        foreach (IGrouping<string, IItem> group in stacked)
        {
            IItem sample = group.First();
            int amount = group.Sum(item => item.Amount ?? 1);
            string name = Language.EntityName(sample);
            if (amount > 1 || sample.Amount.HasValue)
            {
                name = $"{name} ({amount})";
            }

            yield return Language.ItemWithWeight(name, sample.Weight);
        }

        foreach (IItem item in materialised.Where(item => !item.IsStackable))
        {
            string name = Language.EntityName(item);
            if (item.Amount.HasValue)
            {
                name = $"{name} ({item.Amount.Value})";
            }

            yield return Language.ItemWithWeight(name, item.Weight);
        }
    }
}
