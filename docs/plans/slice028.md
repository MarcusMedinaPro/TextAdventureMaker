## Slice 28: Character Arc Tracking

**Mål:** NPCs utvecklas över tid.

### Task 28.1: ICharacterArc — definierar utveckling

### Task 28.2: Milestones som unlocks traits

```csharp
npc.DefineArc("CowardToHero")
    .StartState(Trait.Fearful)
    .Milestone(1, "witness_courage", unlocks: Trait.Hopeful)
    .Milestone(2, "own_brave_act", unlocks: Trait.Brave)
    .EndState(Trait.Heroic)
    .OnComplete(ctx => ctx.UnlockQuest("lead_rebellion"));
```

### Task 28.3: Dialog ändras automatiskt baserat på arc-progress

### Task 28.4: Sandbox — NPC växer från feg till hjälte

---
