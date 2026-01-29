// <copyright file="FluidItem.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace MarcusMedina.TextAdventure.Models;

using MarcusMedina.TextAdventure.Interfaces;

public class FluidItem : Item, IFluid
{
    public FluidItem(string id, string name) : base(id, name)
    {
    }

    public FluidItem(string id, string name, string description) : base(id, name, description)
    {
    }

    public new FluidItem Description(string text)
    {
        _ = base.Description(text);
        return this;
    }

    IFluid IFluid.Description(string text) => Description(text);

    public static implicit operator FluidItem((string id, string name, string description) data) =>
        new(data.id, data.name, data.description);
}
