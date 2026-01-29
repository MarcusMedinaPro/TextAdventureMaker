// <copyright file="TalkCommand.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Helpers;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;
using MarcusMedina.TextAdventure.Models;
using System.Text;

namespace MarcusMedina.TextAdventure.Commands;

public class TalkCommand : ICommand
{
    public string? Target { get; }

    public TalkCommand(string? target)
    {
        Target = target;
    }

    public CommandResult Execute(CommandContext context)
    {
        if (string.IsNullOrWhiteSpace(Target))
        {
            return CommandResult.Fail(Language.NoOneToTalkTo, GameError.MissingArgument);
        }

        ILocation location = context.State.CurrentLocation;
        INpc? npc = location.FindNpc(Target);
        string? suggestion = null;
        if (npc == null && context.State.EnableFuzzyMatching && !FuzzyMatcher.IsLikelyCommandToken(Target))
        {
            INpc? best = FuzzyMatcher.FindBestNpc(location.Npcs, Target, context.State.FuzzyMaxDistance);
            if (best != null)
            {
                npc = best;
                suggestion = best.Name;
            }
        }

        if (npc == null)
        {
            return CommandResult.Fail(Language.NoSuchNpcHere, GameError.TargetNotFound);
        }

        context.State.Events.Publish(new GameEvent(GameEventType.TalkToNpc, context.State, location, npc: npc));

        string? ruleBased = npc.GetRuleBasedDialog(context.State);
        if (!string.IsNullOrWhiteSpace(ruleBased))
        {
            CommandResult ruleResult = CommandResult.Ok(ruleBased);
            return suggestion != null ? ruleResult.WithSuggestion(suggestion) : ruleResult;
        }

        IDialogNode? dialog = npc.DialogRoot;
        if (dialog == null)
        {
            npc.Memory.MarkMet();
            return CommandResult.Ok(Language.NpcHasNothingToSay);
        }

        StringBuilder builder = new();
        _ = builder.Append(dialog.Text);

        if (dialog.Options.Count > 0)
        {
            _ = builder.Append("\n");
            _ = builder.Append(Language.DialogOptionsLabel);
            _ = builder.Append("\n");
            for (int i = 0; i < dialog.Options.Count; i++)
            {
                _ = builder.Append(Language.DialogOption(i + 1, dialog.Options[i].Text));
                if (i < dialog.Options.Count - 1)
                {
                    _ = builder.Append("\n");
                }
            }
        }

        npc.Memory.MarkMet();
        CommandResult result = CommandResult.Ok(builder.ToString());
        return suggestion != null ? result.WithSuggestion(suggestion) : result;
    }
}
