// <copyright file="LockCommand.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Commands;

public class LockCommand(string? target) : ICommand
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

        if (door.State == DoorState.Locked)
            return CommandResult.Fail(Language.DoorAlreadyLockedMessage(doorName), GameError.DoorAlreadyLocked);

        if (door.State == DoorState.Open)
            return CommandResult.Fail(Language.DoorMustBeClosed(doorName), GameError.DoorIsClosed);

        List<IKey> keys = context.State.Inventory.Items.OfType<IKey>().ToList();
        if (keys.Count == 0)
            return CommandResult.Fail(Language.YouNeedAKeyToLockDoor, GameError.WrongKey);

        foreach (IKey key in keys)
        {
            if (door.Lock(key))
            {
                context.State.Events.Publish(new GameEvent(GameEventType.LockDoor, context.State, context.State.CurrentLocation, door: door));
                string? reaction = door.GetReaction(DoorAction.Lock);
                return reaction is not null
                    ? CommandResult.Ok(Language.DoorLockedByPlayer(doorName), reaction)
                    : CommandResult.Ok(Language.DoorLockedByPlayer(doorName));
            }
        }

        return CommandResult.Fail(Language.ThatKeyDoesNotFit, GameError.WrongKey);
    }
}
