// <copyright file="IWorldState.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace MarcusMedina.TextAdventure.Interfaces;

public interface IWorldState
{
    IReadOnlyList<string> Timeline { get; }

    void AddTimeline(string entry);

    int GetCounter(string key);

    bool GetFlag(string key);

    int GetRelationship(string npcId);

    int Increment(string key, int amount = 1);

    void SetFlag(string key, bool value);

    void SetRelationship(string npcId, int value);
}