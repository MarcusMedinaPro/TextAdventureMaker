// <copyright file="AiPluginCommandParser.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.AI.Plugin;

public sealed class AiPluginCommandParser(
    ICommandParser inner,
    AiFeatureModule module,
    AiPluginOptions? options = null) : ICommandParser
{
    private readonly ICommandParser _inner = inner ?? throw new ArgumentNullException(nameof(inner));
    private readonly AiFeatureModule _module = module ?? throw new ArgumentNullException(nameof(module));
    private readonly AiPluginOptions _options = options ?? new AiPluginOptions();

    public ICommand Parse(string input)
    {
        ICommand baseCommand = _inner.Parse(input);

        if (_options.EnableAiDialogue && baseCommand is TalkCommand talk)
            return new AiTalkCommand(talk.Target, _module);

        if (_options.EnableAiDescriptions && baseCommand is LookCommand look)
            return new AiLookCommand(look.Target, _module);

        return baseCommand;
    }
}
