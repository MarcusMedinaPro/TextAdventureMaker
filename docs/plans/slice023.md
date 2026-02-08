## Slice 23: Random Event Pool

**Mål:** Dynamiska slumpmässiga events.

### Task 23.1: IRandomEventPool

### Task 23.2: Viktning, cooldowns, context-awareness

```csharp
game.AddRandomEventPool("forest_encounters")
    .AddEvent("wolf_attack", weight: 3, cooldown: 10)
    .AddEvent("friendly_trader", weight: 5)
    .AddEvent("hidden_treasure", weight: 1)
    .RequireTimePhase(TimePhase.Night);
```

### Task 23.3: Sandbox — random encounters i skogen

---

## Implementation checklist (engine)
- [x] `IRandomEventPool`
- [x] Weighted events
- [x] Cooldowns
- [x] Conditional triggers (context-aware)

## Example checklist (docs/examples)
- [x] Random events demo (`23_Job_Interview.md`)
