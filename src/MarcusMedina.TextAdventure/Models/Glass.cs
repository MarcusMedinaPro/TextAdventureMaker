// <copyright file="Glass.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public class Glass : ContainerItem<IFluid>
{
    public Glass(string id, string name, int maxCount = 1) : base(id, name, maxCount)
    {
    }

    public Glass(string id, string name, string description, int maxCount = 1) : base(id, name, maxCount)
    {
        _ = Description(description);
    }

    public static implicit operator Glass((string id, string name, string description) data)
    {
        return new(data.id, data.name, data.description);
    }
}
