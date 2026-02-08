## Slice 19: Multi-Stage Quests

**Mål:** Quests med stages, optional objectives, failure paths.

### Task 19.1: IQuestStage — delmål

### Task 19.2: Optional vs Required objectives

### Task 19.3: Alternative completion paths

### Task 19.4: Failure consequences, hidden objectives

```csharp
quest.AddStage("find_sword")
     .RequireObjective("search_armory")
     .OptionalObjective("ask_blacksmith")
     .AlternativePath("steal_from_castle")
     .OnFailure(w => w.SpawnHostileGuards())
     .OnComplete(w => w.UnlockStage("confront_dragon"));
```

### Task 19.5: Sandbox — quest med 3 stages, optional hints

---

## Implementation checklist (engine)
- [x] `IQuestStage`
- [x] Optional vs required objectives
- [x] Alternative completion paths
- [x] Failure consequences / hidden objectives

## Example checklist (docs/examples)
- [ ] Multi-stage quest demo
