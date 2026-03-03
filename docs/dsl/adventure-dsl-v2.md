# Adventure DSL v2 (Overview)

This is the short, practical overview for DSL v2.

Detailed proposal and rollout:
- `docs/dsl/adventure-dsl-v2-core-report.md`
- `docs/plans/dsl-v2-master-plan.md`

## Versioning

- v1 syntax remains supported.
- v2 files should declare:

```adventure
dsl_version: 2
```

## File Model (Recommended)

- `world.adventure` for static world/entities/rules.
- `state.start.adventure` for start inventory/stats/world counters.
- `save.slotX.adventure` for runtime snapshots.

## Core v2 Capability Areas

- Rich item schema (stackable/readable/poison/durability/properties)
- Item reactions and consequences
- Recipes/merge rules
- Door/exit extensions (state, aliases, hidden discovery)
- NPC definitions, acceptance ladders, rules, triggers
- Quest condition graphs
- Event/schedule/random automation
- Variable interpolation (`{inventory.*}`) and safe expressions
- Validation/diagnostics + DSLHelper CRUD/migration

## Minimal v2 Example

```adventure
dsl_version: 2
world: Autumn Gate
goal: Reach the observatory before dawn.

location: square | Rain gathers around the old fountain.
location: inn | A warm room with polished oak tables.
exit: north -> inn

item: rope | rope | A sturdy rope. | cost=20 | stackable=true | amount=1
counter: gold=25

buy_rule: market.rope
require: counter:gold>=20
true: message=You bought the rope.; counter_add:gold:-20; inventory_add:rope
false: message=You cannot afford that.

npc: luna | name=Luna | state=friendly | description=A keen-eyed alchemist.
npc_place: square | luna
npc_acceptance: luna | target=player | if=counter.nice_to_luna>50 | level=friend | say=Hello my friend.
npc_acceptance_default: luna | target=player | level=neutral | say=Good evening.

room_desc: square | first_visit=You feel watched from every window.
room_desc_when: square | if=counter:gold>=50 | text=You move with unusual confidence.
```

## Validation Expectations

- Unknown references should be flagged with line-based diagnostics.
- Strict mode should fail on schema errors.
- Ordered rules (for example acceptance ladders) should be checked for unreachable branches.

## Status

DSL v2 planning slices are covered in `slice073.md` to `slice093.md`.
