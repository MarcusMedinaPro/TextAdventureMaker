// <copyright file="CommandAllowlistSafetyPolicy.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Extensions;

namespace MarcusMedina.TextAdventure.AI.Policies;

public sealed class CommandAllowlistSafetyPolicy : IAiCommandSafetyPolicy
{
    private static readonly IReadOnlyCollection<string> DefaultCommands =
    [
        "look", "examine", "inventory", "stats", "go", "move",
        "take", "drop", "use", "combine", "pour",
        "open", "unlock", "close", "lock", "destroy",
        "eat", "drink", "read", "talk", "attack", "flee",
        "save", "load", "quest", "hint", "quit"
    ];

    private readonly HashSet<string> _allowedCommands;

    public CommandAllowlistSafetyPolicy(IEnumerable<string>? allowedCommands = null)
    {
        _allowedCommands = new HashSet<string>(allowedCommands ?? DefaultCommands, StringComparer.OrdinalIgnoreCase);
    }

    public AiSafetyDecision Evaluate(string? commandText)
    {
        if (string.IsNullOrWhiteSpace(commandText))
            return AiSafetyDecision.Reject("AI returned an empty command.");

        string firstToken = commandText
            .Trim()
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .FirstOrDefault()
            .Lower();

        if (string.IsNullOrWhiteSpace(firstToken))
            return AiSafetyDecision.Reject("AI command was missing a verb.");

        return _allowedCommands.Contains(firstToken)
            ? AiSafetyDecision.Allow()
            : AiSafetyDecision.Reject($"AI command '{firstToken}' is not in the allowlist.");
    }
}
