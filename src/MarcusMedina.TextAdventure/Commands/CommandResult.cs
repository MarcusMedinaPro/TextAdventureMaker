// <copyright file="CommandResult.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;

namespace MarcusMedina.TextAdventure.Commands;

public sealed record CommandResult(
    bool Success,
    string Message,
    GameError Error = GameError.None,
    bool ShouldQuit = false,
    IReadOnlyList<string>? Reactions = null)
{
    public IReadOnlyList<string> ReactionsList { get; } = Reactions ?? Array.Empty<string>();

    public static CommandResult Ok(string message)
    {
        return new(true, message);
    }

    public static CommandResult Ok(string message, params string[] reactions)
    {
        return new(true, message, GameError.None, false, reactions);
    }

    public static CommandResult Fail(string message, GameError error)
    {
        return new(false, message, error);
    }

    public static CommandResult Fail(string message, GameError error, params string[] reactions)
    {
        return new(false, message, error, false, reactions);
    }

    public static CommandResult Quit(string message)
    {
        return new(true, message, GameError.None, true);
    }
}
