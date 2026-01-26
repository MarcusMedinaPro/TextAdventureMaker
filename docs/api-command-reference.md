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
