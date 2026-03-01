// <copyright file="MapCommand.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Commands;

/// <summary>
/// Displays an ASCII map of the explored world.
/// Example: "map" - shows current map view
/// </summary>
public sealed class MapCommand : ICommand
{
    /// <summary>
    /// Generates and displays the ASCII map.
    /// </summary>
    public CommandResult Execute(CommandContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var generator = new MapGenerator();
        var options = new MapOptions(
            ShowUnvisited: false,
            ShowItems: false,
            ShowNpcs: false
        );

        var map = generator.GenerateAsciiMap(context.State, options);
        return CommandResult.Ok($"\n{map}");
    }
}
