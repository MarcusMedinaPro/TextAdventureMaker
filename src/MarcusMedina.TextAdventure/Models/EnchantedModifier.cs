// <copyright file="EnchantedModifier.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public class EnchantedModifier(IItem inner) : ItemDecorator(inner)
{
    public override string Name => $"enchanted {Inner.Name}";

    public override string GetDescription()
    {
        string baseDescription = Inner.GetDescription();
        return string.IsNullOrWhiteSpace(baseDescription)
            ? "It hums with a faint magical glow."
            : $"{baseDescription} It hums with a faint magical glow.";
    }
}
