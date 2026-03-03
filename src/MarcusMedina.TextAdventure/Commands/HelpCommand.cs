// <copyright file="HelpCommand.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Commands;

public sealed class HelpCommand(string helpText) : ICommand
{
    private readonly string _helpText = string.IsNullOrWhiteSpace(helpText)
        ? "No help available."
        : helpText;

    public CommandResult Execute(CommandContext context) => CommandResult.Ok(_helpText);
}

