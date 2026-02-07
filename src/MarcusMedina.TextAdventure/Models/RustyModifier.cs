// <copyright file="RustyModifier.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public class RustyModifier : ItemDecorator
{
    public RustyModifier(IItem inner) : base(inner)
    {
    }

    public override string Name => $"rusty {Inner.Name}";

    public override string GetDescription()
    {
        string baseDescription = Inner.GetDescription();
        return string.IsNullOrWhiteSpace(baseDescription)
            ? "It looks old and rusty."
            : $"{baseDescription} It's old and rusty.";
    }
}
