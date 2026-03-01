# 🧠 Semantic Word Object Parser - Revolutionary Language System

## Konceptet: Ord som Objekt

Istället för att matcha exakta strängar, skapar vi **intelligenta ord-objekt** som förstår semantiska relationer och kan kombineras flexibelt.

```csharp
// Grundläggande koncept
var pet = new Verb("pet")
    .WithSynonyms("stroke", "pat", "caress", "touch gently")
    .WithEmotionalTone(EmotionalTone.Gentle, EmotionalTone.Caring);

var cat = new Noun("cat")
    .WithSynonyms("kitten", "feline", "kitty", "moggy")
    .WithCategory(ObjectCategory.Animal)
    .WithProperties(Animate.Yes, Friendly.Usually);

// Semantisk kombination
var petCatAction = pet.Combine(cat)
    .WithFillWords("the", "a", "that", "this", "freaking", "damn", "cute")
    .WithOptionalModifiers("gently", "carefully", "lovingly", "quickly")
    .TriggersAction<PetAnimalAction>();
```

## 🎯 Semantisk Flexibilitet

### Input Variationer som ALLA fungerar:
```
"pet cat"           → PetAnimalAction(cat)
"pet the cat"       → PetAnimalAction(cat)
"pet the cute cat"  → PetAnimalAction(cat, modifier: "cute")
"gently pet cat"    → PetAnimalAction(cat, manner: "gently")
"stroke the kitten" → PetAnimalAction(cat, synonym: "kitten")
"pat that feline"   → PetAnimalAction(cat, synonym: "feline")
"pet the freaking cat lovingly" → PetAnimalAction(cat, intensity: "freaking", manner: "lovingly")
```

## 🏗️ Arkitektur: Word Object System

### Word Base Classes
```csharp
public abstract class WordObject
{
    public string CanonicalForm { get; set; }
    public HashSet<string> Synonyms { get; set; } = new();
    public SemanticProperties Properties { get; set; } = new();
    public EmotionalTone Tone { get; set; }

    public virtual bool Matches(string input) =>
        Synonyms.Contains(input.ToLower()) ||
        CanonicalForm.Equals(input, StringComparison.OrdinalIgnoreCase);
}

public class Verb : WordObject
{
    public VerbType Type { get; set; } // Transitive, Intransitive, etc.
    public List<ObjectCategory> AcceptedObjects { get; set; } = new();
    public ActionDelegate Action { get; set; }
}

public class Noun : WordObject
{
    public ObjectCategory Category { get; set; }
    public AnimacyLevel Animacy { get; set; }
    public SizeCategory Size { get; set; }
    public string ReferenceId { get; set; } // Links to actual game object
}

public class Modifier : WordObject
{
    public ModifierType Type { get; set; } // Adjective, Adverb, Intensifier
    public List<WordObject> AppliesTo { get; set; } = new();
}
```

### Semantic Combination Engine
```csharp
public class SemanticCombination
{
    public Verb MainVerb { get; set; }
    public Noun DirectObject { get; set; }
    public Noun IndirectObject { get; set; }
    public List<Modifier> Modifiers { get; set; } = new();
    public List<string> FillWords { get; set; } = new(); // "the", "a", etc.
    public List<string> IntensityWords { get; set; } = new(); // "freaking", "damn"

    public ActionResult Execute(GameContext context)
    {
        var action = MainVerb.CreateAction();
        action.Target = DirectObject?.ResolveInContext(context);
        action.Modifiers = Modifiers;
        action.Intensity = CalculateIntensity();
        return action.Execute(context);
    }
}
```

## 🧙‍♂️ Intelligent Parser Pipeline

### 1. Tokenization & Classification
```csharp
public class SemanticTokenizer
{
    public List<SemanticToken> Tokenize(string input)
    {
        var tokens = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var result = new List<SemanticToken>();

        foreach (var token in tokens)
        {
            var classification = ClassifyToken(token);
            result.Add(new SemanticToken
            {
                Text = token,
                PossibleTypes = classification,
                Confidence = CalculateConfidence(token, classification)
            });
        }

        return result;
    }

    private List<WordType> ClassifyToken(string token)
    {
        var types = new List<WordType>();

        if (_verbs.Any(v => v.Matches(token))) types.Add(WordType.Verb);
        if (_nouns.Any(n => n.Matches(token))) types.Add(WordType.Noun);
        if (_modifiers.Any(m => m.Matches(token))) types.Add(WordType.Modifier);
        if (_fillWords.Contains(token.ToLower())) types.Add(WordType.FillWord);
        if (_intensifiers.Contains(token.ToLower())) types.Add(WordType.Intensifier);

        return types;
    }
}
```

### 2. Semantic Pattern Matching
```csharp
public class SemanticPatternMatcher
{
    private readonly List<SemanticPattern> _patterns = new()
    {
        // [VERB] [OBJECT] - "pet cat"
        new SemanticPattern
        {
            Structure = new[] { WordType.Verb, WordType.Noun },
            Priority = 100,
            Builder = tokens => new SemanticCombination
            {
                MainVerb = tokens[0].AsVerb(),
                DirectObject = tokens[1].AsNoun()
            }
        },

        // [VERB] [FILL] [OBJECT] - "pet the cat"
        new SemanticPattern
        {
            Structure = new[] { WordType.Verb, WordType.FillWord, WordType.Noun },
            Priority = 95,
            Builder = tokens => new SemanticCombination
            {
                MainVerb = tokens[0].AsVerb(),
                DirectObject = tokens[2].AsNoun(),
                FillWords = { tokens[1].Text }
            }
        },

        // [MODIFIER] [VERB] [FILL] [MODIFIER] [OBJECT] - "gently pet the cute cat"
        new SemanticPattern
        {
            Structure = new[] { WordType.Modifier, WordType.Verb, WordType.FillWord, WordType.Modifier, WordType.Noun },
            Priority = 90,
            Builder = tokens => new SemanticCombination
            {
                MainVerb = tokens[1].AsVerb(),
                DirectObject = tokens[4].AsNoun(),
                Modifiers = { tokens[0].AsModifier(), tokens[3].AsModifier() },
                FillWords = { tokens[2].Text }
            }
        }
    };
}
```

## 🎨 Expressiv Språkhantering

### Emotional & Contextual Parsing
```csharp
public class EmotionalParser
{
    public EmotionalContext AnalyzeInput(string input, GameContext gameContext)
    {
        var emotion = EmotionalTone.Neutral;
        var intensity = 1.0f;

        // Intensifier detection
        if (input.Contains("freaking") || input.Contains("damn"))
            intensity *= 1.5f;
        if (input.Contains("gently") || input.Contains("carefully"))
            emotion = EmotionalTone.Gentle;
        if (input.Contains("lovingly") || input.Contains("tenderly"))
            emotion = EmotionalTone.Affectionate;

        // Context-aware emotion
        if (gameContext.Player.Mood == PlayerMood.Frustrated)
            intensity *= 1.2f;

        return new EmotionalContext(emotion, intensity);
    }
}
```

### Advanced Synonym & Context Resolution
```csharp
public class ContextualNoun : Noun
{
    public Dictionary<GameContext, string> ContextualForms { get; set; } = new();

    public override string GetDisplayForm(GameContext context)
    {
        // Different words in different situations
        if (context.CurrentRoom.HasProperty("formal"))
            return "feline"; // Formal setting
        if (context.Player.Age < 12)
            return "kitty"; // Child player
        if (context.Player.Mood == PlayerMood.Affectionate)
            return "sweet cat";

        return base.GetDisplayForm(context);
    }
}
```

## 🚀 Avancerade Features

### 1. Learning & Adaptation
```csharp
public class AdaptiveParser
{
    private Dictionary<string, float> _playerPreferences = new();

    public void LearnFromPlayer(string input, ActionResult result)
    {
        if (result.Success && result.SatisfactionRating > 0.8f)
        {
            // Player liked this phrasing - prioritize similar patterns
            var pattern = ExtractPattern(input);
            _playerPreferences[pattern] = _playerPreferences.GetValueOrDefault(pattern) + 0.1f;
        }
    }

    public List<SemanticCombination> SuggestAlternatives(string input)
    {
        // "You could also try: 'stroke the cat', 'pet the kitty', 'caress the feline'"
        return GenerateVariations(input)
            .OrderByDescending(v => _playerPreferences.GetValueOrDefault(v.Pattern))
            .Take(3)
            .ToList();
    }
}
```

### 2. Multi-Language Support
```csharp
public class MultilingualWordObject : WordObject
{
    public Dictionary<CultureInfo, LocalizedWordData> Localizations { get; set; } = new();

    public void AddLocalization(CultureInfo culture, string canonicalForm, params string[] synonyms)
    {
        Localizations[culture] = new LocalizedWordData
        {
            CanonicalForm = canonicalForm,
            Synonyms = new HashSet<string>(synonyms),
            GrammaticalRules = GetGrammarRules(culture)
        };
    }
}

// Exempel: Svenska
pet.AddLocalization(new CultureInfo("sv-SE"), "klappa", "kela", "smeka", "krama");
cat.AddLocalization(new CultureInfo("sv-SE"), "katt", "kisse", "mjau", "feline");

// Nu fungerar: "klappa katten", "kela med kissen", "smeka mjau"
```

### 3. Voice & Accessibility Integration
```csharp
public class VoiceSemanticParser : SemanticParser
{
    public override SemanticCombination Parse(string input)
    {
        // Handle speech-to-text ambiguities
        input = ResolveHomophones(input);
        input = CorrectCommonSpeechErrors(input);

        var result = base.Parse(input);

        // Provide audio feedback for successful parsing
        if (result.IsValid)
            _audioFeedback.PlayConfirmation(result.GetDescription());

        return result;
    }

    private string ResolveHomophones(string input)
    {
        // "pet the cat" vs "pet their cat"
        return input.Replace("their cat", "the cat"); // Context-dependent
    }
}
```

## 🎮 Integration med TAF

### Game Object Binding
```csharp
public class GameObjectNoun : Noun
{
    public string GameObjectId { get; set; }

    public override GameObject ResolveInContext(GameContext context)
    {
        var obj = context.CurrentRoom.FindObject(GameObjectId);
        if (obj == null)
            obj = context.Player.Inventory.FindObject(GameObjectId);
        return obj;
    }
}

// Automatic binding
var catInRoom = room.AddObject(new Cat("fluffy"));
var catNoun = new GameObjectNoun("cat")
    .WithSynonyms("kitten", "feline", "fluffy")
    .BindToGameObject(catInRoom.Id);
```

### Fluent Builder Integration
```csharp
public static class SemanticExtensions
{
    public static WorldBuilder WithSemanticParser(this WorldBuilder builder,
        Action<SemanticParserBuilder> configure)
    {
        var parserBuilder = new SemanticParserBuilder();
        configure(parserBuilder);

        builder.SetParser(parserBuilder.Build());
        return builder;
    }
}

// Usage in world creation
var world = new WorldBuilder("MyAdventure")
    .WithSemanticParser(parser => parser
        .AddVerb("pet", v => v
            .WithSynonyms("stroke", "pat", "caress")
            .AcceptsObjects(ObjectCategory.Animal)
            .TriggersAction<PetAnimalAction>())
        .AddNoun("cat", n => n
            .WithSynonyms("kitten", "feline", "kitty")
            .WithCategory(ObjectCategory.Animal)
            .BindToRoomObject("fluffy_cat"))
        .WithEmotionalTones(enabled: true)
        .WithLearning(enabled: true))
    .Build();
```

## 🌟 Benefits av Semantic Approach

### För Spelare:
✅ **Naturligt språk** - skriver som de tänker
✅ **Minskad frustration** - färre "I don't understand" meddelanden
✅ **Expressiv frihet** - kan uttrycka känslor och intensitet
✅ **Accessibility** - fungerar bättre med voice input

### För Utvecklare:
✅ **Mindre hårdkodning** - semantic rules istället för string lists
✅ **Lätt att utöka** - bara lägg till nya word objects
✅ **Multi-språk ready** - samma logik, olika ord
✅ **AI-ready** - perfekt för LLM integration

### För Framtiden:
🚀 **LLM Integration** - kan träna på semantic patterns
🚀 **Procedural Expansion** - AI kan generera nya word combinations
🚀 **Player Analytics** - förstå HUR spelare uttrycker sig
🚀 **Educational Value** - lär spelare rikare språkbruk

---

**Detta är verkligen next-level thinking för TAF! 🧠✨**

Vill du att jag utvecklar någon specifik del av detta system mer detaljerat?