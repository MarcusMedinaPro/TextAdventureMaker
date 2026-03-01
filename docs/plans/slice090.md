## Slice 90: DSL v2 Quality Gates & Fixture Corpus

**Mål:** Införa stark test/valideringsdisciplin för DSL v2 i CI.

**Referens:** `docs/plans/slice082.md` (validator), `docs/plans/slice089.md` (migration)

### Task 90.1: Fixture repository

Skapa organiserad fixture-struktur:
- `fixtures/v1-valid`
- `fixtures/v2-valid`
- `fixtures/v2-invalid`
- `fixtures/migration`

### Task 90.2: Golden diagnostics

För invalid fixtures:
- expected diagnostic codes
- expected line numbers
- expected suggested fixes

### Task 90.3: CI pipeline stage

Nytt steg:
- parse + validate all fixtures
- strict mode on v2 fixtures
- fail build på unexpected diagnostics

### Task 90.4: Performance smoke tests

Mät:
- parse time for large DSL
- validation time
- expression/template resolver throughput

### Task 90.5: Regression tracking

Publicera artefakter:
- diagnostics summary
- parse performance trend
- migration success rate

---

**Definition of Done**
- DSL v2 har robust quality gate med reproducerbara fixtures och diagnostics.
