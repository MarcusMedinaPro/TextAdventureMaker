// <copyright file="NpcRelationshipReactions.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Extensions;

namespace MarcusMedina.TextAdventure.AI.Features;

public static class NpcRelationshipReactions
{
    public static NpcRelationshipSnapshot ApplyAffinityDelta(NpcRelationshipSnapshot snapshot, int delta)
    {
        ArgumentNullException.ThrowIfNull(snapshot);
        int affinity = (snapshot.Affinity + delta).Clamp(-100, 100);
        return snapshot with { Affinity = affinity };
    }
}
