## Slice 73: DSL v2 Entities, Rich Items & Start State

**Mål:** Flytta grundläggande content-authoring till v2 DSL: entity-definitioner, placering, item-fält och start-state.

**Referens:** `docs/dsl/adventure-dsl-v2-core-report.md` (4.1, 4.2, 4.13)

### Task 73.1: Entity definition + placement keywords

Nya keywords:
- `define item`
- `define key`
- `define door`
- `define npc`
- `place item`
- `place npc`

Exempel:
```adventure
define item: torch | name=old torch | desc=A dusty iron torch.
location: crypt_entrance | A cracked arch opens into darkness.
place item: crypt_entrance | torch
```

### Task 73.2: Rich item options i parser

Utöka `ApplyItemOptions(...)` med:
- `stackable`, `amount`, `presence_desc`
- `readable`, `read_text`, `require_take_to_read`, `reading_cost`
- `hidden_from_list`
- `food`, `drinkable`, `poisoned`, `heal`, `poison_damage`, `poison_turns`
- `durability=<current>/<max>`
- `prop.<key>=<value>`

### Task 73.3: Start-state keywords

Nya state keywords:
- `current_location`
- `start_inventory`
- `start_stats`
- `flag`
- `counter`
- `relationship`
- `timeline`

Exempel:
```adventure
current_location: square
start_inventory: clothes.jeans
start_inventory: coin | amount=3
start_stats: health=100 | max_health=100
flag: intro_seen=true
counter: gold=20
relationship: luna=1
```

### Task 73.4: Resolver + validation

Validera:
- `place item/npc` måste referera till befintligt rum.
- `start_inventory` måste referera till definierat item.
- `current_location` måste finnas.
- `durability` måste vara `current <= max`.

### Task 73.5: Exporter support (v2 first pass)

Uppdatera exporter för:
- utökade item options
- world-state bootstrap (`flag/counter/relationship`)
- start inventory/state där möjligt

### Task 73.6: Tests

Minimikrav:
- Parse rich item fields och verifiera modellvärden.
- `start_inventory` hamnar i inventory vid game-start.
- Invalid references ger warnings/errors enligt strict mode.

---

**Definition of Done**
- Adventure kan definiera och placera entities separat.
- Rich item metadata kan beskrivas i DSL.
- Start-state laddas utan C# bootstrap-kod.

## Completion Checklist
- [x] Core implementation for this slice is present in the engine.
- [x] Behaviour is covered by tests and/or deterministic validation paths.
- [x] Demo document: `73_DSL_v2_Entity_Bootstrap.md`.
- [x] Marked complete in project slice status.
