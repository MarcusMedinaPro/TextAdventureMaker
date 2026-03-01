## Slice 82: DSL v2 Validator, DSLHelper CRUD & Migration

**Mål:** Ge content-teamet verktyg för att bygga och validera spel i DSL utan Core-kodändringar.

**Referens:** `docs/dsl/adventure-dsl-v2-core-report.md` (5, 8, 10)

### Task 82.1: Validator diagnostics

Inför tydliga diagnostic-koder:
- `DSLV2-REF-001` missing reference
- `DSLV2-COND-001` invalid condition
- `DSLV2-EFX-001` invalid effect
- `DSLV2-ORD-001` unreachable ordered rule

Output:
- line number
- offending line text
- message
- suggested fix

### Task 82.2: DSLHelper CRUD commands

Implementera i `TextAdventure.DSLHelper`:
- `list rooms`
- `list items [room=<id>]`
- `list npcs [room=<id>]`
- `show <entity> <id>`
- `create <entity> ...`
- `update <entity> <id> <field>=<value>`
- `delete <entity> <id>`
- `move item <item_id> to <room_id>`
- `move npc <npc_id> to <room_id>`
- `validate`
- `export`

### Task 82.3: Migration tooling v1 -> v2

Skapa migreringskommando:
- läser v1 `.adventure`
- skriver v2-struktur (world + optional state)
- markerar ej migrerade features som TODO/warnings

### Task 82.4: Authoring docs + examples

Lägg till:
- v2 cookbook med copy/paste snippets
- exempelspel med acceptance, economy, templates, quests, triggers
- checklista för release-kvalitet

### Task 82.5: CI quality gate

Inför steg i CI:
- parse + validate all `*.adventure`
- fail i strict mode på errors
- warnings rapporteras som artefakt

### Task 82.6: Tests

Minimikrav:
- validator hittar broken refs och visar rätt kod.
- CRUD round-trips bevarar DSL-innehåll.
- migration producerar körbar v2 baseline.

---

**Definition of Done**
- Teamet kan bygga, ändra och validera äventyr via DSL+tooling med minimal C#-wiring.
