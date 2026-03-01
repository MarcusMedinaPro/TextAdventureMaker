// <copyright file="IAiCommandProvider.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.AI.Contracts;

public interface IAiCommandProvider
{
    string Name { get; }

    Task<AiProviderResult> ParseAsync(AiParseRequest request, CancellationToken cancellationToken = default);
}
