// <copyright file="NaturalLanguageParser.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;
using System.Text.RegularExpressions;

namespace MarcusMedina.TextAdventure.Models;

/// <summary>
/// Semantic parser using regex patterns to understand natural language commands.
/// </summary>
public sealed class NaturalLanguageParser : ISemanticParser
{
    private readonly List<PatternRule> _patterns = [];
    private readonly SynonymDictionary _synonyms = new();
    private readonly ContextResolver _resolver = new();
    private readonly CommandSuggester _suggester = new();

    public NaturalLanguageParser()
    {
        InitializePatterns();
    }

    public ParsedIntent Parse(string input, ParserContext context)
    {
        input = NormalizeInput(input);

        foreach (var pattern in _patterns)
        {
            var match = pattern.Regex.Match(input);
            if (match.Success)
            {
                var intent = pattern.Handler(match, context);
                return _resolver.ResolveReferences(intent, context);
            }
        }

        return FallbackParse(input, context);
    }

    public float GetConfidence(ParsedIntent intent) => intent.Confidence;

    public IEnumerable<string> GetSuggestions(string partialInput, ParserContext context) =>
        _suggester.GetSuggestions(partialInput, context);

    private void InitializePatterns()
    {
        // "take the red key from the box"
        _patterns.Add(new PatternRule(
            @"^(?<verb>take|get|grab|pick up)\s+(?:the\s+)?(?<obj>.+?)\s+from\s+(?:the\s+)?(?<source>.+)$",
            (m, ctx) => new ParsedIntent("take", m.Groups["obj"].Value, m.Groups["source"].Value, "from", null, new(), 0.9f)));

        // "put the key in the lock"
        _patterns.Add(new PatternRule(
            @"^(?<verb>put|place|insert)\s+(?:the\s+)?(?<obj>.+?)\s+(?<prep>in|into|on|onto)\s+(?:the\s+)?(?<target>.+)$",
            (m, ctx) => new ParsedIntent("use", m.Groups["obj"].Value, m.Groups["target"].Value, m.Groups["prep"].Value, null, new(), 0.9f)));

        // "ask guard about the key"
        _patterns.Add(new PatternRule(
            @"^(?<verb>ask|talk to|speak to|speak with)\s+(?:the\s+)?(?<npc>.+?)\s+about\s+(?:the\s+)?(?<topic>.+)$",
            (m, ctx) => new ParsedIntent("ask", m.Groups["npc"].Value, m.Groups["topic"].Value, "about", null, new(), 0.85f)));

        // "look at the painting carefully"
        _patterns.Add(new PatternRule(
            @"^(?<verb>look|examine|inspect)\s+(?:at\s+)?(?:the\s+)?(?<obj>.+?)(?:\s+(?<mod>carefully|closely|quickly))?$",
            (m, ctx) => new ParsedIntent("examine", m.Groups["obj"].Value, null, null, null, new() { ["manner"] = m.Groups["mod"].Value }, 0.85f)));

        // Simple verb + object
        _patterns.Add(new PatternRule(
            @"^(?<verb>\w+)\s+(?:the\s+)?(?<obj>.+)$",
            (m, ctx) => new ParsedIntent(_synonyms.GetCanonicalVerb(m.Groups["verb"].Value), m.Groups["obj"].Value, null, null, null, new(), 0.7f)));
    }

    private ParsedIntent FallbackParse(string input, ParserContext context)
    {
        var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var verb = parts.Length > 0 ? _synonyms.GetCanonicalVerb(parts[0]) : "unknown";
        var obj = parts.Length > 1 ? string.Join(" ", parts.Skip(1)) : null;

        return new ParsedIntent(verb, obj, null, null, null, new(), 0.5f);
    }

    private static string NormalizeInput(string input) =>
        input.Trim().ToLowerInvariant()
            .Replace("pick up", "take")
            .Replace("talk to", "ask")
            .Replace("speak to", "ask");

    private sealed class PatternRule
    {
        public Regex Regex { get; }
        public Func<Match, ParserContext, ParsedIntent> Handler { get; }

        public PatternRule(string pattern, Func<Match, ParserContext, ParsedIntent> handler)
        {
            Regex = new Regex(pattern, RegexOptions.IgnoreCase);
            Handler = handler;
        }
    }
}
