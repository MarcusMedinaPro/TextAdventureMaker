## Slice 81: Two-File DSL Architecture + Save Snapshots

**Mål:** Stödja två separata DSL-filer: world-definition och state/save-snapshot.

**Referens:** `docs/dsl/adventure-dsl-v2-core-report.md` + teambeslut (separata DSL-filer)

### Task 81.1: File roles

Definiera:
- `world.adventure` (statisk värld)
- `state.start.adventure` (start-state)
- `save.slotX.adventure` (runtime snapshot)

### Task 81.2: State DSL parser

Skapa separat parser eller mode för:
- `current_location`
- `start_inventory`/`inventory`
- `stats`
- `flag`, `counter`, `relationship`, `timeline`
- event/system runtime states (där rimligt)

### Task 81.3: Save exporter/importer

Nya APIs:
- `ExportStateToDsl(GameState state)`
- `LoadStateFromDsl(GameState state, string dsl)`

Krav:
- läsbar textfil
- stabil round-trip för centralt state

### Task 81.4: Load order

Laddningsordning:
1. parse world DSL
2. apply start/save state DSL
3. resolve references
4. validate

### Task 81.5: Compatibility

V1-spel med en fil ska fortfarande fungera.

### Task 81.6: Tests

Minimikrav:
- world + state filer ger förväntad game start.
- save export -> import återskapar inventory/location/world counters.
- saknad optional state-fil påverkar inte world parse.

---

**Definition of Done**
- Två-filsladdning fungerar.
- DSL kan användas som läsbar savefile utan att ersätta v1-stöd.
