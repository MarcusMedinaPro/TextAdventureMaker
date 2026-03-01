// <copyright file="AiDescriptionCacheInvalidationPolicy.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.AI.Plugin;

internal static class AiDescriptionCacheInvalidationPolicy
{
    public static bool ShouldClear(ICommand command, CommandResult result)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentNullException.ThrowIfNull(result);

        if (!result.Success || result.ShouldQuit)
            return false;

        return command switch
        {
            LookCommand => false,
            AiLookCommand => false,
            InventoryCommand => false,
            StatsCommand => false,
            HintCommand => false,
            AccessibilityCommand => false,
            QuestCommand => false,
            FuzzyCommand => false,
            _ => true
        };
    }
}
