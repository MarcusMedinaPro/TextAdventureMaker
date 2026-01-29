// <copyright file="Fluid.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace MarcusMedina.TextAdventure.Models;

using MarcusMedina.TextAdventure.Interfaces;

public class Fluid : IFluid
{
    private string _description = "";

    public string Id { get; }
    public string Name { get; }

    public Fluid(string id, string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Id = id;
        Name = name;
    }

    public Fluid(string id, string name, string description) : this(id, name) => _description = description ?? "";

    public string GetDescription() => _description;

    public IFluid Description(string text)
    {
        _description = text;
        return this;
    }

    public static implicit operator Fluid((string id, string name, string description) data) =>
        new(data.id, data.name, data.description);
}
