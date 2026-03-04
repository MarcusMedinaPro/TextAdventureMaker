// <copyright file="NpcReactionResolver.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Engine;

/// <summary>
/// After any command executes, checks NPCs in the current room for matching reactions,
/// appends their text to the result, applies any side-effects, and sets ShouldQuit when
/// a reaction has <c>EndGame = true</c>.
/// </summary>
public static class NpcReactionResolver
{
    public static CommandResult Resolve(ICommand command, CommandResult result, GameState state)
    {
        if (result.ShouldQuit)
            return result;

        string[] triggers = BuildTriggers(command, result);
        if (triggers.Length == 0)
            return result;

        var firedReactions = new List<NpcReaction>();
        foreach (INpc npc in state.CurrentLocation.Npcs)
        {
            foreach (string trigger in triggers)
            {
                NpcReaction? reaction = npc.GetReaction(trigger, state);
                if (reaction is not null)
                {
                    firedReactions.Add(reaction);
                    break; // one reaction per NPC per command
                }
            }
        }

        if (firedReactions.Count == 0)
        {
            // If a custom action found no reactions, return an error
            if (command is CustomActionCommand cac)
                return CommandResult.Fail($"You try to {cac.Verb}{(string.IsNullOrWhiteSpace(cac.Target) ? "" : $" {cac.Target}")}, but nothing happens.", Enums.GameError.UnknownCommand);
            return result;
        }

        // Apply side-effects
        foreach (NpcReaction reaction in firedReactions)
            reaction.Effect?.Invoke(state);

        bool endGame = firedReactions.Any(r => r.EndGame);
        var combined = result.ReactionsList
            .Concat(firedReactions.Select(r => r.Text))
            .ToArray();

        return new CommandResult(result.Success, result.Message, result.Error, endGame || result.ShouldQuit, combined);
    }

    internal static string[] BuildTriggers(ICommand command, CommandResult result)
    {
        // TalkCommand only fires reactions when it produced no message of its own
        if (command is TalkCommand && !string.IsNullOrWhiteSpace(result.Message))
            return [];

        if (command is IReactableCommand r)
            return r.GetNpcTriggers();

        return [];
    }
}
