// <copyright file="Fluid.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public class Fluid : IFluid
{
    private string _description = "";

    public string Id { get; }
    public string Name { get; }
    public string? Description => _description;

    public Fluid(string id, string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Id = id;
        Name = name;
    }

    public Fluid(string id, string name, string description) : this(id, name)
    {
        _description = description ?? "";
    }

    public string GetDescription() => _description;

    public IFluid SetDescription(string description)
    {
        _description = description ?? "";
        return this;
    }

    public static implicit operator Fluid((string id, string name, string description) data)
    {
        return new(data.id, data.name, data.description);
    }
}
