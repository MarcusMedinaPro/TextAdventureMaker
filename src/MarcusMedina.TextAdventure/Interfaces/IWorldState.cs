// <copyright file="IWorldState.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace MarcusMedina.TextAdventure.Interfaces;

public interface IWorldState
{
    bool GetFlag(string key);
    void SetFlag(string key, bool value);

    int GetCounter(string key);
    int Increment(string key, int amount = 1);

    int GetRelationship(string npcId);
    void SetRelationship(string npcId, int value);

    IReadOnlyList<string> Timeline { get; }
    void AddTimeline(string entry);
}
