// <copyright file="RustyModifier.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Models;

using MarcusMedina.TextAdventure.Interfaces;

public class RustyModifier(IItem inner) : ItemDecorator(inner)
{

    public override string Name => $"rusty {Inner.Name}";

    public override string GetDescription() =>
        Inner.GetDescription() is { Length: > 0 } desc
            ? $"{desc} It's old and rusty."
            : "It looks old and rusty.";
}
