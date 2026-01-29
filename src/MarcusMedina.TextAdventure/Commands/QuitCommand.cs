// <copyright file="QuitCommand.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace MarcusMedina.TextAdventure.Commands;

using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;

public class QuitCommand : ICommand
{
    public CommandResult Execute(CommandContext context) => CommandResult.Quit(Language.ThanksForPlaying);
}
