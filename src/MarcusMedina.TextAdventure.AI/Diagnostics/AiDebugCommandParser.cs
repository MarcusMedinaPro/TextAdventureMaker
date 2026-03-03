// <copyright file="AiDebugCommandParser.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.AI.Diagnostics;

/// <summary>Wraps a command parser to intercept /debug toggle commands.</summary>
public sealed class AiDebugCommandParser(ICommandParser inner, AiDebugTracker tracker) : ICommandParser
{
    private readonly ICommandParser _inner = inner ?? throw new ArgumentNullException(nameof(inner));
    private readonly AiDebugTracker _tracker = tracker ?? throw new ArgumentNullException(nameof(tracker));

    public ICommand Parse(string input)
    {
        if (DebugToggleModeParser.TryParse(input, out DebugToggleMode mode))
            return new DebugToggleCommand(_tracker, mode);

        return _inner.Parse(input);
    }
}

/// <summary>Executes a debug toggle against an <see cref="AiDebugTracker"/>.</summary>
public sealed class DebugToggleCommand(AiDebugTracker tracker, DebugToggleMode mode) : ICommand
{
    private readonly AiDebugTracker _tracker = tracker ?? throw new ArgumentNullException(nameof(tracker));
    private readonly DebugToggleMode _mode = mode;

    public CommandResult Execute(CommandContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        string message = _mode switch
        {
            DebugToggleMode.Toggle => _tracker.Toggle(),
            DebugToggleMode.On => _tracker.Set(true),
            DebugToggleMode.Off => _tracker.Set(false),
            _ => _tracker.Status()
        };

        return CommandResult.Ok(message);
    }
}

public enum DebugToggleMode { Toggle, On, Off, Status }

/// <summary>Parses /debug [on|off|status] input into a <see cref="DebugToggleMode"/>.</summary>
public static class DebugToggleModeParser
{
    public static bool TryParse(string? input, out DebugToggleMode mode)
    {
        mode = DebugToggleMode.Toggle;
        if (string.IsNullOrWhiteSpace(input))
            return false;

        string trimmed = input.Trim();
        if (!trimmed.StartsWith("/debug", StringComparison.OrdinalIgnoreCase))
            return false;

        if (trimmed.Equals("/debug", StringComparison.OrdinalIgnoreCase))
            return true;

        mode = trimmed["/debug".Length..].Trim().ToLowerInvariant() switch
        {
            "on" => DebugToggleMode.On,
            "off" => DebugToggleMode.Off,
            "status" => DebugToggleMode.Status,
            _ => DebugToggleMode.Toggle
        };

        return true;
    }
}
