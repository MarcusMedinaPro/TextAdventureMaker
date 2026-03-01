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

## Implementation checklist (engine)
- [x] `IWorldState` interface
- [x] Flags (bool), Counters (int), Relationships (int), Timeline
- [x] `WorldState` implementation
- [x] Quest/Event conditions based on WorldState

## Example checklist (docs/examples)
- [x] Flags for branching choices (`09_Pre-Date.md`)
- [x] Counters/relationships/timeline shown (`04-10_Forest_Adventure_Core.md`)
