# Adventure DSL v2 (Core) - Change Report and Concrete Proposals

Date: 2026-03-01  
Scope: Core engine only (`src/MarcusMedina.TextAdventure`)  
Out of scope: AI generation/injection, sandbox examples

## 1. Executive Summary

Current `.adventure` DSL is intentionally minimal (world, goal, start, location, description, item, key, door, exit, timed spawn, timed door).  
Core already contains significantly richer systems that still require C# code.

This report proposes a v2 DSL that:
- Moves most gameplay rules into data.
- Keeps compatibility with current line-based syntax.
- Adds concrete keywords for item logic, door logic, NPCs, quests, triggers, schedules, and crafting.
- Adds variable interpolation and economy rules so dynamic text and shop flows can stay in DSL.
- Reduces Core code needed per adventure to wiring/engine only.

## 2. Current Gap (DSL vs Core)

## 2.1 Already in DSL v1
- World metadata: `world`, `goal`, `start`
- Basic map: `location`, `description`, `exit`
- Basic entities: `item`, `key`, `door`
- Time objects (limited): `timed_spawn`, `timed_door`

## 2.2 Core Capabilities Not Represented in DSL
- Rich item properties (stackable, readable rules, poison/heal, durability, hidden list, presence descriptions, custom properties)
- Item reactions and item consequences (destroy/transform/create/set flag/increment counter/custom effects)
- Recipes/merge (`RecipeBook`, `ItemCombinationRecipe`, `CombineCommand`)
- Door state, aliases, reactions per action/failure
- Hidden exits and discovery/perception rules
- NPC state/stats/movement/dialog rules/triggers/arcs/bonds
- Quest conditions (`AllOf`, `AnyOf`, world flags/counters, item checks, NPC state, relationship)
- Story branches/chapter model hooks
- Scheduled and random events
- World state bootstrap (flags/counters/relationships/timeline seed)

## 3. Design Goals for v2

- Data-first: adventure logic should be editable without C# changes.
- Backward compatible: existing v1 files should parse unchanged.
- Progressive complexity: simple lines for simple games, advanced lines for advanced rules.
- Deterministic: no hidden magic in parser; warnings/errors should remain clear.
- British English text remains content-authoring default.

## 4. Proposed DSL v2 Domains

## 4.1 Entity Definition and Placement

Problem today:
- `item`/`key` are location-scoped only.
- Reusing entities across rooms is awkward.

Proposal:
- Add top-level entity definitions and explicit placement directives.

Proposed keywords:
- `define item`
- `define key`
- `define door`
- `define npc`
- `place item`
- `place npc`

Example:
```adventure
define item: torch | name=old torch | desc=A dusty iron torch.
define item: coin | name=silver coin | desc=A worn coin from the old kingdom.
location: crypt_entrance | A cracked arch opens into darkness.
place item: crypt_entrance | torch
place item: crypt_entrance | coin
```

Core mapping:
- `Item`, `Key`, `Door`, `Npc`, `Location.AddItem`, `Location.AddNpc`

---

## 4.2 Rich Item Properties

Problem today:
- Parser only applies `weight`, `aliases`, `takeable`.

Proposal:
- Add full item options directly to DSL.

Proposed item options:
- `stackable=true|false`
- `amount=<int>`
- `presence_desc=<text>`
- `readable=true|false`
- `read_text=<text>`
- `require_take_to_read=true|false`
- `reading_cost=<int>`
- `hidden_from_list=true|false`
- `food=true|false`
- `drinkable=true|false`
- `poisoned=true|false`
- `heal=<int>`
- `poison_damage=<int>`
- `poison_turns=<int>`
- `durability=<current>/<max>`
- `prop.<key>=<value>`

Example:
```adventure
item: journal | old journal | A leather journal with brittle pages. | readable=true | read_text=The final page is torn out. | require_take_to_read=true | reading_cost=1
item: ration | food ration | Dry but edible. | stackable=true | amount=3 | food=true | heal=5
item: vial | cloudy vial | The liquid shifts unnaturally. | drinkable=true | poisoned=true | poison_damage=2 | poison_turns=4
item: lockpick | lockpick set | Thin steel tools in a roll. | durability=12/12 | prop.rarity=uncommon
```

Core mapping:
- `Item` + `IItem` members already present

---

## 4.3 Item Reactions by Action

Problem today:
- `SetReaction(ItemAction, text)` is code-only.

Proposal:
- Add reaction lines keyed by action.

Proposed keywords:
- `item_reaction`

Example:
```adventure
item_reaction: torch | on=take | text=Cold soot marks your fingers.
item_reaction: torch | on=use | text=You raise the torch and the corridor brightens.
item_reaction: journal | on=read_failed | text=You should take it first.
item_reaction: crate | on=move_failed | text=It is too heavy to shift alone.
```

Supported actions:
- `take`, `drop`, `use`, `destroy`, `read`, `read_failed`, `take_failed`, `drop_failed`, `use_failed`, `move`, `move_failed`

Core mapping:
- `ItemAction` enum + `Item.SetReaction`

---

## 4.4 Item Consequences (Rule Effects)

Problem today:
- Consequence system exists but is configured in C#.

Proposal:
- Add declarative consequence rules for item actions.

Proposed keywords:
- `item_consequence`

Suggested effect grammar:
- `destroy=true`
- `create=<item_id[,item_id...]>`
- `transform=<item_id>`
- `set_flag=<flag>:<bool>`
- `inc_counter=<counter>:<int>`
- `set_item_desc=<item_id>:<text>`
- `message=<text>`

Example:
```adventure
item_consequence: fragile_vase | on=drop | destroy=true | create=ceramic_shards | message=The vase shatters loudly.
item_consequence: sealed_letter | on=use | transform=open_letter | message=You break the wax seal.
item_consequence: shrine_switch | on=use | set_flag=secret_door_open:true | inc_counter=switch_pulls:1 | message=Stone grinds behind the wall.
item_consequence: burning_torch | on=use | set_item_desc=burning_torch:The torch burns lower than before.
```

Core mapping:
- `ActionConsequence`, `ItemConsequenceExtensions`, `WorldState`

---

## 4.5 Item Merge / Recipe System

Problem today:
- `RecipeBook` is runtime-capable but definitions live in code.

Proposal:
- Add DSL recipe keywords with input/output and optional message.

Proposed keywords:
- `recipe`

Example:
```adventure
recipe: torch_recipe | left=stick | right=cloth | create=unlit_torch | message=You bind cloth around the stick.
recipe: lockpick_recipe | left=wire | right=needle | create=lockpick | message=You bend and shape a crude lockpick.
recipe: oil_torch | left=unlit_torch | right=lamp_oil | create=lit_torch | message=The torch ignites with a steady flame.
```

Core mapping:
- `RecipeBook.Add`, `ItemCombinationRecipe`, `CombineCommand`

---

## 4.6 Door and Exit Expansion

Problem today:
- Doors in DSL support key only.
- Exits do not expose hidden/discovery/perception options.

Proposal:
- Expand door and exit options to match `Door` and `Exit`.

Proposed door options:
- `state=open|closed|locked|destroyed`
- `aliases=a,b,c`
- `reaction.<action>=<text>` for `open`, `close`, `lock`, `unlock`, `destroy`, `open_failed`, `unlock_failed`

Proposed exit options:
- Existing: `door=`, `oneway`
- New: `hidden=true`
- New: `discover_if=<condition>`
- New: `perception=<1-100>`

Example:
```adventure
door: crypt_gate | crypt gate | A heavy iron lattice. | key=crypt_key | state=locked | aliases=gate,iron gate | reaction.unlock=The lock yields with a metallic snap.
exit: north -> crypt_hall | door=crypt_gate
exit: east -> hidden_vault | hidden=true | discover_if=flag:wall_cracked=true | perception=35
```

Core mapping:
- `Door`, `DoorAction`, `DoorState`, `Exit.MarkHidden`, `Exit.WithPerceptionCheck`

---

## 4.7 Dynamic Room Descriptions

Problem today:
- Only static `description` is available in DSL.

Proposal:
- Add first-visit and condition-based room text rules.

Proposed keywords:
- `room_desc`
- `room_desc_when`
- `room_var`

Suggested condition format:
- `flag:<key>=<bool>`
- `counter:<key>>=<n>`
- `has_item:<item_id>`
- `npc_state:<npc_id>=<state>`
- `relationship:<npc_id>>=<n>`
- `time_phase:<dawn|day|dusk|night>`

Example:
```adventure
room_desc: crypt_hall | default=An arched chamber lined with empty statues.
room_desc: crypt_hall | first_visit=Your footsteps wake the dust of centuries.
room_desc_when: crypt_hall | if=flag:boss_awake=true | text=The hall trembles as something stirs below.
room_var: crypt_hall | key=city_name | value=Autumnbridge
```

Core mapping:
- `DynamicDescription`, `Location.GetDescription(state)`

---

## 4.8 Location Transformation Rules

Problem today:
- `Location.TransformsInto(...)` exists but has no `.adventure` representation.

Proposal:
- Add transformation directives.

Proposed keywords:
- `room_transform`

Example:
```adventure
room_transform: bridge_intact | target=bridge_collapsed | if=flag:explosives_triggered=true | irreversible=true
```

Core mapping:
- `LocationTransform`, `LocationTransformBuilder`

---

## 4.9 NPC Definition, Dialogue, and Triggers

Problem today:
- `npc` keyword is currently unknown in parser despite mature NPC model.

Proposal:
- Add first-class NPC DSL.

Proposed keywords:
- `npc`
- `npc_place`
- `npc_dialog`
- `npc_dialog_option`
- `npc_rule`
- `npc_trigger`

Suggested options:
- NPC base: `state`, `health`, `description`, `archetype`, `dies_at`
- Movement: `movement=none|random|patrol:<room_ids>`
- Rule dialog: `if=<condition>`, `priority=<int>`, `say=<text>`, `then=<effects>`
- Trigger: `sense=see|hear`, `target=<token>`, `after=<ticks>`, `say=`, `say_once=true`, `flee=true`

Example:
```adventure
npc: watchman | name=Town Watchman | state=friendly | health=30 | description=A tired but dutiful guard.
npc_place: square | watchman
npc_dialog: watchman | text=Keep your lantern lit and your voice low.
npc_rule: watchman | id=warn_night | if=time_phase:night | priority=50 | say=Doors bar early when the fog rolls in.
npc_trigger: watchman | sense=see | target=player | after=0 | say=Stay where I can see you.
npc_trigger: watchman | sense=hear | target=combat | after=1 | say_once=true | say=Enough! Break it up.
```

Core mapping:
- `Npc`, `DialogNode`, `DialogRule`, `NpcTrigger`, `NpcTriggerSystem`

---

## 4.10 Quest and Objective DSL

Problem today:
- Quest structures and condition graph live in code.

Proposal:
- Add quest definitions and reusable condition syntax.

Proposed keywords:
- `quest`
- `quest_condition`
- `quest_stage`
- `quest_objective`
- `quest_on_complete`
- `quest_on_fail`

Condition types:
- `has_item:<item_id>`
- `flag:<key>=<bool>`
- `counter:<key>>=<n>`
- `npc_state:<npc_id>=<state>`
- `relationship:<npc_id>>=<n>`
- combinators: `all(...)`, `any(...)`

Example:
```adventure
quest: dragon_hunt | title=Dragon Hunt | desc=Find the sword and defeat the dragon. | state=active
quest_condition: dragon_hunt | all(has_item:sword, npc_state:dragon=dead, flag:dragon_defeated=true, counter:villagers_saved>=1, relationship:fox>=2)
quest_stage: dragon_hunt | id=prepare | required=find_sword,reach_cave | optional=read_tome
quest_on_complete: dragon_hunt | effects=set_flag:chapter_one_complete:true;inc_counter:heroes:1
```

Core mapping:
- `Quest`, `QuestStage`, `QuestObjective`, `QuestConditionEvaluator`, condition model types

---

## 4.11 Generic Triggers and Effects

Problem today:
- Engine has events/action trigger hooks, but DSL cannot register them.

Proposal:
- Add general trigger declarations with reusable effect syntax.

Proposed keywords:
- `on_enter`
- `on_pickup`
- `on_talk`
- `on_action`
- `on_npc_death`
- `on_tick`

Effect list should reuse same grammar as item consequences:
- `message:<text>`
- `set_flag:<k>:<bool>`
- `inc_counter:<k>:<n>`
- `spawn_item:<item_id>:<location_id>`
- `spawn_npc:<npc_id>:<location_id>`
- `open_door:<door_id>`
- `move_npc:<npc_id>:<location_id>`

Example:
```adventure
on_enter: cave | if=flag:dragon_awake=false | effects=set_flag:dragon_awake:true;message:The dragon stirs and awakens.
on_pickup: apple | effects=inc_counter:villagers_saved:1
on_talk: fox | effects=inc_relationship:fox:1
on_npc_death: dragon | effects=set_flag:dragon_defeated:true
```

Core mapping:
- `EventSystem`, `ActionTriggerSystem`, `WorldState`, `Location`, `Door`

---

## 4.12 Scheduled and Random Events

Problem today:
- `ScheduleQueue` and `RandomEventPool` are code-configured.

Proposal:
- Add schedule/random DSL.

Proposed keywords:
- `schedule_at`
- `schedule_every`
- `schedule_when`
- `random_event`
- `random_settings`

Example:
```adventure
random_settings: enabled=true | chance=0.15
random_event: crows_cry | weight=3 | cooldown=5 | if=time_phase:dusk | effects=message:Crows burst from the trees.
schedule_at: 10 | effects=message=A bell tolls ten times.
schedule_every: 6 | effects=inc_counter:hours_passed:1
schedule_when: flag:alarm=true | effects=message=The alarm continues to ring.
```

Core mapping:
- `ScheduleQueue`, `RandomEventPool`, `TimeSystem`

---

## 4.13 World State Bootstrap

Problem today:
- Flags/counters/relationships are usually initialised in C#.

Proposal:
- Add explicit world-state seed lines.

Proposed keywords:
- `flag`
- `counter`
- `relationship`
- `timeline`

Example:
```adventure
flag: dragon_awake=false
counter: villagers_saved=0
relationship: fox=0
timeline: The autumn watch begins.
```

Core mapping:
- `WorldState`

---

## 4.14 Story Branches and Chapters (Optional v2.x)

Problem today:
- Story/chapter systems exist in Core, but chapter DSL parsers are stubs.

Proposal:
- Add optional chapter/branch blocks in later v2 phases.

Proposed keywords:
- `branch`
- `branch_when`
- `branch_then`
- `chapter`
- `chapter_objective`
- `chapter_next`
- `chapter_end`

Example:
```adventure
branch: trust_fox
branch_when: trust_fox | relationship:fox>=2
branch_then: trust_fox | effects=set_flag:fox_trusted:true

chapter: chapter_one | title=At the Forest Gate
chapter_objective: chapter_one | id=find_sword
chapter_next: chapter_one -> chapter_two
```

Core mapping:
- `StoryBranch`, `StoryState`, `Chapter`, `ChapterSystem`

---

## 4.15 Parser/Command Settings (Optional)

Problem today:
- Parser command aliases/synonyms/fuzzy settings are code-configured.

Proposal:
- Add optional parser config section to DSL.

Proposed keywords:
- `parser_option`
- `command_alias`
- `direction_alias`

Example:
```adventure
parser_option: fuzzy=true | max_distance=1
command_alias: examine=x
command_alias: inventory=i
direction_alias: n=north
```

Core mapping:
- `KeywordParserConfig` and builder

---

## 4.16 Variable References and Text Interpolation

Problem today:
- Text output cannot reference structured DSL data like inventory groups.

Proposal:
- Add variable interpolation with path access, wildcard support, formatters, and fallback.

Token syntax:
- `{path}`
- `{path|formatter}`
- `{path|formatter|join="<sep>"}`
- `{path??fallback}`

Recommended path examples:
- `inventory.<item_id>`
- `inventory.<prefix>.*`
- `counter.gold`
- `flag.dragon_awake`
- `relationship.fox`
- `player.location`

Recommended formatters:
- `names` (default for item collections)
- `ids`
- `count`
- `join="<sep>"`

Example:
```adventure
start_inventory: clothes.jeans
start_inventory: clothes.tshirt

item: clothes.jeans | jeans | Dark denim jeans.
item: clothes.tshirt | T-shirt | A plain white T-shirt.

mirror.look: You are wearing {inventory.clothes.*|names|join=" and "}.
mirror.look_empty: You are wearing {inventory.clothes.*|names|join=" and "??nothing special}.
```

Core mapping:
- `Inventory`, `WorldState` + a new text interpolation resolver used by command/room/item outputs.

---

## 4.17 Economy and Shop Buy Rules

Problem today:
- Cost checks and purchase consequences are coded manually.

Proposal:
- Add declarative economy fields and purchase rule blocks with `require`, `true`, `false` branches.

Proposed keywords:
- `cost`
- `store_stock`
- `buy_rule`
- `require`
- `true`
- `false`

Recommended condition operands:
- `counter:<key>>=<n>`
- `stock:<store_id>.<item_id>>=<n>`
- `flag:<key>=<bool>`

Recommended effect operands:
- `message=<text>`
- `counter_add:<key>:<delta>`
- `inventory_add:<item_id>`
- `inventory_remove:<item_id>`
- `stock_add:<store_id>.<item_id>:<delta>`
- `set_flag:<key>:<bool>`

Example:
```adventure
item: rope | rope | A sturdy rope. | cost=20
store_stock: general_store | rope | qty=3

buy_rule: general_store.rope
require: counter:gold>=20 && stock:general_store.rope>=1
true: message=You bought the rope.; counter_add:gold:-20; inventory_add:rope; stock_add:general_store.rope:-1
false: message=You cannot afford that.
```

Dot-notation variant (compact style):
```adventure
item.cost=20
item.buy.require=counter.gold>=20 && store.inventory.item>=1
item.buy.true=counter.gold-=20; inventory.add=item; store.inventory.item--;
item.buy.false=You cannot afford that.
```

Dynamic price examples:
```adventure
# Base price
item: amulet | bronze amulet | A cheap charm. | cost=20

# Safe dynamic price (always purchasable eventually)
price_expr: amulet | expr=max(1, counter.gold-1)

# Markup model
counter: shop_markup=3
price_expr: amulet | expr=item.cost+counter.shop_markup

# Intentionally impossible example (for testing only)
price_expr: cursed_deal | expr=counter.gold+1
```

Validation recommendation for price expressions:
- Detect and warn about expressions that are always above player currency (`price > gold` in all states).
- Clamp final price to a minimum (`>= 1`) unless explicitly allowed to be free.

Core mapping:
- `WorldState` (gold as counter), `Inventory`, shop stock state, command pipeline for `buy`.

---

## 4.18 NPC Acceptance and Relationship Threshold Rules

Problem today:
- Acceptance/romance/attitude ladders are typically hardcoded in dialogue logic.

Proposal:
- Add declarative acceptance rules per NPC against player/world counters.

Proposed keywords:
- `npc_acceptance`
- `npc_acceptance_default`

Recommended fields:
- `target=player`
- `if=<condition>`
- `level=<token>`
- `say=<text>`
- optional `effects=<...>` (reuse effect grammar)

Recommended condition sources:
- `counter.good`
- `counter.bad`
- `counter.nice_to_<npc>`
- `relationship.<npc_id>`
- `flag.<key>`

Ordered threshold example (first match wins):
```adventure
counter: good=0

npc: luna | name=Luna | description=A sharp-eyed alchemist.

npc_acceptance: luna | target=player | if=counter.good>95 | level=desperately_in_love | say=Marry me!!!
npc_acceptance: luna | target=player | if=counter.good>70 | level=in_love | say=I have fallen for you.
npc_acceptance: luna | target=player | if=counter.good>50 | level=yes | say=I trust you.
npc_acceptance: luna | target=player | if=counter.good>30 | level=no | say=Not yet.
npc_acceptance: luna | target=player | if=counter.good<20 | level=hate_you | say=Go f*ck yourself!
npc_acceptance_default: luna | target=player | level=reject | say=Leave me alone.
```

Compact dot-notation variant:
```adventure
npc.acceptance.luna.if=counter.good>95 -> level=desperately_in_love -> say=Marry me!!!
npc.acceptance.luna.if=counter.good>70 -> level=in_love -> say=I have fallen for you.
npc.acceptance.luna.if=counter.good>50 -> level=yes -> say=I trust you.
npc.acceptance.luna.if=counter.good>30 -> level=no -> say=Not yet.
npc.acceptance.luna.if=counter.good<20 -> level=hate_you -> say=Go f*ck yourself!
npc.acceptance.luna.default=level=reject -> say=Leave me alone.
```

NPC-specific counter example:
```adventure
counter: nice_to_luna=0
npc_acceptance: luna | target=player | if=counter.nice_to_luna>50 | level=friend | say=Hello my friend.
```

Practical notes:
- Evaluate rules top-to-bottom and stop at first match.
- Keep level tokens canonical (`reject`, `no`, `yes`, `in_love`, `desperately_in_love`, `hate_you`) for stable branching.
- Allow follow-up branching in dialogue via acceptance level checks.

Core mapping:
- `WorldState` counters/relationships + dialog rule selection (`DialogRule`, `Npc.GetRuleBasedDialog`).

## 5. DSL CRUD Support (for DSLHelper Tooling)

Not runtime parser behaviour; this is editor/tooling support to minimise manual file edits.

Recommended DSLHelper command set:
- `list rooms`
- `list items [room=<id>]`
- `list npcs [room=<id>]`
- `show <entity> <id>`
- `create <entity> ...`
- `update <entity> <id> <field>=<value>`
- `delete <entity> <id>`
- `move item <item_id> to <room_id>`
- `move npc <npc_id> to <room_id>`
- `validate`
- `export`

Rationale:
- These commands keep adventure authors in DSL-first flow.
- CRUD tooling plus validation reduces direct Core coding.

## 6. Priority Recommendation (What to Build First)

P0 (highest value, lowest risk):
- Rich item properties
- Item reactions
- Item consequences
- Recipe/merge rules
- Variable interpolation and path resolution
- Economy/buy rules backed by world counters
- NPC acceptance threshold rules
- NPC base + placement + simple dialog
- Door/exit expansion (state, aliases, reactions, hidden exits)
- World state bootstrap

P1:
- Quest DSL with conditions
- Generic event triggers
- Scheduled/random events
- Dynamic room descriptions

P2:
- Story branches and chapter DSL
- Parser command alias DSL
- More advanced NPC arcs/bonds as declarative structures

## 7. Backward Compatibility Strategy

- Keep all v1 keywords unchanged.
- Introduce new keywords only; do not overload meaning of v1 lines.
- Unknown keywords should still produce warnings, not hard failure, in non-strict mode.
- Add optional strict mode in validator to enforce v2 schema.

## 8. Validation Rules to Add with v2

Reference validation:
- Every `place item` and `place npc` target room must exist.
- Every exit target room must exist.
- Every `door=` reference must exist.
- Every recipe input/output item must exist.
- Every NPC/quest condition reference must exist.

Logic validation:
- `durability` must be `current <= max`.
- `poison_turns` and `poison_damage` must be non-negative.
- Detect impossible quest conditions (unknown states, malformed comparators).
- Variable paths in `{...}` must resolve to known scopes or have explicit fallback.
- Economy rules must prevent impossible transactions (for example negative stock after purchase).
- `npc_acceptance` rules should be checked for ordering overlap and unreachable branches.
- Detect unreachable rooms and dead-end starts (optional warning level).

## 9. Expected Impact

If P0+P1 are implemented, most adventure logic currently written in C# can be authored in DSL:
- Item behaviour and transformations
- Crafting/merging
- Door interaction messaging and logic
- NPC behaviour and dialogue rules
- Quest and world progression
- Time/random/event-driven changes

Result:
- Less Core glue code per game
- Faster iteration for story designers
- Better maintainability and easier content scaling

## 10. Concrete Next Step

Create a formal `adventure-dsl-v2-spec.md` from this report and implement in this order:
1. Parser extensions for items, reactions, consequences, recipes, NPC base, world state seed.
2. Runtime bindings for new trigger/effect grammar.
3. Validator upgrades and docs/examples for each new keyword family.
