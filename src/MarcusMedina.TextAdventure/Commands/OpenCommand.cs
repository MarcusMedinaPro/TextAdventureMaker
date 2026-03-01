// <copyright file="OpenCommand.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Commands;

public class OpenCommand : ICommand
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
        if (exitWithDoor.Door.State == DoorState.Open)
        {
            return CommandResult.Fail(Language.DoorAlreadyOpenMessage(doorName), GameError.DoorAlreadyOpen);
        }

        if (exitWithDoor.Door.Open())
        {
            context.State.Events.Publish(new GameEvent(GameEventType.OpenDoor, context.State, context.State.CurrentLocation, door: exitWithDoor.Door));
            string? reaction = exitWithDoor.Door.GetReaction(DoorAction.Open);
            return reaction != null
                ? CommandResult.Ok(Language.DoorOpened(doorName), reaction)
                : CommandResult.Ok(Language.DoorOpened(doorName));
        }

        string message = exitWithDoor.Door.State == DoorState.Locked
            ? Language.DoorLocked(doorName)
            : Language.DoorWontBudge(doorName);

        GameError error = exitWithDoor.Door.State == DoorState.Locked
            ? GameError.DoorIsLocked
            : GameError.DoorIsClosed;
        string? failReaction = exitWithDoor.Door.GetReaction(DoorAction.OpenFailed);
        return failReaction != null
            ? CommandResult.Fail(message, error, failReaction)
            : CommandResult.Fail(message, error);
    }
}
