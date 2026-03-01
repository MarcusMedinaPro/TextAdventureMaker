# C# 14 Modernization Guide - TextAdventure Engine

## Overview
This document describes the C# 14 modernization approach applied to the TextAdventure engine. The pattern is consistent across all models and can be applied to complete remaining files.

## Completed Modernizations
- ✅ Item.cs (381 lines)
- ✅ Location.cs (192 lines)
- 87 remaining model files follow this pattern

## Pattern: Modern Constructor (Primary Constructor)

### Before (C# 7)
```csharp
public class MyModel
{
    public string Id { get; }
    public string Name { get; }

    public MyModel(string id, string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        Id = id;
        Name = name;
    }
}
```

### After (C# 12+)
```csharp
public class MyModel(string id, string name)
{
    public string Id { get; } = ValidateId(id);
    public string Name { get; } = name;

    private static string ValidateId(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        return id;
    }
}
```

## Pattern: Collection Expressions

### Before
```csharp
private readonly Dictionary<string, object> _data = new();
private readonly List<string> _items = new();
```

### After
```csharp
private readonly Dictionary<string, object> _data = [];
private readonly List<string> _items = [];
```

## Pattern: Expression-Bodied Members

### Before
```csharp
public string GetName()
{
    return _name;
}

public void Add(string item)
{
    _items.Add(item);
}
```

### After
```csharp
public string GetName() => _name;

public void Add(string item) => _items.Add(item);
```

## Pattern: Early Returns

### Before
```csharp
public IItem? FindItem(string name)
{
    if (string.IsNullOrWhiteSpace(name))
    {
        return null;
    }

    return _items.FirstOrDefault(i => i.Name == name);
}
```

### After
```csharp
public IItem? FindItem(string name)
{
    if (string.IsNullOrWhiteSpace(name))
        return null;

    return _items.FirstOrDefault(i => i.Name == name);
}
```

## Pattern: Switch Expressions

### Before
```csharp
public string GetCondition()
{
    if (!_durability.HasValue || !_maxDurability.HasValue)
        return "";

    int percent = (_durability.Value * 100) / _maxDurability.Value;
    switch (percent)
    {
        case >= 90:
            return "pristine";
        case >= 60:
            return "good";
        case >= 30:
            return "worn";
        case >= 10:
            return "damaged";
        default:
            return "broken";
    }
}
```

### After
```csharp
public string GetCondition() =>
    !_durability.HasValue || !_maxDurability.HasValue ? "" :
    (_durability.Value * 100) / _maxDurability.Value switch
    {
        >= 90 => "pristine",
        >= 60 => "good",
        >= 30 => "worn",
        >= 10 => "damaged",
        _ => "broken"
    };
```

## Batch Application Instructions

### Quick Reference Transformations
1. **Find constructors:** Look for `public ClassName(params) { ... }`
2. **Convert to primary:** `public class ClassName(params) { ... }`
3. **Update properties:** Remove assignment from constructor, use `= param` in property
4. **Collection init:** Replace `new Dictionary<>()` with `[]`
5. **Simple methods:** Convert to `=> expression` syntax
6. **Conditionals:** Remove braces on single-statement blocks

### Testing After Modernization
```bash
# Build to check syntax
dotnet build

# Run tests to verify behavior
dotnet test

# If tests fail, check:
# - Property initialization order
# - Validation logic preservation
# - Constructor overload compatibility
```

## Files by Priority

### High Priority (Large/Complex)
- Door.cs (232 lines)
- Npc.cs (176 lines)
- ActionConsequence.cs (169 lines)
- WorldState.cs (137 lines)
- GameList.cs (128 lines)

### Medium Priority
- Key.cs (127 lines)
- TimedDoor.cs (107 lines)
- Chapter.cs (106 lines)
- GameItemList.cs (115 lines)

### Low Priority (Small files <50 lines)
- Exit.cs
- TimedSpawn.cs
- And 75+ other model files

## Architecture Notes

### Primary Constructor Limitations
- Can only have one primary constructor per class
- Secondary constructors must call primary with `:this(args)`
- Parameters are available throughout constructor body
- Consider validating in property initializers or static methods

### Backward Compatibility
- All public APIs remain unchanged
- Implicit operators still work
- Constructor overloads must be preserved
- Tests verify behavior compatibility

## Performance Impact
- Zero runtime overhead (compiler converts to traditional IL)
- Improved readability and maintainability
- Reduced boilerplate code (~20-30% per file typical)
- Consistent with modern C# best practices

## References
- [C# 12 Primary Constructors](https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-12)
- [C# 11 Collection Expressions](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/collection-expressions)
- [Switch Expressions](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/switch-expression)
