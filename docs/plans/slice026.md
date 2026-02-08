## Slice 26: Mood & Atmosphere System

**Mål:** Stämning som påverkar spelarupplevelsen.

### Task 26.1: IMoodSystem — atmospheric state

### Task 26.2: Mood enum: Peaceful, Tense, Foreboding, Terrifying, Hopeful

### Task 26.3: Environmental cues: sound, smell, temperature, wind

```csharp
location.SetMood(Mood.Foreboding)
    .WithLighting(LightLevel.Dim)
    .WithAmbientSound("distant_dripping")
    .WithSmell("damp earth and decay");
```

### Task 26.4: Mood-modifiers på beskrivningar

```csharp
// Normal: "A cave entrance"
// Foreboding: "A yawning cave entrance, shadows writhing within"
```

### Task 26.5: Mood propagation (angränsande rum påverkar varandra)

### Task 26.6: Sandbox — grotta med ökande skräck ju djupare man går

---

## Implementation checklist (engine)
- [x] `IMoodSystem`
- [x] `Mood` enum
- [x] Environmental cues (lighting/sound/smell/temperature)
- [ ] Mood-modified descriptions
- [x] Mood propagation

## Example checklist (docs/examples)
- [ ] Mood system demo
