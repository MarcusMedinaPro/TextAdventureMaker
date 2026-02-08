// <copyright file="ProppianStructureBuilder.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Engine;

public sealed class ProppianStructureBuilder
{
    private readonly ProppianStructure _structure = new();

    public ProppianStructureBuilder RequiredFunctions(params ProppFunction[] functions)
    {
        _structure.RequiredFunctions(functions);
        return this;
    }

    public ProppianStructureBuilder OptionalFunctions(params ProppFunction[] functions)
    {
        _structure.OptionalFunctions(functions);
        return this;
    }

    public ProppianStructureBuilder ValidateOrder(bool validate)
    {
        _structure.ValidateOrder(validate);
        return this;
    }

    public ProppianStructure Build()
    {
        return _structure;
    }
}
