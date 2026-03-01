// <copyright file="IAiCommandSafetyPolicy.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.AI.Contracts;

public interface IAiCommandSafetyPolicy
{
    AiSafetyDecision Evaluate(string? commandText);
}
