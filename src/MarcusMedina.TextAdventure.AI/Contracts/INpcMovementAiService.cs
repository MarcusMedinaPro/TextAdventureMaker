// <copyright file="INpcMovementAiService.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.AI.Contracts;

public interface INpcMovementAiService
{
    Task<NpcMovementDecision?> ChooseNextLocationAsync(NpcMovementContext context, CancellationToken cancellationToken = default);
}
