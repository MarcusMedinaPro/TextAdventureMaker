// <copyright file="DestroyCommand.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Commands;

public class DestroyCommand(string? target) : ICommand
{
    public string? Target { get; } = target;

    public CommandResult Execute(CommandContext context)
    {
        ILocation location = context.State.CurrentLocation;
        Exit? exitWithDoor = location.Exits.Values.FirstOrDefault(e => e.Door  is not null);
        IDoor? door = exitWithDoor?.Door;

        if (string.IsNullOrWhiteSpace(Target))
        {
            if (door  is null)
            {
                return CommandResult.Fail(Language.NothingToDestroy, GameError.MissingArgument);
            }

            return DestroyDoor(context, door);
        }

        string token = Target.Trim();
        if (door  is not null && door.Matches(token))
        {
            return DestroyDoor(context, door);
        }

        IItem? item = location.FindItem(token);
        if (item  is null)
        {
            return CommandResult.Fail(Language.NoSuchItemHere, GameError.ItemNotFound);
        }

        item.Destroy();
        string displayName = Language.EntityName(item);
        string? reaction = item.GetReaction(ItemAction.Destroy);
        return reaction  is not null
            ? CommandResult.Ok(Language.DestroyedItem(displayName), reaction)
            : CommandResult.Ok(Language.DestroyedItem(displayName));
    }

    private static CommandResult DestroyDoor(CommandContext context, IDoor door)
    {
        string doorName = Language.EntityName(door);
        if (door.State == DoorState.Destroyed)
        {
            return CommandResult.Fail(Language.DoorAlreadyDestroyedMessage(doorName), GameError.DoorIsDestroyed);
        }

        door.Destroy();
        context.State.Events.Publish(new GameEvent(GameEventType.DestroyDoor, context.State, context.State.CurrentLocation, door: door));
        string? reaction = door.GetReaction(DoorAction.Destroy);
        return reaction  is not null
            ? CommandResult.Ok(Language.DoorDestroyed(doorName), reaction)
            : CommandResult.Ok(Language.DoorDestroyed(doorName));
    }
}
