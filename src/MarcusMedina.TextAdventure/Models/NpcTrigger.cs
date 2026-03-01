// <copyright file="NpcTrigger.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public sealed class NpcTrigger(NpcSense sense, string target)
{
    private Action<DialogContext>? _then;

    public NpcSense Sense { get; } = sense;
    public string Target { get; } = target is not null && target.Trim().Length > 0 ? target.Trim() : throw new ArgumentException("Value cannot be null or whitespace.", nameof(target));
    public int DelayTicks { get; private set; }
    public int? ScheduledTick { get; set; }
    public bool SayOnlyOnce { get; private set; }
    public bool HasFired { get; set; }
    public bool ShouldFlee { get; private set; }
    public string? Message { get; private set; }

    public NpcTrigger AfterTicks(int ticks)
    {
        DelayTicks = Math.Max(0, ticks);
        return this;
    }

    public NpcTrigger AfterSeconds(int seconds) => AfterTicks(seconds);

    public NpcTrigger Say(string text)
    {
        Message = text ?? "";
        return this;
    }

    public NpcTrigger SayOnce(string text)
    {
        Message = text ?? "";
        SayOnlyOnce = true;
        return this;
    }

    public NpcTrigger Then(Action<DialogContext> action)
    {
        ArgumentNullException.ThrowIfNull(action);
        _then = action;
        return this;
    }

    public NpcTrigger Flee()
    {
        ShouldFlee = true;
        SayOnlyOnce = true;
        return this;
    }

    public bool Matches(NpcSense sense, string target) =>
        Sense == sense && Target.Equals(target, StringComparison.OrdinalIgnoreCase);

    public void Apply(DialogContext context) => _then?.Invoke(context);
}
