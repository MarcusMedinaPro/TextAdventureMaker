// <copyright file="FlashbackTrigger.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public sealed class FlashbackTrigger(string memoryId)
{
    public string MemoryId { get; } = memoryId ?? "";
    public string? LocationId { get; private set; }
    public Func<IGameState, bool>? Condition { get; private set; }
    public string? TransitionText { get; private set; }
    public string? ReturnLocationId { get; private set; }

    public FlashbackTrigger OnEnterLocation(string locationId)
    {
        LocationId = locationId ?? "";
        return this;
    }

    public FlashbackTrigger When(Func<IGameState, bool> predicate)
    {
        Condition = predicate;
        return this;
    }

    public FlashbackTrigger WithTransition(string text)
    {
        TransitionText = text ?? "";
        return this;
    }

    public FlashbackTrigger ReturnsTo(string locationId)
    {
        ReturnLocationId = locationId ?? "";
        return this;
    }
}
