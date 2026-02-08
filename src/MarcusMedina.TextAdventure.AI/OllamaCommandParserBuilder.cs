// <copyright file="OllamaCommandParserBuilder.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Parsing;

namespace MarcusMedina.TextAdventure.AI;

public sealed class OllamaCommandParserBuilder
{
    private readonly OllamaSettings _settings = new();
    private ICommandParser? _fallback;

    public OllamaCommandParserBuilder WithEndpoint(string endpoint)
    {
        _settings.Endpoint = endpoint ?? "";
        return this;
    }

    public OllamaCommandParserBuilder WithModel(string model)
    {
        _settings.Model = model ?? "";
        return this;
    }

    public OllamaCommandParserBuilder WithSystemPrompt(string prompt)
    {
        _settings.SystemPrompt = prompt ?? "";
        return this;
    }

    public OllamaCommandParserBuilder WithFallback(ICommandParser parser)
    {
        _fallback = parser;
        return this;
    }

    public OllamaCommandParser Build()
    {
        ICommandParser fallback = _fallback ?? new KeywordParser(KeywordParserConfig.Default);
        return new OllamaCommandParser(_settings, fallback);
    }
}
