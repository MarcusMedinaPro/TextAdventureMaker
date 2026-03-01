// <copyright file="DescriptionCacheKeyBuilder.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Extensions;

namespace MarcusMedina.TextAdventure.AI.Features;

public static class DescriptionCacheKeyBuilder
{
    public static string Build(DescriptionRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        string deltaPart = request.Deltas is null || request.Deltas.Count == 0
            ? "base"
            : string.Join('|', request.Deltas.Select(x => $"{x.Tag}:{x.Value}"));

        return $"{request.EntityType.ToId()}::{request.EntityId.ToId()}::{deltaPart.ToId()}";
    }
}
