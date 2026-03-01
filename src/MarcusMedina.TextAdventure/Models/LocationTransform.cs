// <copyright file="LocationTransform.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public sealed class LocationTransform(string targetLocationId)
{
    public string TargetLocationId { get; } = targetLocationId ?? "";
    public Func<IGameState, bool>? Condition { get; private set; }
    public bool IsIrreversible { get; private set; }

    public LocationTransform When(Func<IGameState, bool> predicate)
    {
        Condition = predicate;
        return this;
    }

    public LocationTransform Irreversible()
    {
        IsIrreversible = true;
        return this;
    }
}

public sealed class LocationTransformBuilder(LocationTransform transform)
{
    public LocationTransformBuilder When(Func<IGameState, bool> predicate)
    {
        transform.When(predicate);
        return this;
    }

    public LocationTransformBuilder Irreversible()
    {
        transform.Irreversible();
        return this;
    }
}
