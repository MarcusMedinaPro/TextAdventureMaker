// <copyright file="AdjacentRoom.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

/// <summary>
/// Represents a room adjacent to the current location with visibility and audibility factors.
/// </summary>
public sealed record AdjacentRoom(
    Direction Direction,
    ILocation Location,
    float Visibility,
    float Audibility,
    string? BlockedBy);
