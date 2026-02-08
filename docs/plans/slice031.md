## Slice 31: Scene Transitions & Beats

**Mål:** Stories flödar mellan scenes, inte bara locations.

### Task 31.1: IScene — orkestrerar events

### Task 31.2: Scene beats (dialog/action i ordning)

```csharp
story.DefineScene("Betrayal")
    .Location("throne_room")
    .Participants("king", "advisor", "player")
    .Beat(1, "king_announces_quest")
    .Beat(2, "advisor_whispers_doubt")
    .Beat(3, "reveal_advisor_is_traitor")
    .Transition()
        .To("Escape").Trigger("player_flees")
        .Or()
        .To("Arrest").Trigger("player_trusts_advisor");

scene.Play();  // All dialog/events körs automatiskt
```

### Task 31.3: Scene transitions baserat på player actions

### Task 31.4: Sandbox — betrayal scene med två utgångar

---

## Implementation checklist (engine)
- [ ] `IScene`
- [ ] Scene beats + scripted playback
- [ ] Transitions based on player actions

## Example checklist (docs/examples)
- [ ] Scene transitions demo
