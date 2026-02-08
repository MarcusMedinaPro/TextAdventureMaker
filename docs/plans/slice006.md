## Slice 6: Event System (Observer)

**Mål:** Triggers när saker händer.

### Task 6.1: IEventSystem + EventSystem (Observer) ✅

### Task 6.2: Inbyggda events (OnEnter, OnExit, OnPickup, OnDrop, OnTalk, OnCombatStart) ✅

### Task 6.3: Item/Door events kopplas till EventSystem ✅

### Task 6.4: Sandbox — drake vaknar när man går in i grottan ✅

---

## Implementation checklist (engine)
- [x] `IEventSystem` + `EventSystem`
- [x] `GameEvent` + `GameEventType`
- [x] Built-in events: Enter/Exit location, Pickup/Drop item, Talk, CombatStart
- [x] Door events published (Open/Unlock)
- [x] Item/Door events wired into EventSystem

## Example checklist (docs/examples)
- [x] Subscribe to enter/exit events (`06_The_Key_Under_the_Stone.md`)
- [x] React to door open/unlock events (`06_The_Key_Under_the_Stone.md`)
