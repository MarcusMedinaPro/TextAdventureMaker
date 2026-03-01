## Slice 22: Faction & Reputation System

**Mål:** NPC-grupper med gemensam reputation.

### Task 22.1: IFaction — grupper av NPCs

### Task 22.2: Reputation thresholds → unlock/consequences

```csharp
game.AddFaction("thieves_guild")
    .WithNpcs("shadow", "pickpocket", "fence")
    .OnReputationThreshold(50, unlock: "guild_headquarters")
    .OnReputationThreshold(-50, consequence: w => w.SpawnHitSquad());

player.ModifyReputation("thieves_guild", +20);
```

### Task 22.3: Faction påverkar priser, locations, encounters

### Task 22.4: Sandbox — två factions, val påverkar ending

---

## Implementation checklist (engine)
- [x] `IFaction` + `Faction`
- [x] `IFactionSystem` + `FactionSystem`
- [x] Reputation thresholds with callbacks

## Example checklist (docs/examples)
- [x] Two factions + reputation effects (`22_Street_Robbery.md`)
