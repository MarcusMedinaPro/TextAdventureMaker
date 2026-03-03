## Slice 57: Advanced Semantic Parser

**Mål:** Intelligent parser som förstår naturligt språk, kontext och komplexa kommandon.

**Referens:** `docs/plans/imported/Semantic_Parser_Design.md`, `docs/plans/imported/Semantic_Parser_Advanced.md`

### Task 57.1: ISemanticParser interface

```csharp
public interface ISemanticParser
{
    ParsedIntent Parse(string input, ParserContext context);
    float GetConfidence(ParsedIntent intent);
    IEnumerable<string> GetSuggestions(string partialInput, ParserContext context);
}

public record ParsedIntent(
    string Verb,
    string? DirectObject,
    string? IndirectObject,
    string? Preposition,
    Direction? Direction,
    Dictionary<string, string> Modifiers,
    float Confidence
);

public record ParserContext(
    IGameState State,
    IEnumerable<string> AvailableVerbs,
    IEnumerable<string> VisibleObjects,
    IEnumerable<string> InventoryItems,
    IEnumerable<string> NpcNames
);
```

### Task 57.2: Natural Language Patterns

```csharp
public class NaturalLanguageParser : ISemanticParser
{
    private readonly List<PatternRule> _patterns =
    [
        // "take the red key from the box"
        new PatternRule(
            @"^(?<verb>take|get|grab|pick up)\s+(?:the\s+)?(?<obj>.+?)\s+from\s+(?:the\s+)?(?<source>.+)$",
            (m, ctx) => new ParsedIntent(
                "take", m.Groups["obj"].Value, m.Groups["source"].Value, "from", null, [], 0.9f)),

        // "put the key in the lock"
        new PatternRule(
            @"^(?<verb>put|place|insert)\s+(?:the\s+)?(?<obj>.+?)\s+(?<prep>in|into|on|onto)\s+(?:the\s+)?(?<target>.+)$",
            (m, ctx) => new ParsedIntent(
                "use", m.Groups["obj"].Value, m.Groups["target"].Value, m.Groups["prep"].Value, null, [], 0.9f)),

        // "go north" / "walk to the kitchen"
        new PatternRule(
            @"^(?<verb>go|walk|move|run)\s+(?:to\s+(?:the\s+)?)?(?<target>.+)$",
            (m, ctx) => ParseMovement(m, ctx)),

        // "ask guard about the key"
        new PatternRule(
            @"^(?<verb>ask|talk to|speak to|speak with)\s+(?:the\s+)?(?<npc>.+?)\s+about\s+(?:the\s+)?(?<topic>.+)$",
            (m, ctx) => new ParsedIntent(
                "ask", m.Groups["npc"].Value, m.Groups["topic"].Value, "about", null, [], 0.85f)),

        // "look at the painting carefully"
        new PatternRule(
            @"^(?<verb>look|examine|inspect)\s+(?:at\s+)?(?:the\s+)?(?<obj>.+?)(?:\s+(?<mod>carefully|closely|quickly))?$",
            (m, ctx) => new ParsedIntent(
                "examine", m.Groups["obj"].Value, null, null, null,
                new() { ["manner"] = m.Groups["mod"].Value }, 0.85f)),
    ];

    public ParsedIntent Parse(string input, ParserContext context)
    {
        input = NormalizeInput(input);

        foreach (var pattern in _patterns)
        {
            var match = pattern.Regex.Match(input);
            if (match.Success)
            {
                var intent = pattern.Handler(match, context);
                return ResolveReferences(intent, context);
            }
        }

        return FallbackParse(input, context);
    }

    private static string NormalizeInput(string input) =>
        input.Trim().ToLowerInvariant()
            .Replace("pick up", "take")
            .Replace("grab", "take");
}
```

### Task 57.3: Context-Aware Resolution

```csharp
public class ContextResolver
{
    public ParsedIntent ResolveReferences(ParsedIntent intent, ParserContext context)
    {
        var resolved = intent;

        // Resolve "it" / "that" / "them"
        if (intent.DirectObject is "it" or "that" or "this")
        {
            var lastMentioned = context.State.GetProperty<string>("last_mentioned_object");
            if (lastMentioned != null)
                resolved = resolved with { DirectObject = lastMentioned };
        }

        // Resolve "him" / "her" / "them"
        if (intent.IndirectObject is "him" or "her" or "them")
        {
            var lastNpc = context.State.GetProperty<string>("last_mentioned_npc");
            if (lastNpc != null)
                resolved = resolved with { IndirectObject = lastNpc };
        }

        // Fuzzy match object names
        if (resolved.DirectObject != null)
        {
            var bestMatch = FindBestMatch(resolved.DirectObject, context.VisibleObjects.Concat(context.InventoryItems));
            if (bestMatch != null)
                resolved = resolved with { DirectObject = bestMatch };
        }

        return resolved;
    }

    private static string? FindBestMatch(string input, IEnumerable<string> candidates)
    {
        // Exact match
        var exact = candidates.FirstOrDefault(c => c.Equals(input, StringComparison.OrdinalIgnoreCase));
        if (exact != null) return exact;

        // Partial match
        var partial = candidates.FirstOrDefault(c => c.Contains(input, StringComparison.OrdinalIgnoreCase));
        if (partial != null) return partial;

        // Fuzzy match
        return candidates
            .Select(c => (candidate: c, distance: LevenshteinDistance(input, c)))
            .Where(x => x.distance <= 2)
            .OrderBy(x => x.distance)
            .Select(x => x.candidate)
            .FirstOrDefault();
    }
}
```

### Task 57.4: Synonym Dictionary

```csharp
public class SynonymDictionary
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

    public IEnumerable<string> GetSynonyms(string verb) =>
        _synonyms.TryGetValue(verb.ToLowerInvariant(), out var syns) ? syns : [];
}
```

### Task 57.5: Command Suggestions

```csharp
public class CommandSuggester
{
    public IEnumerable<string> GetSuggestions(string partialInput, ParserContext context)
    {
        var suggestions = new List<string>();

        if (string.IsNullOrWhiteSpace(partialInput))
        {
            // Föreslå vanliga kommandon
            suggestions.AddRange(["look", "inventory", "go north"]);

            // Föreslå baserat på context
            if (context.VisibleObjects.Any())
                suggestions.Add($"take {context.VisibleObjects.First()}");
            if (context.NpcNames.Any())
                suggestions.Add($"talk to {context.NpcNames.First()}");

            return suggestions;
        }

        var words = partialInput.Split(' ');
        var verb = words[0].ToLowerInvariant();

        // Autocomplete verb
        if (words.Length == 1)
        {
            return context.AvailableVerbs
                .Where(v => v.StartsWith(verb, StringComparison.OrdinalIgnoreCase))
                .Take(5);
        }

        // Autocomplete object
        if (words.Length == 2)
        {
            var partial = words[1];
            var candidates = verb switch
            {
                "take" or "get" => context.VisibleObjects,
                "drop" => context.InventoryItems,
                "talk" or "ask" => context.NpcNames,
                "go" => ["north", "south", "east", "west", "up", "down"],
                _ => context.VisibleObjects.Concat(context.InventoryItems)
            };

            return candidates
                .Where(c => c.StartsWith(partial, StringComparison.OrdinalIgnoreCase))
                .Select(c => $"{verb} {c}")
                .Take(5);
        }

        return suggestions;
    }
}
```

### Task 57.6: Multi-Command Support

```csharp
public class MultiCommandParser
{
    private readonly string[] _separators = ["then", "and then", ",", ";", "and"];

    public IEnumerable<string> SplitCommands(string input)
    {
        var commands = new List<string> { input };

        foreach (var sep in _separators)
        {
            commands = commands
                .SelectMany(c => c.Split(new[] { sep }, StringSplitOptions.RemoveEmptyEntries))
                .Select(c => c.Trim())
                .Where(c => !string.IsNullOrEmpty(c))
                .ToList();
        }

        return commands;
    }
}

// "take key and unlock door then go north"
// → ["take key", "unlock door", "go north"]
```

### Task 57.7: Ambiguity Resolution

```csharp
public class AmbiguityResolver
{
    public DisambiguationResult Resolve(string objectName, ParserContext context)
    {
        var matches = context.VisibleObjects
            .Concat(context.InventoryItems)
            .Where(o => o.Contains(objectName, StringComparison.OrdinalIgnoreCase))
            .ToList();

        return matches.Count switch
        {
            0 => new DisambiguationResult(false, null, $"You don't see any '{objectName}' here."),
            1 => new DisambiguationResult(true, matches[0], null),
            _ => new DisambiguationResult(false, null,
                $"Which do you mean: {string.Join(" or ", matches)}?",
                matches)
        };
    }
}

public record DisambiguationResult(
    bool Resolved,
    string? ResolvedName,
    string? Message,
    List<string>? Options = null
);
```

### Task 57.8: Tester

```csharp
[Theory]
[InlineData("take the red key from the chest", "take", "red key", "chest")]
[InlineData("get key", "take", "key", null)]
[InlineData("pick up lamp", "take", "lamp", null)]
public void Parser_UnderstandsVariousFormats(string input, string verb, string obj, string? source)
{
    var parser = new NaturalLanguageParser();
    var result = parser.Parse(input, CreateContext());

    Assert.Equal(verb, result.Verb);
    Assert.Equal(obj, result.DirectObject);
    Assert.Equal(source, result.IndirectObject);
}

[Fact]
public void Parser_ResolvesPronouns()
{
    var context = CreateContext();
    context.State.SetProperty("last_mentioned_object", "golden key");

    var parser = new NaturalLanguageParser();
    var result = parser.Parse("take it", context);

    Assert.Equal("golden key", result.DirectObject);
}
```

### Task 57.9: Sandbox — naturligt språk-äventyr

Demo där spelaren kan använda fritt formulerade kommandon istället för strikta format.

---

## Completion Checklist
- [x] Core implementation for this slice is present in the engine.
- [x] Behaviour is covered by tests and/or deterministic validation paths.
- [x] Demo document: `57_What_Did_You_Mean.md`.
- [x] Marked complete in project slice status.
