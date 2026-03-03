## Slice 59: Procedural Storytelling

**Mål:** Generera dynamiskt innehåll - sidouppdrag, events, lore och textvariation.

**Referens:** `docs/plans/imported/Future_Development_Ideas.md`

### Task 59.1: IContentGenerator interface

```csharp
public interface IContentGenerator<T>
{
    T Generate(GenerationContext context, int? seed = null);
    IEnumerable<T> GenerateMultiple(GenerationContext context, int count, int? seed = null);
}

public record GenerationContext(
    IGameState State,
    string Theme,
    DifficultyLevel Difficulty,
    IReadOnlyList<string> UsedIds  // Undvik dubletter
);
```

### Task 59.2: Side Quest Generator

```csharp
public class SideQuestGenerator : IContentGenerator<Quest>
{
    private readonly List<QuestTemplate> _templates =
    [
        new("fetch", "Fetch Quest", "Retrieve {item} from {location} for {npc}"),
        new("kill", "Hunt Quest", "Defeat {enemy} threatening {location}"),
        new("escort", "Escort Quest", "Guide {npc} safely to {destination}"),
        new("investigate", "Mystery Quest", "Discover what happened at {location}"),
        new("delivery", "Delivery Quest", "Bring {item} to {npc} before {deadline}"),
    ];

    public Quest Generate(GenerationContext context, int? seed = null)
    {
        var rng = seed.HasValue ? new Random(seed.Value) : new Random();
        var template = _templates[rng.Next(_templates.Count)];

        var quest = new Quest
        {
            Id = $"generated_{Guid.NewGuid():N}",
            Name = FillTemplate(template.NameTemplate, context, rng),
            Description = FillTemplate(template.DescriptionTemplate, context, rng),
            Objectives = GenerateObjectives(template, context, rng),
            Rewards = GenerateRewards(context.Difficulty, rng)
        };

        return quest;
    }

    private string FillTemplate(string template, GenerationContext context, Random rng)
    {
        var items = context.State.GetAllItems().ToList();
        var npcs = context.State.GetAllNpcs().Where(n => !n.IsHostile).ToList();
        var locations = context.State.GetAllLocations().ToList();
        var enemies = context.State.GetAllNpcs().Where(n => n.IsHostile).ToList();

        return template
            .Replace("{item}", items.Any() ? items[rng.Next(items.Count)].Name : "a mysterious artifact")
            .Replace("{npc}", npcs.Any() ? npcs[rng.Next(npcs.Count)].Name : "the stranger")
            .Replace("{location}", locations.Any() ? locations[rng.Next(locations.Count)].Name : "the unknown place")
            .Replace("{enemy}", enemies.Any() ? enemies[rng.Next(enemies.Count)].Name : "the beast")
            .Replace("{destination}", locations.Any() ? locations[rng.Next(locations.Count)].Name : "safety")
            .Replace("{deadline}", $"{rng.Next(10, 30)} turns");
    }
}

public record QuestTemplate(string Type, string NameTemplate, string DescriptionTemplate);
```

### Task 59.3: Random Event Generator

```csharp
public class RandomEventGenerator : IContentGenerator<GameEvent>
{
    private readonly WeightedTable<EventTemplate> _events = new()
    {
        { new("weather_change", "The weather shifts...", 0.3f), 10 },
        { new("npc_arrives", "A stranger approaches.", 0.2f), 8 },
        { new("item_discovery", "You notice something glinting.", 0.4f), 6 },
        { new("sound_distant", "You hear a distant sound.", 0.1f), 15 },
        { new("merchant_passing", "A traveling merchant appears.", 0.15f), 4 },
    };

    public GameEvent Generate(GenerationContext context, int? seed = null)
    {
        var rng = seed.HasValue ? new Random(seed.Value) : new Random();
        var template = _events.Select(rng);

        // Kolla om eventet ska triggas baserat på sannolikhet
        if (rng.NextDouble() > template.Probability)
            return GameEvent.None;

        return new GameEvent
        {
            Id = template.Type,
            Description = ExpandDescription(template.Description, context, rng),
            Effects = GenerateEffects(template.Type, context, rng)
        };
    }
}

public class WeightedTable<T>
{
    private readonly List<(T item, int weight)> _items = [];

    public void Add(T item, int weight) => _items.Add((item, weight));

    public T Select(Random rng)
    {
        var total = _items.Sum(x => x.weight);
        var roll = rng.Next(total);
        var cumulative = 0;

        foreach (var (item, weight) in _items)
        {
            cumulative += weight;
            if (roll < cumulative)
                return item;
        }

        return _items[^1].item;
    }
}
```

### Task 59.4: Lore/Codex Generator

```csharp
public class LoreGenerator : IContentGenerator<LoreEntry>
{
    private readonly List<LoreTemplate> _templates =
    [
        new("history", "In the year {year}, {event} changed {place} forever."),
        new("legend", "They say that {hero} once {deed} using the {artifact}."),
        new("rumor", "Some claim that {npc} knows the secret of {mystery}."),
        new("warning", "Beware the {danger} that lurks in {location}."),
    ];

    public LoreEntry Generate(GenerationContext context, int? seed = null)
    {
        var rng = seed.HasValue ? new Random(seed.Value) : new Random();
        var template = _templates[rng.Next(_templates.Count)];

        return new LoreEntry
        {
            Type = template.Type,
            Text = FillLoreTemplate(template.Text, context, rng),
            DiscoveredAt = context.State.CurrentLocation?.Id
        };
    }

    private string FillLoreTemplate(string template, GenerationContext context, Random rng)
    {
        var years = new[] { "ancient times", "the Age of Kings", "a hundred years ago", "the great war" };
        var events = new[] { "the great fire", "the arrival of the strangers", "the plague", "the eclipse" };
        var deeds = new[] { "slew the dragon", "sealed the portal", "found the lost city", "broke the curse" };

        return template
            .Replace("{year}", years[rng.Next(years.Length)])
            .Replace("{event}", events[rng.Next(events.Length)])
            .Replace("{deed}", deeds[rng.Next(deeds.Length)])
            .Replace("{hero}", GenerateName(rng))
            .Replace("{artifact}", GenerateArtifactName(rng))
            .Replace("{mystery}", GenerateMystery(rng))
            .Replace("{danger}", GenerateDanger(rng));
    }
}
```

### Task 59.5: Text Variation System

```csharp
public class TextVariationSystem
{
    // Slumpa mellan varianter av samma mening
    public string Vary(string key, params string[] variants)
    {
        var rng = new Random();
        return variants[rng.Next(variants.Length)];
    }

    // Template-baserad variation
    public string Expand(string template)
    {
        var rng = new Random();

        // {a|b|c} → slumpa mellan a, b, c
        return Regex.Replace(template, @"\{([^}]+)\}", match =>
        {
            var options = match.Groups[1].Value.Split('|');
            return options[rng.Next(options.Length)];
        });
    }
}

// Exempel:
// "Du {ser|upptäcker|noterar} en {liten|gammal|rostig} nyckel på {golvet|bordet|marken}."
// → "Du upptäcker en rostig nyckel på marken."
```

### Task 59.6: Reproducible Generation (Seeds)

```csharp
public class SeededGenerator
{
    private int _currentSeed;

    public SeededGenerator(int baseSeed)
    {
        _currentSeed = baseSeed;
    }

    public int NextSeed()
    {
        // Deterministic seed progression
        _currentSeed = (_currentSeed * 1103515245 + 12345) & 0x7fffffff;
        return _currentSeed;
    }

    // Samma base seed + samma game state = samma genererade innehåll
    public static int GenerateWorldSeed(string worldName) =>
        worldName.GetHashCode();
}
```

### Task 59.7: Content Mixing (Handwritten + Generated)

```csharp
public class ContentMixer
{
    private readonly List<Quest> _handwrittenQuests = [];
    private readonly SideQuestGenerator _generator = new();

    public void AddHandwritten(Quest quest) => _handwrittenQuests.Add(quest);

    public Quest GetNextQuest(GenerationContext context)
    {
        var rng = new Random();

        // 70% handskrivet, 30% genererat
        if (_handwrittenQuests.Any() && rng.NextDouble() < 0.7)
        {
            var quest = _handwrittenQuests[rng.Next(_handwrittenQuests.Count)];
            _handwrittenQuests.Remove(quest);
            return quest;
        }

        return _generator.Generate(context);
    }
}
```

### Task 59.8: Tester

```csharp
[Fact]
public void SideQuestGenerator_GeneratesDifferentQuestsWithDifferentSeeds()
{
    var generator = new SideQuestGenerator();
    var context = CreateContext();

    var quest1 = generator.Generate(context, seed: 12345);
    var quest2 = generator.Generate(context, seed: 67890);

    Assert.NotEqual(quest1.Name, quest2.Name);
}

[Fact]
public void SideQuestGenerator_GeneratesSameQuestWithSameSeed()
{
    var generator = new SideQuestGenerator();
    var context = CreateContext();

    var quest1 = generator.Generate(context, seed: 12345);
    var quest2 = generator.Generate(context, seed: 12345);

    Assert.Equal(quest1.Name, quest2.Name);
}
```

### Task 59.9: Sandbox — oändligt äventyr

Demo med procedurellt genererade quests och events som aldrig tar slut.

---

## Completion Checklist
- [x] Core implementation for this slice is present in the engine.
- [x] Behaviour is covered by tests and/or deterministic validation paths.
- [x] Demo document: `59_The_Neverending_Noticeboard.md`.
- [x] Marked complete in project slice status.
