## Slice 77: NPC Base DSL + Acceptance Thresholds

**Mål:** Definiera NPC:er, placering och acceptance-nivåer i DSL.

**Referens:** `docs/dsl/adventure-dsl-v2-core-report.md` (4.9, 4.18)

### Task 77.1: `npc` och `npc_place`

Nya keywords:
- `npc: <id> | name=... | state=... | health=... | description=...`
- `npc_place: <room_id> | <npc_id>`

### Task 77.2: NPC base fields

Stöd:
- `state` (`NpcState`)
- `health`
- `archetype`
- `dies_at`
- `movement=none|random|patrol:<room_ids>`

### Task 77.3: Basic dialog root

Nytt keyword:
- `npc_dialog: <npc_id> | text=<line>`

Binder till `DialogNode` som root.

### Task 77.4: Acceptance rules

Nya keywords:
- `npc_acceptance`
- `npc_acceptance_default`

Exempel:
```adventure
npc_acceptance: luna | target=player | if=counter.good>95 | level=desperately_in_love | say=Marry me!!!
npc_acceptance: luna | target=player | if=counter.good<20 | level=hate_you | say=Go f*ck yourself!
npc_acceptance: luna | target=player | if=counter.nice_to_luna>50 | level=friend | say=Hello my friend.
npc_acceptance_default: luna | target=player | level=reject | say=Leave me alone.
```

### Task 77.5: Rule ordering + overlap checks

Krav:
- top-to-bottom evaluation
- first match wins
- validator varnar för skuggade/unreachable acceptance rules

### Task 77.6: Tests

Minimikrav:
- npc skapas och placeras från DSL.
- acceptance level ändras korrekt när counter ändras.
- default rule används när inga villkor matchar.

---

**Definition of Done**
- NPC-bas och acceptance-ladders kan beskrivas fullt ut i DSL.
