## Slice 86: Condition/Effect Runtime Engine Hardening

**Mål:** Göra v2 condition/effect-körning robust, snabb och säker.

**Referens:** `docs/plans/slice072.md` (AST foundation), `docs/dsl/adventure-dsl-v2-core-report.md`

### Task 86.1: Compile AST to delegates

Kompilera parsed condition/effect AST till cacheade delegates:
- minskar runtime parse-overhead
- enhetliga evaluator hooks

### Task 86.2: Deterministic effect execution

Regler:
- effects körs i deklarerad ordning
- tydlig policy för stop-on-failure vs continue-on-failure
- samla runtime diagnostics

### Task 86.3: Execution context boundaries

Skapa `DslExecutionContext` som explicit kapslar:
- `IGameState`
- current actor/location
- command metadata (optional)

### Task 86.4: Guard rails

Lägg till skydd mot:
- recursive trigger loops
- cyclic effect chains
- runaway schedule/trigger storms

### Task 86.5: Tests

Minimikrav:
- compile cache används vid upprepade villkor.
- effect order är deterministisk.
- loop guards stoppar självrefererande regler.

---

**Definition of Done**
- V2 rules engine är stabil och produktionssäker för större spel.

## Completion Checklist
- [x] Core implementation for this slice is present in the engine.
- [x] Behaviour is covered by tests and/or deterministic validation paths.
- [x] Demo document: `86_DSL_v2_Rule_Engine_Hardening.md`.
- [x] Marked complete in project slice status.
