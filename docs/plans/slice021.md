## Slice 21: Time System

**Mål:** Dag/natt cycles, tidbaserade events.

### Task 21.1: ITimeSystem — ticks, dagar, faser

### Task 21.2: TimeOfDay: Dawn, Day, Dusk, Night

### Task 21.3: Dag/natt påverkar:

- NPC-platser (shopkeeper hem på natten)
- Events (varulvar spawnar i fullmåne)
- Lighting (fackla behövs i mörka grottor)

```csharp
game.UseTimeSystem()
    .SetStartTime(TimeOfDay.Dawn)
    .TicksPerDay(100)
    .OnPhase(TimePhase.Night, ctx => ctx.SetVisibility(0.3f));
```

### Task 21.4: Move/Turn Limits

```csharp
// Global drag-begränsning (hela spelet)
game.UseTimeSystem()
    .MaxMoves(400)
    .OnMovesRemaining(50, ctx => ctx.ShowWarning("Time is running out!"))
    .OnMovesRemaining(10, ctx => ctx.SetMood(Mood.Desperate))
    .OnMovesExhausted(ctx => ctx.GameOver("You ran out of time."));

// Lokal drag-begränsning (puzzle/sektion)
var bombPuzzle = game.CreateTimedChallenge("defuse_bomb")
    .MaxMoves(30)
    .OnStart(ctx => ctx.ShowMessage("The bomb will explode in 30 moves!"))
    .OnMovesRemaining(10, ctx => ctx.ShowMessage("10 moves left!"))
    .OnSuccess(ctx => ctx.Reward("bomb_defused"))
    .OnFailure(ctx => ctx.Explode());

// Aktivera när spelaren hittar bomben
room.OnEnter(ctx => bombPuzzle.Start());

// Kolla status
if (game.MovesRemaining < 100) { ... }
if (bombPuzzle.IsActive && bombPuzzle.MovesRemaining < 5) { ... }
```

**Features:**

- Globalt: `game.MaxMoves(400)` - hela spelet
- Lokalt: `CreateTimedChallenge()` - specifik puzzle
- Warnings vid trösklar
- Olika consequences vid timeout

### Task 21.5: Sandbox — butik stängd på natten, monster spawnar, bomb puzzle med 30 drag

---

## Implementation checklist (engine)
- [x] `ITimeSystem` + `TimeSystem`
- [x] `TimeOfDay` phases (Dawn/Day/Dusk/Night)
- [x] Phase handlers (`OnPhase`)
- [x] Global move limits + warnings (`OnMovesRemaining`, `OnMovesExhausted`)
- [x] `TimedChallenge` (local move limit)

## Example checklist (docs/examples)
- [x] Time phases + move warnings (`21_The_Bank_Errand.md`)
- [ ] Timed challenge (bomb/puzzle) demo
