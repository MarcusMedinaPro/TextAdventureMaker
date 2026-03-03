## Slice 75: Variable Interpolation & Safe Expressions

**Mål:** Göra DSL-text dynamisk med `{...}` och stödja säkra uttryck för t.ex. prisregler.

**Referens:** `docs/dsl/adventure-dsl-v2-core-report.md` (4.16, 4.17)

### Task 75.1: Template token parser

Stöd:
- `{path}`
- `{path|formatter}`
- `{path|formatter|join=" and "}`
- `{path??fallback}`

Exempel:
```adventure
mirror.look: You are wearing {inventory.clothes.*|names|join=" and "??nothing special}.
```

### Task 75.2: Path resolver

Första scopes:
- `inventory.<item_id>`
- `inventory.<prefix>.*`
- `counter.<key>`
- `flag.<key>`
- `relationship.<npc_id>`
- `player.location`

### Task 75.3: Formatters

Implementera:
- `names`
- `ids`
- `count`
- `join="<sep>"`

### Task 75.4: Expression engine (safe subset)

För `price_expr` och liknande:
- tillåt endast whitelistade operatorer/funktioner (`+ - * /`, `min`, `max`, parenteser)
- inga funktionsanrop utanför whitelist
- inga reflection/dynamic eval

Exempel:
```adventure
price_expr: amulet | expr=max(1, counter.gold-1)
```

### Task 75.5: Validation rules

Validera:
- okända paths i `{...}` ger warning/error.
- expressions med otillåtna tokens stoppas.
- varna vid uppenbart omöjliga prisregler (`counter.gold+1`-mönster).

### Task 75.6: Tests

Minimikrav:
- wildcard inventory-lista renderas korrekt.
- fallback används när värde saknas.
- expression parser blockerar otillåtna uttryck.

---

**Definition of Done**
- Text kan innehålla dynamiska DSL-variabler.
- Runtime expressions är säkra och validerade.

## Completion Checklist
- [x] Core implementation for this slice is present in the engine.
- [x] Behaviour is covered by tests and/or deterministic validation paths.
- [x] Demo document: `75_DSL_v2_Variables_and_Expressions.md`.
- [x] Marked complete in project slice status.
