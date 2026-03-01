// <copyright file="OllamaCommandParser.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.AI;

[Obsolete("Use AiCommandParser instead.")]
public sealed class OllamaCommandParser : AiCommandParser
{
    public OllamaCommandParser(OllamaSettings settings, ICommandParser fallback)
        : base(settings, fallback)
    {
    }

    public OllamaCommandParser(
        IAiProviderRouter router,
        ICommandParser fallback,
        IAiCommandSafetyPolicy? safetyPolicy = null,
        AiParserOptions? options = null,
        string? systemPrompt = null)
        : base(router, fallback, safetyPolicy, options, systemPrompt)
    {
    }
}
