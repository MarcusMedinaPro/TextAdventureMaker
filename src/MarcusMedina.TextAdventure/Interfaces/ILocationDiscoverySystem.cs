// <copyright file="ILocationDiscoverySystem.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace MarcusMedina.TextAdventure.Interfaces;

public interface ILocationDiscoverySystem
{
    IReadOnlyCollection<string> DiscoveredLocations { get; }
    bool FogOfWarEnabled { get; set; }
    bool IsDiscovered(string locationId);
    void Discover(string locationId);
    IEnumerable<ILocation> FilterVisible(IEnumerable<ILocation> locations, bool includeUndiscovered = false);
}
