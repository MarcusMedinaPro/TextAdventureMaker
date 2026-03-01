// <copyright file="INpcDialogueAiService.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.AI.Contracts;

public interface INpcDialogueAiService
{
    Task<NpcDialogueResponse?> GenerateReplyAsync(NpcAiContext context, string playerInput, CancellationToken cancellationToken = default);
}
