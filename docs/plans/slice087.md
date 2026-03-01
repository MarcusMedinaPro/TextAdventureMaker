## Slice 87: Save Snapshot Completeness

**Mål:** Säkerställa att DSL-save faktiskt fångar allt spelkritiskt runtime-state.

**Referens:** `docs/plans/slice081.md` (two-file architecture)

### Task 87.1: Snapshot schema coverage

Utöka state/save DSL för:
- door states
- hidden exit discovery state
- npc location/state/health
- quest stage progress
- story/chapter progression
- time tick/phase

### Task 87.2: System runtime fields

Spara och ladda:
- random event cooldown/last triggered
- schedule cursor om relevant
- active poison/status effects

### Task 87.3: Versioned save metadata

Nya fields:
- `save_version`
- `dsl_version`
- `created_at`

Inför enkel migrationshook för äldre saves.

### Task 87.4: Integrity checks

Validera vid load:
- referenser i save pekar på existerande world entities
- impossible states normaliseras eller ger tydligt fel

### Task 87.5: Tests

Minimikrav:
- gameplay-resume efter save/load återställer samma logiska state.
- save med saknad entity ger begriplig diagnostic.
- äldre save-version hanteras via migration path.

---

**Definition of Done**
- Save DSL är tillräckligt komplett för verklig fortsättning av spel.
