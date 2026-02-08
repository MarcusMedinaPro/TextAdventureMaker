// <copyright file="Memory.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Models;

public sealed class Memory(string id)
{
    public string Id { get; } = id ?? "";
    public FlashbackTrigger? Trigger { get; private set; }
    public string? Content { get; private set; }
    public int Duration { get; private set; } = 1;

    public Memory SetTrigger(Action<FlashbackTriggerBuilder> configure)
    {
        FlashbackTrigger trigger = new(Id);
        FlashbackTriggerBuilder builder = new(trigger);
        configure?.Invoke(builder);
        Trigger = trigger;
        return this;
    }

    public Memory SetContent(string text)
    {
        Content = text ?? "";
        return this;
    }

    public Memory SetDuration(int turns)
    {
        Duration = turns <= 0 ? 1 : turns;
        return this;
    }
}

public sealed class FlashbackTriggerBuilder(FlashbackTrigger trigger)
{
    public FlashbackTriggerBuilder OnEnterLocation(string locationId)
    {
        trigger.OnEnterLocation(locationId);
        return this;
    }
}
