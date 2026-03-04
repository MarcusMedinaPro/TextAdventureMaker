// <copyright file="UnlockCommand.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Commands;

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;
using MarcusMedina.TextAdventure.Models;

public class UnlockCommand(string? target) : ICommand
{
    public string? Target { get; } = target;

    public CommandResult Execute(CommandContext context)
    {
        IDoor? door = OpenCommand.ResolveDoor(context.State.CurrentLocation, Target);

        if (door is null)
            return CommandResult.Fail(Language.NoDoorHere, GameError.NoDoorHere);

        string doorName = Language.EntityName(door);

        if (door.RequiredKey is null)
            return CommandResult.Fail(Language.NoKeyRequired, GameError.NoKeyRequired);

        List<IKey> keys = context.State.Inventory.Items.OfType<IKey>().ToList();
        if (keys.Count == 0)
            return CommandResult.Fail(Language.YouNeedAKeyToOpenDoor, GameError.WrongKey);

        foreach (IKey key in keys)
        {
            if (door.Unlock(key))
            {
                context.State.Events.Publish(new GameEvent(GameEventType.UnlockDoor, context.State, context.State.CurrentLocation, door: door));
                string? reaction = door.GetReaction(DoorAction.Unlock);
                return reaction is not null
                    ? CommandResult.Ok(Language.DoorUnlocked(doorName), reaction)
                    : CommandResult.Ok(Language.DoorUnlocked(doorName));
            }
        }

        string? failReaction = door.GetReaction(DoorAction.UnlockFailed);
        return failReaction is not null
            ? CommandResult.Fail(Language.ThatKeyDoesNotFit, GameError.WrongKey, failReaction)
            : CommandResult.Fail(Language.ThatKeyDoesNotFit, GameError.WrongKey);
    }
}
