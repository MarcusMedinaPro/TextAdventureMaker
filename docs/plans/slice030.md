## Slice 30: Foreshadowing & Callbacks

**Mål:** Chekov's Gun — plantera och betala av.

### Task 30.1: IForeshadowingSystem — spårar planted seeds

### Task 30.2: Tags/connections mellan seemingly unrelated things

```csharp
// Act 1: Plant
location.AddDetail("ancient_runes")
    .Description("Strange symbols, unreadable...")
    .Tag(Foreshadowing.DragonLanguage);

// Act 2: Hint
npc.Dialog("Those runes? Dragon script. Bad omen.")
    .LinksTo(Foreshadowing.DragonLanguage);

// Act 3: Payoff
boss.OnDefeat(ctx => {
    if (ctx.PlayerHasSeenDetail("ancient_runes"))
        ctx.ShowEpilogue("The runes now glow—a warning you understood too late.");
});
```

### Task 30.3: Payoff detection (varning om planted gun aldrig fires)

### Task 30.4: Optional callbacks (om spelaren missade hint)

### Task 30.5: Sandbox — mystiska runer som får mening senare

---
