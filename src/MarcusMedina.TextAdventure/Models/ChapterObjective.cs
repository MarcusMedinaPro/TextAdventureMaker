// <copyright file="ChapterObjective.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public sealed class ChapterObjective(string id, bool required, string? branchTo = null) : IChapterObjective
{
    public string Id { get; } = id ?? "";
    public bool IsRequired { get; } = required;
    public string? BranchTo { get; } = branchTo;
    public bool IsComplete { get; private set; }

    public ChapterObjective Complete()
    {
        IsComplete = true;
        return this;
    }
}
