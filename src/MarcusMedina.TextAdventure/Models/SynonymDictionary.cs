// <copyright file="SynonymDictionary.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Models;

/// <summary>
/// Maps verb synonyms to canonical verb forms for command parsing.
/// </summary>
public sealed class SynonymDictionary
{
    private readonly Dictionary<string, HashSet<string>> _synonyms = new()
    {
        ["take"] = ["get", "grab", "pick", "collect", "acquire"],
        ["drop"] = ["put down", "release", "discard", "leave"],
        ["look"] = ["examine", "inspect", "view", "check", "study"],
        ["go"] = ["walk", "move", "travel", "head", "proceed"],
        ["attack"] = ["hit", "strike", "fight", "kill", "slay"],
        ["talk"] = ["speak", "ask", "chat", "converse", "discuss"],
        ["open"] = ["unlock", "unseal"],
        ["close"] = ["shut", "seal"],
        ["use"] = ["activate", "operate", "employ", "apply"],
    };

    /// <summary>
    /// Gets the canonical verb form for a given verb or synonym.
    /// </summary>
    public string GetCanonicalVerb(string verb)
    {
        verb = verb.ToLowerInvariant();

        foreach (var (canonical, synonyms) in _synonyms)
        {
            if (canonical == verb || synonyms.Contains(verb))
                return canonical;
        }

        return verb;
    }

    /// <summary>
    /// Gets all synonyms for a canonical verb.
    /// </summary>
    public IEnumerable<string> GetSynonyms(string verb) =>
        _synonyms.TryGetValue(verb.ToLowerInvariant(), out var syns) ? syns : [];
}
