// <copyright file="OpenCommand.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Commands;

public class OpenCommand(string? target) : DoorCommandBase(target)
{
    public override CommandResult Execute(CommandContext context)
    {
        ILocation location = context.State.CurrentLocation;

        IDoor? door = ResolveDoor(location, Target);
        if (door is not null)
            return OpenDoor(context, door);

        if (!string.IsNullOrWhiteSpace(Target))
        {
            IItem? item = location.FindItem(Target) ?? context.State.Inventory.FindItem(Target);
            if (item is IContainer<IItem> container)
                return OpenContainer(item, container);
            if (item is not null)
                return CommandResult.Fail(Language.NothingToOpen, GameError.ItemNotUsable);
            return CommandResult.Fail(Language.NoSuchItemHere, GameError.ItemNotFound);
        }

        foreach (IItem roomItem in location.Items)
        {
            if (roomItem is IContainer<IItem> container)
                return OpenContainer(roomItem, container);
        }

        return CommandResult.Fail(Language.NothingToOpen, GameError.NoDoorHere);
    }

    private static CommandResult OpenDoor(CommandContext context, IDoor door)
    {
        string doorName = Language.EntityName(door);

        if (door.State == DoorState.Open)
            return CommandResult.Fail(Language.DoorAlreadyOpenMessage(doorName), GameError.DoorAlreadyOpen);

        if (door.Open())
        {
            context.State.Events.Publish(new GameEvent(GameEventType.OpenDoor, context.State, context.State.CurrentLocation, door: door));
            string? reaction = door.GetReaction(DoorAction.Open);
            return reaction is not null
                ? CommandResult.Ok(Language.DoorOpened(doorName), reaction)
                : CommandResult.Ok(Language.DoorOpened(doorName));
        }

        string message = door.State == DoorState.Locked
            ? Language.DoorLocked(doorName)
            : Language.DoorWontBudge(doorName);
        GameError error = door.State == DoorState.Locked
            ? GameError.DoorIsLocked
            : GameError.DoorIsClosed;
        string? failReaction = door.GetReaction(DoorAction.OpenFailed);
        return failReaction is not null
            ? CommandResult.Fail(message, error, failReaction)
            : CommandResult.Fail(message, error);
    }

    private static CommandResult OpenContainer(IItem item, IContainer<IItem> container)
    {
        string? reaction = item.GetReaction(ItemAction.Open);

        if (container.Contents.Count == 0)
        {
            string msg = Language.ContainerContents(item.Name, Language.ContainerIsEmpty);
            return reaction is not null
                ? CommandResult.Ok(msg, reaction)
                : CommandResult.Ok(msg);
        }

        string contentsList = string.Join(", ", container.Contents.Select(i => i.Name));
        string openMsg = Language.ContainerContents(item.Name, contentsList);
        return reaction is not null
            ? CommandResult.Ok(openMsg, reaction)
            : CommandResult.Ok(openMsg);
    }
}
