// <copyright file="IAgencyTracker.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Interfaces;

public interface IAgencyTracker
{
    int AgencyScore { get; }
    IReadOnlyDictionary<string, int> Choices { get; }
    void Register(string id, int weight = 1);
    bool HasChoice(string id);
}
