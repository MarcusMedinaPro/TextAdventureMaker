## Slice 094: Custom Commands + NPC Reactions DSL

**Goal:** Two tightly coupled keywords: `command:` declares custom verbs the parser should recognise; `npc_reaction:` lets authors define NPC responses to any command — built-in or custom — without writing C#.

**Reference:** `docs/plans/slice093.md`

---

### Motivation

Players will try anything. Without a way to declare custom verbs and attach NPC responses to them, authors must either ignore the input or misuse `command_alias` (which routes through existing command logic with unintended side-effects).

```adventure
command: blow, juggle, sniff, threaten

item_reaction: trumpet | on=blow     | text=A mournful note echoes through the hall.
item_reaction: candle  | on=blow     | text=The flame gutters and dies.

npc_reaction:  guard   | on=blow     | text=The guard gives you a very long, very disapproving stare.
npc_reaction:  guard   | on=threaten | text=The guard raises an eyebrow slowly.
npc_reaction:  guard   | on=attack   | text=The guard's hand moves to his sword hilt.
npc_reaction:  guard   | on=take     | text=The guard watches your hands carefully.
```

Default verbs (go, look, take, talk, use…) never need declaring.

---

### DSL Syntax

#### `command:`

```adventure
command: blow, juggle, sniff, threaten
```

- Comma-separated list of custom verb names.
- Parser accepts `<verb>` and `<verb> <target>` as valid input.
- Fires `CustomActionCommand(verb, target?)`.
- No built-in logic — only reactions defined by the author respond.

#### `npc_reaction:`

```adventure
npc_reaction: <npc_id> | on=<trigger>         | text=<message>
npc_reaction: <npc_id> | on=<trigger>         | text=<message> | if=<condition>
npc_reaction: <npc_id> | on=<verb>:<target_id> | text=<message>
```

**Trigger formats:**

| Trigger | Fires when... |
| --- | --- |
| `blow` | Player uses the custom verb `blow` in the NPC's room |
| `blow:trumpet` | Player uses `blow` targeting a specific item |
| `use` | Player uses any item |
| `use:trumpet` | Player uses a specific item |
| `attack` | Player attempts to attack |
| `take` | Player takes any item in the room |
| `take:<item_id>` | Player takes a specific item |
| `talk` | Player talks (fires only if no dialog tree produces output) |
| Any custom verb | Declared via `command:` |

Specific triggers (`blow:trumpet`) take precedence over general ones (`blow`).

**Optional condition:**

```adventure
npc_reaction: guard | on=blow:trumpet | text=He covers his ears. | if=flag:guard_on_duty=true
```

---

### Task 094.1: `CustomActionCommand`

```csharp
public sealed class CustomActionCommand(string Verb, string? Target) : ICommand
{
    public string Verb { get; } = Verb;
    public string? Target { get; } = Target;

    public CommandResult Execute(CommandContext context)
        => CommandResult.Ok(string.Empty); // reactions are appended by the hook
}
```

---

### Task 094.2: `NpcReaction` model

Add to `INpc` / `Npc`:

```csharp
public sealed record NpcReaction(string Trigger, string Text, Func<IGameState, bool>? Condition = null);

IReadOnlyList<NpcReaction> Reactions { get; }
INpc AddReaction(string trigger, string text, Func<IGameState, bool>? condition = null);
string? GetReaction(string trigger, IGameState state);
```

`GetReaction` returns the first matching reaction text where trigger matches and condition passes, or `null`.
Specific triggers (`blow:trumpet`) are checked before general ones (`blow`).

---

### Task 094.3: C# API

DSL and C# must be fully equivalent. Every DSL keyword has a fluent C# counterpart.

#### Custom verbs — `KeywordParserConfigBuilder`

```csharp
// Single verb
KeywordParserConfigBuilder.BritishDefaults()
    .AddCustomVerb("blow")
    .Build();

// Multiple verbs
KeywordParserConfigBuilder.BritishDefaults()
    .AddCustomVerbs("blow", "threaten", "juggle")
    .Build();
```

#### NPC reactions — `INpc`

```csharp
// Simple reaction
guard.AddReaction("blow", "The guard gives you a very long, very disapproving stare.");

// Specific target takes precedence over general
guard.AddReaction("blow:trumpet", "The guard covers his ears. \"Must you do that here?!\"");
guard.AddReaction("blow", "The guard gives you a very long, very disapproving stare.");

// With condition
guard.AddReaction(
    "blow:trumpet",
    "He covers his ears.",
    state => state.WorldState.GetFlag("guard_on_duty"));

// Chained (INpc returns this)
var guard = new Npc("guard", "Guard")
    .Description("A palace guard in full ceremonial dress.")
    .AddReaction("blow:trumpet", "The guard covers his ears. \"Must you?!\"")
    .AddReaction("blow",         "The guard gives you a very long, very disapproving stare.")
    .AddReaction("threaten",     "The guard raises an eyebrow slowly.")
    .AddReaction("attack",       "The guard's hand moves to his sword hilt.")
    .AddReaction("take",         "The guard watches your hands carefully.");
```

---

### Task 094.4: DSL parser keywords

Register `command:` and `npc_reaction:` in `AdventureDslParser`:

**`command:`**
- Split value by `,`, trim each entry, register each verb in parser config as a custom action verb.

**`npc_reaction:`**
- `npc_id` — look up NPC in registry
- `on=<trigger>` — store as trigger string (lowercase)
- `text=<message>` — reaction text
- `if=<condition>` — optional; parse using existing condition parser

---

### Task 094.4: Command execution hook

After any command executes, resolve NPC reactions in the current room:

1. Determine trigger string from command type:
   - `CustomActionCommand("blow", "trumpet")` → try `blow:trumpet`, fallback `blow`
   - `UseCommand` with target → try `use:<target_id>`, fallback `use`
   - `AttackCommand` → `attack`
   - `TakeCommand` with target → try `take:<target_id>`, fallback `take`
   - `TalkCommand` → `talk` (only if dialog produced no output)
2. For each NPC in current room, call `npc.GetReaction(trigger, state)`.
3. Append non-null reactions to the command result.

---

### Task 094.5: Tests

```csharp
[Fact] void CustomVerb_ParsedAsCustomActionCommand()
// "blow trumpet" → CustomActionCommand("blow", "trumpet")

[Fact] void NpcReaction_Fires_OnCustomVerb()
// blow trumpet → guard reaction appended

[Fact] void NpcReaction_SpecificTarget_TakesPrecedence()
// blow:trumpet fires before blow when both defined

[Fact] void NpcReaction_Respects_Condition()
// reaction only fires when condition passes

[Fact] void NpcReaction_Silent_WhenNpcNotInRoom()
// NPC in other room → no reaction

[Fact] void DslParser_Registers_CustomCommands()
// command: blow → parser accepts "blow" input

[Fact] void DslParser_Registers_NpcReaction()
// DSL round-trip: parse → reaction present on NPC
```

---

### Task 094.6: Update `api-command-reference.md`

Add `command:` and `npc_reaction:` to the DSL Parser section.

---

### Task 094.7: Sandbox demo

```adventure
command: blow, threaten

item: trumpet | golden trumpet | A battered brass trumpet. | takeable=true

npc: guard | name=Guard | state=friendly | description=A palace guard in full ceremonial dress.
npc_place: gatehouse | guard

npc_reaction: guard | on=blow:trumpet | text=The guard covers his ears. "Must you do that here?!"
npc_reaction: guard | on=blow        | text=The guard gives you a very long, very disapproving stare.
npc_reaction: guard | on=threaten    | text=The guard raises an eyebrow slowly.
npc_reaction: guard | on=attack      | text=The guard's hand moves to his sword hilt.
npc_reaction: guard | on=take        | text=The guard watches your hands carefully.
```

---

### Definition of Done

- `command:` declares custom verbs; parser accepts them without error.
- `CustomActionCommand` fires for custom verbs.
- `npc_reaction:` parses cleanly; reactions append to results when NPC is in current room.
- Specific-target triggers take precedence over general ones.
- Condition guard works.
- All tests pass.
- Sandbox demo runs.

## Completion Checklist
- [ ] `CustomActionCommand` implemented
- [ ] `NpcReaction` record and `INpc` members
- [ ] `KeywordParserConfigBuilder.AddCustomVerb(s)` implemented
- [ ] `INpc.AddReaction(trigger, text, condition?)` implemented (fluent, chainable)
- [ ] DSL parser: `command:` keyword
- [ ] DSL parser: `npc_reaction:` keyword
- [ ] Command execution hook
- [ ] Tests written and passing (C# API + DSL round-trip)
- [ ] `api-command-reference.md` updated (DSL + C# sections)
- [ ] Sandbox demo created
