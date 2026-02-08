// <copyright file="LockCommand.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Commands;

public class LockCommand : ICommand
{
    public CommandResult Execute(CommandContext context)
    {
        Exit? exitWithDoor = context.State.CurrentLocation.Exits.Values
            .FirstOrDefault(e => e.Door != null);

        if (exitWithDoor?.Door == null)
        {
            return CommandResult.Fail(Language.NoDoorHere, GameError.NoDoorHere);
        }

        string doorName = Language.EntityName(exitWithDoor.Door);
        if (exitWithDoor.Door.RequiredKey == null)
        {
            return CommandResult.Fail(Language.NoKeyRequired, GameError.NoKeyRequired);
        }

        if (exitWithDoor.Door.State == DoorState.Locked)
        {
            return CommandResult.Fail(Language.DoorAlreadyLockedMessage(doorName), GameError.DoorAlreadyLocked);
        }

        if (exitWithDoor.Door.State == DoorState.Open)
        {
            return CommandResult.Fail(Language.DoorMustBeClosed(doorName), GameError.DoorIsClosed);
        }

        List<IKey> keys = context.State.Inventory.Items.OfType<IKey>().ToList();
        if (keys.Count == 0)
        {
            return CommandResult.Fail(Language.YouNeedAKeyToLockDoor, GameError.WrongKey);
        }

        foreach (IKey? key in keys)
        {
            if (exitWithDoor.Door.Lock(key))
            {
                context.State.Events.Publish(new GameEvent(GameEventType.LockDoor, context.State, context.State.CurrentLocation, door: exitWithDoor.Door));
                string? reaction = exitWithDoor.Door.GetReaction(DoorAction.Lock);
                return reaction != null
                    ? CommandResult.Ok(Language.DoorLockedByPlayer(doorName), reaction)
                    : CommandResult.Ok(Language.DoorLockedByPlayer(doorName));
            }
        }

        return CommandResult.Fail(Language.ThatKeyDoesNotFit, GameError.WrongKey);
    }
}
