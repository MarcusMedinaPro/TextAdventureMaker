// <copyright file="ICommand.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
using MarcusMedina.TextAdventure.Commands;

namespace MarcusMedina.TextAdventure.Interfaces;

public interface ICommand
{
    CommandResult Execute(CommandContext context);
}
