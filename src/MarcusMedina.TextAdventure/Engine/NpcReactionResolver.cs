// <copyright file="NpcReactionResolver.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Engine;

/// <summary>
/// After any command executes, checks NPCs in the current room for matching reactions
/// and appends their text to the command result.
/// </summary>
public static class NpcReactionResolver
{
    /// <summary>
    /// Resolves NPC reactions for the executed command and returns an updated result.
    /// </summary>
    public static CommandResult Resolve(ICommand command, CommandResult result, GameState state)
    {
        if (result.ShouldQuit)
            return result;

        string[] triggers = BuildTriggers(command, result);
        if (triggers.Length == 0)
            return result;

        var npcReactions = new List<string>();
        foreach (INpc npc in state.CurrentLocation.Npcs)
        {
            string? text = null;
            foreach (string trigger in triggers)
            {
                text = npc.GetReaction(trigger, state);
                if (text is not null)
                    break;
            }
            if (text is not null)
                npcReactions.Add(text);
        }

        if (npcReactions.Count == 0)
            return result;

        var combined = result.ReactionsList.Concat(npcReactions).ToArray();
        return new CommandResult(result.Success, result.Message, result.Error, result.ShouldQuit, combined);
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
