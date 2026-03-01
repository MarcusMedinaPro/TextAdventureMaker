// <copyright file="GameAiPluginExtensions.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.AI.Plugin;

public static class GameAiPluginExtensions
{
    public static ICommandParser WithAiPlugin(this ICommandParser parser, AiFeatureModule module, AiPluginOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(parser);
        ArgumentNullException.ThrowIfNull(module);
        return new AiPluginCommandParser(parser, module, options);
    }

    public static GameState EnableAiNpcMovement(this GameState state, AiFeatureModule module, AiPluginOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(state);
        ArgumentNullException.ThrowIfNull(module);

        AiPluginOptions pluginOptions = options ?? new AiPluginOptions();
        if (!pluginOptions.EnableAiNpcMovement)
            return state;

        foreach (ILocation location in state.Locations)
        {
            foreach (INpc npc in location.Npcs)
            {
                INpcMovement current = npc.Movement;
                if (current is AiNpcMovementStrategy)
                    continue;

                _ = npc.SetMovement(new AiNpcMovementStrategy(npc, module, current, pluginOptions));
            }
        }

        return state;
    }

    public static Game EnableAiStoryDirector(this Game game, AiFeatureModule module, AiPluginOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(game);
        ArgumentNullException.ThrowIfNull(module);

        AiPluginOptions pluginOptions = options ?? new AiPluginOptions();
        if (!pluginOptions.EnableAiStoryDirector)
            return game;

        int turnCounter = 0;
        int interval = Math.Max(1, pluginOptions.StoryDirectorAiEveryTurns);
        int timeoutMs = Math.Max(250, pluginOptions.RuntimeFeatureTimeoutMs);
        game.AddTurnEndHandler((g, _, result) =>
        {
            if (!result.Success)
                return;

            turnCounter++;
            if (turnCounter % interval != 0)
                return;

            StoryDirectorContext context = AiPluginContextFactory.BuildStoryContext(g.State);
            using CancellationTokenSource cts = new(TimeSpan.FromMilliseconds(timeoutMs));
            StoryEventProposal? proposal = module.StoryDirector
                .ProposeEventAsync(context, cts.Token)
                .GetAwaiter()
                .GetResult();

            if (proposal == null || string.IsNullOrWhiteSpace(proposal.Summary))
                return;

            g.State.WorldState.AddTimeline($"ai_event:{proposal.EventId}:{proposal.Summary}");
        });

        return game;
    }
}
