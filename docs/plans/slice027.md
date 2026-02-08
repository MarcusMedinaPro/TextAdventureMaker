## Slice 27: Dynamic Description System

**Mål:** Beskrivningar som ändras baserat på context.

### Task 27.1: Context-aware descriptions

```csharp
location.Description()
    .Default("A quiet forest glade")
    .When(TimePhase.Night, "Moonlight filters through the canopy")
    .When(w => w.GetFlag("dragon_dead"), "The forest feels lighter now")
    .When(p => p.HasTrait(Trait.Observant), "You notice fresh tracks leading north")
    .FirstVisit("You've never seen trees this old");
```

### Task 27.2: Variable substitution: `{player_name}`, `{npc_emotion}`, `{item_found}`

### Task 27.3: Dialog templates med parametrar

### Task 27.4: Sandbox — rum beskrivs olika beroende på tid, quest, traits

---

## Implementation checklist (engine)
- [ ] Context-aware location descriptions
- [ ] Variable substitution in descriptions
- [ ] Dialog templates with parameters

## Example checklist (docs/examples)
- [ ] Dynamic description demo
