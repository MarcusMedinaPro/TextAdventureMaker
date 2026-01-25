// <copyright file="Exit.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public class Exit
{
    public ILocation Target { get; }
    public IDoor? Door { get; }
    public bool IsOneWay { get; }

    public bool IsPassable => Door == null || Door.IsPassable;

    public Exit(ILocation target, IDoor? door = null, bool isOneWay = false)
    {
        ArgumentNullException.ThrowIfNull(target);
        Target = target;
        Door = door;
        IsOneWay = isOneWay;
    }
}
