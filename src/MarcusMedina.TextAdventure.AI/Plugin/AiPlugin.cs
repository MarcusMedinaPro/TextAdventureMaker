// <copyright file="AiPlugin.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Dsl;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.AI.Plugin;

/// <summary>
/// Optional AI plugin. Install via GameBuilder.UsePlugin(new AiPlugin(...)).
/// Extend the DSL parser with AI keywords via ExtendDslParser() before parsing.
/// </summary>
public sealed class AiPlugin(AiFeatureModule module, AiPluginOptions? options = null) : IGamePlugin
{
    private readonly AiFeatureModule _module = module ?? throw new ArgumentNullException(nameof(module));
    private readonly AiPluginOptions _options = options ?? new AiPluginOptions();

    // Closure-owned dicts — populated by DSL keyword handlers, consumed in OnGameBuilt.
    private readonly Dictionary<string, string> _aiRoomHints = [];
    private readonly Dictionary<string, string> _aiNpcBehavior = [];

    /// <summary>
    /// Register AI-specific DSL keywords before parsing the adventure file.
    /// <code>
    /// locationAI: room_id | hint text for AI
    /// npcAI: npc_id | movement
    /// npcAI: npc_id | dialogue
    /// </code>
    /// </summary>
    public AiPlugin ExtendDslParser(AdventureDslParser parser)
    {
        ArgumentNullException.ThrowIfNull(parser);

        parser.RegisterKeyword("locationAI", (_, value) =>
        {
            string[] parts = value.Split('|', 2, StringSplitOptions.TrimEntries);
            if (parts.Length == 2 && !string.IsNullOrWhiteSpace(parts[0]))
                _aiRoomHints[parts[0]] = parts[1];
        });

        parser.RegisterKeyword("npcAI", (_, value) =>
        {
            string[] parts = value.Split('|', 2, StringSplitOptions.TrimEntries);
            if (parts.Length == 2 && !string.IsNullOrWhiteSpace(parts[0]))
                _aiNpcBehavior[parts[0]] = parts[1];
        });

        return this;
    }

    /// <summary>Wrap the current parser with AI. Called automatically by GameBuilder.Build().</summary>
    public GameBuilder Configure(GameBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        ICommandParser? current = builder.CurrentParser;
        if (current is not null)
            builder.UseParser(current.WithAiPlugin(_module, _options));

        return builder;
    }

    /// <summary>Inject AI behaviour into NPCs and rooms. Called automatically after Build().</summary>
    public void OnGameBuilt(Game game)
    {
        ArgumentNullException.ThrowIfNull(game);

        AiPluginOptions opts = _options;

        if (opts.EnableAiDescriptions && opts.EnableAiDescriptionCacheInvalidation)
        {
            game.AddTurnEndHandler((_, command, result) =>
            {
                if (AiDescriptionCacheInvalidationPolicy.ShouldClear(command, result))
                    _module.DescriptionCache.Clear();
            });
        }

        InjectNpcAi(game);
        game.EnableAiStoryDirector(_module, opts);
    }

    private void InjectNpcAi(Game game)
    {
        if (!_options.EnableAiNpcMovement)
            return;

        // DSL opt-in: only inject AI for explicitly listed NPCs.
        // Fallback: if no DSL hints, inject globally (backwards compat).
        bool selective = _aiNpcBehavior.Count > 0;

        foreach (ILocation location in game.State.Locations)
        {
            foreach (INpc npc in location.Npcs)
            {
                if (npc.Movement is AiNpcMovementStrategy)
                    continue;

                bool shouldInject = !selective
                    || (_aiNpcBehavior.TryGetValue(npc.Id, out string? behavior)
                        && behavior.Contains("movement", StringComparison.OrdinalIgnoreCase));

                if (shouldInject)
                    npc.SetMovement(new AiNpcMovementStrategy(npc, _module, npc.Movement, _options));
            }
        }
    }
}
