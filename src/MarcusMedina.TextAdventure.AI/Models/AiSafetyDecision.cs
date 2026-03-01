// <copyright file="AiSafetyDecision.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.AI.Models;

public sealed record AiSafetyDecision(bool IsAllowed, string? Message = null)
{
    public static AiSafetyDecision Allow() => new(true);
    public static AiSafetyDecision Reject(string message) => new(false, message);
}
