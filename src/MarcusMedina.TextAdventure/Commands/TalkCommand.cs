// <copyright file="TalkCommand.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
using System.Text;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;
using MarcusMedina.TextAdventure.Models;

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

        var location = context.State.CurrentLocation;
        var npc = location.FindNpc(Target);
        if (npc == null)
        {
            return CommandResult.Fail(Language.NoSuchNpcHere, GameError.TargetNotFound);
        }

        context.State.Events.Publish(new GameEvent(GameEventType.TalkToNpc, context.State, location, npc: npc));

        var dialog = npc.DialogRoot;
        if (dialog == null)
        {
            return CommandResult.Ok(Language.NpcHasNothingToSay);
        }

        var builder = new StringBuilder();
        builder.Append(dialog.Text);

        if (dialog.Options.Count > 0)
        {
            builder.Append("\n");
            builder.Append(Language.DialogOptionsLabel);
            builder.Append("\n");
            for (var i = 0; i < dialog.Options.Count; i++)
            {
                builder.Append(Language.DialogOption(i + 1, dialog.Options[i].Text));
                if (i < dialog.Options.Count - 1)
                {
                    builder.Append("\n");
                }
            }
        }

        return CommandResult.Ok(builder.ToString());
    }
}
