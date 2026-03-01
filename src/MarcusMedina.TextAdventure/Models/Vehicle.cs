// <copyright file="Vehicle.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public sealed class Vehicle(string id, string name) : IGameEntity
{
    public string Id { get; } = id ?? "";
    public string Name { get; } = name ?? "";
    public IDictionary<string, string> Properties { get; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    public bool IsOccupied { get; private set; }
    public ILocation? CurrentLocation { get; private set; }

    public Vehicle PlaceAt(ILocation location)
    {
        CurrentLocation = location;
        return this;
    }

    public Vehicle Enter()
    {
        IsOccupied = true;
        return this;
    }

    public Vehicle Exit()
    {
        IsOccupied = false;
        return this;
    }
}
