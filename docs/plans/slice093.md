## Slice 93: Post-GA Governance & v2.x Backlog

**Mål:** Säkerställa att DSL v2 fortsätter utvecklas kontrollerat efter GA.

**Referens:** `docs/plans/slice092.md`

### Task 93.1: Governance model

Etablera:
- owner for DSL schema
- change review process
- RFC-template för nya keywords

### Task 93.2: Compatibility contract tests

Inför långsiktiga contract tests:
- gamla v2 files ska fortsätta parse:a
- diagnostic codes ska vara stabila
- exporter output ska vara backward safe

### Task 93.3: v2.x backlog seed

Skapa prioriterad backlog för:
- richer chapter DSL
- advanced NPC bonds/arcs
- enhanced tooling UX
- optional visual DSL editor integrations

### Task 93.4: Metrics and feedback loop

Följ upp:
- migration completion rate
- top diagnostics by frequency
- authoring pain points från teamet

### Task 93.5: Quarterly schema cadence

Definiera återkommande schemafönster:
- minor DSL additions per quarter
- patch-only bugfix windows

---

**Definition of Done**
- DSL v2 har en hållbar förvaltningsmodell och tydlig v2.x roadmap.

## Completion Checklist
- [x] Core implementation for this slice is present in the engine.
- [x] Behaviour is covered by tests and/or deterministic validation paths.
- [x] Demo document: `93_DSL_v2_Governance_Roadmap.md`.
- [x] Marked complete in project slice status.
