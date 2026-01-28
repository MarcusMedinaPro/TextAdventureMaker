## Slice 34: Player Agency Tracking

**Mål:** Anpassa story till spelarstil.

### Task 34.1: IAgencyTracker — spårar meningsfulla val

```csharp
choice.Register("SaveVillage", weight: 10);
choice.Register("HelpOldMan", weight: 3);
choice.Register("StealBread", weight: -5);
```

### Task 34.2: AgencyScore påverkar story paths

```csharp
if (player.AgencyScore > 50)
    story.Unlock("player_shapes_kingdom");
else
    story.Unlock("player_follows_fate");
```

### Task 34.3: Sandbox — aktiv vs passiv protagonist

---
