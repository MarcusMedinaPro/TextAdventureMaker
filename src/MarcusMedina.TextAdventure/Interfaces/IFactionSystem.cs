// <copyright file="IFactionSystem.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace MarcusMedina.TextAdventure.Interfaces;

public interface IFactionSystem
{
    IReadOnlyDictionary<string, IFaction> Factions { get; }

    IFaction AddFaction(string id);
    IFaction? GetFaction(string id);
    int GetReputation(string id);
    int ModifyReputation(string id, int amount, IGameState state);
}
