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

### `timed_spawn`
Spawns a timed item in the current location. Accepts tick numbers or time phases.
```
timed_spawn: rat | appears_at=3 | disappears_after=2 | message=A rat scurries past!
timed_spawn: owl | appears_at=night | disappears_at=dawn
```
Options: `appears_at`, `disappears_after`, `disappears_at`, `message`.
Time phases: `dawn`, `day`, `dusk`, `night`.

### `timed_door`
Adds a timed door to an existing exit in the current location. The exit must be defined first.
```
exit: north -> courtyard
timed_door: north | opens_at=dawn | closes_at=dusk | message=The gate swings open. | closed_message=The gate is shut.
timed_door: east | opens_at=5 | closes_at=10
```
Options: `opens_at`, `closes_at`, `message`, `closed_message`.

## Custom keywords
Register new keywords in code:
```csharp
var parser = new AdventureDslParser()
    .RegisterKeyword("mood", (ctx, value) => ctx.SetMetadata("mood", value));
```

## Parsing from strings
Parse adventure definitions from in-memory strings (useful for testing or generated content):
```csharp
var parser = new AdventureDslParser();
DslAdventure adventure = parser.ParseString("""
    world: My Adventure
    location: start | The beginning.
    exit: north -> forest
    location: forest | Tall trees surround you.
    """);
```

## Warnings
Unknown keywords produce warnings (not errors). The parser continues and collects them:
```csharp
DslAdventure adventure = parser.ParseString("npc: guard | A guard.\nlocation: room");
if (adventure.HasWarnings)
{
    foreach (var warning in adventure.Warnings)
        Console.WriteLine(warning); // Line 1: Unknown keyword: 'npc'
}
```
Misspelled keywords get correction suggestions (e.g. `locaton` suggests `location`).
