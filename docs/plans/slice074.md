## Slice 74: Item Reactions, Consequences & Recipes

**Mål:** Deklarera item-beteenden i DSL i stället för C#-wiring.

**Referens:** `docs/dsl/adventure-dsl-v2-core-report.md` (4.3, 4.4, 4.5)

### Task 74.1: `item_reaction` keyword

Nytt keyword:
- `item_reaction: <item_id> | on=<action> | text=<message>`

Supported actions:
- `take`, `drop`, `use`, `destroy`, `read`, `read_failed`, `take_failed`, `drop_failed`, `use_failed`, `move`, `move_failed`

### Task 74.2: `item_consequence` keyword

Nytt keyword:
- `item_consequence: <item_id> | on=<action> | ...effects...`

Stöd:
- `destroy=true`
- `create=<item_id[,item_id...]>`
- `transform=<item_id>`
- `set_flag=<key>:<bool>`
- `inc_counter=<key>:<int>`
- `set_item_desc=<item_id>:<text>`
- `message=<text>`

### Task 74.3: Runtime integration för consequences

I command pipeline (take/drop/use/read/move):
- hämta consequence för `(item_id, action)`
- kör effects via gemensam effect executor
- respektera create/transform/destroy i inventory/location

### Task 74.4: `recipe` keyword + RecipeBook binding

Nytt keyword:
- `recipe: <id> | left=<item_id> | right=<item_id> | create=<item_id> | message=<text>`

Koppla till `RecipeBook.Add(new ItemCombinationRecipe(...))`.

### Task 74.5: Validation

Validera:
- `item_reaction` target item måste finnas.
- `item_consequence` action måste vara giltig enum.
- `recipe` left/right/create måste finnas.

### Task 74.6: Tests

Minimikrav:
- Reaction text används av kommandon.
- Consequence transform/destroy/create fungerar.
- `combine` använder recipe från DSL.

---

**Definition of Done**
- Item reactions och consequences kan beskrivas helt i DSL.
- Recipes/merge fungerar utan extra C#-registration.

## Completion Checklist
- [x] Core implementation for this slice is present in the engine.
- [x] Behaviour is covered by tests and/or deterministic validation paths.
- [x] Demo document: `74_DSL_v2_Item_Behaviours.md`.
- [x] Marked complete in project slice status.
