// <copyright file="AiPluginBootstrapExtensions.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.AI.Plugin;

public static class AiPluginBootstrapExtensions
{
    public static GameBuilder UseAiPluginParser(
        this GameBuilder builder,
        ICommandParser baseParser,
        AiFeatureModule module,
        AiPluginOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(baseParser);
        ArgumentNullException.ThrowIfNull(module);

        return builder.UseParser(baseParser.WithAiPlugin(module, options));
    }

    public static Game EnableAiPluginRuntime(this Game game, AiFeatureModule module, AiPluginOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(game);
        ArgumentNullException.ThrowIfNull(module);

        AiPluginOptions pluginOptions = options ?? new AiPluginOptions();
        if (pluginOptions.EnableAiDescriptions && pluginOptions.EnableAiDescriptionCacheInvalidation)
        {
            game.AddTurnEndHandler((_, command, result) =>
            {
                if (AiDescriptionCacheInvalidationPolicy.ShouldClear(command, result))
                    module.DescriptionCache.Clear();
            });
        }

        _ = game.State.EnableAiNpcMovement(module, pluginOptions);
        _ = game.EnableAiStoryDirector(module, pluginOptions);
        return game;
    }
}
