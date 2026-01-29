// <copyright file="FleeCommand.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Helpers;
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
        ILocation location = context.State.CurrentLocation;
        INpc? npc = !string.IsNullOrWhiteSpace(Target)
            ? location.FindNpc(Target)
            : location.Npcs.FirstOrDefault();

        string? suggestion = null;
        if (npc == null && !string.IsNullOrWhiteSpace(Target) &&
            context.State.EnableFuzzyMatching && !FuzzyMatcher.IsLikelyCommandToken(Target))
        {
            INpc? best = FuzzyMatcher.FindBestNpc(location.Npcs, Target, context.State.FuzzyMaxDistance);
            if (best != null)
            {
                npc = best;
                suggestion = best.Name;
            }
        }

        if (npc == null)
        {
            return CommandResult.Fail(Language.NoOneToFlee, GameError.TargetNotFound);
        }

        CommandResult result = context.State.CombatSystem.Flee(context.State, npc);
        return suggestion != null ? result.WithSuggestion(suggestion) : result;
    }
}
