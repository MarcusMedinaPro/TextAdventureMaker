// <copyright file="ProppianStructure.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;

namespace MarcusMedina.TextAdventure.Models;

public sealed class ProppianStructure
{
    private readonly HashSet<ProppFunction> _required = [];
    private readonly HashSet<ProppFunction> _optional = [];

    public IReadOnlyCollection<ProppFunction> Required => _required;
    public IReadOnlyCollection<ProppFunction> Optional => _optional;
    public bool OrderValidationEnabled { get; private set; }

    public ProppianStructure RequiredFunctions(IEnumerable<ProppFunction> functions)
    {
        AddFunctions(_required, functions);
        return this;
    }

    public ProppianStructure OptionalFunctions(IEnumerable<ProppFunction> functions)
    {
        AddFunctions(_optional, functions);
        return this;
    }

    public ProppianStructure ValidateOrder(bool enabled)
    {
        OrderValidationEnabled = enabled;
        return this;
    }

    private static void AddFunctions(HashSet<ProppFunction> target, IEnumerable<ProppFunction> functions)
    {
        if (functions == null)
        {
            return;
        }

        foreach (ProppFunction function in functions)
        {
            target.Add(function);
        }
    }
}
