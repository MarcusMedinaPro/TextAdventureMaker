## Slice 78: NPC Rules, Triggers & Dialogue Options

**Mål:** Göra NPC-beteende och trigger-baserad dialog deklarativt i DSL.

**Referens:** `docs/dsl/adventure-dsl-v2-core-report.md` (4.9, 4.11)

### Task 78.1: `npc_dialog_option` tree support

Nytt keyword:
- `npc_dialog_option: <npc_id> | from=<node_id> | text=<option_text> | to=<node_id>`

Mål:
- bygga enkla dialoggrenar utan C#.

### Task 78.2: `npc_rule` parser

Nytt keyword:
- `npc_rule: <npc_id> | id=<rule_id> | if=<condition> | priority=<int> | say=<text> | then=<effects>`

Bind till `DialogRule`.

### Task 78.3: `npc_trigger` parser

Nytt keyword:
- `npc_trigger: <npc_id> | sense=see|hear | target=<token> | after=<ticks> | say=<text> | say_once=true|false | flee=true|false`

Bind till `NpcTrigger`.

### Task 78.4: Runtime integration

Koppla triggers till `NpcTriggerSystem` och ensure:
- delayed firing fungerar
- `say_once` respekteras
- `flee` flyttar bort npc från location

### Task 78.5: Validation

Validera:
- `sense` måste vara giltig enum.
- target npc och rum måste finnas.
- `after` >= 0.

### Task 78.6: Tests

Minimikrav:
- `npc_rule` med högre priority väljs först.
- `npc_trigger` på `see player` och `hear combat` fungerar.
- `say_once` och `flee` fungerar korrekt.

---

**Definition of Done**
- NPC rules och triggers kan definieras i DSL och exekveras av Core-systemen.

## Completion Checklist
- [x] Core implementation for this slice is present in the engine.
- [x] Behaviour is covered by tests and/or deterministic validation paths.
- [x] Demo document: `78_DSL_v2_NPC_Triggers.md`.
- [x] Marked complete in project slice status.
