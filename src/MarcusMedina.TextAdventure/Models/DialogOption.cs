// <copyright file="DialogOption.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public sealed class DialogOption(string text, IDialogNode? next = null)
{
    public string Text { get; } = ValidateText(text);
    public IDialogNode? Next { get; } = next;

    private static string ValidateText(string text)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(text);
        return text;
    }
}
