// <copyright file="AiPluginInitOptions.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.AI.Plugin;

public sealed class AiPluginInitOptions
{
    public required AiProviderInitOptions PrimaryProvider { get; init; }
    public IReadOnlyList<AiProviderInitOptions> FallbackProviders { get; init; } = [];
    public ICommandParser? BaseParser { get; init; }
    public IAiDescriptionCache? DescriptionCache { get; init; }
    public Func<IAiProviderRouter, IAiProviderRouter>? RouterDecorator { get; init; }
    public AiPluginOptions PluginOptions { get; init; } = new();
    public AiParserOptions ParserOptions { get; init; } = new();
}
