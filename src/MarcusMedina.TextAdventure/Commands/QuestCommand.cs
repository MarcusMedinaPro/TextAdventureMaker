// <copyright file="QuestCommand.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;
using System.Text;

namespace MarcusMedina.TextAdventure.Commands;
/// <summary>List active and completed quests.</summary>
public sealed class QuestCommand : ICommand
{
    public CommandResult Execute(CommandContext context)
    {
        IQuestLog log = context.State.Quests;
        if (log.Quests.Count == 0)
        {
            return CommandResult.Ok(Language.NoQuests);
        }

        IReadOnlyList<IQuest> active = log.GetByState(QuestState.Active);
        IReadOnlyList<IQuest> completed = log.GetByState(QuestState.Completed);

        StringBuilder builder = new();
        _ = builder.Append(Language.QuestsLabel);

        if (active.Count > 0)
        {
            _ = builder.Append("\n");
            _ = builder.Append(Language.ActiveQuestsLabel);
            foreach (IQuest quest in active)
            {
                _ = builder.Append("\n");
                _ = builder.Append(Language.QuestEntry(quest.Title));
            }
        }

        if (completed.Count > 0)
        {
            _ = builder.Append("\n");
            _ = builder.Append(Language.CompletedQuestsLabel);
            foreach (IQuest quest in completed)
            {
                _ = builder.Append("\n");
                _ = builder.Append(Language.QuestEntry(quest.Title));
            }
        }

        return CommandResult.Ok(builder.ToString());
    }
}
