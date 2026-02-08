// <copyright file="OllamaCommandParser.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.AI;

public sealed class OllamaCommandParser : ICommandParser
{
    private readonly ICommandParser _fallback;

    public OllamaSettings Settings { get; }

    public OllamaCommandParser(OllamaSettings settings, ICommandParser fallback)
    {
        Settings = settings ?? throw new ArgumentNullException(nameof(settings));
        _fallback = fallback ?? throw new ArgumentNullException(nameof(fallback));
    }

    public ICommand Parse(string input)
    {
        return _fallback.Parse(input);
    }
}
