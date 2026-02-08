## Slice 2: Doors + Keys

**Mål:** Dörrar som blockerar utgångar, kräver nycklar.

### Task 2.1: IDoor + Door (State: Open, Closed, Locked, Destroyed) ✅

### Task 2.2: IKey + Key ✅

### Task 2.3: Door events: OnOpen, OnClose, OnLock, OnUnlock, OnDestroy

### Task 2.4: Location.AddExit with door ✅

### Task 2.5: Sandbox — låst dörr till skattkammaren, hitta nyckel ✅

---

## Implementation checklist (engine)
- [x] `IDoor` with state, reactions, aliases, and key requirement
- [x] `Door` model with states: Open/Closed/Locked/Destroyed
- [x] `DoorAction` + `DoorState` enums
- [x] Door events: `OnOpen`, `OnClose`, `OnLock`, `OnUnlock`, `OnDestroy`
- [x] `IKey` + `Key` model
- [x] `Location.AddExit(direction, target, door, oneWay)`
- [x] `OpenCommand` supports door open
- [x] `UnlockCommand` supports door unlock with key
- [ ] Commands for door `Close`, `Lock`, `Destroy`

## Example checklist (docs/examples)
- [x] Locked door gating progress + key required (`02_The_Locked_Drawer.md`)
- [x] Door state transitions (open/close/lock/unlock/destroy) (`02_Door_States.md`)
- [x] Door reactions for success/failure (`02_The_Locked_Drawer.md`, `02_Door_States.md`)
- [ ] Door events demonstrated (OnOpen/OnClose/OnLock/OnUnlock/OnDestroy)
