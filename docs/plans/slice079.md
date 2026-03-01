## Slice 79: Quest DSL & Condition Graph

**Mål:** Definiera quests, objective-flöden och villkorslogik helt i DSL.

**Referens:** `docs/dsl/adventure-dsl-v2-core-report.md` (4.10)

### Task 79.1: Quest basics

Nya keywords:
- `quest`
- `quest_stage`
- `quest_objective`

Exempel:
```adventure
quest: dragon_hunt | title=Dragon Hunt | desc=Find the sword and defeat the dragon. | state=active
quest_stage: dragon_hunt | id=prepare | required=find_sword,reach_cave | optional=read_tome
```

### Task 79.2: Condition grammar for quests

Nytt keyword:
- `quest_condition: <quest_id> | <condition_expr>`

Stöd:
- `has_item:<id>`
- `flag:<key>=<bool>`
- `counter:<key>>=<n>`
- `npc_state:<npc_id>=<state>`
- `relationship:<npc_id>>=<n>`
- `all(...)`, `any(...)`

### Task 79.3: Quest lifecycle effects

Nya keywords:
- `quest_on_complete`
- `quest_on_fail`

Effects återanvänder gemensam effect grammar.

### Task 79.4: Resolver & validation

Validera:
- quest/stage/objective ids unika per scope.
- referenser till npc/item/flags är giltiga.
- condition expressions parserbara och typ-korrekta.

### Task 79.5: Tests

Minimikrav:
- quest aktiveras från DSL.
- conditions evalueras korrekt via `QuestConditionEvaluator`.
- on-complete effects körs.

---

**Definition of Done**
- Quests med condition-graf och stage/objective-flöde fungerar via DSL.
