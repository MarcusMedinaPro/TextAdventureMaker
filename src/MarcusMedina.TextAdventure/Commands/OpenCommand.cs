// <copyright file="OpenCommand.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace MarcusMedina.TextAdventure.Commands;

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;
using MarcusMedina.TextAdventure.Models;

public class OpenCommand : ICommand
{
    public CommandResult Execute(CommandContext context)
    {
        var exitWithDoor = context.State.CurrentLocation.Exits.Values
            .FirstOrDefault(e => e.Door != null);

        if (exitWithDoor?.Door == null)
        {
            return CommandResult.Fail(Language.NoDoorHere, GameError.NoDoorHere);
        }

        if (exitWithDoor.Door.State == DoorState.Open)
        {
            return CommandResult.Fail(Language.DoorAlreadyOpenMessage(exitWithDoor.Door.Name), GameError.DoorAlreadyOpen);
        }

        if (exitWithDoor.Door.Open())
        {
            context.State.Events.Publish(new GameEvent(GameEventType.OpenDoor, context.State, context.State.CurrentLocation, door: exitWithDoor.Door));
            var reaction = exitWithDoor.Door.GetReaction(DoorAction.Open);
            return reaction != null
                ? CommandResult.Ok(Language.DoorOpened(exitWithDoor.Door.Name), reaction)
                : CommandResult.Ok(Language.DoorOpened(exitWithDoor.Door.Name));
        }

        var message = exitWithDoor.Door.State == DoorState.Locked
            ? Language.DoorLocked(exitWithDoor.Door.Name)
            : Language.DoorWontBudge(exitWithDoor.Door.Name);

        var error = exitWithDoor.Door.State == DoorState.Locked
            ? GameError.DoorIsLocked
            : GameError.DoorIsClosed;
        var failReaction = exitWithDoor.Door.GetReaction(DoorAction.OpenFailed);
        return failReaction != null
            ? CommandResult.Fail(message, error, failReaction)
            : CommandResult.Fail(message, error);
    }
}
