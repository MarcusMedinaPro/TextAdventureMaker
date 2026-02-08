// <copyright file="Teleporter.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public sealed class Teleporter(string id, string name) : IGameEntity
{
    public string Id { get; } = id ?? "";
    public string Name { get; } = name ?? "";
    public IDictionary<string, string> Properties { get; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    public string? TargetLocationId { get; private set; }

    public Teleporter SetTarget(string locationId)
    {
        TargetLocationId = locationId ?? "";
        return this;
    }
}
