## Slice 92: DSL v2 Release, Versioning & Deprecation

**Mål:** Förbereda v2 för stabil release med tydlig kompatibilitetspolicy.

**Referens:** `docs/plans/slice090.md`, `docs/plans/slice091.md`

### Task 92.1: Versioning policy

Fastställ:
- `dsl_version` semantics
- compatibility matrix (`engine` x `dsl_version`)
- strict-mode defaults per version

### Task 92.2: Feature flags lifecycle

Definiera:
- experimental -> beta -> stable flags
- removal policy för legacy toggles

### Task 92.3: Documentation pack

Publicera:
- v2 spec
- migration guide
- troubleshooting guide for diagnostics

### Task 92.4: Release checklist

Måste vara grönt före release:
- all fixtures/CI gates
- demo playthrough verified
- migration tool validated on representative v1 files

### Task 92.5: Deprecation timeline

Planera:
- när v1-only syntax blir legacy
- hur länge dual-mode support hålls
- communication notes till användare

---

**Definition of Done**
- DSL v2 kan släppas med tydlig support- och migrationsstrategi.

## Completion Checklist
- [x] Core implementation for this slice is present in the engine.
- [x] Behaviour is covered by tests and/or deterministic validation paths.
- [x] Demo document: `92_DSL_v2_Release_Readiness.md`.
- [x] Marked complete in project slice status.
