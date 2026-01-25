# Codex TODO - TextAdventure Engine

Omedelbara tasks för Codex. Fluent API och Wiki finns i huvudplanen (Slice 39-40).

---

## Slice A: Felkoder & Förlåtande Felhantering ✅

- [x] `Enums/GameError.cs`
- [x] `Commands/CommandResult.cs` - GameError property
- [x] `GoCommand.cs` - NoExitInDirection, DoorIsLocked, DoorIsClosed
- [x] `TakeCommand.cs` - ItemNotFound, ItemNotTakeable, ItemTooHeavy, InventoryFull
- [x] `DropCommand.cs` - ItemNotInInventory
- [x] `OpenCommand.cs` - NoDoorHere, DoorIsLocked, DoorAlreadyOpen
- [x] `UnlockCommand.cs` - NoDoorHere, WrongKey, NoKeyRequired
- [x] `UseCommand.cs` - ItemNotFound, ItemNotUsable

---

## Slice B: Guard Clauses & Null-safety

Exceptions för programmeringsfel (inte spellogik).

### Konstruktorer:

- [x] `Models/Location.cs` - validera `id` (ej null/tom)
- [x] `Models/Door.cs` - validera `id`, `name`
- [x] `Models/Key.cs` - validera `id`, `name`
- [x] `Models/Item.cs` - validera `id`, `name`
- [x] `Models/Exit.cs` - validera `target` (ej null)
- [x] `Engine/GameState.cs` - validera `startLocation` (ej null)

### Mönster:

```csharp
public Location(string id)
{
    ArgumentException.ThrowIfNullOrWhiteSpace(id);
    Id = id;
}
```

### Commands:

- [x] `Commands/CommandContext.cs` - guard clause för State

---

## Slice C: Tester för felhantering

- [x] `LocationTests.cs` - test för null/tom id
- [x] `DoorTests.cs` - test för null id/name
- [x] `ItemTests.cs` - test för null id/name
- [x] `GameStateTests.cs` - test för null startLocation
- [x] `GameErrorTests.cs` - test att rätt felkoder returneras

---

## Notes

- Använd `ArgumentException.ThrowIfNullOrWhiteSpace()` (.NET 7+)
- Håll det enkelt - bara validera publika API:er
- Kör `dotnet test` efter varje ändring
- Filosofi: **GameError** = förlåtande (spellogik), **Exceptions** = programmeringsfel
- Fluent API & Wiki → se huvudplanen Slice 39-40
