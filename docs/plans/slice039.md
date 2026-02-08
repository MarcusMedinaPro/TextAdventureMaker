## Slice 39: Fluent API & Språksnygghet

**Mål:** All syntaktisk socker för snygg, läsbar kod.

### Item Description

- `Models/Item.cs` - lägg till `Description` property
- `Interfaces/IItem.cs` - lägg till `string? Description { get; }`
- Fluent method: `SetDescription(string description)`

### Bulk creation - Items.CreateMany

```csharp
public static class Items
{
    // Tuple-baserad (JSON-tänk)
    public static IEnumerable<Item> CreateMany(
        params (string id, string name, float weight)[] items);

    // Med description
    public static IEnumerable<Item> CreateMany(
        params (string id, string name, float weight, string desc)[] items);
}
```

### Inline DSL - Location.AddDSLItems

```csharp
// Syntax: "Name(weight, takeable|fixed)? | description?"
location.AddDSLItems(
    "Sword(2.5kg, takeable)",
    "Shield(5kg)",
    "Torch",
    "Statue(fixed)",
    "Note | A crumpled letter",
    "Gem(0.1kg) | A sparkling ruby"
);
```

Parser regex: `^(?<name>[\w\s]+)(\((?<props>[^)]+)\))?(\s*\|\s*(?<desc>.+))?$`

### Snabb-add för enkla items

```csharp
// Implicit conversion gör detta möjligt
location.AddItems("Sword", "Shield", "Torch");
```

### Random Extensions (int)

- [ ] `Extensions/RandomExtensions.cs`

```csharp
public static class RandomExtensions
{
    private static readonly Random _rng = new();

    /// <summary>10.Random() → 0-10</summary>
    public static int Random(this int max) => _rng.Next(max + 1);

    /// <summary>10.Random(5) → 5-10</summary>
    public static int Random(this int max, int min) => _rng.Next(min, max + 1);

    /// <summary>6.Dice() → 1-6 (aldrig 0)</summary>
    public static int Dice(this int sides) => _rng.Next(1, sides + 1);

    /// <summary>6.Dice(2) → 2d6 (2-12)</summary>
    public static int Dice(this int sides, int count)
    {
        var total = 0;
        for (var i = 0; i < count; i++)
            total += sides.Dice();
        return total;
    }
}
```

**Användning:**

```csharp
var damage = 6.Dice();           // 1d6 → 1-6
var attack = 20.Dice();          // 1d20 → 1-20
var fireball = 6.Dice(3);        // 3d6 → 3-18
var loot = 100.Random();         // 0-100
var enemyCount = 5.Random(2);    // 2-5
```

### Probability Extensions

- [ ] `Extensions/ProbabilityExtensions.cs`

```csharp
public static class ProbabilityExtensions
{
    private static readonly Random _rng = new();

    /// <summary>50.PercentChance() → true/false</summary>
    public static bool PercentChance(this int percent) =>
        _rng.Next(100) < percent;

    /// <summary>0.3.Chance() → 30% chans</summary>
    public static bool Chance(this double probability) =>
        _rng.NextDouble() < probability;
}
```

### Collection Extensions

- [ ] `Extensions/CollectionExtensions.cs`

```csharp
public static class CollectionExtensions
{
    private static readonly Random _rng = new();

    public static T PickRandom<T>(this IList<T> list) =>
        list[_rng.Next(list.Count)];

    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source) =>
        source.OrderBy(_ => _rng.Next());

    public static T WeightedRandom<T>(this IEnumerable<T> source, Func<T, int> weightSelector)
    {
        var items = source.ToList();
        var totalWeight = items.Sum(weightSelector);
        var roll = _rng.Next(totalWeight);
        var cumulative = 0;
        foreach (var item in items)
        {
            cumulative += weightSelector(item);
            if (roll < cumulative) return item;
        }
        return items.Last();
    }
}
```

### Time Extensions

- [ ] `Extensions/TimeExtensions.cs`

```csharp
public static class TimeExtensions
{
    public static TimeSpan Milliseconds(this int ms) => TimeSpan.FromMilliseconds(ms);
    public static TimeSpan Seconds(this int s) => TimeSpan.FromSeconds(s);
    public static TimeSpan Minutes(this int m) => TimeSpan.FromMinutes(m);
}
```

### Console Extensions (OBS: Endast för Console.Write!)

- [ ] `Extensions/ConsoleExtensions.cs`

```csharp
/// <summary>
/// VIKTIGT: Dessa extensions fungerar ENDAST med Console.Write.
/// Om du använder egen output-hantering, implementera IGameOutput istället.
/// </summary>
public static class ConsoleExtensions
{
    public static void TypewriterPrint(this string text, int delayMs = 50)
    {
        foreach (var c in text)
        {
            Console.Write(c);
            Thread.Sleep(delayMs);
        }
        Console.WriteLine();
    }
}
```

### Range/Clamp Extensions

- [ ] `Extensions/RangeExtensions.cs`

```csharp
public static class RangeExtensions
{
    public static int Clamp(this int value, int min, int max) =>
        Math.Max(min, Math.Min(max, value));

    public static bool IsBetween(this int value, int min, int max) =>
        value >= min && value <= max;
}
```

### Conditional Fluent Extensions

- [ ] `Extensions/ConditionalExtensions.cs`

```csharp
public static class ConditionalExtensions
{
    public static ConditionalResult<string> Then(this bool condition, string trueValue) =>
        new(condition, trueValue);

    public static ConditionalResult<T> Then<T>(this bool condition, Func<T> trueAction) =>
        new(condition, condition ? trueAction() : default);

    public static void Then(this bool condition, Action action)
    {
        if (condition) action();
    }
}

---

## Implementation checklist (engine)
- [ ] `Description` property + `SetDescription(...)` on `Item`
- [ ] `Items.CreateMany(...)` helpers
- [ ] Inline DSL for items (`AddDSLItems`)
- [ ] Implicit `AddItems("Sword", ...)` helpers
- [ ] Random extensions
- [ ] Probability extensions
- [ ] Collection extensions
- [ ] Time extensions
- [ ] Console extensions (typewriter)
- [ ] Range/Clamp extensions
- [ ] Conditional fluent extensions

## Example checklist (docs/examples)
- [ ] Fluent API showcase for these helpers

public class ConditionalResult<T>
{
    private readonly bool _condition;
    private readonly T? _value;

    public ConditionalResult(bool condition, T? value)
    {
        _condition = condition;
        _value = value;
    }

    public T Else(T falseValue) => _condition ? _value! : falseValue;
    public T Else(Func<T> falseAction) => _condition ? _value! : falseAction();
}
```

**Användning:**

```csharp
// Villkorlig text
var desc = isDark.Then("Pitch black.").Else("Sunlight streams in.");

// Villkorlig action
hasKey.Then(() => door.Unlock())
      .Else(() => Console.WriteLine("The door is locked."));

// Enkel trigger
isFirstVisit.Then(() => ShowIntro());
```

### Grammar Extensions (språkberoende - kräver ILanguage override!)

- [ ] `Extensions/GrammarExtensions.cs`
- [ ] `Interfaces/IGrammarProvider.cs`

```csharp
public interface IGrammarProvider
{
    string WithArticle(string noun);           // "a sword", "an apple", "ett svärd"
    string Plural(string noun, int count);     // "3 swords", "3 svärd"
    string NaturalList(IEnumerable<string> items);  // "sword, shield, and torch"
}

// Default implementation (English)
public class EnglishGrammar : IGrammarProvider { ... }

// Extensions använder registrerad provider
public static class GrammarExtensions
{
    public static IGrammarProvider Provider { get; set; } = new EnglishGrammar();

    public static string WithArticle(this string noun) => Provider.WithArticle(noun);
    public static string Plural(this string noun, int count) => Provider.Plural(noun, count);
    public static IEnumerable<string> ToNaturalList(this IEnumerable<string> items) => ...;
}
```

**OBS:** Byt `GrammarExtensions.Provider = new SwedishGrammar()` för svenska.

### Tester

- `FluentApiTests.cs` - test för CreateMany, AddDSLItems
- `RandomExtensionsTests.cs` - test för Random, Dice
- `ProbabilityExtensionsTests.cs` - test för PercentChance, Chance
- `CollectionExtensionsTests.cs` - test för PickRandom, Shuffle, WeightedRandom
- `ConditionalExtensionsTests.cs` - test för Then/Else
- `GrammarExtensionsTests.cs` - test för alla språk

---
