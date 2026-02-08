## Slice 10: Save/Load (Memento)

**Mål:** Spara och ladda spelstatus.

### Task 10.1: IMemento + GameMemento

### Task 10.2: ISaveSystem + JsonSaveSystem

### Task 10.3: SaveCommand, LoadCommand

### Task 10.4: Sandbox — save, quit, load, continue

---

## Implementation checklist (engine)
- [x] `IMemento` + `GameMemento`
- [x] `ISaveSystem` + `JsonSaveSystem`
- [x] `GameState.CreateMemento()` + `ApplyMemento(...)`
- [x] `SaveCommand` + `LoadCommand`

## Example checklist (docs/examples)
- [x] Save/load flow (`10_The_Warm_Library.md`)
- [x] Save/load included in core demo (`04-10_Forest_Adventure_Core.md`)
