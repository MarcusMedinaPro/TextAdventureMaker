// <copyright file="CommandHelper.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace MarcusMedina.TextAdventure.Helpers;

public static class CommandHelper
{
    /// <summary>
    /// Creates a case-insensitive command set from the provided tokens.
    /// </summary>
    public static HashSet<string> NewCommands(params string[] commands)
    {
        var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        if (commands == null) return set;

        foreach (var command in commands)
        {
            if (!string.IsNullOrWhiteSpace(command))
            {
                set.Add(command.Trim());
            }
        }

        return set;
    }
}
