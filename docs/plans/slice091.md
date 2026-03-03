## Slice 91: Demo Adventure in Pure DSL v2

**Mål:** Leverera ett spelbart demo som använder DSL v2 end-to-end istället för Core hardcoding.

**Referens:** `docs/dsl/adventure-dsl-v2-core-report.md`, `docs/plans/slice081.md`

### Task 91.1: Demo world/state files

Skapa demo med:
- `world.adventure`
- `state.start.adventure`

Krav:
- British English all text output
- visar item rules, NPC acceptance, quests, events och shop-regler

### Task 91.2: Narrative arc setup

Bygg demo med tre spelroller i separata chapters:
1. James/Augustine (phase 1)
2. James/Augustine (phase 2)
3. Betty (phase 3)

### Task 91.3: Acceptance/economy/template showcase

Demot ska demonstrera:
- `npc_acceptance` ladder
- dynamic interpolation (`{inventory...}`)
- buy rules med `counter.gold`

### Task 91.4: Playthrough scripts

Lägg till testscript:
- happy path
- low-reputation path
- low-money path

### Task 91.5: Validation + docs

Krav:
- demo passerar strict validation
- README med hur spelet startas i DSL-läge

---

**Definition of Done**
- Ett referensspel finns som bevis på att v2 DSL räcker för majoriteten av game logic.

## Completion Checklist
- [x] Core implementation for this slice is present in the engine.
- [x] Behaviour is covered by tests and/or deterministic validation paths.
- [x] Demo document: `91_DSL_v2_Full_Demo_Adventure.md`.
- [x] Marked complete in project slice status.
