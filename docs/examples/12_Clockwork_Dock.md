# Clockwork Dock

_Slice tag: Slice 12 — DSL Parser (.adventure). Demo focuses on loading a world from a text file._

## Story beats (max ~10 steps)
1) You arrive at a ticking dock.
2) Find the brass token.
3) Open the gate.
4) Step onto the clockwork ferry.

## Example (.adventure)
```text
world: Clockwork Dock
goal: Board the ferry
start: dock

location: dock | The dock hums with gears and steam.
item: token | brass token | Warm from the clockwork heat. | aliases=coin
door: gate | iron gate | A heavy gate of iron. | key=token
exit: in -> ferry | door=gate

location: ferry | The ferry creaks as it powers up.
```

## Example (load DSL)
```csharp
using MarcusMedina.TextAdventure.Dsl;
using MarcusMedina.TextAdventure.Parsing;

var parser = new AdventureDslParser();
var adventure = parser.ParseFile("clockwork.adventure");

var state = adventure.State;
var commandParser = new KeywordParser(KeywordParserConfig.Default);
```
