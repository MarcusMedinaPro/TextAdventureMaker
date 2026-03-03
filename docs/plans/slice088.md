## Slice 88: DSLHelper Advanced CRUD & Refactoring Workflows

**Mål:** Göra `TextAdventure.DSLHelper` till ett praktiskt authoring-verktyg för stora DSL-filer.

**Referens:** `docs/plans/slice082.md`, `docs/dsl/adventure-dsl-v2-core-report.md` (5)

### Task 88.1: Advanced entity refactors

Nya DSLHelper-operationer:
- `rename <entity> <old_id> <new_id>` (med reference updates)
- `copy <entity> <id> to <new_id>`
- `extract item-template <prefix>`

### Task 88.2: Batch operations

Stöd:
- apply command against filtered set
- script file mode (`dslhelper run commands.txt`)
- dry-run med diff preview

### Task 88.3: Safety features

Krav:
- atomic write (temp file + replace)
- backup option (`--backup`)
- lint-before-write i strict mode

### Task 88.4: Query/inspect improvements

Utöka list/show:
- `list rules`
- `list quests`
- `list broken-refs`
- `graph room-exits`

### Task 88.5: Tests

Minimikrav:
- rename uppdaterar alla referenser.
- dry-run ändrar inte filer.
- batch script kan köras idempotent.

---

**Definition of Done**
- DSLHelper stödjer både CRUD och säkra massändringar i content pipelines.

## Completion Checklist
- [x] Core implementation for this slice is present in the engine.
- [x] Behaviour is covered by tests and/or deterministic validation paths.
- [x] Demo document: `88_DSLHelper_Refactor_Workflows.md`.
- [x] Marked complete in project slice status.
