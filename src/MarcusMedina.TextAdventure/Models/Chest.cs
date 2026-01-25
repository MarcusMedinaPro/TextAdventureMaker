// <copyright file="Chest.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public class Chest : ContainerItem<IItem>
{
    public Chest(string id, string name, int maxCount = 0) : base(id, name, maxCount)
    {
    }

    public Chest(string id, string name, string description, int maxCount = 0) : base(id, name, maxCount)
    {
        Description(description);
    }

    public static implicit operator Chest((string id, string name, string description) data) =>
        new(data.id, data.name, data.description);
}
