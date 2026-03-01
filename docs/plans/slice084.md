## Slice 84: Parser & Command Settings via DSL

**Mål:** Låta DSL v2 styra command aliases, direction aliases och parser options.

**Referens:** `docs/dsl/adventure-dsl-v2-core-report.md` (4.15)

### Task 84.1: Parser configuration keywords

Nya keywords:
- `parser_option`
- `command_alias`
- `direction_alias`

Exempel:
```adventure
parser_option: fuzzy=true | max_distance=1
command_alias: examine=x
command_alias: inventory=i
direction_alias: n=north
```

### Task 84.2: Builder integration

Map DSL-data till `KeywordParserConfigBuilder`:
- alias collections
- fuzzy settings
- optional overrides per command family

### Task 84.3: Safety and fallback

Regler:
- om DSL parser config saknas: behåll `BritishDefaults()`
- invalid alias mappings ska ge diagnostics, inte krascha parsern

### Task 84.4: Validation

Validera:
- alias target command finns
- direction alias mappar till giltig `Direction`
- duplicate alias conflict rapporteras

### Task 84.5: Tests

Minimikrav:
- DSL aliases styr faktisk command parsing.
- fuzzy setting från DSL påverkar parsern.
- conflict warnings genereras deterministiskt.

---

**Definition of Done**
- Command-parser beteende kan justeras via DSL utan kodändringar.
