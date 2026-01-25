// <copyright file="EnchantedModifier.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public class EnchantedModifier : ItemDecorator
{
    public EnchantedModifier(IItem inner) : base(inner)
    {
    }

    public override string Name => $"enchanted {Inner.Name}";

    public override string GetDescription()
    {
        var baseDescription = Inner.GetDescription();
        return string.IsNullOrWhiteSpace(baseDescription)
            ? "It hums with a faint magical glow."
            : $"{baseDescription} It hums with a faint magical glow.";
    }
}
