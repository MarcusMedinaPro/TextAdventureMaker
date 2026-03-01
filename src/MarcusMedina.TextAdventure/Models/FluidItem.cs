// <copyright file="FluidItem.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public class FluidItem : Item, IFluid
{
    public FluidItem(string id, string name) : base(id, name)
    {
    }

    public FluidItem(string id, string name, string description) : base(id, name, description)
    {
    }

    public new FluidItem SetDescription(string text)
    {
        _ = base.SetDescription(text);
        return this;
    }

    IFluid IFluid.SetDescription(string text)
    {
        return SetDescription(text);
    }

    public static implicit operator FluidItem((string id, string name, string description) data)
    {
        return new(data.id, data.name, data.description);
    }
}
