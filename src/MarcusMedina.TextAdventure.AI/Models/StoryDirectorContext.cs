// <copyright file="StoryDirectorContext.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.AI.Models;

public sealed record StoryDirectorContext(
    string CurrentLocationId,
    IReadOnlyList<string> ConnectedLocations,
    IReadOnlyList<string> ActiveQuestIds,
    IReadOnlyDictionary<string, bool> Flags,
    IReadOnlyDictionary<string, int>? Counters = null);
