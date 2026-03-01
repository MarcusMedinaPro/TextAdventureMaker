// <copyright file="ThrowCommand.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Commands;

/// <summary>
/// Throws an item in a specified direction into an adjacent room.
/// Example: "throw wallet north"
/// </summary>
public class ThrowCommand(string itemName, Direction direction) : ICommand
{
    public string ItemName { get; } = itemName;
    public Direction Direction { get; } = direction;

    public CommandResult Execute(CommandContext context)
    {
        var item = context.State.Inventory.FindItem(ItemName);
        if (item is null)
            return CommandResult.Fail($"You don't have a {ItemName}.", GameError.ItemNotInInventory);

        if (!item.GetProperty<bool>("throwable", true))
            return CommandResult.Fail($"You can't throw the {item.Name}.", GameError.InvalidArgument);

        var location = context.State.CurrentLocation;
        if (!location.Exits.TryGetValue(Direction, out var exit))
            return CommandResult.Fail($"There's nowhere to throw to the {Direction.ToString().ToLowerInvariant()}.", GameError.NoExitInDirection);

        // Closed door blocks the throw
        if (exit.Door is not null && exit.Door.State == DoorState.Closed)
            return CommandResult.Ok($"The {item.Name.ToLowerInvariant()} bounces off the closed door.");

        // Move item to target location
        context.State.Inventory.Remove(item);
        exit.Target.AddItem(item);

        return CommandResult.Ok($"You throw the {item.Name.ToLowerInvariant()} {Direction.ToString().ToLowerInvariant()}. It lands in the room beyond.");
    }
}
