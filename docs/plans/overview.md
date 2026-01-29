# Text Adventure Engine – Implementation Plan

> **For Claude:** REQUIRED SUB-SKILL: Use `superpowers:executing-plans` to implement this plan task by task.

**Goal:**
Build a fluent, SOLID text adventure engine delivered as two NuGet packages (Core + AI), exposing an expressive internal DSL and a modern, idiomatic C# API.

**Architecture:**
The core engine is built around classic and modern design patterns (State, Command, Observer, Memento, Strategy, Factory, Decorator, Composite, Chain of Responsibility, Visitor, Facade, Flyweight, Proxy, Prototype, Template Method).
All subsystems are hidden behind interfaces and composed through a fluent `GameBuilder`.

The AI package adds Ollama-based natural-language command parsing via `ICommandParser`.

**Technology Stack:**
C# (latest language version), .NET 10, xUnit, System.Text.Json.

All refactoring must target the *modern C# toolset*, explicitly making use of:

* Pattern matching in `if` / `switch` expressions
* Primary constructors
* Records where semantically appropriate
* Expression-bodied members
* Required members
* Init-only setters
* Collection expressions
* Modern `Span` / `ReadOnlySpan` where relevant
* Fluent extension methods to keep APIs readable and discoverable

The codebase should always be kept as *fluent and intention-revealing as possible*, using what already exists in the C# and .NET standard libraries before introducing custom abstractions.

---

## Core Design Decisions

### TDD Approach: Green/Blue

Skip the red phase.
Write test → implement → test passes → refactor → tests still pass.

### Slice Workflow Rule (mandatory - do not skip steps)

Execute these steps in order. Do not skip any step.

#### STEP 1: IDENTIFY SLICE
```
ACTION: Find the current slice document
LOCATION: docs/examples/ or docs/slices/
RULE: If multiple documents exist for same slice, process ONE at a time
```

#### STEP 2: LIST EXTENSIONS
```
ACTION: Before any coding, list ALL existing extensions
LOCATION: src/MarcusMedina.TextAdventure/Extensions/
PURPOSE: You MUST use these throughout your code
OUTPUT: List each extension file and its key methods
```

#### STEP 3: COPY TO SANDBOX
```
ACTION: Copy code from the slice markdown file
DESTINATION: sandbox/TextAdventure.Sandbox/Program.cs
TRANSFORM: Refactor while copying (see CODE STYLE below)
```

#### STEP 4: REFACTOR
```
ACTION: Apply these transformations to the code:
- Replace repeated patterns with existing extensions
- Convert verbose code to expression-bodied members (=>)
- Convert if/else chains to switch expressions
- Convert nested ifs to early returns
- Use pattern matching where applicable
```

#### STEP 5: BUILD AND TEST
```
ACTION: Build the sandbox project
COMMAND: dotnet build sandbox/TextAdventure.Sandbox/
THEN: Run and verify functionality works
```

#### STEP 6: WAIT FOR MARCUS
```
ACTION: Stop and wait for Marcus to test
RESPONSE: Fix any issues he reports
REPEAT: Steps 5-6 until Marcus approves
```

#### STEP 7: UPDATE DOCUMENTATION
```
ACTION: Copy improved code FROM sandbox BACK TO documentation
DESTINATION: The same markdown file from Step 1
```

#### STEP 8: COMMIT
```
ACTION: Commit changes with descriptive message
FORMAT: <type>: <description>

Co-Authored-By: Claude Opus 4.5 <noreply@anthropic.com>
```

### Code Style Rules

#### EXTENSIONS FIRST (MANDATORY)
```
BEFORE writing any code:
1. Check if an extension already does what you need
2. If pattern repeats 2+ times → propose new extension
3. Never duplicate logic that exists in extensions
```

#### EXPRESSION SYNTAX
```csharp
// PREFER: Expression-bodied
string GetName() => _name;

// AVOID: Block body for simple returns
string GetName() { return _name; }
```

#### IF-STATEMENTS
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

#### SWITCH EXPRESSIONS
```csharp
// PREFER: Switch expression
var result = input switch
{
    "a" => HandleA(),
    "b" => HandleB(),
    _ => HandleDefault()
};
```

#### PATTERN MATCHING
```csharp
// PREFER: Pattern matching
if (command is GoCommand go && go.Direction == Direction.North)
    return MoveNorth();
```

#### MODERN C# FEATURES
```
USE:
- Primary constructors: class Foo(string name)
- Collection expressions: [item1, item2]
- Required members where appropriate
- Init-only setters for immutable data
```

#### GAME RULES
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

### Fluent Consistency Rule

If an object gains a property, method, or extension, provide equivalent affordances for related objects (e.g. `Item`, `Key`, `Door`) to preserve a coherent and discoverable fluent API.

### Language Abstraction

Each command supports language-specific keywords (e.g. *go* vs *gå*).
`ICommandParser` maps natural language to semantic `ICommand` objects.

Consider whether a better architectural approach exists and propose improvements.

### Language System

* One language loaded at runtime from file (e.g. `gamelang.en.txt`, `gamelang.sv.txt`)
* Default: English
* All player-visible text comes from the language file

### Bi-directional Exits

Calling:

```csharp
hall.AddExit(Direction.North, bedroom);
```

automatically creates:

```csharp
bedroom.AddExit(Direction.South, hall);
```

unless explicitly disabled for one-way passages.

### Doors

* Exits may have doors (`IDoor`)
* States: Open, Closed, Locked, Destroyed
* Locked doors require an `IKey`
* Events: `OnOpen`, `OnClose`, `OnLock`, `OnUnlock`, `OnDestroy`

### Items and Objects

* `Takeable : bool`
* `Weight : float?` (for inventory limits)
* Containers via Composite
* Generic constraints, e.g. `Glass : IContainer<IFluid>`
* Combinations (ice + fire → water)
* Events: `OnTake`, `OnDrop`, `OnUse`, `OnOpen`, `OnClose`, `OnDestroy`

### Inventory Limits

Configurable by weight, by count, or unlimited.

### NPCs

* State: Friendly, Hostile, Dead, etc.
* Movement: None, Random, Patrol, Follow
* NPCs move on game ticks
