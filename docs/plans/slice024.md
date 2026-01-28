## Slice 24: Location Discovery System

**Mål:** Hidden locations som upptäcks genom exploration.

### Task 24.1: Hidden exits med discover conditions

```csharp
location.AddHiddenExit(Direction.East, secretCave)
    .DiscoverCondition(c => c.HasItem("ancient_map"))
    .Or(c => c.TalkedToNpc("old_hermit"))
    .OnDiscovery(e => e.ShowMessage("You notice a hidden passage!"));
```

### Task 24.2: Perception checks för discovery

### Task 24.3: Fog of war för stora kartor

### Task 24.4: Sandbox — hemlig grotta kräver karta eller NPC-hint

---
