// <copyright file="ListenCommand.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Commands;

/// <summary>
/// Listens for sounds coming from adjacent rooms.
/// Example: "listen south" (listens to the south)
/// Example: "listen" (listens to all directions)
/// </summary>
public class ListenCommand(Direction? direction = null) : ICommand
{
    public Direction? Direction { get; } = direction;

    public CommandResult Execute(CommandContext context)
    {
        var location = context.State.CurrentLocation;
        var sounds = new List<string>();

        IEnumerable<KeyValuePair<Direction, Exit>> exits = Direction.HasValue
            ? location.Exits.Where(e => e.Key == Direction.Value)
            : location.Exits;

        foreach (var (dir, exit) in exits)
        {
            // NPCs that are talking
            var talkingNpcs = exit.Target.Npcs
                .Where(n => n.GetProperty<bool>("talking", false))
                .Select(n => n.Name)
                .ToList();

            foreach (var npcName in talkingNpcs)
                sounds.Add($"You hear voices from the {dir.ToString().ToLowerInvariant()}.");

            // Ambient sounds
            var ambient = exit.Target.GetProperty<string>("ambient_sound", string.Empty);
            if (!string.IsNullOrEmpty(ambient))
                sounds.Add($"From the {dir.ToString().ToLowerInvariant()}: {ambient}");
        }

        if (sounds.Count > 0)
            return CommandResult.Ok(string.Join("\n", sounds));

        return CommandResult.Ok("You strain your ears but hear nothing unusual.");
    }
}
