// <copyright file="FleeCommand.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;

namespace MarcusMedina.TextAdventure.Commands;

public class FleeCommand : ICommand
{
    public string? Target { get; }

    public FleeCommand(string? target = null)
    {
        Target = target;
    }

    public CommandResult Execute(CommandContext context)
    {
        var location = context.State.CurrentLocation;
        var npc = !string.IsNullOrWhiteSpace(Target)
            ? location.FindNpc(Target)
            : location.Npcs.FirstOrDefault();

        if (npc == null)
        {
            return CommandResult.Fail(Language.NoOneToFlee, GameError.TargetNotFound);
        }

        return context.State.CombatSystem.Flee(context.State, npc);
    }
}
