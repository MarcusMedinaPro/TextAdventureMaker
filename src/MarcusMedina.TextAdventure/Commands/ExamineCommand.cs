// <copyright file="ExamineCommand.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;

namespace MarcusMedina.TextAdventure.Commands;

public sealed class ExamineCommand : ICommand
{
    public string? Target { get; }

    public ExamineCommand(string? target = null)
    {
        Target = target;
    }

    public CommandResult Execute(CommandContext context)
    {
        if (string.IsNullOrWhiteSpace(Target))
        {
            return CommandResult.Fail(Language.ExamineWhat, GameError.MissingArgument);
        }

        ILocation location = context.State.CurrentLocation;
        return IsRoomReference(location.Id, Target) ? new LookCommand().Execute(context) : LookCommand.ExecuteTarget(context, Target);
    }

    private static bool IsRoomReference(string locationId, string target)
    {
        if (string.IsNullOrWhiteSpace(target))
        {
            return false;
        }

        string token = target.Trim();

        return token.TextCompare(locationId)
               || token.TextCompare("room")
               || token.TextCompare("here")
               || token.TextCompare("this")
               || token.TextCompare("this room")
               || token.TextCompare("this place")
               || token.TextCompare("place");
    }
}
