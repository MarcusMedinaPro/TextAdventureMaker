// <copyright file="NpcIdleResolver.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Engine;

/// <summary>
/// After every command, ticks idle behaviours for NPCs in the current room.
/// An NPC's idle is skipped if it would react to the triggering command (no double-output).
/// </summary>
public static class NpcIdleResolver
{
    public static CommandResult Resolve(ICommand command, CommandResult result, GameState state)
    {
        if (result.ShouldQuit)
            return result;

        string[] triggers = NpcReactionResolver.BuildTriggers(command, result);

        var idleMessages = new List<string>();
        foreach (INpc npc in state.CurrentLocation.Npcs)
        {
            if (!npc.IsAlive || npc.IdleBehaviors.Count == 0)
                continue;

            // Skip if this NPC would react to the current command
            if (triggers.Any(t => npc.GetReaction(t, state) is not null))
                continue;

            foreach (var behavior in npc.IdleBehaviors)
            {
                string? msg = behavior.Tick();
                if (msg is not null)
                {
                    idleMessages.Add($"{npc.Name}: {msg}");
                    break; // one idle message per NPC per command
                }
            }
        }

        if (idleMessages.Count == 0)
            return result;

        var combined = result.ReactionsList.Concat(idleMessages).ToArray();
        return new CommandResult(result.Success, result.Message, result.Error, result.ShouldQuit, combined);
    }
}
