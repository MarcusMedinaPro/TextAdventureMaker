## Slice 85: DSL v2 Exporter & Round-Trip Fidelity

**Mål:** Exportera world/state i v2-format och stödja stabil parse-export-parse round-trip.

**Referens:** `docs/dsl/adventure-dsl-v2-core-report.md` (7, 10)

### Task 85.1: World exporter v2

Utöka `AdventureDslExporter` med:
- rich item options
- NPC declarations
- quests/rules/triggers
- door/exit extensions

### Task 85.2: State exporter v2

Stöd export till state DSL:
- `current_location`
- inventory + amounts/durability
- flags/counters/relationships/timeline
- relevant runtime snapshots

### Task 85.3: Stable formatting rules

Inför deterministisk ordering:
- entities sorterade per id
- options i fast ordning
- predictable whitespace/escaping

### Task 85.4: Import-export compatibility tests

Golden tests:
- parse -> export -> parse = semantiskt ekvivalent state
- exporter genererar inga okända keywords i strict mode

### Task 85.5: Backward compatibility

Behåll möjlighet att exportera v1-minimal format för legacy användning.

---

**Definition of Done**
- V2 exporter producerar läsbara och stabila filer som går att round-trippa.

## Completion Checklist
- [x] Core implementation for this slice is present in the engine.
- [x] Behaviour is covered by tests and/or deterministic validation paths.
- [x] Demo document: `85_DSL_v2_Roundtrip_Export.md`.
- [x] Marked complete in project slice status.
