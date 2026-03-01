// <copyright file="DebugCommandFactory.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Parsing;

namespace MarcusMedina.TextAdventure.Helpers;

/// <summary>Factory for registering debug commands in the parser.</summary>
public static class DebugCommandFactory
{
    public static KeywordParserConfigBuilder RegisterDebugCommands(
        KeywordParserConfigBuilder builder,
        GameState gameState)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(gameState);

        if (!gameState.DebugMode)
            return builder;

        // Register debug command factories
        builder
            .WithCustomCommand("teleport", (tokens) => new DebugTeleportCommand(gameState, tokens))
            .WithCustomCommand("tp", (tokens) => new DebugTeleportCommand(gameState, tokens))
            .WithCustomCommand("inspect", (tokens) => new DebugInspectCommand(gameState, tokens))
            .WithCustomCommand("setflag", (tokens) => new DebugSetFlagCommand(gameState, tokens))
            .WithCustomCommand("revealmap", (tokens) => new DebugRevealMapCommand(gameState, tokens))
            .WithCustomCommand("godmode", (tokens) => new DebugGodModeCommand(gameState, tokens));

        return builder;
    }
}
