// <copyright file="FleeCommand.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace MarcusMedina.TextAdventure.Commands;

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Helpers;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;

public class FleeCommand : ICommand
{
    public string? Target { get; }

    public FleeCommand(string? target = null) => Target = target;

    public CommandResult Execute(CommandContext context)
    {
        var location = context.State.CurrentLocation;
        var npc = !string.IsNullOrWhiteSpace(Target)
            ? location.FindNpc(Target)
            : location.Npcs.FirstOrDefault();

        string? suggestion = null;
        if (npc == null && !string.IsNullOrWhiteSpace(Target) &&
            context.State.EnableFuzzyMatching && !FuzzyMatcher.IsLikelyCommandToken(Target))
        {
            var best = FuzzyMatcher.FindBestNpc(location.Npcs, Target, context.State.FuzzyMaxDistance);
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

        var result = context.State.CombatSystem.Flee(context.State, npc);
        return suggestion != null ? result.WithSuggestion(suggestion) : result;
    }
}
