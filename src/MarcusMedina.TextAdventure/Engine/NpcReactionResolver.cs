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
            return result;

        // Apply side-effects
        foreach (NpcReaction reaction in firedReactions)
            reaction.Effect?.Invoke(state);

        bool endGame = firedReactions.Any(r => r.EndGame);
        var combined = result.ReactionsList
            .Concat(firedReactions.Select(r => r.Text))
            .ToArray();

        return new CommandResult(result.Success, result.Message, result.Error, endGame || result.ShouldQuit, combined);
    }

    private static string[] BuildTriggers(ICommand command, CommandResult result) => command switch
    {
        CustomActionCommand ca when !string.IsNullOrWhiteSpace(ca.Target) =>
            [$"{ca.Verb.ToLowerInvariant()}:{ca.Target.ToLowerInvariant()}", ca.Verb.ToLowerInvariant()],

        CustomActionCommand ca =>
            [ca.Verb.ToLowerInvariant()],

        UseCommand use when !string.IsNullOrWhiteSpace(use.ItemName) =>
            [$"use:{use.ItemName.ToLowerInvariant()}", "use"],

        AttackCommand =>
            ["attack"],

        TakeCommand take when !string.IsNullOrWhiteSpace(take.ItemName) =>
            [$"take:{take.ItemName.ToLowerInvariant()}", "take"],

        TalkCommand when string.IsNullOrWhiteSpace(result.Message) =>
            ["talk"],

        _ => []
    };
}
