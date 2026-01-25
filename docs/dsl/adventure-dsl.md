# Adventure DSL (.adventure)

_Slice tag: Slice 12 — DSL Parser. This file defines the minimal `.adventure` format supported by the core parser._

## Rules
- One statement per line: `keyword: value`
- `#` and `//` start comments
- Use `|` to separate fields
- IDs are normalized with `ToId()` (lowercase, spaces -> underscores)

## Keywords
### `world`
Sets a title for the adventure.
```
world: Dark Forest
```

### `goal`
Sets a one-line goal.
```
goal: Find the key and unlock the cabin
```

### `start`
Sets the start location id.
```
start: entrance
```

### `location`
Defines or switches the current location.
```
location: entrance | You stand at the forest gate.
```

### `description`
Sets the current location description.
```
description: A thick forest surrounds you.
```

### `item`
Adds an item to the current location.
```
item: ice | ice | A cold chunk of ice. | weight=0.5 | aliases=ice,cold
```

### `key`
Adds a key to the current location.
```
key: cabin_key | brass key | A small brass key. | weight=0.1 | aliases=key
```

### `door`
Defines a door that can be used by exits.
```
door: cabin_door | cabin door | A sturdy wooden door. | key=cabin_key
```

### `exit`
Connects the current location to another.
```
exit: north -> forest
exit: in -> cabin | door=cabin_door
exit: down -> entrance | oneway
```

## Custom keywords
Register new keywords in code:
```csharp
var parser = new AdventureDslParser()
    .RegisterKeyword("mood", (ctx, value) => ctx.SetMetadata("mood", value));
```
