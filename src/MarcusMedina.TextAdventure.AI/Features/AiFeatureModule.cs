// <copyright file="AiFeatureModule.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.AI.Features;

public sealed class AiFeatureModule
{
    public required INpcDialogueAiService NpcDialogue { get; init; }
    public required INpcMovementAiService NpcMovement { get; init; }
    public required IStoryDirectorAiService StoryDirector { get; init; }
    public required INpcCombatAiService NpcCombat { get; init; }
    public required IRoomDescriptionAiService RoomDescriptions { get; init; }
    public required IItemDescriptionAiService ItemDescriptions { get; init; }
    public required IAiDescriptionCache DescriptionCache { get; init; }

    public static AiFeatureModule Create(IAiProviderRouter router, IAiDescriptionCache? cache = null, AiParserOptions? options = null)
    {
        IAiDescriptionCache descriptionCache = cache ?? new SessionAiDescriptionCache();
        return new AiFeatureModule
        {
            NpcDialogue = new NpcDialogueAiService(router, options),
            NpcMovement = new NpcMovementAiService(router, options),
            StoryDirector = new StoryDirectorAiService(router, options),
            NpcCombat = new NpcCombatAiService(router, options),
            RoomDescriptions = new RoomDescriptionAiService(router, descriptionCache, options),
            ItemDescriptions = new ItemDescriptionAiService(router, descriptionCache, options),
            DescriptionCache = descriptionCache
        };
    }
}
