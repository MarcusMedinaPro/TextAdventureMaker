// <copyright file="GoCommand.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace MarcusMedina.TextAdventure.Commands;

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;

public class GoCommand : ICommand
{
    public Direction Direction { get; }

    public GoCommand(Direction direction) => Direction = direction;

    public CommandResult Execute(CommandContext context)
    {
        if (context.State.Move(Direction))
        {
            return CommandResult.Ok(Language.GoDirection(Direction.ToString().Lower()));
        }

        var error = context.State.LastMoveErrorCode != GameError.None
            ? context.State.LastMoveErrorCode
            : GameError.NoExitInDirection;
        return CommandResult.Fail(context.State.LastMoveError ?? Language.CantGoThatWay, error);
    }
}
