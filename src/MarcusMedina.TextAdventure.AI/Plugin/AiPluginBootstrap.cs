// <copyright file="AiPluginBootstrap.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.AI.Plugin;

public sealed class AiPluginBootstrap
{
    public required ICommandParser Parser { get; init; }
    public required AiCommandParser AiParser { get; init; }
    public required AiFeatureModule Module { get; init; }
    public required AiPluginOptions PluginOptions { get; init; }

    public GameBuilder ApplyParserTo(GameBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        return builder.UseParser(Parser);
    }

    public Game EnableRuntime(Game game)
    {
        ArgumentNullException.ThrowIfNull(game);
        return game.EnableAiPluginRuntime(Module, PluginOptions);
    }
}
