// <copyright file="AiPluginOptions.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.AI.Plugin;

public sealed class AiPluginOptions
{
    public bool EnableAiDialogue { get; set; } = true;
    public bool EnableAiDescriptions { get; set; } = true;
    public bool EnableAiDescriptionCacheInvalidation { get; set; } = true;
    public bool EnableAiNpcMovement { get; set; } = true;
    public bool EnableAiStoryDirector { get; set; } = true;
    public int RuntimeFeatureTimeoutMs { get; set; } = 1200;
    public int NpcMovementAiEveryTurns { get; set; } = 3;
    public int StoryDirectorAiEveryTurns { get; set; } = 4;
}
