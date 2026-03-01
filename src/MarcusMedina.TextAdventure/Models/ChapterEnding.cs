// <copyright file="ChapterEnding.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public sealed class ChapterEnding(string id, Func<IGameState, bool>? condition = null, bool isDefault = false)
{
    public string Id { get; } = id ?? "";
    public bool IsDefault { get; } = isDefault;
    public Func<IGameState, bool>? Condition { get; } = condition;
}
