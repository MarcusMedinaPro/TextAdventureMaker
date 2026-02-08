## Slice 3: Command Pattern + Parser

**Mål:** Kommandon som objekt. Keyword-parser. "go north", "look", "quit".

### Task 3.1: ICommand + CommandResult ✅

### Task 3.2: ICommandParser + KeywordParser ✅

### Task 3.3: Inbyggda kommandon (GoCommand, LookCommand, QuitCommand, OpenCommand, UnlockCommand) ✅

### Task 3.4: Sandbox uppdatering — parser istället för raw input ✅

---

## Implementation checklist (engine)
- [x] `ICommand` interface
- [x] `CommandResult`
- [x] `ICommandParser`
- [x] `KeywordParser`
- [x] Built-in commands: `GoCommand`, `LookCommand`, `QuitCommand`, `OpenCommand`, `UnlockCommand`
- [x] Command execution helpers in engine

## Example checklist (docs/examples)
- [x] Parser-driven input loop (`03_Light_in_the_Basement.md`)
- [x] `go`/direction commands (`03_Light_in_the_Basement.md`)
- [x] `look`/`l` (`03_Light_in_the_Basement.md`)
- [x] `quit`/`exit` (`03_Light_in_the_Basement.md`)
- [x] `open`/`unlock` with door (`03_Light_in_the_Basement.md`)
