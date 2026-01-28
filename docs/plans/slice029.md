## Slice 29: Pacing & Tension System

**Mål:** Balans mellan action och lugn.

### Task 29.1: ITensionMeter — current tension (0.0 - 1.0)

### Task 29.2: Tension modifiers från events

```csharp
story.DefineTension()
    .BuildUp("dragon_approach", rate: 2.0f)
    .Peak("dragon_fight")
    .Release("dragon_dead", cooldown: 50);
```

### Task 29.3: Tension påverkar:

- Random encounter frequency
- Music/sound intensity
- Available actions

### Task 29.4: Rest periods (safe zones)

### Task 29.5: Pacing rules: "no major events within X ticks"

### Task 29.6: Sandbox — tension bygger mot dragon fight

---
