## Slice 18: Story Branches & Consequences

**Mål:** Hantera storylines baserat på spelarval.

### Task 18.1: IStoryBranch + IConsequence interfaces

### Task 18.2: StoryState — aktiva/avslutade grenar

### Task 18.3: Branching conditions

```csharp
game.AddStoryBranch("dragon_path")
    .Condition(q => q.IsQuestComplete("slay_dragon"))
    .Consequence(w => w.UnlockLocation("dragon_lair_treasure"))
    .Consequence(w => w.SetNpcState("king", NpcState.Grateful));
```

### Task 18.4: Sandbox — två endings baserat på val

---
