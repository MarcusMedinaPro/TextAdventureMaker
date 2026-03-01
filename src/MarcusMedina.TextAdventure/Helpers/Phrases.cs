// <copyright file="Phrases.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Helpers;

public sealed class Phrases(IEnumerable<string> phrases)
{
    private readonly List<string> _phrases = phrases?.Where(p => !string.IsNullOrWhiteSpace(p)).ToList() ?? [];
    private int _index;

    public string GetRandomPhrase()
    {
        if (_phrases.Count == 0)
        {
            return string.Empty;
        }

        int pick = Random.Shared.Next(_phrases.Count);
        return _phrases[pick];
    }

    public string GetNextPhrase()
    {
        if (_phrases.Count == 0)
        {
            return string.Empty;
        }

        string phrase = _phrases[_index];
        _index = (_index + 1) % _phrases.Count;
        return phrase;
    }
}
