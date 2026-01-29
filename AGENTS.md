# Agent Instructions

You are an agent working on a text adventure engine. Follow these instructions exactly.

---

## SLICE WORKFLOW

Execute these steps in order. Do not skip any step.

### STEP 1: IDENTIFY SLICE
```
ACTION: Find the current slice document
LOCATION: docs/examples/ or docs/slices/
RULE: If multiple documents exist for same slice, process ONE at a time
```

### STEP 2: LIST EXTENSIONS
```
ACTION: Before any coding, list ALL existing extensions
LOCATION: src/MarcusMedina.TextAdventure/Extensions/
PURPOSE: You MUST use these throughout your code
OUTPUT: List each extension file and its key methods
```

### STEP 3: COPY TO SANDBOX
```
ACTION: Copy code from the slice markdown file
DESTINATION: sandbox/TextAdventure.Sandbox/Program.cs
TRANSFORM: Refactor while copying (see CODE STYLE below)
```

### STEP 4: REFACTOR
```
ACTION: Apply these transformations to the code:
- Replace repeated patterns with existing extensions
- Convert verbose code to expression-bodied members (=>)
- Convert if/else chains to switch expressions
- Convert nested ifs to early returns
- Use pattern matching where applicable
```

### STEP 5: BUILD AND TEST
```
ACTION: Build the sandbox project
COMMAND: dotnet build sandbox/TextAdventure.Sandbox/
THEN: Run and verify functionality works
```

### STEP 6: WAIT FOR MARCUS
```
ACTION: Stop and wait for Marcus to test
RESPONSE: Fix any issues he reports
REPEAT: Steps 5-6 until Marcus approves
```

### STEP 7: UPDATE DOCUMENTATION
```
ACTION: Copy improved code FROM sandbox BACK TO documentation
DESTINATION: The same markdown file from Step 1
```

### STEP 8: COMMIT
```
ACTION: Commit changes with descriptive message
FORMAT: <type>: <description>

Co-Authored-By: Claude Opus 4.5 <noreply@anthropic.com>
```

---

## CODE STYLE

### EXTENSIONS FIRST (MANDATORY)
```
BEFORE writing any code:
1. Check if an extension already does what you need
2. If pattern repeats 2+ times → propose new extension
3. Never duplicate logic that exists in extensions
```

### EXPRESSION SYNTAX
```csharp
// PREFER: Expression-bodied
string GetName() => _name;

// AVOID: Block body for simple returns
string GetName() { return _name; }
```

### IF-STATEMENTS
```csharp
// PREFER: Early return, no braces
if (invalid)
    return;

// AVOID: Nested blocks
if (valid)
{
    // lots of code
}
```

### SWITCH EXPRESSIONS
```csharp
// PREFER: Switch expression
var result = input switch
{
    "a" => HandleA(),
    "b" => HandleB(),
    _ => HandleDefault()
};

// AVOID: Switch statement for simple mappings
```

### PATTERN MATCHING
```csharp
// PREFER: Pattern matching
if (command is GoCommand go && go.Direction == Direction.North)
    return MoveNorth();

// AVOID: Type checks + casts
if (command.GetType() == typeof(GoCommand))
{
    var go = (GoCommand)command;
    // ...
}
```

### MODERN C# FEATURES
```
USE:
- Primary constructors: class Foo(string name)
- Collection expressions: [item1, item2]
- Required members where appropriate
- Init-only setters for immutable data
```

---

## GAME RULES

```
ALWAYS:
- Start with intro showing the goal
- Show the starting room on launch
- Auto-look after successful go/move command

NEVER:
- Mention takeable items in room descriptions
- Mention destroyable items in room descriptions
- (Exception: items that are NOT takeable/destroyable)

LANGUAGE:
- All text in British English
- Use "colour" not "color"
- Use "behaviour" not "behavior"
```

---

## QUICK REFERENCE

| Task | Action |
|------|--------|
| Find extensions | `ls src/*/Extensions/` |
| Build sandbox | `dotnet build sandbox/TextAdventure.Sandbox/` |
| Run sandbox | `dotnet run --project sandbox/TextAdventure.Sandbox/` |
| Run tests | `dotnet test` |
