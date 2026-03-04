// <copyright file="CloseCommand.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Commands;

public class CloseCommand(string? target) : ICommand
{
    public string? Target { get; } = target;

    public CommandResult Execute(CommandContext context)
    {
        ILocation location = context.State.CurrentLocation;

        IDoor? door = OpenCommand.ResolveDoor(location, Target);
        if (door is not null)
            return CloseDoor(context, door);

        if (!string.IsNullOrWhiteSpace(Target))
        {
            IItem? item = location.FindItem(Target) ?? context.State.Inventory.FindItem(Target);
            if (item is IContainer<IItem> container)
                return CloseContainer(item, container);
            if (item is not null)
                return CommandResult.Fail(Language.NothingToClose, GameError.ItemNotUsable);
            return CommandResult.Fail(Language.NoSuchItemHere, GameError.ItemNotFound);
        }

        foreach (IItem roomItem in location.Items)
        {
            if (roomItem is IContainer<IItem>)
                return CommandResult.Fail(Language.ContainerAlreadyClosed, GameError.DoorIsClosed);
        }

        return CommandResult.Fail(Language.NothingToClose, GameError.NoDoorHere);
    }

    private static CommandResult CloseDoor(CommandContext context, IDoor door)
    {
        string doorName = Language.EntityName(door);

        if (door.State == DoorState.Destroyed)
            return CommandResult.Fail(Language.DoorAlreadyDestroyedMessage(doorName), GameError.DoorIsDestroyed);

        if (door.State == DoorState.Closed)
            return CommandResult.Fail(Language.DoorAlreadyClosedMessage(doorName), GameError.DoorIsClosed);

        if (door.State == DoorState.Locked)
            return CommandResult.Fail(Language.DoorLocked(doorName), GameError.DoorIsLocked);

        if (door.Close())
        {
            context.State.Events.Publish(new GameEvent(GameEventType.CloseDoor, context.State, context.State.CurrentLocation, door: door));
            string? reaction = door.GetReaction(DoorAction.Close);
            return reaction is not null
                ? CommandResult.Ok(Language.DoorClosedByPlayer(doorName), reaction)
                : CommandResult.Ok(Language.DoorClosedByPlayer(doorName));
        }

        return CommandResult.Fail(Language.DoorWontBudge(doorName), GameError.DoorIsClosed);
    }

    private static CommandResult CloseContainer(IItem item, IContainer<IItem> container)
    {
        string? reaction = item.GetReaction(ItemAction.Close);
        string msg = Language.ContainerClosed(item.Name);
        return reaction is not null
            ? CommandResult.Ok(msg, reaction)
            : CommandResult.Ok(msg);
    }
}
