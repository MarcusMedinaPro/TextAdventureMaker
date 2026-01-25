// <copyright file="DialogNode.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public sealed class DialogNode : IDialogNode
{
    private readonly List<DialogOption> _options = new();

    public string Text { get; }
    public IReadOnlyList<DialogOption> Options => _options;

    public DialogNode(string text)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(text);
        Text = text;
    }

    public DialogNode AddOption(string text, IDialogNode? next = null)
    {
        _options.Add(new DialogOption(text, next));
        return this;
    }
}
