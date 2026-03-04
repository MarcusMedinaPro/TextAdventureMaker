// <copyright file="TalkCommand.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Commands;

using System.Text;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Helpers;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;
using MarcusMedina.TextAdventure.Models;

public class TalkCommand(string? target) : ICommand, IReactableCommand
{
    public string? Target { get; } = target;

    public string[] GetNpcTriggers() => ["talk"];

    public CommandResult Execute(CommandContext context)
    {
        if (string.IsNullOrWhiteSpace(Target))
        {
            return CommandResult.Fail(Language.NoOneToTalkTo, GameError.MissingArgument);
        }

        var location = context.State.CurrentLocation;
        var npc = location.FindNpc(Target);
        string? suggestion = null;
        if (npc  is null && context.State.EnableFuzzyMatching && !FuzzyMatcher.IsLikelyCommandToken(Target))
        {
            var best = FuzzyMatcher.FindBestNpc(location.Npcs, Target, context.State.FuzzyMaxDistance);
            if (best  is not null)
            {
                npc = best;
                suggestion = best.Name;
            }
        }

        if (npc is null)
        {
            // Check if a non-NPC entity with this name exists (item or door) → wrong type
            IItem? matchedItem = location.FindItem(Target);
            if (matchedItem is null)
            {
                IDoor? matchedDoor = location.Exits.Values
                    .Select(e => e.Door)
                    .FirstOrDefault(d => d is not null && d.Matches(Target));
                if (matchedDoor is not null)
                    return CommandResult.Fail(Language.CannotTalkToThat, GameError.WrongObjectType);
            }
            else
            {
                return CommandResult.Fail(Language.CannotTalkToThat, GameError.WrongObjectType);
            }

            return CommandResult.Fail(Language.NoSuchNpcHere, GameError.TargetNotFound);
        }

        context.State.Events.Publish(new GameEvent(GameEventType.TalkToNpc, context.State, location, npc: npc));

        var ruleBased = npc.GetRuleBasedDialog(context.State);
        if (!string.IsNullOrWhiteSpace(ruleBased))
        {
            return CommandResult.Ok(ruleBased).WithOptionalSuggestion(suggestion);
        }

        var dialog = npc.DialogRoot;
        if (dialog  is null)
        {
            npc.Memory.MarkMet();
            return CommandResult.Ok(Language.NpcHasNothingToSay);
        }

        StringBuilder builder = new();
        _ = builder.Append(dialog.Text);

        if (dialog.Options.Count > 0)
        {
            _ = builder.Append('\n');
            _ = builder.Append(Language.DialogOptionsLabel);
            _ = builder.Append('\n');
            for (var i = 0; i < dialog.Options.Count; i++)
            {
                _ = builder.Append(Language.DialogOption(i + 1, dialog.Options[i].Text));
                if (i < dialog.Options.Count - 1)
                {
                    _ = builder.Append('\n');
                }
            }
        }

        npc.Memory.MarkMet();
        return CommandResult.Ok(builder.ToString()).WithOptionalSuggestion(suggestion);
    }
}
