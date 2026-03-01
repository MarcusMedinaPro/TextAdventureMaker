## Slice 8: Quest System

**Mål:** Objectives och progress.

### Task 8.1: IQuest + Quest (State pattern)

### Task 8.2: Quest conditions (Visitor)

### Task 8.3: Sandbox — "Hitta svärdet och döda draken"

---

## Implementation checklist (engine)
- [x] `IQuest` + `Quest` with state (Inactive/Active/Completed/Failed)
- [x] `IQuestLog` + `QuestLog`
- [x] Quest conditions + visitor (`IQuestCondition`, `IQuestConditionVisitor`)
- [x] Built-in conditions (world flags/counters, has item, NPC state, all/any)
- [x] `QuestCommand`

## Example checklist (docs/examples)
- [x] Quest log updates + conditions (`08_The_Forgotten_Password.md`)
- [x] Quest completion reactions (`08_The_Forgotten_Password.md`)
- [x] Quest in core demo (`04-10_Forest_Adventure_Core.md`)
