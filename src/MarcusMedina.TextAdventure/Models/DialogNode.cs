// <copyright file="DialogNode.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace MarcusMedina.TextAdventure.Models;

using MarcusMedina.TextAdventure.Interfaces;

public sealed class DialogNode : IDialogNode
{
    private readonly List<DialogOption> _options = [];

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
