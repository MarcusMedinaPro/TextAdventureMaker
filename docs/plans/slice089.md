## Slice 89: Migration Tooling v1 -> v2

**Mål:** Automatisera övergången från v1-adventures till v2 world/state struktur.

**Referens:** `docs/plans/slice082.md`, `docs/dsl/adventure-dsl-v2-core-report.md` (7)

### Task 89.1: Migration command

Implementera:
- `dslhelper migrate --in old.adventure --out-world world.adventure --out-state state.start.adventure`

### Task 89.2: Mapping rules

Automatisk mapping:
- v1 location/item/key/door/exit -> v2 motsvarighet
- metadata (`world`, `goal`, `start`) bevaras
- inline item options mappas till rich schema

### Task 89.3: Migration report

Generera rapport:
- converted sections
- unresolved semantics
- manual TODO markers

### Task 89.4: Compatibility validation

Efter migration:
- parse both old and new
- jämför kärnstruktur (rooms/exits/items/doors)
- flagga regressionsrisker

### Task 89.5: Tests

Minimikrav:
- sample v1 fixtures migreras utan parse errors.
- rapport innehåller warnings där data ej kan mappas fullt.
- migrated files kan laddas av v2 parser.

---

**Definition of Done**
- V1-innehåll kan flyttas till v2 med låg manuell insats och tydlig riskrapport.
