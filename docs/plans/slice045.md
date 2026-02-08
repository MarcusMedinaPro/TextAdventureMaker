## Slice 45: Generic Fixes

### Task 45.1: Generic Command Aliases (phrase-based)

**Mål:** Ersätt hårdkodade input-checkar (t.ex. `IsSitInput`) med en generisk, fluent alias‑lösning.

**Idé:** Ett lättviktigt “phrase/alias”-system som kan mappa flera fraser till en åtgärd utan att skriva egna `if`‑kedjor.

**Krav:**
- Ska fungera med existerande parser utan att introducera externa beroenden.
- Ska kunna användas i sandbox‑exempel (ex: `"sit"`, `"sit down"`, `"sit on chair"`).
- Ska uppmuntra `pattern matching` och/eller fluent extensions (`input.Is("...")`).

### Task 45.2: Pronoun carry-over + Repeat last command

**Mål:** Förbättra input‑upplevelsen med kontextuell tolkning av “them/it” och “again”.

**Exempel:**
- `take jeans; wear them` → “them” syftar på senaste objektet (jeans).
- `take jeans and wear them` → samma sak i en rad.
- `unlock door and open it` → “it” syftar på senaste objektet (door).
- `again` → upprepar senaste kommandot (senaste fulla input).

**Krav:**
- Fungerar utan externa dependencies.
- Har tydliga felmeddelanden om “them/it” saknar kontext.
- Ska kunna användas i sandbox‑exempel (Slice 9 är ett bra testfall).

**Mål:** Samla upp generella förbättringar som dyker upp under verifiering.

### Task 45.3: Custom Commands via Language Files (Override + Extend)

**Mål:** Tillåt användare att definiera egna kommandon i språkfiler utan att skriva C#‑kod, och även kunna ersätta/överskugga inbyggda kommandon om de vill.

**Exempel:**
- `"pull rubber chicken"` (Monkey Island‑stil)
- `"shoot ant"` (spel från öknen)
- Egna verb som inte finns i core

**Krav:**
- Kommando‑ord och alias ska kunna definieras helt i språkkonfigurationen.
- Användare ska kunna **override** inbyggda kommandon (t.ex. byta ut standardverb).
- Parsern ska kunna ruta custom commands till en registrerad handler‑callback.
- Ska fungera i sandbox utan extra kod per kommando (data‑drivet).

### Förslag på funktioner

- `IItem.Amount` (nullable int) + `Item.SetAmount(int amount)`
- `IItem.DecreaseAmount(int amount = 1)` → bool (om det finns kvar)
- Optional: `Item.OnAmountEmpty` reaction/hook
- `Use()` minskar amount om den finns (och tar bort item när 0)
- `IItem.PresenceDescription` + `Item.SetPresenceDescription(string text)`
- Look visar `PresenceDescription` mellan rumsbeskrivning och items/exits
- KeywordParserConfigBuilder.WithWord("xyz") för att lägga till egna kommandon/ord med standard beteende
- `IItem.IsStackable` + `Item.SetStackable(bool isStackable)`
-

### Förslag på ännu mer coola funktion
- Inventory kombinerar stackable items med samma id till en rad med amount
- Combine command kan kombinera stackable items (t.ex. “combine all ice and fire”)
- Pour command kan hälla från stackable containers (t.ex. “pour all water into glass”)
- Pee command kan hälla från spelaren (t.ex. “pee all into glass”) - just kidding don't do this! :D
- Poo command can also be added for comedic effect (e.g., "poo all into the bushes") - just kidding again! :D
- Text generering för amount (t.ex. “You have 3 apples.” / “You have an apple.” / “You have no apples.”)
- Text colours för amount (t.ex. rött när låg mängd)
- Text warnings när mängd är låg (t.ex. “Only 1 left!”)
- Colours used for specific words or amounts in item descriptions (e.g., highlight "poison" in red if the item is poisonous)
  - Textoutput.Colorizer.HighlightWords(string text, Dictionary<string, ConsoleColor> wordColors)
  - We can add more functions to textoutput or we add the colours to an already existing class
- Phrases class, takes an array of phrases and returns them in random order or sequencially depending on the need
  - Phrases.GetRandomPhrase()
  - Phrases.GetNextPhrase() (start over when done)
- vehicles (cars, bikes, boats, planes, horse) with basic commands (enter, exit, drive, sail, fly) funkar som portaler
- teleporters (magical or technological) that can be used to instantly move between locations
- weather system that affects gameplay (rain makes surfaces slippery, fog reduces visibility)
- hostile npcs that can attack the player or other npcs
- basic combat system with health, damage, and simple tactics (attack, defend, flee)
- food --> healing system (eat food to regain health, drink water to stay hydrated)
  - Item.IsFood + Item.IsPoisoned + Item.HealAmount
  - Player.Eat(Item item) + Player.Drink(Item item)
  - Eating poisoned food causes damage over time
- basic crafting system (combine items to create new items)
- basic economy system (buy/sell items with currency)
  - Haggle
  - "kompispris" (if they like you more, they give you a better price)
  - Barter system (trade items instead of currency)
- crawl or sneak pass npcs without being detected
- Electrical Machines (lamps, computers, vehicles) that can be interacted with using specific commands (turn on/off, use, repair, smash)
- Fuel machines that require fuel items to operate (e.g., car needs gasoline)

### Krav

- Backwards compatible: items utan amount fungerar som tidigare.
- Inventory/Look visar amount när den finns (t.ex. “Tea Thermos (4)”).

---

## Implementation checklist (engine)
- [ ] Generic phrase/alias mapping for sandbox inputs
- [ ] Pronoun carry-over (`it`/`them`) + `again`
- [ ] Custom commands defined in language files (override/extend)
- [ ] Item amount/stacking system
- [ ] Presence description support
- [ ] Parser config: generic word registration (`WithWord`)
- [ ] Misc. QoL helpers listed above (phrases, vehicles, teleporters, weather, food/healing, economy, etc.)

## Example checklist (docs/examples)
- [ ] Sandbox demo for aliases + pronouns + custom commands
