// <copyright file="NarrativeTemplate.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Models;

public sealed class NarrativeTemplate(string name)
{
    private readonly List<string> _steps = [];

    public string Name { get; } = name ?? "";
    public IReadOnlyList<string> Steps => _steps;

    public NarrativeTemplate AddStep(string step)
    {
        if (!string.IsNullOrWhiteSpace(step))
        {
            _steps.Add(step);
        }

        return this;
    }
}
