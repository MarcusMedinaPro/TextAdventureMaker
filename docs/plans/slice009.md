## Slice 9: World State System

**Mål:** Centralt state för att spåra global världsstatus. Foundation för quests, events, stories.

### Task 9.1: IWorldState interface

- Flags: `bool` (isDragonDead, isKingdomAtWar)
- Counters: `int` (villagersSaved, daysElapsed)
- Relationships: NPC-attityd (-100 till +100)
- Timeline: kronologiska händelser

### Task 9.2: WorldState implementation

```csharp
worldState.SetFlag("dragon_defeated", true);
worldState.Increment("reputation", 50);
worldState.SetRelationship("blacksmith", 75);
```

### Task 9.3: Quest/Event conditions mot WorldState

### Task 9.4: Sandbox — villagers räknar, reputation påverkar NPC-dialog

---
