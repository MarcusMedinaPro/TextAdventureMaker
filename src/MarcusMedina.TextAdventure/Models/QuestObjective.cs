// <copyright file="QuestObjective.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public sealed class QuestObjective : IQuestObjective
{
    public string Id { get; }
    public bool IsOptional { get; }
    public bool IsCompleted { get; private set; }

    public QuestObjective(string id, bool isOptional)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        Id = id;
        IsOptional = isOptional;
    }

    public void Complete()
    {
        IsCompleted = true;
    }
}
