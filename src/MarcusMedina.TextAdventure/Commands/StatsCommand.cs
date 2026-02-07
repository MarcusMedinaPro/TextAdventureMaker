// <copyright file="StatsCommand.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;

namespace MarcusMedina.TextAdventure.Commands;

public class StatsCommand : ICommand
{
    public CommandResult Execute(CommandContext context)
    {
        IStats stats = context.State.Stats;
        return CommandResult.Ok(Language.HealthStatus(stats.Health, stats.MaxHealth));
    }
}
