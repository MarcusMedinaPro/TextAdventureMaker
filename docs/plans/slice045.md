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


### Krav

- Backwards compatible: items utan amount fungerar som tidigare.
- Inventory/Look visar amount när den finns (t.ex. “Tea Thermos (4)”).
