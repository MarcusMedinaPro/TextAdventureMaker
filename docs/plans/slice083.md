## Slice 83: Story Branches & Chapter DSL

**Mål:** Göra `StoryBranch` och `ChapterSystem` konfigurerbara via DSL v2.

**Referens:** `docs/dsl/adventure-dsl-v2-core-report.md` (4.14)

### Task 83.1: Story branch keywords

Nya keywords:
- `branch`
- `branch_when`
- `branch_then`

Exempel:
```adventure
branch: trust_fox
branch_when: trust_fox | relationship:fox>=2
branch_then: trust_fox | effects=set_flag:fox_trusted:true
```

### Task 83.2: Chapter keywords

Nya keywords:
- `chapter`
- `chapter_objective`
- `chapter_next`
- `chapter_end`
- `chapter_unlock_if`

### Task 83.3: Resolver mot Core-modeller

Bind till:
- `StoryState.AddBranch(...)`
- `ChapterSystem`
- `Chapter`, `ChapterObjective`, `ChapterEnding`

### Task 83.4: Validation

Validera:
- branch/chapter ids måste vara unika.
- `chapter_next` target måste finnas.
- objective ids får inte dupliceras inom kapitel.

### Task 83.5: Tests

Minimikrav:
- branch condition triggar consequences.
- chapter unlock/advance fungerar med DSL-data.
- invalid chapter links fångas av validator.

---

**Definition of Done**
- Story branches och chapter flow kan byggas utan C# wiring.

## Completion Checklist
- [x] Core implementation for this slice is present in the engine.
- [x] Behaviour is covered by tests and/or deterministic validation paths.
- [x] Demo document: `83_DSL_v2_Story_Chapters.md`.
- [x] Marked complete in project slice status.
