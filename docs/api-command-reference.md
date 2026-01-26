# API Command Reference

This document lists public functions, types, and helpers by slice, with tiny usage snippets in C#.

## Slice 1 — Location + Navigation

| Function / Type                                      | Usage                                                             | Explanation                                                                  |
| ---------------------------------------------------- | ----------------------------------------------------------------- | ---------------------------------------------------------------------------- |
| `Location(id, description)`                          | `var room = new Location("bedroom", "Soft morning light.");`      | Create a room with an ID and description.                                    |
| `Location.AddExit(direction, target, oneWay: false)` | `bedroom.AddExit(Direction.East, hallway);`                       | Connect locations; auto-creates the opposite exit unless `oneWay` is `true`. |
| `Location.Exits`                                     | `var exits = room.Exits.Keys;`                                    | Read available exits from a location.                                        |
| `Location.GetExit(direction)`                        | `var exit = room.GetExit(Direction.North);`                       | Fetch an exit (or `null`) by direction.                                      |
| `GameState(start, worldLocations)`                   | `var state = new GameState(bedroom, new[] { bedroom, hallway });` | Create game state with a start location and world list.                      |
| `GameState.Move(direction)`                          | `var moved = state.Move(Direction.East);`                         | Attempt to move through an exit. Returns `true` if successful.               |
| `GameState.CurrentLocation`                          | `Console.WriteLine(state.CurrentLocation.Id);`                    | Read the player’s current location.                                          |
| `Direction` enum                                     | `Direction.North`                                                 | Cardinal directions for exits and movement.                                  |

## Slice 2 — Doors + Keys

| Function / Type                                            | Usage                                                                | Explanation                                                 |
| ---------------------------------------------------------- | -------------------------------------------------------------------- | ----------------------------------------------------------- |
| `Door(id, name, description, initialState)`                | `var door = new Door("study_door", "study door", "A sturdy door.");` | Create a door with state and description.                   |
| `Door.RequiresKey(key)`                                    | `door.RequiresKey(brassKey);`                                        | Set a required key and lock the door.                       |
| `Door.Unlock(key)`                                         | `door.Unlock(brassKey);`                                             | Unlock a locked door with the correct key.                  |
| `Door.Open()`                                              | `door.Open();`                                                       | Open a closed (unlocked) door.                              |
| `Door.State`                                               | `var state = door.State;`                                            | Check whether a door is open, closed, locked, or destroyed. |
| `Door.SetReaction(action, text)`                           | `door.SetReaction(DoorAction.Unlock, "The lock clicks.");`           | Attach a reaction string to a door action.                  |
| `Door.GetReaction(action)`                                 | `var msg = door.GetReaction(DoorAction.Open);`                       | Read a reaction string for a door action.                   |
| `DoorAction` enum                                          | `DoorAction.Open`                                                    | Actions that can have reactions (open, unlock, etc.).       |
| `DoorState` enum                                           | `DoorState.Locked`                                                   | Door state values.                                          |
| `Key(id, name, description)`                               | `var key = new Key("study_key", "brass key", "A small brass key.");` | Create a key item.                                          |
| `Location.AddExit(direction, target, door, oneWay: false)` | `hallway.AddExit(Direction.East, study, door);`                      | Connect locations with a door on the exit.                  |

## Slice 44 — String Case Utilities

| Function / Type | Usage | Explanation |
| --- | --- | --- |
| `string.ToProperCase()` | `var title = "the long road".ToProperCase();` | Convert to title case using invariant culture. |
| `string.ToSentenceCase()` | `var sentence = "hELLO there".ToSentenceCase();` | Capitalise the first letter and lower-case the rest. |
| `string.ToCrazyCaps()` | `var chaos = "quiet night".ToCrazyCaps();` | Randomise casing per letter using `Random.Shared`. |

## Slice 3 — Command Pattern + Parser

| Function / Type | Usage | Explanation |
| --- | --- | --- |
| `ICommand` | `ICommand cmd = new LookCommand();` | Command interface for all actions. |
| `CommandResult` | `var result = state.Execute(cmd);` | Result of executing a command (message, reactions, quit flag). |
| `ICommandParser` | `ICommandParser parser = new KeywordParser(config);` | Parse text input into commands. |
| `KeywordParser(config)` | `var parser = new KeywordParser(KeywordParserConfig.Default);` | Built-in parser with keyword mapping. |
| `KeywordParserConfig.Default` | `KeywordParserConfig.Default` | Default synonyms and direction aliases. |
| `KeywordParserConfigBuilder.BritishDefaults()` | `KeywordParserConfigBuilder.BritishDefaults().Build();` | Builder for British English defaults. |
| `CommandExtensions.Execute(state, command)` | `var result = state.Execute(command);` | Execute a command with a `GameState`. |
| `GoCommand` | `go down` | Moves by direction (via parser). |
| `LookCommand` | `look` / `look door` | Look at room, item, or door (via parser). |
| `TakeCommand` | `take key` | Take an item into inventory (via parser). |
| `OpenCommand` | `open door` | Open the first door in the room (via parser). |
| `UnlockCommand` | `unlock door` | Unlock the first door in the room (via parser). |
| `UseCommand` | `use flashlight` | Use an item from inventory (via parser). |
| `QuitCommand` | `quit` | End the session (via parser). |

## Slice 4 — Items + Inventory

| Function / Type | Usage | Explanation |
| --- | --- | --- |
| `IItem` | `var item = new Item("ticket", "train ticket");` | Base item type. |
| `Item.SetWeight(weight)` | `item.SetWeight(0.01f);` | Set item weight for inventory totals. |
| `Item.AddAliases(...)` | `item.AddAliases("ticket", "pass");` | Add alternative names. |
| `Item.SetReaction(action, text)` | `item.SetReaction(ItemAction.Use, "…");` | Set reactions for use/take/drop/etc. |
| `IInventory` | `state.Inventory.Add(item);` | Player inventory (items + weight). |
| `InventoryCommand` | `inventory` | List inventory items with total weight. |
| `TakeCommand` | `take ticket` | Take an item from the room. |
| `DropCommand` | `drop ticket` | Drop an item into the room. |
| `UseCommand` | `use tea` / `drink tea` | Use an item (triggers reactions). |

## Slice 5 — NPCs + Dialog + Movement

| Function / Type | Usage | Explanation |
| --- | --- | --- |
| `Npc(id, name, state)` | `var guard = new Npc("guard", "guard");` | Create an NPC with a default friendly state. |
| `Npc.Description(text)` | `guard.Description("A station guard…");` | Set NPC description. |
| `Npc.Dialog(text)` | `guard.Dialog("Evening.");` | Quick single-node dialogue. |
| `Npc.SetDialog(dialogNode)` | `guard.SetDialog(node);` | Attach a dialogue tree. |
| `DialogNode(text).AddOption(text, next)` | `new DialogNode("…").AddOption("Ticket?", next)` | Build dialogue options. |
| `Npc.SetMovement(movement)` | `guard.SetMovement(new PatrolNpcMovement(route));` | Attach NPC movement behaviour. |
| `PatrolNpcMovement(route)` | `new PatrolNpcMovement(new[] { a, b })` | Patrol along a route. |
| `RandomNpcMovement()` | `new RandomNpcMovement()` | Random walk among exits. |
| `Location.AddNpc(npc)` | `concourse.AddNpc(guard);` | Place an NPC in a location. |
| `TalkCommand` | `talk guard` | Trigger dialogue for an NPC. |
| `Game(state, parser)` | `var game = new Game(state, parser);` | Game loop with NPC ticking. |
| `Game.AddTurnEndHandler(handler)` | `game.AddTurnEndHandler((g, c, r) => { … });` | Hook per-turn logic. |
