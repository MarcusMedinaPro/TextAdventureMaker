// <copyright file="CloseCommand.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Commands;

public class CloseCommand : ICommand
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
        if (exitWithDoor.Door.State == DoorState.Destroyed)
        {
            return CommandResult.Fail(Language.DoorAlreadyDestroyedMessage(doorName), GameError.DoorIsDestroyed);
        }

        if (exitWithDoor.Door.State == DoorState.Closed)
        {
            return CommandResult.Fail(Language.DoorAlreadyClosedMessage(doorName), GameError.DoorIsClosed);
        }

        if (exitWithDoor.Door.State == DoorState.Locked)
        {
            return CommandResult.Fail(Language.DoorLocked(doorName), GameError.DoorIsLocked);
        }

        if (exitWithDoor.Door.Close())
        {
            context.State.Events.Publish(new GameEvent(GameEventType.CloseDoor, context.State, context.State.CurrentLocation, door: exitWithDoor.Door));
            string? reaction = exitWithDoor.Door.GetReaction(DoorAction.Close);
            return reaction != null
                ? CommandResult.Ok(Language.DoorClosedByPlayer(doorName), reaction)
                : CommandResult.Ok(Language.DoorClosedByPlayer(doorName));
        }

        return CommandResult.Fail(Language.DoorWontBudge(doorName), GameError.DoorIsClosed);
    }
}
