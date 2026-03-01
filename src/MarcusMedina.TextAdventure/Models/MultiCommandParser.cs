// <copyright file="MultiCommandParser.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Models;

/// <summary>
/// Splits compound commands separated by conjunctions into individual commands.
/// </summary>
public sealed class MultiCommandParser
{
    private readonly string[] _separators = ["then", "and then", ",", ";", "and"];

    /// <summary>
    /// Splits a compound command into multiple individual commands.
    /// </summary>
    /// <remarks>
    /// Example: "take key and unlock door then go north"
    /// → ["take key", "unlock door", "go north"]
    /// </remarks>
    public IEnumerable<string> SplitCommands(string input)
    {
        var commands = new List<string> { input };

        foreach (var sep in _separators)
        {
            commands = commands
                .SelectMany(c => c.Split(new[] { sep }, StringSplitOptions.None))
                .Select(c => c.Trim())
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .ToList();
        }

        return commands;
    }
}
