## Slice 35: Dramatic Irony Tracker

**Mål:** Spänning när spelaren vet mer än karaktären.

### Task 35.1: IDramaticIronySystem — spårar kunskapsskillnader

```csharp
// Player learns:
player.LearnSecret("advisor_is_traitor");

// NPC doesn't know:
king.Dialog("My advisor is my most trusted friend");

// System detects irony:
if (dramaticIrony.Exists())
    player.ReceiveAction("warn_king");

// Consequences:
if (!player.WarnedKing())
    story.Consequence(Tragedy.KingBetrayed);
```

### Task 35.2: Ge spelaren chans att agera på kunskap

### Task 35.3: Sandbox — förrädarscenario

---

## Implementation checklist (engine)
- [ ] `IDramaticIronySystem`
- [ ] Knowledge gap detection
- [ ] Player actions based on secrets

## Example checklist (docs/examples)
- [ ] Dramatic irony demo
