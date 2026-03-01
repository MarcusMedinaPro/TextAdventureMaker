// <copyright file="StoreExtensions.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Extensions;

/// <summary>
/// Extension methods for managing stores in locations.
/// </summary>
public static class StoreExtensions
{
    private static readonly Dictionary<ILocation, IStore> LocationStores = [];

    /// <summary>
    /// Sets a store for a location.
    /// </summary>
    public static void SetStore(this ILocation location, IStore store) =>
        LocationStores[location] = store;

    /// <summary>
    /// Gets the store for a location, if one exists.
    /// </summary>
    public static IStore? GetStore(this ILocation location) =>
        LocationStores.TryGetValue(location, out var store) ? store : null;
}
