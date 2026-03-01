// <copyright file="QuestObjective.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public sealed class QuestObjective(string id, bool isOptional) : IQuestObjective
{
    public string Id { get; } = id is not null && id.Trim().Length > 0 ? id : throw new ArgumentException("Value cannot be null or whitespace.", nameof(id));
    public bool IsOptional { get; } = isOptional;
    public bool IsCompleted { get; private set; }

    public void Complete() => IsCompleted = true;
}
