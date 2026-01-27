// <copyright file="LookCommand.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
using System.Text;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;

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
        var location = context.State.CurrentLocation;
        if (!string.IsNullOrWhiteSpace(Target))
        {
            return LookAtTarget(context, Target);
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
            builder.Append(description.Trim());
        }

        builder.Append(builder.Length > 0 ? "\n" : string.Empty);
        builder.Append(Language.HealthStatus(context.State.Stats.Health, context.State.Stats.MaxHealth));
        builder.Append("\n");
        var items = location.Items
            .Where(i => !i.HiddenFromItemList)
            .Select(i => Language.ItemWithWeight(i.Name, i.Weight))
            .ToList();
        if (!context.State.ShowItemsListOnlyWhenThereAreActuallyThingsToInteractWith || items.Count > 0)
        {
            builder.Append(Language.ItemsHereLabel);
            builder.Append(items.Count > 0 ? items.CommaJoin() : Language.None);
            builder.Append("\n");
        }

        var exitsList = exits.ToList();
        if (!context.State.ShowDirectionsWhenThereAreDirectionsVisibleOnly || exitsList.Count > 0)
        {
            builder.Append(Language.ExitsLabel);
            builder.Append(exitsList.Count > 0 ? exitsList.CommaJoin() : Language.None);
        }

        return CommandResult.Ok(builder.ToString());
    }

    private static CommandResult LookAtTarget(CommandContext context, string target)
    {
        var location = context.State.CurrentLocation;
        var item = location.FindItem(target) ?? context.State.Inventory.FindItem(target);
        if (item != null)
        {
            var description = item.GetDescription();
            return CommandResult.Ok(string.IsNullOrWhiteSpace(description)
                ? Language.ItemDescription(item.Name)
                : description);
        }

        var door = location.Exits.Values
            .Select(e => e.Door)
            .FirstOrDefault(d => d != null && (target.TextCompare("door") || d.Matches(target)));

        if (door != null)
        {
            var description = door.GetDescription();
            return CommandResult.Ok(string.IsNullOrWhiteSpace(description)
                ? Language.ItemDescription(door.Name)
                : description);
        }

        var key = location.Exits.Values
            .Select(e => e.Door?.RequiredKey)
            .FirstOrDefault(k => k != null && k.Name.TextCompare(target));

        if (key != null)
        {
            var description = key.GetDescription();
            return CommandResult.Ok(string.IsNullOrWhiteSpace(description)
                ? Language.ItemDescription(key.Name)
                : description);
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
