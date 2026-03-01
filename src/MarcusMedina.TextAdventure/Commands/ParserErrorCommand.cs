// <copyright file="ParserErrorCommand.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Commands;

public sealed class ParserErrorCommand(string message) : ICommand
{
    private readonly string _message = message ?? "I am not sure what you mean.";

    public CommandResult Execute(CommandContext context)
    {
        return CommandResult.Fail(_message, GameError.InvalidArgument);
    }
}
