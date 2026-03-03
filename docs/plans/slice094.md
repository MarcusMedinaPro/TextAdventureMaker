## Slice 094: NPC Reaction DSL Keyword

**Goal:** Add `npc_reaction` as a DSL keyword so authors can define NPC responses to player commands without writing C#.

**Reference:** `docs/plans/slice093.md`

---

### Motivation

Players will try anything. `command_alias: blow=use` means `blow guard` becomes `use guard`. Without `npc_reaction`, there is no way to handle that gracefully in DSL. Authors need a simple way to attach a contextual NPC response to a command — no code required.

```adventure
npc_reaction: guard | on=use          | text=The guard gives you a very long, very disapproving stare.
npc_reaction: guard | on=use:trumpet  | text=The guard winces at the sound.
npc_reaction: guard | on=attack       | text=The guard draws his sword.
npc_reaction: guard | on=take         | text=The guard watches your hands carefully.
```

---

### DSL Syntax

```
npc_reaction: <npc_id> | on=<trigger> | text=<message>
npc_reaction: <npc_id> | on=<trigger> | text=<message> | if=<condition>
```

**Trigger formats:**

| Trigger | Fires when... |
| --- | --- |
| `use` | Player uses any item while the NPC is in the current room |
| `use:<item_id>` | Player uses a specific item |
| `attack` | Player attempts to attack the NPC |
| `take` | Player takes any item in the current room |
| `take:<item_id>` | Player takes a specific item |
| `talk` | Player talks to the NPC (overridden by dialog tree if present) |
| `blow` | (or any aliased verb) — resolved via `command_alias` before matching |

**Optional condition:**

```adventure
npc_reaction: guard | on=use:trumpet | text=He covers his ears. | if=flag:guard_sleeping=false
```

---

### Task 094.1: NpcReaction model

Add to `INpc` / `Npc`:

```csharp
public sealed record NpcReaction(string Trigger, string Text, Func<IGameState, bool>? Condition = null);

IReadOnlyList<NpcReaction> Reactions { get; }
INpc AddReaction(string trigger, string text, Func<IGameState, bool>? condition = null);
string? GetReaction(string trigger, IGameState state);
```

`GetReaction` returns the first matching reaction text where trigger matches and condition passes (or null).

---

### Task 094.2: DSL parser keyword

Register `npc_reaction` in `AdventureDslParser`:

```
npc_reaction: guard | on=use:trumpet | text=The guard winces. | if=flag:alert=false
```

Parse:
- `npc_id` — look up NPC in registry
- `on=<trigger>` — store as trigger string (lowercase)
- `text=<message>` — reaction text
- `if=<condition>` — optional; parse using existing condition parser

---

### Task 094.3: Command execution hook

After a command executes successfully, check whether the current room has NPCs with a matching reaction and append the reaction text to the result.

Trigger resolution:
- `use` matches any `UseCommand`
- `use:<item_id>` matches `UseCommand` where target matches item id/alias
- `attack` matches `AttackCommand`
- `take` / `take:<item_id>` matches `TakeCommand`
- `talk` matches `TalkCommand` (only when no dialog tree produces output)

---

### Task 094.4: Tests

```csharp
[Fact]
void NpcReaction_Fires_OnUse()
// use any item → guard reaction text appended to result

[Fact]
void NpcReaction_Fires_OnSpecificItem()
// use trumpet → specific reaction; use sword → no reaction

[Fact]
void NpcReaction_Respects_Condition()
// reaction only fires when condition passes

[Fact]
void NpcReaction_Silent_WhenNpcNotInRoom()
// NPC in other room → no reaction

[Fact]
void DslParser_Registers_NpcReaction()
// DSL round-trip: parse → reaction present on NPC
```

---

### Task 094.5: Update api-command-reference.md

Add `npc_reaction` to the DSL Parser section.

---

### Task 094.6: Sandbox demo

```adventure
item: trumpet | golden trumpet | A battered brass trumpet. | takeable=true
command_alias: blow=use

npc: guard | name=Guard | state=friendly | description=A bored-looking guard.
npc_place: gatehouse | guard

npc_reaction: guard | on=use:trumpet  | text=The guard covers his ears. "Must you?!"
npc_reaction: guard | on=use          | text=The guard gives you a very long, very disapproving stare.
npc_reaction: guard | on=attack       | text=The guard's hand moves to his sword hilt.
npc_reaction: guard | on=take         | text=The guard watches your hands carefully.
```

---

### Definition of Done

- `npc_reaction` parses cleanly from DSL.
- Reactions append to command results when NPC is in current room.
- Condition guard works.
- Specific-item triggers (`use:trumpet`) take precedence over general (`use`).
- All tests pass.
- Demo adventure uses it.

## Completion Checklist
- [ ] `NpcReaction` record and `INpc` members
- [ ] DSL parser keyword registered
- [ ] Command execution hook
- [ ] Tests written and passing
- [ ] `api-command-reference.md` updated
- [ ] Sandbox demo created
