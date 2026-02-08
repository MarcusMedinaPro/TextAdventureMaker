// <copyright file="AgencyTracker.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Engine;

public sealed class AgencyTracker : IAgencyTracker
{
    private readonly Dictionary<string, int> _choices = new(StringComparer.OrdinalIgnoreCase);

    public int AgencyScore => _choices.Values.Sum();
    public IReadOnlyDictionary<string, int> Choices => _choices;

    public void Register(string id, int weight = 1)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return;
        }

        _choices[id] = weight;
    }

    public bool HasChoice(string id)
    {
        return !string.IsNullOrWhiteSpace(id) && _choices.ContainsKey(id);
    }
}
