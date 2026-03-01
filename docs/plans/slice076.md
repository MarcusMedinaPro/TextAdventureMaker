## Slice 76: Door/Exit Expansion & Dynamic Rooms

**Mål:** Flytta dörr-/exit-logik och dynamiska rumsbeskrivningar in i DSL.

**Referens:** `docs/dsl/adventure-dsl-v2-core-report.md` (4.6, 4.7, 4.8)

### Task 76.1: Door options v2

Utöka `door:` med:
- `state=open|closed|locked|destroyed`
- `aliases=a,b,c`
- `reaction.<action>=<text>`

### Task 76.2: Exit options v2

Utöka `exit:` med:
- `hidden=true`
- `discover_if=<condition>`
- `perception=<1-100>`
- befintliga `door=` och `oneway` kvar

### Task 76.3: Dynamic room descriptions

Nya keywords:
- `room_desc: <room_id> | default=<text>`
- `room_desc: <room_id> | first_visit=<text>`
- `room_desc_when: <room_id> | if=<condition> | text=<text>`
- `room_var: <room_id> | key=<k> | value=<v>`

### Task 76.4: Room transform rules

Nytt keyword:
- `room_transform: <room_id> | target=<room_id> | if=<condition> | irreversible=true|false`

Bind till `LocationTransform`.

### Task 76.5: Validation

Validera:
- target rooms finns för exits och transforms.
- `perception` inom 1..100.
- door reactions använder giltiga `DoorAction`.

### Task 76.6: Tests

Minimikrav:
- hidden exits blir synliga när condition matchar.
- door state/reactions sätts från DSL.
- first-visit/conditional room text fungerar.
- transform triggas enligt condition.

---

**Definition of Done**
- Dörrar/exits och dynamiska rumsregler kan beskrivas deklarativt i DSL.
