// <copyright file="AllowAllAiCommandSafetyPolicy.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.AI.Policies;

public sealed class AllowAllAiCommandSafetyPolicy : IAiCommandSafetyPolicy
{
    public AiSafetyDecision Evaluate(string? commandText)
    {
        return string.IsNullOrWhiteSpace(commandText)
            ? AiSafetyDecision.Reject("AI returned an empty command.")
            : AiSafetyDecision.Allow();
    }
}
