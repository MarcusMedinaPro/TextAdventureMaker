## Slice 43: Map Generator

**Mål:** Skapa en enkel map generator som kan rendera en ASCII-karta baserat på location-grafen och exits.

### Förslag på funktioner

- `MapGenerator.Render(GameState state)` → `string`
- Valfritt: `MapGenerator.Render(ILocation start, int maxDepth)` → `string`

### Krav

- Fungerar med Slice 1 (Location + Exits) utan extra beroenden.
- Möjlig att använda i sandbox för att visa en karta vid `look`.

---
