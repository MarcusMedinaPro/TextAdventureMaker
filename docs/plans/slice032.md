## Slice 32: Emotional Stakes System

**Mål:** Spelaren bryr sig om saker de investerat i.

### Task 32.1: IBond — emotionell koppling till NPCs

### Task 32.2: Investment moments som bygger bond

```csharp
npc.CreateBond("childhood_friend")
    .InvestmentMoments(
        "shared_danger",
        "revealed_secret",
        "saved_life"
    )
    .Payoff(ctx => {
        if (npc.Dies())
            ctx.ImpactWeight = BondStrength * 10;
    });
```

### Task 32.3: Varning om NPC dör utan established bond

### Task 32.4: Sandbox — vän som dör efter vi byggt relation

---

## Implementation checklist (engine)
- [ ] `IBond`
- [ ] Bond investment moments + payoff
- [ ] Warnings for unearned stakes

## Example checklist (docs/examples)
- [ ] Bond system demo
