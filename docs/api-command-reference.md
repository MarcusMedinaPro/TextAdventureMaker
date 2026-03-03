# API Command Reference

Public types, methods, and helpers for `MarcusMedina.TextAdventure` and `MarcusMedina.TextAdventure.AI`.

---

## World / Game Setup

| Type / Member | Usage | Notes |
| --- | --- | --- |
| `GameBuilder.Create()` | `var builder = GameBuilder.Create();` | Entry point for fluent game construction. |
| `.UseState(state)` | `.UseState(myGameState)` | Set the game state. |
| `.UseParser(parser)` | `.UseParser(new KeywordParser(config))` | Set the command parser. |
| `.UsePlugin(plugin)` | `.UsePlugin(new AiPlugin(module))` | Attach a plugin (e.g. AI). |
| `.UseInput(reader)` | `.UseInput(Console.In)` | Override input stream. |
| `.UseOutput(writer)` | `.UseOutput(Console.Out)` | Override output stream. |
| `.UsePrompt(text)` | `.UsePrompt("> ")` | Set the input prompt. |
| `.UseStartLocation(location)` | `.UseStartLocation(entrance)` | Override the start room. |
| `.UseTimeSystem(ts)` | `.UseTimeSystem(new TimeSystem())` | Attach a time system. |
| `.AddTurnStart(handler)` | `.AddTurnStart(g => g.State.ShowRoom())` | Register a per-turn start hook. |
| `.AddTurnEnd(handler)` | `.AddTurnEnd((g, cmd, r) => { … })` | Register a per-turn end hook. |
| `.DefineStoryBranch(branch)` | `.DefineStoryBranch(myBranch)` | Register a story branch. |
| `.Build()` | `var game = builder.Build();` | Construct and return the configured `Game`. |
| `game.Run()` | `game.Run();` | Start the main blocking game loop. |
| `game.Execute(input)` | `game.Execute("go north")` | Parse and execute a command programmatically. |
| `game.TickNpcs()` | `game.TickNpcs();` | Advance all NPC movement strategies. |
| `game.RequestStop()` | `game.RequestStop();` | Signal the game loop to exit. |
| `game.ActivateChapter(id)` | `game.ActivateChapter("act2")` | Switch to a named chapter. |
| `game.State` | `var s = game.State;` | Access the live `GameState`. |
| `game.CreateValidator()` | `game.CreateValidator().Validate()` | Run integrity checks on the world. |
| `game.CreateExplorer()` | `game.CreateExplorer().ExploreAll()` | Visit every reachable location. |
| `game.EnableTestingMode()` | `game.EnableTestingMode();` | Suppress side-effects for tests. |
| `GameState(start)` | `new GameState(entranceRoom)` | Create game state with start location. |
| `state.CurrentLocation` | `state.CurrentLocation.Id` | Current room. |
| `state.Inventory` | `state.Inventory.Items` | Player inventory. |
| `state.Quests` | `state.Quests.CheckAll(state)` | Quest log. |
| `state.WorldState` | `state.WorldState.SetFlag("lit", true)` | World flags and counters. |
| `state.Events` | `state.Events.Subscribe(type, handler)` | Game event bus. |
| `state.ShowRoom()` | `state.ShowRoom();` | Print current room to console. |
| `state.DisplayResult(cmd, result)` | `state.DisplayResult(cmd, result)` | Print command result contextually. |
| `state.Execute(command)` | `state.Execute(new LookCommand())` | Execute an `ICommand` on the state. |
| `state.Look()` | `var r = state.Look();` | Execute a look and return the result. |
| `state.Stats` | `state.Stats.Health` | Player stats. |
| `state.Wallet` | `state.Wallet.Gold` | Player currency. |
| `state.PlayerHistory` | `state.PlayerHistory` | Record of player actions. |
| `state.DebugMode` | `state.DebugMode = true;` | Enable debug output. |
| `state.Locations` | `foreach (var loc in state.Locations)` | All registered locations. |
| `state.TickNpcTriggers()` | `state.TickNpcTriggers();` | Fire pending NPC triggers. |
| `state.TickPoisons()` | `state.TickPoisons()` | Advance poison effects; returns damage pairs. |

---

## Locations & Navigation

| Type / Member | Usage | Notes |
| --- | --- | --- |
| `new Location(id, description)` | `new Location("cellar", "Dark and damp.")` | Create a room. |
| `location.Description(text)` | `room.Description("Soft morning light.")` | Set or update description. |
| `location.SetDynamicDescription(d)` | `room.SetDynamicDescription(new DynamicDescription()…)` | Attach a state-dependent description. |
| `DynamicDescription().When(pred, text).Default(text)` | `.When(s => s.WorldState.GetFlag("lit"), "Bright.").Default("Dark.")` | Build a dynamic description with conditions. |
| `location.AddExit(dir, target)` | `hall.AddExit(Direction.North, bedroom)` | Connect rooms; auto-creates reverse exit. |
| `location.AddExit(dir, target, door)` | `hall.AddExit(Direction.East, study, door)` | Connect rooms with a door on the exit. |
| `location.GetExit(dir)` | `room.GetExit(Direction.Down)` | Fetch an exit or `null`. |
| `location.Exits` | `room.Exits.Keys` | All available directions. |
| `location.AddItem(item)` | `room.AddItem(key)` | Place an item in the room. |
| `location.RemoveItem(item)` | `room.RemoveItem(torch)` | Remove an item from the room. |
| `location.FindItem(name)` | `room.FindItem("torch")` | Find an item by name or alias. |
| `location.AddNpc(npc)` | `room.AddNpc(guard)` | Place an NPC in the room. |
| `location.FindNpc(name)` | `room.FindNpc("guard")` | Find an NPC by name. |
| `location.Items` | `room.Items` | All items in the room. |
| `location.Npcs` | `room.Npcs` | All NPCs in the room. |
| `location.GetDescription()` | `room.GetDescription()` | Get room description string. |
| `location.GetRoomItems()` | `room.GetRoomItems()` | Formatted item list for display. |
| `location.GetRoomExits()` | `room.GetRoomExits()` | Formatted exit list for display. |
| `location.ShowRoom()` | `room.ShowRoom()` | Print room header + exits + items to console. |
| `state.IsCurrentRoomId(id)` | `state.IsCurrentRoomId("quay")` | Check if player is in a specific room. |
| `Direction` enum | `Direction.North` | North, South, East, West, Up, Down, NE, NW, SE, SW. |
| `IPathfinder.FindPath(start, goal, all)` | `state.Pathfinder.FindPath(a, b, all)` | Find a route between rooms. |
| `ILocationDiscoverySystem` | `state.LocationDiscovery.IsDiscovered("cave")` | Fog-of-war and map discovery. |

---

## Doors & Keys

| Type / Member | Usage | Notes |
| --- | --- | --- |
| `new Door(id, name, initialState)` | `new Door("gate", "iron gate", DoorState.Locked)` | Create a door. |
| `door.RequiresKey(key)` | `door.RequiresKey(brassKey)` | Set required key and lock the door. |
| `door.Unlock(key)` | `door.Unlock(brassKey)` | Unlock with matching key. |
| `door.Open()` | `door.Open()` | Open (must be closed and unlocked). |
| `door.Close()` | `door.Close()` | Close an open door. |
| `door.Lock(key)` | `door.Lock(brassKey)` | Lock the door. |
| `door.Destroy()` | `door.Destroy()` | Destroy the door. |
| `door.State` | `door.State == DoorState.Locked` | Current door state. |
| `door.IsPassable` | `exit.Door?.IsPassable` | Whether the door can be passed. |
| `door.Description(text)` | `door.Description("A sturdy oaken door.")` | Set description. |
| `door.AddAliases(…)` | `door.AddAliases("hatch", "trapdoor")` | Add alternative names. |
| `door.SetReaction(action, text)` | `door.SetReaction(DoorAction.Unlock, "The lock clicks.")` | Attach a reaction string. |
| `door.GetReaction(action)` | `door.GetReaction(DoorAction.Open)` | Read a reaction string. |
| `DoorState` enum | `DoorState.Open \| Closed \| Locked` | Door state values. |
| `DoorAction` enum | `DoorAction.Open \| Close \| Lock \| Unlock \| Destroy` | Actions with optional reaction text. |
| `new Key(id, name, description)` | `new Key("brass_key", "brass key", "A small key.")` | Create a key item. |
| `door.OnOpen += handler` | `door.OnOpen += _ => Console.WriteLine("Creak!")` | Event when door opens. |
| `door.OnUnlock += handler` | `door.OnUnlock += _ => SoundEffect("click")` | Event when door unlocks. |

---

## Items & Inventory

| Type / Member | Usage | Notes |
| --- | --- | --- |
| `new Item(id, name, description)` | `new Item("torch", "torch", "A flickering torch.")` | Create an item. |
| `item.SetTakeable(bool)` | `item.SetTakeable(true)` | Mark as takeable. |
| `item.SetWeight(weight)` | `item.SetWeight(0.5f)` | Set item weight. |
| `item.AddAliases(…)` | `item.AddAliases("lamp", "light")` | Add alternative names. |
| `item.SetDescription(text)` | `item.SetDescription("Worn and battered.")` | Update description. |
| `item.SetPresenceDescription(text)` | `item.SetPresenceDescription("A torch gutters in a bracket.")` | Text shown when item is in room. |
| `item.SetReaction(action, text)` | `item.SetReaction(ItemAction.Take, "You grab the torch.")` | Set reaction text for an action. |
| `item.GetReaction(action)` | `item.GetReaction(ItemAction.Use)` | Read reaction text. |
| `ItemAction` enum | `ItemAction.Take \| Drop \| Use \| Read \| Examine \| Destroy` | Item interaction types. |
| `item.SetReadable()` | `item.SetReadable()` | Mark item as readable. |
| `item.SetReadText(text)` | `item.SetReadText("The map shows a hidden cave.")` | Set readable text. |
| `item.RequireTakeToRead()` | `item.RequireTakeToRead()` | Item must be in inventory to read. |
| `item.SetStackable()` | `item.SetStackable()` | Enable stacking. |
| `item.SetAmount(n)` | `item.SetAmount(3)` | Set stack count. |
| `item.AsStack(amount, weight)` | `item.AsStack(5, 0.2f)` | Shorthand for stackable with amount and weight. |
| `item.AsDrink(heal)` | `item.AsDrink(6)` | Mark as drinkable with heal amount. |
| `item.WithPoison(dmg, turns)` | `item.WithPoison(2, 3)` | Add poison effect to consumable. |
| `item.SetDurability(cur, max)` | `item.SetDurability(10, 10)` | Set durability. |
| `item.SetHint(text)` | `item.SetHint("Use with the valve.")` | Set a gameplay hint. |
| `item.HideFromItemList()` | `item.HideFromItemList()` | Suppress from room item list. |
| `item.Clone()` | `var copy = item.Clone()` | Create a deep copy. |
| `item.OnTake += handler` | `item.OnTake += _ => SetFlag("taken")` | Event when item is taken. |
| `item.OnUse += handler` | `item.OnUse += _ => HealPlayer()` | Event when item is used. |
| `item.OnDestroy += handler` | `item.OnDestroy += _ => SpawnFragment()` | Event when item is destroyed. |
| `new Inventory(limitType, max, maxWeight)` | `new Inventory(InventoryLimitType.MaxWeight, 0, 10f)` | Create inventory with limit. |
| `inventory.Add(item)` | `state.Inventory.Add(ticket)` | Add item to inventory. |
| `inventory.Remove(item)` | `state.Inventory.Remove(ticket)` | Remove item from inventory. |
| `inventory.FindItem(name)` | `state.Inventory.FindItem("key")` | Find by name or alias. |
| `inventory.FindById(id)` | `state.Inventory.FindById("brass_key")` | Find by exact ID. |
| `inventory.CanAdd(item)` | `state.Inventory.CanAdd(stone)` | Check if item fits within limits. |
| `inventory.Items` | `state.Inventory.Items` | All items in inventory. |
| `InventoryLimitType` enum | `InventoryLimitType.Unlimited \| MaxCount \| MaxWeight` | Inventory limit modes. |
| `RecipeBook.Add(recipe)` | `state.RecipeBook.Add(recipe)` | Register a combination recipe. |

---

## NPCs, Dialogue & Movement

| Type / Member | Usage | Notes |
| --- | --- | --- |
| `new Npc(id, name)` | `new Npc("guard", "Guard")` | Create an NPC. |
| `npc.Description(text)` | `npc.Description("A weary station guard.")` | Set NPC description. |
| `npc.Dialog(text)` | `npc.Dialog("Evening.")` | Set a simple single-node dialogue. |
| `npc.SetDialog(node)` | `npc.SetDialog(rootNode)` | Attach a dialogue tree. |
| `npc.SetState(state)` | `npc.SetState(NpcState.Hostile)` | Set NPC mood/state. |
| `npc.SetMovement(movement)` | `npc.SetMovement(new PatrolNpcMovement(route))` | Attach movement behaviour. |
| `NpcState` enum | `NpcState.Friendly \| Hostile \| Neutral \| Dead` | NPC state values. |
| `PatrolNpcMovement(route)` | `new PatrolNpcMovement([a, b, c])` | NPC patrols a fixed route. |
| `RandomNpcMovement()` | `new RandomNpcMovement()` | NPC moves randomly each tick. |
| `FollowNpcMovement()` | `new FollowNpcMovement()` | NPC follows the player. |
| `NoNpcMovement()` | `new NoNpcMovement()` | NPC stays in place. |
| `new DialogNode(text)` | `new DialogNode("What do you want?")` | Create a dialogue node. |
| `node.AddOption(text, next)` | `node.AddOption("Help me.", replyNode)` | Add a player response option. |
| `npc.AddDialogRule(id)` | `npc.AddDialogRule("greet")` | Add a conditional dialog rule. |
| `npc.Memory.Remember(key, val)` | `npc.Memory.Remember("saw_player", true)` | Store a value in NPC memory. |
| `npc.Memory.Recall<T>(key)` | `npc.Memory.Recall<bool>("saw_player")` | Retrieve a stored value. |
| `npc.Personality.Mood` | `npc.Personality.Mood = 20;` | Current mood (-100 to 100). |
| `npc.DefineArc(id)` | `npc.DefineArc("redemption")` | Define a character arc. |
| `npc.CreateBond(id)` | `npc.CreateBond("friendship")` | Create a relationship bond. |
| `npc.SetArchetype(type)` | `npc.SetArchetype(CharacterArchetype.Mentor)` | Set the character archetype. |
| `npc.OnSee(target)` | `npc.OnSee("player").OnComplete(handler)` | Trigger when NPC sees a target. |
| `state.WorldState.GetRelationship(id)` | `state.WorldState.GetRelationship("keeper")` | Get relationship value with an NPC. |
| `state.WorldState.SetRelationship(id, val)` | `state.WorldState.SetRelationship("keeper", 50)` | Set relationship value. |
| `npc.AddReaction(trigger, text)` | `guard.AddReaction("blow", "The guard raises an eyebrow slowly.")` | Add an NPC reaction for a verb trigger. Fluent/chainable. |
| `npc.AddReaction(trigger, text, condition)` | `guard.AddReaction("blow", "He covers his ears.", s => s.WorldState.GetFlag("on_duty"))` | Conditional NPC reaction. |
| `npc.GetReaction(trigger, state)` | `guard.GetReaction("blow:trumpet", state)` | Returns first matching reaction text, or `null`. Specific triggers (`blow:trumpet`) take precedence over general (`blow`). |
| `npc.Reactions` | `guard.Reactions` | All registered reactions (`IReadOnlyList<NpcReaction>`). |

---

## Commands & Parser

| Type / Member | Usage | Notes |
| --- | --- | --- |
| `ICommand` | `ICommand cmd = parser.Parse("go north")` | All commands implement this. |
| `ICommand.Execute(context)` | `cmd.Execute(new CommandContext(state))` | Execute the command. |
| `CommandResult.Ok(msg)` | `CommandResult.Ok("You pick up the key.")` | Successful result. |
| `CommandResult.Fail(msg, error)` | `CommandResult.Fail("Too heavy.", GameError.InventoryFull)` | Failed result. |
| `CommandResult.Quit(msg)` | `CommandResult.Quit("Goodbye.")` | Result that ends the game. |
| `result.Success` | `if (result.Success)` | Whether command succeeded. |
| `result.Message` | `Console.WriteLine(result.Message)` | Output text. |
| `result.ShouldQuit` | `if (result.ShouldQuit) break;` | Whether to exit the game loop. |
| `GameError` enum | `GameError.CannotMove` | Error codes for failed commands. |
| `new KeywordParser(config)` | `new KeywordParser(KeywordParserConfig.Default)` | Built-in keyword-based parser. |
| `KeywordParserConfig.Default` | `KeywordParserConfig.Default` | Default English keyword configuration. |
| `KeywordParserConfigBuilder.BritishDefaults()` | `KeywordParserConfigBuilder.BritishDefaults().Build()` | Builder with British English defaults. |
| `.WithFuzzyMatching(true, 1)` | `.WithFuzzyMatching(true, 1)` | Enable fuzzy matching with max edit distance. |
| `.WithPhraseAlias(from, to)` | `.WithPhraseAlias("look at", "look")` | Map a multi-word phrase to a command keyword. |
| `.WithExamine(…)` | `.WithExamine("examine", "x")` | Configure examine synonyms. |
| `.WithMove(…)` | `.WithMove("move", "push", "shift")` | Configure move/push synonyms. |
| `.WithQuest(…)` | `.WithQuest("quest", "journal")` | Configure quest command keyword. |
| `.AddSynonyms(keyword, …)` | `.AddSynonyms("take", "grab", "snatch")` | Add synonyms for any command keyword. |
| `.AddCustomVerb(verb)` | `.AddCustomVerb("blow")` | Register a custom verb; parser fires `CustomActionCommand(verb, target?)`. |
| `.AddCustomVerbs(…)` | `.AddCustomVerbs("blow", "threaten", "juggle")` | Register multiple custom verbs at once. |
| **Command Types** | | All constructed by the parser from input. |
| `GoCommand` | `go north` | Move in a direction. |
| `LookCommand` | `look` / `look door` | Examine the room or a target. |
| `ExamineCommand` | `examine note` | Examine a specific object in detail. |
| `TakeCommand` | `take key` | Pick up an item. |
| `TakeAllCommand` | `take all` | Pick up all items in the room. |
| `DropCommand` | `drop key` | Drop an item. |
| `DropAllCommand` | `drop all` | Drop all items. |
| `UseCommand` | `use torch` | Use an item from inventory. |
| `ReadCommand` | `read note` | Read a readable item. |
| `EatCommand` | `eat bread` | Eat a food item. |
| `DrinkCommand` | `drink potion` | Drink a liquid item. |
| `InventoryCommand` | `inventory` / `i` | List inventory items and total weight. |
| `StatsCommand` | `stats` | Show player stats. |
| `TalkCommand` | `talk guard` | Initiate NPC dialogue. |
| `OpenCommand` | `open door` | Open a door. |
| `CloseCommand` | `close door` | Close a door. |
| `LockCommand` | `lock door` | Lock a door. |
| `UnlockCommand` | `unlock door` | Unlock a door. |
| `AttackCommand` | `attack guard` | Attack an NPC. |
| `FleeCommand` | `flee` | Flee from combat. |
| `CombineCommand` | `combine rope hook` | Combine two items. |
| `BuyCommand` | `buy rope` | Purchase from a store. |
| `RepairCommand` | `repair sword` | Repair an item. |
| `ThrowCommand` | `throw rock north` | Throw an item in a direction. |
| `ListenCommand` | `listen` / `listen north` | Listen for sounds (optionally directional). |
| `ShoutCommand` | `shout` / `shout north` | Shout a message. |
| `SaveCommand` | `save` | Save game state. |
| `LoadCommand` | `load` | Load saved game state. |
| `UndoCommand` | `undo` | Undo last action. |
| `RedoCommand` | `redo` | Redo undone action. |
| `QuitCommand` | `quit` / `exit` | End the game. |
| `MapCommand` | `map` | Show ASCII map. |
| `QuestCommand` | `quests` | Show quest log. |
| `CustomActionCommand(verb, target?)` | `blow trumpet` (requires `command: blow`) | Fires for custom verbs. No built-in logic — NPC reactions and hooks respond. |

---

## Quest System

| Type / Member | Usage | Notes |
| --- | --- | --- |
| `new Quest(id, title, description)` | `new Quest("find_key", "Find the Key", "Locate the brass key.")` | Create a quest. |
| `quest.Start()` | `quest.Start()` | Move quest to Active. |
| `quest.Complete()` | `quest.Complete()` | Mark as Completed. |
| `quest.Fail()` | `quest.Fail()` | Mark as Failed. |
| `quest.AddCondition(cond)` | `quest.AddCondition(new HasItemCondition("key"))` | Add a completion condition. |
| `quest.CheckProgress(state)` | `quest.CheckProgress(state)` | Evaluate conditions; completes if all met. |
| `QuestState` enum | `QuestState.NotStarted \| Active \| Complete \| Failed` | Quest lifecycle states. |
| `HasItemCondition(itemId)` | `new HasItemCondition("brass_key")` | Condition: player holds item. |
| `WorldFlagCondition(flag)` | `new WorldFlagCondition("beacon_lit")` | Condition: world flag is set. |
| `WorldCounterCondition(key, min)` | `new WorldCounterCondition("bell_rings", 2)` | Condition: counter reaches minimum. |
| `NpcDeadCondition(npcId)` | `new NpcDeadCondition("boss")` | Condition: NPC is dead. |
| `LocationVisitedCondition(id)` | `new LocationVisitedCondition("cellar")` | Condition: location was visited. |
| `ItemDestroyedCondition(itemId)` | `new ItemDestroyedCondition("old_letter")` | Condition: item was destroyed. |
| `AllOfCondition(list)` | `new AllOfCondition([a, b])` | Condition: all child conditions pass. |
| `AnyOfCondition(list)` | `new AnyOfCondition([a, b])` | Condition: any child condition passes. |
| `state.Quests.Add(quest)` | `state.Quests.Add(findQuest)` | Register quest in the log. |
| `state.Quests.CheckAll(state)` | `state.Quests.CheckAll(state)` | Evaluate all active quests. |
| `state.Quests.Find(id)` | `state.Quests.Find("find_key")` | Look up a quest by ID. |
| `state.Quests.GetByState(qs)` | `state.Quests.GetByState(QuestState.Active)` | Filter quests by state. |

---

## World State, Events & Flags

| Type / Member | Usage | Notes |
| --- | --- | --- |
| `state.WorldState.SetFlag(key, val)` | `state.WorldState.SetFlag("door_open", true)` | Set a boolean world flag. |
| `state.WorldState.GetFlag(key)` | `state.WorldState.GetFlag("door_open")` | Read a boolean world flag. |
| `state.WorldState.Increment(key, n)` | `state.WorldState.Increment("crossings")` | Increment a counter; returns new value. |
| `state.WorldState.GetCounter(key)` | `state.WorldState.GetCounter("turns")` | Read a counter value. |
| `state.WorldState.AddTimeline(text)` | `state.WorldState.AddTimeline("Beacon lit.")` | Append a narrative timeline entry. |
| `state.WorldState.Timeline` | `state.WorldState.Timeline` | All timeline entries. |
| `state.Events.Subscribe(type, handler)` | `state.Events.Subscribe(GameEventType.ItemTaken, e => { … })` | Subscribe to a game event. |
| `state.Events.Publish(event)` | `state.Events.Publish(new GameEvent(…))` | Publish a custom event. |
| `state.Events.Unsubscribe(type, h)` | `state.Events.Unsubscribe(GameEventType.ItemTaken, handler)` | Unsubscribe. |
| `GameEventType` enum | `GameEventType.ItemTaken \| NpcDeath \| QuestCompleted \| …` | All subscribable event types. |
| `game.OnAction(id, handler)` | `game.OnAction("ring_bell", ctx => { … })` | Register a named custom action callback. |
| `game.OnNpcDeath(id, handler)` | `game.OnNpcDeath("boss", ctx => ShowEnding())` | Register NPC death callback. |
| `game.OnItemPickup(id, handler)` | `game.OnItemPickup("crown", ctx => Unlock())` | Register item pickup callback. |
| `state.RandomEvents` | `state.RandomEvents.Tick(state)` | Pool of random narrative events. |
| `state.Factions` | `state.Factions.GetRelation("guild", "player")` | Faction relationship system. |
| `state.DramaticIrony` | `state.DramaticIrony.PlayerLearn("secret")` | Dramatic irony / dramatic reveal system. |
| `state.Tension` | `state.Tension.Tick(state)` | Narrative tension system. |

---

## Combat & Stats

| Type / Member | Usage | Notes |
| --- | --- | --- |
| `new Stats(maxHealth, currentHealth?)` | `new Stats(100)` | Create player/NPC stats. |
| `stats.Health` | `stats.Health` | Current health. |
| `stats.MaxHealth` | `stats.MaxHealth` | Maximum health. |
| `stats.Damage(amount)` | `stats.Damage(10)` | Reduce health. |
| `stats.Heal(amount)` | `stats.Heal(5)` | Restore health. |
| `state.CombatSystem.Attack(state, npc)` | `state.CombatSystem.Attack(state, guard)` | Initiate attack on NPC. |
| `state.CombatSystem.Flee(state, npc?)` | `state.CombatSystem.Flee(state)` | Attempt to flee combat. |
| `state.TickPoisons()` | `foreach (var (src, dmg) in state.TickPoisons())` | Advance poison effects; returns source/damage pairs. |
| `item.WithPoison(dmg, turns)` | `potion.WithPoison(2, 4)` | Add poison to a consumable item. |

---

## Save / Load

| Type / Member | Usage | Notes |
| --- | --- | --- |
| `ISaveSystem.Save(path, memento)` | `state.SaveSystem.Save("save.json", state.History.Save())` | Save game to a file. |
| `ISaveSystem.Load(path)` | `state.SaveSystem.Load("save.json")` | Load game from a file. |
| `JsonSaveSystem` | `new JsonSaveSystem()` | Default JSON-based save system. |
| `MementoCaretaker.Save(state)` | `state.History.Save(state)` | Take a snapshot for undo/redo. |
| `MementoCaretaker.Undo()` | `state.History.Undo()` | Restore the previous snapshot. |
| `MementoCaretaker.Redo()` | `state.History.Redo()` | Re-apply an undone snapshot. |

---

## Time System

| Type / Member | Usage | Notes |
| --- | --- | --- |
| `new TimeSystem()` | `new TimeSystem()` | Create a time system. |
| `.Enable()` | `.Enable()` | Start advancing time on each tick. |
| `.SetTicksPerDay(n)` | `.SetTicksPerDay(8)` | Set ticks per in-game day. |
| `.SetStartTime(phase)` | `.SetStartTime(TimeOfDay.Dusk)` | Set starting time of day. |
| `.SetMaxMoves(n)` | `.SetMaxMoves(50)` | Cap total player moves. |
| `.OnPhase(phase, handler)` | `.OnPhase(TimeOfDay.Night, s => ShowMoon())` | Register a callback when phase begins. |
| `.OnMovesRemaining(n, handler)` | `.OnMovesRemaining(5, s => Warn())` | Callback when n moves remain. |
| `.OnMovesExhausted(handler)` | `.OnMovesExhausted(s => GameOver())` | Callback when moves run out. |
| `ts.Tick(state)` | `ts.Tick(state)` | Advance time by one turn. |
| `ts.CurrentPhase` | `ts.CurrentPhase` | Current `TimePhase`. |
| `ts.CurrentTimeOfDay` | `ts.CurrentTimeOfDay` | Current `TimeOfDay`. |
| `ts.CurrentTick` | `ts.CurrentTick` | Raw tick counter. |
| `ts.MovesRemaining` | `ts.MovesRemaining` | Moves left (if limited). |
| `TimeOfDay` enum | `TimeOfDay.Dawn \| Morning \| Afternoon \| Evening \| Dusk \| Night \| Midnight` | Time of day phases. |
| `TimePhase` enum | `TimePhase.Day \| Night` | Broad day/night phase. |

---

## Localization / Language

| Type / Member | Usage | Notes |
| --- | --- | --- |
| `ILanguageProvider` | `ILanguageProvider lang = new JsonLanguageProvider(…)` | Abstraction for localized strings. |
| `JsonLanguageProvider` | `new JsonLanguageProvider("gamelang.en.json")` | Load language strings from a JSON file. |
| `Language.DialogOptionsLabel` | `Console.WriteLine(Language.DialogOptionsLabel)` | Localized "Choose an option:" label. |
| `Language.DialogOption(n, text)` | `Language.DialogOption(1, "Yes")` | Format a numbered dialogue option. |
| `IGrammarProvider.WithArticle(noun)` | `grammar.WithArticle("apple")` | Add "a"/"an"/"the" before a noun. |
| `IGrammarProvider.Plural(noun, count)` | `grammar.Plural("coin", 3)` | Pluralise based on count. |

---

## Narrative & Story

| Type / Member | Usage | Notes |
| --- | --- | --- |
| `game.DefineChapters()` | `game.DefineChapters().Add("act1", …)` | Define chapter progression. |
| `game.ActivateChapter(id)` | `game.ActivateChapter("act2")` | Switch to a named chapter. |
| `game.AdvanceChapter()` | `game.AdvanceChapter()` | Move to the next chapter. |
| `game.AddMemory(id)` | `game.AddMemory("old_house")` | Register a flashback memory. |
| `location.TriggerFlashback(id)` | `location.TriggerFlashback("old_house")` | Link a location to a flashback. |
| `state.Foreshadowing` | `state.Foreshadowing` | Foreshadowing hint system. |
| `state.NarrativeVoice` | `state.NarrativeVoice` | Narrator voice and tone control. |
| `state.Agency` | `state.Agency` | Track player-driven choices. |
| `state.DramaticIrony` | `state.DramaticIrony.PlayerLearn("secret")` | Manage player vs. NPC knowledge. |
| `state.Tension` | `state.Tension.Tick(state)` | Manage narrative tension level. |
| `state.Flashbacks` | `state.Flashbacks` | Manage flashback memories. |
| `game.DefineStoryBranch(branch)` | `game.DefineStoryBranch(myBranch)` | Register a conditional story branch. |
| `game.UseHeroJourneyTemplate()` | `game.UseHeroJourneyTemplate()` | Apply hero journey arc template. |
| `game.UseTragicArc()` | `game.UseTragicArc()` | Apply tragic arc template. |
| `game.UseTransformationArc()` | `game.UseTransformationArc()` | Apply transformation arc template. |
| `game.UseEnsembleJourney()` | `game.UseEnsembleJourney()` | Apply ensemble journey template. |
| `game.UseDescentArc()` | `game.UseDescentArc()` | Apply descent arc template. |
| `game.UseMoralLabyrinth()` | `game.UseMoralLabyrinth()` | Apply moral labyrinth template. |
| `game.UseLayeredNarrative()` | `game.UseLayeredNarrative()` | Apply layered narrative template. |
| `game.UseFramedNarrative()` | `game.UseFramedNarrative()` | Apply framed narrative template. |
| `game.UseSpiralNarrative()` | `game.UseSpiralNarrative()` | Apply spiral narrative template. |
| `game.UseCaretakerArc()` | `game.UseCaretakerArc()` | Apply caretaker arc template. |
| `game.UseWitnessArc()` | `game.UseWitnessArc()` | Apply witness arc template. |
| `game.UseWorldShiftArc()` | `game.UseWorldShiftArc()` | Apply world shift arc template. |

---

## DSL Parser

| Type / Member | Usage | Notes |
| --- | --- | --- |
| `new AdventureDslParser()` | `new AdventureDslParser()` | Parser for `.adventure` DSL files. |
| `parser.ParseFile(path)` | `parser.ParseFile("game.adventure")` | Load and parse a DSL file. |
| `parser.Parse(text)` | `parser.Parse(dslText)` | Parse DSL from a string. |
| `parser.RegisterKeyword(kw, handler)` | `parser.RegisterKeyword("myKey", (_, v) => { … })` | Add a custom keyword extension. |
| `AdventureDslExporter` | `new AdventureDslExporter().Export(game)` | Export game state back to DSL text. |
| `DslParseError` | `error.Line \| error.Message \| error.Suggestion` | Diagnostic produced by the parser. |
| `DslParseException` | `ex.Errors` | Thrown on parse failure; contains all errors. |
| `DslHelper` | `DslHelper.Validate(…)` | CRUD and migration utilities for DSL files. |
| **Core DSL Keywords** | | |
| `dsl_version: 2` | `dsl_version: 2` | Declare DSL v2 file. |
| `world: name` | `world: Dark Forest` | Set world name. |
| `goal: text` | `goal: Find the key.` | Set the player goal. |
| `start: id` | `start: entrance` | Set the starting location. |
| `location: id \| description` | `location: cave \| Dark and damp.` | Define a location. |
| `exit: dir -> id` | `exit: north -> forest` | Add a one-way exit. |
| `exit: dir -> id \| door=id` | `exit: east -> study \| door=oak_door` | Exit with a door. |
| `item: id \| name \| desc` | `item: torch \| torch \| A flickering torch.` | Define an item. |
| `key: id \| name \| desc` | `key: gate_key \| gate key \| An iron key.` | Define a key item. |
| `door: id \| name \| desc \| key=id` | `door: gate \| iron gate \| Rusted. \| key=gate_key` | Define a door. |
| `npc: id \| name=… \| state=…` | `npc: guard \| name=Guard \| state=friendly` | Define an NPC. |
| `npc_place: location_id \| npc_id` | `npc_place: square \| guard` | Place an NPC in a location. |
| `counter: key=value` | `counter: gold=25` | Set a world counter. |
| `flag: key=true/false` | `flag: beacon_lit=false` | Set a world flag. |
| `room_desc: id \| text` | `room_desc: cave \| first_visit=Eerie silence greets you.` | Conditional room description. |
| `room_desc_when: id \| if=expr \| text=…` | `room_desc_when: cave \| if=flag:lit=true \| text=Now bright.` | State-dependent room description. |
| `command: verb, …` | `command: blow, threaten, juggle` | Declare custom player verbs. Parser accepts `<verb>` and `<verb> <target>`; fires `CustomActionCommand`. Default verbs (go, look, take…) need not be declared. |
| `npc_reaction: npc_id \| on=trigger \| text=…` | `npc_reaction: guard \| on=blow \| text=The guard raises an eyebrow.` | Attach a text reaction to an NPC for any verb trigger. Fires when the NPC is in the current room. |
| `npc_reaction: npc_id \| on=verb:target \| text=…` | `npc_reaction: guard \| on=blow:trumpet \| text=He covers his ears.` | Specific-target trigger; takes precedence over the general form. |
| `npc_reaction: … \| if=flag:key=true` | `npc_reaction: guard \| on=blow \| text=… \| if=flag:guard_on_duty=true` | Conditional reaction — only fires when the world flag condition passes. |

---

## String Utilities

| Type / Member | Usage | Notes |
| --- | --- | --- |
| `text.TextCompare(other)` | `"TAKE".TextCompare("take")` | Case-insensitive comparison (trimmed). |
| `text.StartsWithIgnoreCase(prefix)` | `text.StartsWithIgnoreCase("/debug")` | Case-insensitive starts-with. |
| `text.EndsWithIgnoreCase(suffix)` | `text.EndsWithIgnoreCase(".md")` | Case-insensitive ends-with. |
| `text.ToId()` | `"Dark Forest".ToId()` | Lowercase + underscores for identifiers. |
| `text.ToProperCase()` | `"the long road".ToProperCase()` | Title-case conversion. |
| `text.ToSentenceCase()` | `"hELLO WORLD".ToSentenceCase()` | Capitalise first letter only. |
| `text.ToCrazyCaps()` | `"quiet night".ToCrazyCaps()` | Randomise capitalisation. |
| `text.CollapseRepeats()` | `"loooook".CollapseRepeats()` | Collapse repeated characters. |
| `text.FuzzyMatch(other, maxDist)` | `"look".FuzzyMatch("lokk", 1)` | True when fuzzy distance ≤ max. |
| `text.FuzzyDistanceTo(other, max)` | `"look".FuzzyDistanceTo("loook", 1)` | Edit distance with collapsed repeats. |
| `text.LevenshteinDistanceTo(other)` | `"look".LevenshteinDistanceTo("lokk")` | Raw Levenshtein distance. |
| `text.SimilarTo(other)` | `"look".SimilarTo("lokk")` | Similarity score. |
| `text.SoundexKey()` | `"Steven".SoundexKey()` | Soundex phonetic key. |
| `text.SoundsLike(other)` | `"Steven".SoundsLike("Stephen")` | True when Soundex keys match. |
| `text.IsHelpRequest()` | `input.IsHelpRequest()` | True for "help", "?", etc. |
| `text.Lower()` | `input.Lower()` | Lowercase + trim. |

---

## Validation & Tools

| Type / Member | Usage | Notes |
| --- | --- | --- |
| `GameValidator(game)` | `new GameValidator(game)` | Validates world integrity (orphan exits, missing items, etc.). |
| `validator.Validate()` | `validator.Validate()` | Returns `IEnumerable<string>` of issues. |
| `game.CreateValidator()` | `game.CreateValidator().Validate()` | Extension shorthand. |
| `GameExplorer(game)` | `new GameExplorer(game)` | Traverses all reachable locations. |
| `explorer.ExploreAll()` | `explorer.ExploreAll()` | Visit every room programmatically. |
| `game.CreateExplorer()` | `game.CreateExplorer().ExploreAll()` | Extension shorthand. |
| `MapGenerator.Render(state)` | `MapGenerator.Render(state)` | Render an ASCII map of visited locations. |
| `IDevLogger` | `builder.UseDevLogger(myLogger)` | Attach a developer/debug logger. |
| `IStoryLogger` | `builder.UseStoryLogger(myLogger)` | Attach a narrative event logger. |

---

## AI — Plugin & Setup

| Type / Member | Usage | Notes |
| --- | --- | --- |
| `new AiPlugin(module, options?)` | `new AiPlugin(featureModule)` | AI plugin; install via `GameBuilder.UsePlugin(…)`. |
| `plugin.ExtendDslParser(parser)` | `plugin.ExtendDslParser(dslParser)` | Register AI DSL keywords before parsing. |
| `plugin.Configure(builder)` | `plugin.Configure(builder)` | Wraps parser with AI (called automatically by `Build()`). |
| `plugin.OnGameBuilt(game)` | `plugin.OnGameBuilt(game)` | Injects AI into NPCs and rooms (called automatically after `Build()`). |
| `AiPluginBootstrap` | `new AiPluginBootstrapFactory().Create(opts)` | Bootstrap record returned by the factory. |
| `bootstrap.ApplyParserTo(builder)` | `bootstrap.ApplyParserTo(builder)` | Apply the AI parser to a `GameBuilder`. |
| `bootstrap.EnableRuntime(game)` | `bootstrap.EnableRuntime(game)` | Enable AI features at runtime on a built game. |
| `AiPluginOptions` | `new AiPluginOptions { EnableAiNpcMovement = true }` | Runtime flags for enabling AI features per category. |
| `AiPluginInitOptions` | `new AiPluginInitOptions { … }` | Initialisation options passed to the factory. |
| `AiFeatureModule` | `new AiFeatureModule(router, …)` | DI container for all AI feature services. |

---

## AI — Providers & Router

| Type / Member | Usage | Notes |
| --- | --- | --- |
| `IAiCommandProvider` | `IAiCommandProvider p = new OllamaCommandProvider(…)` | Single AI provider. |
| `provider.ParseAsync(request, ct)` | `await provider.ParseAsync(req)` | Send input to provider; returns `AiProviderResult`. |
| `IAiProviderRouter` | `IAiProviderRouter router = new AiProviderRouter(providers)` | Routes across multiple providers with fallback. |
| `router.RouteAsync(request, ct)` | `await router.RouteAsync(req)` | Try providers in order; returns `AiRoutingResult`. |
| `AiProviderRouter(providers, budget?, sink?)` | `new AiProviderRouter([ollama, openai])` | Create router with optional budget and telemetry. |
| `AiParseRequest` | `new AiParseRequest(context, input, tokens)` | Request model sent to providers. |
| `AiProviderResult.IsSuccess` | `result.IsSuccess` | Whether the provider produced a command. |
| `AiProviderResult.CommandText` | `result.CommandText` | The parsed command text. |
| `AiRoutingResult.IsSuccess` | `routing.IsSuccess` | Whether any provider succeeded. |
| `AiRoutingResult.Attempts` | `routing.Attempts` | All provider attempts with outcomes. |
| `AiAttemptOutcome` enum | `AiAttemptOutcome.Success \| Failed \| SkippedBudget \| Exception` | Outcome of a single provider attempt. |
| **Provider Classes** | | |
| `OllamaCommandProvider` | `new OllamaCommandProvider(settings)` | Local Ollama inference. |
| `OpenAiCommandProvider` | `new OpenAiCommandProvider(settings)` | OpenAI GPT. |
| `ClaudeCommandProvider` | `new ClaudeCommandProvider(settings)` | Anthropic Claude. |
| `GeminiCommandProvider` | `new GeminiCommandProvider(settings)` | Google Gemini. |
| `MistralCommandProvider` | `new MistralCommandProvider(settings)` | Mistral AI. |
| `OpenRouterCommandProvider` | `new OpenRouterCommandProvider(settings)` | OpenRouter multi-provider gateway. |
| `OneMinAiCommandProvider` | `new OneMinAiCommandProvider(settings)` | 1Min.AI. |
| `LmStudioCommandProvider` | `new LmStudioCommandProvider(settings)` | LM Studio local server. |
| `DockerAiCommandProvider` | `new DockerAiCommandProvider(settings)` | Docker-hosted AI endpoint. |
| `JsonHttpAiCommandProviderBase` | Subclass to add a custom HTTP/JSON provider. | Base class for custom HTTP providers. |

---

## AI — Features

| Type / Member | Usage | Notes |
| --- | --- | --- |
| `INpcDialogueAiService.GenerateReplyAsync(ctx, input, ct)` | `await module.NpcDialogue.GenerateReplyAsync(ctx, "Hello")` | Generate an NPC reply using AI. |
| `INpcMovementAiService.ChooseNextLocationAsync(ctx, ct)` | `await module.NpcMovement.ChooseNextLocationAsync(ctx)` | AI decides NPC's next room. |
| `INpcCombatAiService.DecideActionAsync(ctx, ct)` | `await module.NpcCombat.DecideActionAsync(ctx)` | AI chooses NPC combat action. |
| `IStoryDirectorAiService.ProposeEventAsync(ctx, ct)` | `await module.StoryDirector.ProposeEventAsync(ctx)` | AI proposes a dynamic story event. |
| `IRoomDescriptionAiService.GenerateDescriptionAsync(req, ct)` | `await module.RoomDescription.GenerateDescriptionAsync(req)` | AI generates a room description. |
| `IItemDescriptionAiService.GenerateDescriptionAsync(req, ct)` | `await module.ItemDescription.GenerateDescriptionAsync(req)` | AI generates an item description. |
| `AiContextBuilder` | `new AiContextBuilder().Build(state)` | Build a compact game-state context for AI prompts. |
| `AiNpcMovementStrategy` | Injected automatically by `AiPlugin` when `EnableAiNpcMovement = true`. | Replaces NPC movement with AI decisions. |
| `AiLookCommand` | Wraps `LookCommand`; generates AI room descriptions if enabled. | Automatically installed by plugin. |
| `AiTalkCommand` | Wraps `TalkCommand`; generates AI NPC responses if enabled. | Automatically installed by plugin. |

---

## AI — DSL Extension

| Type / Member | Usage | Notes |
| --- | --- | --- |
| `plugin.ExtendDslParser(parser)` | Called before `parser.ParseFile(…)` | Register AI-specific DSL keywords on the parser. |
| `locationAI: id \| hint` | `locationAI: cave \| The cave feels ancient and hostile.` | Attach an AI context hint to a room. |
| `npcAI: id \| movement` | `npcAI: guard \| movement` | Opt an NPC into AI-driven movement. |
| `npcAI: id \| dialogue` | `npcAI: luna \| dialogue` | Opt an NPC into AI-driven dialogue. |

---

## AI — Router Utilities

| Type / Member | Usage | Notes |
| --- | --- | --- |
| `IAiSpinner` | `IAiSpinner spinner = new AiConsoleSpinner()` | Abstraction for a spinner during AI calls. |
| `spinner.Begin()` | `using var _ = spinner.Begin()` | Start spinner; dispose scope to stop it. |
| `new AiConsoleSpinner(enabled?)` | `new AiConsoleSpinner()` | Console spinner; no-ops when output is redirected. |
| `new SpinnerAiProviderRouter(inner, spinner)` | `new SpinnerAiProviderRouter(router, spinner)` | Decorator that shows a spinner during `RouteAsync`. |

---

## AI — Diagnostics

| Type / Member | Usage | Notes |
| --- | --- | --- |
| `new AiDebugTracker(enabled?)` | `new AiDebugTracker()` | Per-turn AI/cache event recorder. |
| `tracker.StartTurn()` | `tracker.StartTurn()` | Clear events at the start of each turn. |
| `tracker.Record(event, payload)` | `tracker.Record("parser.ai.call", input)` | Record a named debug event. |
| `tracker.Toggle()` | `tracker.Toggle()` | Toggle verbose output; returns status message. |
| `tracker.Set(bool)` | `tracker.Set(true)` | Set verbose mode explicitly. |
| `tracker.Status()` | `tracker.Status()` | Return current mode as a readable string. |
| `tracker.BuildTurnLines()` | `tracker.BuildTurnLines()` | Return display lines for the turn (`[AI]`, `[Cache]`, detail lines). |
| `new AiDebugCommandParser(inner, tracker)` | `new AiDebugCommandParser(parser, tracker)` | Wraps parser to intercept `/debug [on\|off\|status]`. |
| `DebugToggleMode` enum | `DebugToggleMode.Toggle \| On \| Off \| Status` | Modes for the `/debug` command. |
| `DebugToggleModeParser.TryParse(input, out mode)` | `DebugToggleModeParser.TryParse(input, out var mode)` | Parse `/debug` input into a mode. |

---

## AI — Safety & Budget

| Type / Member | Usage | Notes |
| --- | --- | --- |
| `IAiCommandSafetyPolicy.Evaluate(text)` | `policy.Evaluate("go north")` | Check whether a parsed command is safe to execute. |
| `CommandAllowlistSafetyPolicy` | `new CommandAllowlistSafetyPolicy(allowedVerbs)` | Allow only explicitly listed command verbs. |
| `AllowAllAiCommandSafetyPolicy` | `new AllowAllAiCommandSafetyPolicy()` | No restrictions (development use). |
| `AiSafetyDecision.IsSafe` | `decision.IsSafe` | Whether the command passed the safety check. |
| `ITokenBudgetPolicy.CanUse(provider, tokens)` | `budget.CanUse("openai", 200)` | Check if estimated tokens fit within the budget. |
| `ITokenBudgetPolicy.TrackUsage(provider, tokens)` | `budget.TrackUsage("openai", 150)` | Record actual usage. |
| `InMemoryDailyTokenBudgetPolicy` | `new InMemoryDailyTokenBudgetPolicy(limits)` | Daily per-provider token budget. |
| `IAiTelemetrySink.Record(event)` | `sink.Record(telemetryEvent)` | Record a routing telemetry event. |
| `InMemoryAiTelemetrySink` | `new InMemoryAiTelemetrySink()` | Store telemetry in memory for inspection. |
| `AiTelemetryEvent` | `event.ProviderName \| event.Outcome \| event.TokensUsed` | Telemetry event produced by the router. |
