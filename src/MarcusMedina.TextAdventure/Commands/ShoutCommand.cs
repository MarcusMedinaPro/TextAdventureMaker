// <copyright file="ShoutCommand.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Commands;

/// <summary>
/// Shouts a message that can be heard in adjacent rooms.
/// Example: "shout east Help!"
/// Example: "shout" (shouts in all directions)
/// </summary>
public class ShoutCommand(Direction? direction = null, string? message = null) : ICommand
{
    public Direction? Direction { get; } = direction;
    public string? Message { get; } = message;

    public CommandResult Execute(CommandContext context)
    {
        var location = context.State.CurrentLocation;
        var heardBy = new List<string>();

        if (Direction.HasValue)
        {
            // Directional shout
            if (location.Exits.TryGetValue(Direction.Value, out var exit))
            {
                var visibleNpcs = exit.Target.Npcs
                    .Where(n => n.GetProperty<bool>("visible", true))
                    .Select(n => n.Name)
                    .ToList();

                heardBy.AddRange(visibleNpcs);
            }
        }
        else
        {
            // Shout in all directions
            foreach (var (dir, exit) in location.Exits)
            {
                var visibleNpcs = exit.Target.Npcs
                    .Where(n => n.GetProperty<bool>("visible", true))
                    .Select(n => n.Name)
                    .ToList();

                heardBy.AddRange(visibleNpcs);
            }
        }

        if (heardBy.Count > 0)
        {
            var heard = heardBy.Distinct().CommaJoin();
            return CommandResult.Ok($"Your shout echoes. {heard} heard you.");
        }

        return CommandResult.Ok("Your voice echoes, but no one seems to hear.");
    }
}
