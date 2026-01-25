// <copyright file="UnlockCommand.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Commands;

public class UnlockCommand : ICommand
{
    public CommandResult Execute(CommandContext context)
    {
        var exitWithDoor = context.State.CurrentLocation.Exits.Values
            .FirstOrDefault(e => e.Door != null);

        if (exitWithDoor?.Door == null)
        {
            return CommandResult.Fail(Language.NoDoorHere, GameError.NoDoorHere);
        }

        if (exitWithDoor.Door.RequiredKey == null)
        {
            return CommandResult.Fail(Language.NoKeyRequired, GameError.NoKeyRequired);
        }

        var keys = context.State.Inventory.Items.OfType<IKey>().ToList();
        if (keys.Count == 0)
        {
            return CommandResult.Fail(Language.YouNeedAKeyToOpenDoor, GameError.WrongKey);
        }

        foreach (var key in keys)
        {
            if (exitWithDoor.Door.Unlock(key))
            {
                context.State.Events.Publish(new GameEvent(GameEventType.UnlockDoor, context.State, context.State.CurrentLocation, door: exitWithDoor.Door));
                var reaction = exitWithDoor.Door.GetReaction(DoorAction.Unlock);
                return reaction != null
                    ? CommandResult.Ok(Language.DoorUnlocked(exitWithDoor.Door.Name), reaction)
                    : CommandResult.Ok(Language.DoorUnlocked(exitWithDoor.Door.Name));
            }
        }

        var failReaction = exitWithDoor.Door.GetReaction(DoorAction.UnlockFailed);
        return failReaction != null
            ? CommandResult.Fail(Language.ThatKeyDoesNotFit, GameError.WrongKey, failReaction)
            : CommandResult.Fail(Language.ThatKeyDoesNotFit, GameError.WrongKey);
    }
}
