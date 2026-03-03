## Slice 80: Event, Schedule & Random Automation DSL

**Mål:** Förflytta event-driven automation från C# wiring till DSL.

**Referens:** `docs/dsl/adventure-dsl-v2-core-report.md` (4.11, 4.12)

### Task 80.1: Generic trigger keywords

Nya keywords:
- `on_enter`
- `on_pickup`
- `on_talk`
- `on_action`
- `on_npc_death`
- `on_tick`

Varje keyword stödjer:
- `if=<condition>` (optional)
- `effects=<effect_list>`

### Task 80.2: Schedule keywords

Nya keywords:
- `schedule_at: <tick> | effects=...`
- `schedule_every: <ticks> | effects=...`
- `schedule_when: <condition> | effects=...`

Bind till `ScheduleQueue`.

### Task 80.3: Random event keywords

Nya keywords:
- `random_settings: enabled=true|false | chance=<0..1>`
- `random_event: <id> | weight=<int> | cooldown=<ticks> | if=<condition> | effects=...`

Bind till `RandomEventPool`.

### Task 80.4: Effect executor

Utöka gemensam effect executor med:
- `spawn_item:<item_id>:<location_id>`
- `spawn_npc:<npc_id>:<location_id>`
- `open_door:<door_id>`
- `move_npc:<npc_id>:<location_id>`
- `message:<text>`

### Task 80.5: Tests

Minimikrav:
- `on_enter` kör effect exakt när location nås.
- schedule `at/every/when` kör vid rätt ticks.
- random events följer condition + cooldown.

---

**Definition of Done**
- Event/schedule/random automation kan definieras i DSL och exekveras av Core-systemen.

## Completion Checklist
- [x] Core implementation for this slice is present in the engine.
- [x] Behaviour is covered by tests and/or deterministic validation paths.
- [x] Demo document: `80_DSL_v2_Event_Automation.md`.
- [x] Marked complete in project slice status.
